using System;
using System.Collections;
using System.Collections.Generic;

using Nethereum.JsonRpc.Client;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;

using RallyProtocol.Networks;

namespace RallyProtocol.Core
{

    /// <summary>
    /// A web3/eth provider, implement this interface to customize the Web3 and RPC client.
    /// </summary>
    public interface IRallyWeb3Provider
    {

        /// <summary>
        /// Creates a new instance of <see cref="Web3"/> using the provided <see cref="RallyNetworkConfig"/>.
        /// </summary>
        /// <param name="config">The network configuration</param>
        /// <returns>Returns a new instance of <see cref="Web3"/></returns>
        public Web3 GetWeb3(RallyNetworkConfig config);

        /// <summary>
        /// Creates a new instance of <see cref="Web3"/> using the provided <see cref="RallyNetworkConfig"/> and <see cref="Account"/>.
        /// </summary>
        /// <param name="account">The Web3 account</param>
        /// <param name="config">The network configuration</param>
        /// <returns>Returns a new instance of <see cref="Web3"/></returns>
        public Web3 GetWeb3(Account account, RallyNetworkConfig config);

        /// <summary>
        /// Creates a new RPC client using the provided <see cref="RallyNetworkConfig"/>.
        /// </summary>
        /// <param name="config">The network configuration</param>
        /// <returns>Returns a new RPC client instance</returns>
        public IClient GetRpcClient(RallyNetworkConfig config);

    }

}