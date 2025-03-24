# Security Best Practices

This document outlines security best practices for integrating the Rally Protocol Unity SDK into your game or application.

## Secure Account Management

### Private Key Storage

Private keys are sensitive cryptographic assets that should be handled with care. The SDK handles private key storage through an encrypted local wallet system:

```csharp
// The SDK manages private keys internally
// You should NEVER request or handle user private keys directly
var account = await RallyUnityManager.Instance.RlyNetwork.GetAccountAsync();
string address = account.Address; // Safe to use and display the address
```

### Keystore Encryption

The SDK uses encryption to protect local account information:

- Local keystores are encrypted with industry-standard algorithms
- Encryption keys are securely managed by the platform's secure storage system
- You should never need to handle the raw keystores directly

## Secure Transaction Handling

### Signature Verification

Verify the signature of transactions to ensure they haven't been tampered with:

```csharp
// The SDK handles signature verification internally
// For custom verification:
public async Task VerifyTransactionSignature(string message, string signature, string expectedAddress)
{
    // Get a reference to the network
    var network = RallyUnityManager.Instance.RlyNetwork;
    
    // Use the web3 provider for verification
    var web3 = await network.GetProviderAsync();
    var recoveredAddress = web3.Eth.SignVerify(message, signature);
    
    // Validate that the recovered address matches the expected address
    if (recoveredAddress.ToLower() != expectedAddress.ToLower())
    {
        throw new SecurityException("Signature verification failed");
    }
}
```

### Gas Limits and Fees

Set appropriate gas limits and fees to prevent transaction issues:

```csharp
// The SDK manages gas limits and fees automatically
// For transfers with specific settings, use the appropriate overloads
// You rarely need to modify these parameters directly

// Example with auto gas limits (recommended)
await rallyTransfer.TransferTokensAsync(toAddress, amount);
```

## Secure API Usage

### API Key Security

If your implementation requires API keys:

```csharp
// Set API key securely
// Store API keys in a secure location, not hardcoded in scripts
string apiKey = await SecureStorageService.GetApiKeyAsync();
network.SetApiKey(apiKey);
```

### HTTPS Enforcement

Ensure all network requests use HTTPS:

```csharp
// The SDK uses HTTPS by default for all network connections
// When creating custom network settings, always use HTTPS URLs:
var settings = new RallyProtocolSettingsPreset
{
    RpcUrl = "https://secure-rpc.example.com",  // Always use HTTPS
    // Other settings...
};
```

## Input Validation

### Address Validation

Always validate addresses before sending transactions:

```csharp
// Validate address format before sending tokens
bool isValidAddress = !string.IsNullOrEmpty(recipientAddress) && 
                     recipientAddress.StartsWith("0x") && 
                     recipientAddress.Length == 42;

if (!isValidAddress)
{
    Debug.LogError("Invalid Ethereum address format");
    return;
}

// Then proceed with transfer
await rallyTransfer.TransferTokensAsync(recipientAddress, amount);
```

### Amount Validation

Validate token amounts before transactions:

```csharp
// Validate token amount before sending
if (amount <= 0)
{
    Debug.LogError("Amount must be greater than zero");
    return;
}

// Check if user has sufficient balance
decimal balance = await network.GetDisplayBalanceAsync();
if (amount > balance)
{
    Debug.LogError("Insufficient balance for transfer");
    return;
}

// Then proceed with transfer
await rallyTransfer.TransferTokensAsync(recipientAddress, amount);
```

## Error Handling and Logging

### Secure Error Handling

Handle errors securely to prevent information leakage:

```csharp
try
{
    // Attempt sensitive operation
    await network.TransferAsync(recipientAddress, amount);
}
catch (Exception ex)
{
    // Log securely - never expose full errors to users
    Debug.LogError($"Transfer failed: {ex.GetType().Name}");
    
    // User-facing message should be generic
    ShowUserFriendlyError("Unable to complete transaction. Please try again later.");
    
    // Log details securely for developers only
    SecureLogger.LogException(ex);
}
```

### Transaction Logging

Implement secure logging for transactions:

```csharp
// Log transaction details securely
public async Task LogTransaction(string txHash, string recipient, decimal amount)
{
    // Don't log sensitive information such as private keys
    SecureLogger.LogInfo($"Transaction sent: {txHash.Substring(0, 10)}... to {recipient.Substring(0, 10)}...");
    
    // Monitor transaction status
    var network = RallyUnityManager.Instance.RlyNetwork;
    var web3 = await network.GetProviderAsync();
    var receipt = await web3.Eth.TransactionReceipt.SendRequestAsync(txHash);
    
    if (receipt.Status.Value == 1)
    {
        SecureLogger.LogInfo($"Transaction {txHash.Substring(0, 10)}... succeeded");
    }
    else
    {
        SecureLogger.LogWarning($"Transaction {txHash.Substring(0, 10)}... failed");
    }
}
```

## Secure Backend Integration

### Server-Side Verification

For games with a backend, always verify transactions server-side:

```csharp
// Client-side: Send transaction hash to your game server
async Task NotifyGameServer(string txHash, string operation)
{
    var requestData = new Dictionary<string, string>
    {
        { "txHash", txHash },
        { "operation", operation },
        { "timestamp", DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString() }
    };
    
    // Sign the request with the user's account for verification
    var network = RallyUnityManager.Instance.RlyNetwork;
    var account = await network.GetAccountAsync();
    var signatureData = JsonConvert.SerializeObject(requestData);
    
    // Your game server should verify this transaction on the blockchain
    // and verify the signature matches the user's address
    await GameServerAPI.NotifyTransaction(requestData, signatureData);
}
```

## Protecting User Privacy

### Minimize Data Collection

Only collect necessary blockchain data:

```csharp
// Only request and store necessary data
public async Task<UserGameData> GetUserGameData()
{
    var network = RallyUnityManager.Instance.RlyNetwork;
    var account = await network.GetAccountAsync();
    
    return new UserGameData
    {
        // Only store the address, not private keys or other sensitive data
        PublicAddress = account.Address,
        // Only query the specific game token balance
        GameTokenBalance = await network.GetDisplayBalanceAsync(gameTokenAddress)
    };
}
```

## Regular Security Updates

Keep the SDK updated to benefit from security improvements:

```csharp
// Implement your own version checking mechanism
// The SDK does not provide direct version access
CheckForSDKUpdates();
```

## Related Documentation

- [Error Handling](./error-handling.md) - Detailed error handling strategies
- [Transaction Callbacks](./transaction-callbacks.md) - Safe transaction handling
