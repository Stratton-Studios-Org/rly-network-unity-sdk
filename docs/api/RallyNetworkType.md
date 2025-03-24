# RallyNetworkType

The `RallyNetworkType` enum defines the supported blockchain networks in the Rally Protocol Unity SDK.

## Enum Definition

```csharp
public enum RallyNetworkType
{
    Amoy,
    Polygon,
    Local,
    Test,
    Custom,
    BaseSepolia,
    Base
}
```

## Values

| Value | Description |
|-------|-------------|
| `Amoy` | Amoy testnet network. |
| `Polygon` | Polygon mainnet network. |
| `Local` | Local development network. |
| `Test` | Configuration for testing purposes. |
| `Custom` | Custom network configuration. |
| `BaseSepolia` | Base Sepolia testnet network. |
| `Base` | Base mainnet network. |

## Usage

The `RallyNetworkType` enum is used to specify which network to connect to when creating a Rally Network instance:

```csharp
// Create a network instance for Base Sepolia testnet
IRallyNetwork network = RallyNetworkFactory.Create(
    web3Provider,
    httpHandler,
    logger,
    accountManager,
    RallyNetworkType.BaseSepolia,
    "your-api-key"
);
```

It's also used in the RallyProtocolSettingsPreset to specify which network to use:

```csharp
// In a MonoBehaviour script
[SerializeField] private RallyProtocolSettingsPreset settingsPreset;

private void Start()
{
    // Get the network type from the settings preset
    RallyNetworkType networkType = settingsPreset.NetworkType;
    
    // Use it to create a network
    // ...
}
```

## Network Selection Recommendations

### Development and Testing

For development and testing purposes, it's recommended to use one of the testnet configurations:

- `RallyNetworkType.BaseSepolia`: The Base Sepolia testnet, which provides a stable environment for testing without using real assets.
- `RallyNetworkType.Amoy`: The Amoy testnet.
- `RallyNetworkType.Local`: For local development and testing, particularly useful with local blockchain nodes.
- `RallyNetworkType.Test`: For automated tests and CI/CD environments.

### Production

For production applications, use one of the mainnet configurations:

- `RallyNetworkType.Base`: The Base mainnet.
- `RallyNetworkType.Polygon`: The Polygon mainnet.

### Custom Networks

The `RallyNetworkType.Custom` value is used when you need to connect to a network that's not part of the predefined options. When using this option, you need to provide a complete custom configuration with all required contract addresses and settings:

```csharp
// Create a custom network configuration
RallyNetworkConfig customConfig = RallyNetworkConfig.BaseSepolia.Clone();
customConfig.Contracts.RlyERC20 = "0xCustomTokenAddress";
customConfig.Gsn.RelayUrl = "https://custom-relay.example.com";

// Use the custom configuration
IRallyNetwork network = RallyNetworkFactory.Create(
    web3Provider,
    httpHandler,
    logger,
    accountManager,
    customConfig,
    "your-api-key"
);
```

## Related Classes

- [RallyNetworkConfig](./RallyNetworkConfig.md): Contains predefined configurations for each network type
- [RallyNetworkFactory](./RallyNetworkFactory.md): Factory for creating network instances
- [IRallyNetwork](./IRallyNetwork.md): Interface for interacting with the network
