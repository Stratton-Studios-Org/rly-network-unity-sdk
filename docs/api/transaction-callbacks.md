# Transaction Callbacks

The Rally Protocol Unity SDK provides mechanisms for tracking transaction status through component-based callbacks and events.

## Overview

Transaction callbacks are essential for providing feedback to users and implementing game logic that depends on successful transactions. The SDK offers different ways to handle transaction callbacks:

1. Component events
2. UnityEvent callbacks in the Inspector
3. Async/await with UniTask

## Component-Based Event Callbacks

The `RallyTransfer` component provides events for tracking transaction status:

```csharp
// Get reference to a RallyTransfer component
RallyTransfer transferComponent = GetComponent<RallyTransfer>();

// Subscribe to transaction events
transferComponent.Transferring += HandleTransferring;
transferComponent.Transferred += HandleTransferred;

// Event handler methods
private void HandleTransferring(object sender, EventArgs e)
{
    Debug.Log("Transaction is being submitted");
    // Update UI to show pending transaction
}

private void HandleTransferred(object sender, RallyTransferEventArgs e)
{
    Debug.Log($"Transaction confirmed with hash: {e.TransactionHash}");
    // Update UI to show successful transaction
    // Update game state or rewards based on transaction
}
```

Don't forget to unsubscribe from events when they are no longer needed:

```csharp
private void OnDisable()
{
    var transferComponent = GetComponent<RallyTransfer>();
    if (transferComponent != null)
    {
        transferComponent.Transferring -= HandleTransferring;
        transferComponent.Transferred -= HandleTransferred;
    }
}
```

## Inspector-Based Callbacks with UnityEvents

The `RallyTransfer` component exposes UnityEvents that can be configured directly in the Unity Inspector:

```csharp
// These fields are already defined in the RallyTransfer component
[SerializeField] private UnityEvent onTransferring;
[SerializeField] private UnityEvent<string> onTransferred;
```

To use them in the Inspector:
1. Select the GameObject with the RallyTransfer component
2. In the Inspector, locate the Events section (onTransferring and onTransferred)
3. Click the "+" button to add a new event listener
4. Drag the target GameObject and select the method to call

## Async/Await with UniTask

For more control over transaction flow, you can use the async methods with UniTask:

```csharp
// Get reference to a RallyTransfer component
RallyTransfer transferComponent = GetComponent<RallyTransfer>();

// Use async/await pattern
private async void HandleTransferButtonClick()
{
    try
    {
        // Show loading indicator
        loadingPanel.SetActive(true);
        
        // Wait for transaction to complete
        await transferComponent.TransferAsync("0xRecipientAddress", 10.0m);
        
        // Handle success
        Debug.Log("Transfer completed successfully!");
        UpdateUIAfterSuccess();
    }
    catch (Exception ex)
    {
        // Handle error
        Debug.LogError($"Transfer failed: {ex.Message}");
        ShowErrorUI(ex.Message);
    }
    finally
    {
        // Hide loading indicator
        loadingPanel.SetActive(false);
    }
}
```

## Handling Pending Transactions

The `RallyTransfer` component automatically handles pending transactions to prevent concurrent transfers:

```csharp
// Check if a transfer is already in progress
bool isTransferPending = transferComponent.PendingTransferTask.HasValue;

if (isTransferPending)
{
    Debug.Log("A transfer is already in progress. Please wait.");
    return;
}

// If no transfer is pending, proceed with new transfer
transferComponent.Transfer("0xRecipientAddress", 10.0m);
```

## Best Practices

### State Management

Maintain a transaction state manager to track pending transactions:

```csharp
public class TransactionStateManager : MonoBehaviour
{
    private Dictionary<string, TransactionState> pendingTransactions = new Dictionary<string, TransactionState>();
    
    [SerializeField] private RallyTransfer transferComponent;
    
    private void Start()
    {
        // Subscribe to transfer events
        transferComponent.Transferring += HandleTransferring;
        transferComponent.Transferred += HandleTransferred;
    }
    
    private void HandleTransferring(object sender, EventArgs e)
    {
        // Update UI to show transfer in progress
        UpdateTransactionUI("Sending transaction...");
    }
    
    private void HandleTransferred(object sender, RallyTransferEventArgs e)
    {
        // Add transaction to tracking
        pendingTransactions[e.TransactionHash] = new TransactionState
        {
            Hash = e.TransactionHash,
            Status = TransactionStatus.Confirmed,
            CompletionTime = DateTime.Now
        };
        
        // Update UI
        UpdateTransactionUI($"Transaction confirmed: {e.TransactionHash}");
        
        // Remove transaction after some time
        StartCoroutine(RemoveTransactionAfterDelay(e.TransactionHash, 60));
    }
    
    private IEnumerator RemoveTransactionAfterDelay(string hash, float delay)
    {
        yield return new WaitForSeconds(delay);
        pendingTransactions.Remove(hash);
        UpdateTransactionUI();
    }
    
    private void UpdateTransactionUI(string message = null)
    {
        // Update UI based on transaction state
        // ...
    }
    
    private void OnDisable()
    {
        // Unsubscribe from events
        if (transferComponent != null)
        {
            transferComponent.Transferring -= HandleTransferring;
            transferComponent.Transferred -= HandleTransferred;
        }
    }
}

public enum TransactionStatus
{
    Pending,
    Confirmed,
    Failed
}

public class TransactionState
{
    public string Hash { get; set; }
    public TransactionStatus Status { get; set; }
    public DateTime SubmissionTime { get; set; }
    public DateTime? CompletionTime { get; set; }
}
```

### Error Handling

Implement proper error handling for transaction operations:

```csharp
private async void SendTransactionWithErrorHandling()
{
    try
    {
        RallyTransfer transferComponent = GetComponent<RallyTransfer>();
        
        await transferComponent.TransferAsync("0xRecipientAddress", 0.01m);
        ShowSuccessUI("Transaction completed successfully!");
    }
    catch (Exception ex)
    {
        // Categorize error types
        if (ex.Message.Contains("insufficient funds"))
        {
            ShowInsufficientFundsUI();
        }
        else if (ex.Message.Contains("user rejected"))
        {
            ShowUserRejectedUI();
        }
        else
        {
            ShowGenericErrorUI(ex.Message);
        }
    }
}

private void ShowInsufficientFundsUI() { /* Show insufficient funds UI */ }
private void ShowUserRejectedUI() { /* Show user rejected UI */ }
private void ShowGenericErrorUI(string error) { /* Show generic error UI */ }
private void ShowSuccessUI(string message) { /* Show success UI */ }
```

## Related Documentation

- [Event System](./event-system.md) - For more information on the event system
- [Error Handling](./error-handling.md) - For detailed error handling strategies
- [GSN Operations](./gsn-operations.md) - For gasless transactions using GSN

