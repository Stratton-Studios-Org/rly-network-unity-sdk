using System.Collections;
using System.Collections.Generic;

using Newtonsoft.Json;

namespace RallyProtocol.GSN
{

    public class GsnServerConfigPayload
    {

        #region Fields

        [JsonProperty("relayWorkerAddress")]
        public virtual string RelayWorkerAddress { get; set; } = string.Empty;

        [JsonProperty("relayManagerAddress")]
        public virtual string RelayManagerAddress { get; set; } = string.Empty;

        [JsonProperty("relayHubAddress")]
        public virtual string RelayHubAddress { get; set; } = string.Empty;

        [JsonProperty("ownerAddress")]
        public virtual string OwnerAddress { get; set; } = string.Empty;

        [JsonProperty("minMaxPriorityFeePerGas")]
        public virtual string MinMaxPriorityFeePerGas { get; set; } = string.Empty;

        [JsonProperty("maxMaxFeePerGas")]
        public virtual string MaxMaxFeePerGas { get; set; } = string.Empty;

        [JsonProperty("minMaxFeePerGas")]
        public virtual string MinMaxFeePerGas { get; set; } = string.Empty;

        [JsonProperty("maxAcceptanceBudget")]
        public virtual string MaxAcceptanceBudget { get; set; } = string.Empty;

        [JsonProperty("chainId")]
        public virtual string ChainId { get; set; } = string.Empty;

        [JsonProperty("networkId")]
        public virtual string NetworkId { get; set; } = string.Empty;

        [JsonProperty("ready")]
        public virtual bool Ready { get; set; } = default;

        [JsonProperty("version")]
        public virtual string Version { get; set; } = string.Empty;

        #endregion

    }

}