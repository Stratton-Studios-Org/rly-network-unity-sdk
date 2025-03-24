# GSN Operations

This document explains how to use the Gas Station Network (GSN) with the Rally Protocol Unity SDK to enable gasless transactions for your users.

## What is GSN?

The Gas Station Network (GSN) is a decentralized system that allows dApp developers to pay for their users' gas costs, eliminating the need for users to hold ETH to interact with blockchain applications. This significantly improves the user experience by removing the barrier to entry for new users.

## GSN Benefits

- **Improved User Experience**: Users can interact with blockchain without holding ETH
- **Lower Friction**: No need for users to purchase crypto to use your application
- **Higher Conversion Rates**: Removes a significant barrier to entry for new users
- **Simplified Onboarding**: Users can start using your app immediately

## Using GSN with Rally Protocol

The Rally Protocol Unity SDK provides built-in support for GSN through the `IGsnClient` interface and related components. Here's how to use it:

### Accessing the GSN Client

```csharp
// Get the GSN client from the Rally Network instance
IGsnClient gsnClient = rlyNetwork.GsnClient;
```

### Creating a GSN Transaction

To create a gasless transaction using GSN:

```csharp
// Create GSN transaction details
GsnTransactionDetails txDetails = new GsnTransactionDetails
{
    From = await rlyNetwork.AccountManager.GetPublicAddressAsync(),
    To = "0xTargetContractAddress",
    Data = "0x...", // Contract method call data
    Gas = 100000,   // Gas limit
    // Additional optional parameters
    Value = 0,      // ETH value to send with transaction (typically 0 for token transfers)
    MaxFeePerGas = null,  // Maximum fee per gas unit
    MaxPriorityFeePerGas = null // Maximum priority fee per gas unit
};

// Relay the transaction through GSN
string txHash = await rlyNetwork.RelayAsync(txDetails);
```

### Using MetaTxMethod with Transfers

For simple token transfers, you can use the `MetaTxMethod.Gsn` parameter with transfer functions:

```csharp
// Transfer tokens using GSN for gas payment
string txHash = await rlyNetwork.TransferAsync(
    "0x123...789",      // Destination address
    10.5m,              // Amount (10.5 tokens)
    MetaTxMethod.Gsn    // Use GSN for gasless transaction
);
```

### Advanced GSN Configuration

The GsnClient can be configured with various parameters:

```csharp
// Get GSN client for advanced configuration
IGsnClient gsnClient = rlyNetwork.GsnClient;

// Configure GSN client with custom parameters
gsnClient.SetConfig(new GsnConfig 
{
    PaymasterAddress = "0xCustomPaymasterAddress",
    RelayHubAddress = "0xCustomRelayHubAddress",
    ChainId = 137, // Example: Polygon chain ID
    MaxAcceptanceBudget = 100000,
    DomainSeparatorName = "My App"
});
```

## GsnTransactionDetails Properties

The `GsnTransactionDetails` class contains the following properties:

```csharp
public class GsnTransactionDetails
{
    // Required properties
    public string From { get; set; }      // Sender address
    public string To { get; set; }        // Target contract address
    public string Data { get; set; }      // Encoded function call data
    public uint Gas { get; set; }         // Gas limit for the transaction
    
    // Optional properties
    public BigInteger? Value { get; set; } // ETH value to send (usually 0)
    public BigInteger? MaxFeePerGas { get; set; } // Maximum fee per gas unit (EIP-1559)
    public BigInteger? MaxPriorityFeePerGas { get; set; } // Maximum priority fee (EIP-1559)
    public string? PaymasterData { get; set; } // Custom paymaster data if needed
}
```

## Error Handling for GSN Operations

GSN transactions may fail for various reasons. It's important to handle these errors appropriately:

```csharp
try
{
    string txHash = await rlyNetwork.RelayAsync(gsnTxDetails);
    Debug.Log($"GSN transaction successful: {txHash}");
}
catch (GsnRelayException ex)
{
    // Handle GSN-specific errors
    Debug.LogError($"GSN relay error: {ex.Message}");
    
    // You might want to fall back to regular transactions
    if (ex.Message.Contains("insufficient funds"))
    {
        Debug.Log("Falling back to regular transaction...");
        // Implement fallback logic
    }
}
catch (Exception ex)
{
    Debug.LogError($"Transaction failed: {ex.Message}");
}
```

## GSN Limitations and Considerations

When using GSN, keep the following in mind:

1. **Gas Costs**: While users don't pay gas, you (the developer) still pay for it
2. **Supported Networks**: Check which networks are supported by GSN
3. **Transaction Complexity**: Very complex transactions may exceed acceptance budget
4. **Paymaster Balance**: Ensure your paymaster has sufficient funds
5. **Relay Availability**: In rare cases, relayers might be temporarily unavailable

## Best Practices for GSN

1. **Set reasonable gas limits** to control costs
2. **Implement fallback mechanisms** in case GSN relay fails
3. **Monitor your paymaster balance** to ensure it doesn't run out
4. **Test thoroughly** on testnets before deploying to production
5. **Consider implementing usage limits** per user to prevent abuse

## Example: Complete GSN Transfer Flow

```csharp
// Get user wallet address
string userAddress = await rlyNetwork.AccountManager.GetPublicAddressAsync();

// Check if user has sufficient token balance
decimal tokenBalance = await rlyNetwork.GetDisplayBalanceAsync();
decimal transferAmount = 5.0m;

if (tokenBalance >= transferAmount)
{
    try
    {
        // Perform token transfer using GSN
        string txHash = await rlyNetwork.TransferAsync(
            "0xRecipientAddress",
            transferAmount,
            MetaTxMethod.Gsn
        );
        
        Debug.Log($"Transfer initiated with GSN: {txHash}");
        
        // Wait for transaction confirmation
        Web3 web3 = await rlyNetwork.GetProviderAsync();
        bool confirmed = false;
        int attempts = 0;
        
        while (!confirmed && attempts < 10)
        {
            var receipt = await web3.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(txHash);
            if (receipt != null)
            {
                confirmed = true;
                Debug.Log("Transfer confirmed!");
            }
            else
            {
                attempts++;
                await Task.Delay(2000); // Wait 2 seconds between checks
            }
        }
    }
    catch (Exception ex)
    {
        Debug.LogError($"GSN transfer failed: {ex.Message}");
    }
}
else
{
    Debug.LogWarning("Insufficient token balance for transfer");
}
```
