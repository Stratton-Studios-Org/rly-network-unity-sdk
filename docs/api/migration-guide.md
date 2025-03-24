# Migration Guide

This guide provides instructions for migrating between different versions of the Rally Protocol Unity SDK.

## Migrating to v1.3.x

### Changes in v1.3.x

Version 1.3.x introduces improvements to error handling and stability enhancements:

1. Enhanced error types with more specific exceptions
2. Improved transaction reliability with automatic retries for network issues
3. Better event handling with more granular events

### Migration Steps

#### Error Handling Updates

The error handling mechanism has been updated with more specific exceptions:

```csharp
// Old approach (before v1.3.0)
try {
    await rallyTransfer.TransferTokensAsync(recipient, amount);
} catch (Exception ex) {
    Debug.LogError("Transfer failed: " + ex.Message);
}

// New approach (v1.3.0+)
try {
    await rallyTransfer.TransferTokensAsync(recipient, amount);
} catch (TransactionException ex) {
    // Handle specific transaction errors
    Debug.LogError("Transaction error: " + ex.Message);
} catch (ConnectionException ex) {
    // Handle network connection errors
    Debug.LogError("Network error: " + ex.Message);
} catch (Exception ex) {
    // Handle other unexpected errors
    Debug.LogError("Unexpected error: " + ex.Message);
}
```

#### Event System Updates

More granular events have been added to component classes:

```csharp
// Old approach (before v1.3.0)
rallyTransfer.OnTransferred.AddListener((txHash) => {
    Debug.Log("Transfer complete with hash: " + txHash);
});

// New approach (v1.3.0+)
// More specific events
rallyTransfer.OnTransferring.AddListener((recipient, amount) => {
    Debug.Log($"Transfer of {amount} starting to {recipient}");
});

rallyTransfer.OnTransferred.AddListener((txHash) => {
    Debug.Log("Transfer complete with hash: " + txHash);
});

rallyTransfer.OnError.AddListener((errorMessage) => {
    Debug.LogError("Transfer error: " + errorMessage);
});
```

## Migrating to v1.2.x

### Changes in v1.2.x

Version 1.2.x introduces several new features:

1. Balance polling system for automatic balance updates
2. Enhanced event system for component notifications
3. Improved network configuration options

### Migration Steps

#### Balance Handling Updates

The balance system now supports automatic polling:

```csharp
// Old approach (before v1.2.0)
public async void RefreshBalance() {
    decimal balance = await RallyUnityManager.Instance.RlyNetwork.GetDisplayBalanceAsync();
    balanceText.text = balance.ToString();
}

// Call manually when needed
RefreshBalance();

// New approach (v1.2.0+)
public RallyGetBalance balanceComponent;

void Start() {
    // Configure auto-refresh
    balanceComponent.pollingEnabled = true;
    balanceComponent.pollingIntervalSeconds = 15;
    
    // Listen for balance updates
    balanceComponent.OnBalanceUpdated.AddListener((newBalance) => {
        balanceText.text = newBalance.ToString();
    });
    
    // Start polling
    balanceComponent.StartPolling();
}

void OnDestroy() {
    balanceComponent.StopPolling();
}
```

#### Network Configuration Changes

Network configuration has been enhanced with preset support:

```csharp
// Old approach (before v1.2.0)
IRallyNetwork network = RallyUnityNetworkFactory.Create(RallyNetworkType.Polygon);

// New approach (v1.2.0+)
// Using presets
var networkSettings = RallyUnityNetworkFactory.LoadMainSettingsPreset();
IRallyNetwork network = RallyUnityNetworkFactory.Create(networkSettings);

// Or more customization
var customSettings = new RallyProtocolSettingsPreset {
    ChainId = 137,
    NetworkType = RallyNetworkType.Polygon,
    RpcUrl = "https://your-custom-rpc.example.com",
    ContractAddresses = new ContractAddressConfig {
        RlyERC20 = "0x1234...",
        // Other contract addresses
    }
};
IRallyNetwork network = RallyUnityNetworkFactory.Create(customSettings);
```

## Migrating to v1.1.x

### Changes in v1.1.x

Version 1.1.x introduces support for Gas Station Network (GSN) for gasless transactions:

1. Added GSN support for gasless transactions
2. Enhanced transfer methods with meta transaction options
3. Improved account management

### Migration Steps

#### Transfer Method Updates

The transfer methods now support meta transaction methods:

```csharp
// Old approach (before v1.1.0)
string txHash = await network.TransferAsync(recipient, amount);

// New approach (v1.1.0+)
// Regular transfer (same as before)
string txHash = await network.TransferAsync(recipient, amount);

// Gasless transfer using GSN
string txHash = await network.TransferAsync(recipient, amount, MetaTxMethod.Gsn);
```

#### Component Updates

The component classes have been updated to support GSN:

```csharp
// Old approach (before v1.1.0)
public RallyTransfer rallyTransfer;

void SendTokens() {
    rallyTransfer.recipientAddress = "0xRecipient";
    rallyTransfer.amount = 1.5m;
    rallyTransfer.TransferTokens();
}

// New approach (v1.1.0+)
public RallyTransfer rallyTransfer;

void SendTokens() {
    rallyTransfer.recipientAddress = "0xRecipient";
    rallyTransfer.amount = 1.5m;
    rallyTransfer.useGaslessTransactions = true; // Enable GSN
    rallyTransfer.TransferTokens();
}
```

## Migrating to v1.0.x

### Initial Setup for v1.0.x

If you are starting with v1.0.x, follow these steps to set up the SDK:

1. Import the Rally Protocol Unity SDK package
2. Add the RallyUnityManager prefab to your scene
3. Configure the network type in the inspector
4. Initialize the SDK at runtime

```csharp
// Basic initialization
void Start() {
    // The SDK is automatically initialized when the RallyUnityManager is in your scene
    // You can check if initialization is complete:
    if (RallyUnityManager.Instance.IsInitialized) {
        Debug.Log("SDK is ready to use");
    } else {
        RallyUnityManager.Instance.OnInitialized += () => {
            Debug.Log("SDK initialization completed");
        };
    }
}
```

## General Migration Best Practices

1. **Always backup your project** before upgrading the SDK
2. Read the release notes for each version to understand all changes
3. Test thoroughly in a development environment before deploying
4. Update one version at a time if migrating across multiple versions
5. Update your error handling to match new exception patterns

## Troubleshooting Common Migration Issues

### Issue: Component References Lost After Upgrade

**Solution:** If Inspector references to SDK components are lost after upgrading:

1. Check for any renamed components
2. Re-assign the references in the Inspector
3. Ensure you're using the correct namespaces in scripts

### Issue: Compilation Errors with API Changes

**Solution:** If you encounter compilation errors due to API changes:

1. Review the specific error messages
2. Check this migration guide for specific API changes
3. Update your code to use the new API patterns
4. Remove any references to deprecated methods

### Issue: Runtime Errors After Migration

**Solution:** If you encounter runtime errors after migrating:

1. Enable debug logs for more detailed error information
2. Check for null references to SDK components
3. Verify that initialization is complete before calling SDK methods
4. Ensure your Unity version is compatible with the SDK version

## Related Documentation

- [Versioning](./versioning.md) - For version compatibility information
- [Error Handling](./error-handling.md) - For updated error handling approaches
