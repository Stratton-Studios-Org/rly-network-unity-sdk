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
using RallyProtocol.GSN.Models;
using RallyProtocol.Logging;
using RallyProtocol.Networks;

using Account = Nethereum.Web3.Accounts.Account;

namespace RallyProtocol.GSN
{

    public class GsnTransactionHelper
    {

        public const string GsnDomainSeparatorVersion = "3";

        protected IRallyLogger logger;

        public IRallyLogger Logger => this.logger;

        public GsnTransactionHelper(IRallyLogger logger)
        {
            this.logger = logger;
        }

        public async Task<string> HandleGsnResponse(RallyHttpResponse httpResponse, Web3 provider)
        {
            if (!httpResponse.IsCompletedSuccessfully)
            {
                throw new System.Exception($"Unable to perform web request, error: {httpResponse.ErrorMessage}\nRequest URL: {httpResponse.Url}\nResponse Code: {httpResponse.ResponseCode}\nResponse Text: {httpResponse.ResponseText}\nRequest Text: {httpResponse.RequestText}");
            }

            GsnResponse response = JsonConvert.DeserializeObject<GsnResponse>(httpResponse.ResponseText);
            if (!string.IsNullOrEmpty(response.Error))
            {
                Logger.LogError($"Relay failed, error: {httpResponse.ErrorMessage}\nRequest URL: {httpResponse.Url}\nResponse Code: {httpResponse.ResponseCode}\nResponse Text: {httpResponse.ResponseText}\nRequest Text: {httpResponse.RequestText}");
                throw new RelayException(new System.Exception(response.Error));
            }
            else
            {
                var txHash = $"0x{Sha3Keccack.Current.CalculateHashFromHex(response.SignedTx)}";
                TransactionReceipt receipt;
                do
                {
                    receipt = await provider.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(txHash);
                    await Task.Delay(2000);
                }
                while (receipt == null);

                //string txHash = Sha3Keccack.Current.CalculateHash(response.SignedTx);
                //await provider.Eth.TransactionManager.TransactionReceiptService.PollForReceiptAsync(txHash);
                //await provider.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(txHash);
                return txHash;
            }
        }

        public string GetRelayRequestId(GsnRelayRequest gsnRelayRequest, string signature)
        {
            RelayRequest relayRequest = CreateAbiRelayRequest(gsnRelayRequest);
            ABIValue[] values = new ABIValue[] {
                new("address", relayRequest.Request.From),
                new("uint256", relayRequest.Request.Nonce),
                new("bytes", signature.HexToByteArray()),
            };

            string hash = new ABIEncode().GetSha3ABIEncoded(values).ToHex(false);

            string rawRelayRequestId = hash.PadLeft(64, '0');
            const int prefixSize = 8;
            string prefixedRelayRequestId = new string('0', prefixSize) + rawRelayRequestId[prefixSize..];

            return $"0x{prefixedRelayRequestId}";
        }

        public Task<string> SignRequest(GsnRelayRequest relayRequest, string domainSeparatorName, string chainId, Account account)
        {
            Eip712TypedDataSigner signer = new();
            TypedData<DomainWithChainIdString> typedData = new();
            GsnRequestMessage message = CreateGsnRequestMessage(relayRequest);
            typedData.PrimaryType = TypedGsnRequestData.PrimaryType;
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

            MemberValue[] messageData = new MemberValue[] {
                new() { TypeName = "address", Value = relayRequest.Request.From.ToLowerInvariant() },
                new() { TypeName = "address", Value = relayRequest.Request.To.ToLowerInvariant() },
                new() { TypeName = "uint256", Value = relayRequest.Request.Value },
                new() { TypeName = "uint256", Value = relayRequest.Request.Gas },
                new() { TypeName = "uint256", Value = relayRequest.Request.Nonce },
                new() { TypeName = "bytes", Value = relayRequest.Request.Data },
                new() { TypeName = "uint256", Value = relayRequest.Request.ValidUntilTime },
                new() { TypeName = "RelayData", Value = relayRequest.RelayData.ToEip712Values() }
            };

            typedData.Message = messageData;
            //typedData.SetMessage(message);
            DomainWithChainIdString domain = new()
            {
                Name = domainSeparatorName,
                Version = TypedGsnRequestData.Version,
                ChainId = chainId,
                VerifyingContract = relayRequest.RelayData.Forwarder
            };
            typedData.Domain = domain;
            string signature = signer.SignTypedDataV4(typedData, new EthECKey(account.PrivateKey));

            var hashedData = Sha3Keccack.Current.CalculateHash(Eip712TypedDataSigner.Current.EncodeTypedData(typedData));
            string newSig = EthECDSASignature.CreateStringSignature(new EthECKey(account.PrivateKey).SignAndCalculateV(hashedData));
            return Task.FromResult(newSig);
        }

        public GsnRequestMessage CreateGsnRequestMessage(GsnRelayRequest gsnRelayRequest)
        {
            RelayRequest relayRequest = CreateAbiRelayRequest(gsnRelayRequest);
            return new(relayRequest.Request, relayRequest.RelayData);
        }

        public (int ZeroBytes, int NonZeroBytes) CalculateCallDataBytesZeroNonZero(string callData)
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

        public int CalculateCallDataCost(string msgData, int gtxDataNonZero, int gtxDataZero)
        {
            var callDataBytes = CalculateCallDataBytesZeroNonZero(msgData);
            return callDataBytes.ZeroBytes * gtxDataZero + callDataBytes.NonZeroBytes * gtxDataNonZero;
        }

        public string EstimateGasWithoutCallData(GsnTransactionDetails transaction, int gtxDataNonZero, int gtxDataZero)
        {
            string originalGas = transaction.Gas;
            int callDataCost = CalculateCallDataCost(transaction.Data, gtxDataNonZero, gtxDataZero);
            HexBigInteger adjustedGas = new(originalGas);
            adjustedGas.Value -= callDataCost;
            return adjustedGas.HexValue;
        }

        public Task<HexBigInteger> EstimateCallDataCostForRequest(GsnRelayRequest relayRequestOriginal, RallyGSNConfig config, Web3 provider)
        {
            const int MaxSignatureLength = 65;

            // protecting the original object from temporary modifications done here
            GsnRelayRequest relayRequest = relayRequestOriginal.Clone();
            relayRequest.RelayData.TransactionCalldataGasUsed = "0xffffffffff";
            relayRequest.RelayData.PaymasterData = "0x" + "ff".PadLeft(config.MaxPaymasterDataLength * 2, 'f');

            BigInteger maxAcceptanceBudget = new HexBigInteger("0xffffffffff");
            string signature = "0x" + "ff".PadLeft(MaxSignatureLength * 2, 'f');
            string approvalData = "0x" + "ff".PadLeft(config.MaxApprovalDataLength * 2, 'f');
            RelayCallFunction functionInput = new()
            {
                DomainSeparatorName = config.DomainSeparatorName,
                MaxAcceptanceBudget = maxAcceptanceBudget,
                RelayRequest = CreateAbiRelayRequest(relayRequest),
                Signature = signature.HexToByteArray(),
                ApprovalData = approvalData.HexToByteArray()
            };

            string data = functionInput.GetCallData().ToHex(true);
            if (string.IsNullOrEmpty(data))
            {
                throw new System.Exception("Transaction data is empty");
            }

            return Task.FromResult(new HexBigInteger(CalculateCallDataCost(data, config.GtxDataNonZero, config.GtxDataZero)));
        }

        public RelayRequest CreateAbiRelayRequest(GsnRelayRequest gsnRelayRequest)
        {
            return new()
            {
                RelayData = CreateAbiRelayData(gsnRelayRequest.RelayData),
                Request = CreateAbiForwardRequest(gsnRelayRequest.Request)
            };
        }

        public RelayData CreateAbiRelayData(GsnRelayData gsnRelayData)
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
                MaxFeePerGas = BigInteger.Parse(gsnRelayData.MaxFeePerGas),
                MaxPriorityFeePerGas = BigInteger.Parse(gsnRelayData.MaxPriorityFeePerGas),
                TransactionCalldataGasUsed = transactionCalldataGasUsed,
                RelayWorker = gsnRelayData.RelayWorker,
                Paymaster = gsnRelayData.Paymaster,
                PaymasterData = gsnRelayData.PaymasterData.HexToByteArray(),
                ClientId = BigInteger.Parse(gsnRelayData.ClientId),
                Forwarder = gsnRelayData.Forwarder,
            };
        }

        public ForwardRequest CreateAbiForwardRequest(GsnForwardRequest gsnForwardRequest)
        {
            BigInteger gas;
            if (gsnForwardRequest.Gas.IsHex())
            {
                gas = new HexBigInteger(gsnForwardRequest.Gas);
            }
            else
            {
                gas = BigInteger.Parse(gsnForwardRequest.Gas);
            }

            return new()
            {
                Data = gsnForwardRequest.Data.HexToByteArray(),
                From = gsnForwardRequest.From,
                To = gsnForwardRequest.To,
                Gas = gas,
                Nonce = BigInteger.Parse(gsnForwardRequest.Nonce),
                ValidUntilTime = BigInteger.Parse(gsnForwardRequest.ValidUntilTime),
                Value = BigInteger.Parse(gsnForwardRequest.Value)
            };
        }

        public async Task<BigInteger> GetSenderNonce(string sender, string forwarderAddress, Web3 provider)
        {
            IContractQueryHandler<GetNonceFunction> function = provider.Eth.GetContractQueryHandler<GetNonceFunction>();
            return await function.QueryAsync<BigInteger>(forwarderAddress, new() { From = sender });
        }

        public async Task<BigInteger> GetSenderContractNonce(Web3 provider, string tokenAddress, string address)
        {
            try
            {
                return await provider.Eth.GetContractQueryHandler<NoncesFunction>().QueryAsync<BigInteger>(tokenAddress, new NoncesFunction { Owner = address });
            }
            catch
            {
                return await provider.Eth.GetContractQueryHandler<GetNonceFunction>().QueryAsync<BigInteger>(tokenAddress, new GetNonceFunction { From = address });
            }
        }

    }

}