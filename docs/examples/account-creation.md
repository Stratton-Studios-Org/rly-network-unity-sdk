# Account Creation Example

This example demonstrates how to create and manage user accounts with the Rally Protocol Unity SDK.

## Prerequisites

Before using this example, ensure you have:

1. Installed the Rally Protocol Unity SDK
2. Configured your API key and network settings

## Basic Account Creation

To create a new account for a user:

```csharp
using UnityEngine;
using System;
using System.Threading.Tasks;
using RallyProtocolUnity.Runtime.Core;

public class AccountExample : MonoBehaviour
{
    private IRallyNetwork rlyNetwork;
    private IRallyAccountManager accountManager;

    void Start()
    {
        // Get the Rally Network instance
        this.rlyNetwork = RallyUnityManager.Instance.RlyNetwork;
        
        // Get the account manager
        this.accountManager = this.rlyNetwork.AccountManager;
    }

    // Call this method to create a new account
    public async void CreateNewAccount()
    {
        try
        {
            Debug.Log("Creating new account...");
            
            // Create a new account with default options
            await this.accountManager.CreateAccountAsync();
            
            Debug.Log("Account created successfully!");
            
            // Get the account information
            var account = await this.accountManager.GetAccountAsync();
            Debug.Log($"Account address: {account.Address}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to create account: {ex.Message}");
        }
    }
}
```

## Advanced Account Creation

For more control over the account creation process, you can use additional options:

```csharp
public async void CreateAdvancedAccount()
{
    try
    {
        Debug.Log("Creating new account with advanced options...");
        
        // Create account with specific options
        await this.accountManager.CreateAccountAsync(new()
        {
            // Set to true to replace an existing account
            Overwrite = false,
            
            // Configure storage options
            StorageOptions = new()
            {
                // Enable cloud storage backup
                SaveToCloud = true,
                
                // If true, account creation will fail if cloud save fails
                RejectOnCloudSaveFailure = false
            }
        });
        
        Debug.Log("Account created successfully!");
    }
    catch (Exception ex)
    {
        Debug.LogError($"Failed to create account: {ex.Message}");
    }
}
```

## Checking for Existing Accounts

Before creating a new account, you might want to check if the user already has one:

```csharp
public async void CheckAndCreateAccount()
{
    try
    {
        // Try to get the existing account
        var existingAccount = await this.accountManager.GetAccountAsync();
        
        if (existingAccount != null)
        {
            Debug.Log($"Found existing account: {existingAccount.Address}");
            // Use the existing account
        }
        else
        {
            Debug.Log("No existing account found. Creating a new one...");
            
            // Create a new account
            await this.accountManager.CreateAccountAsync();
            
            Debug.Log("Account created successfully!");
        }
    }
    catch (Exception ex)
    {
        Debug.LogError($"Error checking/creating account: {ex.Message}");
    }
}
```

## Getting Account Information

To retrieve information about the current account:

```csharp
public async void DisplayAccountInfo()
{
    try
    {
        var account = await this.accountManager.GetAccountAsync();
        
        if (account != null)
        {
            Debug.Log($"Account Address: {account.Address}");
            
            // Get the RLY token balance
            decimal balance = await this.rlyNetwork.GetBalanceAsync();
            Debug.Log($"RLY Balance: {balance}");
        }
        else
        {
            Debug.Log("No account found");
        }
    }
    catch (Exception ex)
    {
        Debug.LogError($"Error getting account info: {ex.Message}");
    }
}
```

## Complete Example

Here's a complete example showing account management in a Unity component:

```csharp
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Threading.Tasks;
using RallyProtocolUnity.Runtime.Core;

public class AccountManager : MonoBehaviour
{
    [SerializeField] private Button createAccountButton;
    [SerializeField] private Button checkBalanceButton;
    [SerializeField] private Text statusText;
    [SerializeField] private Text addressText;
    [SerializeField] private Text balanceText;
    
    private IRallyNetwork rlyNetwork;
    private IRallyAccountManager accountManager;
    
    void Start()
    {
        // Initialize
        this.rlyNetwork = RallyUnityManager.Instance.RlyNetwork;
        this.accountManager = this.rlyNetwork.AccountManager;
        
        // Set up button listeners
        createAccountButton.onClick.AddListener(CreateAccount);
        checkBalanceButton.onClick.AddListener(CheckBalance);
        
        // Initial check for existing account
        CheckForExistingAccount();
    }
    
    private async void CheckForExistingAccount()
    {
        try
        {
            var account = await this.accountManager.GetAccountAsync();
            
            if (account != null)
            {
                statusText.text = "Account Loaded";
                addressText.text = $"Address: {account.Address}";
                await CheckBalance();
            }
            else
            {
                statusText.text = "No Account Found";
                addressText.text = "Address: None";
                balanceText.text = "Balance: 0";
            }
        }
        catch (Exception ex)
        {
            statusText.text = "Error Loading Account";
            Debug.LogError($"Error: {ex.Message}");
        }
    }
    
    private async void CreateAccount()
    {
        try
        {
            statusText.text = "Creating Account...";
            
            await this.accountManager.CreateAccountAsync(new()
            {
                Overwrite = false,
                StorageOptions = new()
                {
                    SaveToCloud = true,
                    RejectOnCloudSaveFailure = false
                }
            });
            
            var account = await this.accountManager.GetAccountAsync();
            statusText.text = "Account Created";
            addressText.text = $"Address: {account.Address}";
            
            await CheckBalance();
        }
        catch (Exception ex)
        {
            statusText.text = "Error Creating Account";
            Debug.LogError($"Error: {ex.Message}");
        }
    }
    
    private async Task CheckBalance()
    {
        try
        {
            decimal balance = await this.rlyNetwork.GetBalanceAsync();
            balanceText.text = $"Balance: {balance} RLY";
            return;
        }
        catch (Exception ex)
        {
            balanceText.text = "Balance: Error";
            Debug.LogError($"Error checking balance: {ex.Message}");
        }
    }
}
```

## Best Practices

1. **Error Handling**: Always wrap account operations in try-catch blocks to handle exceptions gracefully
2. **Check Existing Accounts**: Before creating a new account, check if one already exists
3. **Secure Storage**: Use cloud storage options when appropriate for key backup
4. **UI Feedback**: Provide clear feedback to users during account operations
5. **Async Operations**: Remember that all account operations are asynchronous and should be handled appropriately
