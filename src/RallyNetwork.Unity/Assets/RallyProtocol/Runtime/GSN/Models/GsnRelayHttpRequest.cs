using System.Collections;
using System.Collections.Generic;

using Newtonsoft.Json;

namespace RallyProtocol.GSN.Models
{

    public class GsnRelayHttpRequest
    {

        #region Fields

        [JsonProperty("relayRequest")]
        public virtual GsnRelayRequest RelayRequest { get; set; }

        [JsonProperty("metadata")]
        public virtual GsnRelayHttpRequestMetadata Metadata { get; set; }

        #endregion

    }

}