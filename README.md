# Rally Protocol Unity SDK

The Rally Protocol Unity SDK enables Unity developers to integrate blockchain functionality into their games and applications, leveraging the Rally Protocol for decentralized finance and blockchain interactions.

[![License](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE)
[![Unity](https://img.shields.io/badge/Unity-2021.3+-black.svg)](https://unity.com)

## Overview

The Rally Protocol Unity SDK provides a complete solution for integrating blockchain functionality into Unity games and applications. With features for account management, token transfers, gasless transactions, and more, the SDK makes it easy to incorporate Web3 functionality into your Unity projects.

## Documentation

For comprehensive documentation, visit our [Documentation Website](https://docs.rallyprotocol.com/) or explore the [docs](./docs/) directory in this repository.

- [Getting Started Guide](./docs/guides/getting-started.md)
- [API Reference](./docs/api/index.md)
- [Examples](./docs/examples/index.md)
- [Guides](./docs/guides/index.md)

## Project Structure

- `src/RallyProtocol.Unity`: The primary Unity project and SDK code
- `src/RallyProtocol.Unity.Publish`: The Unity publish project for exporting the installer
- `src/RallyProtocol.Unity.AndroidPlugin`: The Android plugin for the Unity project
- `src/RallyProtocol.Unity.iOSPlugin`: The iOS plugin for the Unity project
- `src/RallyProtocol.NET`: The .NET SDK for the Rally Protocol
- `src/Dependencies`: The dependencies for the .NET SDK project
- `docs`: Comprehensive documentation

## Quick Start

### Installation

#### Method 1: Using the Unity Package Manager

1. Open your Unity project
2. Go to **Window > Package Manager**
3. Click the **+** button and select **Add package from git URL...**
4. Enter: `https://github.com/rally-dfs/rly-network-unity-sdk.git`
5. Click **Add**

#### Method 2: Using the Rally Protocol Installer

1. Download the latest installer from the [GitHub releases page](https://github.com/rally-dfs/rly-network-unity-sdk/releases)
2. Import the package into your Unity project via **Assets > Import Package > Custom Package...**

### Basic Usage

#### 1. Setup
Open the Rally Protocol setup window via **Window > Rally Protocol > Setup** to configure your API key and network.

#### 2. Creating an Account

```csharp
IRallyNetwork rlyNetwork = RallyUnityManager.Instance.RlyNetwork;
IRallyAccountManager accountManager = rlyNetwork.AccountManager;

// Create a new account
await accountManager.CreateAccountAsync(new()
{
    Overwrite = false,
    StorageOptions = new()
    {
        SaveToCloud = true,
        RejectOnCloudSaveFailure = false
    }
});
```

#### 3. Claiming and Transferring Tokens

```csharp
// Claim RLY tokens
await rlyNetwork.ClaimRlyAsync();

// Check balance
decimal balance = await rlyNetwork.GetBalanceAsync();

// Transfer tokens
string destinationAddress = "0xRecipientAddressHere";
decimal amount = 5.0m;
await rlyNetwork.TransferAsync(destinationAddress, amount);
```

## Features

- **Account Management**: Create, store, and manage blockchain accounts
- **Token Operations**: Claim and transfer RLY tokens
- **Gasless Transactions**: Use the Gas Station Network for gasless transactions
- **Cross-Platform Support**: iOS and Android support via platform-specific plugins
- **Unity Integration**: Seamless integration with the Unity editor and runtime

## Troubleshooting

You need to enable Git longpaths before cloning, just run the below command in the terminal (this is intended for Windows users only):

```
git config --system core.longpaths true
```

If you're using Git GUIs, use `--global` like so:

```
git config --global core.longpaths true
```

## Running Tests

The tests are included in the `src/RallyProtocol.Unity` project. To run the tests, open the project in Unity and use the Unity Test Runner window.

## Building the Installer

To build the installer, open the `src/RallyProtocol.Unity.Publish` project in Unity and export the installer from the **Stratton Studios > Export Package Utility** menu by selecting **Rally Protocol - Installer** package.

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Resources

- [Official Website](https://rallyprotocol.com)
- [Documentation](https://docs.rallyprotocol.com/)
- [Flutter SDK](https://github.com/rally-dfs/flutter-sdk)
- [Mobile SDK](https://github.com/rally-dfs/rly-network-mobile-sdk)
