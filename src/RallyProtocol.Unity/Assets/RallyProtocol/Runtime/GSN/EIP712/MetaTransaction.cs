using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;

using Nethereum.ABI.EIP712;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;
using Nethereum.Contracts.Standards.ERC20;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.Hex.HexTypes;
using Nethereum.Model;
using Nethereum.RPC.Fee1559Suggestions;
using Nethereum.Signer;
using Nethereum.Signer.EIP712;
using Nethereum.Web3;

using RallyProtocol.Accounts;
using RallyProtocol.GSN.Models;
using RallyProtocol.Logging;
using RallyProtocol.Networks;
using RallyProtocol.Utilities;

using Account = Nethereum.Web3.Accounts.Account;

namespace RallyProtocol.GSN
{

    public class MetaTransaction
    {

        #region Fields

        protected IRallyLogger logger;
        protected GsnTransactionHelper transactionHelper;

        #endregion

        #region Properties

        public IRallyLogger Logger => this.logger;

        #endregion

        #region Constructors

        public MetaTransaction(IRallyLogger logger, GsnTransactionHelper transactionHelper)
        {
            this.logger = logger;
            this.transactionHelper = transactionHelper;
        }

        #endregion

        #region Public Methods

        public async Task<bool> HasExecuteMetaTransaction(Account account, string destinationAddress, BigInteger amount, RallyNetworkConfig config, string contractAddress, Web3 provider)
        {
            try
            {
                ERC20ContractService token = new(provider.Eth, contractAddress);
                string name = await token.NameQueryAsync();
                BigInteger nonce = await this.transactionHelper.GetSenderContractNonce(provider, contractAddress, account.Address);

                TransferFunction transferFunction = new()
                {
                    Recipient = destinationAddress,
                    Amount = amount,
                };
                string data = token.ContractHandler.GetFunction<TransferFunction>().GetData(transferFunction);
                ISignature signature = await GetMetaTransactionEIP712Signature(account, name, token.ContractAddress, data, config, nonce);

                ExecuteMetaTransactionFunction executeMetaTransactionFunction = new()
                {
                    UserAddress = account.Address,
                    FunctionSignature = data.HexToByteArray(),
                    SigR = signature.R,
                    SigS = signature.S,
                    SigV = signature.V[0],
                    FromAddress = account.Address
                };
                await token.ContractHandler.EstimateGasAsync(executeMetaTransactionFunction);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public TypedData<DomainWithoutChainIdButSalt> GetTypedMetaTransaction(string name, string version, byte[] salt, string verifyingContract, BigInteger nonce, string from, string functionSignature)
        {
            TypedData<DomainWithoutChainIdButSalt> typedData = new();
            typedData.Types = new Dictionary<string, MemberDescription[]>()
            {
                {
                    "EIP712Domain",
                    new MemberDescription[] {
                        new() { Name = "name", Type = "string" },
                        new() { Name = "version", Type = "string" },
                        new() { Name = "verifyingContract", Type = "address" },
                        new() { Name = "salt", Type = "bytes32" },
                    }
                },
                {
                    "MetaTransaction",
                    new MemberDescription[] {
                        new() { Name = "nonce", Type = "uint256" },
                        new() { Name = "from", Type = "address" },
                        new() { Name = "functionSignature", Type = "bytes" },
                    }
                }
            };
            typedData.PrimaryType = "MetaTransaction";
            typedData.Domain = new()
            {
                Name = name,
                Version = version,
                VerifyingContract = verifyingContract,
                Salt = salt
            };
            typedData.Message = new MemberValue[]
            {
                new() { TypeName = "uint256", Value = nonce },
                new() { TypeName = "address", Value = from },
                new() { TypeName = "bytes", Value = functionSignature }
            };

            return typedData;
        }

        public Task<ISignature> GetMetaTransactionEIP712Signature(Account account, string contractName, string contractAddress, string functionSignatue, RallyNetworkConfig config, BigInteger nonce)
        {

            // name and chainId to be used in EIP712
            BigInteger chainId = BigInteger.Parse(config.Gsn.ChainId);
            ;
            // typed data for signing
            TypedData<DomainWithoutChainIdButSalt> eip712Data = GetTypedMetaTransaction(contractName, "1", new HexBigInteger(chainId).HexValue.HexZeroPad(32).HexToByteArray(), contractAddress, nonce, account.Address, functionSignatue);

            //signature for metatransaction
            string signature = account.SignTypedDataV4(eip712Data);
            EthECDSASignature ethSignature = EthECDSASignatureFactory.ExtractECDSASignature(signature);
            return Task.FromResult<ISignature>(ethSignature);
        }

        public async Task<GsnTransactionDetails> GetExecuteMetaTransactionTx(Account account, string destinationAddress, BigInteger amount, RallyNetworkConfig config, string contractAddress, Web3 provider)
        {
            ERC20ContractService token = new(provider.Eth, contractAddress);
            string name = await token.NameQueryAsync();
            BigInteger nonce = await this.transactionHelper.GetSenderContractNonce(provider, contractAddress, account.Address);

            TransferFunction transferFunction = new()
            {
                Recipient = destinationAddress,
                Amount = amount,
            };
            string data = token.ContractHandler.GetFunction<TransferFunction>().GetData(transferFunction);
            ISignature signature = await GetMetaTransactionEIP712Signature(account, name, token.ContractAddress, data, config, nonce);
            ExecuteMetaTransactionFunction executeMetaTransactionFunction = new()
            {
                UserAddress = account.Address,
                FunctionSignature = data.HexToByteArray(),
                SigR = signature.R,
                SigS = signature.S,
                SigV = signature.V[0],
                FromAddress = account.Address,
            };
            HexBigInteger gas = await token.ContractHandler.EstimateGasAsync(executeMetaTransactionFunction);
            string executeMetaTransactionFunctionData = token.ContractHandler.GetFunction<ExecuteMetaTransactionFunction>().GetData(executeMetaTransactionFunction);
            //Fee1559 fee = await provider.FeeSuggestion.GetSimpleFeeSuggestionStrategy().SuggestFeeAsync();
            FeeData feeData = await FeeData.GetFeeData(provider);

            GsnTransactionDetails gsnTx = new()
            {
                From = account.Address,
                Data = executeMetaTransactionFunctionData,
                Value = "0",
                To = token.ContractAddress,
                Gas = gas.HexValue,
                MaxFeePerGas = new HexBigInteger(feeData.MaxFeePerGas).HexValue,
                MaxPriorityFeePerGas = new HexBigInteger(feeData.MaxPriorityFeePerGas).HexValue
            };

            return gsnTx;
        }

        #endregion

        #region Contract Definition

        public partial class TransferFunction : TransferFunctionBase { }

        [Function("transfer", "bool")]
        public class TransferFunctionBase : FunctionMessage
        {
            [Parameter("address", "recipient", 1)]
            public virtual string Recipient { get; set; }
            [Parameter("uint256", "amount", 2)]
            public virtual BigInteger Amount { get; set; }
        }

        public partial class ExecuteMetaTransactionFunction : ExecuteMetaTransactionFunctionBase { }

        [Function("executeMetaTransaction", "bytes")]
        public class ExecuteMetaTransactionFunctionBase : FunctionMessage
        {
            [Parameter("address", "userAddress", 1)]
            public virtual string UserAddress { get; set; }
            [Parameter("bytes", "functionSignature", 2)]
            public virtual byte[] FunctionSignature { get; set; }
            [Parameter("bytes32", "sigR", 3)]
            public virtual byte[] SigR { get; set; }
            [Parameter("bytes32", "sigS", 4)]
            public virtual byte[] SigS { get; set; }
            [Parameter("uint8", "sigV", 5)]
            public virtual byte SigV { get; set; }
        }

        #endregion

    }

}