# Getting Started with Rally Protocol Unity SDK

This guide will help you get started with the Rally Protocol Unity SDK in your Unity project.

## Prerequisites

- Unity 2021.3 or newer
- Basic knowledge of Unity development
- An API key from Rally Protocol (obtain one at [Rally Protocol Developer Portal](https://docs.rallyprotocol.com/))

## Installation

### Method 1: Using the Unity Package Manager

1. Open your Unity project
2. Go to **Window > Package Manager**
3. Click the **+** button in the top-left corner
4. Select **Add package from git URL...**
5. Enter the following URL: `https://github.com/rally-dfs/rly-network-unity-sdk.git`
6. Click **Add**

### Method 2: Using the Rally Protocol Installer

1. Open your Unity project
2. Download the latest Rally Protocol Unity SDK installer from the [GitHub releases page](https://github.com/rally-dfs/rly-network-unity-sdk/releases)
3. Double-click the downloaded package or import it via **Assets > Import Package > Custom Package...**
4. Ensure all components are selected and click **Import**

## Setting Up

1. After installation, open the Rally Protocol setup window via **Window > Rally Protocol > Setup**
2. Enter your API key in the provided field
3. Select the appropriate network type for your project (e.g., Polygon)
4. Click **Save Settings**

## Initial Configuration

The Rally Protocol SDK requires minimal configuration to get started. Once you've set up your API key and network, you can access the Rally Network instance through:

```csharp
IRallyNetwork rlyNetwork = RallyUnityManager.Instance.RlyNetwork;
```

## Creating Your First Account

To create a new account for your users:

```csharp
IRallyAccountManager accountManager = rlyNetwork.AccountManager;

// Create a new account
await accountManager.CreateAccountAsync(new()
{
    Overwrite = false,
    StorageOptions = new()
    {
        RejectOnCloudSaveFailure = true,
        SaveToCloud = true
    }
});
```

## Making Your First Transaction

After creating an account, you can claim RLY tokens:

```csharp
// Claim RLY
await rlyNetwork.ClaimRlyAsync();

// Or transfer some RLY to another address
string destinationAddress = "0xRecipientAddressHere";
decimal amount = 5;
await rlyNetwork.Transfer(destinationAddress, amount);
```

## Next Steps

- Explore the [Examples](../examples/README.md) to see more advanced use cases
- Learn about the [Core Concepts](./core-concepts.md) of the Rally Protocol
- Dive into the [API Reference](../api/README.md) for detailed documentation

If you encounter any issues during setup, please check the [Troubleshooting](./troubleshooting.md) page or raise an issue on the [GitHub repository](https://github.com/rally-dfs/rly-network-unity-sdk/issues).
