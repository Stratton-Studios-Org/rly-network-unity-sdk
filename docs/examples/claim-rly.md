# Claiming RLY Tokens Example

This example demonstrates how to implement RLY token claiming functionality in your Unity application.

## Overview

Claiming RLY tokens is a core feature of the Rally Protocol that allows users to receive tokens without needing to purchase them. This example provides a complete implementation of a token claiming system that you can integrate into your Unity project.

## Implementation

### Step 1: Create the Token Claim UI

First, set up a basic UI for the claiming functionality:

1. Create a new Canvas in your Unity scene
2. Add a button for claiming tokens
3. Add text elements for displaying balance and status messages

### Step 2: Create the Token Claim Script

Create a new C# script named `TokenClaimExample.cs` with the following code:

```csharp
using RallyProtocol.Unity;
using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class TokenClaimExample : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Button claimButton;
    [SerializeField] private Text balanceText;
    [SerializeField] private Text statusText;
    [SerializeField] private Text cooldownText;
    
    [Header("Configuration")]
    [SerializeField] private float refreshCooldownInterval = 10f; // in seconds
    
    private IRallyNetwork _rallyNetwork;
    private float _lastRefreshTime;
    
    private void Start()
    {
        // Initialize Rally Network reference
        _rallyNetwork = RallyUnityManager.Instance.RlyNetwork;
        
        // Set up UI events
        claimButton.onClick.AddListener(OnClaimButtonClicked);
        
        // Initial updates
        UpdateBalanceAsync();
        UpdateClaimStatusAsync();
    }
    
    private void Update()
    {
        // Refresh claim status periodically
        if (Time.time - _lastRefreshTime > refreshCooldownInterval)
        {
            _lastRefreshTime = Time.time;
            UpdateClaimStatusAsync();
        }
    }
    
    private async void OnClaimButtonClicked()
    {
        await ClaimTokensAsync();
    }
    
    private async Task ClaimTokensAsync()
    {
        // Disable button during claiming
        claimButton.interactable = false;
        
        try
        {
            // Update status
            UpdateStatus("Claiming RLY tokens...");
            
            // Check if the user can claim
            bool canClaim = await _rallyNetwork.CanClaimRlyAsync();
            if (!canClaim)
            {
                TimeSpan timeUntilNextClaim = await _rallyNetwork.GetTimeUntilNextClaimAsync();
                UpdateStatus($"Cannot claim now. Try again in {FormatTimeSpan(timeUntilNextClaim)}.");
                claimButton.interactable = true;
                return;
            }
            
            // Perform the claim operation
            string txHash = await _rallyNetwork.ClaimRlyAsync();
            
            // Update status with success message
            UpdateStatus($"RLY tokens claimed successfully! Transaction hash: {txHash}");
            
            // Update balance after claim
            await UpdateBalanceAsync();
            
            // Update claim status
            await UpdateClaimStatusAsync();
        }
        catch (Exception ex)
        {
            // Update status with error message
            UpdateStatus($"Failed to claim tokens: {ex.Message}");
            Debug.LogError($"Token claim error: {ex}");
        }
        finally
        {
            // Re-enable button
            claimButton.interactable = true;
        }
    }
    
    private async Task UpdateBalanceAsync()
    {
        try
        {
            decimal balance = await _rallyNetwork.GetBalanceAsync();
            balanceText.text = $"Balance: {balance} RLY";
        }
        catch (Exception ex)
        {
            balanceText.text = "Balance: Unable to fetch";
            Debug.LogError($"Failed to update balance: {ex.Message}");
        }
    }
    
    private async Task UpdateClaimStatusAsync()
    {
        try
        {
            bool canClaim = await _rallyNetwork.CanClaimRlyAsync();
            
            if (canClaim)
            {
                cooldownText.text = "Ready to claim!";
                claimButton.interactable = true;
            }
            else
            {
                TimeSpan timeUntilNextClaim = await _rallyNetwork.GetTimeUntilNextClaimAsync();
                cooldownText.text = $"Next claim available in: {FormatTimeSpan(timeUntilNextClaim)}";
                claimButton.interactable = false;
            }
        }
        catch (Exception ex)
        {
            cooldownText.text = "Claim status: Unable to fetch";
            Debug.LogError($"Failed to update claim status: {ex.Message}");
        }
    }
    
    private void UpdateStatus(string message)
    {
        statusText.text = message;
        Debug.Log(message);
    }
    
    private string FormatTimeSpan(TimeSpan timeSpan)
    {
        if (timeSpan.TotalHours >= 1)
        {
            return $"{(int)timeSpan.TotalHours}h {timeSpan.Minutes}m";
        }
        else if (timeSpan.TotalMinutes >= 1)
        {
            return $"{timeSpan.Minutes}m {timeSpan.Seconds}s";
        }
        else
        {
            return $"{timeSpan.Seconds}s";
        }
    }
}
```

### Step 3: Add Visual Feedback

To enhance the user experience, let's add a loading indicator while the claim operation is in progress:

```csharp
// Add this to the class member variables
[SerializeField] private GameObject loadingIndicator;

// Update the ClaimTokensAsync method
private async Task ClaimTokensAsync()
{
    // Disable button during claiming
    claimButton.interactable = false;
    
    // Show loading indicator
    loadingIndicator.SetActive(true);
    
    try
    {
        // Existing code...
    }
    catch (Exception ex)
    {
        // Existing code...
    }
    finally
    {
        // Hide loading indicator
        loadingIndicator.SetActive(false);
        
        // Re-enable button
        claimButton.interactable = true;
    }
}
```

## Advanced Implementation

### Adding Reward Animations

To make the claiming experience more engaging, you can add animations when tokens are successfully claimed:

```csharp
[SerializeField] private ParticleSystem rewardParticles;
[SerializeField] private AudioSource rewardSound;

// Update the ClaimTokensAsync method
private async Task ClaimTokensAsync()
{
    // ... existing code ...
    
    try
    {
        // ... existing code ...
        
        // Perform the claim operation
        string txHash = await _rallyNetwork.ClaimRlyAsync();
        
        // Play success animations and sounds
        if (rewardParticles != null)
            rewardParticles.Play();
            
        if (rewardSound != null)
            rewardSound.Play();
        
        // ... existing code ...
    }
    catch (Exception ex)
    {
        // ... existing code ...
    }
}
```

### Integrating with Game Achievement System

You might want to tie token claiming to game achievements or milestones:

```csharp
public async Task ClaimRewardForAchievementAsync(string achievementId)
{
    // Verify that the achievement is completed
    if (!GameAchievementManager.IsAchievementCompleted(achievementId))
    {
        UpdateStatus("Complete the achievement first!");
        return;
    }
    
    // Check if the achievement reward was already claimed
    if (GameAchievementManager.WasRewardClaimed(achievementId))
    {
        UpdateStatus("Reward already claimed for this achievement!");
        return;
    }
    
    // Claim tokens
    await ClaimTokensAsync();
    
    // Mark the achievement reward as claimed
    GameAchievementManager.MarkRewardAsClaimed(achievementId);
}
```

## Complete Example with Achievement Integration

Here's a more complete example integrating with a hypothetical achievement system:

```csharp
using RallyProtocol.Unity;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class AdvancedTokenClaimExample : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Button claimButton;
    [SerializeField] private Button claimAllButton;
    [SerializeField] private Text balanceText;
    [SerializeField] private Text statusText;
    [SerializeField] private GameObject loadingIndicator;
    [SerializeField] private Transform achievementListContainer;
    [SerializeField] private GameObject achievementItemPrefab;
    
    [Header("Effects")]
    [SerializeField] private ParticleSystem rewardParticles;
    [SerializeField] private AudioSource rewardSound;
    
    private IRallyNetwork _rallyNetwork;
    private Dictionary<string, AchievementItem> _achievementItems = new Dictionary<string, AchievementItem>();
    
    private void Start()
    {
        // Initialize Rally Network reference
        _rallyNetwork = RallyUnityManager.Instance.RlyNetwork;
        
        // Set up UI events
        claimButton.onClick.AddListener(OnClaimButtonClicked);
        claimAllButton.onClick.AddListener(OnClaimAllButtonClicked);
        
        // Initial updates
        UpdateBalanceAsync();
        PopulateAchievementList();
    }
    
    private void PopulateAchievementList()
    {
        // Clear existing items
        foreach (Transform child in achievementListContainer)
        {
            Destroy(child.gameObject);
        }
        _achievementItems.Clear();
        
        // Get achievements from the game system
        var achievements = GameAchievementManager.GetAllAchievements();
        
        // Create UI items for each achievement
        foreach (var achievement in achievements)
        {
            GameObject itemGO = Instantiate(achievementItemPrefab, achievementListContainer);
            AchievementItem item = itemGO.GetComponent<AchievementItem>();
            
            if (item != null)
            {
                item.Initialize(
                    achievement.Id,
                    achievement.Title,
                    achievement.Description,
                    achievement.TokenReward,
                    achievement.IsCompleted,
                    achievement.IsRewardClaimed
                );
                
                // Add claim button listener
                item.ClaimButton.onClick.AddListener(() => OnAchievementClaimButtonClicked(achievement.Id));
                
                // Store reference
                _achievementItems[achievement.Id] = item;
            }
        }
    }
    
    private async void OnClaimButtonClicked()
    {
        await ClaimTokensAsync();
    }
    
    private async void OnClaimAllButtonClicked()
    {
        await ClaimAllAchievementRewardsAsync();
    }
    
    private async void OnAchievementClaimButtonClicked(string achievementId)
    {
        await ClaimRewardForAchievementAsync(achievementId);
    }
    
    private async Task ClaimTokensAsync()
    {
        // Disable button during claiming
        claimButton.interactable = false;
        
        // Show loading indicator
        loadingIndicator.SetActive(true);
        
        try
        {
            // Update status
            UpdateStatus("Claiming RLY tokens...");
            
            // Check if the user can claim
            bool canClaim = await _rallyNetwork.CanClaimRlyAsync();
            if (!canClaim)
            {
                TimeSpan timeUntilNextClaim = await _rallyNetwork.GetTimeUntilNextClaimAsync();
                UpdateStatus($"Cannot claim now. Try again in {FormatTimeSpan(timeUntilNextClaim)}.");
                return;
            }
            
            // Perform the claim operation
            string txHash = await _rallyNetwork.ClaimRlyAsync();
            
            // Play success animations and sounds
            if (rewardParticles != null)
                rewardParticles.Play();
                
            if (rewardSound != null)
                rewardSound.Play();
            
            // Update status with success message
            UpdateStatus($"RLY tokens claimed successfully! Transaction hash: {txHash}");
            
            // Update balance after claim
            await UpdateBalanceAsync();
        }
        catch (Exception ex)
        {
            // Update status with error message
            UpdateStatus($"Failed to claim tokens: {ex.Message}");
            Debug.LogError($"Token claim error: {ex}");
        }
        finally
        {
            // Hide loading indicator
            loadingIndicator.SetActive(false);
            
            // Re-enable button
            claimButton.interactable = true;
        }
    }
    
    private async Task ClaimRewardForAchievementAsync(string achievementId)
    {
        // Get the achievement item
        if (!_achievementItems.TryGetValue(achievementId, out AchievementItem item))
        {
            Debug.LogError($"Achievement item not found for ID: {achievementId}");
            return;
        }
        
        // Check if the achievement is completed and not claimed
        if (!item.IsCompleted || item.IsRewardClaimed)
        {
            UpdateStatus(item.IsRewardClaimed 
                ? "Reward already claimed for this achievement!" 
                : "Complete the achievement first!");
            return;
        }
        
        // Disable the claim button
        item.ClaimButton.interactable = false;
        
        // Show loading indicator
        loadingIndicator.SetActive(true);
        
        try
        {
            // Update status
            UpdateStatus($"Claiming reward for {item.Title}...");
            
            // Claim the tokens
            await ClaimTokensAsync();
            
            // Mark the achievement reward as claimed
            GameAchievementManager.MarkRewardAsClaimed(achievementId);
            item.SetRewardClaimed(true);
            
            // Update status
            UpdateStatus($"Reward claimed for {item.Title}!");
        }
        catch (Exception ex)
        {
            // Update status with error message
            UpdateStatus($"Failed to claim reward: {ex.Message}");
            Debug.LogError($"Reward claim error: {ex}");
            
            // Re-enable the claim button
            item.ClaimButton.interactable = true;
        }
        finally
        {
            // Hide loading indicator
            loadingIndicator.SetActive(false);
        }
    }
    
    private async Task ClaimAllAchievementRewardsAsync()
    {
        // Disable claim all button
        claimAllButton.interactable = false;
        
        // Show loading indicator
        loadingIndicator.SetActive(true);
        
        try
        {
            // Get all completed but unclaimed achievements
            var unclaimedAchievements = GameAchievementManager.GetCompletedUnclaimedAchievements();
            
            if (unclaimedAchievements.Count == 0)
            {
                UpdateStatus("No unclaimed rewards available!");
                return;
            }
            
            // Update status
            UpdateStatus($"Claiming rewards for {unclaimedAchievements.Count} achievements...");
            
            // Claim rewards for each achievement
            foreach (var achievement in unclaimedAchievements)
            {
                await ClaimRewardForAchievementAsync(achievement.Id);
                
                // Small delay between claims to avoid overwhelming the network
                await Task.Delay(500);
            }
            
            // Update status
            UpdateStatus("All rewards claimed successfully!");
        }
        catch (Exception ex)
        {
            // Update status with error message
            UpdateStatus($"Failed to claim all rewards: {ex.Message}");
            Debug.LogError($"Claim all rewards error: {ex}");
        }
        finally
        {
            // Hide loading indicator
            loadingIndicator.SetActive(false);
            
            // Re-enable claim all button
            claimAllButton.interactable = true;
        }
    }
    
    private async Task UpdateBalanceAsync()
    {
        try
        {
            decimal balance = await _rallyNetwork.GetBalanceAsync();
            balanceText.text = $"Balance: {balance} RLY";
        }
        catch (Exception ex)
        {
            balanceText.text = "Balance: Unable to fetch";
            Debug.LogError($"Failed to update balance: {ex.Message}");
        }
    }
    
    private void UpdateStatus(string message)
    {
        statusText.text = message;
        Debug.Log(message);
    }
    
    private string FormatTimeSpan(TimeSpan timeSpan)
    {
        if (timeSpan.TotalHours >= 1)
        {
            return $"{(int)timeSpan.TotalHours}h {timeSpan.Minutes}m";
        }
        else if (timeSpan.TotalMinutes >= 1)
        {
            return $"{timeSpan.Minutes}m {timeSpan.Seconds}s";
        }
        else
        {
            return $"{timeSpan.Seconds}s";
        }
    }
}

// Achievement Item UI Component
public class AchievementItem : MonoBehaviour
{
    [SerializeField] private Text titleText;
    [SerializeField] private Text descriptionText;
    [SerializeField] private Text rewardText;
    [SerializeField] private Button claimButton;
    [SerializeField] private GameObject completedIcon;
    [SerializeField] private GameObject claimedIcon;
    
    public string Id { get; private set; }
    public string Title => titleText.text;
    public bool IsCompleted { get; private set; }
    public bool IsRewardClaimed { get; private set; }
    public Button ClaimButton => claimButton;
    
    public void Initialize(string id, string title, string description, decimal reward, bool isCompleted, bool isRewardClaimed)
    {
        Id = id;
        titleText.text = title;
        descriptionText.text = description;
        rewardText.text = $"+{reward} RLY";
        
        IsCompleted = isCompleted;
        IsRewardClaimed = isRewardClaimed;
        
        // Update UI state
        UpdateState();
    }
    
    public void SetRewardClaimed(bool claimed)
    {
        IsRewardClaimed = claimed;
        UpdateState();
    }
    
    private void UpdateState()
    {
        // Update completion status
        completedIcon.SetActive(IsCompleted);
        
        // Update claim status
        claimedIcon.SetActive(IsRewardClaimed);
        
        // Update claim button
        claimButton.interactable = IsCompleted && !IsRewardClaimed;
    }
}
```

## Unity Scene Setup

Here's how to set up your Unity scene to implement this example:

1. Create a new Canvas with UI Scale Mode set to "Scale With Screen Size"
2. Add the following UI elements:
   - Panel (background)
   - Text for title ("Claim RLY Tokens")
   - Text for balance display
   - Text for status messages
   - Text for cooldown timer
   - Button for claiming tokens
   - Loading indicator (can be a simple spinner animation)
3. Add a Particle System for the reward animation (optional)
4. Add an Audio Source for sound effects (optional)
5. Attach the `TokenClaimExample` script to a GameObject in your scene
6. Assign all the UI references in the Inspector

## Testing

To test the token claiming functionality:

1. Ensure your Rally Protocol Unity SDK is properly initialized
2. Ensure you have a valid API key configured
3. Run your Unity scene
4. Click the "Claim" button to claim tokens
5. Verify that the balance updates after claiming
6. Check the cooldown functionality by trying to claim again

## Best Practices

### Performance

- **Minimize Network Calls**: Avoid making frequent API calls for status checks
- **Cache Results**: Store claim status and only refresh periodically
- **Handle Background/Foreground Transitions**: Refresh data when the app returns to the foreground

### User Experience

- **Clear Feedback**: Always provide clear feedback about claim status
- **Helpful Error Messages**: Display user-friendly error messages
- **Visual Rewards**: Add animations and sound effects for successful claims
- **Progressive Disclosure**: Show advanced options only when relevant

### Security

- **Verify Transactions**: Always verify transaction success before updating UI
- **Rate Limiting**: Implement client-side rate limiting to prevent spam
- **Secure Storage**: Ensure wallet information is securely stored

## Next Steps

After implementing token claiming, consider exploring these related features:

- [Token Transfers](./token-transfers.md)
- [Balance Checking](./balance-checking.md)
- [Transaction History](./transaction-history.md)

## Related Documentation

- [Token Claiming](../features/token-claiming.md)
- [Gasless Transaction Settings](../features/gasless-settings.md)
- [RLY Token Integration](../features/rly-token.md)
