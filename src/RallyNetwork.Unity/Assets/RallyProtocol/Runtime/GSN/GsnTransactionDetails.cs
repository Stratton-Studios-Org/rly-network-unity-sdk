using System.Collections;
using System.Collections.Generic;
using System.Numerics;

using Nethereum.Hex.HexTypes;

using Newtonsoft.Json;

namespace RallyProtocol
{

    public class GsnTransactionDetails
    {

        #region Fields

        /// <summary>
        /// User's address
        /// </summary>
        [JsonProperty("from")]
        public virtual string From { get; set; }

        /// <summary>
        /// Transaction data
        /// </summary>
        [JsonProperty("data")]
        public virtual string Data { get; set; }

        /// <summary>
        /// Contract address
        /// </summary>
        [JsonProperty("to")]
        public virtual string To { get; set; }

        /// <summary>
        /// Ether value
        /// </summary>
        [JsonProperty("value")]
        public virtual string Value { get; set; }

        /// <summary>
        /// Optional gas
        /// </summary>
        [JsonProperty("gas")]
        public virtual string Gas { get; set; }

        /// <summary>
        /// Should be in hex format.
        /// </summary>
        [JsonProperty("maxFeePerGas")]
        public virtual string MaxFeePerGas { get; set; }

        /// <summary>
        /// Should be in hex format.
        /// </summary>
        [JsonProperty("maxPriorityFeePerGas")]
        public virtual string MaxPriorityFeePerGas { get; set; }

        /// <summary>
        /// Paymaster contract address.
        /// </summary>
        [JsonProperty("paymasterData")]
        public virtual string PaymasterData { get; set; }

        /// <summary>
        /// Value used to identify applications in RelayRequests.
        /// </summary>
        [JsonProperty("clientId")]
        public virtual string ClientId { get; set; }

        /// <summary>
        /// Set to 'false' to create a direct transaction.
        /// </summary>
        /// <remarks>
        /// Optional parameter for relay providers only.
        /// </remarks>
        [JsonProperty("useGSN")]
        public virtual bool? UseGSN { get; set; }

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