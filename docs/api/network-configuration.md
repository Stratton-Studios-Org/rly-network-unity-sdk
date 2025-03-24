# Network Configuration

This document explains how to configure network settings for the Rally Protocol Unity SDK.

## Network Types

The Rally Protocol Unity SDK supports various blockchain networks, which are defined in the `RallyNetworkType` enum:

```csharp
public enum RallyNetworkType
{
    Amoy,       // Amoy testnet
    Polygon,    // Polygon mainnet
    Local,      // Local development network
    Test,       // General test network
    Custom,     // Custom network configuration
    BaseSepolia, // Base Sepolia testnet
    Base        // Base mainnet
}
```

## RallyNetworkConfig

The `RallyNetworkConfig` class is used to configure network parameters:

```csharp
public class RallyNetworkConfig
{
    // Network identification
    public RallyNetworkType NetworkType { get; set; }
    public string ChainName { get; set; }
    public int ChainId { get; set; }
    
    // RPC endpoints
    public string RpcUrl { get; set; }
    public string WebSocketRpcUrl { get; set; }
    
    // Contract addresses
    public string RelayHubAddress { get; set; }
    public string TokenAddress { get; set; }
    public string StakingAddress { get; set; }
    public string PaymasterAddress { get; set; }
    
    // GSN configuration
    public GsnConfig GsnConfig { get; set; }
    
    // Additional options
    public bool EnableDebugLogs { get; set; }
    public int RetryAttempts { get; set; }
    public TimeSpan RetryDelay { get; set; }
}
```

## Creating Network Configurations

### Using Built-in Presets

The easiest way to configure a network is to use the built-in presets:

```csharp
// Create a configuration for Polygon mainnet
RallyNetworkConfig polygonConfig = RallyProtocolSettingsPreset.GetNetworkConfigFor(RallyNetworkType.Polygon);

// Create a configuration for Base Sepolia testnet
RallyNetworkConfig baseSepoliaConfig = RallyProtocolSettingsPreset.GetNetworkConfigFor(RallyNetworkType.BaseSepolia);
```

### Creating Custom Configurations

For custom networks, you can create a configuration manually:

```csharp
// Create a custom network configuration
RallyNetworkConfig customConfig = new RallyNetworkConfig
{
    NetworkType = RallyNetworkType.Custom,
    ChainName = "My Custom Network",
    ChainId = 123456,
    RpcUrl = "https://mycustomnetwork.example.com/rpc",
    WebSocketRpcUrl = "wss://mycustomnetwork.example.com/ws",
    RelayHubAddress = "0xRelayHubAddress",
    TokenAddress = "0xTokenAddress",
    StakingAddress = "0xStakingAddress",
    PaymasterAddress = "0xPaymasterAddress",
    EnableDebugLogs = true,
    RetryAttempts = 3,
    RetryDelay = TimeSpan.FromSeconds(2),
    GsnConfig = new GsnConfig
    {
        PaymasterAddress = "0xPaymasterAddress",
        RelayHubAddress = "0xRelayHubAddress",
        ChainId = 123456,
        MaxAcceptanceBudget = 100000,
        DomainSeparatorName = "My App"
    }
};
```

## Initializing with Network Configuration

### Using RallyNetworkFactory

The recommended way to create a Rally Network instance is to use the `RallyNetworkFactory`:

```csharp
// Get a network configuration
RallyNetworkConfig config = RallyProtocolSettingsPreset.GetNetworkConfigFor(RallyNetworkType.Polygon);

// Create a network instance
IRallyNetwork network = await RallyNetworkFactory.CreateNetworkAsync(config);
```

### With RallyUnityManager

If you're using the Unity-specific components, you can set the configuration on the `RallyUnityManager`:

```csharp
// Get a reference to the RallyUnityManager
RallyUnityManager manager = RallyUnityManager.Instance;

// Set the network configuration
manager.NetworkConfig = RallyProtocolSettingsPreset.GetNetworkConfigFor(RallyNetworkType.Base);

// Initialize the manager (this will create the network with the specified configuration)
await manager.InitializeAsync();

// Access the network
IRallyNetwork network = manager.RlyNetwork;
```

## Setting Network Configuration in the Inspector

If you're using the `RallyUnityManager` component in your Unity scene, you can configure the network directly in the Inspector:

1. Add the `RallyUnityManager` component to a GameObject in your scene
2. In the Inspector, find the "Network Configuration" section
3. Set the "Network Type" to your desired network
4. (Optional) Expand the "Custom Config" section to override specific settings

## Switching Networks at Runtime

To switch networks at runtime:

```csharp
// Get a reference to the RallyUnityManager
RallyUnityManager manager = RallyUnityManager.Instance;

// Create a new network configuration
RallyNetworkConfig newConfig = RallyProtocolSettingsPreset.GetNetworkConfigFor(RallyNetworkType.Base);

// Set the new configuration
manager.NetworkConfig = newConfig;

// Reinitialize with the new configuration
await manager.InitializeAsync();
```

## GSN Configuration

The Gas Station Network (GSN) can be configured using the `GsnConfig` property:

```csharp
// Create a network configuration
RallyNetworkConfig config = RallyProtocolSettingsPreset.GetNetworkConfigFor(RallyNetworkType.Polygon);

// Customize GSN configuration
config.GsnConfig = new GsnConfig
{
    PaymasterAddress = "0xCustomPaymasterAddress",
    RelayHubAddress = "0xCustomRelayHubAddress",
    ChainId = config.ChainId,
    MaxAcceptanceBudget = 150000,
    DomainSeparatorName = "My Game"
};

// Create a network instance with the custom GSN configuration
IRallyNetwork network = await RallyNetworkFactory.CreateNetworkAsync(config);
```

## Error Handling

When working with network configurations, proper error handling is important:

```csharp
try
{
    // Get a network configuration
    RallyNetworkConfig config = RallyProtocolSettingsPreset.GetNetworkConfigFor(RallyNetworkType.Polygon);
    
    // Create a network instance
    IRallyNetwork network = await RallyNetworkFactory.CreateNetworkAsync(config);
    
    Debug.Log($"Successfully connected to {config.ChainName}");
}
catch (RallyNetworkException ex)
{
    Debug.LogError($"Failed to initialize network: {ex.Message}");
    
    // You might want to fall back to a different network
    try
    {
        Debug.Log("Falling back to Base Sepolia testnet...");
        RallyNetworkConfig fallbackConfig = RallyProtocolSettingsPreset.GetNetworkConfigFor(RallyNetworkType.BaseSepolia);
        IRallyNetwork fallbackNetwork = await RallyNetworkFactory.CreateNetworkAsync(fallbackConfig);
        
        Debug.Log("Successfully connected to fallback network");
    }
    catch (Exception fallbackEx)
    {
        Debug.LogError($"Failed to connect to fallback network: {fallbackEx.Message}");
    }
}
```

## Best Practices

1. **Use presets for standard networks**: The built-in presets contain optimized settings for each network.
2. **Cache network instances**: Creating network instances can be resource-intensive, so cache them when possible.
3. **Implement proper error handling**: Always handle exceptions that may occur during network initialization.
4. **Provide fallback options**: Consider implementing a fallback to an alternative network if the primary one fails.
5. **Configure retry attempts**: Set appropriate retry values to handle temporary network issues.
6. **Test on multiple networks**: Ensure your application works correctly on all networks you plan to support.
7. **Consider network-specific optimizations**: Adjust gas settings and other parameters based on the network's characteristics.
