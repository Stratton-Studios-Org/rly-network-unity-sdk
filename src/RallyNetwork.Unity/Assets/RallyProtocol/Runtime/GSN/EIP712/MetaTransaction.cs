using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;

using Nethereum.ABI.EIP712;
using Nethereum.Contracts.Standards.ERC20;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.Hex.HexTypes;
using Nethereum.Model;
using Nethereum.RPC.Fee1559Suggestions;
using Nethereum.Signer;
using Nethereum.Signer.EIP712;
using Nethereum.Web3;

using RallyProtocol.Utilities;

using UnityEngine;

using Account = Nethereum.Web3.Accounts.Account;

namespace RallyProtocol.GSN
{

    public class MetaTransaction
    {

        public static TypedData<DomainWithSalt> GetTypedMetaTransaction(string name, string version, byte[] salt, string verifyingContract, BigInteger nonce, string from, string functionSignature)
        {
            TypedData<DomainWithSalt> typedData = new();
            typedData.Types = new Dictionary<string, MemberDescription[]>()
            {
                { "MetaTransaction", new MemberDescription[] {
                    new() { Name = "nonce", Type = "uint256" },
                    new() { Name = "from", Type = "address" },
                    new() { Name = "functionSignature", Type = "bytes" },
                } }
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
                new() { TypeName = "owner", Value = nonce },
                new() { TypeName = "from", Value = from },
                new() { TypeName = "functionSignature", Value = functionSignature }
            };

            return typedData;
        }

        public static Task<ISignature> GetMetaTransactionEIP712Signature(Account account, string contractName, string contractAddress, string functionSignatuer, RallyNetworkConfig config, BigInteger nonce)
        {

            // name and chainId to be used in EIP712
            BigInteger chainId = BigInteger.Parse(config.Gsn.ChainId);
            ;
            // typed data for signing
            TypedData<DomainWithSalt> eip712Data = GetTypedMetaTransaction(contractName, "1", new HexBigInteger(chainId).HexValue.HexZeroPad(32).HexToByteArray(), contractAddress, nonce, account.Address, functionSignatuer);

            //signature for metatransaction
            Eip712TypedDataSigner signer = new();
            string signature = signer.SignTypedData(eip712Data, new EthECKey(account.PrivateKey));
            EthECDSASignature ethSignature = EthECDSASignatureFactory.ExtractECDSASignature(signature);

            return Task.FromResult<ISignature>(ethSignature);
        }

        public static async Task<GsnTransactionDetails> GetExecuteMetaTransactionTx(Account account, string destinationAddress, BigInteger amount, RallyNetworkConfig config, string contractAddress, Web3 provider)
        {
            ERC20ContractService token = new(provider.Eth, contractAddress);
            string name = await token.NameQueryAsync();
            BigInteger nonce = await GsnTransactionHelper.GetSenderContractNonce(token, account.Address);

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
            Fee1559 fee = await provider.FeeSuggestion.GetSimpleFeeSuggestionStrategy().SuggestFeeAsync();

            GsnTransactionDetails gsnTx = new()
            {
                From = account.Address,
                Data = executeMetaTransactionFunctionData,
                Value = BigInteger.Zero,
                To = token.ContractAddress,
                Gas = gas.HexValue,
                MaxFeePerGas = new HexBigInteger(fee.MaxFeePerGas.Value).HexValue,
                MaxPriorityFeePerGas = new HexBigInteger(fee.MaxPriorityFeePerGas.Value).HexValue
            };

            return gsnTx;
        }

    }

}