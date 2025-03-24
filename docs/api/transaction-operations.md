# Transaction Operations

This document provides guidance on how to perform blockchain transactions using the Rally Protocol Unity SDK.

## Basic Transactions

The Rally Protocol Unity SDK provides several methods for performing transactions on the blockchain:

- `TransferAsync` - Transfer tokens in human-readable decimal format
- `TransferExactAsync` - Transfer tokens using exact BigInteger amounts
- `ClaimRlyAsync` - Claim free RLY tokens from the faucet
- `RelayAsync` - Execute gasless transactions using the Gas Station Network (GSN)

## Transferring Tokens

### Transfer with User-Friendly Amounts

```csharp
/// <summary>
/// Transfers tokens from the current account to another address.
/// </summary>
/// <param name="destinationAddress">The Ethereum address to send tokens to</param>
/// <param name="amount">The amount of tokens to transfer in decimal format</param>
/// <param name="metaTxMethod">Optional meta-transaction method to use (Permit or ExecuteMetaTransaction)</param>
/// <param name="tokenAddress">Optional token address (defaults to RLY if null)</param>
/// <param name="tokenConfig">Optional token configuration</param>
/// <returns>Transaction hash of the transfer</returns>
Task<string> TransferAsync(string destinationAddress, decimal amount, MetaTxMethod? metaTxMethod = null, string? tokenAddress = null, TokenConfig tokenConfig = null);
```

**Example:**

```csharp
// Simple RLY token transfer with auto-detection of meta-transaction method
string txHash = await rlyNetwork.TransferAsync(
    "0x123...789", // Destination address
    10.5m         // Amount (10.5 RLY tokens)
);

// Transfer with explicit meta-transaction method
string txHash = await rlyNetwork.TransferAsync(
    "0x123...789",                         // Destination address
    10.5m,                                 // Amount (10.5 RLY tokens)
    MetaTxMethod.ExecuteMetaTransaction    // Use ExecuteMetaTransaction method
);

// Transfer with Permit method
string txHash = await rlyNetwork.TransferAsync(
    "0x123...789",         // Destination address
    10.5m,                 // Amount (10.5 RLY tokens)
    MetaTxMethod.Permit    // Use Permit method
);

// Transfer custom token
string txHash = await rlyNetwork.TransferAsync(
    "0x123...789",                           // Destination address
    100.0m,                                  // Amount (100 tokens)
    null,                                    // Auto-detect meta-transaction method
    "0xabc...def",                           // Custom token address
    new TokenConfig { Eip712Domain = "..." } // Token configuration with domain
);
```

### Transfer with Exact BigInteger Amounts

```csharp
/// <summary>
/// Transfers the exact token amount from the current account to another address.
/// </summary>
/// <param name="destinationAddress">The Ethereum address to send tokens to</param>
/// <param name="amount">The exact amount of tokens to transfer as BigInteger</param>
/// <param name="metaTxMethod">Optional meta-transaction method to use (Permit or ExecuteMetaTransaction)</param>
/// <param name="tokenAddress">Optional token address (defaults to RLY if null)</param>
/// <param name="tokenConfig">Optional token configuration</param>
/// <returns>Transaction hash of the transfer</returns>
Task<string> TransferExactAsync(string destinationAddress, BigInteger amount, MetaTxMethod? metaTxMethod = null, string? tokenAddress = null, TokenConfig tokenConfig = null);
```

**Example:**

```csharp
// Using BigInteger for precise amount control
BigInteger exactAmount = BigInteger.Parse("10500000000000000000"); // 10.5 tokens with 18 decimals
string txHash = await rlyNetwork.TransferExactAsync(
    "0x123...789", // Destination address
    exactAmount    // Exact amount as BigInteger
);

// With explicit meta-transaction method
string txHash = await rlyNetwork.TransferExactAsync(
    "0x123...789",                         // Destination address
    exactAmount,                           // Exact amount as BigInteger
    MetaTxMethod.ExecuteMetaTransaction    // Use ExecuteMetaTransaction method
);
```

### How Meta-Transaction Methods Work

The SDK supports two main types of meta-transaction methods:

1. **ExecuteMetaTransaction** - Uses the token contract's built-in executeMetaTransaction function
2. **Permit** - Uses the EIP-2612 permit standard

When you pass `null` as the `metaTxMethod` parameter, the SDK will automatically detect which method is supported by the token contract and use it:

```csharp
// Auto-detection of meta-transaction method
string txHash = await rlyNetwork.TransferAsync(
    "0x123...789", // Destination address
    10.5m,         // Amount
    null           // Auto-detect meta-transaction method
);
```

Internally, the SDK checks if the token contract supports `executeMetaTransaction` first. If it does, it uses that method. Otherwise, it checks if the token contract supports the `permit` function. If neither is supported, it throws a `TransferMethodNotSupportedException`.

## Claiming RLY Tokens

The SDK provides a simple way to claim free RLY tokens from the faucet:

```csharp
/// <summary>
/// Claims free RLY tokens from the faucet.
/// </summary>
/// <returns>Transaction hash of the claim operation</returns>
Task<string> ClaimRlyAsync();
```

**Example:**

```csharp
try {
    string txHash = await rlyNetwork.ClaimRlyAsync();
    Debug.Log($"Successfully claimed RLY tokens. Transaction: {txHash}");
    
    // Check the new balance
    decimal balance = await rlyNetwork.GetDisplayBalanceAsync();
    Debug.Log($"New RLY balance: {balance}");
} catch (PriorDustingException) {
    Debug.LogError("Cannot claim tokens due to prior dusting issue");
} catch (Exception ex) {
    Debug.LogError($"Failed to claim tokens: {ex.Message}");
}
```

## Error Handling

The SDK throws specific exceptions for different error conditions:

- `InsufficientBalanceException` - Thrown when the account doesn't have enough tokens
- `TransferMethodNotSupportedException` - Thrown when no supported transfer method is available
- `MissingWalletException` - Thrown when trying to perform operations without a wallet
- `PriorDustingException` - Thrown during claim operations when there are issues with prior token dusting

Example with error handling:

```csharp
try {
    string txHash = await rlyNetwork.TransferAsync(destinationAddress, amount);
    Debug.Log($"Transfer successful! Transaction hash: {txHash}");
} catch (InsufficientBalanceException) {
    Debug.LogError("Not enough tokens to complete the transfer");
} catch (TransferMethodNotSupportedException) {
    Debug.LogError("The token doesn't support the required transfer methods");
} catch (MissingWalletException) {
    Debug.LogError("No wallet found. Please create or import a wallet first");
} catch (Exception ex) {
    Debug.LogError($"Transfer failed: {ex.Message}");
}
```

## Working with the Gas Station Network (GSN)

For more advanced use cases, you might need to create and relay custom transactions using the GSN directly:

```csharp
// Create GSN transaction details
GsnTransactionDetails tx = new GsnTransactionDetails
{
    From = account.Address,
    To = contractAddress,
    Data = encodedFunctionData,
    Value = "0",
    Gas = estimatedGas.HexValue,
    MaxFeePerGas = maxFeePerGas.HexValue,
    MaxPriorityFeePerGas = maxPriorityFeePerGas.HexValue
};

// Relay the transaction
string txHash = await rlyNetwork.RelayAsync(tx);
```

See the [GSN Operations](./gsn-operations.md) document for more details on working directly with the Gas Station Network.

## Monitoring Transaction Status

After initiating a transaction, you might want to monitor its status:

```csharp
// Get the Web3 provider
Web3 web3 = await rlyNetwork.GetProviderAsync();

// Check the transaction receipt to verify completion
var receipt = await web3.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(txHash);
if (receipt != null && receipt.Status.Value == 1)
{
    Debug.Log("Transaction was successful");
}
else if (receipt != null && receipt.Status.Value == 0)
{
    Debug.LogError("Transaction failed");
}
else
{
    Debug.Log("Transaction is still pending");
}
```

## Best Practices

1. **Always validate user input** before submitting transactions
2. **Implement proper error handling** to provide feedback to users
3. **Consider using gasless transactions** with GSN to improve user experience
4. **Wait for transaction confirmations** for important operations
5. **Store transaction hashes** for later reference or verification
6. **Implement retry logic** for failed transactions
7. **Use event callbacks** to update your UI when transactions are processed
