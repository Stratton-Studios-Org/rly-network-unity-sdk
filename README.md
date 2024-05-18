# Rally Protocol Unity SDK

The Rally Protocol Unity SDK, learn more at https://docs.rallyprotocol.com/

## Getting Started

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

## Resources

- [Flutter SDK](https://github.com/rally-dfs/flutter-sdk)
- [Mobile SDK](https://github.com/rally-dfs/rly-network-mobile-sdk)
- [Unity SDK (WIP)](https://github.com/rally-dfs/rly-network-unity-sdk)
