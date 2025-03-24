# Rally Protocol Unity SDK - Frequently Asked Questions

This document provides answers to frequently asked questions about the Rally Protocol Unity SDK. If you don't find the answer to your question here, please check the official documentation or contact Rally Protocol support.

## General Questions

### What is the Rally Protocol Unity SDK?

The Rally Protocol Unity SDK is a software development kit that enables Unity game developers to integrate blockchain functionality into their games and applications. It provides tools for creating and managing blockchain wallets, handling transactions, and implementing token-based features like rewards, purchases, and trading.

### What platforms does the Rally Protocol Unity SDK support?

The SDK supports the following platforms:

- **iOS**: Fully supported for deployment
- **Android**: Fully supported for deployment
- **Unity Editor**: Supported for development on Windows, macOS, and Linux

Note: Standalone builds for Windows, macOS, and Linux are not supported for deployment.

### What are the minimum Unity requirements?

The SDK requires Unity 2021.3 or newer. We recommend using the latest LTS (Long Term Support) version of Unity for the best compatibility and features.

### Is the Rally Protocol Unity SDK free to use?

Yes, the SDK itself is free to use. However, some operations on the blockchain may incur gas fees or other costs depending on the network and operations performed. Rally Protocol uses a gas-less transaction system for many operations, which can significantly reduce or eliminate costs for end users.

### How do I get started with the Rally Protocol Unity SDK?

To get started:

1. Import the SDK package into your Unity project
2. Configure the SDK with your API key
3. Initialize the SDK in your game
4. Start implementing blockchain features

Detailed step-by-step instructions are available in our [Getting Started Guide](getting-started.md).

## Technical Questions

### How does the Rally Protocol handle wallet creation and management?

The Rally Protocol SDK provides multiple options for wallet creation and management:

1. **Custodial Wallets**: The SDK can create managed wallets that are stored securely in the Rally Protocol backend.
2. **Local Wallets**: Private keys can be stored locally on the device, encrypted with a password.
3. **External Wallet Connections**: Users can connect their existing wallets (like MetaMask) to your application.

The default approach uses a combination of local storage with cloud backup options.

### What blockchains does the Rally Protocol support?

The Rally Protocol currently supports:

- Ethereum
- Polygon
- Base
- Other EVM-compatible networks

Check the [Configuration Guide](configuration.md) for the most up-to-date list of supported networks.

### How are transactions processed?

Transactions are processed using the Gas Station Network (GSN), which allows for gasless transactions. This means users can perform blockchain operations without needing to hold the native token (ETH, MATIC, etc.) of the blockchain.

The Rally Protocol abstracts away the complexities of gas management, making blockchain interactions seamless for end users.

### How do I handle different network environments (testnet vs mainnet)?

The SDK supports multiple network configurations:

```csharp
// Example configuration for different environments
[SerializeField] private RallyNetworkType networkEnvironment = RallyNetworkType.BaseSepolia;

private void InitializeRallyWithEnvironment()
{
    // Create a network instance with the specified environment
    IRallyNetwork rlyNetwork = RallyUnityNetworkFactory.Create(networkEnvironment);
    
    // Or use the RallyUnityManager singleton which is initialized automatically
    if (!RallyUnityManager.Instance.IsInitialized)
    {
        RallyUnityManager.Instance.Initialize();
    }
}
```

You can easily switch between environments in your configuration. Always use testnets during development and testing.

### How secure is the Rally Protocol Unity SDK?

The Rally Protocol Unity SDK implements several security measures:

1. **Encrypted Storage**: Private keys are encrypted when stored locally
2. **Secure Communication**: All API calls use HTTPS
3. **API Key Authentication**: Your application is authenticated using your API key
4. **Rate Limiting**: Protection against brute force attacks

Additionally, the SDK undergoes regular security audits and updates.

### Can I use the Rally Protocol SDK with other blockchain SDKs?

Yes, the Rally Protocol SDK can coexist with other blockchain SDKs in your project. However, be careful to avoid conflicts in initialization, event handling, and wallet management. If you need to use multiple blockchain SDKs, we recommend carefully managing the initialization order and creating separate managers for each SDK.

## Implementation Questions

### How do I implement a reward system using the Rally Protocol SDK?

To implement a reward system:

1. Initialize the Rally Protocol SDK
2. Create an account for the user if they don't have one
3. Implement reward trigger points in your game
4. Use `ClaimRlyAsync()` or custom token transfer methods to issue rewards
5. Display balance and transaction history to the user

For a detailed implementation guide, see our [Reward System Tutorial](tutorials/reward-system.md).

### How do I handle offline gameplay and synchronize later?

The SDK provides mechanisms for queuing transactions while offline:

```csharp
// Example offline transaction handling
private List<PendingTransaction> pendingTransactions = new List<PendingTransaction>();

private void QueueTransaction(TransactionType type, decimal amount, string recipient)
{
    pendingTransactions.Add(new PendingTransaction
    {
        Type = type,
        Amount = amount,
        Recipient = recipient,
        Timestamp = System.DateTime.UtcNow
    });
    
    SavePendingTransactionsToPlayerPrefs();
}

private async void ProcessPendingTransactionsWhenOnline()
{
    if (!IsNetworkAvailable() || pendingTransactions.Count == 0)
        return;
        
    foreach (var tx in pendingTransactions.ToList())
    {
        bool success = await ProcessTransaction(tx);
        if (success)
        {
            pendingTransactions.Remove(tx);
        }
    }
    
    SavePendingTransactionsToPlayerPrefs();
}
```

For detailed offline handling strategies, see our [Offline Mode Guide](guides/offline-mode.md).

### How do I integrate with an existing user account system?

The Rally Protocol SDK can be integrated with existing account systems:

1. Generate a unique identifier for each user in your system
2. Use this identifier to create or recover a Rally Protocol wallet
3. Associate the wallet address with the user's account in your system
4. Store this association in your backend

This approach allows users to maintain a single identity across your traditional backend and blockchain interactions.

### How do I handle errors and exceptions?

The SDK provides comprehensive error handling:

```csharp
try
{
    await rlyNetwork.ClaimRlyAsync();
    Debug.Log("Successfully claimed RLY tokens");
}
catch (Exception ex)
{
    if (ex is MissingWalletException)
    {
        Debug.LogError("Wallet not initialized. Please create or recover a wallet first.");
    }
    else if (ex is InsufficientBalanceException)
    {
        Debug.LogError("Insufficient funds for this operation.");
    }
    else
    {
        Debug.LogError($"Error: {ex.Message}");
    }
}
```

For a complete list of error codes and handling strategies, see our [Error Handling Guide](guides/error-handling.md).

### How do I implement in-app purchases with cryptocurrencies?

To implement crypto-based in-app purchases:

1. Define your purchasable items with token prices
2. When a user initiates a purchase, check their token balance
3. If sufficient, execute a token transfer from the user's wallet to your game's wallet
4. Confirm the transaction and grant the purchased item

Here's a simple example:

```csharp
public class StoreManager : MonoBehaviour
{
    [SerializeField] private RallyTransfer rallyTransfer;
    private IRallyNetwork rlyNetwork;
    
    private async void Start()
    {
        // Get reference to the network
        this.rlyNetwork = RallyUnityManager.Instance.RlyNetwork;
    }
    
    public async Task<bool> PurchaseItem(string itemId, decimal price, string storeWalletAddress)
    {
        try
        {
            // Check balance first
            decimal balance = await rlyNetwork.GetDisplayBalanceAsync();
            if (balance < price)
            {
                Debug.LogError("Insufficient funds to purchase this item");
                return false;
            }
            
            // Use the RallyTransfer component for the transfer
            rallyTransfer.DestinationAddress = storeWalletAddress;
            rallyTransfer.Amount = price.ToString();
            
            // Wait for the transfer to complete
            await rallyTransfer.TransferAsync();
            
            // Grant the item to the player (implement your game-specific logic here)
            GrantItemToPlayer(itemId);
            
            return true;
        }
        catch (Exception ex)
        {
            Debug.LogError($"Purchase failed: {ex.Message}");
            return false;
        }
    }
    
    private void GrantItemToPlayer(string itemId)
    {
        // Implement your game-specific item granting logic
        Debug.Log($"Granted item {itemId} to player");
    }
}
```

For platform-specific considerations (iOS, Android), see our [In-App Purchase Guide](guides/in-app-purchases.md).

## Performance Questions

### How will the Rally Protocol SDK affect my game's performance?

The SDK is designed to minimize performance impact:

1. **Asynchronous Operations**: All blockchain interactions happen asynchronously
2. **Optimized APIs**: The API calls are optimized for minimal data transfer
3. **Caching**: Common data like balances are cached to reduce network calls

However, initial SDK initialization and wallet creation may take a moment, so we recommend performing these operations during loading screens.

### What is the typical response time for blockchain operations?

Response times vary by operation and network conditions:

- **Balance Checks**: 1-3 seconds
- **Token Transfers**: 10-30 seconds for confirmation
- **Account Creation**: 2-5 seconds

These times can vary based on network congestion and the specific blockchain being used. Always implement proper loading states and feedback for users during blockchain operations.

### How can I optimize blockchain operations in my game?

To optimize blockchain operations:

1. **Batch Operations**: Group multiple operations when possible
2. **Lazy Loading**: Only load blockchain data when needed
3. **Background Processing**: Perform non-critical operations in the background
4. **Caching**: Cache results to minimize repeated calls

For more optimization techniques, see our [Performance Optimization Guide](guides/performance.md).

## Platform-Specific Questions

### Are there any special considerations for iOS apps?

iOS has specific requirements:

1. **App Store Guidelines**: Follow Apple's guidelines for cryptocurrency apps
2. **In-App Purchases**: Be aware of Apple's policies regarding crypto transactions
3. **App Tracking Transparency**: Implement necessary permission requests
4. **Deep Linking**: Configure properly for wallet connections

See our [iOS Integration Guide](platform-guides/ios-integration.md) for detailed instructions.

### What about Android-specific considerations?

Android considerations include:

1. **Google Play Policies**: Follow Google's policies for cryptocurrency in apps
2. **Background Operations**: Properly handle blockchain operations when the app is in the background
3. **Power Management**: Be aware of Android's battery optimization features

See our [Android Integration Guide](platform-guides/android-integration.md) for detailed instructions.

### How do I handle WebGL builds?

For WebGL builds:

1. **Browser Wallet Integration**: Seamlessly connect with browser wallets like MetaMask
2. **Cross-Origin Policies**: Configure CORS for API calls
3. **Storage Limitations**: Be aware of local storage limitations in browsers

See our [WebGL Integration Guide](platform-guides/webgl-integration.md) for detailed instructions.

## Account and Wallet Questions

### How are user accounts created and managed?

User accounts can be created and managed in several ways:

1. **Automatic Creation**: The SDK can automatically create new accounts
2. **Email/Password**: Users can create accounts with email and password
3. **Social Login**: Integration with social login providers
4. **Existing Wallets**: Users can connect existing wallets

The default flow uses email verification with optional social login.

### What happens if a user loses their password or device?

The SDK provides multiple recovery options:

1. **Email Recovery**: Users can recover via email verification
2. **Recovery Phrase**: A 12-word recovery phrase can be provided at account creation
3. **Cloud Backup**: Optional encrypted cloud backup of wallet information

We recommend implementing multiple recovery methods for the best user experience.

### Can users transfer tokens to external wallets?

Yes, users can transfer tokens to any external wallet address using standard transfer methods:

```csharp
// Example token transfer to external wallet
public async Task<bool> TransferTokensToExternalWallet(string destinationAddress, decimal amount)
{
    try
    {
        // Validate the address (implement your own validation or use a helper)
        if (string.IsNullOrEmpty(destinationAddress) || !destinationAddress.StartsWith("0x"))
        {
            Debug.LogError("Invalid destination address");
            return false;
        }
        
        // Execute the transfer
        string txHash = await rlyNetwork.TransferAsync(destinationAddress, amount);
        Debug.Log($"Transfer successful. Transaction hash: {txHash}");
        
        return true;
    }
    catch (Exception ex)
    {
        Debug.LogError($"Transfer failed: {ex.Message}");
        return false;
    }
}
```

### How do users view their transaction history?

Users can view their transaction history through:

1. **In-Game UI**: Implement a transaction history page in your game
2. **Rally Dashboard**: Users can access the Rally web dashboard
3. **Blockchain Explorer**: Advanced users can use a blockchain explorer with their address

The SDK provides methods to fetch transaction history:

```csharp
// Example fetching transaction history
public async Task<List<TransactionInfo>> GetUserTransactionHistory(int limit = 20)
{
    try
    {
        // Note: GetTransactionHistoryAsync is not part of the standard API
        // Instead, you would need to implement this functionality using the blockchain explorer APIs or events
        var account = await rlyNetwork.GetAccountAsync();
        string address = account.Address;
        
        // Example of how this would be implemented with a block explorer service
        // For actual implementation, refer to your specific network documentation
        // var transactions = await blockExplorerService.GetTransactionsForAddress(address, limit);
        
        // Placeholder for demonstration, replace with actual implementation
        return new List<TransactionInfo>();
    }
    catch (Exception ex)
    {
        Debug.LogError($"Failed to get transaction history: {ex.Message}");
        return new List<TransactionInfo>();
    }
}
```

## Troubleshooting

### Why am I getting "API key not valid" errors?

This could be due to:

- Incorrect API key in your configuration
- API key not activated yet
- Using a testnet key on mainnet or vice versa

Solution: Verify your API key in the Rally Protocol Developer Dashboard and ensure you're using the correct key for your environment.

### Transactions are failing with "insufficient funds" even though the user has tokens

This could be due to:

- Not enough native tokens (ETH, MATIC) for gas fees
- Attempting to transfer more tokens than the user has, including gas costs
- Network configuration issue

Solution: Ensure you're using gasless transactions or that the user has enough native tokens for gas. Also verify token balances include a buffer for fees.

### Users can't see their tokens in external wallets

This could be due to:

- Token contract not added to the external wallet
- Using a testnet token that's not visible in the wallet by default
- Recent transactions that haven't confirmed yet

Solution: Provide users with instructions to add the token contract to their wallet, and ensure you're using the correct network and contract addresses.

### The SDK initialization is taking too long

This could be due to:

- Network connectivity issues
- Server-side issues
- Device performance limitations

Solution: Implement a timeout and retry mechanism, and consider showing a loading indicator during initialization.

## Business Questions

### How do I monetize with the Rally Protocol SDK?

You can monetize your application in several ways:

1. **In-Game Purchases**: Sell in-game items for crypto tokens
2. **NFT Sales**: Create and sell NFTs representing unique items or achievements
3. **Transaction Fees**: Take a small percentage of in-game transactions
4. **Premium Features**: Offer premium features available for token holders

For more monetization strategies, see our [Business Models Guide](guides/business-models.md).

### What are the legal considerations for implementing cryptocurrency in my game?

Legal considerations vary by region but generally include:

1. **Regulatory Compliance**: Understand cryptocurrency regulations in your target markets
2. **KYC/AML Requirements**: Know Your Customer and Anti-Money Laundering rules
3. **Tax Implications**: Understand how cryptocurrency transactions are taxed
4. **Terms of Service**: Update your terms of service to cover blockchain features

We recommend consulting with a legal expert familiar with blockchain technology for your specific case.

### How do I handle customer support for blockchain features?

For effective customer support:

1. **Education**: Provide clear documentation and tutorials
2. **In-Game Support**: Implement in-game help and FAQs
3. **Transaction Lookup**: Create tools to look up transaction status
4. **Support Staff Training**: Ensure your support team understands blockchain basics

Rally Protocol provides additional support resources you can leverage for your users.

## Conclusion

This FAQ covers the most common questions about the Rally Protocol Unity SDK. For more detailed information, please refer to the specific guides and tutorials in our documentation.

If you have questions not covered here, please contact [Rally Protocol Support](https://docs.rallyprotocol.com) or visit our [Developer Forum](https://docs.rallyprotocol.com).
