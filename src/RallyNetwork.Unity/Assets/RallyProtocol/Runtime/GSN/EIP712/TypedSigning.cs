using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;

using Nethereum.ABI.EIP712;
using Nethereum.ABI.FunctionEncoding.Attributes;

using Newtonsoft.Json;

using RallyProtocol.GSN.Contracts;

using UnityEngine;

namespace RallyProtocol.GSN
{

    public class MessageTypeProperty
    {

        [JsonProperty("name")]
        public string Name;

        [JsonProperty("type")]
        public string Type;

    }

    //public class EIP712Domain
    //{

    //    [JsonProperty("name")]
    //    public string Name;

    //    [JsonProperty("version")]
    //    public string Version;

    //    [JsonProperty("chainId")]
    //    public int ChainId;

    //    [JsonProperty("verifyingContract")]
    //    public string VerifyingContract;

    //}

    [Struct("EIP712Domain")]
    public class DomainWithChainIdString : IDomain
    {
        [Parameter("string", "name", 1)]
        public virtual string Name { get; set; } = string.Empty;

        [Parameter("string", "version", 2)]
        public virtual string Version { get; set; } = string.Empty;

        [Parameter("uint256", "chainId", 3)]
        public virtual string ChainId { get; set; } = string.Empty;

        [Parameter("address", "verifyingContract", 4)]
        public virtual string VerifyingContract { get; set; } = string.Empty;
    }

    [Struct("EIP712Domain")]
    public class DomainWithoutChainIdButSalt : IDomain
    {
        [Parameter("string", "name", 1)]
        public virtual string Name { get; set; } = string.Empty;

        [Parameter("string", "version", 2)]
        public virtual string Version { get; set; } = string.Empty;

        [Parameter("address", "verifyingContract", 3)]
        public virtual string VerifyingContract { get; set; } = string.Empty;

        [Parameter("bytes32", "salt", 4)]
        public virtual byte[] Salt { get; set; } = Array.Empty<byte>();
    }

    [Struct("EIP712Domain")]
    public class EIP712Domain : IDomain
    {
        [Parameter("string", "name", 1)]
        public virtual string Name { get; set; }

        [Parameter("string", "version", 2)]
        public virtual string Version { get; set; }

        [Parameter("uint256", "chainId", 3)]
        public virtual BigInteger? ChainId { get; set; }

        [Parameter("address", "verifyingContract", 4)]
        public virtual string VerifyingContract { get; set; }

        [Parameter("string", "salt", 5)]
        public virtual string Salt { get; set; }
    }

    public class GsnPrimaryType
    {

        public List<MessageTypeProperty> RelayRequest;
        public List<MessageTypeProperty> RelayData;

    }

    public class GsnRequestMessage
    {

        [Parameter("address", "from", 1)]
        public virtual string From { get; set; }
        [Parameter("address", "to", 2)]
        public virtual string To { get; set; }
        [Parameter("uint256", "value", 3)]
        public virtual BigInteger Value { get; set; }
        [Parameter("uint256", "gas", 4)]
        public virtual BigInteger Gas { get; set; }
        [Parameter("uint256", "nonce", 5)]
        public virtual BigInteger Nonce { get; set; }
        [Parameter("bytes", "data", 6)]
        public virtual byte[] Data { get; set; }
        [Parameter("uint256", "validUntilTime", 7)]
        public virtual BigInteger ValidUntilTime { get; set; }
        [Parameter("tuple", "relayData", 8, "RelayData")]
        public virtual RelayData RelayData { get; set; }

        public GsnRequestMessage() { }

        public GsnRequestMessage(ForwardRequest request, RelayData relayData)
        {
            From = request.From;
            To = request.To;
            Value = request.Value;
            Gas = request.Gas;
            Nonce = request.Nonce;
            Data = request.Data;
            ValidUntilTime = request.ValidUntilTime;
            RelayData = relayData;
        }

    }

    public class TypedGsnRequestData
    {

        public const string PrimaryType = "RelayRequest";

        public const string Version = "3";
        public const string GsnDomainSeparatorPrefix = "string name,string version";

        public static readonly List<MessageTypeProperty> RelayDataType = new()
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

        public static readonly List<MessageTypeProperty> ForwardRequestType = new()
        {
            new () { Name = "from", Type = "address" },
            new () { Name = "to", Type = "address" },
            new () { Name = "value", Type = "uint256" },
            new () { Name = "gas", Type = "uint256" },
            new () { Name = "nonce", Type = "uint256" },
            new () { Name = "data", Type = "bytes" },
            new () { Name = "validUntilTime", Type = "uint256" },
        };

        public static readonly List<MessageTypeProperty> RelayRequestType = new(ForwardRequestType)
        {
            new () { Name = "relayData", Type = "RelayData" },
        };

        public static readonly List<MemberDescription> RelayDataType2 = new()
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

        public static readonly List<MemberDescription> ForwardRequestType2 = new()
        {
            new () { Name = "from", Type = "address" },
            new () { Name = "to", Type = "address" },
            new () { Name = "value", Type = "uint256" },
            new () { Name = "gas", Type = "uint256" },
            new () { Name = "nonce", Type = "uint256" },
            new () { Name = "data", Type = "bytes" },
            new () { Name = "validUntilTime", Type = "uint256" },
        };

        public static readonly List<MemberDescription> RelayRequestType2 = new(ForwardRequestType2)
        {
            new () { Name = "relayData", Type = "RelayData" },
        };

        static TypedGsnRequestData()
        {

        }

        public readonly GsnPrimaryType Types;
        //public readonly EIP712Domain Domain;
        public readonly GsnRequestMessage Message;

        public TypedGsnRequestData(string name, int chainId, string verifier, RelayRequest relayRequest)
        {
            //this.Types = new()
            //{
            //    RelayRequest = RelayRequestType,
            //    RelayData = RelayDataType
            //};
            //this.Domain = GetDomainSeparator(name, verifier, chainId);
            //this.PrimaryType = "RelayRequest";
            //// in the signature, all "request" fields are flattened out at the top structure.
            //// other params are inside "relayData" sub-type
            //this.Message = new(relayRequest.Request, relayRequest.RelayData);
        }

        //private static EIP712Domain GetDomainSeparator(string name, string address, int chainId)
        //{
        //    return new()
        //    {
        //        Name = name,
        //        Version = GsnDomainSeparatorVersion,
        //        ChainId = chainId,
        //        VerifyingContract = address
        //    };
        //}

    }

}