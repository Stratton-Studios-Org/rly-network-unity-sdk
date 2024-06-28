using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Nethereum.ABI.EIP712;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;
using Nethereum.Contracts.Standards.ERC20;
using Nethereum.Contracts.Standards.ERC721;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.Hex.HexTypes;
using Nethereum.Model;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Signer;
using Nethereum.Web3;

using RallyProtocol.Accounts;
using RallyProtocol.Logging;
using RallyProtocol.Networks;

using Account = Nethereum.Web3.Accounts.Account;

namespace RallyProtocol.GSN
{

    public class PermitTransaction
    {

        #region Constants

        public const string HashZero = "0x0000000000000000000000000000000000000000000000000000000000000000";

        #endregion

        #region Fields

        protected IRallyLogger logger;
        protected GsnTransactionHelper transactionHelper;

        #endregion

        #region Properties

        public IRallyLogger Logger => this.logger;

        #endregion

        #region Constructors

        public PermitTransaction(IRallyLogger logger, GsnTransactionHelper transactionHelper)
        {
            this.logger = logger;
            this.transactionHelper = transactionHelper;
        }

        #endregion

        #region Public Methods

        public async Task<bool> HasPermit(Account account, BigInteger amount, RallyNetworkConfig config, string contractAddress, Web3 provider)
        {
            try
            {
                ERC20ContractService token = new(provider.Eth, contractAddress);

                string name = await token.NameQueryAsync();
                BigInteger nonce = await this.transactionHelper.GetSenderContractNonce(provider, contractAddress, account.Address);
                BigInteger deadline = await GetPermitDeadline(provider);
                Eip712DomainOutputDTO eip712Domain = await token.ContractHandler.QueryAsync<Eip712DomainFunction, Eip712DomainOutputDTO>();

                byte[] salt = eip712Domain.Salt;

                ISignature signature = await GetPermitEIP712Signature(account, name, token.ContractAddress, config, nonce, amount, deadline, salt);
                PermitFunction permitFunction = new()
                {
                    Owner = account.Address,
                    Spender = config.Gsn.PaymasterAddress,
                    Value = amount,
                    Deadline = deadline,
                    V = signature.V[0],
                    R = signature.R,
                    S = signature.S,
                    FromAddress = account.Address
                };
                await token.ContractHandler.EstimateGasAsync(permitFunction);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public string GetSignedTypedPermitTransaction(Account account, string name, string version, BigInteger chainId, string verifyingContract, string owner, string spender, BigInteger value, BigInteger nonce, BigInteger deadline, byte[] salt)
        {
            const string primaryType = "Permit";
            bool hasSalt = salt != null && salt.Length > 0;
            if (hasSalt)
            {
                bool allZero = true;
                for (int i = 0; i < salt.Length; i++)
                {
                    allZero &= salt[i] == 0;
                }

                hasSalt = !allZero;
            }

            List<MemberDescription> domainType = new()
            {
                new() { Name = "name", Type = "string" },
                new() { Name = "version", Type = "string" },
                new() { Name = "chainId", Type = "uint256" },
                new() { Name = "verifyingContract", Type = "address" },
            };
            List<MemberValue> domainValues = new()
            {
                new() { TypeName = "name", Value = name },
                new() { TypeName = "version", Value = version },
                new() { TypeName = "verifyingContract", Value = verifyingContract },
                new() { TypeName = "chainId", Value = chainId }
            };

            if (hasSalt)
            {
                domainType.Add(new() { Name = "salt", Type = "bytes32" });
                domainValues.Add(new() { TypeName = "salt", Value = salt });
            }

            MemberValue[] message = new MemberValue[]
            {
                new() { TypeName = "address", Value = owner },
                new() { TypeName = "address", Value = spender },
                new() { TypeName = "uint256", Value = value },
                new() { TypeName = "uint256", Value = nonce },
                new() { TypeName = "uint256", Value = deadline }
            };
            Dictionary<string, MemberDescription[]> types = new()
            {
                {
                    "EIP712Domain",
                    domainType.ToArray()
                },
                {
                    "Permit",
                    new MemberDescription[] {
                        new() { Name = "owner", Type = "address" },
                        new() { Name = "spender", Type = "address" },
                        new() { Name = "value", Type = "uint256" },
                        new() { Name = "nonce", Type = "uint256" },
                        new() { Name = "deadline", Type = "uint256" },
                    }
                }
            };

            if (hasSalt)
            {
                TypedData<DomainWithSalt> typedData = new();
                typedData.Domain = new()
                {
                    Name = name,
                    Version = version,
                    ChainId = chainId,
                    VerifyingContract = verifyingContract,
                    Salt = salt
                };
                typedData.PrimaryType = primaryType;
                typedData.Types = types;
                typedData.Message = message;
                typedData.EnsureDomainRawValuesAreInitialised();
                return account.SignTypedDataV4(typedData);
            }
            else
            {
                TypedData<Domain> typedData = new();
                typedData.Domain = new()
                {
                    Name = name,
                    Version = version,
                    ChainId = chainId,
                    VerifyingContract = verifyingContract,
                };
                typedData.PrimaryType = primaryType;
                typedData.Types = types;
                typedData.Message = message;
                typedData.EnsureDomainRawValuesAreInitialised();
                return account.SignTypedDataV4(typedData);
            }
        }

        public Task<ISignature> GetPermitEIP712Signature(Account account, string contractName, string contractAddress, RallyNetworkConfig config, BigInteger nonce, BigInteger amount, BigInteger deadline, byte[] salt)
        {

            // chainId to be used in EIP712
            BigInteger chainId = BigInteger.Parse(config.Gsn.ChainId);

            // typed data for signing
            string signature = GetSignedTypedPermitTransaction(account, contractName, "1", chainId, contractAddress, account.Address, config.Gsn.PaymasterAddress, amount, nonce, deadline, salt);

            // signature for permit
            EthECDSASignature ethSignature = EthECDSASignatureFactory.ExtractECDSASignature(signature);
            return Task.FromResult<ISignature>(ethSignature);
        }

        public async Task<GsnTransactionDetails> GetPermitTx(Account account, string destinationAddress, BigInteger amount, RallyNetworkConfig config, string contractAddress, Web3 provider)
        {
            ERC721ContractService token = new(provider.Eth, contractAddress);

            BigInteger nonce = await this.transactionHelper.GetSenderContractNonce(provider, contractAddress, account.Address);
            string name = await token.NameQueryAsync();

            BigInteger deadline = await GetPermitDeadline(provider);
            Eip712DomainOutputDTO eip712Domain = await provider.Eth.GetContractQueryHandler<Eip712DomainFunction>().QueryAsync<Eip712DomainOutputDTO>(contractAddress);
            byte[] salt = eip712Domain.Salt;

            ISignature signature = await GetPermitEIP712Signature(account, name, token.ContractAddress, config, nonce, amount, deadline, salt);
            TransferFromFunction transferTx = new()
            {
                From = account.Address,
                To = destinationAddress,
                Value = amount
            };
            PermitFunction permitTx = new()
            {
                FromAddress = account.Address,
                Owner = account.Address,
                Spender = config.Gsn.PaymasterAddress,
                Value = amount,
                Deadline = deadline,
                V = signature.V[0],
                R = signature.R,
                S = signature.S,
            };

            HexBigInteger gas = await provider.Eth.GetContractTransactionHandler<PermitFunction>().EstimateGasAsync(contractAddress, permitTx);

            string paymasterData = $"0x{contractAddress.Replace("0x", "")}{transferTx.GetCallData().ToHex(false)}";
            BlockWithTransactions info = await provider.Eth.Blocks.GetBlockWithTransactionsByNumber.SendRequestAsync(BlockParameter.CreateLatest());

            BigInteger maxPriorityFeePerGas = BigInteger.Parse("1500000000");
            BigInteger maxFeePerGas = info.BaseFeePerGas.Value * 2 + maxPriorityFeePerGas;

            GsnTransactionDetails gsnTx = new()
            {
                From = account.Address,
                Data = permitTx.GetCallData().ToHex(true),
                Value = "0",
                To = token.ContractAddress,
                Gas = gas.HexValue,
                MaxFeePerGas = new HexBigInteger(maxFeePerGas).HexValue,
                MaxPriorityFeePerGas = new HexBigInteger(maxPriorityFeePerGas).HexValue,
                PaymasterData = paymasterData,
            };

            return gsnTx;
        }

        public async Task<BigInteger> GetPermitDeadline(Web3 provider)
        {
            BlockWithTransactions block = await provider.Eth.Blocks.GetBlockWithTransactionsByNumber.SendRequestAsync(BlockParameter.CreateLatest());
            return (block.Timestamp.Value + 45) * 1000;
        }

        #endregion

        #region Contract Definition

        public partial class PermitFunction : PermitFunctionBase { }

        [Function("permit", "bool")]
        public class PermitFunctionBase : FunctionMessage
        {
            [Parameter("address", "owner", 1)]
            public virtual string Owner { get; set; }
            [Parameter("address", "spender", 2)]
            public virtual string Spender { get; set; }
            [Parameter("uint256", "value", 3)]
            public virtual BigInteger Value { get; set; }
            [Parameter("uint256", "deadline", 4)]
            public virtual BigInteger Deadline { get; set; }
            [Parameter("uint8", "v", 5)]
            public virtual byte V { get; set; }
            [Parameter("bytes32", "r", 6)]
            public virtual byte[] R { get; set; }
            [Parameter("bytes32", "s", 7)]
            public virtual byte[] S { get; set; }
        }

        public partial class TransferFromFunction : TransferFromFunctionBase { }

        [Function("transferFrom", "bool")]
        public class TransferFromFunctionBase : FunctionMessage
        {
            [Parameter("address", "from", 1)]
            public virtual string From { get; set; } = string.Empty;

            [Parameter("address", "to", 2)]
            public virtual string To { get; set; } = string.Empty;

            [Parameter("uint256", "value", 3)]
            public virtual BigInteger Value { get; set; }
        }

        public partial class Eip712DomainFunction : Eip712DomainFunctionBase { }

        [Function("eip712Domain", typeof(Eip712DomainOutputDTO))]
        public class Eip712DomainFunctionBase : FunctionMessage
        {

        }

        public partial class Eip712DomainOutputDTO : Eip712DomainOutputDTOBase { }

        [FunctionOutput]
        public class Eip712DomainOutputDTOBase : IFunctionOutputDTO
        {
            [Parameter("bytes1", "fields", 1)]
            public virtual byte[] Fields { get; set; }
            [Parameter("string", "name", 2)]
            public virtual string Name { get; set; }
            [Parameter("string", "version", 3)]
            public virtual string Version { get; set; }
            [Parameter("uint256", "chainId", 4)]
            public virtual BigInteger ChainId { get; set; }
            [Parameter("address", "verifyingContract", 5)]
            public virtual string VerifyingContract { get; set; }
            [Parameter("bytes32", "salt", 6)]
            public virtual byte[] Salt { get; set; }
            //[Parameter("uint256[]", "extensions", 7)]
            //public virtual List<BigInteger> Extensions { get; set; }
        }

        #endregion

    }

}