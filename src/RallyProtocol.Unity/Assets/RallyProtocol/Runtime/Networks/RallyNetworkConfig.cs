using System.Collections;
using System.Collections.Generic;

namespace RallyProtocol.Networks
{

    [System.Serializable]
    public class RallyNetworkConfig
    {

        #region Fields

        public static readonly RallyNetworkConfig BaseSepoliaDefault = new()
        {
            Contracts = RallyContracts.BaseSepoliaDefault,
            Gsn = RallyGSNConfig.BaseSepoliaDefault
        };

        public static readonly RallyNetworkConfig BaseDefault = new()
        {
            Contracts = RallyContracts.BaseDefault,
            Gsn = RallyGSNConfig.BaseDefault
        };

        public static readonly RallyNetworkConfig AmoyDefault = new()
        {
            Contracts = RallyContracts.AmoyDefault,
            Gsn = RallyGSNConfig.AmoyDefault
        };

        public static readonly RallyNetworkConfig AmoyWithPermitDefault = new()
        {
            Contracts = RallyContracts.AmoyWithPermitDefault,
            Gsn = RallyGSNConfig.AmoyWithPermitDefault
        };

        public static readonly RallyNetworkConfig PolygonDefault = new()
        {
            Contracts = RallyContracts.PolygonDefault,
            Gsn = RallyGSNConfig.PolygonDefault
        };

        public static readonly RallyNetworkConfig LocalDefault = new()
        {
            Contracts = RallyContracts.LocalDefault,
            Gsn = RallyGSNConfig.LocalDefault
        };

        public static readonly RallyNetworkConfig TestDefault = new()
        {
            Contracts = RallyContracts.TestDefault,
            Gsn = RallyGSNConfig.TestDefault
        };

        #endregion

        #region Properties

        public static RallyNetworkConfig BaseSepolia => BaseSepoliaDefault.Clone();
        public static RallyNetworkConfig Base => BaseDefault.Clone();
        public static RallyNetworkConfig Amoy => AmoyDefault.Clone();
        public static RallyNetworkConfig AmoyWithPermit => AmoyWithPermitDefault.Clone();
        public static RallyNetworkConfig Polygon => PolygonDefault.Clone();
        public static RallyNetworkConfig Local => LocalDefault.Clone();
        public static RallyNetworkConfig Test => TestDefault.Clone();

        public virtual RallyContracts Contracts { get; set; }
        public virtual RallyGSNConfig Gsn { get; set; }
        public virtual string RelayerApiKey { get; set; }

        #endregion

        #region Public Methods

        public RallyNetworkConfig Clone()
        {
            return new()
            {
                Contracts = Contracts.Clone(),
                Gsn = Gsn.Clone(),
                RelayerApiKey = RelayerApiKey
            };
        }

        public override string ToString()
        {
            return $"NetworkConfig{{contracts: {Contracts}, gsn: {Gsn}, relayerApiKey: {RelayerApiKey}}}";
        }

        #endregion

    }

    [System.Serializable]
    public class RallyContracts
    {

        #region Fields

        public static readonly RallyContracts BaseSepoliaDefault = new()
        {
            RlyERC20 = "0x16723e9bb894EfC09449994eC5bCF5b41EE0D9b2",
            TokenFaucet = "0xCeCFB48a9e7C0765Ed1319ee1Bc0F719a30641Ce",
        };

        public static readonly RallyContracts BaseDefault = new()
        {
            // TODO: there are no contracts for base mainnet. Client needs update to handle this case gracefully.
            RlyERC20 = "0x000",
            TokenFaucet = "0x000",
        };

        public static readonly RallyContracts AmoyDefault = new()
        {
            RlyERC20 = "0x846d8a5fb8a003b431b67115f809a9b9fffe5012",
            TokenFaucet = "0xb8c8274f775474f4f2549edcc4db45cbad936fac",
        };

        public static readonly RallyContracts AmoyWithPermitDefault = new()
        {
            RlyERC20 = "0x758641a1b566998CaC5Bc5fC8032F001e1CEBeEf",
            TokenFaucet = "0xAb5C5633a5c483499047e552C96E1760136dc70A",
        };

        public static readonly RallyContracts PolygonDefault = new()
        {
            RlyERC20 = "0x76b8D57e5ac6afAc5D415a054453d1DD2c3C0094",
            TokenFaucet = "0x78a0794Bb3BB06238ed5f8D926419bD8fc9546d8",
        };

        public static readonly RallyContracts LocalDefault = new()
        {
            RlyERC20 = "0x3Aa5ebB10DC797CAC828524e59A333d0A371443c",
            TokenFaucet = "0x3Aa5ebB10DC797CAC828524e59A333d0A371443c",
        };

        public static readonly RallyContracts TestDefault = new()
        {
            RlyERC20 = "0x1C7312Cb60b40cF586e796FEdD60Cf243286c9E9",
            TokenFaucet = "0xe7C3BD692C77Ec0C0bde523455B9D142c49720fF",
        };

        #endregion

        #region Properties

        public static RallyContracts BaseSepolia => BaseSepoliaDefault.Clone();
        public static RallyContracts Amoy => AmoyDefault.Clone();
        public static RallyContracts AmoyWithPermit => AmoyWithPermitDefault.Clone();
        public static RallyContracts Polygon => PolygonDefault.Clone();
        public static RallyContracts Local => LocalDefault.Clone();
        public static RallyContracts Test => TestDefault.Clone();

        public virtual string TokenFaucet { get; set; }
        public virtual string RlyERC20 { get; set; }

        #endregion

        #region Public Methods

        public RallyContracts Clone()
        {
            return new()
            {
                TokenFaucet = TokenFaucet,
                RlyERC20 = RlyERC20
            };
        }

        public override string ToString()
        {
            return $"Contracts{{tokenFaucet: {TokenFaucet}, rlyERC20: {RlyERC20}}}";
        }

        #endregion

    }

    [System.Serializable]
    public class RallyGSNConfig
    {

        #region Fields

        public static readonly RallyGSNConfig BaseSepoliaDefault = new()
        {
            PaymasterAddress = "0x9bf59A7924cBa2475A03AD77e92fcf1Eaddb2Cc2",
            ForwarderAddress = "0xabf9Fa3b2b2d9bDd77f4271A0d5A309AA465BCBa",
            RelayHubAddress = "0xb570b57b821670707fF4E38Ea53fcb67192278F8",
            RelayWorkerAddress = "0xdb1d6c7b07c857cc22a4ef10ac7b1dd06dd7501f",
            RelayUrl = "https://api.rallyprotocol.com",
            RpcUrl = "https://api.rallyprotocol.com/rpc",
            ChainId = "84532",
            MaxAcceptanceBudget = "285252",
            DomainSeparatorName = "GSN Relayed Transaction",
            GtxDataNonZero = 16,
            GtxDataZero = 4,
            RequestValidSeconds = 172800,
            MaxPaymasterDataLength = 300,
            MaxApprovalDataLength = 300,
            MaxRelayNonceGap = 3,
        };

        public static readonly RallyGSNConfig BaseDefault = new()
        {
            PaymasterAddress = "0x01B83B33F0DD8be68627a9BE68E9e7E3c209a6b1",
            ForwarderAddress = "0x524266345fB331cb624E27D2Cf5B61E769527FCC",
            RelayHubAddress = "0x54623092d2dB00D706e0Ad4ADaCc024F9cB9E915",
            RelayWorkerAddress = "0x7c5b7cf606ab2b56ead90b583bad47c5fd2c3417",
            RelayUrl = "https://api.rallyprotocol.com",
            RpcUrl = "https://api.rallyprotocol.com/rpc",
            ChainId = "8453",
            MaxAcceptanceBudget = "285252",
            DomainSeparatorName = "GSN Relayed Transaction",
            GtxDataNonZero = 16,
            GtxDataZero = 4,
            RequestValidSeconds = 172800,
            MaxPaymasterDataLength = 300,
            MaxApprovalDataLength = 300,
            MaxRelayNonceGap = 3,
        };

        public static readonly RallyGSNConfig AmoyDefault = new()
        {
            PaymasterAddress = "0xb570b57b821670707fF4E38Ea53fcb67192278F8",
            ForwarderAddress = "0x0ae8FC9867CB4a124d7114B8bd15C4c78C4D40E5",
            RelayHubAddress = "0xe213A20A9E6CBAfd8456f9669D8a0b9e41Cb2751",
            RelayWorkerAddress = "0xb9950b71ec94cbb274aeb1be98e697678077a17f",
            RelayUrl = "https://api.rallyprotocol.com",
            RpcUrl = "https://api.rallyprotocol.com/rpc",
            ChainId = "80002",
            MaxAcceptanceBudget = "285252",
            DomainSeparatorName = "GSN Relayed Transaction",
            GtxDataNonZero = 16,
            GtxDataZero = 4,
            RequestValidSeconds = 172800,
            MaxPaymasterDataLength = 300,
            MaxApprovalDataLength = 300,
            MaxRelayNonceGap = 3,
        };

        public static readonly RallyGSNConfig AmoyWithPermitDefault = new()
        {
            PaymasterAddress = "0xb570b57b821670707fF4E38Ea53fcb67192278F8",
            ForwarderAddress = "0x0ae8FC9867CB4a124d7114B8bd15C4c78C4D40E5",
            RelayHubAddress = "0xe213A20A9E6CBAfd8456f9669D8a0b9e41Cb2751",
            RelayWorkerAddress = "0xb9950b71ec94cbb274aeb1be98e697678077a17f",
            RelayUrl = "https://api.rallyprotocol.com",
            RpcUrl = "https://polygon-amoy.g.alchemy.com/v2/oOsX9gjRzWeq5WQrlM3zvWAXZ9nIT2Cr",
            ChainId = "80002",
            MaxAcceptanceBudget = "285252",
            DomainSeparatorName = "GSN Relayed Transaction",
            GtxDataNonZero = 16,
            GtxDataZero = 4,
            RequestValidSeconds = 172800,
            MaxPaymasterDataLength = 300,
            MaxApprovalDataLength = 300,
            MaxRelayNonceGap = 3,
        };

        public static readonly RallyGSNConfig PolygonDefault = new()
        {
            PaymasterAddress = "0x29CAa31142D17545C310437825aA4C53FbE621C3",
            ForwarderAddress = "0xB2b5841DBeF766d4b521221732F9B618fCf34A87",
            RelayHubAddress = "0xfCEE9036EDc85cD5c12A9De6b267c4672Eb4bA1B",
            RelayWorkerAddress = "0x579de7c56cd9a07330504a7c734023a9f703778a",
            RelayUrl = "https://api.rallyprotocol.com",
            RpcUrl = "https://api.rallyprotocol.com/rpc",
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

        public static readonly RallyGSNConfig LocalDefault = new()
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

        public static readonly RallyGSNConfig TestDefault = new()
        {
            PaymasterAddress = "0x8b3a505413Ca3B0A17F077e507aF8E3b3ad4Ce4d",
            ForwarderAddress = "0xB2b5841DBeF766d4b521221732F9B618fCf34A87",
            RelayHubAddress = "0x3232f21A6E08312654270c78A773f00dd61d60f5",
            RelayWorkerAddress = "0xb9950b71ec94cbb274aeb1be98e697678077a17f",
            RelayUrl = "http://localhost:3004",
            RpcUrl = "http://localhost:3004/rpc4",
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

        #endregion

        #region Properties

        public static RallyGSNConfig BaseSepolia => BaseSepoliaDefault.Clone();
        public static RallyGSNConfig Amoy => AmoyDefault.Clone();
        public static RallyGSNConfig AmoyWithPermit => AmoyWithPermit.Clone();
        public static RallyGSNConfig Polygon => PolygonDefault.Clone();
        public static RallyGSNConfig Local => LocalDefault.Clone();
        public static RallyGSNConfig Test => TestDefault.Clone();

        public virtual string PaymasterAddress { get; set; }
        public virtual string ForwarderAddress { get; set; }
        public virtual string RelayHubAddress { get; set; }
        public virtual string RelayWorkerAddress { get; set; }
        public virtual string RelayUrl { get; set; }
        public virtual string RpcUrl { get; set; }
        public virtual string ChainId { get; set; }
        public virtual string MaxAcceptanceBudget { get; set; }
        public virtual string DomainSeparatorName { get; set; }
        public virtual int GtxDataZero { get; set; }
        public virtual int GtxDataNonZero { get; set; }
        public virtual int RequestValidSeconds { get; set; }
        public virtual int MaxPaymasterDataLength { get; set; }
        public virtual int MaxApprovalDataLength { get; set; }
        public virtual int MaxRelayNonceGap { get; set; }

        #endregion

        #region Public Methods

        public RallyGSNConfig Clone()
        {
            return new()
            {
                PaymasterAddress = PaymasterAddress,
                ForwarderAddress = ForwarderAddress,
                RelayHubAddress = RelayHubAddress,
                RelayWorkerAddress = RelayWorkerAddress,
                RelayUrl = RelayUrl,
                RpcUrl = RpcUrl,
                ChainId = ChainId,
                MaxAcceptanceBudget = MaxAcceptanceBudget,
                DomainSeparatorName = DomainSeparatorName,
                GtxDataZero = GtxDataZero,
                GtxDataNonZero = GtxDataNonZero,
                RequestValidSeconds = RequestValidSeconds,
                MaxPaymasterDataLength = MaxPaymasterDataLength,
                MaxApprovalDataLength = MaxApprovalDataLength,
                MaxRelayNonceGap = MaxRelayNonceGap,
            };
        }

        public override string ToString()
        {
            return $"GSNConfig{{paymasterAddress: {PaymasterAddress}, forwarderAddress: {ForwarderAddress}, relayHubAddress: {RelayHubAddress}, relayWorkerAddress: {RelayWorkerAddress}, relayUrl: {RelayUrl}, rpcUrl: {RpcUrl}, chainId: {ChainId}, maxAcceptanceBudget: {MaxAcceptanceBudget}, domainSeparatorName: {DomainSeparatorName}, gtxDataZero: {GtxDataZero}, gtxDataNonZero: {GtxDataNonZero}, requestValidSeconds: {RequestValidSeconds}, maxPaymasterDataLength: {MaxPaymasterDataLength}, maxApprovalDataLength: {MaxApprovalDataLength}, maxRelayNonceGap: {MaxRelayNonceGap}}}";
        }

        #endregion

    }

}