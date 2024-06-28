using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;

using Nethereum.ABI.EIP712;

using RallyProtocol.GSN.Models;

namespace RallyProtocol.GSN
{

    public static class TypedGsnRequestData
    {

        #region Constants

        public const string PrimaryType = "RelayRequest";
        public const string Version = "3";
        public const string GsnDomainSeparatorPrefix = "string name,string version";

        #endregion

        #region Fields

        public static readonly List<MemberDescription> DomainType = new()
        {
            new() { Name = "name", Type = "string" },
            new() { Name = "version", Type = "string" },
            new() { Name = "chainId", Type = "uint256" },
            new() { Name = "verifyingContract", Type = "address" },
        };

        public static readonly List<MemberDescription> RelayDataType = new()
        {
            new () { Name = "maxFeePerGas", Type = "uint256" },
            new () { Name = "maxPriorityFeePerGas", Type = "uint256" },
            new () { Name = "transactionCalldataGasUsed", Type = "uint256" },
            new () { Name = "relayWorker", Type = "address" },
            new () { Name = "paymaster", Type = "address" },
            new () { Name = "forwarder", Type = "address" },
            new () { Name = "paymasterData", Type = "bytes" },
            new () { Name = "clientId", Type = "uint256" },
        };

        public static readonly List<MemberDescription> ForwardRequestType = new()
        {
            new () { Name = "from", Type = "address" },
            new () { Name = "to", Type = "address" },
            new () { Name = "value", Type = "uint256" },
            new () { Name = "gas", Type = "uint256" },
            new () { Name = "nonce", Type = "uint256" },
            new () { Name = "data", Type = "bytes" },
            new () { Name = "validUntilTime", Type = "uint256" },
        };

        public static readonly List<MemberDescription> RelayRequestType = new(ForwardRequestType)
        {
            new () { Name = "relayData", Type = "RelayData" },
        };

        #endregion

        #region Public Methods

        public static MemberValue[] CreateMessage(GsnRelayRequest relayRequest)
        {
            return new MemberValue[] {
                new() { TypeName = "address", Value = relayRequest.Request.From.ToLowerInvariant() },
                new() { TypeName = "address", Value = relayRequest.Request.To.ToLowerInvariant() },
                new() { TypeName = "uint256", Value = relayRequest.Request.Value },
                new() { TypeName = "uint256", Value = relayRequest.Request.Gas },
                new() { TypeName = "uint256", Value = relayRequest.Request.Nonce },
                new() { TypeName = "bytes", Value = relayRequest.Request.Data },
                new() { TypeName = "uint256", Value = relayRequest.Request.ValidUntilTime },
                new() { TypeName = "RelayData", Value = relayRequest.RelayData.ToEip712Values() }
            };
        }

        #endregion

    }

}