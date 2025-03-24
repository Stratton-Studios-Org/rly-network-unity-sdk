# Implementing a Token Economy in Your Unity Game

This tutorial walks through how to implement a complete token economy in a Unity game using the Rally Protocol Unity SDK. We'll cover setting up token-based rewards, creating an in-game shop, and implementing token utility.

## Prerequisites

Before starting this tutorial, ensure you have:

- Installed the Rally Protocol Unity SDK
- Set up authentication and account creation in your game
- Basic familiarity with Unity UI elements
- Understanding of basic blockchain concepts

## Overview of a Game Token Economy

A token economy in games typically consists of:

1. **Token Acquisition**: Ways players earn tokens (quests, achievements, daily rewards)
2. **Token Utility**: Ways players spend tokens (purchases, upgrades, access)
3. **Economy Balance**: Systems to maintain a healthy economic balance

## Part 1: Setting Up Token Rewards

### Step 1: Create a Reward Manager

First, let's create a RewardManager to handle token rewards:

```csharp
using RallyProtocol.Unity;
using System.Threading.Tasks;
using UnityEngine;

public class RewardManager : MonoBehaviour
{
    private IRallyNetwork rlyNetwork;
    
    private void Awake()
    {
        // Get reference to RallyNetwork
        rlyNetwork = RallyUnityManager.Instance.RlyNetwork;
    }
    
    public async Task AwardTokensForAchievement(string achievementId, decimal amount)
    {
        // Check if achievement is completed
        if (IsAchievementCompleted(achievementId))
            return;
            
        // Award tokens
        bool success = await ClaimTokensFromGame(amount);
        
        if (success)
        {
            // Mark achievement as completed
            MarkAchievementCompleted(achievementId);
            
            // Notify UI
            EventManager.TriggerEvent("TokensAwarded", amount);
        }
    }
    
    public async Task AwardDailyLoginBonus(decimal amount)
    {
        if (HasClaimedDailyBonus())
            return;
            
        // Award tokens
        bool success = await ClaimTokensFromGame(amount);
        
        if (success)
        {
            // Mark daily bonus as claimed
            SetDailyBonusClaimed();
            
            // Notify UI
            EventManager.TriggerEvent("DailyBonusClaimed", amount);
        }
    }
    
    private async Task<bool> ClaimTokensFromGame(decimal amount)
    {
        try
        {
            // Implementation depends on your specific game's economy design
            // This could be a direct transfer from a game wallet or a custom claim mechanism
            
            // Option 1: Direct claiming of RLY tokens (if you've set up a custom claiming system)
            await rlyNetwork.ClaimRlyAsync();
            
            // Option 2: Transfer tokens from a game treasury wallet to the player
            // string treasuryWalletAddress = "YOUR_TREASURY_WALLET_ADDRESS";
            // await rlyNetwork.TransferAsync(rlyNetwork.AccountManager.GetAccountAddress(), amount);
            
            // Option 3: Integration with a custom backend for token distribution
            // await YourGameServer.DistributeTokensAsync(playerId, amount);
            
            // Always verify the balance was updated
            decimal newBalance = await rlyNetwork.GetBalanceAsync();
            Debug.Log($"New balance after claim: {newBalance}");
            
            // Update UI
            EventManager.TriggerEvent("BalanceUpdated");
            
            return true;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to award tokens: {e.Message}");
            
            // Implement proper fallback/retry logic
            // For example, store failed claims to retry later
            SaveFailedClaim(amount);
            
            return false;
        }
    }
    
    // Helper method to save failed claims for later retry
    private void SaveFailedClaim(decimal amount)
    {
        // Implementation could store to PlayerPrefs or other persistent storage
        string claimsJson = PlayerPrefs.GetString("PendingTokenClaims", "[]");
        List<PendingClaim> pendingClaims;
        
        try
        {
            pendingClaims = JsonUtility.FromJson<List<PendingClaim>>(claimsJson);
        }
        catch
        {
            pendingClaims = new List<PendingClaim>();
        }
        
        pendingClaims.Add(new PendingClaim 
        { 
            Amount = amount,
            Timestamp = System.DateTime.Now.ToString("o")
        });
        
        string updatedJson = JsonUtility.ToJson(pendingClaims);
        PlayerPrefs.SetString("PendingTokenClaims", updatedJson);
        PlayerPrefs.Save();
    }
    
    // Data structure for pending claims
    [System.Serializable]
    private class PendingClaim
    {
        public decimal Amount;
        public string Timestamp;
    }
    
    // Game-specific achievement tracking methods
    private bool IsAchievementCompleted(string achievementId)
    {
        // Implement based on your achievement system
        return PlayerPrefs.GetInt($"Achievement_{achievementId}", 0) == 1;
    }
    
    private void MarkAchievementCompleted(string achievementId)
    {
        PlayerPrefs.SetInt($"Achievement_{achievementId}", 1);
        PlayerPrefs.Save();
    }
    
    private bool HasClaimedDailyBonus()
    {
        string lastClaimDate = PlayerPrefs.GetString("LastDailyBonusDate", "");
        string currentDate = System.DateTime.Now.ToString("yyyy-MM-dd");
        return lastClaimDate == currentDate;
    }
    
    private void SetDailyBonusClaimed()
    {
        string currentDate = System.DateTime.Now.ToString("yyyy-MM-dd");
        PlayerPrefs.SetString("LastDailyBonusDate", currentDate);
        PlayerPrefs.Save();
    }
}
```

### Step 2: Implement Achievement System Integration

Integrate the RewardManager with your achievement system:

```csharp
public class AchievementSystem : MonoBehaviour
{
    [SerializeField] private RewardManager rewardManager;
    
    // Sample achievement data
    private Dictionary<string, AchievementData> achievements = new Dictionary<string, AchievementData>
    {
        { "first_win", new AchievementData("First Win", "Win your first match", 5.0m) },
        { "ten_matches", new AchievementData("Dedicated Player", "Play 10 matches", 10.0m) },
        { "level_10", new AchievementData("Rising Star", "Reach level 10", 20.0m) }
    };
    
    public async void UnlockAchievement(string achievementId)
    {
        if (!achievements.ContainsKey(achievementId))
            return;
            
        AchievementData achievement = achievements[achievementId];
        
        // Award tokens
        await rewardManager.AwardTokensForAchievement(achievementId, achievement.TokenReward);
        
        // Show achievement UI
        ShowAchievementUnlocked(achievement);
    }
    
    private void ShowAchievementUnlocked(AchievementData achievement)
    {
        // Implement UI notification
    }
}

public class AchievementData
{
    public string Title { get; private set; }
    public string Description { get; private set; }
    public decimal TokenReward { get; private set; }
    
    public AchievementData(string title, string description, decimal tokenReward)
    {
        Title = title;
        Description = description;
        TokenReward = tokenReward;
    }
}
```

## Part 2: Creating the Token Shop

### Step 1: Create Shop Item Data Structure

```csharp
[System.Serializable]
public class ShopItem
{
    public string Id;
    public string Name;
    public string Description;
    public decimal Price;
    public Sprite Icon;
    public ItemType Type;
    
    public enum ItemType
    {
        Cosmetic,
        Powerup,
        Currency,
        Special
    }
}
```

### Step 2: Implement Shop Manager

```csharp
using RallyProtocol.Unity;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    [SerializeField] private List<ShopItem> availableItems = new List<ShopItem>();
    
    private IRallyNetwork rlyNetwork;
    
    private void Awake()
    {
        // Get reference to RallyNetwork
        rlyNetwork = RallyUnityManager.Instance.RlyNetwork;
    }
    
    public async Task<bool> PurchaseItem(ShopItem item)
    {
        try
        {
            // Check if player has enough tokens
            decimal balance = await rlyNetwork.GetBalanceAsync();
            
            if (balance < item.Price)
            {
                Debug.Log("Insufficient funds to purchase item");
                return false;
            }
            
            // Implementation depends on your game economy design
            // Option 1: Transfer tokens to a game treasury/merchant wallet
            string gameTreasuryAddress = "YOUR_GAME_TREASURY_ADDRESS"; // Replace with your actual address
            
            // Execute the transfer
            string txHash = await rlyNetwork.TransferAsync(gameTreasuryAddress, item.Price);
            
            // Wait for transaction confirmation if needed
            // bool confirmed = await rlyNetwork.WaitForTransactionConfirmationAsync(txHash);
            // if (!confirmed) {
            //    Debug.LogError("Transaction was not confirmed within the timeout period");
            //    return false;
            // }
            
            Debug.Log($"Purchased item: {item.Name} for {item.Price} tokens. Transaction hash: {txHash}");
            
            // Grant the item to the player
            GrantItemToPlayer(item);
            
            // Track the purchase in analytics
            TrackPurchase(item);
            
            return true;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error purchasing item: {e.Message}");
            return false;
        }
    }
    
    // Track purchase for analytics
    private void TrackPurchase(ShopItem item)
    {
        // Get reference to analytics if available
        EconomyAnalytics analytics = FindObjectOfType<EconomyAnalytics>();
        if (analytics != null)
        {
            analytics.TrackTokensSpent("shop_purchase", item.Price);
        }
        
        // You could also implement additional tracking here
        // For example, track specific item types purchased
        Dictionary<string, object> eventData = new Dictionary<string, object>
        {
            { "item_id", item.Id },
            { "item_name", item.Name },
            { "item_type", item.Type.ToString() },
            { "price", item.Price },
            { "currency", "RLY" }
        };
        
        // Send to your analytics system
        // AnalyticsService.LogEvent("item_purchased", eventData);
    }
    
    private void GrantItemToPlayer(ShopItem item)
    {
        // Implement based on your inventory system
        switch (item.Type)
        {
            case ShopItem.ItemType.Cosmetic:
                UnlockCosmetic(item.Id);
                break;
            case ShopItem.ItemType.Powerup:
                AddPowerupToInventory(item.Id);
                break;
            case ShopItem.ItemType.Currency:
                AddInGameCurrency(item.Id);
                break;
            case ShopItem.ItemType.Special:
                UnlockSpecialFeature(item.Id);
                break;
        }
        
        // Save player data
        SavePlayerData();
    }
    
    // Implement these methods based on your game's systems
    private void UnlockCosmetic(string id) { /* Implementation */ }
    private void AddPowerupToInventory(string id) { /* Implementation */ }
    private void AddInGameCurrency(string id) { /* Implementation */ }
    private void UnlockSpecialFeature(string id) { /* Implementation */ }
    private void SavePlayerData() { /* Implementation */ }
}
```

### Step 3: Create Shop UI

```csharp
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopUI : MonoBehaviour
{
    [SerializeField] private ShopManager shopManager;
    [SerializeField] private GameObject shopItemPrefab;
    [SerializeField] private Transform shopItemsContainer;
    [SerializeField] private Text balanceText;
    
    private IRallyNetwork rlyNetwork;
    
    private void Start()
    {
        // Get reference to RallyNetwork
        rlyNetwork = RallyUnityManager.Instance.RlyNetwork;
        
        // Initialize shop UI
        InitializeShop();
        
        // Update balance display
        UpdateBalanceDisplay();
    }
    
    private async void UpdateBalanceDisplay()
    {
        decimal balance = await rlyNetwork.GetBalanceAsync();
        balanceText.text = $"Balance: {balance} RLY";
    }
    
    private void InitializeShop()
    {
        // Clear existing items
        foreach (Transform child in shopItemsContainer)
        {
            Destroy(child.gameObject);
        }
        
        // Create new item entries
        foreach (ShopItem item in shopManager.AvailableItems)
        {
            GameObject itemObject = Instantiate(shopItemPrefab, shopItemsContainer);
            ShopItemUI itemUI = itemObject.GetComponent<ShopItemUI>();
            
            if (itemUI != null)
            {
                itemUI.Initialize(item, OnItemPurchaseRequested);
            }
        }
    }
    
    private async void OnItemPurchaseRequested(ShopItem item)
    {
        // Attempt purchase
        bool success = await shopManager.PurchaseItem(item);
        
        if (success)
        {
            // Show success message
            ShowPurchaseSuccessMessage(item);
            
            // Update balance display
            UpdateBalanceDisplay();
        }
        else
        {
            // Show failure message
            ShowPurchaseFailureMessage();
        }
    }
    
    private void ShowPurchaseSuccessMessage(ShopItem item)
    {
        // Implement UI notification
        Debug.Log($"Successfully purchased {item.Name}");
    }
    
    private void ShowPurchaseFailureMessage()
    {
        // Implement UI notification
        Debug.Log("Purchase failed - insufficient funds");
    }
}
```

## Part 3: Implementing Economy Balance

### Token Faucets and Sinks

To maintain a healthy token economy, implement both "faucets" (ways players earn tokens) and "sinks" (ways tokens are removed from circulation):

#### Faucets (already implemented above)

- Achievement rewards
- Daily login bonuses
- Quest completion
- Level-up rewards

#### Sinks (methods to remove tokens from circulation)

- Shop purchases
- Premium features
- Time-limited boosts
- Cosmetic upgrades

### Monitoring Economy Health

Create systems to monitor your economy's health:

```csharp
public class EconomyAnalytics : MonoBehaviour
{
    private Dictionary<string, decimal> tokensAwarded = new Dictionary<string, decimal>();
    private Dictionary<string, decimal> tokensSpent = new Dictionary<string, decimal>();
    
    public void TrackTokensAwarded(string reason, decimal amount)
    {
        if (tokensAwarded.ContainsKey(reason))
            tokensAwarded[reason] += amount;
        else
            tokensAwarded[reason] = amount;
            
        // Optional: Send analytics data to server
        SendAnalyticsData("token_award", new Dictionary<string, object>
        {
            { "reason", reason },
            { "amount", amount }
        });
    }
    
    public void TrackTokensSpent(string reason, decimal amount)
    {
        if (tokensSpent.ContainsKey(reason))
            tokensSpent[reason] += amount;
        else
            tokensSpent[reason] = amount;
            
        // Optional: Send analytics data to server
        SendAnalyticsData("token_spend", new Dictionary<string, object>
        {
            { "reason", reason },
            { "amount", amount }
        });
    }
    
    private void SendAnalyticsData(string eventName, Dictionary<string, object> data)
    {
        // Implement analytics data sending
        // This could be a custom server, Unity Analytics, or another service
    }
}
```

## Best Practices for Token Economies

1. **Balance Carefully**: Create a balanced ecosystem of token acquisition and spending
2. **Start Conservative**: Begin with lower token rewards and adjust upward based on data
3. **Create Meaningful Utility**: Ensure tokens have clear utility that players value
4. **Monitor Metrics**: Track token velocity, average holdings, and engagement metrics
5. **Limit Inflation**: Control the total supply of tokens in your economy
6. **Implement Sinks**: Create compelling ways for players to spend tokens
7. **Test Thoroughly**: Test your economy with real players before launch

## Conclusion

By following this tutorial, you've created a complete token economy for your Unity game using the Rally Protocol Unity SDK. You've implemented:

- Token rewards for achievements and daily login
- A shop system for token spending
- Economy monitoring tools

This foundation can be expanded with additional features like:

- Player-to-player trading
- NFT integration
- Token staking and rewards
- Seasonal events with special rewards

## Next Steps

- [Explore the trading tutorial](./implementing-p2p-trading.md) to add player-to-player trading
- Learn about [integrating NFTs](./implementing-nfts.md) in your game
- Discover [advanced economy mechanisms](./advanced-token-economy.md) for long-term sustainability

Remember to continuously monitor and adjust your economy based on player behavior and feedback to ensure a healthy, engaging environment.
