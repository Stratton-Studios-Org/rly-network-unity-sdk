using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Nethereum.ABI.EIP712;
using Nethereum.ABI.EIP712.EIP2612;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts.Standards.ERC20;
using Nethereum.Contracts.Standards.ERC721;
using Nethereum.Hex.HexTypes;
using Nethereum.Model;
using Nethereum.RPC;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.RPC.Fee1559Suggestions;
using Nethereum.Signer;
using Nethereum.Signer.EIP712;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;

using Org.BouncyCastle.Tsp;

using UnityEngine;

using Account = Nethereum.Web3.Accounts.Account;

namespace RallyProtocol.GSN
{

    public class PermitTransaction
    {

        public static async Task<bool> HasPermit(Account account, BigInteger amount, RallyNetworkConfig config, string contractAddress, Web3 provider)
        {
            try
            {
                ERC20ContractService token = new(provider.Eth, contractAddress);

                string name = await token.NameQueryAsync();
                BigInteger nonce = await GsnTransactionHelper.GetSenderContractNonce(token, account.Address);
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

        public static TypedData<DomainWithSalt> GetTypedPermitTransaction(string name, string version, BigInteger chainId, string verifyingContract, string owner, string spender, BigInteger value, BigInteger nonce, BigInteger deadline, byte[] salt)
        {
            TypedData<DomainWithSalt> typedData = new();
            typedData.Types = new Dictionary<string, MemberDescription[]>()
            {
                { "Permit", new MemberDescription[] {
                    new() { Name = "owner", Type = "address" },
                    new() { Name = "spender", Type = "address" },
                    new() { Name = "value", Type = "uint256" },
                    new() { Name = "nonce", Type = "uint256" },
                    new() { Name = "deadline", Type = "uint256" },
                } }
            };
            typedData.PrimaryType = "Permit";
            typedData.Domain = new()
            {
                Name = name,
                Version = version,
                ChainId = chainId,
                VerifyingContract = verifyingContract,
                Salt = salt
            };
            typedData.Message = new MemberValue[]
            {
                new() { TypeName = "owner", Value = owner },
                new() { TypeName = "spender", Value = spender },
                new() { TypeName = "value", Value = value },
                new() { TypeName = "nonce", Value = nonce },
                new() { TypeName = "deadline", Value = deadline }
            };

            return typedData;
        }

        public static async Task<GsnTransactionDetails> GetPermitTx(Account account, string destinationAddress, BigInteger amount, RallyNetworkConfig config, string contractAddress, Web3 provider)
        {
            ERC20ContractService token = new(provider.Eth, contractAddress);
            string name = await token.NameQueryAsync();
            BigInteger nonce = await GsnTransactionHelper.GetSenderContractNonce(token, account.Address);
            BigInteger deadline = await GetPermitDeadline(provider);
            Eip712DomainOutputDTO eip712Domain = await token.ContractHandler.QueryAsync<Eip712DomainFunction, Eip712DomainOutputDTO>();

            byte[] salt = eip712Domain.Salt;
            ISignature signature = await GetPermitEIP712Signature(account, name, token.ContractAddress, config, nonce, amount, deadline, salt);
            PermitFunction permitFunction = new()
            {
                Owner = account.Address,
                Spender = config.Gsn.PaymasterAddress,
                AmountToSend = amount,
                Deadline = deadline,
                V = signature.V[0],
                R = signature.R,
                S = signature.S,
                FromAddress = account.Address
            };
            string permitFunctionData = token.ContractHandler.GetFunction<PermitFunction>().GetData(permitFunction);
            HexBigInteger gas = await token.ContractHandler.EstimateGasAsync(permitFunction);
            TransferFromFunction transferFromFunction = new()
            {
                Sender = account.Address,
                Recipient = destinationAddress,
                Amount = amount
            };

            string transferFromFunctionData = token.ContractHandler.GetFunction<TransferFromFunction>().GetData(transferFromFunction);
            string paymasterData = "0x" + Regex.Replace(token.ContractAddress, "/^0x/", "") + Regex.Replace(transferFromFunctionData, "/^0x/", "");
            Fee1559 fee = await provider.FeeSuggestion.GetSimpleFeeSuggestionStrategy().SuggestFeeAsync();

            GsnTransactionDetails gsnTx = new()
            {
                From = account.Address,
                Data = permitFunctionData,
                Value = BigInteger.Zero,
                To = token.ContractAddress,
                Gas = gas.HexValue,
                MaxFeePerGas = new HexBigInteger(fee.MaxFeePerGas.Value).HexValue,
                MaxPriorityFeePerGas = new HexBigInteger(fee.MaxPriorityFeePerGas.Value).HexValue,
                PaymasterData = paymasterData,
            };

            return gsnTx;
        }

        public static async Task<BigInteger> GetPermitDeadline(Web3 provider)
        {
            HexBigInteger latestBlockNumber = await provider.Eth.Blocks.GetBlockNumber.SendRequestAsync();
            BlockWithTransactionHashes latestBlock = await provider.Eth.Blocks.GetBlockWithTransactionsHashesByNumber.SendRequestAsync(latestBlockNumber);
            return latestBlock.Timestamp.Value + 45;
        }

        public static Task<ISignature> GetPermitEIP712Signature(Account account, string contractName, string contractAddress, RallyNetworkConfig config, BigInteger nonce, BigInteger amount, BigInteger deadline, byte[] salt)
        {

            // chainId to be used in EIP712
            BigInteger chainId = BigInteger.Parse(config.Gsn.ChainId);

            // typed data for signing
            TypedData<DomainWithSalt> eip712Data = GetTypedPermitTransaction(contractName, "1", chainId, contractAddress, account.Address, config.Gsn.PaymasterAddress, amount, nonce, deadline, salt);

            //signature for metatransaction
            Eip712TypedDataSigner signer = new();
            string signature = signer.SignTypedData(eip712Data, new EthECKey(account.PrivateKey));
            EthECDSASignature ethSignature = EthECDSASignatureFactory.ExtractECDSASignature(signature);

            return Task.FromResult<ISignature>(ethSignature);
        }

    }

}