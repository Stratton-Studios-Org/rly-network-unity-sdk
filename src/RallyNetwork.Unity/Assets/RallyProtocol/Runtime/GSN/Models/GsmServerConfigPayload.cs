using System.Collections;
using System.Collections.Generic;

using Newtonsoft.Json;

using UnityEngine;

namespace RallyProtocol.GSN
{

    public class GsnServerConfigPayload
    {

        #region Fields

        [JsonProperty("relayWorkerAddress")]
        public virtual string RelayWorkerAddress { get; set; }

        [JsonProperty("relayManagerAddress")]
        public virtual string RelayManagerAddress { get; set; }

        [JsonProperty("relayHubAddress")]
        public virtual string RelayHubAddress { get; set; }

        [JsonProperty("ownerAddress")]
        public virtual string OwnerAddress { get; set; }

        [JsonProperty("minMaxPriorityFeePerGas")]
        public virtual string MinMaxPriorityFeePerGas { get; set; }

        [JsonProperty("maxMaxFeePerGas")]
        public virtual string MaxMaxFeePerGas { get; set; }

        [JsonProperty("minMaxFeePerGas")]
        public virtual string MinMaxFeePerGas { get; set; }

        [JsonProperty("maxAcceptanceBudget")]
        public virtual string MaxAcceptanceBudget { get; set; }

        [JsonProperty("chainId")]
        public virtual string ChainId { get; set; }

        [JsonProperty("networkId")]
        public virtual string NetworkId { get; set; }

        [JsonProperty("ready")]
        public virtual bool Ready { get; set; }

        [JsonProperty("version")]
        public virtual string Version { get; set; }

        #endregion

    }

}