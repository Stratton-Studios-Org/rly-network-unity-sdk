using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;

using Nethereum.Web3;
using Nethereum.Web3.Accounts;

using RallyProtocol.Accounts;
using RallyProtocol.Core;
using RallyProtocol.GSN;
using RallyProtocol.GSN.Models;
using RallyProtocol.Logging;

namespace RallyProtocol.Networks
{

    public enum RallyNetworkType
    {
        Amoy,
        Polygon,
        Local,
        Test,
        Custom,
        BaseSepolia,
        Base
    }

    public interface IRallyNetwork
    {

        #region Proerpties

        public IRallyAccountManager AccountManager { get; }
        public IRallyLogger Logger { get; }
        public IRallyHttpHandler HttpHandler { get; }
        public IGsnClient GsnClient { get; }

        #endregion

        #region Public Methods

        public Task<Web3> GetProviderAsync();
        public Task<Account> GetAccountAsync();
        public Task<decimal> GetDisplayBalanceAsync(string? tokenAddress = null);
        public Task<BigInteger> GetExactBalanceAsync(string? tokenAddress = null);
        public Task<string> TransferAsync(string destinationAddress, decimal amount, MetaTxMethod? metaTxMethod = null, string? tokenAddress = null, TokenConfig tokenConfig = null);
        public Task<string> TransferExactAsync(string destinationAddress, BigInteger amount, MetaTxMethod? metaTxMethod = null, string? tokenAddress = null, TokenConfig tokenConfig = null);
        public Task<string> ClaimRlyAsync();
        public Task<string> RelayAsync(GsnTransactionDetails tx);
        public void SetApiKey(string apiKey);

        #endregion

    }

}