using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;

using Newtonsoft.Json;

namespace RallyProtocol.GSN.Models
{

    public class GsnForwardRequest
    {

        [JsonProperty("from")]
        public virtual string From { get; set; }

        [JsonProperty("to")]
        public virtual string To { get; set; }

        [JsonProperty("value")]
        public virtual string Value { get; set; }

        [JsonProperty("gas")]
        public virtual string Gas { get; set; }

        [JsonProperty("nonce")]
        public virtual string Nonce { get; set; }

        [JsonProperty("data")]
        public virtual string Data { get; set; }

        [JsonProperty("validUntilTime")]
        public virtual string ValidUntilTime { get; set; }

        public GsnForwardRequest Clone()
        {
            return new()
            {
                From = From,
                To = To,
                Value = Value,
                Gas = Gas,
                Nonce = Nonce,
                Data = Data,
                ValidUntilTime = ValidUntilTime
            };
        }

    }

}