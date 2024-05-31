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

using RallyProtocol.Accounts;
using RallyProtocol.Contracts.Forwarder;
using RallyProtocol.Contracts.RelayHub;
using RallyProtocol.Core;
using RallyProtocol.GSN.Models;
using RallyProtocol.Logging;
using RallyProtocol.Networks;

using Account = Nethereum.Web3.Accounts.Account;

namespace RallyProtocol.GSN
{

    public class GsnTransactionHelper
    {

        #region Constants

        public const string GsnDomainSeparatorVersion = "3";

        #endregion

        #region Fields

        protected IRallyLogger logger;

        #endregion

        #region Properties

        public IRallyLogger Logger => this.logger;

        #endregion

        #region Constructors

        public GsnTransactionHelper(IRallyLogger logger)
        {
            this.logger = logger;
        }

        #endregion

        #region Public Methods

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
            RelayRequest relayRequest = gsnRelayRequest.ToAbi();
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
            TypedData<DomainWithChainIdString> typedData = new();
            typedData.PrimaryType = TypedGsnRequestData.PrimaryType;

            // Setup types
            typedData.Types = new Dictionary<string, MemberDescription[]>()
            {
                ["EIP712Domain"] = TypedGsnRequestData.DomainType.ToArray(),
                ["RelayRequest"] = TypedGsnRequestData.RelayRequestType.ToArray(),
                ["RelayData"] = TypedGsnRequestData.RelayDataType.ToArray(),
            };

            // Setup message
            typedData.Message = TypedGsnRequestData.CreateMessage(relayRequest);

            // Setup domain
            DomainWithChainIdString domain = new()
            {
                Name = domainSeparatorName,
                Version = TypedGsnRequestData.Version,
                ChainId = chainId,
                VerifyingContract = relayRequest.RelayData.Forwarder
            };
            typedData.Domain = domain;

            // Sign the typed data
            string signature = account.SignTypedDataV4(typedData);
            //byte[] hashedData = Sha3Keccack.Current.CalculateHash(Eip712TypedDataSigner.Current.EncodeTypedData(typedData));
            //string newSig = EthECDSASignature.CreateStringSignature(new EthECKey(account.PrivateKey).SignAndCalculateV(hashedData));
            return Task.FromResult(signature);
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
                RelayRequest = relayRequest.ToAbi(),
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

        #endregion

    }

}