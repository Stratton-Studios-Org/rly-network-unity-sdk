using System.Collections;
using System.Collections.Generic;

using Newtonsoft.Json;

using RallyProtocol.Contracts.RelayHub;

namespace RallyProtocol.GSN.Models
{

    public class GsnRelayRequest
    {

        #region Properties

        [JsonProperty("request")]
        public virtual GsnForwardRequest Request { get; set; }

        [JsonProperty("relayData")]
        public virtual GsnRelayData RelayData { get; set; }

        #endregion

        #region Public Methods

        public GsnRelayRequest Clone()
        {
            return new()
            {
                Request = Request.Clone(),
                RelayData = RelayData.Clone(),
            };
        }

        public RelayRequest ToAbi()
        {
            return new()
            {
                Request = Request.ToAbi(),
                RelayData = RelayData.ToAbi()
            };
        }

        #endregion

    }

}