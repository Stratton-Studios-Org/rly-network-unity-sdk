# Custom Network Integration

The Rally Protocol Unity SDK provides built-in support for standard networks like Polygon, but also allows you to integrate with custom networks. This guide explains how to configure and use custom networks with the SDK.

## Using Network Settings Presets

The easiest way to customize network settings is through the built-in preset system. Network presets define all the parameters needed to connect to a specific blockchain network.

### Using Built-in Presets

The SDK comes with presets for commonly used networks:

```csharp
// Use the Polygon main network preset
var mainnetSettings = RallyUnityNetworkFactory.LoadMainSettingsPreset();
IRallyNetwork mainnetNetwork = RallyUnityNetworkFactory.Create(mainnetSettings);

// Use the Polygon Mumbai testnet preset
var testnetSettings = RallyUnityNetworkFactory.LoadTestSettingsPreset();
IRallyNetwork testnetNetwork = RallyUnityNetworkFactory.Create(testnetSettings);
```

### Creating Custom Network Presets

You can create custom network presets for any EVM-compatible blockchain:

```csharp
// Create a custom network preset
var customSettings = new RallyProtocolSettingsPreset
{
    ChainId = 1234,                                // Network chain ID
    NetworkType = RallyNetworkType.Custom,         // Use Custom type for custom networks
    RpcUrl = "https://my-custom-rpc.example.com",  // RPC endpoint URL
    ContractAddresses = new ContractAddressConfig
    {
        RlyERC20 = "0x123...",                     // RLY token contract address
        FeeCollector = "0x456...",                 // Fee collector contract address
        // Add other required contract addresses...
    },
    RelayerUrl = "https://my-relayer.example.com"  // Optional GSN relayer URL
};

// Create network instance using custom preset
IRallyNetwork customNetwork = RallyUnityNetworkFactory.Create(customSettings);
```

### Saving and Loading Custom Presets

You can save custom presets for reuse:

```csharp
// Save the custom preset
RallyUnityNetworkPresetManager.SavePreset(customSettings, "MyCustomNetwork");

// Load a saved preset
var loadedSettings = RallyUnityNetworkPresetManager.LoadPreset("MyCustomNetwork");
IRallyNetwork network = RallyUnityNetworkFactory.Create(loadedSettings);
```

## Implementing Custom Network Interfaces

For more advanced customization, you can implement the `IRallyNetwork` interface directly:

```csharp
public class MyCustomNetwork : IRallyNetwork
{
    // Implement all required interface methods
    
    public async Task<decimal> GetDisplayBalanceAsync(string tokenAddress = null)
    {
        // Custom implementation for getting token balance
    }
    
    public async Task<string> TransferAsync(string to, decimal amount, 
        MetaTxMethod? metaTxMethod = null, string tokenAddress = null)
    {
        // Custom implementation for transferring tokens
    }
    
    // Implement all other required interface methods...
}

// Use the custom implementation
IRallyNetwork network = new MyCustomNetwork();
RallyUnityManager.Instance.SetRlyNetwork(network);
```

## Creating Network Settings in Unity Inspector

The SDK provides MonoBehaviour components that allow you to configure network settings in the Unity Inspector:

1. Create a new GameObject in your scene
2. Add the `RallyNetworkSettings` component
3. Configure the settings in the Inspector
4. Reference this component when initializing your network

```csharp
// Reference the settings component from your scene
public RallyNetworkSettings networkSettings;

void Start()
{
    // Create a network using the settings from the component
    IRallyNetwork network = RallyUnityNetworkFactory.Create(networkSettings.GetSettings());
    RallyUnityManager.Instance.SetRlyNetwork(network);
}
```

## Testing Custom Networks

When testing with custom networks, you may want to use local development chains:

```csharp
// Create a preset for local development (e.g., Hardhat or Ganache)
var localDevSettings = new RallyProtocolSettingsPreset
{
    ChainId = 31337,                           // Local chain ID (Hardhat default)
    NetworkType = RallyNetworkType.Development,
    RpcUrl = "http://localhost:8545",          // Local RPC endpoint
    ContractAddresses = new ContractAddressConfig
    {
        // Use contract addresses from your local deployment
        RlyERC20 = "0x123...",
        FeeCollector = "0x456...",
        // Other required contract addresses...
    }
};

// Create network instance for local development
IRallyNetwork localNetwork = RallyUnityNetworkFactory.Create(localDevSettings);
```

## Best Practices for Custom Networks

1. **Security Considerations**: Ensure your custom RPC endpoint is secure, especially when using in production.

2. **Contract Compatibility**: Verify that the smart contracts on your custom network are compatible with the Rally Protocol SDK.

3. **Testing**: Thoroughly test custom network implementations with both regular and meta transactions.

4. **API Key Management**: If your custom RPC requires API keys, store them securely.

```csharp
// Example of setting API key for a custom network
IRallyNetwork network = RallyUnityNetworkFactory.Create(customSettings);
network.SetApiKey("your-secure-api-key");
```

5. **Error Handling**: Implement robust error handling for custom network operations.

6. **Fallback RPC Endpoints**: Consider providing fallback RPC endpoints for better reliability.

```csharp
var customSettings = new RallyProtocolSettingsPreset
{
    // Primary RPC URL
    RpcUrl = "https://primary-rpc.example.com",
    
    // Fallback RPC URLs
    FallbackRpcUrls = new string[]
    {
        "https://fallback1-rpc.example.com",
        "https://fallback2-rpc.example.com"
    },
    // Other settings...
};
```

## Related Documentation

- [IRallyNetwork](./IRallyNetwork.md) - Interface documentation for implementing custom networks
- [Network Configuration](./network-configuration.md) - Detailed network configuration options
