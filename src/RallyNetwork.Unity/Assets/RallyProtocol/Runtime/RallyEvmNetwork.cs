using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;

using Cysharp.Threading.Tasks;

using Nethereum.ABI;
using Nethereum.ABI.EIP712.EIP2612;
using Nethereum.ABI.FunctionEncoding;
using Nethereum.ABI.Model;
using Nethereum.Contracts;
using Nethereum.Contracts.ContractHandlers;
using Nethereum.Contracts.Standards.ERC20;
using Nethereum.Contracts.Standards.ERC20.ContractDefinition;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.Hex.HexTypes;
using Nethereum.JsonRpc.Client;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.RPC.Fee1559Suggestions;
using Nethereum.Unity.Rpc;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;

using RallyProtocol.Contracts;
using RallyProtocol.GSN;

using UnityEngine;
using UnityEngine.Networking;

namespace RallyProtocol
{

    public enum RallyNetworkType
    {
        Local,
        Amoy,
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

        public static IRallyNetwork Create(RallyNetworkType type, string apiKey = null)
        {
            switch (type)
            {
                default:
                case RallyNetworkType.Local:
                    return Create(RallyNetworkConfig.Local, apiKey);
                case RallyNetworkType.Amoy:
                    return Create(RallyNetworkConfig.Amoy, apiKey);
                case RallyNetworkType.Polygon:
                    return Create(RallyNetworkConfig.Polygon, apiKey);
                case RallyNetworkType.Test:
                    return Create(RallyNetworkConfig.Test, apiKey);
            }
        }

        public static IRallyNetwork Create(RallyNetworkConfig config, string apiKey = null)
        {
            RallyEvmNetwork network = new(config);
            network.SetApiKey(apiKey);
            return network;
        }

    }

    public class RallyEvmNetwork : IRallyNetwork
    {

        protected RallyNetworkConfig config;
        protected GsnClient gsnClient;

        public RallyEvmNetwork(RallyNetworkConfig config)
        {
            this.config = config;
            this.gsnClient = new();
        }

        public Web3 GetProvider()
        {
            UnityWebRequestRpcTaskClient unityClient = new(new Uri(this.config.Gsn.RpcUrl), null, null);
            string apiKey = "";
            if (!string.IsNullOrEmpty(this.config.RelayerApiKey))
            {
                apiKey = UnityWebRequest.EscapeURL(config.RelayerApiKey);
                apiKey = this.config.RelayerApiKey;
            }

            unityClient.RequestHeaders["Authorization"] = $"Bearer {apiKey}";
            return new Web3(unityClient);
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
                    transferTx = await PermitTransaction.GetPermitTx(account, destinationAddress, amount, this.config, tokenAddress, provider);
                }
                else if (metaTxMethod == MetaTxMethod.ExecuteMetaTransaction)
                {
                    transferTx = await MetaTransaction.GetExecuteMetaTransactionTx(account, destinationAddress, amount, this.config, tokenAddress, provider);
                }
            }
            else
            {
                bool executeMetaTransactionSupported = await MetaTransaction.HasExecuteMetaTransaction(account, destinationAddress, amount, this.config, tokenAddress, provider);

                bool permitSupported = await PermitTransaction.HasPermit(account, amount, this.config, tokenAddress, provider);

                if (executeMetaTransactionSupported)
                {
                    transferTx = await MetaTransaction.GetExecuteMetaTransactionTx(account, destinationAddress, amount, this.config, tokenAddress, provider);
                }
                else if (permitSupported)
                {
                    transferTx = await PermitTransaction.GetPermitTx(account, destinationAddress, amount, this.config, tokenAddress, provider);
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
            ClaimFunction claimFunctionInput = new()
            {
                FromAddress = account.Address,
            };
            HexBigInteger estimatedGas = await provider.Eth.GetContractTransactionHandler<ClaimFunction>().EstimateGasAsync(this.config.Contracts.TokenFaucet, claimFunctionInput);

            BlockWithTransactions blockInformation = await provider.Eth.Blocks.GetBlockWithTransactionsByNumber.SendRequestAsync(BlockParameter.CreateLatest());
            BigInteger maxPriorityFeePerGas = BigInteger.Parse("1500000000");
            BigInteger maxFeePerGas = blockInformation.BaseFeePerGas.Value * 2 + maxPriorityFeePerGas;

            //Fee1559 fee = await provider.FeeSuggestion.GetTimePreferenceFeeSuggestionStrategy().SuggestFeeAsync();
            //string maxFeePerGas = string.Empty;
            //if (fee.MaxFeePerGas != null)
            //{
            //    maxFeePerGas = new HexBigInteger(fee.MaxFeePerGas.Value).HexValue;
            //}

            //string maxPriorityFeePerGas = string.Empty;
            //if (fee.MaxPriorityFeePerGas != null)
            //{
            //    maxPriorityFeePerGas = new HexBigInteger(fee.MaxPriorityFeePerGas.Value).HexValue;
            //}

            GsnTransactionDetails gsnTx = new()
            {
                From = account.Address,
                Data = claimFunctionInput.GetCallData().ToHex(true),
                To = this.config.Contracts.TokenFaucet,
                MaxFeePerGas = new HexBigInteger(maxFeePerGas).HexValue,
                MaxPriorityFeePerGas = new HexBigInteger(maxPriorityFeePerGas).HexValue,
                Value = "0",
                Gas = estimatedGas.HexValue,
            };

            return await Relay(gsnTx);
        }

        public async Task<decimal> GetDisplayBalance(string tokenAddress = null)
        {
            tokenAddress = GetTokenAddress(tokenAddress);
            Web3 provider = GetProvider();
            ERC20ContractService token = new(provider.Eth, tokenAddress);
            Account account = await GetAccountAsync();
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

        public async Task<string> Relay(GsnTransactionDetails tx)
        {
            Account account = await GetAccountAsync();
            return await this.gsnClient.RelayTransaction(account, this.config, tx);
        }

        public void SetApiKey(string apiKey)
        {
            this.config.RelayerApiKey = apiKey;
        }

        private string GetTokenAddress(string tokenAddress)
        {
            return string.IsNullOrEmpty(tokenAddress) ? this.config.Contracts.RlyERC20 : tokenAddress;
        }
        private static async Task<SmartContractRevertException> TryGetRevertMessage<TFunction>(
            Web3 web3, string contractAddress, TFunction functionArgs, BlockParameter blockParameter = null)
            where TFunction : FunctionMessage, new()
        {
            try
            {
                Console.WriteLine($"* Querying Function {typeof(TFunction).Name}");
                // instead of sending a transaction again, we do a query with the same function parameters
                // the smart contract code will be executed but no changes will be made on chain
                var functionHandler = web3.Eth.GetContractQueryHandler<TFunction>();
                // we're not bothered about the return value here
                // we'd only get that if it was successful
                // we only want the revert reason which we'll get from the exception
                // we cant use QueryRaw as that will never throw a SmartContractRevertException
                await functionHandler.QueryAsync<bool>(contractAddress, functionArgs, blockParameter);

                // if we got here there was no revert message to retrieve
                return null;
            }
            catch (SmartContractRevertException revertException)
            {
                return revertException;
            }
        }

    }

}