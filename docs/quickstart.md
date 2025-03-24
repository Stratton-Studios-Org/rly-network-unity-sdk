# Rally Protocol Unity SDK - Quick Start Guide

This guide will help you quickly get started with the Rally Protocol Unity SDK. The SDK allows you to integrate blockchain functionality into your Unity games, including RLY token transfers, wallet integration, and token rewards.

## Supported Platforms

The SDK supports the following platforms:

- **iOS**: Fully supported for deployment
- **Android**: Fully supported for deployment
- **Unity Editor**: Supported for development on Windows, macOS, and Linux

> **Important Note**: Standalone builds for Windows, macOS, and Linux are not supported for deployment.

## Prerequisites

Before you begin:

- Unity 2021.3 or newer installed
- Basic understanding of C# and Unity development
- Basic knowledge of blockchain concepts (optional but helpful)

## Installation

### Option 1: Unity Package Manager (Recommended)

1. Open your Unity project
2. Go to **Window > Package Manager**
3. Click the **+** button in the top-left corner
4. Select **Add package from git URL...**
5. Enter the Rally Protocol Unity SDK repository URL:

   ```
   https://github.com/rally-io/rly-network-unity-sdk.git
   ```

6. Click **Add**

### Option 2: Manual Installation

1. Download the latest release from the [GitHub repository](https://github.com/rally-io/rly-network-unity-sdk/releases)
2. Unzip the package
3. In your Unity project, go to **Assets > Import Package > Custom Package...**
4. Navigate to the downloaded `.unitypackage` file and import it

## Initial Setup

### 1. Configure the SDK

1. In your Unity project, go to **Window > Rally Protocol > Configuration**
2. In the configuration window that opens:
   - Enter your Rally Protocol API key (sign up at [Rally Developer Portal](https://docs.rallyprotocol.com) if you don't have one)
   - Select the network type (BaseSepolia for testing, Base for production)
   - Click **Save Configuration**

Alternatively, you can create a settings preset:

1. Right-click in the Project window
2. Select **Create > Rally Protocol > Settings Preset**
3. Name the preset (e.g., "MainSettings")
4. Configure the preset in the Inspector
5. Move the preset to `Resources/RallyProtocol/Settings/Main` to make it the default

### 2. Initialize the SDK

The SDK can be initialized automatically by accessing the singleton instance, or manually in your scripts:

```csharp
using RallyProtocolUnity.Components;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private void Awake()
    {
        // This will automatically initialize the SDK
        var rallyManager = RallyUnityManager.Instance;
        
        Debug.Log($"Rally Protocol SDK initialized: {rallyManager.IsInitialized}");
    }
}
```

## Basic Usage

### Creating a Wallet

When a user first interacts with your application, you'll need to create a wallet for them:

```csharp
using RallyProtocolUnity.Components;
using UnityEngine;
using System.Threading.Tasks;

public class WalletManager : MonoBehaviour
{
    private async Task CreateWallet()
    {
        try
        {
            var rlyNetwork = RallyUnityManager.Instance.RlyNetwork;
            var account = await rlyNetwork.AccountManager.GetAccountAsync();
            
            Debug.Log($"Wallet created! Address: {account.Address}");
            
            // Store this address if needed
            PlayerPrefs.SetString("WalletAddress", account.Address);
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Failed to create wallet: {ex.Message}");
        }
    }
    
    // Call this method when the user starts the game for the first time
    public async void OnFirstTimeSetup()
    {
        await CreateWallet();
    }
}
```

### Checking Token Balance

```csharp
using RallyProtocolUnity.Components;
using UnityEngine;
using System.Threading.Tasks;

public class BalanceManager : MonoBehaviour
{
    public TMPro.TextMeshProUGUI balanceText;
    
    public async Task UpdateBalance()
    {
        try
        {
            var rlyNetwork = RallyUnityManager.Instance.RlyNetwork;
            decimal balance = await rlyNetwork.GetDisplayBalanceAsync();
            
            Debug.Log($"Current balance: {balance} RLY");
            
            if (balanceText != null)
            {
                balanceText.text = $"{balance} RLY";
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Failed to get balance: {ex.Message}");
        }
    }
    
    // Call this periodically to update the UI
    private void Start()
    {
        UpdateBalance();
    }
}
```

### Claiming RLY Tokens (Testnet)

On test networks like BaseSepolia, you can claim free RLY tokens for testing:

```csharp
using RallyProtocolUnity.Components;
using UnityEngine;
using System.Threading.Tasks;

public class TokenFaucet : MonoBehaviour
{
    [SerializeField] private GameObject claimButton;
    [SerializeField] private GameObject claimingIndicator;
    
    public async Task ClaimTokens()
    {
        claimButton.SetActive(false);
        claimingIndicator.SetActive(true);
        
        try
        {
            var rlyNetwork = RallyUnityManager.Instance.RlyNetwork;
            string txHash = await rlyNetwork.ClaimRlyAsync();
            
            Debug.Log($"Tokens claimed successfully! Transaction: {txHash}");
            
            // Wait for confirmation and update balance
            await UpdateBalanceAfterDelay(5000); // 5 seconds delay
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Failed to claim tokens: {ex.Message}");
        }
        finally
        {
            claimButton.SetActive(true);
            claimingIndicator.SetActive(false);
        }
    }
    
    private async Task UpdateBalanceAfterDelay(int milliseconds)
    {
        await Task.Delay(milliseconds);
        
        // Find the balance manager and update the display
        var balanceManager = FindObjectOfType<BalanceManager>();
        if (balanceManager != null)
        {
            await balanceManager.UpdateBalance();
        }
    }
    
    // Connect this to your UI button
    public void OnClaimButtonClicked()
    {
        ClaimTokens();
    }
}
```

### Transferring Tokens

To transfer tokens to another address:

```csharp
using RallyProtocolUnity.Components;
using UnityEngine;
using System.Threading.Tasks;

public class TransferManager : MonoBehaviour
{
    [SerializeField] private TMPro.TMP_InputField addressInput;
    [SerializeField] private TMPro.TMP_InputField amountInput;
    [SerializeField] private GameObject transferButton;
    [SerializeField] private GameObject transferringIndicator;
    
    public async Task TransferTokens()
    {
        string destinationAddress = addressInput.text;
        
        if (!decimal.TryParse(amountInput.text, out decimal amount))
        {
            Debug.LogError("Invalid amount");
            return;
        }
        
        transferButton.SetActive(false);
        transferringIndicator.SetActive(true);
        
        try
        {
            var rlyNetwork = RallyUnityManager.Instance.RlyNetwork;
            string txHash = await rlyNetwork.TransferAsync(destinationAddress, amount);
            
            Debug.Log($"Transfer successful! Transaction: {txHash}");
            
            // Wait for confirmation and update balance
            await UpdateBalanceAfterDelay(5000); // 5 seconds delay
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Transfer failed: {ex.Message}");
        }
        finally
        {
            transferButton.SetActive(true);
            transferringIndicator.SetActive(false);
        }
    }
    
    private async Task UpdateBalanceAfterDelay(int milliseconds)
    {
        await Task.Delay(milliseconds);
        
        // Find the balance manager and update the display
        var balanceManager = FindObjectOfType<BalanceManager>();
        if (balanceManager != null)
        {
            await balanceManager.UpdateBalance();
        }
    }
    
    // Connect this to your UI button
    public void OnTransferButtonClicked()
    {
        TransferTokens();
    }
}
```

## Using Components

The Rally Protocol Unity SDK includes several ready-to-use Unity components that simplify integration.

### RallyUnityManager Component

The core component that manages the Rally Protocol SDK. Add it to a persistent GameObject in your scene:

1. Create an empty GameObject (e.g., "RallyManager")
2. Add the **RallyUnityManager** component to it:
   - Select the GameObject
   - Click **Add Component** in the Inspector
   - Search for "Rally Unity Manager" and add it
3. Set **Dont Destroy On Load** to ensure it persists across scenes

### RallyClaimRly Component

Allows claiming RLY tokens from the faucet:

1. Create a GameObject with a button UI element
2. Add the **RallyClaimRly** component to it
3. Connect the button's onClick event to the component's **Claim** method
4. Optionally connect UI elements to display status

```csharp
// Listening for events from the component
void Start()
{
    var claimComponent = GetComponent<RallyClaimRly>();
    
    claimComponent.Claiming += OnClaiming;
    claimComponent.Claimed += OnClaimed;
}

void OnClaiming(object sender, System.EventArgs e)
{
    statusText.text = "Claiming tokens...";
}

void OnClaimed(object sender, RallyClaimRlyEventArgs e)
{
    statusText.text = $"Claimed! Tx: {e.TransactionHash.Substring(0, 10)}...";
}
```

### RallyTransfer Component

Enables token transfers:

1. Create a GameObject with input fields for address and amount, and a transfer button
2. Add the **RallyTransfer** component to it
3. Connect the UI elements in the Inspector:
   - Assign the destination address input field
   - Assign the amount input field
   - Connect the button's onClick event to the component's **Transfer** method

```csharp
// Listening for events from the component
void Start()
{
    var transferComponent = GetComponent<RallyTransfer>();
    
    transferComponent.Transferring += OnTransferring;
    transferComponent.Transferred += OnTransferred;
}

void OnTransferring(object sender, System.EventArgs e)
{
    statusText.text = "Transfer in progress...";
}

void OnTransferred(object sender, RallyTransferEventArgs e)
{
    statusText.text = $"Transfer complete! Tx: {e.TransactionHash.Substring(0, 10)}...";
}
```

## Rewarding Players

### Simple Reward Function

```csharp
using RallyProtocolUnity.Components;
using UnityEngine;
using System.Threading.Tasks;

public class PlayerRewardSystem : MonoBehaviour
{
    [SerializeField] private decimal levelCompletionReward = 1.0m;
    [SerializeField] private decimal dailyLoginReward = 0.5m;
    
    public async Task RewardForLevelCompletion(int levelNumber)
    {
        try
        {
            // Get player wallet address
            string playerAddress = PlayerPrefs.GetString("PlayerWalletAddress");
            
            if (string.IsNullOrEmpty(playerAddress))
            {
                Debug.LogError("Player wallet address not found");
                return;
            }
            
            // Calculate reward amount (could be based on level number, time, score, etc.)
            decimal rewardAmount = levelCompletionReward * (1 + (levelNumber * 0.1m));
            
            // Send the reward
            var rlyNetwork = RallyUnityManager.Instance.RlyNetwork;
            string txHash = await rlyNetwork.TransferAsync(playerAddress, rewardAmount);
            
            Debug.Log($"Level completion reward sent! Amount: {rewardAmount} RLY, Transaction: {txHash}");
            
            // Update UI or show notification to player
            ShowRewardNotification(rewardAmount);
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Failed to send reward: {ex.Message}");
        }
    }
    
    public async Task RewardForDailyLogin()
    {
        try
        {
            // Get player wallet address
            string playerAddress = PlayerPrefs.GetString("PlayerWalletAddress");
            
            if (string.IsNullOrEmpty(playerAddress))
            {
                Debug.LogError("Player wallet address not found");
                return;
            }
            
            // Send the daily login reward
            var rlyNetwork = RallyUnityManager.Instance.RlyNetwork;
            string txHash = await rlyNetwork.TransferAsync(playerAddress, dailyLoginReward);
            
            Debug.Log($"Daily login reward sent! Amount: {dailyLoginReward} RLY, Transaction: {txHash}");
            
            // Update UI or show notification to player
            ShowRewardNotification(dailyLoginReward);
            
            // Save the timestamp of the reward
            PlayerPrefs.SetString("LastDailyReward", System.DateTime.Now.ToString("yyyy-MM-dd"));
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Failed to send daily reward: {ex.Message}");
        }
    }
    
    private void ShowRewardNotification(decimal amount)
    {
        // Implement your notification system here
        Debug.Log($"Player earned {amount} RLY tokens!");
    }
    
    // Check for daily rewards on game start
    private void Start()
    {
        CheckDailyReward();
    }
    
    private void CheckDailyReward()
    {
        string lastRewardDate = PlayerPrefs.GetString("LastDailyReward", "");
        string today = System.DateTime.Now.ToString("yyyy-MM-dd");
        
        if (lastRewardDate != today)
        {
            RewardForDailyLogin();
        }
    }
}
```

## Error Handling

It's important to handle blockchain-related errors gracefully:

```csharp
using RallyProtocolUnity.Components;
using UnityEngine;
using System;
using System.Threading.Tasks;

public class RallyErrorHandler : MonoBehaviour
{
    [SerializeField] private TMPro.TextMeshProUGUI errorText;
    [SerializeField] private GameObject errorPanel;
    
    public async Task<bool> ExecuteWithErrorHandling(Func<Task> operation, string operationName)
    {
        try
        {
            // Show loading indicator
            // ...
            
            await operation();
            
            // Hide loading indicator
            // ...
            
            return true;
        }
        catch (Exception ex)
        {
            // Hide loading indicator
            // ...
            
            // Log the error
            Debug.LogError($"{operationName} failed: {ex.Message}");
            
            // Show user-friendly error message
            ShowError(GetUserFriendlyError(ex, operationName));
            
            return false;
        }
    }
    
    private void ShowError(string message)
    {
        if (errorText != null)
        {
            errorText.text = message;
        }
        
        if (errorPanel != null)
        {
            errorPanel.SetActive(true);
        }
    }
    
    private string GetUserFriendlyError(Exception ex, string operation)
    {
        // Network connection issues
        if (ex.Message.Contains("network") || ex.Message.Contains("connection"))
        {
            return "Unable to connect to the network. Please check your internet connection and try again.";
        }
        
        // Insufficient funds
        if (ex.Message.Contains("insufficient") || ex.Message.Contains("balance"))
        {
            return "You don't have enough tokens to complete this transaction.";
        }
        
        // Invalid address
        if (ex.Message.Contains("invalid address") || ex.Message.Contains("address format"))
        {
            return "The wallet address you entered is not valid. Please check and try again.";
        }
        
        // Generic error
        return $"An error occurred while {operation.ToLower()}. Please try again later.";
    }
    
    // Example usage:
    public async void OnTransferButtonClicked()
    {
        await ExecuteWithErrorHandling(async () => 
        {
            string address = addressInput.text;
            decimal amount = decimal.Parse(amountInput.text);
            
            var rlyNetwork = RallyUnityManager.Instance.RlyNetwork;
            await rlyNetwork.TransferAsync(address, amount);
        }, "Transferring tokens");
    }
}
```

## Testing Your Integration

1. **Editor Testing**: Test basic functionality in the Unity Editor
2. **Testnet Testing**: Deploy to Base Sepolia testnet for full testing
3. **End-to-End Testing**: Test the complete user flow
4. **Edge Cases**: Test error scenarios, network disconnections, etc.

## Next Steps

Now that you've got the basics working, you can:

1. Customize your token economy
2. Implement more advanced features
3. Create a proper player wallet management system
4. Design engaging reward mechanics
5. Deploy to production when ready

For more detailed information, explore the full documentation:

- [Complete API Reference](api/README.md)
- [Tutorial: Implementing a Player Reward System](tutorials/reward-system.md)
- [Platform-Specific Guides](platform-guides/README.md)
- [Troubleshooting Guide](troubleshooting.md)

## Support

If you encounter any issues:

1. Check the [Troubleshooting Guide](troubleshooting.md)
2. Visit the [Rally Protocol Discord](https://discord.gg/rlynetwork)
3. Open an issue on [GitHub](https://github.com/rally-io/rly-network-unity-sdk/issues)
