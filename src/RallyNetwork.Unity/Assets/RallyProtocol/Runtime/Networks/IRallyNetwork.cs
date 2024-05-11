using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;

using Nethereum.Web3;
using Nethereum.Web3.Accounts;

using RallyProtocol.Accounts;
using RallyProtocol.GSN;
using RallyProtocol.Logging;

using UnityEngine;

namespace RallyProtocol.Networks
{

    public enum RallyNetworkType
    {
        Amoy,
        Polygon,
        Local,
        Test,
        Custom
    }

    public interface IRallyNetwork
    {

        public IRallyAccountManager AccountManager { get; }
        public IRallyLogger Logger { get; }
        public IRallyHttpHandler HttpHandler { get; }
        public IGsnClient GsnClient { get; }

        public Task<Web3> GetProviderAsync();
        public Task<Account> GetAccountAsync();
        public Task<decimal> GetDisplayBalanceAsync(string tokenAddress = null);
        public Task<BigInteger> GetExactBalanceAsync(string tokenAddress = null);
        public Task<string> TransferAsync(string destinationAddress, decimal amount, MetaTxMethod metaTxMethod, string tokenAddress = null);
        public Task<string> TransferExactAsync(string destinationAddress, BigInteger amount, MetaTxMethod? metaTxMethod, string tokenAddress = null);
        public Task<string> ClaimRlyAsync();
        public Task<string> RelayAsync(GsnTransactionDetails tx);
        public void SetApiKey(string apiKey);

    }

}