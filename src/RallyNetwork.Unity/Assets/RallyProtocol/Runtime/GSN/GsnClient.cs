using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using System.Transactions;

using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.Hex.HexTypes;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;

using Newtonsoft.Json;

using RallyProtocol.GSN.Contracts;

using UnityEngine;
using UnityEngine.Networking;

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

    }

    public class GsnClient
    {

        public GsnClient()
        {
        }

        #region Public Methods

        public Web3 GetProvider(RallyNetworkConfig config)
        {
            return new Web3(config.Gsn.RpcUrl, authenticationHeader: new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", config.RelayerApiKey));
        }

        public async Task<string> RelayTransaction(Account account, RallyNetworkConfig config, GsnTransactionDetails transaction)
        {
            Web3 provider = GetProvider(config);
            var updatedConfig = await UpdateConfig(config, transaction);
            RelayRequest relayRequest = await BuildRelayRequest(updatedConfig.Transaction, updatedConfig.Config, account, provider);
            RelayHttpRequest httpRequest = await BuildRelayHttpRequest(relayRequest, updatedConfig.Config, account, provider);

            // Update request metadata with relayRequestId
            string relayRequestId = GsnTransactionHelper.GetRelayRequestId(httpRequest.RelayRequest, httpRequest.Metadata.Signature);
            httpRequest.Metadata.RelayRequestId = relayRequestId;

            UnityWebRequest request = UnityWebRequest.Post($"{config.Gsn.RelayUrl}/relay", JsonConvert.SerializeObject(httpRequest), "application/json");
            AddAuthHeader(request, config);
            request.SendWebRequest();
            while (!request.isDone)
            {
                await Task.Yield();
            }

            return await GsnTransactionHelper.HandleGsnResponse(request, provider);
        }

        #endregion

        #region Protected Methods

        protected async Task<RelayHttpRequest> BuildRelayHttpRequest(RelayRequest relayRequest, RallyNetworkConfig config, Account account, Web3 provider)
        {
            string signature = await GsnTransactionHelper.SignRequest(relayRequest, config.Gsn.DomainSeparatorName, config.Gsn.ChainId, account);
            HexBigInteger relayLastKnownNonce = await provider.Eth.Transactions.GetTransactionCount.SendRequestAsync(relayRequest.RelayData.RelayWorker);
            BigInteger relayMaxNonce = relayLastKnownNonce.Value + config.Gsn.MaxRelayNonceGap;
            RelayHttpRequestMetadata metadata = new()
            {
                MaxAcceptanceBudget = config.Gsn.MaxAcceptanceBudget,
                RelayHubAddress = config.Gsn.RelayHubAddress,
                Signature = signature,
                ApprovalData = "0x",
                RelayLastKnownNonce = relayLastKnownNonce,
                DomainSeparatorName = config.Gsn.DomainSeparatorName,
                RelayRequestId = ""
            };
            RelayHttpRequest httpRequest = new()
            {
                RelayRequest = relayRequest,
                Metadata = metadata,
            };

            return httpRequest;
        }

        protected async Task<RelayRequest> BuildRelayRequest(GsnTransactionDetails transaction, RallyNetworkConfig config, Account account, Web3 provider)
        {
            transaction.Gas = GsnTransactionHelper.EstimateGasWithoutCallData(transaction, config.Gsn.GtxDataNonZero, config.Gsn.GtxDataZero);
            double secondsNow = TimeSpan.FromTicks(DateTime.UtcNow.Ticks).TotalSeconds;
            double validUntilTime = (secondsNow + config.Gsn.RequestValidSeconds);

            BigInteger senderNonce = await GsnTransactionHelper.GetSenderNonce(account.Address, config.Gsn.ForwarderAddress, provider);

            RelayRequest relayRequest = new()
            {
                Request = new()
                {
                    From = transaction.From,
                    To = transaction.To,
                    Value = transaction.Value == null ? BigInteger.Zero : transaction.Value.Value,
                    Gas = new HexBigInteger(transaction.Gas).Value, // parse from hex to bigint
                    Nonce = senderNonce,
                    Data = transaction.Data == null ? new byte[0] : transaction.Data.HexToByteArray(),
                    ValidUntilTime = new BigInteger(validUntilTime),
                },
                RelayData = new()
                {
                    MaxFeePerGas = BigInteger.Parse(transaction.MaxFeePerGas),
                    MaxPriorityFeePerGas = BigInteger.Parse(transaction.MaxPriorityFeePerGas),
                    TransactionCalldataGasUsed = BigInteger.Zero,
                    RelayWorker = config.Gsn.RelayHubAddress,
                    Paymaster = config.Gsn.PaymasterAddress,
                    Forwarder = config.Gsn.ForwarderAddress,
                    PaymasterData = string.IsNullOrEmpty(transaction.PaymasterData) ? new HexBigInteger("0x").ToHexByteArray() : new HexBigInteger(transaction.PaymasterData).ToHexByteArray(),
                    ClientId = BigInteger.One
                }
            };

            string transactionCallDataGasUsed = await GsnTransactionHelper.EstimateCallDataCostForRequest(relayRequest, config.Gsn, provider);
            relayRequest.RelayData.TransactionCalldataGasUsed = new HexBigInteger(transactionCallDataGasUsed);

            return relayRequest;
        }

        protected async Task<(RallyNetworkConfig Config, GsnTransactionDetails Transaction)> UpdateConfig(RallyNetworkConfig config, GsnTransactionDetails transaction)
        {
            string url = $"{config.Gsn.RelayUrl}/getaddr";
            UnityWebRequest request = UnityWebRequest.Get(url);
            AddAuthHeader(request, config);
            request.SendWebRequest();
            while (!request.isDone)
            {
                await Task.Yield();
            }

            string response = request.downloadHandler.text;
            GsnServerConfigPayload serverConfigUpdate = JsonConvert.DeserializeObject<GsnServerConfigPayload>(response);
            SetGasFeesForTransaction(transaction, serverConfigUpdate);
            return (Config: config, Transaction: transaction);
        }

        protected void SetGasFeesForTransaction(GsnTransactionDetails transaction, GsnServerConfigPayload serverConfigUpdate)
        {
            int serverSuggestedMinPriorityFeePerGas = Convert.ToInt32(serverConfigUpdate.MinMaxPriorityFeePerGas, 10);
            int paddedMaxPriority = Mathf.RoundToInt(serverSuggestedMinPriorityFeePerGas / 1.4f);
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

        protected UnityWebRequest AddAuthHeader(UnityWebRequest request, RallyNetworkConfig config)
        {
            request.SetRequestHeader("Authorization", $"Bearer ${config.RelayerApiKey ?? ""}");
            return request;
        }

        #endregion

    }

}
