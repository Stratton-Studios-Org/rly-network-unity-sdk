using System.Collections;
using System.Collections.Generic;
using System.Numerics;

using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;

namespace RallyProtocol.GSN
{

    public partial class PermitFunction : PermitFunctionBase { }

    [Function("permit", "bool")]
    public class PermitFunctionBase : FunctionMessage
    {
        [Parameter("address", "owner", 1)]
        public virtual string Owner { get; set; }
        [Parameter("address", "spender", 2)]
        public virtual string Spender { get; set; }
        [Parameter("uint256", "value", 3)]
        public virtual BigInteger Value { get; set; }
        [Parameter("uint256", "deadline", 4)]
        public virtual BigInteger Deadline { get; set; }
        [Parameter("uint8", "v", 5)]
        public virtual byte V { get; set; }
        [Parameter("bytes32", "r", 6)]
        public virtual byte[] R { get; set; }
        [Parameter("bytes32", "s", 7)]
        public virtual byte[] S { get; set; }
    }

    public partial class TransferFunction : TransferFunctionBase { }

    [Function("transfer", "bool")]
    public class TransferFunctionBase : FunctionMessage
    {
        [Parameter("address", "recipient", 1)]
        public virtual string Recipient { get; set; }
        [Parameter("uint256", "amount", 2)]
        public virtual BigInteger Amount { get; set; }
    }

    public partial class TransferFromFunction : TransferFromFunctionBase { }

    [Function("transferFrom", "bool")]
    public class TransferFromFunctionBase : FunctionMessage
    {
        [Parameter("address", "sender", 1)]
        public virtual string Sender { get; set; }
        [Parameter("address", "recipient", 2)]
        public virtual string Recipient { get; set; }
        [Parameter("uint256", "amount", 3)]
        public virtual BigInteger Amount { get; set; }
    }

    public partial class ExecuteMetaTransactionFunction : ExecuteMetaTransactionFunctionBase { }

    [Function("executeMetaTransaction", "bytes")]
    public class ExecuteMetaTransactionFunctionBase : FunctionMessage
    {
        [Parameter("address", "userAddress", 1)]
        public virtual string UserAddress { get; set; }
        [Parameter("bytes", "functionSignature", 2)]
        public virtual byte[] FunctionSignature { get; set; }
        [Parameter("bytes32", "sigR", 3)]
        public virtual byte[] SigR { get; set; }
        [Parameter("bytes32", "sigS", 4)]
        public virtual byte[] SigS { get; set; }
        [Parameter("uint8", "sigV", 5)]
        public virtual byte SigV { get; set; }
    }

    public partial class Eip712DomainFunction : Eip712DomainFunctionBase { }

    [Function("eip712Domain", typeof(Eip712DomainOutputDTO))]
    public class Eip712DomainFunctionBase : FunctionMessage
    {

    }

    public partial class ERC20GetNonceFunction : ERC20GetNonceFunctionBase { }

    [Function("getNonce", "uint256")]
    public class ERC20GetNonceFunctionBase : FunctionMessage
    {
        [Parameter("address", "user", 1)]
        public virtual string User { get; set; }
    }

    public partial class Eip712DomainOutputDTO : Eip712DomainOutputDTOBase { }

    [FunctionOutput]
    public class Eip712DomainOutputDTOBase : IFunctionOutputDTO
    {
        [Parameter("bytes1", "fields", 1)]
        public virtual byte[] Fields { get; set; }
        [Parameter("string", "name", 2)]
        public virtual string Name { get; set; }
        [Parameter("string", "version", 3)]
        public virtual string Version { get; set; }
        [Parameter("uint256", "chainId", 4)]
        public virtual BigInteger ChainId { get; set; }
        [Parameter("address", "verifyingContract", 5)]
        public virtual string VerifyingContract { get; set; }
        [Parameter("bytes32", "salt", 6)]
        public virtual byte[] Salt { get; set; }
        //[Parameter("uint256[]", "extensions", 7)]
        //public virtual List<BigInteger> Extensions { get; set; }
    }

}