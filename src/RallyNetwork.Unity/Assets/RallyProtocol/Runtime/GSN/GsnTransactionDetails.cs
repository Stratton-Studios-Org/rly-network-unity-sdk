using System.Collections;
using System.Collections.Generic;
using System.Numerics;

using Nethereum.Hex.HexTypes;

using Newtonsoft.Json;

namespace RallyProtocol
{

    /// <summary>
    /// Rally's GSN transaction details.
    /// </summary>
    public class GsnTransactionDetails
    {

        #region Fields

        /// <summary>
        /// User's address
        /// </summary>
        [JsonProperty("from")]
        public virtual string From { get; set; } = string.Empty;

        /// <summary>
        /// Transaction data
        /// </summary>
        [JsonProperty("data")]
        public virtual string Data { get; set; } = string.Empty;

        /// <summary>
        /// Contract address
        /// </summary>
        [JsonProperty("to")]
        public virtual string To { get; set; } = string.Empty;

        /// <summary>
        /// Ether value
        /// </summary>
        [JsonProperty("value")]
        public virtual string? Value { get; set; }

        /// <summary>
        /// Optional gas
        /// </summary>
        [JsonProperty("gas")]
        public virtual string? Gas { get; set; }

        /// <summary>
        /// Should be in hex format.
        /// </summary>
        [JsonProperty("maxFeePerGas")]
        public virtual string MaxFeePerGas { get; set; } = string.Empty;

        /// <summary>
        /// Should be in hex format.
        /// </summary>
        [JsonProperty("maxPriorityFeePerGas")]
        public virtual string MaxPriorityFeePerGas { get; set; } = string.Empty;

        /// <summary>
        /// Paymaster contract address.
        /// </summary>
        [JsonProperty("paymasterData")]
        public virtual string? PaymasterData { get; set; }

        /// <summary>
        /// Value used to identify applications in RelayRequests.
        /// </summary>
        [JsonProperty("clientId")]
        public virtual string? ClientId { get; set; }

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

        /// <summary>
        /// Creates a new instance of <see cref="GsnTransactionDetails"/>
        /// </summary>
        public GsnTransactionDetails()
        {
        }

        /// <summary>
        /// Create a new instance of <see cref="GsnTransactionDetails"/>
        /// </summary>
        /// <param name="from">From address</param>
        /// <param name="data">Transaction data</param>
        /// <param name="to">To address</param>
        /// <param name="maxFeePerGas">Max fee per gas</param>
        /// <param name="maxPriorityFeePerGas">Max priority fee per gas</param>
        /// <param name="value">Value</param>
        /// <param name="gas">Gas</param>
        /// <param name="paymasterData">Paymaster data</param>
        /// <param name="clientId">Client ID</param>
        /// <param name="useGsn">Use GSN</param>
        public GsnTransactionDetails(string from, string data, string to, string maxFeePerGas, string maxPriorityFeePerGas, string? value = null, string? gas = null, string? paymasterData = null, string? clientId = null, bool? useGsn = null)
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

        /// <summary>
        /// Returns the string representation of <see cref="GsnTransactionDetails"/>
        /// </summary>
        /// <returns>Returns the transaction as string</returns>
        public override string ToString()
        {
            return $"from: {From}, data: {Data}, to: {To}, value: {Value}, gas: {Gas}, maxFeePerGas: {MaxFeePerGas}, maxPriorityFeePerGas: {MaxPriorityFeePerGas}, paymasterData: {PaymasterData}, clientId: {ClientId}, useGSN: {UseGSN}";
        }

        #endregion

    }

}