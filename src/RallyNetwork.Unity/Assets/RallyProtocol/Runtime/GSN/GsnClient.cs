using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using System.Transactions;

using Nethereum.Hex.HexTypes;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;

using Newtonsoft.Json;

using UnityEngine;
using UnityEngine.Networking;

namespace RallyProtocol.GSN
{

    public class GsnServerConfigPayload
    {

        [JsonProperty("relayWorkerAddress")]
        public string RelayWorkerAddress;

        [JsonProperty("relayManagerAddress")]
        public string RelayManagerAddress;

        [JsonProperty("relayHubAddress")]
        public string RelayHubAddress;

        [JsonProperty("ownerAddress")]
        public string OwnerAddress;

        [JsonProperty("minMaxPriorityFeePerGas")]
        public string MinMaxPriorityFeePerGas;

        [JsonProperty("maxMaxFeePerGas")]
        public string MaxMaxFeePerGas;

        [JsonProperty("minMaxFeePerGas")]
        public string MinMaxFeePerGas;

        [JsonProperty("maxAcceptanceBudget")]
        public string MaxAcceptanceBudget;

        [JsonProperty("chainId")]
        public string ChainId;

        [JsonProperty("networkId")]
        public string NetworkId;

        [JsonProperty("ready")]
        public bool Ready;

        [JsonProperty("version")]
        public string Version;

    }

    public interface IGsnClient
    {

    }

    public class GsnClient
    {

        protected Account account;
        protected Web3 web3;
        protected RallyNetworkConfig config;

        public GsnClient(Web3 web3, Account account, RallyNetworkConfig config)
        {
        }

        public void RelayTransaction(RallyNetworkConfig config, GsnTransactionDetails transaction)
        {
            var updatedConfig = UpdateConfig(config, transaction);

        }

        public async Task<(RallyNetworkConfig, GsnTransactionDetails)> UpdateConfig(RallyNetworkConfig config, GsnTransactionDetails transaction)
        {
            string url = $"{config.Gsn.RelayUrl}/getaddr";
            UnityWebRequest request = UnityWebRequest.Get(url);
            request.SetRequestHeader("Authorization", $"Bearer ${config.RelayerApiKey ?? ""}");
            request.SendWebRequest();
            while (!request.isDone)
            {
                await Task.Yield();
            }

            string response = request.downloadHandler.text;
            GsnServerConfigPayload serverConfigUpdate = JsonConvert.DeserializeObject<GsnServerConfigPayload>(response);
            SetGasFeesForTransaction(transaction, serverConfigUpdate);
            return (config, transaction);
        }

        public void SetGasFeesForTransaction(GsnTransactionDetails transaction, GsnServerConfigPayload serverConfigUpdate)
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

    }

}
