# Best Practices

This guide outlines best practices for using the Rally Protocol Unity SDK in your Unity projects.

## Application Architecture

### Initialization

- **Initialize early**: Initialize the Rally Protocol SDK early in your application lifecycle, ideally during a dedicated loading screen.

```csharp
public class GameBootstrapper : MonoBehaviour
{
    [SerializeField] private GameObject loadingScreen;
    
    private async void Start()
    {
        loadingScreen.SetActive(true);
        
        // Initialize Rally Protocol
        IRallyNetwork rlyNetwork = RallyUnityManager.Instance.RlyNetwork;
        
        // Check for existing account or create one
        await EnsureAccountExists(rlyNetwork.AccountManager);
        
        loadingScreen.SetActive(false);
    }
    
    private async Task EnsureAccountExists(IRallyAccountManager accountManager)
    {
        var account = await accountManager.GetAccountAsync();
        if (account == null)
        {
            await accountManager.CreateAccountAsync();
        }
    }
}
```

- **Use a service layer**: Create a service layer to abstract blockchain operations from your game logic.

```csharp
public class RallyService : MonoBehaviour
{
    private IRallyNetwork rlyNetwork;
    
    public void Initialize()
    {
        rlyNetwork = RallyUnityManager.Instance.RlyNetwork;
    }
    
    public async Task<decimal> GetPlayerBalance()
    {
        return await rlyNetwork.GetBalanceAsync();
    }
    
    public async Task<bool> RewardPlayer(decimal amount)
    {
        try
        {
            // Implementation for rewarding player
            return true;
        }
        catch
        {
            return false;
        }
    }
}
```

### Performance

- **Cache blockchain data**: Cache blockchain data to minimize network requests.

```csharp
public class TokenBalanceManager : MonoBehaviour
{
    private IRallyNetwork rlyNetwork;
    private decimal cachedBalance;
    private DateTime lastBalanceUpdate;
    
    // Only refresh balance if more than 30 seconds have passed
    private readonly TimeSpan balanceRefreshInterval = TimeSpan.FromSeconds(30);
    
    public async Task<decimal> GetBalance(bool forceRefresh = false)
    {
        if (forceRefresh || DateTime.Now - lastBalanceUpdate > balanceRefreshInterval)
        {
            cachedBalance = await rlyNetwork.GetBalanceAsync();
            lastBalanceUpdate = DateTime.Now;
        }
        
        return cachedBalance;
    }
}
```

- **Optimize transaction batching**: Batch transactions when possible rather than sending many small transactions.

- **UI interactions**: Don't block the UI during blockchain operations.

```csharp
// Bad: Blocking UI
public async void OnButtonClick()
{
    button.interactable = false;
    await rlyNetwork.ClaimRlyAsync(); // UI frozen during operation
    button.interactable = true;
}

// Good: Non-blocking UI with feedback
public async void OnButtonClick()
{
    button.interactable = false;
    loadingIndicator.SetActive(true);
    
    try
    {
        // Run on a background thread
        await Task.Run(async () => 
        {
            await rlyNetwork.ClaimRlyAsync();
        });
        
        statusText.text = "RLY claimed successfully!";
    }
    catch (Exception ex)
    {
        statusText.text = "Failed to claim RLY.";
        Debug.LogException(ex);
    }
    finally
    {
        loadingIndicator.SetActive(false);
        button.interactable = true;
    }
}
```

## Error Handling

- **Use specific exception handling**: Handle specific exceptions with appropriate responses.

```csharp
public async Task<bool> TryTransferTokens(string destinationAddress, decimal amount)
{
    try
    {
        await rlyNetwork.TransferAsync(destinationAddress, amount);
        return true;
    }
    catch (RallyAccountException)
    {
        // Account-related issues
        Debug.LogError("Account error. Please ensure you have a valid account.");
        return false;
    }
    catch (RallyNetworkException)
    {
        // Network-related issues
        Debug.LogError("Network error. Please check your internet connection.");
        return false;
    }
    catch (Exception ex)
    {
        // Other unexpected issues
        Debug.LogException(ex);
        return false;
    }
}
```

- **Provide user feedback**: Always provide clear feedback to users during blockchain operations.

```csharp
public async void OnTransferButtonClicked()
{
    statusText.text = "Initiating transfer...";
    
    try
    {
        string txHash = await rlyNetwork.TransferAsync(addressInput.text, decimal.Parse(amountInput.text));
        
        statusText.text = "Transfer successful!";
        transactionHashText.text = $"Transaction: {txHash}";
    }
    catch (Exception ex)
    {
        statusText.text = $"Transfer failed: {ex.Message}";
    }
}
```

## Security

- **Never hardcode API keys**: Store API keys securely, not in code.

```csharp
// Bad: Hardcoded API key
string apiKey = "your-api-key-here";

// Good: Load from settings
string apiKey = RallyUnityNetworkFactory.LoadMainSettingsPreset().ApiKey;
```

- **Validate user inputs**: Always validate user inputs before sending transactions.

```csharp
public bool ValidateTransferInputs(string address, string amountText)
{
    // Validate address format
    if (!address.StartsWith("0x") || address.Length != 42)
    {
        statusText.text = "Invalid Ethereum address format.";
        return false;
    }
    
    // Validate amount
    if (!decimal.TryParse(amountText, out decimal amount))
    {
        statusText.text = "Invalid amount format.";
        return false;
    }
    
    if (amount <= 0)
    {
        statusText.text = "Amount must be greater than zero.";
        return false;
    }
    
    return true;
}
```

- **Use cloud storage wisely**: Configure cloud storage options based on your security requirements.

```csharp
// High security (fails if cloud save fails)
await accountManager.CreateAccountAsync(new()
{
    StorageOptions = new()
    {
        SaveToCloud = true,
        RejectOnCloudSaveFailure = true
    }
});

// Flexible security (saves locally if cloud fails)
await accountManager.CreateAccountAsync(new()
{
    StorageOptions = new()
    {
        SaveToCloud = true,
        RejectOnCloudSaveFailure = false
    }
});
```

## Testing

- **Use test networks**: Always use test networks (BaseSepolia, Mumbai) during development.

```csharp
#if DEVELOPMENT_BUILD
    // Use test network
    IRallyNetwork rlyNetwork = RallyUnityNetworkFactory.Create(RallyNetworkType.BaseSepolia, apiKey);
#else
    // Use production network
    IRallyNetwork rlyNetwork = RallyUnityNetworkFactory.Create(RallyNetworkType.Base, apiKey);
#endif
```

- **Implement mock services**: Create mock services for testing without blockchain calls.

```csharp
public interface IRallyService
{
    Task<decimal> GetBalance();
    Task<string> ClaimTokens();
    Task<string> Transfer(string address, decimal amount);
}

public class RealRallyService : IRallyService
{
    private readonly IRallyNetwork rlyNetwork;
    
    // Real implementation
}

public class MockRallyService : IRallyService
{
    private decimal mockBalance = 100;
    
    public Task<decimal> GetBalance()
    {
        return Task.FromResult(mockBalance);
    }
    
    // Mock implementations for testing
}
```

## Mobile Considerations

- **Optimize for mobile**: Be mindful of battery and data usage on mobile devices.

- **Handle application lifecycle**: Properly handle application pause/resume events.

```csharp
public class RallyLifecycleHandler : MonoBehaviour
{
    private IRallyNetwork rlyNetwork;
    
    private void Awake()
    {
        rlyNetwork = RallyUnityManager.Instance.RlyNetwork;
    }
    
    private void OnApplicationPause(bool isPaused)
    {
        if (isPaused)
        {
            // Save any pending state
        }
        else
        {
            // Reconnect or refresh if needed
        }
    }
}
```

- **Platform-specific considerations**: Handle iOS and Android differences accordingly.

## Production Readiness

- **Adjust logging levels**: Reduce logging in production builds.

```csharp
private void SetupLogging()
{
#if DEBUG
    RallyUnityLogger.Default.UnityLogger.filterLogType = LogType.Log;
#else
    RallyUnityLogger.Default.UnityLogger.filterLogType = LogType.Warning;
#endif
}
```

- **Implement analytics**: Track blockchain operations for analytics and debugging.

- **Create a fallback mechanism**: Implement fallbacks for when blockchain operations fail.

```csharp
public async Task<bool> RewardPlayerWithFallback(decimal amount)
{
    try
    {
        // Try blockchain reward
        await tokenRewardService.RewardPlayer(amount);
        return true;
    }
    catch
    {
        // Fallback to in-game currency if blockchain fails
        gameEconomyService.AddVirtualCurrency(amount);
        pendingRewardsService.QueueBlockchainReward(amount); // Try again later
        return false;
    }
}
```

## Memory Management

- **Dispose of resources**: Ensure proper cleanup of resources.

- **Avoid memory leaks**: Be careful with event subscriptions and references.

```csharp
private void OnEnable()
{
    EventBus.Subscribe<WalletEvent>(OnWalletEvent);
}

private void OnDisable()
{
    EventBus.Unsubscribe<WalletEvent>(OnWalletEvent);
}
```

By following these best practices, you'll create more robust, performant, and user-friendly applications with the Rally Protocol Unity SDK.
