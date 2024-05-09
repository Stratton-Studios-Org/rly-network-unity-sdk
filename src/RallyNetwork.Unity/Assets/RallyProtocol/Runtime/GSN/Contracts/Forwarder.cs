using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;

using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;

namespace RallyProtocol.GSN.Contracts
{

    public partial class ForwarderDeployment : ForwarderDeploymentBase
    {
        public ForwarderDeployment() : base(BYTECODE) { }
        public ForwarderDeployment(string byteCode) : base(byteCode) { }
    }

    public class ForwarderDeploymentBase : ContractDeploymentMessage
    {
        public static string BYTECODE = "";
        public ForwarderDeploymentBase() : base(BYTECODE) { }
        public ForwarderDeploymentBase(string byteCode) : base(byteCode) { }

    }

    public partial class SupportsInterfaceFunction : SupportsInterfaceFunctionBase { }

    [Function("supportsInterface", "bool")]
    public class SupportsInterfaceFunctionBase : FunctionMessage
    {
        [Parameter("bytes4", "interfaceId", 1)]
        public virtual byte[] InterfaceId { get; set; }
    }

    public partial class GetNonceFunction : GetNonceFunctionBase { }

    [Function("getNonce", "uint256")]
    public class GetNonceFunctionBase : FunctionMessage
    {
        [Parameter("address", "from", 1)]
        public virtual string From { get; set; }
    }

    public partial class VerifyFunction : VerifyFunctionBase { }

    [Function("verify")]
    public class VerifyFunctionBase : FunctionMessage
    {
        [Parameter("tuple", "forwardRequest", 1)]
        public virtual ForwardRequest ForwardRequest { get; set; }
        [Parameter("bytes32", "domainSeparator", 2)]
        public virtual byte[] DomainSeparator { get; set; }
        [Parameter("bytes32", "requestTypeHash", 3)]
        public virtual byte[] RequestTypeHash { get; set; }
        [Parameter("bytes", "suffixData", 4)]
        public virtual byte[] SuffixData { get; set; }
        [Parameter("bytes", "signature", 5)]
        public virtual byte[] Signature { get; set; }
    }

    public partial class ExecuteFunction : ExecuteFunctionBase { }

    [Function("execute", typeof(ExecuteOutputDTO))]
    public class ExecuteFunctionBase : FunctionMessage
    {
        [Parameter("tuple", "forwardRequest", 1)]
        public virtual ForwardRequest ForwardRequest { get; set; }
        [Parameter("bytes32", "domainSeparator", 2)]
        public virtual byte[] DomainSeparator { get; set; }
        [Parameter("bytes32", "requestTypeHash", 3)]
        public virtual byte[] RequestTypeHash { get; set; }
        [Parameter("bytes", "suffixData", 4)]
        public virtual byte[] SuffixData { get; set; }
        [Parameter("bytes", "signature", 5)]
        public virtual byte[] Signature { get; set; }
    }

    public partial class RegisterRequestTypeFunction : RegisterRequestTypeFunctionBase { }

    [Function("registerRequestType")]
    public class RegisterRequestTypeFunctionBase : FunctionMessage
    {
        [Parameter("string", "typeName", 1)]
        public virtual string TypeName { get; set; }
        [Parameter("string", "typeSuffix", 2)]
        public virtual string TypeSuffix { get; set; }
    }

    public partial class RegisterDomainSeparatorFunction : RegisterDomainSeparatorFunctionBase { }

    [Function("registerDomainSeparator")]
    public class RegisterDomainSeparatorFunctionBase : FunctionMessage
    {
        [Parameter("string", "name", 1)]
        public virtual string Name { get; set; }
        [Parameter("string", "version", 2)]
        public virtual string Version { get; set; }
    }

    public partial class DomainRegisteredEventDTO : DomainRegisteredEventDTOBase { }

    [Event("DomainRegistered")]
    public class DomainRegisteredEventDTOBase : IEventDTO
    {
        [Parameter("bytes32", "domainSeparator", 1, true)]
        public virtual byte[] DomainSeparator { get; set; }
        [Parameter("bytes", "domainValue", 2, false)]
        public virtual byte[] DomainValue { get; set; }
    }

    public partial class RequestTypeRegisteredEventDTO : RequestTypeRegisteredEventDTOBase { }

    [Event("RequestTypeRegistered")]
    public class RequestTypeRegisteredEventDTOBase : IEventDTO
    {
        [Parameter("bytes32", "typeHash", 1, true)]
        public virtual byte[] TypeHash { get; set; }
        [Parameter("string", "typeStr", 2, false)]
        public virtual string TypeStr { get; set; }
    }

    public partial class SupportsInterfaceOutputDTO : SupportsInterfaceOutputDTOBase { }

    [FunctionOutput]
    public class SupportsInterfaceOutputDTOBase : IFunctionOutputDTO
    {
        [Parameter("bool", "", 1)]
        public virtual bool ReturnValue1 { get; set; }
    }

    public partial class GetNonceOutputDTO : GetNonceOutputDTOBase { }

    [FunctionOutput]
    public class GetNonceOutputDTOBase : IFunctionOutputDTO
    {
        [Parameter("uint256", "", 1)]
        public virtual BigInteger ReturnValue1 { get; set; }
    }

    public partial class ExecuteOutputDTO : ExecuteOutputDTOBase { }

    [FunctionOutput]
    public class ExecuteOutputDTOBase : IFunctionOutputDTO
    {
        [Parameter("bool", "success", 1)]
        public virtual bool Success { get; set; }
        [Parameter("bytes", "ret", 2)]
        public virtual byte[] Ret { get; set; }
    }

    public partial class ForwardRequest : ForwardRequestBase { }

    public class ForwardRequestBase
    {
        [Parameter("address", "from", 1)]
        public virtual string From { get; set; } = string.Empty;
        [Parameter("address", "to", 2)]
        public virtual string To { get; set; } = string.Empty;
        [Parameter("uint256", "value", 3)]
        public virtual BigInteger Value { get; set; }
        [Parameter("uint256", "gas", 4)]
        public virtual BigInteger Gas { get; set; }
        [Parameter("uint256", "nonce", 5)]
        public virtual BigInteger Nonce { get; set; }
        [Parameter("bytes", "data", 6)]
        public virtual byte[] Data { get; set; } = Array.Empty<byte>();
        [Parameter("uint256", "validUntilTime", 7)]
        public virtual BigInteger ValidUntilTime { get; set; }
    }

}
