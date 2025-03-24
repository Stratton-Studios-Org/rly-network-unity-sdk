# Rally Network Factory

The Rally Network Factory provides methods to create instances of the Rally Network for interacting with blockchain networks.

## RallyNetworkFactory

The `RallyNetworkFactory` class provides static methods for creating Rally Network instances.

```csharp
public class RallyNetworkFactory
{
    public static IRallyNetwork Create(
        IRallyWeb3Provider web3Provider, 
        IRallyHttpHandler httpHandler, 
        IRallyLogger logger, 
        IRallyAccountManager accountManager, 
        RallyNetworkType type, 
        string? apiKey = null);
        
    public static IRallyNetwork Create(
        IRallyWeb3Provider web3Provider, 
        IRallyHttpHandler httpHandler, 
        IRallyLogger logger, 
        IRallyAccountManager accountManager, 
        RallyNetworkConfig config, 
        string? apiKey = null);
}
```

## Methods

### Create (with network type)

```csharp
public static IRallyNetwork Create(
    IRallyWeb3Provider web3Provider, 
    IRallyHttpHandler httpHandler, 
    IRallyLogger logger, 
    IRallyAccountManager accountManager, 
    RallyNetworkType type, 
    string? apiKey = null)
```

Creates a Rally Network instance using predefined configuration for the specified network type.

**Parameters:**

- `web3Provider`: Provider for Web3 functionality
- `httpHandler`: Handler for HTTP requests
- `logger`: Logger for the network operations
- `accountManager`: Manager for handling accounts
- `type`: The type of network to create (e.g., BaseSepolia, Base, Polygon)
- `apiKey` (optional): API key for Rally Protocol services

**Returns:**

- `IRallyNetwork`: An implementation of the Rally Network interface

### Create (with custom config)

```csharp
public static IRallyNetwork Create(
    IRallyWeb3Provider web3Provider, 
    IRallyHttpHandler httpHandler, 
    IRallyLogger logger, 
    IRallyAccountManager accountManager, 
    RallyNetworkConfig config, 
    string? apiKey = null)
```

Creates a Rally Network instance using a custom network configuration.

**Parameters:**

- `web3Provider`: Provider for Web3 functionality
- `httpHandler`: Handler for HTTP requests
- `logger`: Logger for the network operations
- `accountManager`: Manager for handling accounts
- `config`: Custom network configuration
- `apiKey` (optional): API key for Rally Protocol services

**Returns:**

- `IRallyNetwork`: An implementation of the Rally Network interface

## Implementation Notes

The factory creates a `RallyEvmNetwork` internally, which implements the `IRallyNetwork` interface:

```csharp
// Implementation excerpt from RallyNetworkFactory.Create
RallyEvmNetwork network = new(web3Provider, httpHandler, logger, accountManager, config);
if (!string.IsNullOrEmpty(apiKey))
{
    network.SetApiKey(apiKey);
}
return network;
```

## Network Types

The `RallyNetworkType` enum defines the supported network configurations:

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

## Usage Example

```csharp
// Dependencies would need to be provided or injected in your application
IRallyWeb3Provider web3Provider = /* create or obtain instance */;
IRallyHttpHandler httpHandler = /* create or obtain instance */;
IRallyLogger logger = /* create or obtain instance */;
IRallyAccountManager accountManager = /* create or obtain instance */;

// Create network using a predefined network type
IRallyNetwork network = RallyNetworkFactory.Create(
    web3Provider,
    httpHandler,
    logger,
    accountManager,
    RallyNetworkType.BaseSepolia,
    "your-api-key"
);

// Example with custom configuration
RallyNetworkConfig customConfig = /* create custom configuration */;
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

1. **Component Initialization**: Ensure all required components (web3Provider, httpHandler, logger, accountManager) are properly initialized before creating a network instance.

2. **API Key Management**: Never hardcode API keys in your scripts. Instead, use a secure configuration system or environment variables.

3. **Network Type Selection**:
   - Use `BaseSepolia` for development and testing.
   - Use `Base` for production on Base mainnet.
   - Use `Custom` when you need to specify custom contract addresses or configurations.

4. **Error Handling**: Always implement proper error handling when interacting with blockchain functions, as network operations can fail due to various reasons.

```csharp
using System;
using RallyProtocol.Networks;
using UnityEngine;

public class ErrorHandlingExample : MonoBehaviour
{
    private IRallyNetwork rlyNetwork;
    
    private async void TransferTokens(string destinationAddress, decimal amount)
    {
        try
        {
            string txHash = await rlyNetwork.TransferAsync(destinationAddress, amount);
            Debug.Log($"Transfer successful! Transaction hash: {txHash}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Transfer failed: {ex.Message}");
            // Handle the error appropriately
        }
    }
}
```
