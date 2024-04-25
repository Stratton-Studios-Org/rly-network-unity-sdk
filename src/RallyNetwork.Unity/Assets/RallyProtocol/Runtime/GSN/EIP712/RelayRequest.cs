using System.Collections;
using System.Collections.Generic;
using System.Numerics;

using Newtonsoft.Json;

using RallyProtocol.GSN.Contracts;

using UnityEngine;

namespace RallyProtocol.GSN
{

    public class OldRelayRequest
    {

        [JsonProperty("request")]
        public OldForwardRequest Request;

        [JsonProperty("relayData")]
        public OldRelayData RelayData;

        public OldRelayRequest Clone()
        {
            return new()
            {
                Request = this.Request.Clone(),
                RelayData = this.RelayData.Clone()
            };

        }

    }

    public class RelayHttpRequest
    {

        [JsonProperty("relayRequest")]
        public RelayRequest RelayRequest;

        [JsonProperty("metaData")]
        public RelayHttpRequestMetadata Metadata;

    }

    public class RelayHttpRequestMetadata
    {

        [JsonProperty("maxAcceptanceBudget")]
        public string MaxAcceptanceBudget;

        [JsonProperty("relayHubAddress")]
        public string RelayHubAddress;

        [JsonProperty("signature")]
        public string Signature;

        [JsonProperty("approvalData")]
        public string ApprovalData;

        [JsonProperty("relayMaxNonce")]
        public BigInteger RelayLastKnownNonce;

        [JsonProperty("domainSeparatorName")]
        public string DomainSeparatorName;

        [JsonProperty("relayRequestId")]
        public string RelayRequestId;

    }

}