# RallyNetworkConfig

The `RallyNetworkConfig` class defines the configuration options for connecting to a Rally Protocol network.

## Class Definition

```csharp
[System.Serializable]
public class RallyNetworkConfig
{
    // Static properties for predefined configurations
    public static RallyNetworkConfig BaseSepolia { get; }
    public static RallyNetworkConfig Base { get; }
    public static RallyNetworkConfig Amoy { get; }
    public static RallyNetworkConfig AmoyWithPermit { get; }
    public static RallyNetworkConfig Polygon { get; }
    public static RallyNetworkConfig Local { get; }
    public static RallyNetworkConfig Test { get; }
    
    // Properties
    public virtual RallyContracts Contracts { get; set; }
    public virtual RallyGSNConfig Gsn { get; set; }
    public virtual string RelayerApiKey { get; set; }
    
    // Methods
    public RallyNetworkConfig Clone();
    public override string ToString();
}
```

## Properties

### Predefined Network Configurations

| Property | Description |
|----------|-------------|
| `BaseSepolia` | Returns a configuration for Base Sepolia testnet |
| `Base` | Returns a configuration for Base mainnet |
| `Amoy` | Returns a configuration for Amoy testnet |
| `AmoyWithPermit` | Returns a configuration for Amoy testnet with permit support |
| `Polygon` | Returns a configuration for Polygon mainnet |
| `Local` | Returns a configuration for local development |
| `Test` | Returns a configuration for testing environments |

### Configuration Properties

| Property | Type | Description |
|----------|------|-------------|
| `Contracts` | `RallyContracts` | Contains contract addresses for the network |
| `Gsn` | `RallyGSNConfig` | Contains GSN (Gas Station Network) configuration |
| `RelayerApiKey` | `string` | API key for the relayer service |

## Methods

### Clone

```csharp
public RallyNetworkConfig Clone()
```

Creates a deep copy of the network configuration.

**Returns:**

- A new `RallyNetworkConfig` instance with the same values

**Example:**

```csharp
// Get a base configuration and modify it
RallyNetworkConfig config = RallyNetworkConfig.BaseSepolia.Clone();
config.RelayerApiKey = "your-api-key";
```

### ToString

```csharp
public override string ToString()
```

Returns a string representation of the network configuration.

**Returns:**

- String containing the network configuration details in the format: `NetworkConfig{contracts: {Contracts}, gsn: {Gsn}, relayerApiKey: {RelayerApiKey}}`

## RallyContracts Class

The `RallyContracts` class defines contract addresses for Rally Protocol networks.

```csharp
[System.Serializable]
public class RallyContracts
{
    // Static properties for predefined contracts
    public static RallyContracts BaseSepolia { get; }
    public static RallyContracts Base { get; }
    public static RallyContracts Amoy { get; }
    public static RallyContracts AmoyWithPermit { get; }
    public static RallyContracts Polygon { get; }
    public static RallyContracts Local { get; }
    public static RallyContracts Test { get; }
    
    // Properties
    public virtual string TokenFaucet { get; set; }
    public virtual string RlyERC20 { get; set; }
    
    // Methods
    public RallyContracts Clone();
    public override string ToString();
}
```

### Properties

#### Predefined Contract Sets

| Property | Description |
|----------|-------------|
| `BaseSepolia` | Contract addresses for Base Sepolia testnet |
| `Base` | Contract addresses for Base mainnet |
| `Amoy` | Contract addresses for Amoy testnet |
| `AmoyWithPermit` | Contract addresses for Amoy testnet with permit support |
| `Polygon` | Contract addresses for Polygon mainnet |
| `Local` | Contract addresses for local development |
| `Test` | Contract addresses for testing environments |

#### Contract Addresses

| Property | Description |
|----------|-------------|
| `TokenFaucet` | Address of the token faucet contract |
| `RlyERC20` | Address of the RLY ERC20 token contract |

### Methods

#### Clone

```csharp
public RallyContracts Clone()
```

Creates a deep copy of the contracts configuration.

**Returns:**

- A new `RallyContracts` instance with the same values

#### ToString

```csharp
public override string ToString()
```

Returns a string representation of the contracts configuration.

**Returns:**

- String containing the contracts configuration details in the format: `Contracts{tokenFaucet: {TokenFaucet}, rlyERC20: {RlyERC20}}`

## RallyGSNConfig Class

The `RallyGSNConfig` class defines the Gas Station Network (GSN) configuration options for Rally Protocol networks.

```csharp
[System.Serializable]
public class RallyGSNConfig
{
    // Static properties for predefined GSN configurations
    public static RallyGSNConfig BaseSepolia { get; }
    public static RallyGSNConfig Base { get; }
    public static RallyGSNConfig Amoy { get; }
    public static RallyGSNConfig AmoyWithPermit { get; }
    public static RallyGSNConfig Polygon { get; }
    public static RallyGSNConfig Local { get; }
    public static RallyGSNConfig Test { get; }
    
    // Properties
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
    
    // Methods
    public RallyGSNConfig Clone();
    public override string ToString();
}
```

### Properties

#### Predefined GSN Configurations

| Property | Description |
|----------|-------------|
| `BaseSepolia` | GSN configuration for Base Sepolia testnet |
| `Base` | GSN configuration for Base mainnet |
| `Amoy` | GSN configuration for Amoy testnet |
| `AmoyWithPermit` | GSN configuration for Amoy testnet with permit support |
| `Polygon` | GSN configuration for Polygon mainnet |
| `Local` | GSN configuration for local development |
| `Test` | GSN configuration for testing environments |

#### GSN Configuration Properties

| Property | Type | Description |
|----------|------|-------------|
| `PaymasterAddress` | `string` | Address of the GSN paymaster contract |
| `ForwarderAddress` | `string` | Address of the GSN forwarder contract |
| `RelayHubAddress` | `string` | Address of the GSN relay hub contract |
| `RelayWorkerAddress` | `string` | Address of the GSN relay worker |
| `RelayUrl` | `string` | URL of the GSN relay server |
| `RpcUrl` | `string` | RPC URL for blockchain access |
| `ChainId` | `string` | Chain ID of the network |
| `MaxAcceptanceBudget` | `string` | Maximum budget for GSN transactions |
| `DomainSeparatorName` | `string` | Domain separator name for GSN transactions |
| `GtxDataZero` | `int` | Gas cost for zero bytes in GSN transactions |
| `GtxDataNonZero` | `int` | Gas cost for non-zero bytes in GSN transactions |
| `RequestValidSeconds` | `int` | Validity period for GSN requests in seconds |
| `MaxPaymasterDataLength` | `int` | Maximum length of paymaster data |
| `MaxApprovalDataLength` | `int` | Maximum length of approval data |
| `MaxRelayNonceGap` | `int` | Maximum gap allowed in relay nonce |

### Methods

#### Clone

```csharp
public RallyGSNConfig Clone()
```

Creates a deep copy of the GSN configuration.

**Returns:**

- A new `RallyGSNConfig` instance with the same values

#### ToString

```csharp
public override string ToString()
```

Returns a string representation of the GSN configuration.

**Returns:**

- String containing the GSN configuration details

## Usage Examples

### Creating a Network Configuration

```csharp
// Use a predefined network configuration
RallyNetworkConfig config = RallyNetworkConfig.BaseSepolia;

// Set an API key
config.RelayerApiKey = "your-api-key";

// Create a custom configuration by modifying a predefined one
RallyNetworkConfig customConfig = RallyNetworkConfig.BaseSepolia.Clone();
customConfig.Contracts.RlyERC20 = "0xCustomTokenAddress";
customConfig.Gsn.RelayUrl = "https://custom-relay.example.com";
```

### Using the Network Configuration with RallyNetworkFactory

```csharp
// Create a network with a predefined configuration
IRallyNetwork network = RallyNetworkFactory.Create(
    web3Provider,
    httpHandler,
    logger,
    accountManager,
    RallyNetworkConfig.BaseSepolia,
    "your-api-key"
);

// Create a network with a custom configuration
RallyNetworkConfig customConfig = RallyNetworkConfig.BaseSepolia.Clone();
customConfig.Contracts.RlyERC20 = "0xCustomTokenAddress";

IRallyNetwork customNetwork = RallyNetworkFactory.Create(
    web3Provider,
    httpHandler,
    logger,
    accountManager,
    customConfig,
    "your-api-key"
);
```

## Best Practices

1. **Use the Predefined Configurations**: Whenever possible, use the predefined network configurations to ensure compatibility.

2. **Clone Before Modifying**: Always use the `Clone()` method before modifying a predefined configuration to avoid accidentally changing the static instances.

3. **API Key Security**: Handle the RelayerApiKey securely and avoid hardcoding it in your application.

4. **Custom Networks**: When creating configurations for custom networks, ensure all required parameters are properly set, especially contract addresses and GSN configuration.

## Related Classes

- [RallyNetworkType](./RallyNetworkType.md): Enum of supported network types
- [RallyNetworkFactory](./RallyNetworkFactory.md): Factory for creating network instances
- [IRallyNetwork](./IRallyNetwork.md): Interface for interacting with the network
