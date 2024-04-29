using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Nethereum.ABI;
using Nethereum.ABI.EIP712;
using Nethereum.Contracts;
using Nethereum.Contracts.ContractHandlers;
using Nethereum.Contracts.Standards.ERC20;
using Nethereum.Contracts.Standards.ERC721;
using Nethereum.Contracts.Standards.ERC721.ContractDefinition;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.Hex.HexTypes;
using Nethereum.Model;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Signer;
using Nethereum.Signer.EIP712;
using Nethereum.Util;
using Nethereum.Web3;

using Newtonsoft.Json;

using RallyProtocol.Contracts;
using RallyProtocol.GSN.Contracts;

using UnityEngine;
using UnityEngine.Networking;

using Account = Nethereum.Web3.Accounts.Account;

namespace RallyProtocol.GSN
{

    public class GsnTransactionHelper
    {

        public const string GsnDomainSeparatorVersion = "3";

        public static async Task<string> HandleGsnResponse(UnityWebRequest request, Web3 provider)
        {
            if (request.result != UnityWebRequest.Result.Success)
            {
                throw new System.Exception($"Unable to perform web request, error: {request.error}\nRequest URL: {request.url}\nResponse Code: {request.responseCode}\nResponse Text: {request.downloadHandler.text}\nRequest Text: {Encoding.UTF8.GetString(request.uploadHandler.data)}");
            }

            GsnResponse response = JsonConvert.DeserializeObject<GsnResponse>(request.downloadHandler.text);
            if (!string.IsNullOrEmpty(response.Error))
            {
                Debug.LogError($"Relay failed, error: {request.error}\nRequest URL: {request.url}\nResponse Code: {request.responseCode}\nResponse Text: {request.downloadHandler.text}\nRequest Text: {Encoding.UTF8.GetString(request.uploadHandler.data)}");
                throw new RelayException(new System.Exception(response.Error));
            }
            else
            {
                string txHash = Sha3Keccack.Current.CalculateHash(response.SignedTx);
                await provider.Eth.TransactionManager.TransactionReceiptService.PollForReceiptAsync(txHash);
                //await provider.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(txHash);
                return txHash;
            }
        }

        public static string GetRelayRequestId(GsnRelayRequest gsnRelayRequest, string signature)
        {
            RelayRequest relayRequest = CreateAbiRelayRequest(gsnRelayRequest);
            ABIValue[] values = new ABIValue[] {
                new("address", relayRequest.Request.From),
                new("uint256", relayRequest.Request.Nonce),
                new("bytes", signature.HexToByteArray()),
            };
            ABIEncode abiEncode = new();
            string hash = abiEncode.GetSha3ABIEncoded(values).ToHex(false);
            string rawRelayRequestId = hash.PadLeft(64, '0');
            int prefixSize = 8;
            Regex regex = new("^.{" + prefixSize + "}");
            string prefixedRelayRequestId = regex.Replace(rawRelayRequestId, "0".PadLeft(prefixSize, '0'));
            return $"0x{prefixedRelayRequestId}";
        }

        public static Task<string> SignRequest(GsnRelayRequest relayRequest, string domainSeparatorName, string chainId, Account account)
        {
            Eip712TypedDataSigner signer = new();
            TypedData<Domain> typedData = new();
            GsnRequestMessage message = CreateGsnRequestMessage(relayRequest);
            typedData.PrimaryType = "RelayRequest";
            typedData.Types = new Dictionary<string, MemberDescription[]>()
            {
                ["EIP712Domain"] = new MemberDescription[]
                    {
                        new() {Name = "name", Type = "string"},
                        new() {Name = "version", Type = "string"},
                        new() {Name = "chainId", Type = "uint256"},
                        new() {Name = "verifyingContract", Type = "address"},
                    },
                ["RelayRequest"] = TypedGsnRequestData.RelayRequestType2.ToArray(),
                ["RelayData"] = TypedGsnRequestData.RelayDataType2.ToArray(),
            };
            typedData.SetMessage(message);
            Domain domain = new()
            {
                Name = domainSeparatorName,
                Version = GsnDomainSeparatorVersion,
                ChainId = BigInteger.Parse(chainId),
                VerifyingContract = relayRequest.RelayData.Forwarder
            };
            typedData.Domain = domain;
            string signature = signer.SignTypedDataV4(message, typedData, new EthECKey(account.PrivateKey));
            return Task.FromResult(signature);
        }

        public static GsnRequestMessage CreateGsnRequestMessage(GsnRelayRequest gsnRelayRequest)
        {
            RelayRequest relayRequest = CreateAbiRelayRequest(gsnRelayRequest);
            return new(relayRequest.Request, relayRequest.RelayData);
        }

        public static (int ZeroBytes, int NonZeroBytes) CalculateCallDataBytesZeroNonZero(string callData)
        {
            byte[] callDataBuf = callData.HexToByteArray();
            int callDataZeroBytes = 0;
            int callDataNonZeroBytes = 0;
            for (int i = 0; i < callDataBuf.Length; i++)
            {
                byte ch = callDataBuf[i];
                if (ch == 0)
                {
                    callDataZeroBytes++;
                }
                else
                {
                    callDataNonZeroBytes++;
                }
            }

            return (ZeroBytes: callDataZeroBytes, NonZeroBytes: callDataNonZeroBytes);
        }

        public static int CalculateCallDataCost(string msgData, int gtxDataNonZero, int gtxDataZero)
        {
            var callDataBytes = CalculateCallDataBytesZeroNonZero(msgData);
            return callDataBytes.ZeroBytes * gtxDataZero + callDataBytes.NonZeroBytes * gtxDataNonZero;
        }

        public static string EstimateGasWithoutCallData(GsnTransactionDetails transaction, int gtxDataNonZero, int gtxDataZero)
        {
            string originalGas = transaction.Gas;
            int callDataCost = CalculateCallDataCost(transaction.Data, gtxDataNonZero, gtxDataZero);
            HexBigInteger adjustedGas = new(originalGas);
            adjustedGas.Value -= callDataCost;
            return adjustedGas.HexValue;
        }

        public static async Task<HexBigInteger> EstimateCallDataCostForRequest(GsnRelayRequest relayRequestOriginal, RallyGSNConfig config, Web3 provider)
        {
            const int MaxSignatureLength = 65;

            // protecting the original object from temporary modifications done here
            GsnRelayRequest relayRequest = relayRequestOriginal.Clone();
            relayRequest.RelayData.TransactionCalldataGasUsed = "0xffffffffff";
            relayRequest.RelayData.PaymasterData = "0x" + "ff".PadLeft(config.MaxPaymasterDataLength * 2, 'f');

            BigInteger maxAcceptanceBudget = new HexBigInteger("0xffffffffff");
            string signature = "0x" + "ff".PadLeft(MaxSignatureLength * 2, 'f');
            string approvalData = "0x" + "ff".PadLeft(config.MaxApprovalDataLength * 2, 'f');

            ContractHandler handler = provider.Eth.GetContractHandler(config.RelayHubAddress);
            Function<RelayCallFunction> relayCallFunction = handler.GetFunction<RelayCallFunction>();
            RelayCallFunction functionInput = new()
            {
                DomainSeparatorName = config.DomainSeparatorName,
                MaxAcceptanceBudget = maxAcceptanceBudget,
                RelayRequest = CreateAbiRelayRequest(relayRequest),
                Signature = signature.HexToByteArray(),
                ApprovalData = approvalData.HexToByteArray()
            };

            IContractTransactionHandler<RelayCallFunction> transactionHandler = provider.Eth.GetContractTransactionHandler<RelayCallFunction>();
            string data = relayCallFunction.GetData(functionInput);
            // TODO: Check if this is the correct method of "populateTransaction"
            //TransactionInput transaction = await transactionHandler.CreateTransactionInputEstimatingGasAsync(config.RelayHubAddress, functionInput);
            //TransactionInput transaction = relayCallFunction.CreateTransactionInput(functionInput, config.RelayHubAddress);
            if (string.IsNullOrEmpty(data))
            {
                throw new System.Exception("Transaction data is empty");
            }

            return new HexBigInteger(CalculateCallDataCost(data, config.GtxDataNonZero, config.GtxDataZero));
        }

        public static RelayRequest CreateAbiRelayRequest(GsnRelayRequest gsnRelayRequest)
        {
            return new()
            {
                RelayData = CreateAbiRelayData(gsnRelayRequest.RelayData),
                Request = CreateAbiForwardRequest(gsnRelayRequest.Request)
            };
        }

        public static RelayData CreateAbiRelayData(GsnRelayData gsnRelayData)
        {
            BigInteger transactionCalldataGasUsed;
            if (gsnRelayData.TransactionCalldataGasUsed.IsHex())
            {
                transactionCalldataGasUsed = new HexBigInteger(gsnRelayData.TransactionCalldataGasUsed);
            }
            else
            {
                transactionCalldataGasUsed = BigInteger.Parse(gsnRelayData.TransactionCalldataGasUsed);
            }

            return new()
            {
                ClientId = BigInteger.Parse(gsnRelayData.ClientId),
                Forwarder = gsnRelayData.Forwarder,
                MaxFeePerGas = BigInteger.Parse(gsnRelayData.MaxFeePerGas),
                MaxPriorityFeePerGas = BigInteger.Parse(gsnRelayData.MaxPriorityFeePerGas),
                Paymaster = gsnRelayData.Paymaster,
                PaymasterData = gsnRelayData.PaymasterData.HexToByteArray(),
                RelayWorker = gsnRelayData.RelayWorker,
                TransactionCalldataGasUsed = transactionCalldataGasUsed
            };
        }

        public static ForwardRequest CreateAbiForwardRequest(GsnForwardRequest gsnForwardRequest)
        {
            return new()
            {
                Data = gsnForwardRequest.Data.HexToByteArray(),
                From = gsnForwardRequest.From,
                To = gsnForwardRequest.To,
                Gas = BigInteger.Parse(gsnForwardRequest.Gas),
                Nonce = BigInteger.Parse(gsnForwardRequest.Nonce),
                ValidUntilTime = BigInteger.Parse(gsnForwardRequest.ValidUntilTime),
                Value = BigInteger.Parse(gsnForwardRequest.Value)
            };
        }

        public static Task<BigInteger> GetSenderNonce(string sender, string forwarderAddress, Web3 provider)
        {
            ContractHandler handler = provider.Eth.GetContractHandler(forwarderAddress);
            Function<GetNonceFunction> nonceFunction = handler.GetFunction<GetNonceFunction>();
            GetNonceFunction functionInput = new() { From = sender };
            return nonceFunction.CallAsync<BigInteger>(functionInput);
        }

        public static async Task<BigInteger> GetSenderContractNonce(ERC20ContractService contract, string address)
        {

            // 0x2d0335ab is the function selector for the 'getNonce' function
            // 0x7ecebe00 is the function selector for the 'nonces' function
            string getNonceSelectr = "2d0335ab";
            string noncesSelector = "7ecebe00";

            string bytecode = await contract.ContractHandler.EthApiContractService.GetCode.SendRequestAsync(address);
            if (string.IsNullOrEmpty(bytecode) || bytecode.Length <= 2)
            {
                throw new RallyException("Could not get nonce: No bytecode found for token");
            }

            // check if the bytecode includes the function selector for the relative functions
            // there is still an issue with fallback functions, and data that may include the function selector
            if (bytecode.Contains(getNonceSelectr))
            {
                ERC20GetNonceFunction getNonceFunction = new();
                getNonceFunction.User = address;
                return await contract.ContractHandler.QueryAsync<ERC20GetNonceFunction, BigInteger>(getNonceFunction);
            }
            else if (bytecode.Contains(noncesSelector))
            {
                NoncesFunction noncesFunction = new();
                noncesFunction.Owner = address;
                return await contract.ContractHandler.QueryAsync<NoncesFunction, BigInteger>(noncesFunction);
            }
            else
            {
                throw new RallyException("Could not get nonce: No nonce function found for token");
            }
        }

    }

}