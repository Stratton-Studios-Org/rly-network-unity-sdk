using System.Collections;
using System.Collections.Generic;
using System.Numerics;

using Newtonsoft.Json;

namespace RallyProtocol.GSN.Models
{

    public class GsnRelayHttpRequestMetadata
    {

        [JsonProperty("maxAcceptanceBudget")]
        public string MaxAcceptanceBudget;

        [JsonProperty("relayHubAddress")]
        public string RelayHubAddress;

        [JsonProperty("signature")]
        public string Signature;

        [JsonProperty("approvalData")]
        public string ApprovalData;

        [JsonProperty("relayLastKnownNonce")]
        public BigInteger RelayLastKnownNonce;

        [JsonProperty("relayMaxNonce")]
        public BigInteger RelayMaxNonce;

        [JsonProperty("domainSeparatorName")]
        public string DomainSeparatorName;

        [JsonProperty("relayRequestId")]
        public string RelayRequestId;

    }

}