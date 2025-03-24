# Core Concepts

This document explains the fundamental concepts of the Rally Protocol Unity SDK.

## Rally Protocol Overview

Rally Protocol is a blockchain infrastructure that allows developers to incorporate decentralized finance features into their applications. The Unity SDK specifically enables Unity developers to integrate these features into their games and applications.

## Key Components

### RallyNetwork

The `IRallyNetwork` interface is the primary entry point to the Rally Protocol functionality. It provides access to account management, token transactions, and other blockchain operations.

```csharp
IRallyNetwork rlyNetwork = RallyUnityManager.Instance.RlyNetwork;
```

### Account Management

The `IRallyAccountManager` handles wallet creation, key management, and account operations:

```csharp
IRallyAccountManager accountManager = rlyNetwork.AccountManager;
```

Account management includes:

- Creating new wallets
- Loading existing wallets
- Secure storage of wallet keys
- Cloud backup options for keys

### Network Types and Configuration

The SDK supports different blockchain networks:

```csharp
// Using a predefined network type
RallyNetworkType networkType = RallyNetworkType.Polygon;
IRallyNetwork rlyNetwork = RallyUnityNetworkFactory.Create(networkType, apiKey);

// Using a custom network configuration
RallyNetworkConfig config = RallyNetworkConfig.Polygon;
config.Gsn.RelayUrl = "https://api.rallyprotocol.com";
IRallyNetwork rlyNetwork = RallyUnityNetworkFactory.Create(config, apiKey);
```

### Gas Station Network (GSN)

The Rally Protocol uses GSN to enable gasless transactions for users. This means:

- Users don't need ETH to pay for gas
- Transaction fees are covered by the application developer
- Transactions are relayed through the GSN infrastructure

```csharp
// Define a GSN transaction
GsnTransactionDetails gsnTx = new GsnTransactionDetails
{
    // Transaction details
};

// Relay the transaction
await rlyNetwork.RelayAsync(gsnTx);
```

### RLY Token

RLY is the native token of the Rally Protocol ecosystem:

```csharp
// Claim RLY tokens
await rlyNetwork.ClaimRlyAsync();

// Transfer RLY tokens
await rlyNetwork.Transfer(destinationAddress, amount);

// Get RLY balance
decimal balance = await rlyNetwork.GetBalanceAsync();
```

## Threading and Asynchronous Operations

Most Rally Protocol operations are asynchronous and should be handled appropriately in your Unity application:

```csharp
public async void CreateAccountExample()
{
    try
    {
        // Asynchronous operation
        await accountManager.CreateAccountAsync();
    }
    catch (Exception ex)
    {
        Debug.LogError($"Error: {ex.Message}");
    }
}
```

## Settings Presets

The SDK provides a settings preset system for easily configuring the SDK:

```csharp
// Load a custom settings preset
RallyProtocolSettingsPreset preset = Resources.Load<RallyProtocolSettingsPreset>("myPreset");
IRallyNetwork rlyNetwork = RallyUnityNetworkFactory.Create(preset);
```

## Logging

The SDK includes a comprehensive logging system:

```csharp
// Configure logging
RallyUnityLogger.Default.UnityLogger.filterLogType = LogType.Log;
```

## Web3 Integration

The SDK provides a bridge to Web3 functionality through Nethereum:

```csharp
// Access the underlying Web3 instance
Nethereum.Web3.Web3 web3 = rlyNetwork.Web3;
```

This allows you to access the full power of Ethereum interactions for advanced use cases.
