using System.Collections;
using System.Collections.Generic;
using System.Numerics;

using Newtonsoft.Json;

namespace RallyProtocol.GSN.Models
{

    public class GsnRelayHttpRequestMetadata
    {

        #region Fields

        [JsonProperty("maxAcceptanceBudget")]
        public virtual string MaxAcceptanceBudget { get; set; }

        [JsonProperty("relayHubAddress")]
        public string RelayHubAddress { get; set; }

        [JsonProperty("signature")]
        public virtual string Signature { get; set; }

        [JsonProperty("approvalData")]
        public virtual string ApprovalData { get; set; }

        [JsonProperty("relayLastKnownNonce")]
        public virtual BigInteger RelayLastKnownNonce { get; set; }

        [JsonProperty("relayMaxNonce")]
        public BigInteger RelayMaxNonce { get; set; }

        [JsonProperty("domainSeparatorName")]
        public virtual string DomainSeparatorName { get; set; }

        [JsonProperty("relayRequestId")]
        public virtual string RelayRequestId { get; set; }

        #endregion

    }

}