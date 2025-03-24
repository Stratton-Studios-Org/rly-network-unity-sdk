# Wallet Creation and Management

This guide covers how to create, manage, and use blockchain wallets within your Unity application using the Rally Protocol Unity SDK.

## Overview

The Rally Protocol Unity SDK provides a comprehensive set of tools for creating and managing blockchain wallets. These wallets are used to interact with the blockchain, store assets, and perform transactions.

## Creating a Wallet

### Basic Wallet Creation

Creating a wallet is a straightforward process with the Rally Protocol Unity SDK. Here's a basic example:

```csharp
using RallyProtocol.Unity;
using System.Threading.Tasks;

public class WalletManager : MonoBehaviour
{
    private IRallyNetwork _rallyNetwork;
    private IRallyAccountManager _accountManager;

    private async void Start()
    {
        // Get references to the network and account manager
        _rallyNetwork = RallyUnityManager.Instance.RlyNetwork;
        _accountManager = _rallyNetwork.AccountManager;

        // Create a new account
        await CreateWalletAsync();
    }

    public async Task CreateWalletAsync()
    {
        try
        {
            // Create a new account with default settings
            var result = await _accountManager.CreateAccountAsync(new AccountCreationOptions
            {
                Overwrite = false,
                StorageOptions = new AccountStorageOptions
                {
                    SaveToCloud = true,
                    RejectOnCloudSaveFailure = false
                }
            });

            Debug.Log($"Wallet created successfully! Address: {result.Address}");
            
            // You can now use this wallet for transactions
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to create wallet: {ex.Message}");
        }
    }
}
```

### Creation Options

When creating a wallet, you can specify several options:

- `Overwrite`: If set to `true`, it will overwrite any existing account with the same identifier. Default is `false`.
- `StorageOptions`: Controls how the account is stored:
  - `SaveToCloud`: If set to `true`, attempts to save the account to cloud storage.
  - `RejectOnCloudSaveFailure`: If set to `true`, the account creation will fail if it cannot be saved to the cloud.

## Accessing an Existing Wallet

Once a wallet has been created, you can access it using the `IRallyAccountManager`:

```csharp
public async Task<string> GetWalletAddressAsync()
{
    try
    {
        var account = await _accountManager.GetAccountAsync();
        return account.Address;
    }
    catch (Exception ex)
    {
        Debug.LogError($"Failed to get wallet address: {ex.Message}");
        return null;
    }
}
```

## Wallet Management

### Checking if a Wallet Exists

Before attempting to create or access a wallet, you might want to check if a wallet already exists:

```csharp
public async Task<bool> DoesWalletExistAsync()
{
    try
    {
        return await _accountManager.HasAccountAsync();
    }
    catch (Exception ex)
    {
        Debug.LogError($"Error checking wallet existence: {ex.Message}");
        return false;
    }
}
```

### Removing a Wallet

In some cases, you might need to remove a wallet:

```csharp
public async Task RemoveWalletAsync()
{
    try
    {
        await _accountManager.RemoveAccountAsync();
        Debug.Log("Wallet removed successfully");
    }
    catch (Exception ex)
    {
        Debug.LogError($"Failed to remove wallet: {ex.Message}");
    }
}
```

## Best Practices

- **Storage Security**: Always handle wallet keys securely. Consider using device-specific secure storage.
- **Error Handling**: Always wrap wallet operations in try-catch blocks to handle errors gracefully.
- **User Confirmation**: Before performing important wallet operations (like deletion), always confirm with the user.
- **Backup Reminders**: Remind users to backup their wallet information, especially recovery phrases.
- **Testing**: Test wallet functionality extensively on different platforms to ensure consistent behavior.

## Platform-Specific Considerations

### iOS

- On iOS, wallet information is stored in the device's Keychain for enhanced security.
- Ensure your app has the appropriate entitlements for Keychain access.

### Android

- On Android, wallet information is stored in the Android Keystore system.
- Ensure your app has the appropriate permissions for secure storage.

## Troubleshooting

### Common Issues

- **"Failed to Create Wallet"**: This could indicate issues with storage permissions or corrupted user data. Try clearing application data or checking permissions.
- **"Wallet Not Found"**: The wallet might have been deleted or never created. Check if the wallet exists before attempting to access it.
- **"Cloud Storage Failure"**: Issues with cloud connectivity or permissions. Check the device's internet connection and cloud service status.

## Next Steps

After creating a wallet, you'll typically want to:

1. [Check the wallet's balance](./balance-checking.md)
2. [Claim RLY tokens](./token-claiming.md)
3. [Perform token transfers](./token-transfers.md)

## Related Documentation

- [Private Key Security](./key-security.md)
- [Cloud Backup and Sync](./cloud-backup.md)
- [Secure Storage](./secure-storage.md)
