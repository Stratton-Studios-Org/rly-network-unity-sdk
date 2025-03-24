# RallyAccountException

The `RallyAccountException` class represents exceptions specific to account operations in the Rally Protocol Unity SDK.

## Class Definition

```csharp
using System;

namespace RallyProtocol.Exceptions
{
    public class RallyAccountException : RallyException
    {
        public RallyAccountException() : base() { }
        
        public RallyAccountException(string message) : base(message) { }
        
        public RallyAccountException(string message, Exception innerException) 
            : base(message, innerException) { }
    }
}
```

## Overview

The `RallyAccountException` is thrown when errors occur during account-related operations, such as:

- Account creation
- Account recovery
- Key management
- Mnemonic phrase operations
- Cloud backup issues
- Signature operations

## Common Error Scenarios

### No Account Exists

Thrown when attempting to perform operations that require an existing account, but no account is found.

```csharp
try
{
    string? address = await rlyNetwork.AccountManager.GetPublicAddressAsync();
    
    if (address == null)
    {
        // Handle null address (no account)
    }
}
catch (RallyAccountException ex)
{
    if (ex.Message.Contains("no account exists"))
    {
        Debug.LogWarning("No account found. Creating a new account...");
        await rlyNetwork.AccountManager.CreateAccountAsync();
    }
    else
    {
        Debug.LogError($"Account error: {ex.Message}");
    }
}
```

### Account Already Exists

Thrown when attempting to create an account when one already exists, without setting the `Overwrite` flag.

```csharp
try
{
    // Try to create a new account
    await rlyNetwork.AccountManager.CreateAccountAsync();
}
catch (RallyAccountException ex)
{
    if (ex.Message.Contains("already exists"))
    {
        Debug.LogWarning("An account already exists. Would you like to overwrite it?");
        
        // If user confirms overwrite
        if (ConfirmOverwrite())
        {
            await rlyNetwork.AccountManager.CreateAccountAsync(new CreateAccountOptions
            {
                Overwrite = true
            });
        }
    }
    else
    {
        Debug.LogError($"Account error: {ex.Message}");
    }
}
```

### Cloud Backup Failure

Thrown when cloud backup fails and `RejectOnCloudSaveFailure` is set to `true`.

```csharp
try
{
    // Try to create an account with cloud backup
    await rlyNetwork.AccountManager.CreateAccountAsync(new CreateAccountOptions
    {
        StorageOptions = new KeyStorageConfig
        {
            SaveToCloud = true,
            RejectOnCloudSaveFailure = true
        }
    });
}
catch (RallyAccountException ex)
{
    if (ex.Message.Contains("cloud backup") || ex.Message.Contains("cloud save"))
    {
        Debug.LogWarning("Could not back up to cloud. Would you like to continue with local storage only?");
        
        // If user confirms local-only storage
        if (ConfirmLocalOnlyStorage())
        {
            await rlyNetwork.AccountManager.CreateAccountAsync(new CreateAccountOptions
            {
                StorageOptions = new KeyStorageConfig
                {
                    SaveToCloud = true,
                    RejectOnCloudSaveFailure = false
                }
            });
        }
    }
    else
    {
        Debug.LogError($"Account error: {ex.Message}");
    }
}
```

### Invalid Mnemonic

Thrown when attempting to import an account with an invalid mnemonic phrase.

```csharp
try
{
    // Try to import an account with a mnemonic
    string mnemonic = GetUserInputMnemonic();
    await rlyNetwork.AccountManager.ImportExistingAccountAsync(mnemonic, new CreateAccountOptions());
}
catch (RallyAccountException ex)
{
    if (ex.Message.Contains("invalid mnemonic") || ex.Message.Contains("invalid phrase"))
    {
        Debug.LogError("The recovery phrase you entered is invalid. Please check and try again.");
        ShowInvalidMnemonicUI();
    }
    else
    {
        Debug.LogError($"Account error: {ex.Message}");
    }
}
```

### Storage Access Issues

Thrown when there are problems accessing the secure storage.

```csharp
try
{
    // Try to access account
    Account? account = await rlyNetwork.AccountManager.GetAccountAsync();
}
catch (RallyAccountException ex)
{
    if (ex.Message.Contains("access") && ex.Message.Contains("storage"))
    {
        Debug.LogError("Could not access secure storage. This may be due to device restrictions or permissions.");
        ShowStoragePermissionUI();
    }
    else
    {
        Debug.LogError($"Account error: {ex.Message}");
    }
}
```

### Signing Failures

Thrown when signing operations fail.

```csharp
try
{
    // Try to sign a message
    string signature = await rlyNetwork.AccountManager.SignMessageAsync("Hello, Rally!");
}
catch (RallyAccountException ex)
{
    if (ex.Message.Contains("sign") || ex.Message.Contains("signature"))
    {
        Debug.LogError("Failed to sign message. The account may be corrupted or inaccessible.");
        ShowRecoveryOptionsUI();
    }
    else
    {
        Debug.LogError($"Account error: {ex.Message}");
    }
}
```

## Handling RallyAccountException

### Basic Error Handling

```csharp
try
{
    // Account operation
    await rlyNetwork.AccountManager.CreateAccountAsync();
}
catch (RallyAccountException ex)
{
    Debug.LogError($"Account error: {ex.Message}");
    ShowErrorUI("There was a problem with your account.");
}
```

### Advanced Error Handling

For more sophisticated error handling, you can categorize the exceptions based on the message:

```csharp
try
{
    // Account operation
    await rlyNetwork.AccountManager.CreateAccountAsync();
}
catch (RallyAccountException ex)
{
    string message = ex.Message.ToLower();
    
    if (message.Contains("already exists"))
    {
        HandleExistingAccountError();
    }
    else if (message.Contains("cloud") || message.Contains("backup"))
    {
        HandleCloudBackupError(ex);
    }
    else if (message.Contains("storage") || message.Contains("keychain"))
    {
        HandleStorageError(ex);
    }
    else if (message.Contains("mnemonic") || message.Contains("phrase"))
    {
        HandleMnemonicError(ex);
    }
    else if (message.Contains("sign") || message.Contains("signature"))
    {
        HandleSigningError(ex);
    }
    else
    {
        // General account error
        HandleGeneralAccountError(ex);
    }
}

private void HandleExistingAccountError()
{
    Debug.LogWarning("An account already exists.");
    ShowAccountExistsDialog();
}

private void HandleCloudBackupError(RallyAccountException ex)
{
    Debug.LogWarning($"Cloud backup issue: {ex.Message}");
    ShowCloudBackupFailureDialog();
}

// ... other error handlers
```

## Best Practices

### Prevention

1. **Check Account Existence**: Before performing operations that require an existing account, check if the account exists.

```csharp
Account? account = await rlyNetwork.AccountManager.GetAccountAsync();
if (account == null)
{
    // Handle case where no account exists
    ShowCreateAccountUI();
    return;
}

// Proceed with operations that require an account
```

2. **Handle Cloud Backup Gracefully**: Set `RejectOnCloudSaveFailure = false` for better user experience.

```csharp
await rlyNetwork.AccountManager.CreateAccountAsync(new CreateAccountOptions
{
    StorageOptions = new KeyStorageConfig
    {
        SaveToCloud = true,
        RejectOnCloudSaveFailure = false
    }
});

// Check if backup succeeded
bool isBackedUp = await rlyNetwork.AccountManager.IsWalletBackedUpToCloudAsync();
if (!isBackedUp)
{
    ShowBackupRecommendationUI();
}
```

3. **Validate Inputs**: Validate mnemonic phrases before attempting to import them.

```csharp
string mnemonic = GetUserInputMnemonic();
if (!IsValidBIP39Mnemonic(mnemonic))
{
    ShowInvalidMnemonicUI("The recovery phrase you entered is invalid. Please check and try again.");
    return;
}

// Proceed with import
await rlyNetwork.AccountManager.ImportExistingAccountAsync(mnemonic, options);
```

### Recovery

1. **Offer Account Reset**: When encountering persistent account issues, offer the option to reset.

```csharp
async void ResetAccount()
{
    try
    {
        await rlyNetwork.AccountManager.PermanentlyDeleteAccountAsync();
        Debug.Log("Account successfully deleted.");
        
        // Create a new account
        await rlyNetwork.AccountManager.CreateAccountAsync();
        Debug.Log("New account created successfully.");
    }
    catch (RallyException ex)
    {
        Debug.LogError($"Failed to reset account: {ex.Message}");
    }
}
```

2. **Prompt for Mnemonic Backup**: Always prompt users to back up their mnemonic phrase.

```csharp
async void BackupMnemonic()
{
    try
    {
        string? mnemonic = await rlyNetwork.AccountManager.GetAccountPhraseAsync();
        if (mnemonic != null)
        {
            ShowBackupUI(mnemonic);
        }
    }
    catch (RallyAccountException ex)
    {
        Debug.LogError($"Failed to get mnemonic: {ex.Message}");
    }
}
```

3. **Implement Recovery Flow**: Create a dedicated account recovery flow.

```csharp
async void RecoverAccount(string mnemonic)
{
    try
    {
        await rlyNetwork.AccountManager.ImportExistingAccountAsync(mnemonic, new CreateAccountOptions
        {
            Overwrite = true
        });
        Debug.Log("Account recovered successfully!");
    }
    catch (RallyAccountException ex)
    {
        Debug.LogError($"Failed to recover account: {ex.Message}");
        ShowRecoveryFailureUI();
    }
}
```

## Related Documentation

- [RallyException](./RallyException.md): Base exception class for Rally Protocol
- [IRallyAccountManager](./IRallyAccountManager.md): Interface for account management
- [AccountOptions](./AccountOptions.md): Options for account creation and management
- [StorageOptions](./StorageOptions.md): Options for key storage
