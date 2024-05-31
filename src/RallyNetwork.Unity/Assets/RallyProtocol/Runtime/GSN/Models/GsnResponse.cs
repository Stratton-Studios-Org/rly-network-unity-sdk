using System.Collections;
using System.Collections.Generic;

using Newtonsoft.Json;

namespace RallyProtocol.GSN.Models
{

    public class GsnResponse
    {

        [JsonProperty("error")]
        public virtual string Error { get; set; }

        [JsonProperty("signedTx")]
        public virtual string SignedTx { get; set; }

    }

}