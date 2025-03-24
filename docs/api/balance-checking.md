# Balance Checking API

This document provides detailed information about the balance checking API in the Rally Protocol Unity SDK.

## Overview

The balance checking API allows you to retrieve token balances for a user's account. This is essential for displaying current balances, validating if a user can perform a transaction, and tracking balance changes over time.

## API Reference

### IRallyNetwork Methods

The following methods related to balance checking are available through the `IRallyNetwork` interface:

#### GetDisplayBalanceAsync

Retrieves the token balance for the current account in a human-readable decimal format.

```csharp
Task<decimal> GetDisplayBalanceAsync(string? tokenAddress = null);
```

##### Parameters

- `tokenAddress` (optional): The address of the token contract. If null, the RLY token address is used.

##### Returns

`Task<decimal>`: A task that resolves to the decimal balance of tokens.

##### Exceptions

- `RallyNetworkException`: Thrown when there's an error communicating with the Rally Protocol network.
- `RallyAccountException`: Thrown when there's no active account or the account is not properly initialized.

##### Example

```csharp
using RallyProtocol.Networks;
using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class BalanceManager : MonoBehaviour
{
    [SerializeField] private Text balanceText;
    [SerializeField] private Button refreshButton;
    
    private IRallyNetwork rallyNetwork;
    
    private void Start()
    {
        // Get a reference to the Rally Network (depends on your initialization)
        // rallyNetwork = ...
        
        // Set up the refresh button
        refreshButton.onClick.AddListener(OnRefreshButtonClicked);
        
        // Initial balance check
        UpdateBalanceAsync();
    }
    
    private async void OnRefreshButtonClicked()
    {
        await UpdateBalanceAsync();
    }
    
    private async Task UpdateBalanceAsync()
    {
        try
        {
            balanceText.text = "Checking balance...";
            
            // Get the current RLY balance
            decimal balance = await rallyNetwork.GetDisplayBalanceAsync();
            
            // Update the UI
            balanceText.text = $"Balance: {balance} RLY";
            
            // Example: Get the balance of a custom token
            // string customTokenAddress = "0x1234567890123456789012345678901234567890";
            // decimal customBalance = await rallyNetwork.GetDisplayBalanceAsync(customTokenAddress);
            // Debug.Log($"Custom token balance: {customBalance}");
        }
        catch (Exception ex)
        {
            balanceText.text = "Error checking balance";
            Debug.LogError($"Failed to check balance: {ex.Message}");
        }
    }

#### GetExactBalanceAsync

Retrieves the exact token balance for the current account as a BigInteger, without any decimal formatting.

```csharp
Task<BigInteger> GetExactBalanceAsync(string? tokenAddress = null);
```

##### Parameters

- `tokenAddress` (optional): The address of the token contract. If null, the RLY token address is used.

##### Returns

`Task<BigInteger>`: A task that resolves to the exact balance as a BigInteger.

##### Exceptions

- `RallyNetworkException`: Thrown when there's an error communicating with the Rally Protocol network.
- `RallyAccountException`: Thrown when there's no active account or the account is not properly initialized.

##### Example

```csharp
using RallyProtocol.Networks;
using System;
using System.Numerics;
using System.Threading.Tasks;
using UnityEngine;

public class ExactBalanceExample : MonoBehaviour
{
    private IRallyNetwork rallyNetwork;
    
    private async Task DisplayExactBalanceAsync()
    {
        try
        {
            // Get the exact balance
            BigInteger exactBalance = await rallyNetwork.GetExactBalanceAsync();
            
            // Display the exact balance
            Debug.Log($"Exact balance (in smallest units): {exactBalance}");
            
            // Convert to decimal if needed for display
            // Note: 18 decimal places is common for ERC-20 tokens, but can vary
            decimal displayBalance = (decimal)exactBalance / (decimal)Math.Pow(10, 18);
            Debug.Log($"Converted value: {displayBalance}");
            
            // Example with custom token
            // string customTokenAddress = "0x1234567890123456789012345678901234567890";
            // BigInteger customExactBalance = await rallyNetwork.GetExactBalanceAsync(customTokenAddress);
            // Debug.Log($"Custom token exact balance: {customExactBalance}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to get exact balance: {ex.Message}");
        }
    }
}
```

### RallyUnityManager Methods

The `RallyUnityManager` singleton provides convenient access to the Rally Network:

```csharp
// Get the RallyUnityManager instance
var manager = RallyUnityManager.Instance;

// Get the IRallyNetwork instance
IRallyNetwork network = manager.RlyNetwork;

// Now you can call the balance checking methods
decimal balance = await network.GetDisplayBalanceAsync();
```

## Best Practices

### Caching Balances

For performance optimization, consider caching balance results and refreshing them only when necessary:

```csharp
public class OptimizedBalanceManager : MonoBehaviour
{
    [SerializeField] private Text balanceText;
    [SerializeField] private float refreshInterval = 60f; // seconds
    
    private IRallyNetwork rallyNetwork;
    private decimal cachedBalance;
    private float lastRefreshTime;
    
    private void Start()
    {
        // Get a reference to the Rally Network (depends on your initialization)
        // rallyNetwork = ...
        
        lastRefreshTime = -refreshInterval; // Force initial update
        
        // Initial balance check
        UpdateBalanceIfNeededAsync();
    }
    
    private void Update()
    {
        // Check if it's time to refresh
        if (Time.time - lastRefreshTime >= refreshInterval)
        {
            UpdateBalanceIfNeededAsync();
        }
    }
    
    private async void UpdateBalanceIfNeededAsync()
    {
        try
        {
            lastRefreshTime = Time.time;
            
            // Get the current balance
            decimal balance = await rallyNetwork.GetDisplayBalanceAsync();
            
            // Only update UI if the balance has changed
            if (balance != cachedBalance)
            {
                cachedBalance = balance;
                balanceText.text = $"Balance: {balance} RLY";
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to update balance: {ex.Message}");
        }
    }
    
    // Call this method when you know the balance may have changed
    // (e.g., after a transfer or claim operation)
    public void ForceBalanceUpdate()
    {
        lastRefreshTime = -refreshInterval; // Force update on next Update
    }
}
```

### Balance Formatting

Consider formatting the balance to make it more user-friendly:

```csharp
private string FormatBalance(decimal balance)
{
    // For small balances, show more decimal places
    if (balance < 0.1m)
    {
        return balance.ToString("0.####") + " RLY";
    }
    // For medium balances
    else if (balance < 1000m)
    {
        return balance.ToString("0.##") + " RLY";
    }
    // For large balances
    else
    {
        return balance.ToString("#,##0.##") + " RLY";
    }
}
```

### Handling Multiple Token Types

When your application deals with multiple token types, consider creating a more comprehensive token management system:

```csharp
public class TokenManager : MonoBehaviour
{
    // Define known tokens
    private Dictionary<string, TokenInfo> tokens = new Dictionary<string, TokenInfo>
    {
        { "RLY", new TokenInfo { Name = "Rally", Symbol = "RLY", Decimals = 18, Address = null } },
        { "CUSTOM", new TokenInfo { Name = "Custom Token", Symbol = "CUSTOM", Decimals = 18, Address = "0x1234567890123456789012345678901234567890" } }
    };
    
    private IRallyNetwork rallyNetwork;
    
    private async Task<decimal> GetTokenBalanceAsync(string tokenSymbol)
    {
        if (!tokens.TryGetValue(tokenSymbol, out TokenInfo token))
        {
            throw new ArgumentException($"Unknown token symbol: {tokenSymbol}");
        }
        
        return await rallyNetwork.GetDisplayBalanceAsync(token.Address);
    }
    
    private class TokenInfo
    {
        public string Name { get; set; }
        public string Symbol { get; set; }
        public int Decimals { get; set; }
        public string Address { get; set; }
    }
}
```

### Error Handling

Always implement proper error handling for balance checking operations:

```csharp
public async Task<decimal> SafeGetBalanceAsync()
{
    try
    {
        return await rallyNetwork.GetDisplayBalanceAsync();
    }
    catch (Exception ex)
    {
        // Log the error
        Debug.LogError($"Error getting balance: {ex.Message}");
        
        // Return a fallback value
        return -1; // Or any other indicator value
    }
}
```

## Balance Change Events

To monitor balance changes, you can implement a polling mechanism or check the balance after known operations:

```csharp
public class BalanceChangeMonitor : MonoBehaviour
{
    [SerializeField] private float checkInterval = 5f;
    
    private IRallyNetwork rallyNetwork;
    private decimal lastKnownBalance;
    private bool isInitialized;
    
    // Event that fires when the balance changes
    public event Action<decimal, decimal> OnBalanceChanged; // (oldBalance, newBalance)
    
    private void Start()
    {
        // Get a reference to the Rally Network (depends on your initialization)
        // rallyNetwork = ...
        StartCoroutine(MonitorBalanceChanges());
    }
    
    private IEnumerator MonitorBalanceChanges()
    {
        while (true)
        {
            try
            {
                decimal currentBalance = await rallyNetwork.GetDisplayBalanceAsync();
                
                if (isInitialized && currentBalance != lastKnownBalance)
                {
                    // Balance has changed
                    OnBalanceChanged?.Invoke(lastKnownBalance, currentBalance);
                }
                
                lastKnownBalance = currentBalance;
                isInitialized = true;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error monitoring balance changes: {ex.Message}");
            }
            
            yield return new WaitForSeconds(checkInterval);
        }
    }
}
```

## Related Documentation

- [IRallyNetwork API Reference](./IRallyNetwork.md)
- [Token Operations](../features/token-operations.md)
- [Token Transfers](../examples/token-transfers.md)
- [Token Claiming](../examples/claim-rly.md)
