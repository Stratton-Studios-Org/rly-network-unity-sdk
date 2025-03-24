# Building a Player Reward System with Rally Protocol

This tutorial will guide you through implementing a complete player reward system using the Rally Protocol Unity SDK. By the end, you'll have a functional reward system that can issue RLY tokens to players for completing in-game achievements.

## Prerequisites

Before starting this tutorial, ensure you have:

- Unity 2021.3 or newer
- Rally Protocol Unity SDK installed
- Basic understanding of C# and Unity development
- An API key from Rally Protocol

## Step 1: Set Up the Project

First, let's create the necessary components for our reward system:

1. Create a new C# script named `RewardManager.cs` in your project
2. Open the script and replace its contents with the following skeleton:

    ```csharp
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using UnityEngine;
    using RallyProtocolUnity.Runtime.Core;

    public class RewardManager : MonoBehaviour
    {
        [Header("Configuration")]
        [SerializeField] private bool initializeOnStart = true;
        [SerializeField] private bool createAccountIfMissing = true;
        
        [Header("Rewards")]
        [SerializeField] private decimal smallRewardAmount = 1.0m;
        [SerializeField] private decimal mediumRewardAmount = 3.0m;
        [SerializeField] private decimal largeRewardAmount = 5.0m;
        
        private IRallyNetwork rlyNetwork;
        private bool isInitialized = false;
        
        // Events
        public event Action OnInitialized;
        public event Action<decimal> OnRewardIssued;
        public event Action<string> OnError;
        
        private void Start()
        {
            if (initializeOnStart)
            {
                Initialize();
            }
        }
        
        // We'll implement these methods in the next steps
        public async void Initialize() { }
        public async Task<bool> IssueReward(decimal amount) { return false; }
        public async Task<decimal> GetCurrentBalance() { return 0; }
    }
    ```

3. Create a new empty GameObject in your scene and name it "RewardManager"
4. Attach the `RewardManager.cs` script to this GameObject

## Step 2: Initialize the Rally Protocol

Now, let's implement the initialization logic for our reward system:

```csharp
public async void Initialize()
{
    if (isInitialized)
    {
        Debug.Log("Reward Manager already initialized.");
        return;
    }
    
    try
    {
        Debug.Log("Initializing Reward Manager...");
        
        // Get the Rally Network instance
        rlyNetwork = RallyUnityManager.Instance.RlyNetwork;
        
        // Get the account manager
        var accountManager = rlyNetwork.AccountManager;
        
        // Check if the user has an account
        var account = await accountManager.GetAccountAsync();
        
        // Create a new account if needed
        if (account == null && createAccountIfMissing)
        {
            Debug.Log("No account found. Creating a new account...");
            
            await accountManager.CreateAccountAsync(new()
            {
                Overwrite = false,
                StorageOptions = new()
                {
                    SaveToCloud = true,
                    RejectOnCloudSaveFailure = false
                }
            });
            
            account = await accountManager.GetAccountAsync();
            Debug.Log($"New account created: {account.Address}");
        }
        else if (account == null)
        {
            Debug.LogWarning("No account found and account creation is disabled.");
            OnError?.Invoke("No account available. Enable account creation or create an account manually.");
            return;
        }
        else
        {
            Debug.Log($"Using existing account: {account.Address}");
        }
        
        isInitialized = true;
        OnInitialized?.Invoke();
        
        // Get initial balance
        decimal balance = await GetCurrentBalance();
        Debug.Log($"Current balance: {balance} RLY");
    }
    catch (Exception ex)
    {
        Debug.LogError($"Failed to initialize Reward Manager: {ex.Message}");
        OnError?.Invoke($"Initialization failed: {ex.Message}");
    }
}
```

## Step 3: Implement Balance Checking

Next, let's implement the method to check the player's current RLY balance:

```csharp
public async Task<decimal> GetCurrentBalance()
{
    if (!isInitialized)
    {
        Debug.LogWarning("Reward Manager is not initialized. Call Initialize() first.");
        return 0;
    }
    
    try
    {
        decimal balance = await rlyNetwork.GetBalanceAsync();
        return balance;
    }
    catch (Exception ex)
    {
        Debug.LogError($"Failed to get balance: {ex.Message}");
        OnError?.Invoke($"Balance check failed: {ex.Message}");
        return 0;
    }
}
```

## Step 4: Implement Reward Issuance

Now, let's implement the most important part - issuing rewards to players:

```csharp
public async Task<bool> IssueReward(decimal amount)
{
    if (!isInitialized)
    {
        Debug.LogWarning("Reward Manager is not initialized. Call Initialize() first.");
        return false;
    }
    
    if (amount <= 0)
    {
        Debug.LogError("Reward amount must be greater than zero.");
        OnError?.Invoke("Invalid reward amount");
        return false;
    }
    
    try
    {
        // For this example, we're using the claim RLY functionality
        // In a production environment, you might use a custom token contract 
        // or other distribution mechanism
        
        Debug.Log($"Issuing reward of {amount} RLY...");
        
        // In a real application, you would implement a custom reward distribution
        // mechanism here. For demo purposes, we're using ClaimRlyAsync
        await rlyNetwork.ClaimRlyAsync();
        
        // Notify subscribers
        OnRewardIssued?.Invoke(amount);
        
        Debug.Log($"Reward issued successfully: {amount} RLY");
        
        // Get updated balance
        decimal newBalance = await GetCurrentBalance();
        Debug.Log($"New balance: {newBalance} RLY");
        
        return true;
    }
    catch (Exception ex)
    {
        Debug.LogError($"Failed to issue reward: {ex.Message}");
        OnError?.Invoke($"Reward failed: {ex.Message}");
        return false;
    }
}
```

## Step 5: Add Convenience Methods

Let's add some convenience methods for different reward tiers:

```csharp
public async Task<bool> IssueSmallReward()
{
    return await IssueReward(smallRewardAmount);
}

public async Task<bool> IssueMediumReward()
{
    return await IssueReward(mediumRewardAmount);
}

public async Task<bool> IssueLargeReward()
{
    return await IssueReward(largeRewardAmount);
}
```

## Step 6: Create a UI Controller

Now, let's create a UI controller to interact with our reward system:

1. Create a new C# script named `RewardUIController.cs`
2. Replace its contents with the following code:

```csharp
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RewardUIController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RewardManager rewardManager;
    
    [Header("UI Elements")]
    [SerializeField] private Button initializeButton;
    [SerializeField] private Button smallRewardButton;
    [SerializeField] private Button mediumRewardButton;
    [SerializeField] private Button largeRewardButton;
    [SerializeField] private Button refreshBalanceButton;
    [SerializeField] private TextMeshProUGUI balanceText;
    [SerializeField] private TextMeshProUGUI statusText;
    
    private void Start()
    {
        // Set up button listeners
        initializeButton.onClick.AddListener(OnInitializeClicked);
        smallRewardButton.onClick.AddListener(OnSmallRewardClicked);
        mediumRewardButton.onClick.AddListener(OnMediumRewardClicked);
        largeRewardButton.onClick.AddListener(OnLargeRewardClicked);
        refreshBalanceButton.onClick.AddListener(OnRefreshBalanceClicked);
        
        // Set up event listeners
        rewardManager.OnInitialized += OnRewardManagerInitialized;
        rewardManager.OnRewardIssued += OnRewardIssued;
        rewardManager.OnError += OnRewardManagerError;
        
        // Initialize UI state
        UpdateUIState(false);
    }
    
    private void OnDestroy()
    {
        // Clean up event listeners
        rewardManager.OnInitialized -= OnRewardManagerInitialized;
        rewardManager.OnRewardIssued -= OnRewardIssued;
        rewardManager.OnError -= OnRewardManagerError;
    }
    
    private void UpdateUIState(bool initialized)
    {
        initializeButton.interactable = !initialized;
        smallRewardButton.interactable = initialized;
        mediumRewardButton.interactable = initialized;
        largeRewardButton.interactable = initialized;
        refreshBalanceButton.interactable = initialized;
    }
    
    private void OnInitializeClicked()
    {
        statusText.text = "Initializing...";
        initializeButton.interactable = false;
        rewardManager.Initialize();
    }
    
    private async void OnSmallRewardClicked()
    {
        await ProcessReward(rewardManager.IssueSmallReward, "small");
    }
    
    private async void OnMediumRewardClicked()
    {
        await ProcessReward(rewardManager.IssueMediumReward, "medium");
    }
    
    private async void OnLargeRewardClicked()
    {
        await ProcessReward(rewardManager.IssueLargeReward, "large");
    }
    
    private async Task ProcessReward(System.Func<Task<bool>> rewardFunc, string rewardSize)
    {
        statusText.text = $"Issuing {rewardSize} reward...";
        SetButtonsInteractable(false);
        
        bool success = await rewardFunc();
        
        if (success)
        {
            statusText.text = $"{rewardSize.ToUpperInvariant()} reward issued successfully!";
            await UpdateBalance();
        }
        
        SetButtonsInteractable(true);
    }
    
    private void SetButtonsInteractable(bool interactable)
    {
        smallRewardButton.interactable = interactable;
        mediumRewardButton.interactable = interactable;
        largeRewardButton.interactable = interactable;
    }
    
    private async void OnRefreshBalanceClicked()
    {
        await UpdateBalance();
    }
    
    private async Task UpdateBalance()
    {
        decimal balance = await rewardManager.GetCurrentBalance();
        balanceText.text = $"Balance: {balance} RLY";
    }
    
    private void OnRewardManagerInitialized()
    {
        UpdateUIState(true);
        statusText.text = "Initialized successfully";
    }
    
    private void OnRewardIssued(decimal amount)
    {
        statusText.text = $"Reward issued: {amount} RLY";
    }
    
    private void OnRewardManagerError(string errorMessage)
    {
        statusText.text = $"Error: {errorMessage}";
    }
}
```

## Step 7: Create the UI

Now, let's create a simple UI for our reward system:

1. Create a Canvas in your scene (GameObject > UI > Canvas)
2. Add a Panel element to the Canvas
3. Add the following UI elements to the Panel:
   - TextMeshProUGUI element for the title
   - TextMeshProUGUI element for the balance display
   - Button for initializing the system
   - Three buttons for different reward tiers
   - Button for refreshing the balance
   - TextMeshProUGUI element for status messages
4. Attach the `RewardUIController.cs` script to the Panel
5. Assign the `RewardManager` GameObject to the `rewardManager` field
6. Assign all UI elements to their respective fields in the inspector

## Step 8: Connect to Game Events

In a real game, you'll want to connect the reward system to actual game events. Here's an example of how you might do that:

```csharp
// Example game achievement manager
public class AchievementManager : MonoBehaviour
{
    [SerializeField] private RewardManager rewardManager;
    
    // Called when the player completes a level
    public async void OnLevelCompleted(int levelNumber, int stars)
    {
        Debug.Log($"Level {levelNumber} completed with {stars} stars");
        
        // Issue rewards based on performance
        if (stars == 3)
        {
            await rewardManager.IssueLargeReward();
        }
        else if (stars == 2)
        {
            await rewardManager.IssueMediumReward();
        }
        else if (stars == 1)
        {
            await rewardManager.IssueSmallReward();
        }
    }
    
    // Called when the player unlocks an achievement
    public async void OnAchievementUnlocked(string achievementId)
    {
        Debug.Log($"Achievement unlocked: {achievementId}");
        
        // Map achievements to reward tiers
        switch (achievementId)
        {
            case "first_win":
            case "login_streak_3":
                await rewardManager.IssueSmallReward();
                break;
                
            case "win_streak_5":
            case "collect_10_items":
                await rewardManager.IssueMediumReward();
                break;
                
            case "complete_all_levels":
            case "perfect_score":
                await rewardManager.IssueLargeReward();
                break;
        }
    }
}
```

## Step 9: Add Error Handling and Fallbacks

In a production environment, you'll want to add robust error handling and fallbacks for when blockchain operations fail:

```csharp
// Add this to the RewardManager class
private Dictionary<string, QueuedReward> queuedRewards = new Dictionary<string, QueuedReward>();

private class QueuedReward
{
    public decimal Amount { get; set; }
    public DateTime Timestamp { get; set; }
    public int RetryCount { get; set; }
}

public async Task<bool> IssueRewardWithFallback(decimal amount)
{
    if (!isInitialized)
    {
        // Store the reward for later when we're initialized
        string rewardId = Guid.NewGuid().ToString();
        queuedRewards[rewardId] = new QueuedReward 
        { 
            Amount = amount, 
            Timestamp = DateTime.Now,
            RetryCount = 0
        };
        
        Debug.LogWarning($"Reward Manager not initialized. Reward of {amount} RLY queued for later.");
        return false;
    }
    
    try
    {
        return await IssueReward(amount);
    }
    catch (Exception ex)
    {
        // If blockchain operation fails, store for retry
        string rewardId = Guid.NewGuid().ToString();
        queuedRewards[rewardId] = new QueuedReward 
        { 
            Amount = amount, 
            Timestamp = DateTime.Now,
            RetryCount = 0
        };
        
        Debug.LogError($"Failed to issue reward, queued for retry: {ex.Message}");
        return false;
    }
}

// Add a method to process queued rewards
public async Task ProcessQueuedRewards()
{
    if (!isInitialized || queuedRewards.Count == 0)
    {
        return;
    }
    
    List<string> completedRewards = new List<string>();
    
    foreach (var kvp in queuedRewards)
    {
        string rewardId = kvp.Key;
        QueuedReward reward = kvp.Value;
        
        // Skip rewards that have been retried too many times
        if (reward.RetryCount >= 3)
        {
            Debug.LogWarning($"Skipping reward {rewardId}, max retries reached");
            continue;
        }
        
        try
        {
            Debug.Log($"Processing queued reward {rewardId} of {reward.Amount} RLY");
            bool success = await IssueReward(reward.Amount);
            
            if (success)
            {
                completedRewards.Add(rewardId);
            }
            else
            {
                reward.RetryCount++;
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to process queued reward {rewardId}: {ex.Message}");
            reward.RetryCount++;
        }
    }
    
    // Remove completed rewards
    foreach (string rewardId in completedRewards)
    {
        queuedRewards.Remove(rewardId);
    }
}
```

## Step 10: Testing the Reward System

Now that everything is set up, you can test your reward system:

1. Enter Play mode in Unity
2. Click the "Initialize" button to set up the Rally Protocol
3. Wait for initialization to complete
4. Test the different reward tiers by clicking the reward buttons
5. Verify that the balance updates correctly

## Advanced Topics

### Customizing the Reward Distribution

In a real application, you'll likely want to use a custom token contract or a more sophisticated reward mechanism. You could modify the `IssueReward` method to interact with a custom contract:

```csharp
public async Task<bool> IssueCustomTokenReward(decimal amount)
{
    if (!isInitialized)
    {
        Debug.LogWarning("Reward Manager is not initialized. Call Initialize() first.");
        return false;
    }
    
    try
    {
        // Create a contract interface
        string contractAddress = "0xYourTokenContractAddress";
        string abi = "your-contract-abi";
        
        var contract = rlyNetwork.Web3.Eth.GetContract(abi, contractAddress);
        var transferFunction = contract.GetFunction("transfer");
        
        // Get account
        var account = await rlyNetwork.AccountManager.GetAccountAsync();
        
        // Create transaction data
        var functionCallData = transferFunction.GetData(account.Address, amount);
        
        // Create GSN transaction
        var txDetails = new GsnTransactionDetails
        {
            From = account.Address,
            To = contractAddress,
            Data = functionCallData
        };
        
        // Relay the transaction
        string txHash = await rlyNetwork.RelayAsync(txDetails);
        
        Debug.Log($"Custom token reward transaction sent: {txHash}");
        OnRewardIssued?.Invoke(amount);
        
        return true;
    }
    catch (Exception ex)
    {
        Debug.LogError($"Failed to issue custom token reward: {ex.Message}");
        OnError?.Invoke($"Custom reward failed: {ex.Message}");
        return false;
    }
}
```

### Reward Analytics

You might want to track reward issuance for analytics purposes:

```csharp
// Add these fields to RewardManager
private int totalRewardsIssued = 0;
private decimal totalAmountIssued = 0;

// Modify the IssueReward method
public async Task<bool> IssueReward(decimal amount)
{
    // Existing code...
    
    if (success)
    {
        // Update analytics
        totalRewardsIssued++;
        totalAmountIssued += amount;
        
        // You could send these metrics to a server or analytics service
        Debug.Log($"Total rewards issued: {totalRewardsIssued}, Total amount: {totalAmountIssued} RLY");
    }
    
    // Rest of existing code...
}
```

## Conclusion

You've now implemented a complete player reward system using the Rally Protocol Unity SDK. Players can earn RLY tokens for their achievements, which can be used within your game economy or transferred out to external wallets.

This implementation includes:

- Initialization of the Rally Protocol
- Account creation and management
- Reward issuance in different tiers
- UI for interacting with the reward system
- Error handling and fallbacks
- Integration with game events

You can extend this system further by:

- Adding custom token contracts
- Implementing more sophisticated reward logic
- Adding analytics and tracking
- Enhancing the UI with animations and effects
- Implementing a reward schedule or limits
