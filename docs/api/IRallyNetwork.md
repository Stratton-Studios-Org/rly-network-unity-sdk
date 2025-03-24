# IRallyNetwork

The `IRallyNetwork` interface is the primary entry point for interacting with the Rally Protocol in your Unity application.

## Interface Definition

```csharp
public interface IRallyNetwork
{
    IRallyAccountManager AccountManager { get; }
    IRallyLogger Logger { get; }
    IRallyHttpHandler HttpHandler { get; }
    IGsnClient GsnClient { get; }
    
    Task<Web3> GetProviderAsync();
    Task<Account> GetAccountAsync();
    Task<decimal> GetDisplayBalanceAsync(string? tokenAddress = null);
    Task<BigInteger> GetExactBalanceAsync(string? tokenAddress = null);
    Task<string> TransferAsync(string destinationAddress, decimal amount, MetaTxMethod? metaTxMethod = null, string? tokenAddress = null, TokenConfig tokenConfig = null);
    Task<string> TransferExactAsync(string destinationAddress, BigInteger amount, MetaTxMethod? metaTxMethod = null, string? tokenAddress = null, TokenConfig tokenConfig = null);
    Task<string> ClaimRlyAsync();
    Task<string> RelayAsync(GsnTransactionDetails tx);
    void SetApiKey(string apiKey);
}
```

## Properties

### AccountManager

```csharp
IRallyAccountManager AccountManager { get; }
```

Provides access to the account management functionality, including account creation, loading, and key management.

**Example:**

```csharp
IRallyAccountManager accountManager = rlyNetwork.AccountManager;
var account = await accountManager.GetAccountAsync();
```

### Logger

```csharp
IRallyLogger Logger { get; }
```

Provides access to the Rally Protocol logging system.

**Example:**

```csharp
IRallyLogger logger = rlyNetwork.Logger;
logger.Log("Message from Rally Network");
```

### HttpHandler

```csharp
IRallyHttpHandler HttpHandler { get; }
```

Provides access to the HTTP handler used for network requests.

**Example:**

```csharp
IRallyHttpHandler httpHandler = rlyNetwork.HttpHandler;
```

### GsnClient

```csharp
IGsnClient GsnClient { get; }
```

Provides access to the Gas Station Network (GSN) client for gasless transactions.

**Example:**

```csharp
IGsnClient gsnClient = rlyNetwork.GsnClient;
```

## Methods

### GetProviderAsync

```csharp
Task<Web3> GetProviderAsync()
```

Provides access to the underlying Nethereum Web3 instance, allowing for direct blockchain interactions beyond the built-in Rally Protocol functionality.

**Returns:** A task that resolves to the Web3 provider instance.

**Example:**

```csharp
Web3 web3 = await rlyNetwork.GetProviderAsync();
var blockNumber = await web3.Eth.Blocks.GetBlockNumber.SendRequestAsync();
```

### GetAccountAsync

```csharp
Task<Account> GetAccountAsync()
```

Gets the current Nethereum account associated with the network.

**Returns:** A task that resolves to the current account.

**Throws:** `MissingWalletException` if no account is available.

**Example:**

```csharp
Account account = await rlyNetwork.GetAccountAsync();
Debug.Log($"Account address: {account.Address}");
```

### GetDisplayBalanceAsync

```csharp
Task<decimal> GetDisplayBalanceAsync(string? tokenAddress = null)
```

Retrieves the token balance for the current account in a human-readable decimal format.

**Parameters:**

- `tokenAddress` (optional): The address of the token contract. If null, the RLY token address is used.

**Returns:** A task that resolves to the decimal balance of tokens.

**Example:**

```csharp
// Get RLY balance
decimal rlyBalance = await rlyNetwork.GetDisplayBalanceAsync();
Debug.Log($"RLY Balance: {rlyBalance}");

// Get balance of a specific token
string customTokenAddress = "0x1234567890123456789012345678901234567890";
decimal tokenBalance = await rlyNetwork.GetDisplayBalanceAsync(customTokenAddress);
Debug.Log($"Token Balance: {tokenBalance}");
```

### GetExactBalanceAsync

```csharp
Task<BigInteger> GetExactBalanceAsync(string? tokenAddress = null)
```

Retrieves the exact token balance for the current account as a BigInteger.

**Parameters:**

- `tokenAddress` (optional): The address of the token contract. If null, the RLY token address is used.

**Returns:** A task that resolves to the exact balance as a BigInteger.

**Throws:** `RallyNoAccountException` if no account is available.

**Example:**

```csharp
// Get exact RLY balance
BigInteger exactRlyBalance = await rlyNetwork.GetExactBalanceAsync();
Debug.Log($"Exact RLY Balance: {exactRlyBalance}");

// Get exact balance of a specific token
string customTokenAddress = "0x1234567890123456789012345678901234567890";
BigInteger exactTokenBalance = await rlyNetwork.GetExactBalanceAsync(customTokenAddress);
Debug.Log($"Exact Token Balance: {exactTokenBalance}");
```

### TransferAsync

```csharp
Task<string> TransferAsync(string destinationAddress, decimal amount, MetaTxMethod? metaTxMethod = null, string? tokenAddress = null, TokenConfig tokenConfig = null)
```

Transfers tokens from the current account to another address.

**Parameters:**

- `destinationAddress`: The Ethereum address to send tokens to.
- `amount`: The amount of tokens to transfer in decimal format.
- `metaTxMethod` (optional): The meta-transaction method to use (Permit or ExecuteMetaTransaction).
- `tokenAddress` (optional): The address of the token contract. If null, the RLY token address is used.
- `tokenConfig` (optional): Configuration for the token transfer.

**Returns:** A task that resolves to the transaction hash of the transfer transaction.

**Throws:** 
- `InsufficientBalanceException` if the account has insufficient funds.
- `TransferMethodNotSupportedException` if no supported transfer method is available.

**Example:**

```csharp
try
{
    string destinationAddress = "0x1234567890123456789012345678901234567890";
    decimal amount = 5.0m;
    
    string txHash = await rlyNetwork.TransferAsync(destinationAddress, amount);
    Debug.Log($"Transferred {amount} RLY tokens successfully. Transaction: {txHash}");
    
    // Transfer with custom token address
    string customTokenAddress = "0x1234567890123456789012345678901234567890";
    txHash = await rlyNetwork.TransferAsync(destinationAddress, amount, null, customTokenAddress);
    Debug.Log($"Transferred {amount} custom tokens successfully. Transaction: {txHash}");
}
catch (Exception ex)
{
    Debug.LogError($"Failed to transfer tokens: {ex.Message}");
}
```

### TransferExactAsync

```csharp
Task<string> TransferExactAsync(string destinationAddress, BigInteger amount, MetaTxMethod? metaTxMethod = null, string? tokenAddress = null, TokenConfig tokenConfig = null)
```

Transfers the exact amount of tokens from the current account to another address.

**Parameters:**

- `destinationAddress`: The Ethereum address to send tokens to.
- `amount`: The exact amount of tokens to transfer as a BigInteger.
- `metaTxMethod` (optional): The meta-transaction method to use (Permit or ExecuteMetaTransaction).
- `tokenAddress` (optional): The address of the token contract. If null, the RLY token address is used.
- `tokenConfig` (optional): Configuration for the token transfer.

**Returns:** A task that resolves to the transaction hash of the transfer transaction.

**Throws:** 
- `InsufficientBalanceException` if the account has insufficient funds.
- `TransferMethodNotSupportedException` if no supported transfer method is available.

**Example:**

```csharp
try
{
    string destinationAddress = "0x1234567890123456789012345678901234567890";
    BigInteger amount = new BigInteger(5000000000000000000); // 5 tokens with 18 decimals
    
    string txHash = await rlyNetwork.TransferExactAsync(destinationAddress, amount);
    Debug.Log($"Transferred exact amount successfully. Transaction: {txHash}");
}
catch (Exception ex)
{
    Debug.LogError($"Failed to transfer exact amount: {ex.Message}");
}
```

### ClaimRlyAsync

```csharp
Task<string> ClaimRlyAsync()
```

Claims RLY tokens from the token faucet contract. This is typically used to obtain initial tokens for testing or onboarding.

**Returns:** A task that resolves to the transaction hash of the claim transaction.

**Throws:**
- `PriorDustingException` if there's an issue with the account's balance.

**Example:**

```csharp
try
{
    string txHash = await rlyNetwork.ClaimRlyAsync();
    Debug.Log($"Claimed RLY tokens successfully. Transaction: {txHash}");
}
catch (Exception ex)
{
    Debug.LogError($"Failed to claim RLY tokens: {ex.Message}");
}
```

### RelayAsync

```csharp
Task<string> RelayAsync(GsnTransactionDetails tx)
```

Relays a transaction through the Gas Station Network (GSN) to enable gasless transactions.

**Parameters:**

- `tx`: The GSN transaction details.

**Returns:** A task that resolves to the transaction hash of the relayed transaction.

**Example:**

```csharp
// This is typically used internally by other methods
GsnTransactionDetails tx = new GsnTransactionDetails
{
    From = "0xYourAddress",
    To = "0xDestinationAddress",
    Data = "0xEncodedFunctionData",
    // Other required parameters
};

string txHash = await rlyNetwork.RelayAsync(tx);
Debug.Log($"Transaction relayed successfully: {txHash}");
```

### SetApiKey

```csharp
void SetApiKey(string apiKey)
```

Sets the API key for the Rally Protocol services.

**Parameters:**

- `apiKey`: The API key to use for Rally Protocol services.

**Example:**

```csharp
rlyNetwork.SetApiKey("your-api-key-here");
```

## Usage Examples

### Initializing and Using the Network

```csharp
using UnityEngine;
using System;
using System.Threading.Tasks;
using RallyProtocolUnity.Runtime.Core;

public class RallyNetworkExample : MonoBehaviour
{
    private IRallyNetwork rlyNetwork;

    async void Start()
    {
        // Get the network instance - Implementation depends on your setup
        // For example, using the RallyNetworkFactory:
        // this.rlyNetwork = RallyNetworkFactory.Create(...);
        
        // Create an account if needed
        var accountManager = this.rlyNetwork.AccountManager;
        var account = await accountManager.GetAccountAsync();
        
        if (account == null)
        {
            await accountManager.CreateAccountAsync();
            account = await accountManager.GetAccountAsync();
        }
        
        Debug.Log($"Account address: {account.Address}");
        
        // Get balance
        decimal balance = await this.rlyNetwork.GetDisplayBalanceAsync();
        Debug.Log($"Initial balance: {balance} RLY");
        
        // Claim RLY if balance is low
        if (balance < 1.0m)
        {
            try
            {
                string txHash = await this.rlyNetwork.ClaimRlyAsync();
                Debug.Log($"Claimed RLY. Transaction: {txHash}");
                
                // Get updated balance
                balance = await this.rlyNetwork.GetDisplayBalanceAsync();
                Debug.Log($"New balance: {balance} RLY");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to claim RLY: {ex.Message}");
            }
        }
    }
}
```

## Best Practices

1. Always check for an existing account before performing operations
2. Handle exceptions properly for all network operations
3. Use the appropriate balance method for your needs: `GetDisplayBalanceAsync` for human-readable balances and `GetExactBalanceAsync` for precise calculations
4. Use the `GetProviderAsync()` method carefully to access the Web3 instance for operations not covered by the built-in methods
5. Remember that all methods are asynchronous and should be awaited properly
