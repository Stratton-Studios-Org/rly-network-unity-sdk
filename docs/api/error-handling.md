# Error Handling

The Rally Protocol Unity SDK provides structured error handling to help you manage exceptions and error states in your blockchain applications.

## Common Error Types

The SDK throws specific exception types for different error scenarios:

- `RlyNetworkException` - Base exception type for network-related errors
- `TransactionException` - Issues with transaction execution
- `AccountException` - Problems with account access or creation
- `ConnectionException` - Network connectivity issues
- `ConfigurationException` - Invalid SDK configuration

## Basic Error Handling

For basic error handling, use try-catch blocks around SDK operations:

```csharp
try
{
    // Attempt a token transfer
    string txHash = await RallyUnityManager.Instance.RlyNetwork.TransferAsync(
        "0xRecipientAddress", 
        1.5m
    );
    Debug.Log($"Transfer successful with hash: {txHash}");
}
catch (TransactionException ex)
{
    // Handle transaction-specific errors
    Debug.LogError($"Transaction failed: {ex.Message}");
}
catch (Exception ex)
{
    // Handle any other unexpected errors
    Debug.LogError($"Error: {ex.Message}");
}
```

## Component-Based Error Handling

When using Unity components like `RallyTransfer`, you can subscribe to error events:

```csharp
using UnityEngine;
using RallyProtocol.Unity;

public class TransferErrorHandler : MonoBehaviour
{
    public RallyTransfer rallyTransfer;

    void Start()
    {
        // Subscribe to the error event
        rallyTransfer.OnError += HandleTransferError;
    }

    void OnDestroy()
    {
        // Always unsubscribe when the component is destroyed
        rallyTransfer.OnError -= HandleTransferError;
    }

    private void HandleTransferError(string errorMessage)
    {
        // Log the error
        Debug.LogError($"Transfer error: {errorMessage}");
        
        // Update UI to show error
        UpdateUIWithError(errorMessage);
    }
    
    private void UpdateUIWithError(string errorMessage)
    {
        // Update your UI elements to display the error
        // This implementation will vary based on your UI system
    }
}
```

## Async/Await Error Handling

For async operations, you can use try-catch with await pattern:

```csharp
using UnityEngine;
using RallyProtocol.Unity;
using Cysharp.Threading.Tasks;

public class AsyncErrorHandler : MonoBehaviour
{
    public RallyTransfer rallyTransfer;
    
    public async UniTaskVoid TransferWithErrorHandling(string recipient, decimal amount)
    {
        try
        {
            // Show loading indicator
            ShowLoadingUI(true);
            
            // Attempt the transfer
            string txHash = await rallyTransfer.TransferTokensAsync(recipient, amount);
            
            // Handle success
            Debug.Log($"Transfer successful: {txHash}");
            ShowSuccessUI($"Transferred {amount} tokens!");
        }
        catch (TransactionException ex) when (ex.Message.Contains("insufficient funds"))
        {
            // Specific handling for insufficient funds
            Debug.LogWarning("Insufficient funds for transfer");
            ShowErrorUI("You don't have enough tokens for this transfer.");
        }
        catch (TransactionException ex)
        {
            // Generic transaction error handling
            Debug.LogError($"Transaction error: {ex.Message}");
            ShowErrorUI("Unable to complete transaction. Please try again.");
        }
        catch (ConnectionException ex)
        {
            // Network connection error
            Debug.LogError($"Connection error: {ex.Message}");
            ShowErrorUI("Network connection issue. Check your internet connection.");
        }
        catch (Exception ex)
        {
            // Fallback for unexpected errors
            Debug.LogError($"Unexpected error: {ex.Message}");
            ShowErrorUI("An unexpected error occurred.");
        }
        finally
        {
            // Always hide loading indicator when done
            ShowLoadingUI(false);
        }
    }
    
    private void ShowLoadingUI(bool isVisible) { /* Implementation */ }
    private void ShowSuccessUI(string message) { /* Implementation */ }
    private void ShowErrorUI(string message) { /* Implementation */ }
}
```

## Error Categorization

It's useful to categorize errors to provide appropriate feedback to users:

```csharp
public enum ErrorCategory
{
    UserError,          // User made a mistake (invalid input, etc.)
    InsufficientFunds,  // Not enough tokens for the operation
    NetworkError,       // Network connectivity or blockchain issues
    RejectedOperation,  // User rejected or cancelled the operation
    SystemError         // Internal system or SDK error
}

public class ErrorHandler
{
    public ErrorCategory CategorizeError(Exception ex)
    {
        // Check for insufficient funds
        if (ex is TransactionException && ex.Message.Contains("insufficient funds"))
        {
            return ErrorCategory.InsufficientFunds;
        }
        
        // Check for network errors
        if (ex is ConnectionException)
        {
            return ErrorCategory.NetworkError;
        }
        
        // Check for user-rejected transactions
        if (ex is TransactionException && ex.Message.Contains("user rejected"))
        {
            return ErrorCategory.RejectedOperation;
        }
        
        // Check for invalid input
        if (ex is ArgumentException || ex is FormatException)
        {
            return ErrorCategory.UserError;
        }
        
        // Default to system error
        return ErrorCategory.SystemError;
    }
    
    public string GetUserFriendlyMessage(Exception ex)
    {
        ErrorCategory category = CategorizeError(ex);
        
        switch (category)
        {
            case ErrorCategory.UserError:
                return "Invalid input. Please check your information and try again.";
                
            case ErrorCategory.InsufficientFunds:
                return "You don't have enough tokens to complete this action.";
                
            case ErrorCategory.NetworkError:
                return "Network issue detected. Please check your internet connection and try again.";
                
            case ErrorCategory.RejectedOperation:
                return "The operation was cancelled.";
                
            case ErrorCategory.SystemError:
            default:
                return "An unexpected error occurred. Please try again later.";
        }
    }
}
```

## Retry Logic

For transient errors, implement retry logic:

```csharp
public class TransactionWithRetry
{
    private IRallyNetwork _network;
    private int _maxRetries = 3;
    private float _retryDelaySeconds = 2f;
    
    public TransactionWithRetry(IRallyNetwork network)
    {
        _network = network;
    }
    
    public async UniTask<string> TransferWithRetryAsync(
        string to, 
        decimal amount, 
        int maxRetries = 3)
    {
        _maxRetries = maxRetries;
        int attempts = 0;
        Exception lastException = null;
        
        while (attempts < _maxRetries)
        {
            try
            {
                return await _network.TransferAsync(to, amount);
            }
            catch (ConnectionException ex)
            {
                // Only retry network/connection issues
                attempts++;
                lastException = ex;
                
                if (attempts < _maxRetries)
                {
                    Debug.LogWarning($"Transfer attempt {attempts} failed, retrying in {_retryDelaySeconds} seconds. Error: {ex.Message}");
                    await UniTask.Delay(TimeSpan.FromSeconds(_retryDelaySeconds));
                    // Increase delay for each retry (exponential backoff)
                    _retryDelaySeconds *= 1.5f;
                }
            }
            catch (Exception ex)
            {
                // Don't retry other types of exceptions
                throw;
            }
        }
        
        // If we get here, all retries failed
        throw new Exception($"Transfer failed after {_maxRetries} attempts. Last error: {lastException.Message}", lastException);
    }
}
```

## Application State Management

Manage application state during error conditions:

```csharp
public class TransactionStateManager : MonoBehaviour
{
    // Possible states for a transaction
    public enum TransactionState
    {
        Idle,
        Preparing,
        Submitting,
        Pending,
        Completed,
        Failed
    }
    
    // Current state of the transaction
    private TransactionState _currentState = TransactionState.Idle;
    
    // Event for state changes
    public event Action<TransactionState> OnStateChanged;
    
    // Update state and notify listeners
    public void UpdateState(TransactionState newState)
    {
        if (_currentState != newState)
        {
            _currentState = newState;
            OnStateChanged?.Invoke(_currentState);
        }
    }
    
    // Example transaction with state management
    public async UniTask<string> ExecuteTransactionWithStateAsync(
        RallyTransfer transferComponent, 
        string to, 
        decimal amount)
    {
        try
        {
            // Update state to preparing
            UpdateState(TransactionState.Preparing);
            
            // Validate inputs
            if (string.IsNullOrEmpty(to) || amount <= 0)
            {
                UpdateState(TransactionState.Failed);
                throw new ArgumentException("Invalid recipient or amount");
            }
            
            // Update state to submitting
            UpdateState(TransactionState.Submitting);
            
            // Execute the transaction
            string txHash = await transferComponent.TransferTokensAsync(to, amount);
            
            // Update state to pending
            UpdateState(TransactionState.Pending);
            
            // Monitor transaction (optional)
            bool success = await MonitorTransactionAsync(txHash);
            
            // Update final state
            UpdateState(success ? TransactionState.Completed : TransactionState.Failed);
            
            return txHash;
        }
        catch (Exception ex)
        {
            // Update state to failed
            UpdateState(TransactionState.Failed);
            
            // Re-throw the exception for further handling
            throw;
        }
    }
    
    private async UniTask<bool> MonitorTransactionAsync(string txHash)
    {
        // Implement transaction monitoring logic here
        // This is a simplified example
        await UniTask.Delay(2000); // Simulate waiting
        return true; // Simulate success
    }
}
```

## Related Documentation

- [Transaction Callbacks](./transaction-callbacks.md) - For handling transaction events
- [Security Best Practices](./security-best-practices.md) - For secure error handling practices
