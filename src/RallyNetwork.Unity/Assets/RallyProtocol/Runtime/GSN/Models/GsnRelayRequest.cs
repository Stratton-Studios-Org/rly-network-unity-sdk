using System.Collections;
using System.Collections.Generic;

using Newtonsoft.Json;

namespace RallyProtocol.GSN.Models
{

    public class GsnRelayRequest
    {

        [JsonProperty("request")]
        public virtual GsnForwardRequest Request { get; set; }

        [JsonProperty("relayData")]
        public virtual GsnRelayData RelayData { get; set; }

        public GsnRelayRequest Clone()
        {
            return new()
            {
                Request = Request.Clone(),
                RelayData = RelayData.Clone(),
            };
        }

    }

}