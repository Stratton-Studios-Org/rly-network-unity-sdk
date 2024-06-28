using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;

using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Web3;

using Newtonsoft.Json.Linq;

using UnityEngine;

namespace RallyProtocol.Samples
{

    public class NFT
    {

        public const string TestNftResourcePath = "RallyProtocol/Samples/TestNft";

        readonly Contract contract;
        readonly string walletAddress;
        readonly Web3 provider;

        public NFT(string contractAddress, string walletAddress, Web3 provider)
        {
            TextAsset textAsset = Resources.Load<TextAsset>(TestNftResourcePath);
            var testNftJson = JToken.Parse(textAsset.text);

            contract = provider.Eth.GetContract(testNftJson["abi"]!.ToString(), contractAddress);
            this.walletAddress = walletAddress;
            this.provider = provider;
        }

        public async Task<int> GetCurrentNFTIdAsync()
        {
            var function = contract.GetFunction("tokenIds");
            var result = await function.CallAsync<int>();
            return result;
        }

        public async Task<string> GetTokenURIAsync(int tokenId)
        {
            var function = contract.GetFunction("tokenURI");
            var result = await function.CallAsync<string>(tokenId);
            return result;
        }

        public async Task<GsnTransactionDetails> GetMinftNFTTx()
        {
            var tx = new MintFunction()
            {
                FromAddress = walletAddress
            };

            var gas = await provider.Eth.GetContractTransactionHandler<MintFunction>().EstimateGasAsync(contract.Address, tx);

            var info = await provider.Eth.Blocks.GetBlockWithTransactionsByNumber.SendRequestAsync(BlockParameter.CreateLatest());

            var maxPriorityFeePerGas = BigInteger.Parse("1500000000");
            var maxFeePerGas = info.BaseFeePerGas.Value * 2 + maxPriorityFeePerGas;

            return new GsnTransactionDetails(walletAddress, tx.GetCallData().ToHex(true), contract.Address, maxFeePerGas.ToString(), maxPriorityFeePerGas.ToString(), "0", $"0x{gas.Value:X2}");
        }

        [Function("mint")]
        class MintFunction : FunctionMessage
        {
        }

    }

}