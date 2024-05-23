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

        #region Fields

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

        /// <summary>
        /// Optional gas
        /// </summary>
        [JsonProperty("gas")]
        public string Gas;

        /// <summary>
        /// Should be in hex format.
        /// </summary>
        [JsonProperty("maxFeePerGas")]
        public string MaxFeePerGas;

        /// <summary>
        /// Should be in hex format.
        /// </summary>
        [JsonProperty("maxPriorityFeePerGas")]
        public string MaxPriorityFeePerGas;

        /// <summary>
        /// Paymaster contract address.
        /// </summary>
        [JsonProperty("paymasterData")]
        public string PaymasterData;

        /// <summary>
        /// Value used to identify applications in RelayRequests.
        /// </summary>
        [JsonProperty("clientId")]
        public string ClientId;

        /// <summary>
        /// Set to 'false' to create a direct transaction.
        /// </summary>
        /// <remarks>
        /// Optional parameters for relay providers only.
        /// </remarks>
        [JsonProperty("useGSN")]
        public bool? UseGSN;

        #endregion

        #region Constructors

        public GsnTransactionDetails() { }

        public GsnTransactionDetails(string from, string data, string to, string maxFeePerGas, string maxPriorityFeePerGas, string value = null, string gas = null, string paymasterData = null, string clientId = null, bool? useGsn = null)
        {
            From = from.ToLowerInvariant();
            Data = data;
            To = to.ToLowerInvariant();
            Value = value;
            Gas = gas;
            MaxFeePerGas = maxFeePerGas;
            MaxPriorityFeePerGas = maxPriorityFeePerGas;
            PaymasterData = paymasterData;
            ClientId = clientId;
            UseGSN = useGsn;
        }

        #endregion

        #region Public Methods

        public override string ToString()
        {
            return $"from: {From}, data: {Data}, to: {To}, value: {Value}, gas: {Gas}, maxFeePerGas: {MaxFeePerGas}, maxPriorityFeePerGas: {MaxPriorityFeePerGas}, paymasterData: {PaymasterData}, clientId: {ClientId}, useGSN: {UseGSN}";
        }

        #endregion

    }

}