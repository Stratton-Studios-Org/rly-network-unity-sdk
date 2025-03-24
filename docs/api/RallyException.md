# RallyException

The `RallyException` class serves as the base exception class for the Rally Protocol Unity SDK. All specialized exceptions in the SDK inherit from this base class.

## Class Definition

```csharp
using System;

namespace RallyProtocol.Exceptions
{
    public class RallyException : Exception
    {
        public RallyException() : base() { }
        
        public RallyException(string message) : base(message) { }
        
        public RallyException(string message, Exception innerException) 
            : base(message, innerException) { }
    }
}
```

## Usage

The `RallyException` class is the parent class for all exceptions thrown by the Rally Protocol SDK. You can catch this exception type to handle all Rally-related errors, or catch more specific exception types for targeted error handling.

### Basic Exception Handling

```csharp
try
{
    // Rally Protocol operations
    await rlyNetwork.ClaimRlyAsync();
}
catch (RallyException ex)
{
    // Handle any Rally Protocol-related exception
    Debug.LogError($"Rally Protocol error: {ex.Message}");
}
```

### Specialized Exception Handling

```csharp
try
{
    // Rally Protocol operations
    string txHash = await rlyNetwork.TransferAsync("0xRecipient", 10.0m);
}
catch (RallyAccountException ex)
{
    // Handle account-related exceptions
    Debug.LogError($"Account error: {ex.Message}");
}
catch (RallyNetworkException ex)
{
    // Handle network-related exceptions
    Debug.LogError($"Network error: {ex.Message}");
}
catch (RallyException ex)
{
    // Handle other Rally Protocol exceptions
    Debug.LogError($"Other Rally error: {ex.Message}");
}
catch (Exception ex)
{
    // Handle non-Rally exceptions
    Debug.LogError($"General error: {ex.Message}");
}
```

## Properties

The `RallyException` class inherits all properties from the standard `System.Exception` class:

| Property | Type | Description |
|----------|------|-------------|
| `Message` | `string` | Gets a message that describes the current exception |
| `InnerException` | `Exception` | Gets the Exception instance that caused the current exception |
| `StackTrace` | `string` | Gets a string representation of the stack trace |
| `Source` | `string` | Gets or sets the name of the application or object that caused the error |
| `HelpLink` | `string` | Gets or sets a link to the help file associated with this exception |

## Derived Exception Classes

The Rally Protocol Unity SDK includes several specialized exception types that inherit from `RallyException`:

### RallyAccountException

Thrown for errors related to account operations, such as creation, loading, or key management.

```csharp
// Example of RallyAccountException
try
{
    await rlyNetwork.AccountManager.GetAccountPhraseAsync();
}
catch (RallyAccountException ex)
{
    if (ex.Message.Contains("no account exists"))
    {
        Debug.LogWarning("No account found. Create an account first.");
    }
    else
    {
        Debug.LogError($"Account error: {ex.Message}");
    }
}
```

### RallyNetworkException

Thrown for errors related to network operations, such as connectivity issues or failed transactions.

```csharp
// Example of RallyNetworkException
try
{
    await rlyNetwork.GetDisplayBalanceAsync();
}
catch (RallyNetworkException ex)
{
    if (ex.Message.Contains("connection"))
    {
        Debug.LogWarning("Network connection issue. Check your internet connection.");
    }
    else
    {
        Debug.LogError($"Network error: {ex.Message}");
    }
}
```

### RallyTransactionException

Thrown for errors specifically related to blockchain transactions.

```csharp
// Example of RallyTransactionException
try
{
    await rlyNetwork.TransferAsync("0xRecipient", 1000.0m);
}
catch (RallyTransactionException ex)
{
    if (ex.Message.Contains("insufficient funds"))
    {
        Debug.LogWarning("Insufficient funds for transfer.");
    }
    else
    {
        Debug.LogError($"Transaction error: {ex.Message}");
    }
}
```

### RallyConfigurationException

Thrown for errors related to SDK configuration issues.

```csharp
// Example of RallyConfigurationException
try
{
    // Operations that might have configuration issues
    await RallyNetworkFactory.CreateNetworkAsync(config);
}
catch (RallyConfigurationException ex)
{
    Debug.LogError($"Configuration error: {ex.Message}");
}
```

## Common Error Scenarios and Handling

### Network Connectivity Issues

```csharp
try
{
    // Network operation
    decimal balance = await rlyNetwork.GetDisplayBalanceAsync();
}
catch (RallyNetworkException ex)
{
    // Check for specific network issues
    if (ex.Message.Contains("connection") || ex.Message.Contains("timeout"))
    {
        // Handle connectivity issues
        Debug.LogWarning("Network connection issue. Check your internet connection.");
        ShowNetworkErrorUI("Please check your internet connection and try again.");
    }
    else if (ex.Message.Contains("rate limit"))
    {
        // Handle rate limiting
        Debug.LogWarning("API rate limit exceeded. Implementing backoff...");
        await Task.Delay(5000); // Wait 5 seconds before retrying
        RetryOperation();
    }
    else
    {
        // Handle other network errors
        Debug.LogError($"Network error: {ex.Message}");
        ShowErrorUI("An unexpected network error occurred.");
    }
}
```

### Account Access Issues

```csharp
try
{
    // Account operation
    Account account = await rlyNetwork.AccountManager.GetAccountAsync();
    
    if (account == null)
    {
        // No account exists
        ShowCreateAccountUI();
    }
}
catch (RallyAccountException ex)
{
    if (ex.Message.Contains("corrupt") || ex.Message.Contains("invalid key"))
    {
        // Handle corrupted account data
        Debug.LogError("Account data is corrupted.");
        ShowRecoveryUI("Your account data appears to be corrupted. Would you like to recover using your backup phrase?");
    }
    else
    {
        // Handle other account errors
        Debug.LogError($"Account error: {ex.Message}");
        ShowErrorUI("There was a problem accessing your account.");
    }
}
```

### Transaction Failures

```csharp
try
{
    // Transaction operation
    string txHash = await rlyNetwork.TransferAsync("0xRecipient", 10.0m);
    ShowSuccessUI($"Transfer successful! Transaction: {txHash}");
}
catch (RallyTransactionException ex)
{
    if (ex.Message.Contains("insufficient funds"))
    {
        // Handle insufficient funds
        Debug.LogWarning("Insufficient funds for transfer.");
        ShowErrorUI("You don't have enough tokens for this transfer.");
    }
    else if (ex.Message.Contains("gas"))
    {
        // Handle gas-related issues
        Debug.LogWarning("Gas estimation or price issue.");
        ShowErrorUI("There was an issue with transaction gas. Try again later.");
    }
    else
    {
        // Handle other transaction errors
        Debug.LogError($"Transaction error: {ex.Message}");
        ShowErrorUI("Your transaction could not be completed.");
    }
}
```

## Custom Exception Extension

You can extend the Rally exception hierarchy for your own application's needs:

```csharp
// Custom exception for your application
public class MyGameRallyException : RallyException
{
    public MyGameRallyException(string message) : base(message) { }
    
    public MyGameRallyException(string message, Exception innerException) 
        : base(message, innerException) { }
}

// Usage example
try
{
    // Game-specific Rally Protocol operation
    await ProcessInGamePurchase();
}
catch (Exception ex)
{
    // Wrap the exception in your custom type
    throw new MyGameRallyException("Failed to process in-game purchase", ex);
}
```

## Global Error Handling

For Unity applications, you might want to implement global error handling for Rally Protocol exceptions:

```csharp
// In your main manager class
private void Start()
{
    // Set up global exception handler
    Application.logMessageReceived += HandleLog;
}

private void HandleLog(string logString, string stackTrace, LogType type)
{
    if (type == LogType.Exception && logString.Contains("Rally"))
    {
        // Handle Rally exceptions globally
        HandleRallyException(logString);
    }
}

private void HandleRallyException(string exceptionMessage)
{
    // Show user-friendly UI
    if (exceptionMessage.Contains("account"))
    {
        ShowAccountErrorUI();
    }
    else if (exceptionMessage.Contains("network") || exceptionMessage.Contains("connection"))
    {
        ShowNetworkErrorUI();
    }
    else if (exceptionMessage.Contains("transaction") || exceptionMessage.Contains("transfer"))
    {
        ShowTransactionErrorUI();
    }
    else
    {
        ShowGenericErrorUI();
    }
}
```

## Related Documentation

- [RallyAccountException](./RallyAccountException.md): Account-specific exceptions
- [RallyNetworkException](./RallyNetworkException.md): Network-specific exceptions
- [Error Handling](./error-handling.md): Comprehensive guide to error handling
