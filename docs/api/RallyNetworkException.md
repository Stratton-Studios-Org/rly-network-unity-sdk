# RallyNetworkException

The `RallyNetworkException` class represents exceptions specific to network operations in the Rally Protocol Unity SDK.

## Class Definition

```csharp
using System;

namespace RallyProtocol.Exceptions
{
    public class RallyNetworkException : RallyException
    {
        public RallyNetworkException() : base() { }
        
        public RallyNetworkException(string message) : base(message) { }
        
        public RallyNetworkException(string message, Exception innerException) 
            : base(message, innerException) { }
    }
}
```

## Overview

The `RallyNetworkException` is thrown when errors occur during network-related operations, such as:

- Blockchain connectivity issues
- RPC endpoint failures
- Transaction submission errors
- Network timeouts
- API communication errors
- Web3 provider issues

This exception helps distinguish network-related issues from other types of problems in the Rally Protocol SDK.

## Common Error Scenarios

### Connection Issues

Thrown when the SDK cannot connect to the blockchain network.

```csharp
try
{
    decimal balance = await rlyNetwork.GetDisplayBalanceAsync();
}
catch (RallyNetworkException ex)
{
    if (ex.Message.Contains("connection") || ex.Message.Contains("connect"))
    {
        Debug.LogWarning("Could not connect to the network. Please check your internet connection.");
        ShowNetworkConnectionErrorUI();
    }
    else
    {
        Debug.LogError($"Network error: {ex.Message}");
    }
}
```

### RPC Errors

Thrown when the RPC endpoint returns an error.

```csharp
try
{
    string txHash = await rlyNetwork.TransferAsync("0xRecipient", 10.0m);
}
catch (RallyNetworkException ex)
{
    if (ex.Message.Contains("RPC") || ex.Message.Contains("provider"))
    {
        Debug.LogError($"RPC error: {ex.Message}");
        ShowRpcErrorUI("There's an issue with the network provider. Please try again later.");
    }
    else
    {
        Debug.LogError($"Network error: {ex.Message}");
    }
}
```

### Transaction Timeouts

Thrown when a transaction takes too long to be processed.

```csharp
try
{
    string txHash = await rlyNetwork.TransferAsync("0xRecipient", 10.0m);
}
catch (RallyNetworkException ex)
{
    if (ex.Message.Contains("timeout") || ex.Message.Contains("timed out"))
    {
        Debug.LogWarning("Transaction timed out. The network may be congested.");
        ShowTimeoutUI("Your transaction is taking longer than expected. You can check its status later.");
    }
    else
    {
        Debug.LogError($"Network error: {ex.Message}");
    }
}
```

### API Communication Errors

Thrown when there are issues communicating with Rally Protocol APIs.

```csharp
try
{
    // Operation that uses Rally API
    string txHash = await rlyNetwork.ClaimRlyAsync();
}
catch (RallyNetworkException ex)
{
    if (ex.Message.Contains("API") || ex.Message.Contains("service"))
    {
        Debug.LogError($"API error: {ex.Message}");
        ShowApiErrorUI("There was an issue with the Rally service. Please try again later.");
    }
    else
    {
        Debug.LogError($"Network error: {ex.Message}");
    }
}
```

### Chain ID Mismatch

Thrown when there's a mismatch between the configured chain ID and the connected network.

```csharp
try
{
    // Operation that involves chain ID
    await rlyNetwork.GetProviderAsync();
}
catch (RallyNetworkException ex)
{
    if (ex.Message.Contains("chain ID") || ex.Message.Contains("network ID"))
    {
        Debug.LogError("Chain ID mismatch. The configured network doesn't match the connected network.");
        ShowNetworkMismatchUI();
    }
    else
    {
        Debug.LogError($"Network error: {ex.Message}");
    }
}
```

### Rate Limiting

Thrown when API or RPC endpoints impose rate limits.

```csharp
try
{
    // Multiple operations in quick succession
    for (int i = 0; i < 10; i++)
    {
        await rlyNetwork.GetDisplayBalanceAsync();
    }
}
catch (RallyNetworkException ex)
{
    if (ex.Message.Contains("rate limit") || ex.Message.Contains("too many requests"))
    {
        Debug.LogWarning("Rate limit exceeded. Implementing backoff strategy.");
        await ImplementBackoffStrategy();
    }
    else
    {
        Debug.LogError($"Network error: {ex.Message}");
    }
}

async Task ImplementBackoffStrategy()
{
    // Simple exponential backoff
    int attempt = 1;
    int maxAttempts = 5;
    int baseDelay = 1000; // milliseconds
    
    while (attempt <= maxAttempts)
    {
        try
        {
            int delay = baseDelay * (int)Math.Pow(2, attempt - 1);
            Debug.Log($"Backing off for {delay}ms before retry");
            await Task.Delay(delay);
            
            // Retry operation
            await rlyNetwork.GetDisplayBalanceAsync();
            
            // Success, break the loop
            Debug.Log("Operation succeeded after backoff");
            break;
        }
        catch (RallyNetworkException ex)
        {
            attempt++;
            
            if (attempt > maxAttempts)
            {
                Debug.LogError("Max retry attempts reached");
                throw;
            }
        }
    }
}
```

## Handling RallyNetworkException

### Basic Error Handling

```csharp
try
{
    // Network operation
    decimal balance = await rlyNetwork.GetDisplayBalanceAsync();
}
catch (RallyNetworkException ex)
{
    Debug.LogError($"Network error: {ex.Message}");
    ShowErrorUI("There was a network issue. Please check your connection and try again.");
}
```

### Advanced Error Handling

For more sophisticated error handling, categorize the exceptions based on the message:

```csharp
try
{
    // Network operation
    decimal balance = await rlyNetwork.GetDisplayBalanceAsync();
}
catch (RallyNetworkException ex)
{
    string message = ex.Message.ToLower();
    
    if (message.Contains("connection") || message.Contains("connect") || message.Contains("internet"))
    {
        HandleConnectionError(ex);
    }
    else if (message.Contains("rpc") || message.Contains("provider"))
    {
        HandleRpcError(ex);
    }
    else if (message.Contains("timeout") || message.Contains("timed out"))
    {
        HandleTimeoutError(ex);
    }
    else if (message.Contains("api") || message.Contains("service"))
    {
        HandleApiError(ex);
    }
    else if (message.Contains("chain id") || message.Contains("network id"))
    {
        HandleChainIdError(ex);
    }
    else if (message.Contains("rate limit") || message.Contains("too many requests"))
    {
        HandleRateLimitError(ex);
    }
    else
    {
        // General network error
        HandleGeneralNetworkError(ex);
    }
}

private void HandleConnectionError(RallyNetworkException ex)
{
    Debug.LogWarning($"Connection issue: {ex.Message}");
    ShowNetworkConnectionErrorUI();
}

private void HandleRpcError(RallyNetworkException ex)
{
    Debug.LogError($"RPC error: {ex.Message}");
    ShowRpcErrorUI();
}

// ... other error handlers
```

## Best Practices

### Prevention

1. **Check Network Connectivity**: Verify network connectivity before performing blockchain operations.

```csharp
public async Task<bool> IsNetworkAvailable()
{
    try
    {
        // Simple ping to check network connectivity
        Web3 web3 = await rlyNetwork.GetProviderAsync();
        var blockNumber = await web3.Eth.Blocks.GetBlockNumber.SendRequestAsync();
        return true;
    }
    catch (RallyNetworkException)
    {
        return false;
    }
}

// Usage
if (await IsNetworkAvailable())
{
    // Proceed with network operations
    await PerformBlockchainOperation();
}
else
{
    ShowNoNetworkUI("Please check your internet connection and try again.");
}
```

2. **Implement Retry Logic**: Automatically retry failed operations with backoff.

```csharp
public async Task<T> WithRetry<T>(Func<Task<T>> operation, int maxAttempts = 3)
{
    int attempt = 1;
    while (true)
    {
        try
        {
            return await operation();
        }
        catch (RallyNetworkException ex)
        {
            if (attempt >= maxAttempts)
            {
                Debug.LogError($"Operation failed after {maxAttempts} attempts: {ex.Message}");
                throw;
            }
            
            int delay = 1000 * (int)Math.Pow(2, attempt - 1);
            Debug.Log($"Attempt {attempt} failed, retrying in {delay}ms: {ex.Message}");
            await Task.Delay(delay);
            attempt++;
        }
    }
}

// Usage
try
{
    decimal balance = await WithRetry(() => rlyNetwork.GetDisplayBalanceAsync());
    Debug.Log($"Balance: {balance}");
}
catch (RallyNetworkException ex)
{
    Debug.LogError($"Failed after retries: {ex.Message}");
    ShowErrorUI("Network operation failed. Please try again later.");
}
```

3. **Use Timeouts**: Set reasonable timeouts for network operations.

```csharp
public async Task<T> WithTimeout<T>(Func<Task<T>> operation, int timeoutMs = 10000)
{
    var operationTask = operation();
    var timeoutTask = Task.Delay(timeoutMs);
    
    var completedTask = await Task.WhenAny(operationTask, timeoutTask);
    if (completedTask == timeoutTask)
    {
        throw new RallyNetworkException("Operation timed out");
    }
    
    return await operationTask;
}

// Usage
try
{
    decimal balance = await WithTimeout(() => rlyNetwork.GetDisplayBalanceAsync(), 5000);
    Debug.Log($"Balance: {balance}");
}
catch (RallyNetworkException ex)
{
    Debug.LogError($"Network error: {ex.Message}");
    if (ex.Message.Contains("timed out"))
    {
        ShowTimeoutUI();
    }
    else
    {
        ShowErrorUI("Network operation failed.");
    }
}
```

### Recovery

1. **Fallback Networks**: Implement fallback to alternative networks when primary networks fail.

```csharp
async Task<IRallyNetwork> GetNetworkWithFallback()
{
    try
    {
        // Try primary network (Base)
        var primaryConfig = RallyProtocolSettingsPreset.GetNetworkConfigFor(RallyNetworkType.Base);
        return await RallyNetworkFactory.CreateNetworkAsync(primaryConfig);
    }
    catch (RallyNetworkException ex)
    {
        Debug.LogWarning($"Primary network failed: {ex.Message}. Trying fallback...");
        
        try
        {
            // Try fallback network (Polygon)
            var fallbackConfig = RallyProtocolSettingsPreset.GetNetworkConfigFor(RallyNetworkType.Polygon);
            return await RallyNetworkFactory.CreateNetworkAsync(fallbackConfig);
        }
        catch (RallyNetworkException fallbackEx)
        {
            Debug.LogError($"Fallback network also failed: {fallbackEx.Message}");
            throw new RallyNetworkException("All networks unavailable", fallbackEx);
        }
    }
}
```

2. **Cache Critical Data**: Cache critical data to provide offline functionality.

```csharp
decimal cachedBalance = 0;
DateTime lastBalanceUpdate = DateTime.MinValue;

async Task<decimal> GetBalanceWithCaching()
{
    try
    {
        // Try to get fresh balance
        decimal balance = await rlyNetwork.GetDisplayBalanceAsync();
        
        // Update cache
        cachedBalance = balance;
        lastBalanceUpdate = DateTime.Now;
        
        return balance;
    }
    catch (RallyNetworkException)
    {
        // Return cached balance if relatively fresh (last 5 minutes)
        if (DateTime.Now - lastBalanceUpdate < TimeSpan.FromMinutes(5))
        {
            Debug.Log("Using cached balance due to network error");
            return cachedBalance;
        }
        
        // Otherwise, rethrow the exception
        throw;
    }
}
```

3. **Provide User Feedback**: Keep users informed during network operations.

```csharp
async Task PerformNetworkOperationWithFeedback()
{
    ShowLoadingUI("Connecting to network...");
    
    try
    {
        decimal balance = await rlyNetwork.GetDisplayBalanceAsync();
        HideLoadingUI();
        ShowSuccessUI($"Balance: {balance}");
    }
    catch (RallyNetworkException ex)
    {
        HideLoadingUI();
        ShowErrorUI($"Network error: {ex.Message}");
    }
}
```

## Related Documentation

- [RallyException](./RallyException.md): Base exception class for Rally Protocol
- [Network Configuration](./network-configuration.md): Network configuration options
- [RallyNetworkType](./RallyNetworkType.md): Supported network types
- [RallyNetworkFactory](./RallyNetworkFactory.md): Factory for creating network instances
