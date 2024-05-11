using System.Collections;
using System.Collections.Generic;

using Newtonsoft.Json;

namespace RallyProtocol.GSN.Models
{

    public class GsnRelayHttpRequest
    {

        [JsonProperty("relayRequest")]
        public GsnRelayRequest RelayRequest;

        [JsonProperty("metadata")]
        public GsnRelayHttpRequestMetadata Metadata;

    }

}