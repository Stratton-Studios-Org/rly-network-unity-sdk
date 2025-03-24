# Storage Options

This document covers the storage options available in the Rally Protocol Unity SDK for securely storing account keys and other sensitive information.

## Overview

The Rally Protocol Unity SDK provides mechanisms for securely storing and retrieving blockchain account keys across various platforms. These storage options are designed to balance security, ease of use, and recoverability.

## KeyStorageConfig

The primary configuration class for key storage is `KeyStorageConfig`:

```csharp
namespace RallyProtocol.Accounts
{
    public class KeyStorageConfig
    {
        public bool SaveToCloud { get; set; } = true;
        public bool RejectOnCloudSaveFailure { get; set; } = true;
    }
}
```

### Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `SaveToCloud` | `bool` | `true` | Whether to attempt saving keys to platform-specific secure cloud storage. |
| `RejectOnCloudSaveFailure` | `bool` | `true` | Whether to fail account creation if cloud storage backup fails. |

## Storage Mechanisms

The SDK uses different storage mechanisms depending on the platform:

### iOS

On iOS, the SDK uses:

- **Local Storage**: iOS Keychain for secure local storage
- **Cloud Storage**: iCloud Keychain for secure cloud-synced storage (when `SaveToCloud` is `true`)

### Android

On Android, the SDK uses:

- **Local Storage**: Android Keystore for secure local storage
- **Cloud Storage**: AndroidX Security library with DataStore for encrypted storage with optional Google Account-based backup

### Other Platforms (PC, Mac, etc.)

On desktop and other platforms:

- **Local Storage**: Encrypted files in the application's persistent data path
- **Cloud Storage**: Not available by default on these platforms

## Using Storage Options

### During Account Creation

```csharp
// Create an account with default storage options (cloud backup enabled)
Account account = await rlyNetwork.AccountManager.CreateAccountAsync();

// Create an account with custom storage options
Account account = await rlyNetwork.AccountManager.CreateAccountAsync(new CreateAccountOptions
{
    Overwrite = false,
    StorageOptions = new KeyStorageConfig
    {
        SaveToCloud = true,
        RejectOnCloudSaveFailure = false
    }
});
```

### During Account Import

```csharp
// Import an account with custom storage options
string mnemonic = "word1 word2 word3 word4 word5 word6 word7 word8 word9 word10 word11 word12";
Account account = await rlyNetwork.AccountManager.ImportExistingAccountAsync(
    mnemonic,
    new CreateAccountOptions
    {
        Overwrite = true,
        StorageOptions = new KeyStorageConfig
        {
            SaveToCloud = true,
            RejectOnCloudSaveFailure = false
        }
    }
);
```

### Checking Cloud Backup Status

```csharp
// Check if the wallet is backed up to cloud
bool isBackedUp = await rlyNetwork.AccountManager.IsWalletBackedUpToCloudAsync();

if (isBackedUp)
{
    Debug.Log("Wallet is securely backed up to cloud");
}
else
{
    Debug.Log("Wallet is only stored locally - recommend backing up recovery phrase");
}
```

## Platform-Specific Considerations

### iOS

On iOS, cloud backup requires:

- iCloud to be enabled on the device
- The application to have the `iCloud` entitlement in its capabilities
- The user to be signed in to an Apple ID

If any of these conditions are not met and `RejectOnCloudSaveFailure` is `true`, account creation will fail. If `RejectOnCloudSaveFailure` is `false`, the account will be created but only stored locally.

### Android

On Android, cloud backup requires:

- Google Play Services to be available on the device
- The user to be signed in to a Google Account
- Required permissions to be granted

Similar to iOS, if these conditions are not met, the behavior depends on the `RejectOnCloudSaveFailure` flag.

### WebGL

In WebGL builds, storage options are limited to browser-based mechanisms:

- `SaveToCloud` is ignored in WebGL builds
- Keys are stored in IndexedDB with encryption
- Users should be prompted to back up their recovery phrase manually

## Best Practices

### Security Recommendations

1. **Cloud Backup**: For consumer applications, enabling cloud backup (`SaveToCloud = true`) provides the best user experience by preventing key loss.

2. **Flexible Error Handling**: For wider platform compatibility, set `RejectOnCloudSaveFailure = false` to gracefully handle cases where cloud backup isn't available.

3. **Recovery Phrase Backup**: Always provide a way for users to view and back up their recovery phrase, regardless of cloud backup status:

```csharp
async void PromptUserForBackup()
{
    bool isCloudBackedUp = await rlyNetwork.AccountManager.IsWalletBackedUpToCloudAsync();
    
    if (!isCloudBackedUp)
    {
        // Show UI to get and display mnemonic
        string? mnemonic = await rlyNetwork.AccountManager.GetAccountPhraseAsync();
        if (mnemonic != null)
        {
            DisplayBackupUI(mnemonic);
        }
    }
}
```

### User Experience Guidelines

1. **Explain Storage Options**: Clearly communicate to users how their keys are being stored and the implications for recovery.

2. **Provide Multiple Backup Methods**: Offer both automatic (cloud) and manual (mnemonic phrase) backup methods.

3. **Verify Recovery**: Consider implementing a recovery verification flow where users confirm they've backed up their recovery phrase by entering a portion of it.

## Implementation Patterns

### Multi-Factor Recovery

For sensitive applications, implement a multi-factor recovery system:

```csharp
async Task CreateSecureAccountAsync()
{
    // Create account with cloud backup
    Account account = await rlyNetwork.AccountManager.CreateAccountAsync(new CreateAccountOptions
    {
        StorageOptions = new KeyStorageConfig
        {
            SaveToCloud = true,
            RejectOnCloudSaveFailure = false
        }
    });
    
    // Get recovery phrase for manual backup
    string? mnemonic = await rlyNetwork.AccountManager.GetAccountPhraseAsync();
    if (mnemonic != null)
    {
        // Show backup UI and verify user has backed up phrase
        bool userConfirmedBackup = await ShowBackupUIAndVerify(mnemonic);
        
        if (userConfirmedBackup)
        {
            // Store a flag indicating user has completed backup
            PlayerPrefs.SetInt("WalletBackupComplete", 1);
            PlayerPrefs.Save();
        }
    }
}
```

### Platform-Specific Configuration

```csharp
KeyStorageConfig GetOptimalStorageConfig()
{
    KeyStorageConfig config = new KeyStorageConfig();
    
    #if UNITY_IOS || UNITY_ANDROID
        // On mobile, prefer cloud backup but don't fail if it's not available
        config.SaveToCloud = true;
        config.RejectOnCloudSaveFailure = false;
    #elif UNITY_WEBGL
        // WebGL doesn't support cloud backup
        config.SaveToCloud = false;
    #else
        // For desktop platforms, default to local only
        config.SaveToCloud = false;
    #endif
    
    return config;
}
```

## Related Documentation

- [AccountOptions](./AccountOptions.md): Additional options for account creation and management
- [IRallyAccountManager](./IRallyAccountManager.md): Interface for account management
- [Security Best Practices](./security-best-practices.md): General security recommendations
