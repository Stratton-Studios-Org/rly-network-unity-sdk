# API Keys

This document explains how to set up and use API keys with the Rally Protocol Unity SDK.

## Overview

API keys are used to authenticate your application with Rally Protocol services. They are required for certain operations, particularly those that interact with Rally backend services.

## Setting Up an API Key

### Getting an API Key

1. Visit the [Rally Protocol Developer Portal](https://app.rallyprotocol.com) (URL may change)
2. Create a developer account or log in to your existing account
3. Create a new application or select an existing application
4. Generate an API key for your application
5. Note down the API key for use in your application

### Securing Your API Key

Your API key should be kept secure and not exposed in client-side code without proper protection. Consider these best practices:

1. **Never hardcode API keys** directly in your source code
2. **Use environment variables** or secure configuration management
3. In production, consider implementing a backend service to handle sensitive API operations
4. For mobile and desktop games, implement obfuscation techniques

## Using API Keys in the SDK

### Setting the API Key

The API key can be set through the `IRallyNetwork` interface:

```csharp
// Get a reference to the Rally Network
IRallyNetwork rlyNetwork = RallyUnityManager.Instance.RlyNetwork;

// Set the API key
rlyNetwork.SetApiKey("your-api-key-here");
```

### Setting API Key in RallyUnityManager

If you're using the `RallyUnityManager` component, you can set the API key in the Inspector or via code:

```csharp
// Set the API key directly on the manager
RallyUnityManager.Instance.ApiKey = "your-api-key-here";

// The API key will be applied automatically during initialization
await RallyUnityManager.Instance.InitializeAsync();
```

### Setting API Key at Runtime

If you need to change the API key at runtime:

```csharp
// Update the API key at runtime
RallyUnityManager.Instance.ApiKey = "new-api-key";
rlyNetwork.SetApiKey("new-api-key");
```

## Operations Requiring API Keys

The following operations typically require a valid API key:

- Claiming RLY tokens from the faucet
- Using gasless transactions with the Gas Station Network (GSN)
- Accessing certain Rally ecosystem services
- Retrieving token metadata and analytics

Example with API key for token claim:

```csharp
try
{
    // Ensure API key is set before claiming tokens
    rlyNetwork.SetApiKey("your-api-key-here");
    
    // Claim RLY tokens
    string txHash = await rlyNetwork.ClaimRlyAsync();
    Debug.Log($"Successfully claimed RLY tokens. Transaction: {txHash}");
}
catch (RallyNetworkException ex)
{
    // Handle API key issues
    if (ex.Message.Contains("API key"))
    {
        Debug.LogError("API key error. Please check your API key.");
    }
    else
    {
        Debug.LogError($"Failed to claim tokens: {ex.Message}");
    }
}
```

## API Key Management Strategies

### Development vs. Production Keys

It's recommended to use different API keys for development and production:

```csharp
void SetEnvironmentApiKey()
{
    #if DEBUG
        // Development API key
        rlyNetwork.SetApiKey("dev-api-key");
    #else
        // Production API key
        rlyNetwork.SetApiKey("prod-api-key");
    #endif
}
```

### Loading API Keys from Configuration

For better security, load API keys from a configuration file or secure storage:

```csharp
async Task LoadApiKeyFromConfigAsync()
{
    // Example: Load from a configuration file
    TextAsset configFile = Resources.Load<TextAsset>("rally_config");
    if (configFile != null)
    {
        var config = JsonUtility.FromJson<RallyConfig>(configFile.text);
        rlyNetwork.SetApiKey(config.apiKey);
    }
    else
    {
        Debug.LogError("Could not load Rally configuration.");
    }
}

// Simple configuration class
[System.Serializable]
private class RallyConfig
{
    public string apiKey;
}
```

### Runtime Key Distribution

For production applications, consider distributing API keys securely at runtime:

```csharp
async Task FetchApiKeyFromSecureServerAsync()
{
    // Example: Fetch API key from a secure server
    // This would typically involve authentication to ensure only valid users receive the key
    using (UnityWebRequest request = UnityWebRequest.Get("https://your-secure-server.com/api/rally-key"))
    {
        // Add authentication headers
        request.SetRequestHeader("Authorization", "Bearer " + userAuthToken);
        
        await request.SendWebRequest();
        
        if (request.result == UnityWebRequest.Result.Success)
        {
            var response = JsonUtility.FromJson<KeyResponse>(request.downloadHandler.text);
            rlyNetwork.SetApiKey(response.apiKey);
        }
        else
        {
            Debug.LogError($"Failed to retrieve API key: {request.error}");
        }
    }
}

[System.Serializable]
private class KeyResponse
{
    public string apiKey;
}
```

## Security Best Practices

1. **Revoke compromised keys** immediately via the developer portal
2. **Rotate API keys** periodically for enhanced security
3. **Implement rate limiting** on your backend to prevent abuse
4. **Monitor API usage** for unexpected patterns that might indicate a compromised key
5. **Use key permissions** to limit what each API key can access
6. **Implement IP restrictions** if supported by the Rally Protocol services

## Troubleshooting API Key Issues

If you encounter issues with your API key:

1. Verify the API key is correct and not expired
2. Check that the API key has the necessary permissions
3. Ensure the API key is being correctly set before operations that require it
4. Look for error messages that specifically mention API key issues
5. Test with a new API key to rule out key-specific issues

Error handling example:

```csharp
try
{
    // Operation requiring API key
    string txHash = await rlyNetwork.ClaimRlyAsync();
}
catch (RallyNetworkException ex)
{
    // Common API key error messages
    if (ex.Message.Contains("Invalid API key"))
    {
        Debug.LogError("The API key is invalid. Please generate a new one.");
    }
    else if (ex.Message.Contains("API key expired"))
    {
        Debug.LogError("The API key has expired. Please refresh it.");
    }
    else if (ex.Message.Contains("API key missing"))
    {
        Debug.LogError("No API key was provided. Set the API key before calling this method.");
    }
    else if (ex.Message.Contains("API key unauthorized"))
    {
        Debug.LogError("The API key doesn't have permission for this operation.");
    }
    else
    {
        Debug.LogError($"Operation failed: {ex.Message}");
    }
}
```

## Related Documentation

- [RallyNetworkFactory](./RallyNetworkFactory.md) - Factory class for creating network instances
- [IRallyNetwork](./IRallyNetwork.md) - Main interface with SetApiKey method
- [Security Best Practices](./security-best-practices.md) - General security recommendations
