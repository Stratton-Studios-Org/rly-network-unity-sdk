using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;

using Nethereum.RPC.Eth.DTOs;
using Nethereum.Web3;

using RallyProtocol.Core;

namespace RallyProtocol.GSN
{

    public class FeeData
    {

        public BigInteger MaxFeePerGas;
        public BigInteger MaxPriorityFeePerGas;

        public FeeData(BigInteger maxFeePerGas, BigInteger maxPriorityFeePerGas)
        {
            MaxFeePerGas = maxFeePerGas;
            MaxPriorityFeePerGas = maxPriorityFeePerGas;
        }

        public static async Task<FeeData> GetFeeData(Web3 web3)
        {
            Block block = await web3.Eth.Blocks.GetBlockWithTransactionsByNumber.SendRequestAsync(BlockParameter.CreateLatest());

            BigInteger maxPriorityFeePerGas = await ComputeMaxPriorityFeePerGas(block, web3);
            BigInteger maxFeePerGas;
            if (block != null)
            {
                maxFeePerGas = (block.BaseFeePerGas.Value * 2) + maxPriorityFeePerGas;
            }

            return new FeeData(maxFeePerGas, maxPriorityFeePerGas);
        }

        public static async Task<BigInteger> ComputeMaxPriorityFeePerGas(Block block, Web3 web3)
        {
            if (block == null)
            {
                return BigInteger.Parse("1500000000");
            }

            BigInteger gasPrice = await web3.Eth.GasPrice.SendRequestAsync();

            BigInteger tenPercentBuffer =
                (gasPrice * new BigInteger(0.1 * 100)) / new BigInteger(100);

            return (gasPrice + tenPercentBuffer) - block.BaseFeePerGas;
        }

    }

}
