using System.Collections;
using System.Collections.Generic;

using Newtonsoft.Json;

using UnityEngine;

namespace RallyProtocol.GSN
{

    public class OldForwardRequest
    {

        [JsonProperty("from")]
        public string From;

        [JsonProperty("to")]
        public string To;

        [JsonProperty("value")]
        public string Value;

        [JsonProperty("gas")]
        public string Gas;

        [JsonProperty("nonce")]
        public string Nonce;

        [JsonProperty("data")]
        public string Data;

        [JsonProperty("validUntilTime")]
        public string ValidUntilTime;

        public OldForwardRequest Clone()
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