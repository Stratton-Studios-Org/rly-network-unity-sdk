# Token Claiming

This guide provides detailed information about claiming RLY tokens using the Rally Protocol Unity SDK.

## Overview

Token claiming is a fundamental feature of the Rally Protocol ecosystem that allows users to receive RLY tokens. These tokens can be used for various purposes within your application, such as rewards, in-app purchases, or as part of your game economy.

## How Token Claiming Works

The Rally Protocol uses a faucet mechanism to distribute RLY tokens to users. This process is designed to be simple and gas-free, allowing users to claim tokens without needing ETH for gas fees.

When a user claims tokens:

1. The SDK sends a claim request to the Rally Protocol backend
2. The backend verifies the user's eligibility
3. If eligible, tokens are transferred to the user's wallet
4. The transaction is processed using the Gas Station Network (GSN), making it free for the user

## Basic Implementation

Here's how to implement token claiming in your Unity application:

```csharp
using RallyProtocol.Unity;
using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class TokenClaimManager : MonoBehaviour
{
    [SerializeField] private Button claimButton;
    [SerializeField] private Text statusText;
    [SerializeField] private Text balanceText;

    private IRallyNetwork _rallyNetwork;

    private void Start()
    {
        // Get a reference to the Rally Network
        _rallyNetwork = RallyUnityManager.Instance.RlyNetwork;
        
        // Set up the UI button
        claimButton.onClick.AddListener(OnClaimButtonClicked);
        
        // Initial balance check
        UpdateBalanceAsync();
    }

    private async void OnClaimButtonClicked()
    {
        // Disable the button while claiming
        claimButton.interactable = false;
        UpdateStatus("Claiming tokens...");
        
        try
        {
            // Claim tokens
            string txHash = await _rallyNetwork.ClaimRlyAsync();
            
            UpdateStatus($"Tokens claimed successfully! Transaction hash: {txHash}");
            
            // Update the balance after claiming
            await UpdateBalanceAsync();
        }
        catch (Exception ex)
        {
            UpdateStatus($"Failed to claim tokens: {ex.Message}");
            Debug.LogError($"Token claim error: {ex}");
        }
        finally
        {
            // Re-enable the button
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
            Debug.LogError($"Failed to update balance: {ex.Message}");
        }
    }

    private void UpdateStatus(string message)
    {
        statusText.text = message;
        Debug.Log(message);
    }
}
```

## Advanced Configuration

### Custom Claim Amounts

By default, the Rally Protocol determines the amount of tokens to distribute. However, you can configure custom claim amounts using the Rally Developer Dashboard. This allows you to control the token economy in your application.

### Claim Frequency

There are built-in rate limits for claiming tokens to prevent abuse. These limits can be configured in the Rally Developer Dashboard:

- Claim cooldown period (how often a user can claim)
- Daily claim limits
- Total claim limits

### Claim Eligibility

You can implement custom eligibility criteria for token claiming. For example:

- Only allow claiming after completing specific in-game achievements
- Require a minimum level or score
- Implement a time-based reward system

```csharp
public async Task<bool> CanUserClaimTokensAsync()
{
    try
    {
        // Check if the user meets your custom criteria
        bool meetsGameCriteria = CheckUserAchievements();
        
        // Check if the user is within the Rally Protocol claim limits
        bool isEligibleForClaim = await _rallyNetwork.CanClaimRlyAsync();
        
        return meetsGameCriteria && isEligibleForClaim;
    }
    catch (Exception ex)
    {
        Debug.LogError($"Error checking claim eligibility: {ex.Message}");
        return false;
    }
}

private bool CheckUserAchievements()
{
    // Implement your custom logic here
    // For example, check if the user has completed certain tasks
    // return GameProgressManager.Instance.HasCompletedRequiredAchievements();
    
    // Placeholder implementation
    return true;
}
```

## Error Handling

When implementing token claiming, consider these potential error scenarios:

### Common Error Scenarios

1. **Network Connectivity Issues**
   - Handle timeout errors with appropriate retry logic
   - Provide offline indicator when network is unavailable

2. **Rate Limit Exceeded**
   - Check if the user can claim before attempting
   - Display the remaining time until the next claim is available

3. **Account Issues**
   - Ensure the user has a valid wallet before claiming
   - Handle account creation errors gracefully

### Example Error Handling Implementation

```csharp
public async Task ClaimTokensWithRetryAsync(int maxRetries = 3)
{
    int attempts = 0;
    bool success = false;
    
    while (!success && attempts < maxRetries)
    {
        attempts++;
        try
        {
            UpdateStatus($"Claiming tokens (attempt {attempts}/{maxRetries})...");
            
            // Check if claiming is possible
            if (!await _rallyNetwork.CanClaimRlyAsync())
            {
                // Get time until next claim is available
                TimeSpan timeUntilNextClaim = await _rallyNetwork.GetTimeUntilNextClaimAsync();
                UpdateStatus($"Claim not available. Try again in {timeUntilNextClaim.TotalMinutes:0} minutes.");
                return;
            }
            
            // Attempt to claim
            string txHash = await _rallyNetwork.ClaimRlyAsync();
            UpdateStatus($"Tokens claimed successfully! Transaction hash: {txHash}");
            
            // Update the balance after claiming
            await UpdateBalanceAsync();
            
            success = true;
        }
        catch (RallyNetworkException ex)
        {
            if (ex.ErrorCode == RallyErrorCodes.RateLimitExceeded)
            {
                UpdateStatus("Rate limit exceeded. Try again later.");
                return;
            }
            else if (attempts >= maxRetries)
            {
                UpdateStatus($"Failed to claim tokens after {maxRetries} attempts: {ex.Message}");
                throw;
            }
            else
            {
                // Wait before retrying
                await Task.Delay(1000 * attempts); // Exponential backoff
            }
        }
        catch (Exception ex)
        {
            UpdateStatus($"Failed to claim tokens: {ex.Message}");
            throw;
        }
    }
}
```

## Best Practices

### Integration with Game Design

- **Make claiming meaningful**: Tie token claiming to meaningful in-game actions or achievements
- **Progressive rewards**: Consider implementing a progressive reward system where claim amounts increase with user engagement
- **Balanced economy**: Design your token economy carefully to maintain balance and avoid inflation

### User Experience

- **Visual feedback**: Provide clear visual feedback during the claiming process
- **Transaction history**: Show users their claim history and current balance
- **Onboarding**: Guide new users through the claiming process as part of onboarding

### Performance Considerations

- **Asynchronous operations**: All token operations should be handled asynchronously to avoid blocking the main thread
- **Loading indicators**: Show loading indicators during network operations
- **Caching**: Cache balance and eligibility information to reduce network calls

## Platform-Specific Considerations

### Mobile Platforms

- **Battery usage**: Minimize network calls to conserve battery
- **Offline handling**: Implement graceful offline handling for mobile users
- **Background processing**: Consider how token operations will behave when the app is in the background

### WebGL

- **Browser limitations**: Be aware of browser-specific limitations for WebGL builds
- **Session persistence**: Implement appropriate session management for web users

## Troubleshooting

### Common Issues and Solutions

- **"Cannot claim tokens at this time"**: Check rate limits and eligibility criteria
- **Transaction stuck pending**: Ensure network connectivity and wait for network confirmation
- **Zero balance after claiming**: Verify the transaction was successful and check for network delays

## Next Steps

After implementing token claiming, consider integrating these related features:

- [Token Transfers](./token-transfers.md) - Allow users to transfer tokens to other addresses
- [Balance Checking](./balance-checking.md) - Display token balances to users
- [Transaction History](./transaction-history.md) - Show users their claiming and transaction history

## Related Documentation

- [RLY Token Integration](./rly-token.md)
- [Gasless Transaction Settings](./gasless-settings.md)
- [Network Configuration](./network-configuration.md) 