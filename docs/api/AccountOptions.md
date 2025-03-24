# AccountOptions

This document describes the options available for account creation and management in the Rally Protocol Unity SDK.

## CreateAccountOptions

The `CreateAccountOptions` class provides configuration options when creating or importing an account.

### Class Definition

```csharp
namespace RallyProtocol.Accounts
{
    public class CreateAccountOptions
    {
        public bool Overwrite { get; set; }
        public KeyStorageConfig StorageOptions { get; set; }
    }
}
```

### Properties

| Property | Type | Description |
|----------|------|-------------|
| `Overwrite` | `bool` | When `true`, allows overwriting an existing account. When `false` (default), creation will fail if an account already exists. |
| `StorageOptions` | `KeyStorageConfig` | Configuration for how the account's keys should be stored. |

### Example Usage

```csharp
// Create an account with default options
Account account = await rlyNetwork.AccountManager.CreateAccountAsync();

// Create an account with custom options
Account account = await rlyNetwork.AccountManager.CreateAccountAsync(new CreateAccountOptions
{
    Overwrite = true, // Will overwrite existing account if one exists
    StorageOptions = new KeyStorageConfig
    {
        SaveToCloud = true,
        RejectOnCloudSaveFailure = false
    }
});

// Import an existing account with specified options
string mnemonic = "word1 word2 word3 word4 word5 word6 word7 word8 word9 word10 word11 word12";
Account account = await rlyNetwork.AccountManager.ImportExistingAccountAsync(
    mnemonic,
    new CreateAccountOptions
    {
        Overwrite = true,
        StorageOptions = new KeyStorageConfig
        {
            SaveToCloud = true
        }
    }
);
```

## KeyStorageConfig

The `KeyStorageConfig` class defines how account keys should be stored and managed.

### Class Definition

```csharp
namespace RallyProtocol.Accounts
{
    public class KeyStorageConfig
    {
        public bool SaveToCloud { get; set; }
        public bool RejectOnCloudSaveFailure { get; set; }
    }
}
```

### Properties

| Property | Type | Description |
|----------|------|-------------|
| `SaveToCloud` | `bool` | When `true`, attempts to save the account keys to cloud storage for backup. Default: `true`. |
| `RejectOnCloudSaveFailure` | `bool` | When `true`, the account creation will fail if cloud backup fails. When `false`, account creation will succeed even if cloud backup fails. Default: `true`. |

### Example Usage

```csharp
// Configuration that attempts cloud backup but doesn't fail if it's unavailable
KeyStorageConfig flexibleStorageConfig = new KeyStorageConfig
{
    SaveToCloud = true,
    RejectOnCloudSaveFailure = false
};

// Configuration that requires successful cloud backup
KeyStorageConfig strictStorageConfig = new KeyStorageConfig
{
    SaveToCloud = true,
    RejectOnCloudSaveFailure = true
};

// Configuration that only uses local storage
KeyStorageConfig localOnlyConfig = new KeyStorageConfig
{
    SaveToCloud = false
};
```

## Cloud Storage Behavior

When `SaveToCloud` is enabled, the SDK attempts to save account keys to the platform's cloud storage service:

- **iOS**: iCloud Keychain
- **Android**: Android Keystore with optional cloud backup
- **Other platforms**: Platform-specific secure storage mechanisms

The cloud backup process happens automatically during account creation or import. You can check if an account was successfully backed up to the cloud using the `IsWalletBackedUpToCloudAsync` method:

```csharp
bool isBackedUp = await rlyNetwork.AccountManager.IsWalletBackedUpToCloudAsync();
if (isBackedUp)
{
    Debug.Log("Account is safely backed up to cloud");
}
else
{
    Debug.Log("Account is only stored locally - recommend user to back up phrase");
}
```

## Best Practices

### Security Considerations

1. **Default to cloud backup**: For most consumer applications, enabling cloud backup (`SaveToCloud = true`) provides the best user experience by preventing key loss.

2. **Handle cloud backup failures gracefully**: Consider setting `RejectOnCloudSaveFailure = false` and providing alternative backup options to users when cloud backup isn't available.

3. **Offer mnemonic phrase backup**: Even with cloud backup enabled, provide users with the option to back up their mnemonic phrase:

```csharp
string? mnemonic = await rlyNetwork.AccountManager.GetAccountPhraseAsync();
if (mnemonic != null)
{
    // Display mnemonic to user for backup
    DisplayMnemonicToUser(mnemonic);
}
```

### Implementation Patterns

#### Sequential Backup Strategy

```csharp
async Task CreateAccountWithBackupAsync()
{
    try
    {
        // Try to create with cloud backup first
        Account account = await rlyNetwork.AccountManager.CreateAccountAsync(new CreateAccountOptions
        {
            StorageOptions = new KeyStorageConfig
            {
                SaveToCloud = true,
                RejectOnCloudSaveFailure = false
            }
        });
        
        // Check if cloud backup succeeded
        bool isBackedUpToCloud = await rlyNetwork.AccountManager.IsWalletBackedUpToCloudAsync();
        
        if (!isBackedUpToCloud)
        {
            // Cloud backup failed, offer mnemonic phrase backup
            string? mnemonic = await rlyNetwork.AccountManager.GetAccountPhraseAsync();
            if (mnemonic != null)
            {
                // Show backup UI
                ShowBackupUI(mnemonic);
            }
        }
    }
    catch (Exception ex)
    {
        Debug.LogError($"Failed to create account: {ex.Message}");
    }
}
```

#### Environment-Based Configuration

```csharp
CreateAccountOptions GetAccountOptions()
{
    // Different options based on platform or environment
    #if UNITY_IOS || UNITY_ANDROID
        return new CreateAccountOptions
        {
            StorageOptions = new KeyStorageConfig
            {
                SaveToCloud = true,
                RejectOnCloudSaveFailure = false
            }
        };
    #else
        return new CreateAccountOptions
        {
            StorageOptions = new KeyStorageConfig
            {
                SaveToCloud = false
            }
        };
    #endif
}
```

## Related Documentation

- [IRallyAccountManager](./IRallyAccountManager.md): Interface for account management
- [StorageOptions](./StorageOptions.md): Details on storage options and behavior
- [RallyAccountException](./RallyAccountException.md): Exception thrown during account operations
