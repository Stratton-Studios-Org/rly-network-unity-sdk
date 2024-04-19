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

        public RelayRequest RelayRequest;
        public RelayHttpRequestMetadata Metadata;

    }

    public class RelayHttpRequestMetadata
    {

        public string MaxAcceptanceBudget;
        public string RelayHubAddress;
        public string Signature;
        public string ApprovalData;
        public BigInteger RelayLastKnownNonce;
        public string DomainSeparatorName;
        public string RelayRequestId;

    }

}