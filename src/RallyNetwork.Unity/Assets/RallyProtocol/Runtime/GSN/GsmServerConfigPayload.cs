using System.Collections;
using System.Collections.Generic;

using Newtonsoft.Json;

using UnityEngine;

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

}