# Gas Station Network (GSN) Integration

This guide explains how the Rally Protocol Unity SDK integrates with the Gas Station Network (GSN) to provide gasless transactions for your users.

## Overview

The Gas Station Network is a decentralized protocol that allows dApp developers to pay for their users' transaction fees, removing the need for users to have ETH in their wallets to perform operations on the blockchain. The Rally Protocol Unity SDK fully integrates with GSN, allowing your users to interact with the blockchain without managing gas fees.

## How GSN Works

When a user initiates a transaction through the Rally Protocol Unity SDK:

1. Instead of sending the transaction directly to the blockchain, the SDK sends it to a GSN Relayer
2. The Relayer verifies the transaction and forwards it to the blockchain
3. The Relayer pays for the gas fees
4. The Rally Protocol reimburses the Relayer

The entire process is transparent to the user, who simply experiences a normal transaction without needing to pay for gas.

## Benefits of GSN Integration

- **Improved User Experience**: Users don't need to acquire ETH before using your application
- **Lower Barrier to Entry**: New users can start using blockchain features immediately
- **Simplified Onboarding**: Remove the complexity of explaining gas fees to new users
- **Predictable Costs**: Developers can predict and manage transaction costs

## Implementation Details

The Rally Protocol Unity SDK handles all GSN integration automatically. When you use the core SDK methods like `TransferAsync` or `ClaimRlyAsync`, gasless transactions are used by default.

### Example: Gasless Token Transfer

```csharp
using RallyProtocol.Unity;
using System;
using System.Threading.Tasks;
using UnityEngine;

public class GaslessTransferExample : MonoBehaviour
{
    private IRallyNetwork _rallyNetwork;

    private async void Start()
    {
        // Get a reference to the Rally Network
        _rallyNetwork = RallyUnityManager.Instance.RlyNetwork;
        
        // Perform a gasless transfer
        await PerformGaslessTransferAsync();
    }

    private async Task PerformGaslessTransferAsync()
    {
        try
        {
            string recipientAddress = "0xRecipientAddressHere";
            decimal amount = 10.0m;
            
            // This transfer will be performed without the user needing ETH for gas
            string txHash = await _rallyNetwork.TransferAsync(recipientAddress, amount);
            
            Debug.Log($"Gasless transfer completed successfully! Transaction hash: {txHash}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Gasless transfer failed: {ex.Message}");
        }
    }
}
```

## Advanced Configuration

### GSN Transaction Options

While the SDK handles GSN integration automatically, you can customize certain aspects of the GSN behavior through the Rally Protocol configuration:

```csharp
// Configure GSN options during SDK initialization
var config = new RallyNetworkConfig
{
    ApiKey = "YOUR_API_KEY",
    Network = RallyNetwork.TestNet,
    GsnOptions = new GsnOptions
    {
        MaxGasLimit = 500000,
        RetryAttempts = 3,
        RetryDelayMs = 1000
    }
};

// Initialize the SDK with custom GSN options
await RallyUnityManager.Instance.InitializeAsync(config);
```

### Available GSN Options

- `MaxGasLimit`: The maximum gas limit for GSN transactions (default: 500000)
- `RetryAttempts`: Number of retry attempts for failed GSN transactions (default: 3)
- `RetryDelayMs`: Delay between retry attempts in milliseconds (default: 1000)

## Transaction Status Monitoring

When using GSN transactions, it's important to monitor transaction status, as they might take longer to confirm than direct transactions:

```csharp
public async Task<TransactionStatus> CheckTransactionStatusAsync(string txHash)
{
    try
    {
        // Check transaction status
        var status = await _rallyNetwork.GetTransactionStatusAsync(txHash);
        
        return status;
    }
    catch (Exception ex)
    {
        Debug.LogError($"Failed to check transaction status: {ex.Message}");
        return TransactionStatus.Failed;
    }
}
```

## Troubleshooting GSN Issues

### Common GSN Issues

1. **Transaction Rejected by Relayer**
   - GSN Relayers may reject transactions if they don't meet certain criteria
   - Check the transaction parameters and retry with adjusted values

2. **Slow Confirmation Times**
   - GSN transactions may take longer to confirm, especially during network congestion
   - Implement appropriate UI feedback to keep users informed

3. **Relayer Connection Issues**
   - If the GSN Relayer network is experiencing issues, transactions may fail
   - Implement retry logic with exponential backoff

### Handling GSN Errors

```csharp
public async Task HandleGsnTransactionWithRetryAsync(Func<Task<string>> transactionFunc)
{
    int attempts = 0;
    int maxAttempts = 3;
    bool success = false;
    
    while (!success && attempts < maxAttempts)
    {
        attempts++;
        try
        {
            Debug.Log($"Attempting GSN transaction (attempt {attempts}/{maxAttempts})...");
            
            // Execute the transaction function (could be transfer, claim, etc.)
            string txHash = await transactionFunc();
            
            Debug.Log($"GSN transaction submitted successfully! Transaction hash: {txHash}");
            
            // Monitor transaction status
            await MonitorTransactionUntilCompletionAsync(txHash);
            
            success = true;
        }
        catch (RallyGsnException ex)
        {
            if (attempts >= maxAttempts)
            {
                Debug.LogError($"GSN transaction failed after {maxAttempts} attempts: {ex.Message}");
                throw;
            }
            else
            {
                Debug.LogWarning($"GSN transaction attempt {attempts} failed: {ex.Message}. Retrying...");
                await Task.Delay(1000 * attempts); // Exponential backoff
            }
        }
    }
}

private async Task MonitorTransactionUntilCompletionAsync(string txHash)
{
    bool isCompleted = false;
    int checkAttempts = 0;
    int maxCheckAttempts = 10;
    
    while (!isCompleted && checkAttempts < maxCheckAttempts)
    {
        checkAttempts++;
        
        var status = await _rallyNetwork.GetTransactionStatusAsync(txHash);
        
        switch (status)
        {
            case TransactionStatus.Confirmed:
                Debug.Log($"Transaction confirmed! Hash: {txHash}");
                isCompleted = true;
                break;
                
            case TransactionStatus.Failed:
                Debug.LogError($"Transaction failed! Hash: {txHash}");
                isCompleted = true;
                throw new Exception($"Transaction failed: {txHash}");
                
            case TransactionStatus.Pending:
                Debug.Log($"Transaction still pending... ({checkAttempts}/{maxCheckAttempts})");
                await Task.Delay(2000); // Wait 2 seconds before checking again
                break;
        }
    }
    
    if (!isCompleted)
    {
        Debug.LogWarning($"Transaction status check timed out after {maxCheckAttempts} attempts. Transaction may still be pending.");
    }
}
```

## Best Practices

### Performance Considerations

- **Batch Transactions**: Where possible, batch multiple operations into a single transaction to reduce GSN overhead
- **Throttle Transactions**: Implement rate limiting to prevent overwhelming the GSN relayers
- **Cache Transaction Results**: Cache transaction results to avoid unnecessary status checks

### User Experience

- **Provide Clear Feedback**: Inform users about transaction progress and status
- **Handle Timeouts Gracefully**: Implement UI for handling long-running transactions
- **Offer Manual Retry**: Allow users to manually retry failed transactions

## Limitations

While GSN provides many benefits, be aware of these limitations:

- **Transaction Size**: Very large or complex transactions may exceed the GSN gas limit
- **Network Congestion**: During extreme network congestion, GSN transactions might experience delays
- **Relayer Availability**: GSN relies on a network of relayers, which may have varying availability

## Next Steps

After understanding GSN integration, consider exploring these related features:

- [Network Configuration](./network-configuration.md)
- [Error Handling](./error-handling.md)
- [Transaction Security](./transaction-security.md)

## Related Documentation

- [Gasless Transaction Settings](./gasless-settings.md)
- [Transaction Sponsorship](./transaction-sponsorship.md)
- [API Reference: IRallyNetwork](../api/IRallyNetwork.md)
