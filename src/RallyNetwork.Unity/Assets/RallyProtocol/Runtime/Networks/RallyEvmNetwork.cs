using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;

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

using RallyProtocol.Accounts;
using RallyProtocol.Contracts;
using RallyProtocol.Core;
using RallyProtocol.GSN;
using RallyProtocol.Logging;

namespace RallyProtocol.Networks
{

    public class RallyEvmNetwork : IRallyNetwork
    {

        #region Fields

        protected IRallyWeb3Provider web3Provider;
        protected IRallyHttpHandler httpHandler;
        protected IRallyLogger logger;
        protected IRallyAccountManager accountManager;
        protected RallyNetworkConfig config;
        protected GsnClient gsnClient;

        protected PermitTransaction permitTransaction;
        protected MetaTransaction metaTransaction;

        #endregion

        #region Constructors

        public RallyEvmNetwork(IRallyWeb3Provider web3Provider, IRallyHttpHandler httpHandler, IRallyLogger logger, IRallyAccountManager accountManager, RallyNetworkConfig config)
        {
            this.web3Provider = web3Provider;
            this.httpHandler = httpHandler;
            this.logger = logger;
            this.accountManager = accountManager;
            this.config = config;
            this.gsnClient = new(web3Provider, httpHandler, logger);

            this.permitTransaction = new(this.logger, this.gsnClient.TransactionHelper);
            this.metaTransaction = new(this.logger, this.gsnClient.TransactionHelper);
        }

        #endregion

        #region Private Methods

        private string GetTokenAddress(string tokenAddress)
        {
            return string.IsNullOrEmpty(tokenAddress) ? this.config.Contracts.RlyERC20 : tokenAddress;
        }

        #endregion

        #region Public Methods

        public Web3 GetProvider()
        {
            return web3Provider.GetWeb3(this.config);
        }

        public async Task<Account> GetAccountAsync()
        {
            Account account = await this.accountManager.GetAccountAsync();
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
                    transferTx = await this.permitTransaction.GetPermitTx(account, destinationAddress, amount, this.config, tokenAddress, provider);
                }
                else if (metaTxMethod == MetaTxMethod.ExecuteMetaTransaction)
                {
                    transferTx = await this.metaTransaction.GetExecuteMetaTransactionTx(account, destinationAddress, amount, this.config, tokenAddress, provider);
                }
            }
            else
            {
                bool executeMetaTransactionSupported = await this.metaTransaction.HasExecuteMetaTransaction(account, destinationAddress, amount, this.config, tokenAddress, provider);

                bool permitSupported = await this.permitTransaction.HasPermit(account, amount, this.config, tokenAddress, provider);

                if (executeMetaTransactionSupported)
                {
                    transferTx = await this.metaTransaction.GetExecuteMetaTransactionTx(account, destinationAddress, amount, this.config, tokenAddress, provider);
                }
                else if (permitSupported)
                {
                    transferTx = await this.permitTransaction.GetPermitTx(account, destinationAddress, amount, this.config, tokenAddress, provider);
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

        #endregion

    }

}