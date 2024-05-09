using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.RPC.NonceServices;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;

using Newtonsoft.Json;

using RallyProtocol.Core;
using RallyProtocol.GSN.Contracts;
using RallyProtocol.GSN.Models;
using RallyProtocol.Logging;
using RallyProtocol.Networks;

namespace RallyProtocol.GSN
{

    public class GsnResponse
    {

        [JsonProperty("error")]
        public string Error { get; set; }

        [JsonProperty("signedTx")]
        public string SignedTx { get; set; }

    }

    public interface IGsnClient
    {

        public Task<string> RelayTransaction(Account account, RallyNetworkConfig config, GsnTransactionDetails transaction);

    }

    public class GsnClient
    {

        protected IRallyWeb3Provider web3Provider;
        protected IRallyHttpHandler httpHandler;

        protected IRallyLogger logger;
        protected GsnTransactionHelper transactionHelper;

        public IRallyLogger Logger => this.logger;
        public GsnTransactionHelper TransactionHelper => this.transactionHelper;

        public GsnClient(IRallyWeb3Provider web3Provider, IRallyHttpHandler httpHandler, IRallyLogger logger)
        {
            this.web3Provider = web3Provider;
            this.httpHandler = httpHandler;
            this.logger = logger;
            this.transactionHelper = new(logger);
        }

        #region Public Methods

        public Web3 GetProvider(RallyNetworkConfig config)
        {
            return this.web3Provider.GetWeb3(config);
        }

        public async Task<string> RelayTransaction(Account account, RallyNetworkConfig config, GsnTransactionDetails transaction)
        {
            Web3 provider = GetProvider(config);
            var updatedConfig = await UpdateConfig(config, transaction);
            GsnRelayRequest relayRequest = await BuildRelayRequest(updatedConfig.Transaction, updatedConfig.Config, account, provider);
            GsnRelayHttpRequest httpRequest = await BuildRelayHttpRequest(relayRequest, updatedConfig.Config, account, provider);

            // Update request metadata with relayRequestId
            string relayRequestId = this.transactionHelper.GetRelayRequestId(httpRequest.RelayRequest, httpRequest.Metadata.Signature);
            httpRequest.Metadata.RelayRequestId = relayRequestId;

            string url = $"{config.Gsn.RelayUrl}/relay";
            RallyHttpResponse response = await this.httpHandler.PostJson(url, JsonConvert.SerializeObject(httpRequest), AddAuthHeader(config));

            return await this.transactionHelper.HandleGsnResponse(response, provider);
        }

        #endregion

        #region Protected Methods

        protected async Task<GsnRelayHttpRequest> BuildRelayHttpRequest(GsnRelayRequest relayRequest, RallyNetworkConfig config, Account account, Web3 provider)
        {
            string signature = await this.transactionHelper.SignRequest(relayRequest, config.Gsn.DomainSeparatorName, config.Gsn.ChainId, account);
            const string approvalData = "0x";

            HexBigInteger relayLastKnownNonce = await provider.Eth.Transactions.GetTransactionCount.SendRequestAsync(relayRequest.RelayData.RelayWorker);
            BigInteger relayMaxNonce = relayLastKnownNonce.Value + config.Gsn.MaxRelayNonceGap;

            GsnRelayHttpRequestMetadata metadata = new()
            {
                MaxAcceptanceBudget = config.Gsn.MaxAcceptanceBudget,
                RelayHubAddress = config.Gsn.RelayHubAddress,
                Signature = signature,
                ApprovalData = approvalData,
                RelayLastKnownNonce = relayLastKnownNonce,
                RelayMaxNonce = relayMaxNonce,
                DomainSeparatorName = config.Gsn.DomainSeparatorName,
                RelayRequestId = string.Empty
            };
            GsnRelayHttpRequest httpRequest = new()
            {
                RelayRequest = relayRequest,
                Metadata = metadata,
            };

            return httpRequest;
        }

        protected async Task<GsnRelayRequest> BuildRelayRequest(GsnTransactionDetails transaction, RallyNetworkConfig config, Account account, Web3 provider)
        {
            transaction.Gas = this.transactionHelper.EstimateGasWithoutCallData(transaction, config.Gsn.GtxDataNonZero, config.Gsn.GtxDataZero);

            long secondsNow = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            long validUntilTime = secondsNow + config.Gsn.RequestValidSeconds;

            BigInteger senderNonce = await this.transactionHelper.GetSenderNonce(account.Address, config.Gsn.ForwarderAddress, provider);

            GsnRelayRequest relayRequest = new()
            {
                Request = new()
                {
                    From = transaction.From,
                    To = transaction.To,
                    Value = string.IsNullOrEmpty(transaction.Value) ? "0" : transaction.Value,
                    Gas = new HexBigInteger(transaction.Gas).HexValue.ToString(), // parse from hex to bigint
                    Nonce = senderNonce.ToString(),
                    Data = transaction.Data,
                    ValidUntilTime = validUntilTime.ToString(),
                },
                RelayData = new()
                {
                    MaxFeePerGas = transaction.MaxFeePerGas,
                    MaxPriorityFeePerGas = transaction.MaxPriorityFeePerGas,
                    TransactionCalldataGasUsed = string.Empty,
                    RelayWorker = config.Gsn.RelayWorkerAddress,
                    Paymaster = config.Gsn.PaymasterAddress,
                    Forwarder = config.Gsn.ForwarderAddress,
                    PaymasterData = string.IsNullOrEmpty(transaction.PaymasterData) ? "0x" : transaction.PaymasterData,
                    ClientId = "1"
                }
            };

            HexBigInteger transactionCallDataGasUsed = await this.transactionHelper.EstimateCallDataCostForRequest(relayRequest, config.Gsn, provider);
            relayRequest.RelayData.TransactionCalldataGasUsed = transactionCallDataGasUsed.Value.ToString();

            return relayRequest;
        }

        protected async Task<(RallyNetworkConfig Config, GsnTransactionDetails Transaction)> UpdateConfig(RallyNetworkConfig config, GsnTransactionDetails transaction)
        {
            string url = $"{config.Gsn.RelayUrl}/getaddr";
            RallyHttpResponse httpResponse = await this.httpHandler.Get(url, AddAuthHeader(config));
            if (!httpResponse.IsCompletedSuccessfully)
            {
                throw new RallyException($"Updating config failed:\nResponse Code: {httpResponse.ResponseCode}\nError: {httpResponse.ErrorMessage}\nResponse Text: {httpResponse.ResponseText}");
            }

            GsnServerConfigPayload serverConfigUpdate = httpResponse.DeserializeJson<GsnServerConfigPayload>();
            config.Gsn.RelayWorkerAddress = serverConfigUpdate.RelayWorkerAddress;
            SetGasFeesForTransaction(transaction, serverConfigUpdate);
            return (Config: config, Transaction: transaction);
        }

        protected void SetGasFeesForTransaction(GsnTransactionDetails transaction, GsnServerConfigPayload serverConfigUpdate)
        {
            BigInteger serverSuggestedMinPriorityFeePerGas = BigInteger.Parse(serverConfigUpdate.MinMaxPriorityFeePerGas);
            BigInteger paddedMaxPriority = serverSuggestedMinPriorityFeePerGas * 140 / 100;
            transaction.MaxPriorityFeePerGas = paddedMaxPriority.ToString();

            // Special handling for mumbai because of quirk with gas estimate returned by GSN for mumbai
            if (serverConfigUpdate.ChainId == "80001")
            {
                transaction.MaxFeePerGas = paddedMaxPriority.ToString();
            }
            else
            {
                transaction.MaxFeePerGas = serverConfigUpdate.MaxMaxFeePerGas;
            }
        }

        protected Dictionary<string, string> AddAuthHeader(RallyNetworkConfig config, Dictionary<string, string> existingHeaders = null)
        {
            if (existingHeaders == null)
            {
                existingHeaders = new();
            }

            string apiKey = "";
            if (!string.IsNullOrEmpty(config.RelayerApiKey))
            {
                apiKey = config.RelayerApiKey;
            }

            existingHeaders["Authorization"] = $"Bearer {apiKey}";
            return existingHeaders;
        }

        #endregion

    }

}
