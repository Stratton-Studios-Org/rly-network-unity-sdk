using System.Collections;
using System.Collections.Generic;
using System.Numerics;

using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;

namespace RallyProtocol.Contracts.TokenFaucet
{

    public partial class ClaimFunction : ClaimFunctionBase { }

    [Function("claim", "bool")]
    public class ClaimFunctionBase : FunctionMessage
    {

    }

}