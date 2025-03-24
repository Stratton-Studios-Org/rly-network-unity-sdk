# Events and Callbacks

The Rally Protocol Unity SDK provides callback mechanisms for handling asynchronous operations. This document outlines the recommended approaches for handling callbacks in your application.

## Overview

When working with blockchain operations through the Rally Protocol SDK, most operations are asynchronous and can be handled using:

1. Async/await patterns with Task-based methods
2. Component-specific events and Unity events

## Task-based Asynchronous Programming

The primary way to handle asynchronous operations in the SDK is through Task-based asynchronous programming:

```csharp
using RallyProtocol.Networks;
using UnityEngine;
using System.Threading.Tasks;

public class ExampleController : MonoBehaviour
{
    private IRallyNetwork rlyNetwork;

    // Example of async method
    public async Task CheckBalanceAsync()
    {
        try
        {
            // Awaiting an async operation
            decimal balance = await rlyNetwork.GetBalanceAsync();
            Debug.Log($"Current balance: {balance}");
            
            // Process the result
            UpdateBalanceUI(balance);
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Error checking balance: {ex.Message}");
            // Handle error appropriately
        }
    }
    
    private void UpdateBalanceUI(decimal balance)
    {
        // Update UI elements
    }
}
```

## Unity Coroutine Wrappers

For compatibility with Unity's coroutine system, you can wrap Task-based operations:

```csharp
using System.Collections;
using RallyProtocol.Networks;
using UnityEngine;

public class CoroutineExample : MonoBehaviour
{
    private IRallyNetwork rlyNetwork;
    
    // Start a coroutine that uses the async methods
    public void CheckBalance()
    {
        StartCoroutine(CheckBalanceCoroutine());
    }
    
    private IEnumerator CheckBalanceCoroutine()
    {
        // Start the async operation
        var task = rlyNetwork.GetBalanceAsync();
        
        // Wait until the task completes
        while (!task.IsCompleted)
        {
            yield return null;
        }
        
        if (task.IsFaulted)
        {
            Debug.LogError($"Error checking balance: {task.Exception.Message}");
            // Handle error
            yield break;
        }
        
        // Process the result
        decimal balance = task.Result;
        Debug.Log($"Current balance: {balance}");
        UpdateBalanceUI(balance);
    }
    
    private void UpdateBalanceUI(decimal balance)
    {
        // Update UI elements
    }
}
```

## Handling Callbacks for Common Operations

### Transaction Callbacks

For transactions, you can track their status using the TransactionReceipt:

```csharp
public async Task SendTransactionExample()
{
    try
    {
        // Perform the transaction
        string txHash = await rlyNetwork.TransferAsync(destinationAddress, amount);
        Debug.Log($"Transaction initiated with hash: {txHash}");
        
        // Optionally wait for confirmation
        var receipt = await rlyNetwork.WaitForTransactionReceiptAsync(txHash);
        if (receipt.Status.Value == 1)
        {
            Debug.Log("Transaction confirmed successfully");
        }
        else
        {
            Debug.LogError("Transaction failed");
        }
    }
    catch (System.Exception ex)
    {
        Debug.LogError($"Transaction error: {ex.Message}");
    }
}
```

## Using Component Events

SDK components provide specific events that you can subscribe to. Here are examples of using these events:

### RallyTransfer Events

The `RallyTransfer` component provides two main events:

```csharp
public class TransferExample : MonoBehaviour
{
    [SerializeField]
    private RallyTransfer transferComponent;
    
    private void OnEnable()
    {
        // Subscribe to the Transferring event (triggered when a transfer begins)
        transferComponent.Transferring += OnTransferring;
        
        // Subscribe to the Transferred event (triggered when a transfer completes)
        transferComponent.Transferred += OnTransferred;
    }
    
    private void OnDisable()
    {
        // Always unsubscribe when the component is disabled
        transferComponent.Transferring -= OnTransferring;
        transferComponent.Transferred -= OnTransferred;
    }
    
    private void OnTransferring(object sender, EventArgs e)
    {
        Debug.Log("Token transfer has started");
        // Show loading UI
    }
    
    private void OnTransferred(object sender, RallyTransferEventArgs e)
    {
        Debug.Log($"Transfer completed with transaction hash: {e.TransactionHash}");
        // Update UI, play success animation, etc.
    }
}
```

### RallyClaimRly Events

The `RallyClaimRly` component provides similar events:

```csharp
public class ClaimExample : MonoBehaviour
{
    [SerializeField]
    private RallyClaimRly claimComponent;
    
    private void OnEnable()
    {
        // Subscribe to the Claiming event (triggered when claiming begins)
        claimComponent.Claiming += OnClaiming;
        
        // Subscribe to the Claimed event (triggered when claiming completes)
        claimComponent.Claimed += OnClaimed;
    }
    
    private void OnDisable()
    {
        // Always unsubscribe when the component is disabled
        claimComponent.Claiming -= OnClaiming;
        claimComponent.Claimed -= OnClaimed;
    }
    
    private void OnClaiming(object sender, EventArgs e)
    {
        Debug.Log("RLY token claim has started");
        // Show loading UI
    }
    
    private void OnClaimed(object sender, RallyClaimRlyEventArgs e)
    {
        Debug.Log($"Claim completed with transaction hash: {e.TransactionHash}");
        // Update UI, play success animation, etc.
    }
}
```

## Unity Events in Inspector

In addition to C# events, SDK components also provide Unity Events that can be connected in the Inspector:

- `RallyTransfer` provides `transferringEvent` and `transferredEvent`
- `RallyClaimRly` provides `claimingEvent` and `claimedEvent`

These can be used to connect UI elements or trigger other components without writing code.

## Thread Safety Considerations

Operations that interact with the blockchain happen on background threads. When updating Unity UI or other Unity-specific components, ensure you're on the main thread:

```csharp
// Example using a common pattern for main thread dispatching
public async Task UpdateUIAfterOperation()
{
    try {
        decimal balance = await rlyNetwork.GetBalanceAsync();
        
        // Make sure UI updates happen on the main thread
        UnityMainThreadDispatcher.Instance().Enqueue(() => {
            balanceText.text = $"{balance} RLY";
        });
    }
    catch (System.Exception ex) {
        UnityMainThreadDispatcher.Instance().Enqueue(() => {
            errorText.text = ex.Message;
        });
    }
}
```

## Best Practices

1. **Error handling**: Always use try/catch blocks around async operations to handle exceptions properly

2. **Cancellation support**: For long-running operations, consider using CancellationToken for proper cancellation

3. **Progress indication**: For operations that might take time, provide visual feedback to users

4. **Avoid blocking the main thread**: Do not use .Result or .Wait() on Tasks from the main thread

5. **Clean up resources**: Properly dispose of any resources when your operations complete

6. **Unsubscribe from events**: Always unsubscribe from events when the subscribing object is being destroyed
