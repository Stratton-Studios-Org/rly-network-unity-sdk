using System.Collections;
using System.Collections.Generic;
using System.Numerics;

using Nethereum.Hex.HexTypes;

using Newtonsoft.Json;

using UnityEngine;

namespace RallyProtocol
{

    public class GsnTransactionDetails
    {

        /// <summary>
        /// Users address
        /// </summary>
        [JsonProperty("from")]
        public string From;

        /// <summary>
        /// Transaction data
        /// </summary>
        [JsonProperty("data")]
        public string Data;

        /// <summary>
        /// Contract address
        /// </summary>
        [JsonProperty("to")]
        public string To;

        /// <summary>
        /// Ether value
        /// </summary>
        [JsonProperty("value")]
        public string Value;
        //optional gas
        [JsonProperty("gas")]
        public string Gas;

        //should be hex
        [JsonProperty("maxFeePerGas")]
        public string MaxFeePerGas;
        //should be hex
        [JsonProperty("maxPriorityFeePerGas")]
        public string MaxPriorityFeePerGas;
        //paymaster contract address
        [JsonProperty("paymasterData")]
        public string? PaymasterData;

        //Value used to identify applications in RelayRequests.
        [JsonProperty("clientId")]
        public string ClientId;

        // Optional parameters for RelayProvider only:
        /**
         * Set to 'false' to create a direct transaction
         */
        [JsonProperty("useGSN")]
        public bool? UseGSN;

    }

}