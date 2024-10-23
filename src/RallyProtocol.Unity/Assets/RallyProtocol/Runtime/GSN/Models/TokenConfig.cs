using System.Collections;
using System.Collections.Generic;

using Newtonsoft.Json;

using UnityEngine;

namespace RallyProtocol.GSN.Models
{

    /// <summary>
    /// <see cref="TokenConfig"/>> is a configuration object that is used to define the properties of a token.
    /// </summary>
    public class TokenConfig
    {

        #region Fields

        /// <summary>
        /// Pre-built configuration for USDC on Base Mainnet
        /// </summary>
        public static TokenConfig BaseUsdcDefault = new TokenConfig()
        {
            Address = "0x833589fcd6edb6e08f4c7c32d4f71b54bda02913",
            MetaTxMethod = MetaTxMethod.Permit,
            Eip712Domain = new PermitTransaction.Eip712Domain()
            {
                Version = "2",
            }
        };

        /// <summary>
        /// Pre-built configuration for USDC on Sepolia Base.
        /// </summary>
        /// <remarks>
        /// To get access to USDC on Sepolia Base, you can use the faucet at https://faucet.circle.com/
        /// </remarks>
        public static TokenConfig BaseSepoliaUsdcDefault = new TokenConfig()
        {
            Address = "0x036CbD53842c5426634e7929541eC2318f3dCF7e",
            MetaTxMethod = MetaTxMethod.Permit,
            Eip712Domain = new PermitTransaction.Eip712Domain()
            {
                Version = "2",
            }
        };

        /// <summary>
        /// Pre built configuration for RLY on Sepolia Base.
        /// </summary>
        /// <remarks>
        /// This is the token this SDK tests with by default.
        /// </remarks>
        public static TokenConfig BaseSepoliaRlyDefault = new TokenConfig()
        {
            Address = "0x846D8a5fb8a003b431b67115f809a9B9FFFe5012",
            MetaTxMethod = MetaTxMethod.Permit,
            Eip712Domain = new PermitTransaction.Eip712Domain()
            {
                Version = "1",
            }
        };

        /// <summary>
        /// This is a custom version of RLY configured to support the executeMetaTransaction style of meta transactions.
        /// </summary>
        /// <remarks>
        /// Should only be used for specific testing purposes. If you aren't sure whether you need <see cref="MetaTxMethod.ExecuteMetaTransaction"/>, you probably don't.
        /// </remarks>
        public static TokenConfig BaseSepoliaExecMetaRlyDefault = new TokenConfig()
        {
            Address = "0x16723e9bb894EfC09449994eC5bCF5b41EE0D9b2",
            MetaTxMethod = MetaTxMethod.ExecuteMetaTransaction,
        };

        /// <summary>
        /// The address of the token contract.
        /// </summary>
        [JsonProperty("address")]
        public string Address { get; set; }

        /// <summary>
        /// The method of meta transaction that the token supports. See <see cref="MetaTxMethod"/>> for more information.
        /// </summary>
        /// <remarks>
        /// This is most likely going to be MetaTxMethod.Permit.
        /// </remarks>
        [JsonProperty("metaTxMethod")]
        public MetaTxMethod MetaTxMethod { get; set; }

        /// <summary>
        /// The EIP712 domain object for the token. This is only required if the token uses non default values for EIP712 signature generation.
        /// </summary>
        [JsonProperty("eip712Domain")]
        public PermitTransaction.Eip712Domain Eip712Domain { get; set; }

        #endregion

        #region Properties

        public static TokenConfig BaseUsdc => BaseUsdcDefault.Clone();

        public static TokenConfig BaseSepoliaUsdc => BaseSepoliaUsdcDefault.Clone();

        public static TokenConfig BaseSepoliaRly => BaseSepoliaRlyDefault.Clone();

        public static TokenConfig BaseSepoliaExecMetaRly => BaseSepoliaExecMetaRlyDefault.Clone();

        #endregion

        #region Methods

        public TokenConfig Clone()
        {
            return new TokenConfig()
            {
                Address = Address,
                MetaTxMethod = MetaTxMethod,
                Eip712Domain = Eip712Domain,
            };
        }

        #endregion

    }

}