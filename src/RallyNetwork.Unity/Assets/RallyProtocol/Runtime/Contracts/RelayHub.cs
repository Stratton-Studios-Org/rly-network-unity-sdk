using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;

using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;

using Newtonsoft.Json;

using RallyProtocol.Contracts.Forwarder;

namespace RallyProtocol.Contracts.RelayHub
{

    public partial class RelayCallFunction : RelayCallFunctionBase { }

    [Function("relayCall", typeof(RelayCallOutputDTO))]
    public class RelayCallFunctionBase : FunctionMessage
    {
        [Parameter("string", "domainSeparatorName", 1)]
        public virtual string DomainSeparatorName { get; set; }
        [Parameter("uint256", "maxAcceptanceBudget", 2)]
        public virtual BigInteger MaxAcceptanceBudget { get; set; }
        [Parameter("tuple", "relayRequest", 3)]
        public virtual RelayRequest RelayRequest { get; set; }
        [Parameter("bytes", "signature", 4)]
        public virtual byte[] Signature { get; set; }
        [Parameter("bytes", "approvalData", 5)]
        public virtual byte[] ApprovalData { get; set; }
    }

    public partial class RelayCallOutputDTO : RelayCallOutputDTOBase { }

    [FunctionOutput]
    public class RelayCallOutputDTOBase : IFunctionOutputDTO
    {
        [Parameter("bool", "paymasterAccepted", 1)]
        public virtual bool PaymasterAccepted { get; set; }
        [Parameter("uint256", "charge", 2)]
        public virtual BigInteger Charge { get; set; }
        [Parameter("uint8", "status", 3)]
        public virtual byte Status { get; set; }
        [Parameter("bytes", "returnValue", 4)]
        public virtual byte[] ReturnValue { get; set; }
    }

    public partial class RelayData : RelayDataBase { }

    public class RelayDataBase
    {
        [Parameter("uint256", "maxFeePerGas", 1)]
        public virtual BigInteger MaxFeePerGas { get; set; }
        [Parameter("uint256", "maxPriorityFeePerGas", 2)]
        public virtual BigInteger MaxPriorityFeePerGas { get; set; }
        [Parameter("uint256", "transactionCalldataGasUsed", 3)]
        public virtual BigInteger TransactionCalldataGasUsed { get; set; }
        [Parameter("address", "relayWorker", 4)]
        public virtual string RelayWorker { get; set; } = string.Empty;
        [Parameter("address", "paymaster", 5)]
        public virtual string Paymaster { get; set; } = string.Empty;
        [Parameter("address", "forwarder", 6)]
        public virtual string Forwarder { get; set; } = string.Empty;
        [Parameter("bytes", "paymasterData", 7)]
        public virtual byte[] PaymasterData { get; set; } = Array.Empty<byte>();
        [Parameter("uint256", "clientId", 8)]
        public virtual BigInteger ClientId { get; set; }
    }

    public partial class RelayRequest : RelayRequestBase { }

    public class RelayRequestBase
    {
        [Parameter("tuple", "request", 1)]
        public virtual ForwardRequest Request { get; set; } = new ForwardRequest();
        [Parameter("tuple", "relayData", 2)]
        public virtual RelayData RelayData { get; set; } = new RelayData();
    }

}