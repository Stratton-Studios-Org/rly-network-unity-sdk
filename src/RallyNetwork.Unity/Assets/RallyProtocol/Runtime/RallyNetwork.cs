using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;

using Nethereum.ABI;
using Nethereum.ABI.EIP712.EIP2612;
using Nethereum.ABI.Model;
using Nethereum.Contracts;
using Nethereum.Contracts.ContractHandlers;
using Nethereum.Contracts.Standards.ERC20;
using Nethereum.Contracts.Standards.ERC20.ContractDefinition;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;

using RallyProtocol.Contracts;
using RallyProtocol.GSN;

using UnityEngine;

namespace RallyProtocol
{

    public enum RallyNetworkType
    {
        Local,
        Mumbai,
        Polygon,
        Test
    }

    public interface IRallyNetwork
    {

        public Task<decimal> GetDisplayBalance(string tokenAddress = null);
        public Task<BigInteger> GetExactBalance(string tokenAddress = null);
        public Task<string> Transfer(string destinationAddress, decimal amount, MetaTxMethod metaTxMethod, string tokenAddress = null);
        public Task<string> TransferExact(string destinationAddress, BigInteger amount, MetaTxMethod? metaTxMethod, string tokenAddress = null);
        public Task<string> ClaimRly();
        public Task<string> Relay(GsnTransactionDetails tx);
        public void SetApiKey(string apiKey);

    }

    public class RallyNetworkFactory
    {

        public static IRallyNetwork Create(RallyNetworkType type)
        {
            switch (type)
            {
                default:
                case RallyNetworkType.Local:
                    return Create(RallyNetworkConfig.Local);
                case RallyNetworkType.Mumbai:
                    return Create(RallyNetworkConfig.Mumbai);
                case RallyNetworkType.Polygon:
                    return Create(RallyNetworkConfig.Polygon);
                case RallyNetworkType.Test:
                    return Create(RallyNetworkConfig.Test);
            }
        }

        public static IRallyNetwork Create(RallyNetworkConfig config)
        {
            return new RallyNetwork(config);
        }

    }

    public class RallyNetwork : IRallyNetwork
    {

        protected RallyNetworkConfig config;

        public RallyNetwork(RallyNetworkConfig config)
        {
            this.config = config;
        }

        public Web3 GetProvider()
        {
            return new Web3(this.config.Gsn.RpcUrl, authenticationHeader: new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", this.config.RelayerApiKey ?? ""));
        }

        public async Task<Account> GetAccountAsync()
        {
            Account account = await WalletManager.Default.GetAccountAsync();
            if (account == null)
            {
                throw new MissingWalletException();
            }

            return account;
        }

        public async Task<string> Transfer(string destinationAddress, decimal amount, MetaTxMethod metaTxMethod, string tokenAddress = null)
        {
            Web3 provider = GetProvider();
            Account account = await GetAccountAsync();
            tokenAddress = GetTokenAddress(tokenAddress);
            ERC20ContractService token = new(provider.Eth, tokenAddress);
            byte decimals = await token.DecimalsQueryAsync();
            BigInteger amountBigNum = Web3.Convert.ToWei(amount, decimals);
            return await TransferExact(destinationAddress, amountBigNum, metaTxMethod, tokenAddress);
        }

        public async Task<string> TransferExact(string destinationAddress, BigInteger amount, MetaTxMethod? metaTxMethod = null, string tokenAddress = null)
        {
            Web3 provider = GetProvider();
            Account account = await GetAccountAsync();
            tokenAddress = GetTokenAddress(tokenAddress);
            BigInteger sourceBalance = await GetExactBalance(tokenAddress);
            BigInteger sourceFinalBalance = sourceBalance - amount;
            if (sourceFinalBalance < 0)
            {
                throw new InsufficientBalanceException();
            }

            GsnTransactionDetails transferTx = null;
            if (metaTxMethod != null && metaTxMethod == MetaTxMethod.Permit || metaTxMethod == MetaTxMethod.ExecuteMetaTransaction)
            {
                if (metaTxMethod == MetaTxMethod.Permit)
                {
                    transferTx = await PermitTransaction.GetPermitTx(account, destinationAddress, amount, config, tokenAddress, provider);
                }
                else if (metaTxMethod == MetaTxMethod.ExecuteMetaTransaction)
                {
                    transferTx = await MetaTransaction.GetExecuteMetaTransactionTx(account, destinationAddress, amount, config, tokenAddress, provider);
                }
            }
            else
            {
                bool executeMetaTransactionSupported = await MetaTransaction.HasExecuteMetaTransaction(account, destinationAddress, amount, config, tokenAddress, provider);

                bool permitSupported = await PermitTransaction.HasPermit(account, amount, config, tokenAddress, provider);

                if (executeMetaTransactionSupported)
                {
                    transferTx = await MetaTransaction.GetExecuteMetaTransactionTx(account, destinationAddress, amount, config, tokenAddress, provider);
                }
                else if (permitSupported)
                {
                    transferTx = await PermitTransaction.GetPermitTx(account, destinationAddress, amount, config, tokenAddress, provider);
                }
                else
                {
                    throw new TransferMethodNotSupportedException();
                }
            }

            return await Relay(transferTx);
        }

        public async Task<string> ClaimRly()
        {
            Account account = await GetAccountAsync();

            decimal balance = await GetDisplayBalance();
            if (balance < 0)
            {
                throw new PriorDustingException();
            }

            string contractAddress = this.config.Contracts.TokenFaucet;
            Web3 provider = GetProvider();
            IContractTransactionHandler<ClaimFunction> claimHandler = provider.Eth.GetContractTransactionHandler<ClaimFunction>();
            ClaimFunction claim = new()
            {
                FromAddress = account.Address,
            };
            TransactionInput input = await claimHandler.CreateTransactionInputEstimatingGasAsync(contractAddress, claim);
            GsnTransactionDetails gsnTx = new()
            {
                From = account.Address,
                Data = input.Data,
                Value = BigInteger.Zero,
                To = input.To,
                Gas = input.Gas.HexValue,
                MaxFeePerGas = input.MaxFeePerGas.HexValue,
                MaxPriorityFeePerGas = input.MaxPriorityFeePerGas.HexValue
            };

            return await Relay(gsnTx);
        }

        public async Task<decimal> GetDisplayBalance(string tokenAddress = null)
        {
            tokenAddress = GetTokenAddress(tokenAddress);
            Web3 provider = GetProvider();
            ERC20ContractService token = new(provider.Eth, tokenAddress);
            byte decimals = await token.DecimalsQueryAsync();
            BigInteger exactBalance = await GetExactBalance(tokenAddress);
            return Web3.Convert.FromWei(exactBalance, decimals);
        }

        public async Task<BigInteger> GetExactBalance(string tokenAddress = null)
        {
            Account account = await GetAccountAsync();
            tokenAddress = GetTokenAddress(tokenAddress);
            Web3 provider = GetProvider();
            ERC20ContractService token = new(provider.Eth, tokenAddress);
            BigInteger bal = await token.BalanceOfQueryAsync(account.Address);
            return bal;
        }

        public Task<string> Relay(GsnTransactionDetails tx)
        {
            throw new System.NotImplementedException();
        }

        public void SetApiKey(string apiKey)
        {
            config.RelayerApiKey = apiKey;
        }

        private string GetTokenAddress(string tokenAddress)
        {
            return string.IsNullOrEmpty(tokenAddress) ? config.Contracts.RlyERC20 : tokenAddress;
        }

    }

}