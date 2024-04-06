using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;

using Nethereum.Web3;
using Nethereum.Web3.Accounts;

using UnityEngine;

namespace RallyProtocol
{

    public enum RallyNetworkType
    {
        Local,
        Mumbai,
        Polygon,
        Custom
    }

    public interface IRallyNetwork
    {

        public Task<string> GetBalance(string tokenAddress = null, bool humanReadable = false);
        public Task<double> GetDisplayBalance(string tokenAddress = null);
        public Task<BigInteger> GetExactBalance(string tokenAddress = null);
        public Task<string> Transfer(string destinationAddress, double amount, MetaTxMethod metaTxMethod, string tokenAddress = null);
        public Task<string> TransferExact(string destinationAddress, BigInteger amount, MetaTxMethod metaTxMethod, string tokenAddress = null);
        public Task<string> SimpleTransfer(string destinationAddress, double amount, string tokenAddress = null, MetaTxMethod? metaTxMethod = null);
        public Task<string> ClaimRly();
        public Task<string> RegisterAccount();
        public Task<string> Relay(GsnTransactionDetails tx);
        public void SetApiKey(string apiKey);

    }

    public class RallyNetwork : IRallyNetwork
    {

        protected RallyNetworkConfig config;

        public RallyNetwork(RallyNetworkConfig config)
        {
            this.config = config;
        }

        public Task<GsnTransactionDetails> GetClaimTx(Account account, RallyNetworkConfig networkConfig, Web3 client)
        {

        }

        public Web3 GetEthClient(string apiUrl)
        {
            return new(apiUrl);
        }

        public async Task<string> ClaimRly()
        {
            Account account = await WalletManager.Default.GetAccount();
            if (account == null)
            {
                throw new System.Exception("Account does not exists");
            }

            BigInteger balance = await GetExactBalance();
            if (balance < BigInteger.Zero)
            {
                throw new System.Exception("Account already dusted, will not dust again");
            }

            Web3 client = GetEthClient(config.Gsn.RpcUrl);

        }

        public Task<string> GetBalance(string tokenAddress = null, bool humanReadable = false)
        {
            throw new System.NotImplementedException();
        }

        public Task<double> GetDisplayBalance(string tokenAddress = null)
        {
            throw new System.NotImplementedException();
        }

        public Task<BigInteger> GetExactBalance(string tokenAddress = null)
        {
            throw new System.NotImplementedException();
        }

        public Task<string> RegisterAccount()
        {
            throw new System.NotImplementedException();
        }

        public Task<string> Relay(GsnTransactionDetails tx)
        {
            throw new System.NotImplementedException();
        }

        public void SetApiKey(string apiKey)
        {
            throw new System.NotImplementedException();
        }

        public Task<string> SimpleTransfer(string destinationAddress, double amount, string tokenAddress = null, MetaTxMethod? metaTxMethod = null)
        {
            throw new System.NotImplementedException();
        }

        public Task<string> Transfer(string destinationAddress, double amount, MetaTxMethod metaTxMethod, string tokenAddress = null)
        {
            throw new System.NotImplementedException();
        }

        public Task<string> TransferExact(string destinationAddress, BigInteger amount, MetaTxMethod metaTxMethod, string tokenAddress = null)
        {
            throw new System.NotImplementedException();
        }
    }

}