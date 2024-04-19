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
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Signer;
using Nethereum.Signer.EIP712;
using Nethereum.Util;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;

using Newtonsoft.Json;

using RallyProtocol.Contracts;
using RallyProtocol.GSN.Contracts;

using UnityEngine;
using UnityEngine.Networking;

namespace RallyProtocol.GSN
{

    public class GsnTransactionHelper
    {

        public const string RelayError = "Unable to perform action, transaction relay error";
        public const string GsnDomainSeparatorVersion = "3";

        public static async Task<string> HandleGsnResponse(UnityWebRequest request, Web3 provider)
        {
            if (request.result != UnityWebRequest.Result.Success)
            {
                throw new System.Exception($"Unable to perform web request, error: {request.error}\nresponse code: {request.responseCode}");
            }

            GsnResponse response = JsonConvert.DeserializeObject<GsnResponse>(request.downloadHandler.text);
            if (!string.IsNullOrEmpty(response.Error))
            {
                throw new System.Exception(RelayError, new System.Exception(response.Error));
            }
            else
            {
                string txHash = Sha3Keccack.Current.CalculateHash(response.SignedTx);
                await provider.Eth.TransactionManager.TransactionReceiptService.PollForReceiptAsync(txHash);
                //await provider.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(txHash);
                return txHash;
            }
        }

        public static string GetRelayRequestId(RelayRequest relayRequest, string signature)
        {
            ABIValue[] values = new ABIValue[] {
                new("address", relayRequest.Request.From),
                new("uint256", relayRequest.Request.Nonce),
                new("bytes", signature),
            };
            ABIEncode abiEncode = new();
            string hash = abiEncode.GetSha3ABIEncoded(values).ToHex(false);
            string rawRelayRequestId = hash.PadLeft(64, '0');
            int prefixSize = 8;
            Regex regex = new($"^.{{{prefixSize}}}");
            string prefixedRelayRequestId = regex.Replace(rawRelayRequestId, "0".PadLeft(6));
            return $"0x{prefixedRelayRequestId}";
        }

        public static Task<string> SignRequest(RelayRequest relayRequest, string domainSeparatorName, string chainId, Account account)
        {
            Eip712TypedDataSigner signer = new();
            TypedData<Domain> typedData = new();
            typedData.Domain = new()
            {
                Name = domainSeparatorName,
                Version = GsnDomainSeparatorVersion,
                ChainId = int.Parse(chainId),
                VerifyingContract = relayRequest.RelayData.Forwarder
            };
            typedData.PrimaryType = "RelayRequest";
            typedData.Types = new Dictionary<string, MemberDescription[]>()
            {
                { "RelayRequest", TypedGsnRequestData.RelayRequestType2.ToArray() },
                { "RelayData", TypedGsnRequestData.RelayDataType2.ToArray() }
            };
            typedData.SetMessage<GsnRequestMessage>(new(relayRequest.Request, relayRequest.RelayData));
            return Task.FromResult(signer.SignTypedData(typedData, new EthECKey(account.PrivateKey)));
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

        public static async Task<string> EstimateCallDataCostForRequest(RelayRequest relayRequestOriginal, RallyGSNConfig config, Web3 provider)
        {
            const int MaxSignatureLength = 65;

            // protecting the original object from temporary modifications done here
            RelayRequest relayRequest = relayRequestOriginal.Clone();
            relayRequest.RelayData.TransactionCalldataGasUsed = new HexBigInteger("0xffffffffff");
            StringBuilder paymasterData = new("0xff");
            for (int i = 0; i < config.MaxPaymasterDataLength; i++)
            {
                paymasterData.Append("ff");
            }

            relayRequest.RelayData.PaymasterData = new HexBigInteger(paymasterData.ToString());
            BigInteger maxAcceptanceBudget = new HexBigInteger("0xffffffffff");
            StringBuilder signature = new("0xff");
            for (int i = 0; i < MaxSignatureLength; i++)
            {
                signature.Append("ff");
            }

            StringBuilder approvalData = new("0xff");
            for (int i = 0; i < config.MaxApprovalDataLength; i++)
            {
                approvalData.Append("ff");
            }
            ContractHandler handler = provider.Eth.GetContractHandler(config.RelayHubAddress);
            Function<RelayCallFunction> relayCallFunction = handler.GetFunction<RelayCallFunction>();
            RelayCallFunction functionInput = new()
            {
                DomainSeparatorName = config.DomainSeparatorName,
                MaxAcceptanceBudget = maxAcceptanceBudget,
                RelayRequest = relayRequest,
                Signature = signature.ToString().HexToByteArray(),
                ApprovalData = approvalData.ToString().HexToByteArray()
            };

            IContractTransactionHandler<RelayCallFunction> transactionHandler = provider.Eth.GetContractTransactionHandler<RelayCallFunction>();
            // TODO: Check if this is the correct method of "populateTransaction"
            TransactionInput transaction = await transactionHandler.CreateTransactionInputEstimatingGasAsync(config.RelayHubAddress, functionInput);
            //TransactionInput transaction = relayCallFunction.CreateTransactionInput(functionInput, config.RelayHubAddress);
            if (string.IsNullOrEmpty(transaction.Data))
            {
                throw new System.Exception("Transaction data is empty");
            }

            return new HexBigInteger(CalculateCallDataCost(transaction.Data, config.GtxDataNonZero, config.GtxDataZero)).HexValue;
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