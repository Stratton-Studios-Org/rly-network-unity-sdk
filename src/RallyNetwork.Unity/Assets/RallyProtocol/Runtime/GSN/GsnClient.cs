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
            UnityWebRequestRpcTaskClient unityClient = new(new Uri(config.Gsn.RpcUrl), null, null);
            string apiKey = "";
            if (!string.IsNullOrEmpty(config.RelayerApiKey))
            {
                apiKey = UnityWebRequest.EscapeURL(config.RelayerApiKey);
                apiKey = config.RelayerApiKey;
            }

            unityClient.RequestHeaders["Authorization"] = $"Bearer {apiKey}";
            return new Web3(unityClient);
        }

        public async Task<string> RelayTransaction(Account account, RallyNetworkConfig config, GsnTransactionDetails transaction)
        {
            Web3 provider = GetProvider(config);
            var updatedConfig = await UpdateConfig(config, transaction);
            GsnRelayRequest relayRequest = await BuildRelayRequest(updatedConfig.Transaction, updatedConfig.Config, account, provider);
            GsnRelayHttpRequest httpRequest = await BuildRelayHttpRequest(relayRequest, updatedConfig.Config, account, provider);

            // Update request metadata with relayRequestId
            string relayRequestId = GsnTransactionHelper.GetRelayRequestId(httpRequest.RelayRequest, httpRequest.Metadata.Signature);
            httpRequest.Metadata.RelayRequestId = relayRequestId;

            string url = $"{config.Gsn.RelayUrl}/relay";
            //UnityWebRequest request = new(url, UnityWebRequest.kHttpVerbPOST);
            //request.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(httpRequest)));
            //request.uploadHandler.contentType = "application/json";
            //request.SetRequestHeader("Content-Type", "application/json");
            //request.downloadHandler = new DownloadHandlerBuffer();
            UnityWebRequest request = UnityWebRequest.Post(url, JsonConvert.SerializeObject(httpRequest), "application/json");
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

        protected async Task<GsnRelayHttpRequest> BuildRelayHttpRequest(GsnRelayRequest relayRequest, RallyNetworkConfig config, Account account, Web3 provider)
        {
            string signature = await GsnTransactionHelper.SignRequest(relayRequest, config.Gsn.DomainSeparatorName, config.Gsn.ChainId, account);
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
            transaction.Gas = GsnTransactionHelper.EstimateGasWithoutCallData(transaction, config.Gsn.GtxDataNonZero, config.Gsn.GtxDataZero);

            long secondsNow = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            long validUntilTime = secondsNow + config.Gsn.RequestValidSeconds;

            BigInteger senderNonce = await GsnTransactionHelper.GetSenderNonce(account.Address, config.Gsn.ForwarderAddress, provider);

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

            HexBigInteger transactionCallDataGasUsed = await GsnTransactionHelper.EstimateCallDataCostForRequest(relayRequest, config.Gsn, provider);
            relayRequest.RelayData.TransactionCalldataGasUsed = transactionCallDataGasUsed.Value.ToString();

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

            if (request.result != UnityWebRequest.Result.Success)
            {
                throw new RallyException($"Updating config failed:\nResponse Code: {request.responseCode}\nError: {request.error}\nResponse Text: {request.downloadHandler.text}");
            }
            string response = request.downloadHandler.text;
            GsnServerConfigPayload serverConfigUpdate = JsonConvert.DeserializeObject<GsnServerConfigPayload>(response);

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

        protected UnityWebRequest AddAuthHeader(UnityWebRequest request, RallyNetworkConfig config)
        {
            string apiKey = "";
            if (!string.IsNullOrEmpty(config.RelayerApiKey))
            {
                apiKey = UnityWebRequest.EscapeURL(config.RelayerApiKey);
                apiKey = config.RelayerApiKey;
            }

            request.SetRequestHeader("Authorization", $"Bearer {apiKey}");
            return request;
        }

        #endregion

    }

}
