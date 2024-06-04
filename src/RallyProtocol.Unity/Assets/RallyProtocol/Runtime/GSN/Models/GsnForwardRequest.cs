using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;

using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.Hex.HexTypes;

using Newtonsoft.Json;

using RallyProtocol.Contracts.Forwarder;

namespace RallyProtocol.GSN.Models
{

    public class GsnForwardRequest
    {

        #region Properties

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

        #endregion

        #region Public Methods

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

        public ForwardRequest ToAbi()
        {
            BigInteger gas;
            if (Gas.IsHex())
            {
                gas = new HexBigInteger(Gas);
            }
            else
            {
                gas = BigInteger.Parse(Gas);
            }

            return new()
            {
                Data = Data.HexToByteArray(),
                From = From,
                To = To,
                Gas = gas,
                Nonce = BigInteger.Parse(Nonce),
                ValidUntilTime = BigInteger.Parse(ValidUntilTime),
                Value = BigInteger.Parse(Value)
            };
        }

        #endregion

    }

}