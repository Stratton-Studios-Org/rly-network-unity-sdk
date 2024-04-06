using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace RallyProtocol
{

    public class RallyNetworkConfig
    {

        public static readonly RallyNetworkConfig Local = new()
        {
            Contracts = RallyContracts.Local,
            Gsn = RallyGSNConfig.Local
        };

        public static readonly RallyNetworkConfig Mumbai = new()
        {
            Contracts = RallyContracts.Mumbai,
            Gsn = RallyGSNConfig.Mumbai
        };

        public static readonly RallyNetworkConfig Polygon = new()
        {
            Contracts = RallyContracts.Polygon,
            Gsn = RallyGSNConfig.Polygon
        };

        public RallyContracts Contracts;
        public RallyGSNConfig Gsn;

        public override string ToString()
        {
            return $"NetworkConfig{{contracts: {Contracts}, gsn: {Gsn}, relayerApiKey: $relayerApiKey}}";
        }

    }

    public class RallyContracts
    {

        public static readonly RallyContracts Local = new()
        {
            TokenFaucet = "0xa85233C63b9Ee964Add6F2cffe00Fd84eb32338f",
            RlyERC20 = "0xc6e7DF5E7b4f2A278906862b61205850344D4e7d",
        };

        public static readonly RallyContracts Mumbai = new()
        {
            RlyERC20 = "0x1C7312Cb60b40cF586e796FEdD60Cf243286c9E9",
            TokenFaucet = "0xe7C3BD692C77Ec0C0bde523455B9D142c49720fF",
        };

        public static readonly RallyContracts Polygon = new()
        {
            RlyERC20 = "0x78a0794Bb3BB06238ed5f8D926419bD8fc9546d8",
            TokenFaucet = "0x76b8D57e5ac6afAc5D415a054453d1DD2c3C0094",
        };

        public string TokenFaucet;
        public string RlyERC20;

        public override string ToString()
        {
            return $"Contracts{{tokenFaucet: {TokenFaucet}, rlyERC20: {RlyERC20}}}";
        }

    }

    public class RallyGSNConfig
    {

        public static readonly RallyGSNConfig Local = new()
        {
            PaymasterAddress = "0x7a2088a1bFc9d81c55368AE168C2C02570cB814F",
            ForwarderAddress = "0xCf7Ed3AccA5a467e9e704C703E8D87F634fB0Fc9",
            RelayHubAddress = "0xDc64a140Aa3E981100a9becA4E685f962f0cF6C9",
            RelayWorkerAddress = "0x84ef35506635109ce61544193e8f87b0a1a1b4fd",
            RelayUrl = "http://localhost:8090",
            RpcUrl = "http://127.0.0.1:8545",
            ChainId = "1337",
            MaxAcceptanceBudget = "285252",
            DomainSeparatorName = "GSN Relayed Transaction",
            GtxDataNonZero = 16,
            GtxDataZero = 4,
            RequestValidSeconds = 172800,
            MaxPaymasterDataLength = 300,
            MaxApprovalDataLength = 0,
            MaxRelayNonceGap = 3,
        };

        public static readonly RallyGSNConfig Mumbai = new()
        {
            PaymasterAddress = "0x8b3a505413Ca3B0A17F077e507aF8E3b3ad4Ce4d",
            ForwarderAddress = "0xB2b5841DBeF766d4b521221732F9B618fCf34A87",
            RelayHubAddress = "0x3232f21A6E08312654270c78A773f00dd61d60f5",
            RelayWorkerAddress = "0xb9950b71ec94cbb274aeb1be98e697678077a17f",
            RelayUrl = "https://api.rallyprotocol.com",
            RpcUrl =
                "https://polygon-mumbai.g.alchemy.com/v2/-dYNjZXvre3GC9kYtwDzzX4N8tcgomU4",
            ChainId = "80001",
            MaxAcceptanceBudget = "285252",
            DomainSeparatorName = "GSN Relayed Transaction",
            GtxDataNonZero = 16,
            GtxDataZero = 4,
            RequestValidSeconds = 172800,
            MaxPaymasterDataLength = 300,
            MaxApprovalDataLength = 300,
            MaxRelayNonceGap = 3,
        };

        public static readonly RallyGSNConfig Polygon = new()
        {
            PaymasterAddress = "0x29CAa31142D17545C310437825aA4C53FbE621C3",
            ForwarderAddress = "0xB2b5841DBeF766d4b521221732F9B618fCf34A87",
            RelayHubAddress = "0xfCEE9036EDc85cD5c12A9De6b267c4672Eb4bA1B",
            RelayWorkerAddress = "0x579de7c56cd9a07330504a7c734023a9f703778a",
            RelayUrl = "https://api.rallyprotocol.com",
            RpcUrl =
                "https://polygon-mainnet.g.alchemy.com/v2/-dYNjZXvre3GC9kYtwDzzX4N8tcgomU4",
            ChainId = "137",
            MaxAcceptanceBudget = "285252",
            DomainSeparatorName = "GSN Relayed Transaction",
            GtxDataNonZero = 16,
            GtxDataZero = 4,
            RequestValidSeconds = 172800,
            MaxPaymasterDataLength = 300,
            MaxApprovalDataLength = 0,
            MaxRelayNonceGap = 3,
        };

        public string PaymasterAddress;
        public string ForwarderAddress;
        public string RelayHubAddress;
        public string RelayWorkerAddress;
        public string RelayUrl;
        public string RpcUrl;
        public string ChainId;
        public string MaxAcceptanceBudget;
        public string DomainSeparatorName;
        public int GtxDataZero;
        public int GtxDataNonZero;
        public int RequestValidSeconds;
        public int MaxPaymasterDataLength;
        public int MaxApprovalDataLength;
        public int MaxRelayNonceGap;

        public override string ToString()
        {
            return $"GSNConfig{{paymasterAddress: {PaymasterAddress}, forwarderAddress: {ForwarderAddress}, relayHubAddress: {RelayHubAddress}, relayWorkerAddress: {RelayWorkerAddress}, relayUrl: {RelayUrl}, rpcUrl: {RpcUrl}, chainId: {ChainId}, maxAcceptanceBudget: {MaxAcceptanceBudget}, domainSeparatorName: {DomainSeparatorName}, gtxDataZero: {GtxDataZero}, gtxDataNonZero: {GtxDataNonZero}, requestValidSeconds: {RequestValidSeconds}, maxPaymasterDataLength: {MaxPaymasterDataLength}, maxApprovalDataLength: {MaxApprovalDataLength}, maxRelayNonceGap: {MaxRelayNonceGap}}}";
        }

    }

}