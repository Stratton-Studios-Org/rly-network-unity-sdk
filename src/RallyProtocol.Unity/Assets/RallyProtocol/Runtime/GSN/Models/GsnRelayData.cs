using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;

using Nethereum.ABI.EIP712;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.Hex.HexTypes;

using Newtonsoft.Json;

using RallyProtocol.Contracts.RelayHub;

namespace RallyProtocol.GSN.Models
{

    public class GsnRelayData
    {

        #region Properties

        [JsonProperty("maxFeePerGas")]
        public virtual string MaxFeePerGas { get; set; }

        [JsonProperty("maxPriorityFeePerGas")]
        public virtual string MaxPriorityFeePerGas { get; set; }

        [JsonProperty("transactionCalldataGasUsed")]
        public virtual string TransactionCalldataGasUsed { get; set; }

        [JsonProperty("relayWorker")]
        public virtual string RelayWorker { get; set; }

        [JsonProperty("paymaster")]
        public virtual string Paymaster { get; set; }

        [JsonProperty("forwarder")]
        public virtual string Forwarder { get; set; }

        [JsonProperty("paymasterData")]
        public virtual string PaymasterData { get; set; }

        [JsonProperty("clientId")]
        public virtual string ClientId { get; set; }

        #endregion

        #region Public Methods

        public GsnRelayData Clone()
        {
            return new()
            {
                MaxFeePerGas = MaxFeePerGas,
                MaxPriorityFeePerGas = MaxPriorityFeePerGas,
                TransactionCalldataGasUsed = TransactionCalldataGasUsed,
                RelayWorker = RelayWorker,
                Paymaster = Paymaster,
                Forwarder = Forwarder,
                PaymasterData = PaymasterData,
                ClientId = ClientId
            };
        }

        public RelayData ToAbi()
        {
            return new()
            {
                MaxFeePerGas = MaxFeePerGas.IsHex() ? new HexBigInteger(MaxFeePerGas) : BigInteger.Parse(MaxFeePerGas),
                MaxPriorityFeePerGas = MaxPriorityFeePerGas.IsHex() ? new HexBigInteger(MaxPriorityFeePerGas) : BigInteger.Parse(MaxPriorityFeePerGas),
                TransactionCalldataGasUsed = TransactionCalldataGasUsed.IsHex() ? new HexBigInteger(TransactionCalldataGasUsed) : BigInteger.Parse(TransactionCalldataGasUsed),
                RelayWorker = RelayWorker,
                Paymaster = Paymaster,
                PaymasterData = PaymasterData.HexToByteArray(),
                ClientId = BigInteger.Parse(ClientId),
                Forwarder = Forwarder,
            };
        }

        public MemberValue[] ToEip712Values()
        {
            return new MemberValue[]
            {
                new() { TypeName = "uint256", Value = MaxFeePerGas },
                new() { TypeName = "uint256", Value = MaxPriorityFeePerGas },
                new() { TypeName = "uint256", Value = TransactionCalldataGasUsed },
                new() { TypeName = "address", Value = RelayWorker },
                new() { TypeName = "address", Value = Paymaster },
                new() { TypeName = "address", Value = Forwarder },
                new() { TypeName = "bytes", Value = PaymasterData },
                new() { TypeName = "uint256", Value = ClientId }
            };
        }

        #endregion

    }

}