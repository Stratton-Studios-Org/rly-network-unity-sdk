using System.Collections;
using System.Collections.Generic;

using Nethereum.ABI.EIP712;
using Nethereum.ABI.FunctionEncoding.Attributes;

namespace RallyProtocol.GSN.Models
{

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

}