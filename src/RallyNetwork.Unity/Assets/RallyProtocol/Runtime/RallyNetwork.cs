using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;

using Nethereum.ABI;
using Nethereum.ABI.Model;
using Nethereum.Contracts;
using Nethereum.Contracts.ContractHandlers;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;

using RallyProtocol.Contracts;

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

        public Web3 GetRpcClient()
        {
            return new Web3(this.config.Gsn.RpcUrl, authenticationHeader: new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", this.config.RelayerApiKey ?? ""));
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

            string contractAddress = this.config.Contracts.TokenFaucet;
            Web3 web3 = GetRpcClient();
            IContractTransactionHandler<ClaimFunction> claimHandler = web3.Eth.GetContractTransactionHandler<ClaimFunction>();
            ClaimFunction claim = new()
            {
                FromAddress = account.Address,
            };
            TransactionInput input = await claimHandler.CreateTransactionInputEstimatingGasAsync(contractAddress, claim);
            GsnTransactionDetails gsnTx = new()
            {
                From = account.Address,
                Data = input.Data,
                Value = "0",
                To = input.To,
                Gas = input.Gas,
                MaxFeePerGas = input.MaxFeePerGas,
                MaxPriorityFeePerGas = input.MaxPriorityFeePerGas
            };

            return await Relay(gsnTx);
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