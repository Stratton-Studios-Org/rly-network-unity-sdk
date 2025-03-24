# Web3 Utilities

The Rally Protocol Unity SDK provides utility functions for common Web3 operations. These utilities simplify interactions with the blockchain and help with common conversion and validation tasks.

> **Note:** This documentation describes utility functions that may be available through the underlying network interfaces. The exact implementation and availability of these functions may vary. When using the SDK's higher-level components like `RallyTransfer`, many of these operations are handled automatically.

## Access to Web3 Instance

To access the Web3 instance for direct blockchain interactions:

```csharp
// Get reference to the Rally network
IRallyNetwork network = RallyUnityManager.Instance.RlyNetwork;

// Get the Web3 provider
var web3 = await network.GetProviderAsync();

// Now you can use web3 for direct Nethereum operations if needed
```

## Working with Accounts

The SDK provides methods to work with blockchain accounts:

```csharp
// Get the current account
var account = await network.GetAccountAsync();

// Access the account address
string address = account.Address;
```

## Balance Operations

To check token balances:

```csharp
// Get user-friendly display balance (in decimal form)
decimal balance = await network.GetDisplayBalanceAsync();

// Get exact balance as BigInteger (in wei or smallest token unit)
BigInteger exactBalance = await network.GetExactBalanceAsync();

// Get balance of a specific token
decimal tokenBalance = await network.GetDisplayBalanceAsync("0xTokenAddress");
```

## Transaction Operations

For sending transactions:

```csharp
// Transfer tokens in decimal format (e.g., 1.5 tokens)
string txHash = await network.TransferAsync(
    "0xRecipientAddress", 
    1.5m
);

// Transfer exact amount (in wei or smallest token unit)
string txHash = await network.TransferExactAsync(
    "0xRecipientAddress",
    BigInteger.Parse("1500000000000000000") // 1.5 tokens with 18 decimals
);

// Transfer tokens with gas station network for gasless transactions
string txHash = await network.TransferAsync(
    "0xRecipientAddress",
    1.5m,
    MetaTxMethod.Gsn
);

// Transfer a specific token
string txHash = await network.TransferAsync(
    "0xRecipientAddress",
    1.5m,
    null, // Default meta tx method
    "0xTokenAddress"
);
```

## GSN Operations

For Gas Station Network operations:

```csharp
// Create GSN transaction details
GsnTransactionDetails tx = new GsnTransactionDetails
{
    ContractAddress = "0xContractAddress",
    EncodedFunction = "0xEncodedFunction",
    Gas = 100000
};

// Send a transaction via GSN
string txHash = await network.RelayAsync(tx);
```

## Network Settings

To configure network settings:

```csharp
// Set API key
network.SetApiKey("your-api-key");
```

## Unity-Specific Utilities

The SDK provides several Unity-specific components and utilities:

### RallyUnityManager

```csharp
// Access the singleton manager
RallyUnityManager manager = RallyUnityManager.Instance;

// Get the Rally network instance
IRallyNetwork network = manager.RlyNetwork;

// Check if the manager is initialized
bool isInitialized = manager.IsInitialized;

// Initialize the manager manually if needed
manager.Initialize();
```

### RallyUnityNetworkFactory

```csharp
// Create a network instance with default settings
IRallyNetwork network = RallyUnityNetworkFactory.Create();

// Create a network with a specific API key
IRallyNetwork network = RallyUnityNetworkFactory.Create("your-api-key");

// Create a network with a specific preset
RallyProtocolSettingsPreset preset = RallyUnityNetworkFactory.LoadMainSettingsPreset();
IRallyNetwork network = RallyUnityNetworkFactory.Create(preset);

// Create a network with a specific type
IRallyNetwork network = RallyUnityNetworkFactory.Create(RallyNetworkType.Polygon);
```

### Unity Components

The SDK provides several Unity components that wrap the common Web3 operations:

- `RallyTransfer`: For transferring tokens
- `RallyGetBalance`: For checking balances
- `RallyInitializeAccount`: For initializing user accounts
- `RallyGetAccountPublicAddress`: For retrieving account addresses
- `RallyClaimRly`: For claiming Rally tokens

## Related Documentation

- [Conversion Utilities](./conversion-utilities.md) - For unit conversion utilities
- [IRallyNetwork](./IRallyNetwork.md) - For main network interface documentation
- [Transaction Operations](./transaction-operations.md) - For transaction operation details
