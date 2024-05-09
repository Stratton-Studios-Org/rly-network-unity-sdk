using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;

using UnityEngine;

namespace RallyProtocol.Networks
{

    public enum RallyNetworkType
    {
        Local,
        Amoy,
        Polygon,
        Test,
        Custom
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

}