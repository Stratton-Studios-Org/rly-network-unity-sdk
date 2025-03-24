# IRallyAccountManager

The `IRallyAccountManager` interface provides methods for managing blockchain accounts in the Rally Protocol Unity SDK. It handles account creation, recovery, storage, and key management.

## Interface Definition

```csharp
public interface IRallyAccountManager
{
    Task<Account> CreateAccountAsync(CreateAccountOptions? options = null);
    Task<Account> ImportExistingAccountAsync(string mnemonic, CreateAccountOptions options);
    Task<bool> IsWalletBackedUpToCloudAsync();
    Task<Account?> GetAccountAsync();
    Task<string?> GetPublicAddressAsync();
    Task PermanentlyDeleteAccountAsync();
    Task<string?> GetAccountPhraseAsync();
    Task<string> SignMessageAsync(string message);
    Task<string> SignTransactionAsync<T>(T transaction) where T : SignedTypeTransaction;
}
```

## Properties

None (interface only exposes methods).

## Methods

### CreateAccountAsync

Creates a new wallet and saves it to the device based on the storage options provided.

```csharp
Task<Account> CreateAccountAsync(CreateAccountOptions? options = null);
```

**Parameters:**

- `options` (optional): Configuration options for account creation, including storage preferences.

**Returns:**

- `Task<Account>`: A task that resolves to the newly created account.

**Throws:**

- `RallyAccountExistsException`: If an account already exists and the `Overwrite` flag is not set to true.

**Example:**

```csharp
IRallyAccountManager accountManager = rlyNetwork.AccountManager;

// Create account with default options
Account account = await accountManager.CreateAccountAsync();

// Create account with custom options
Account account = await accountManager.CreateAccountAsync(new CreateAccountOptions
{
    Overwrite = false,
    StorageOptions = new KeyStorageConfig
    {
        SaveToCloud = true,
        RejectOnCloudSaveFailure = false
    }
});
```

### ImportExistingAccountAsync

Imports an existing mnemonic phrase and creates a wallet from that.

```csharp
Task<Account> ImportExistingAccountAsync(string mnemonic, CreateAccountOptions options);
```

**Parameters:**

- `mnemonic`: The mnemonic phrase to import and create account from.
- `options`: Configuration options for the imported account.

**Returns:**

- `Task<Account>`: A task that resolves to the newly imported account.

**Throws:**

- `RallyAccountExistsException`: If an account already exists and the `Overwrite` flag is not set to true.

**Example:**

```csharp
IRallyAccountManager accountManager = rlyNetwork.AccountManager;
string mnemonic = "word1 word2 word3 word4 word5 word6 word7 word8 word9 word10 word11 word12";

Account account = await accountManager.ImportExistingAccountAsync(mnemonic, new CreateAccountOptions
{
    Overwrite = true,
    StorageOptions = new KeyStorageConfig
    {
        SaveToCloud = true
    }
});
```

### IsWalletBackedUpToCloudAsync

Returns the cloud backup status of the existing wallet.

```csharp
Task<bool> IsWalletBackedUpToCloudAsync();
```

**Returns:**

- `Task<bool>`: A task that resolves to a boolean indicating the cloud backup status. Returns `true` if the wallet is backed up to cloud, `false` if not backed up or no wallet exists.

**Important Note:**
This method returns `false` if there is currently no wallet. It should not be used as a check for wallet existence as it will return `false` both if there is no wallet or if the wallet exists but is not backed up to cloud.

**Example:**

```csharp
IRallyAccountManager accountManager = rlyNetwork.AccountManager;
bool isBackedUp = await accountManager.IsWalletBackedUpToCloudAsync();

if (isBackedUp)
{
    Debug.Log("Wallet is backed up to cloud");
}
else
{
    Debug.Log("Wallet is not backed up to cloud or no wallet exists");
}
```

### GetAccountAsync

Checks if there is any account currently loaded. If not, checks if there is a mnemonic stored and creates an account from it.

```csharp
Task<Account?> GetAccountAsync();
```

**Returns:**

- `Task<Account?>`: A task that resolves to the current account if one exists, or a newly created account from stored mnemonic. Returns `null` if no account or mnemonic exists.

**Example:**

```csharp
IRallyAccountManager accountManager = rlyNetwork.AccountManager;
Account? account = await accountManager.GetAccountAsync();

if (account != null)
{
    Debug.Log($"Account address: {account.Address}");
}
else
{
    Debug.Log("No account exists");
}
```

### GetPublicAddressAsync

Gets the public address of the current account.

```csharp
Task<string?> GetPublicAddressAsync();
```

**Returns:**

- `Task<string?>`: A task that resolves to the public address of the current account. Returns `null` if no account exists.

**Example:**

```csharp
IRallyAccountManager accountManager = rlyNetwork.AccountManager;
string? address = await accountManager.GetPublicAddressAsync();

if (address != null)
{
    Debug.Log($"Account address: {address}");
}
else
{
    Debug.Log("No account exists");
}
```

### PermanentlyDeleteAccountAsync

Permanently deletes the account and all associated data.

```csharp
Task PermanentlyDeleteAccountAsync();
```

**Returns:**

- `Task`: A task that completes when the account has been deleted.

**Example:**

```csharp
IRallyAccountManager accountManager = rlyNetwork.AccountManager;
await accountManager.PermanentlyDeleteAccountAsync();
Debug.Log("Account has been permanently deleted");
```

### GetAccountPhraseAsync

Gets the mnemonic phrase for the current account.

```csharp
Task<string?> GetAccountPhraseAsync();
```

**Returns:**

- `Task<string?>`: A task that resolves to the mnemonic phrase for the current account. Returns `null` if no account exists or if an error occurs while retrieving the phrase.

**Security Warning:**
The mnemonic phrase gives full control of the account. It should be treated as sensitive information and never shared with untrusted parties.

**Example:**

```csharp
IRallyAccountManager accountManager = rlyNetwork.AccountManager;
string? mnemonic = await accountManager.GetAccountPhraseAsync();

if (mnemonic != null)
{
    // Display securely to user for backup
    Debug.Log("Mnemonic phrase retrieved successfully");
}
else
{
    Debug.Log("No account exists or failed to retrieve mnemonic");
}
```

### SignMessageAsync

Signs a message using the current account's private key.

```csharp
Task<string> SignMessageAsync(string message);
```

**Parameters:**

- `message`: The message to sign.

**Returns:**

- `Task<string>`: A task that resolves to the signature of the message.

**Throws:**

- `RallyNoAccountException`: If no account exists.

**Example:**

```csharp
IRallyAccountManager accountManager = rlyNetwork.AccountManager;
try
{
    string message = "Hello, World!";
    string signature = await accountManager.SignMessageAsync(message);
    Debug.Log($"Signature: {signature}");
}
catch (RallyNoAccountException)
{
    Debug.LogError("No account exists to sign the message");
}
```

### SignTransactionAsync

Signs a transaction using the current account's private key.

```csharp
Task<string> SignTransactionAsync<T>(T transaction) where T : SignedTypeTransaction;
```

**Parameters:**

- `transaction`: The transaction to sign.

**Returns:**

- `Task<string>`: A task that resolves to the signature of the transaction.

**Throws:**

- `RallyNoAccountException`: If no account exists.

**Example:**

```csharp
IRallyAccountManager accountManager = rlyNetwork.AccountManager;
try
{
    // Create a transaction (example)
    var transaction = new LegacyTransactionChainId
    {
        To = "0x1234567890123456789012345678901234567890",
        Value = new HexBigInteger(Web3.Convert.ToWei(1, UnitConversion.EthUnit.Ether)),
        GasPrice = new HexBigInteger(Web3.Convert.ToWei(20, UnitConversion.EthUnit.Gwei)),
        Gas = new HexBigInteger(21000),
        Nonce = new HexBigInteger(0),
        ChainId = new HexBigInteger(1) // Mainnet
    };
    
    string signature = await accountManager.SignTransactionAsync(transaction);
    Debug.Log($"Transaction signature: {signature}");
}
catch (RallyNoAccountException)
{
    Debug.LogError("No account exists to sign the transaction");
}
```

## Supporting Types

### CreateAccountOptions

```csharp
[System.Serializable]
public class CreateAccountOptions
{
    /// <summary>
    /// Whether to overwrite the existing account if there are any.
    /// </summary>
    public virtual bool? Overwrite { get; set; }

    /// <summary>
    /// The key storage options
    /// </summary>
    public virtual KeyStorageConfig StorageOptions { get; set; }
}
```

The `CreateAccountOptions` class is used to configure how an account is created or imported.

**Properties:**

- `Overwrite`: If set to `true`, any existing account will be overwritten. If `false`, the operation will fail if an account already exists.
- `StorageOptions`: Configuration for how the wallet key should be stored, including cloud backup options.

### KeyStorageConfig

```csharp
[System.Serializable]
public class KeyStorageConfig
{
    /// <summary>
    /// Gets or sets whether to save the mnemonic in a way that is eligible for device OS cloud storage. 
    /// If set to false, the mnemonic will only be stored on device.
    /// </summary>
    public virtual bool? SaveToCloud { get; set; }

    /// <summary>
    /// Gets or sets whether to raise an error if saving to cloud fails. 
    /// If set to false, the mnemonic will silently fall back to local on device only storage.
    /// </summary>
    public virtual bool? RejectOnCloudSaveFailure { get; set; }
}
```

The `KeyStorageConfig` class provides options for storing wallet keys, including cloud backup settings.

**Properties:**

- `SaveToCloud`: If set to `true`, the wallet keys will be eligible for the device's cloud storage mechanism (iCloud Keychain on iOS, Blockstore on Android). If `false`, keys are only stored locally.
- `RejectOnCloudSaveFailure`: If set to `true`, an error will be raised if cloud storage fails. If `false`, the system will silently fall back to local storage.

## Best Practices

1. **Security First**: Always handle mnemonics and private keys securely. Never log them or expose them to users unless explicitly for backup purposes.

2. **Cloud Backup**: When using cloud storage for account backup:
   - Ensure users understand what data is being stored in the cloud
   - Be aware that moving from local to cloud storage may cause issues if different wallets exist on different devices
   - Remember that cloud backup depends on user settings (logged into iCloud/Google Play, having device passcodes set, etc.)

3. **Overwrite Protection**: Set `Overwrite = false` when creating new accounts to prevent accidentally overwriting existing accounts.

4. **Error Handling**: Always implement proper error handling around account operations, especially when dealing with cloud storage.

## Common Issues

- **Cloud Storage Failures**: Cloud storage can fail if the user is not logged into the appropriate platform account (Apple ID, Google Account) or if device security requirements are not met.

- **Wallet Recovery Issues**: If a wallet cannot be recovered, check if the mnemonic phrase is correct and complete, and verify that the device has the necessary permissions.

- **Multiple Device Consistency**: Be cautious when using cloud storage across multiple devices, as it may lead to unexpected behavior if different wallets exist on different devices.

## See Also

- [IRallyNetwork](./IRallyNetwork.md) - The main network interface
- [Token Management](./token-management.md) - How to manage tokens
- [Wallet Guide](../guides/wallet-management.md) - Guide to wallet management
