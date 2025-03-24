# Integrating Rally Protocol with an Existing Game

This example demonstrates how to integrate the Rally Protocol into an existing Unity game with minimal changes to your current architecture.

## Overview

When integrating blockchain functionality into an established game, you'll typically want to:

1. Keep existing systems intact
2. Add blockchain functionality as a layer on top
3. Create adapter patterns to connect blockchain operations with game systems
4. Ensure backward compatibility

This guide demonstrates patterns for smooth integration that respect your existing codebase.

## Prerequisites

- An existing Unity game with its own currency/economy system
- Rally Protocol Unity SDK installed in your project
- Basic understanding of how your game manages player data and currency

## Step 1: Create a Bridge Layer

First, create an interface layer that will bridge your existing game systems with Rally Protocol:

```csharp
using RallyProtocol.Unity;
using System.Threading.Tasks;
using UnityEngine;

// This class acts as a bridge between your existing game systems and Rally Protocol
public class RallyGameBridge : MonoBehaviour
{
    // Singleton pattern for easy access
    public static RallyGameBridge Instance { get; private set; }
    
    // Reference to your existing game economy manager
    [SerializeField] private GameEconomyManager gameEconomyManager;
    
    // Reference to the Rally Protocol network interface
    private IRallyNetwork rlyNetwork;
    
    // Configuration
    [SerializeField] private bool rallyFeaturesEnabled = true;
    [SerializeField] private float gameToRallyConversionRate = 10.0f; // Example: 10 game coins = 1 RLY
    
    private void Awake()
    {
        // Singleton setup
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    
    private void Start()
    {
        // Initialize Rally Protocol if enabled
        if (rallyFeaturesEnabled)
        {
            InitializeRallyProtocol();
        }
    }
    
    private void InitializeRallyProtocol()
    {
        // Get reference to RallyNetwork
        rlyNetwork = RallyUnityManager.Instance.RlyNetwork;
        
        // Check if an account exists, if not we'll need to create one
        CheckAccountStatus();
    }
    
    private async void CheckAccountStatus()
    {
        try
        {
            // Check if we have an account
            bool hasAccount = await rlyNetwork.AccountManager.HasAccountAsync();
            
            if (!hasAccount)
            {
                // We could auto-create an account or prompt the user
                Debug.Log("No Rally account found. User should create one before using Rally features.");
            }
            else
            {
                Debug.Log("Rally account exists and is ready to use.");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error checking account status: {e.Message}");
        }
    }
    
    // Public methods that your game can call
    
    public async Task<bool> ConvertGameCurrencyToRallyTokens(int gameCurrencyAmount)
    {
        if (!rallyFeaturesEnabled || rlyNetwork == null)
        {
            Debug.LogWarning("Rally features are not enabled.");
            return false;
        }
        
        try
        {
            // Calculate the amount of RLY tokens based on the conversion rate
            decimal rlyAmount = (decimal)(gameCurrencyAmount / gameToRallyConversionRate);
            
            // First, check if we have an account
            bool hasAccount = await rlyNetwork.AccountManager.HasAccountAsync();
            if (!hasAccount)
            {
                Debug.LogError("Cannot convert currency: No Rally account exists.");
                return false;
            }
            
            // This part would need to integrate with your game's economy system
            // For example, deducting the game currency from the player's balance
            bool deductedSuccessfully = gameEconomyManager.TryDeductCurrency(gameCurrencyAmount);
            
            if (!deductedSuccessfully)
            {
                Debug.LogError("Failed to deduct game currency.");
                return false;
            }
            
            // In a real implementation, you would likely call your backend to mint/transfer tokens
            // This is just a placeholder for demonstration purposes
            Debug.Log($"Successfully converted {gameCurrencyAmount} game currency to {rlyAmount} RLY tokens");
            
            // Update UI or notify other systems
            EventManager.TriggerEvent("CurrencyConverted", new { GameAmount = gameCurrencyAmount, RlyAmount = rlyAmount });
            
            return true;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error converting currency: {e.Message}");
            return false;
        }
    }
    
    public async Task<decimal> GetRallyTokenBalance()
    {
        if (!rallyFeaturesEnabled || rlyNetwork == null)
        {
            Debug.LogWarning("Rally features are not enabled.");
            return 0;
        }
        
        try
        {
            // Check if we have an account
            bool hasAccount = await rlyNetwork.AccountManager.HasAccountAsync();
            if (!hasAccount)
            {
                Debug.LogError("Cannot get balance: No Rally account exists.");
                return 0;
            }
            
            // Get the RLY token balance
            decimal balance = await rlyNetwork.GetBalanceAsync();
            return balance;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error getting balance: {e.Message}");
            return 0;
        }
    }
}
```

## Step 2: Add UI for Rally Protocol Features

Now, create a UI that allows players to interact with the Rally Protocol features without disrupting the existing game UI:

```csharp
using RallyProtocol.Unity;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class RallyFeaturesUI : MonoBehaviour
{
    // References to UI elements
    [SerializeField] private GameObject rallyPanel;
    [SerializeField] private Text balanceText;
    [SerializeField] private Button createAccountButton;
    [SerializeField] private Button convertCurrencyButton;
    [SerializeField] private InputField conversionAmountInput;
    [SerializeField] private Text conversionRateText;
    [SerializeField] private Button closeButton;
    
    // Reference to the Rally Game Bridge
    private RallyGameBridge rallyBridge;
    private IRallyNetwork rlyNetwork;
    
    private void Start()
    {
        // Get references
        rallyBridge = RallyGameBridge.Instance;
        rlyNetwork = RallyUnityManager.Instance.RlyNetwork;
        
        // Set up UI event handlers
        createAccountButton.onClick.AddListener(OnCreateAccountClicked);
        convertCurrencyButton.onClick.AddListener(OnConvertCurrencyClicked);
        closeButton.onClick.AddListener(() => rallyPanel.SetActive(false));
        
        // Display conversion rate
        UpdateConversionRateText();
        
        // Update the UI based on whether the user has an account
        UpdateUIBasedOnAccountStatus();
    }
    
    private void OnEnable()
    {
        // Refresh UI when panel becomes visible
        UpdateUIBasedOnAccountStatus();
        UpdateBalanceText();
    }
    
    private void UpdateConversionRateText()
    {
        // This would need to pull the current rate from your RallyGameBridge or configuration
        float conversionRate = 10.0f; // Example: 10 game coins = 1 RLY
        conversionRateText.text = $"Conversion Rate: {conversionRate} Game Coins = 1 RLY Token";
    }
    
    private async void UpdateUIBasedOnAccountStatus()
    {
        try
        {
            bool hasAccount = await rlyNetwork.AccountManager.HasAccountAsync();
            
            // Show/hide appropriate UI elements based on account status
            createAccountButton.gameObject.SetActive(!hasAccount);
            convertCurrencyButton.gameObject.SetActive(hasAccount);
            conversionAmountInput.gameObject.SetActive(hasAccount);
            balanceText.gameObject.SetActive(hasAccount);
            
            if (hasAccount)
            {
                UpdateBalanceText();
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error checking account status: {e.Message}");
        }
    }
    
    private async void UpdateBalanceText()
    {
        decimal balance = await rallyBridge.GetRallyTokenBalance();
        balanceText.text = $"RLY Balance: {balance}";
    }
    
    private async void OnCreateAccountClicked()
    {
        try
        {
            // Create a new account
            await rlyNetwork.AccountManager.CreateAccountAsync(new()
            {
                Overwrite = false,
                StorageOptions = new()
                {
                    SaveToCloud = true,
                    RejectOnCloudSaveFailure = false
                }
            });
            
            // Update UI to reflect the new account
            UpdateUIBasedOnAccountStatus();
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error creating account: {e.Message}");
        }
    }
    
    private async void OnConvertCurrencyClicked()
    {
        if (string.IsNullOrEmpty(conversionAmountInput.text))
        {
            // Show error message to user
            Debug.LogError("Please enter an amount to convert");
            return;
        }
        
        if (!int.TryParse(conversionAmountInput.text, out int amountToConvert))
        {
            // Show error message to user
            Debug.LogError("Please enter a valid number");
            return;
        }
        
        // Attempt the conversion
        bool success = await rallyBridge.ConvertGameCurrencyToRallyTokens(amountToConvert);
        
        if (success)
        {
            // Clear the input field
            conversionAmountInput.text = "";
            
            // Update the balance display
            UpdateBalanceText();
            
            // Show success message
            Debug.Log($"Successfully converted {amountToConvert} game currency to RLY tokens");
        }
        else
        {
            // Show error message
            Debug.LogError("Conversion failed");
        }
    }
}
```

## Step 3: Integrate with Existing Game Economy

Now, let's adapt your existing game economy manager to work with the Rally Protocol:

```csharp
// Assume this is your existing game economy manager class
public class GameEconomyManager : MonoBehaviour
{
    // Your existing properties
    private int playerCurrency = 1000;
    
    // Event to notify other systems of currency changes
    public event System.Action<int> OnCurrencyChanged;
    
    // Your existing methods
    public int GetPlayerCurrency()
    {
        return playerCurrency;
    }
    
    public void AddCurrency(int amount)
    {
        playerCurrency += amount;
        OnCurrencyChanged?.Invoke(playerCurrency);
    }
    
    // New method for Rally Protocol integration
    public bool TryDeductCurrency(int amount)
    {
        if (playerCurrency >= amount)
        {
            playerCurrency -= amount;
            OnCurrencyChanged?.Invoke(playerCurrency);
            return true;
        }
        
        return false;
    }
    
    // You might add new methods for Rally-specific functionality
    public void ShowRallyFeaturesUI()
    {
        // Find and show the Rally Features UI panel
        RallyFeaturesUI rallyUI = FindObjectOfType<RallyFeaturesUI>(true);
        if (rallyUI != null)
        {
            rallyUI.gameObject.SetActive(true);
        }
    }
}
```

## Step 4: Add Entry Points to Your Game

Finally, add natural entry points in your game for players to access Rally Protocol features:

```csharp
// Example integration in a shop or currency management screen
public class GameShopUI : MonoBehaviour
{
    [SerializeField] private Button openRallyFeaturesButton;
    [SerializeField] private GameEconomyManager economyManager;
    
    private void Start()
    {
        // Add button to open Rally features
        openRallyFeaturesButton.onClick.AddListener(OnOpenRallyFeaturesClicked);
    }
    
    private void OnOpenRallyFeaturesClicked()
    {
        // Use your existing game economy manager to show the Rally features UI
        economyManager.ShowRallyFeaturesUI();
    }
}
```

## Optional: Progressive Integration Pattern

For seamless integration that doesn't disrupt existing players, consider a progressive approach:

```csharp
public class RallyProgressiveIntegration : MonoBehaviour
{
    [SerializeField] private bool rallyFeaturesEnabled = false;
    [SerializeField] private GameObject rallyFeaturesTeaserPanel;
    [SerializeField] private Button enableRallyFeaturesButton;
    
    // Player preference key
    private const string RALLY_FEATURES_PREF_KEY = "RallyFeaturesEnabled";
    
    private void Start()
    {
        // Load user preference
        rallyFeaturesEnabled = PlayerPrefs.GetInt(RALLY_FEATURES_PREF_KEY, 0) == 1;
        
        // Set up UI
        rallyFeaturesTeaserPanel.SetActive(!rallyFeaturesEnabled);
        
        // Button to enable features
        enableRallyFeaturesButton.onClick.AddListener(OnEnableRallyFeaturesClicked);
        
        // Initialize Rally if enabled
        if (rallyFeaturesEnabled)
        {
            RallyGameBridge.Instance.InitializeRallyFeatures();
        }
    }
    
    private void OnEnableRallyFeaturesClicked()
    {
        // Save user preference
        rallyFeaturesEnabled = true;
        PlayerPrefs.SetInt(RALLY_FEATURES_PREF_KEY, 1);
        PlayerPrefs.Save();
        
        // Hide teaser panel
        rallyFeaturesTeaserPanel.SetActive(false);
        
        // Initialize Rally features
        RallyGameBridge.Instance.InitializeRallyFeatures();
    }
}
```

## Best Practices for Integration

1. **Use the Bridge Pattern**: Create a clear separation between existing game systems and Rally Protocol functionality
2. **Make It Optional**: Allow players to opt-in to blockchain features rather than forcing them
3. **Consistent UX**: Maintain your game's visual style and UX patterns when adding blockchain features
4. **Graceful Fallbacks**: Handle potential blockchain operation failures gracefully
5. **Error Handling**: Implement comprehensive error handling and user-friendly error messages
6. **Performance Considerations**: Keep blockchain operations off the main thread to prevent UI freezes
7. **Testing Strategy**: Test both with and without Rally features enabled to ensure backward compatibility

## Conclusion

By following this integration pattern, you can add Rally Protocol functionality to your existing game with minimal disruption to your current codebase or player experience. The bridge layer approach creates a clean separation of concerns while allowing your game to take advantage of blockchain features.

## Next Steps

- Check out the [Token Economy Tutorial](../tutorials/implementing-token-economy.md) for more advanced integration ideas
- Explore [GSN Integration](../features/gsn-integration.md) to provide gasless transactions for your players
- See [Best Practices](../guides/best-practices.md) for additional recommendations
