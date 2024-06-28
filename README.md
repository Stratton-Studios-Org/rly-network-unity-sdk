# Rally Protocol Unity SDK

The Rally Protocol Unity SDK, learn more at https://docs.rallyprotocol.com/

## Folder Structure

- `src/RallyProtocol.Unity`: The primary Unity project and SDK code
- `src/RallyProtocol.Unity.Publish`: The Unity publish project for exporting the installer
- `src/RallyProtocol.Unity.AndroidPlugin`: The Android plugin for the Unity project
- `src/RallyProtocol.Unity.iOSPlugin`: The iOS plugin for the Unity project
- `src/RallyProtocol.NET`: The .NET SDK for the Rally Protocol
- `src/Dependencies`: The dependencies for the .NET SDK project

### Initialization

You can initialize a Rally Network instance through any of the below methods:

- Initialize through the main settings preset file that is setup using the **Window > Rally Protocol > Setup** window:

  ```cs
  IRallyNetwork rlyNetwork = RallyUnityManager.Instance.RlyNetwork;
  ```

- Equivelant to the above:

  ```cs
  IRallyNetwork rlyNetwork = RallyUnityNetworkFactory.Create();
  ```

- Create using custom settings preset

  ```cs
  RallyProtocolSettingsPreset preset = Resources.Load<RallyProtocolSettingsPreset>("myPreset");
  IRallyNetwork rlyNetwork = RallyUnityNetworkFactory.Create(preset);
  ```

- Create using network type

  ```cs
  string myApiKey = "...";
  RallyNetworkType networkType = RallyNetworkType.Polygon;
  IRallyNetwork rlyNetwork = RallyUnityNetworkFactory.Create(networkType, myApiKey);
  ```

- Create using network config

  ```cs
  string myApiKey = "...";

  // Create a clone from default Polygon config and apply any changes as needed
  RallyNetworkConfig config = RallyNetworkConfig.Polygon;
  config.Gsn.RelayUrl = "https://api.rallyprotocol.com";

  IRallyNetwork rlyNetwork = RallyUnityNetworkFactory.Create(config, myApiKey);
  ```

## Getting Started

1. Setup the API key and network through Window > Rally Protocol > Setup window
2. Use `RallyUnityManager.Instance.RlyNetwork` to access the network created based upon the settings that were setup

  ```cs
  IRallyNetwork rlyNetwork = RallyUnityManager.Instance.RlyNetwork;

  // Get account manager
  IRallyAccountManager accountManager = rlyNetwork.AccountManager;

  // Load any existing account
  Nethereum.Web3.Accounts.Account account = await accountManager.GetAccountAsync();

  // Create a new account if there are no existing accounts
  if (account == null) {
    await accountManager.CreateAccountAsync(new()
    {
      Overwrite = false,
      StorageOptions = new()
      {
        RejectOnCloudSaveFailure = true,
        SaveToCloud = true
      }
    });
  }

  // Claim RLY for example
  await rlyNetwork.ClaimRlyAsync();

  // Or transfer some RLY
  string destinationAddress = "...";
  decimal amount = 5;
  await rlyNetwork.Transfer(destinationAddress, amount);
  ```

- Get the default instance of Rally Account Manager and Rally Network (EVM network):

  ```cs
  IRallyAccountManager accountManager;
  IRallyNetwork rlyNetwork;

  this.accountManager RallyUnityAccountManager.Default;
  this.rlyNetwork = RallyUnityNetworkFactory.Create();
  ```

- Create a new account

  ```cs
  public async void CreateAccount()
  {
    try
    {
        Debug.Log("Creating account...");

        // Create a new account & overwrite everytime for testing purposes
        await this.accountManager.CreateAccountAsync(new() { Overwrite = true });
        Debug.Log("Account created successfully");
    }
    catch (Exception ex)
    {
        Debug.LogError("Account creation failed");
        Debug.LogException(ex);
    }
  }
  ```

- Claim RLY for the new account:

  ```cs
  public async void ClaimRly()
  {
    try
    {
        Debug.Log("Claiming RLY...");
        await this.rlyNetwork.ClaimRlyAsync();
        Debug.Log("Claimed RLY successfully");
    }
    catch (Exception ex)
    {
        Debug.LogError("Claiming RLY failed");
        Debug.LogException(ex);
    }
  }
  ```

- Relay transactions

  ```cs
  // Define the transaction
  GsnTransactionDetails gsnTx = ...;

  // Relay the transaction
  await this.rlyNetwork.RelayAsync(gsnTx);
  ```

- Change Logging Filter

  ```cs
  // You can set the logging filter using the default logger instance in Unity
  RallyUnityLogger.Default.UnityLogger.filterLogType = LogType.Log;
  ```

## Running Tests

The tests are included in the `src/RallyProtocol.Unity` project. To run the tests, open the project in Unity and run the tests from the Test Runner window.

## Building the Installer

To build the installer, open the `src/RallyProtocol.Unity.Publish` project in Unity and export the installer from the **Stratton Studios > Export Package Utility** menu and selecting **Rally Protocol - Installer** package.

## Resources

- [Flutter SDK](https://github.com/rally-dfs/flutter-sdk)
- [Mobile SDK](https://github.com/rally-dfs/rly-network-mobile-sdk)
- [Unity SDK (WIP)](https://github.com/rally-dfs/rly-network-unity-sdk)
