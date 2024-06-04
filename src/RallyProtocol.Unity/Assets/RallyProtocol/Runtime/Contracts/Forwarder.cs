using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;

using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;

namespace RallyProtocol.Contracts.Forwarder
{

    public partial class GetNonceFunction : GetNonceFunctionBase { }

    [Function("getNonce", "uint256")]
    public class GetNonceFunctionBase : FunctionMessage
    {
        [Parameter("address", "from", 1)]
        public virtual string From { get; set; }
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
