# Implementing Player-to-Player Trading

This tutorial demonstrates how to implement a player-to-player (P2P) trading system in your Unity game using the Rally Protocol SDK. Players will be able to trade tokens and in-game assets with each other directly.

## Prerequisites

Before starting this tutorial, ensure you have:

- Completed the [Getting Started](../guides/getting-started.md) guide
- Set up player accounts using the [Account Creation](../examples/account-creation.md) example
- Implemented token functionality using the [Token Transfers](../examples/token-transfers.md) example
- Basic understanding of Unity UI and networking concepts

## Overview

A P2P trading system typically involves:

1. **Player Discovery** - Finding other players to trade with
2. **Trade Initiation** - Making trade offers
3. **Trade Negotiation** - Modifying offers and counter-offers
4. **Trade Execution** - Finalizing the exchange of assets
5. **Security Features** - Ensuring trades are secure and can't be exploited

## Part 1: Implementing the Trading System Backend

First, let's create the core trading system classes:

### Step 1: Create Trade Data Structures

```csharp
using System;
using System.Collections.Generic;
using UnityEngine;
using RallyProtocol.Unity;

[System.Serializable]
public class TradeOffer
{
    public string TradeId;               // Unique identifier for this trade
    public string OfferPlayerAddress;    // Address of player offering the trade
    public string TargetPlayerAddress;   // Address of player receiving the offer
    public decimal TokenAmount;          // Amount of RLY tokens offered
    public List<GameItem> ItemsOffered;  // Game items offered
    public List<GameItem> ItemsRequested; // Game items requested
    public TradeStatus Status;           // Current status of the trade
    public DateTime CreatedAt;           // When the trade was created
    public DateTime UpdatedAt;           // When the trade was last updated
    
    public enum TradeStatus
    {
        Pending,    // Offer sent, waiting for response
        Accepted,   // Offer accepted, waiting for execution
        Rejected,   // Offer rejected
        Cancelled,  // Offer cancelled by initiator
        Completed,  // Trade successfully completed
        Failed      // Trade execution failed
    }
}

[System.Serializable]
public class GameItem
{
    public string ItemId;     // Unique identifier for the item
    public string ItemName;   // Display name
    public string ItemType;   // Type of item (e.g., "weapon", "cosmetic")
    public string ImageUrl;   // URL to item image
    public int Quantity;      // How many of this item
    
    // Game-specific properties can be added here
}
```

### Step 2: Create a Trade Manager

```csharp
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using RallyProtocol.Unity;

public class TradeManager : MonoBehaviour
{
    private IRallyNetwork rlyNetwork;
    private Dictionary<string, TradeOffer> activeTrades = new Dictionary<string, TradeOffer>();
    
    // Events for UI and other systems to listen to
    public event Action<TradeOffer> OnTradeOfferReceived;
    public event Action<TradeOffer> OnTradeOfferUpdated;
    public event Action<TradeOffer> OnTradeCompleted;
    public event Action<TradeOffer, string> OnTradeFailed;
    
    private void Awake()
    {
        rlyNetwork = RallyUnityManager.Instance.RlyNetwork;
    }
    
    // Create a new trade offer
    public async Task<TradeOffer> CreateTradeOfferAsync(string targetPlayerAddress, 
                                                       decimal tokenAmount,
                                                       List<GameItem> itemsOffered, 
                                                       List<GameItem> itemsRequested)
    {
        try
        {
            // Validate inputs
            if (string.IsNullOrEmpty(targetPlayerAddress))
                throw new ArgumentException("Target player address cannot be empty");
                
            if (tokenAmount < 0)
                throw new ArgumentException("Token amount cannot be negative");
                
            // Check if player has sufficient balance for the trade
            if (tokenAmount > 0)
            {
                decimal balance = await rlyNetwork.GetBalanceAsync();
                if (balance < tokenAmount)
                {
                    throw new InvalidOperationException("Insufficient token balance for this trade");
                }
            }
            
            // Check if player owns the offered items
            foreach (var item in itemsOffered)
            {
                if (!InventoryManager.Instance.HasItem(item.ItemId, item.Quantity))
                {
                    throw new InvalidOperationException($"You don't have enough {item.ItemName} to offer");
                }
            }
            
            // Create trade offer
            string tradeId = GenerateTradeId();
            string myAddress = rlyNetwork.AccountManager.GetAccountAddress();
            
            TradeOffer offer = new TradeOffer
            {
                TradeId = tradeId,
                OfferPlayerAddress = myAddress,
                TargetPlayerAddress = targetPlayerAddress,
                TokenAmount = tokenAmount,
                ItemsOffered = itemsOffered,
                ItemsRequested = itemsRequested,
                Status = TradeOffer.TradeStatus.Pending,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };
            
            // Add to active trades
            activeTrades[tradeId] = offer;
            
            // In a real application, you would send the offer to your game server
            // which would forward it to the other player
            await SendTradeOfferToServer(offer);
            
            return offer;
        }
        catch (Exception e)
        {
            Debug.LogError($"Error creating trade offer: {e.Message}");
            throw;
        }
    }
    
    // Accept a received trade offer
    public async Task<bool> AcceptTradeOfferAsync(string tradeId)
    {
        if (!activeTrades.TryGetValue(tradeId, out TradeOffer offer))
        {
            Debug.LogError($"Trade offer with ID {tradeId} not found");
            return false;
        }
        
        try
        {
            // Update offer status
            offer.Status = TradeOffer.TradeStatus.Accepted;
            offer.UpdatedAt = DateTime.Now;
            
            // In a real application, notify your game server about the acceptance
            await NotifyServerOfTradeAcceptance(offer);
            
            // Execute the trade (this would be triggered by the server in a real app)
            return await ExecuteTradeAsync(offer);
        }
        catch (Exception e)
        {
            Debug.LogError($"Error accepting trade offer: {e.Message}");
            OnTradeFailed?.Invoke(offer, e.Message);
            return false;
        }
    }
    
    // Reject a received trade offer
    public async Task RejectTradeOfferAsync(string tradeId, string reason = null)
    {
        if (!activeTrades.TryGetValue(tradeId, out TradeOffer offer))
        {
            Debug.LogError($"Trade offer with ID {tradeId} not found");
            return;
        }
        
        try
        {
            // Update offer status
            offer.Status = TradeOffer.TradeStatus.Rejected;
            offer.UpdatedAt = DateTime.Now;
            
            // In a real application, notify your game server about the rejection
            await NotifyServerOfTradeRejection(offer, reason);
            
            // Remove from active trades
            activeTrades.Remove(tradeId);
            
            // Notify UI
            OnTradeOfferUpdated?.Invoke(offer);
        }
        catch (Exception e)
        {
            Debug.LogError($"Error rejecting trade offer: {e.Message}");
        }
    }
    
    // Cancel a trade offer you've created
    public async Task CancelTradeOfferAsync(string tradeId)
    {
        if (!activeTrades.TryGetValue(tradeId, out TradeOffer offer))
        {
            Debug.LogError($"Trade offer with ID {tradeId} not found");
            return;
        }
        
        if (offer.OfferPlayerAddress != rlyNetwork.AccountManager.GetAccountAddress())
        {
            Debug.LogError("Cannot cancel a trade offer you didn't create");
            return;
        }
        
        try
        {
            // Update offer status
            offer.Status = TradeOffer.TradeStatus.Cancelled;
            offer.UpdatedAt = DateTime.Now;
            
            // In a real application, notify your game server about the cancellation
            await NotifyServerOfTradeCancellation(offer);
            
            // Remove from active trades
            activeTrades.Remove(tradeId);
            
            // Notify UI
            OnTradeOfferUpdated?.Invoke(offer);
        }
        catch (Exception e)
        {
            Debug.LogError($"Error cancelling trade offer: {e.Message}");
        }
    }
    
    // Execute the trade (transfer tokens and items)
    private async Task<bool> ExecuteTradeAsync(TradeOffer offer)
    {
        Debug.Log($"Executing trade {offer.TradeId}");
        
        try
        {
            // Transfer tokens if applicable
            if (offer.TokenAmount > 0)
            {
                string txHash = await rlyNetwork.TransferAsync(offer.TargetPlayerAddress, offer.TokenAmount);
                Debug.Log($"Token transfer successful: {txHash}");
                
                // In a real app, you should wait for transaction confirmation
                // bool confirmed = await rlyNetwork.WaitForTransactionConfirmationAsync(txHash);
                // if (!confirmed) throw new Exception("Token transfer failed to confirm");
            }
            
            // Transfer items (this would involve your game's inventory system)
            // In a real application, this would likely be handled by your game server
            // which would update both players' inventories
            
            foreach (var item in offer.ItemsOffered)
            {
                // Transfer offered items from offerer to target
                InventoryManager.Instance.RemoveItem(item.ItemId, item.Quantity);
                // Server would add the item to target player's inventory
            }
            
            foreach (var item in offer.ItemsRequested)
            {
                // Transfer requested items from target to offerer
                // Server would handle this part in a real application
            }
            
            // Update trade status
            offer.Status = TradeOffer.TradeStatus.Completed;
            offer.UpdatedAt = DateTime.Now;
            
            // Notify UI
            OnTradeCompleted?.Invoke(offer);
            
            // Remove from active trades
            activeTrades.Remove(offer.TradeId);
            
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"Error executing trade: {e.Message}");
            offer.Status = TradeOffer.TradeStatus.Failed;
            offer.UpdatedAt = DateTime.Now;
            
            OnTradeFailed?.Invoke(offer, e.Message);
            
            // In a real application, you would have a mechanism to rollback partial trades
            
            return false;
        }
    }
    
    // Handle incoming trade offers (would be called when server notifies about new offers)
    public void HandleIncomingTradeOffer(TradeOffer offer)
    {
        // Add to active trades
        activeTrades[offer.TradeId] = offer;
        
        // Notify UI
        OnTradeOfferReceived?.Invoke(offer);
    }
    
    // Generate a unique trade ID
    private string GenerateTradeId()
    {
        return Guid.NewGuid().ToString();
    }
    
    // These methods would interface with your game server in a real application
    private async Task SendTradeOfferToServer(TradeOffer offer)
    {
        // Placeholder for sending to server
        // In a real app, this would use your networking system
        await Task.Delay(100); // Simulating network request
        Debug.Log($"Trade offer {offer.TradeId} sent to server");
    }
    
    private async Task NotifyServerOfTradeAcceptance(TradeOffer offer)
    {
        await Task.Delay(100); // Simulating network request
        Debug.Log($"Server notified of trade {offer.TradeId} acceptance");
    }
    
    private async Task NotifyServerOfTradeRejection(TradeOffer offer, string reason)
    {
        await Task.Delay(100); // Simulating network request
        Debug.Log($"Server notified of trade {offer.TradeId} rejection: {reason}");
    }
    
    private async Task NotifyServerOfTradeCancellation(TradeOffer offer)
    {
        await Task.Delay(100); // Simulating network request
        Debug.Log($"Server notified of trade {offer.TradeId} cancellation");
    }
}
```

### Step 3: Create a Placeholder Inventory Manager

This is a simplified inventory manager for the example. In your game, you would use your existing inventory system.

```csharp
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }
    
    private Dictionary<string, int> inventory = new Dictionary<string, int>();
    
    public event System.Action<string, int> OnItemAdded;
    public event System.Action<string, int> OnItemRemoved;
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
        DontDestroyOnLoad(gameObject);
        
        // Initialize with some test items
        AddItem("sword_basic", 1);
        AddItem("shield_basic", 1);
        AddItem("potion_health", 5);
        AddItem("gem_ruby", 3);
    }
    
    public bool HasItem(string itemId, int quantity = 1)
    {
        if (inventory.TryGetValue(itemId, out int currentQuantity))
        {
            return currentQuantity >= quantity;
        }
        return false;
    }
    
    public void AddItem(string itemId, int quantity = 1)
    {
        if (inventory.ContainsKey(itemId))
        {
            inventory[itemId] += quantity;
        }
        else
        {
            inventory[itemId] = quantity;
        }
        
        OnItemAdded?.Invoke(itemId, quantity);
    }
    
    public bool RemoveItem(string itemId, int quantity = 1)
    {
        if (!inventory.TryGetValue(itemId, out int currentQuantity) || currentQuantity < quantity)
        {
            return false;
        }
        
        inventory[itemId] -= quantity;
        
        if (inventory[itemId] <= 0)
        {
            inventory.Remove(itemId);
        }
        
        OnItemRemoved?.Invoke(itemId, quantity);
        return true;
    }
    
    public Dictionary<string, int> GetInventory()
    {
        return new Dictionary<string, int>(inventory);
    }
    
    public int GetItemQuantity(string itemId)
    {
        if (inventory.TryGetValue(itemId, out int quantity))
        {
            return quantity;
        }
        return 0;
    }
}
```

## Part 2: Creating the Trading UI

Now let's create a UI for trading:

### Step 1: Create Trade UI Components

```csharp
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using RallyProtocol.Unity;

public class TradeUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TradeManager tradeManager;
    [SerializeField] private GameObject tradePanel;
    [SerializeField] private TMP_InputField targetAddressInput;
    [SerializeField] private TMP_InputField tokenAmountInput;
    [SerializeField] private Transform myItemsContainer;
    [SerializeField] private Transform requestedItemsContainer;
    [SerializeField] private GameObject itemPrefab;
    [SerializeField] private Button createOfferButton;
    [SerializeField] private Button cancelButton;
    
    [Header("Trade Offer View")]
    [SerializeField] private GameObject tradeOfferPanel;
    [SerializeField] private TextMeshProUGUI offerPlayerText;
    [SerializeField] private TextMeshProUGUI tokenAmountText;
    [SerializeField] private Transform offeredItemsContainer;
    [SerializeField] private Transform requestedItemsListContainer;
    [SerializeField] private Button acceptOfferButton;
    [SerializeField] private Button rejectOfferButton;
    
    private IRallyNetwork rlyNetwork;
    private List<GameItem> selectedItemsToOffer = new List<GameItem>();
    private List<GameItem> selectedItemsToRequest = new List<GameItem>();
    private TradeOffer currentViewingOffer;
    
    private void Start()
    {
        rlyNetwork = RallyUnityManager.Instance.RlyNetwork;
        
        // Set up button listeners
        createOfferButton.onClick.AddListener(OnCreateOfferClicked);
        cancelButton.onClick.AddListener(() => tradePanel.SetActive(false));
        acceptOfferButton.onClick.AddListener(OnAcceptOfferClicked);
        rejectOfferButton.onClick.AddListener(OnRejectOfferClicked);
        
        // Subscribe to trade manager events
        tradeManager.OnTradeOfferReceived += HandleTradeOfferReceived;
        tradeManager.OnTradeCompleted += HandleTradeCompleted;
        tradeManager.OnTradeFailed += HandleTradeFailed;
        
        // Hide panels by default
        tradePanel.SetActive(false);
        tradeOfferPanel.SetActive(false);
    }
    
    public void ShowTradePanel()
    {
        // Reset fields
        targetAddressInput.text = "";
        tokenAmountInput.text = "0";
        selectedItemsToOffer.Clear();
        selectedItemsToRequest.Clear();
        
        // Populate item lists
        PopulateMyItemsList();
        
        tradePanel.SetActive(true);
    }
    
    private void PopulateMyItemsList()
    {
        // Clear container
        foreach (Transform child in myItemsContainer)
        {
            Destroy(child.gameObject);
        }
        
        // Add each inventory item
        Dictionary<string, int> inventory = InventoryManager.Instance.GetInventory();
        
        foreach (var item in inventory)
        {
            GameObject itemObject = Instantiate(itemPrefab, myItemsContainer);
            TradeItemUI itemUI = itemObject.GetComponent<TradeItemUI>();
            
            if (itemUI != null)
            {
                // In a real game, you would load item details from your item database
                GameItem gameItem = new GameItem
                {
                    ItemId = item.Key,
                    ItemName = GetItemName(item.Key), // You'd implement this
                    ItemType = GetItemType(item.Key), // You'd implement this
                    Quantity = item.Value
                };
                
                itemUI.Initialize(gameItem, true, OnMyItemSelected);
            }
        }
    }
    
    private void OnMyItemSelected(GameItem item, bool selected)
    {
        if (selected)
        {
            selectedItemsToOffer.Add(item);
        }
        else
        {
            selectedItemsToOffer.RemoveAll(i => i.ItemId == item.ItemId);
        }
    }
    
    private void OnRequestedItemSelected(GameItem item, bool selected)
    {
        if (selected)
        {
            selectedItemsToRequest.Add(item);
        }
        else
        {
            selectedItemsToRequest.RemoveAll(i => i.ItemId == item.ItemId);
        }
    }
    
    private async void OnCreateOfferClicked()
    {
        string targetAddress = targetAddressInput.text;
        
        if (string.IsNullOrEmpty(targetAddress))
        {
            Debug.LogError("Target address cannot be empty");
            return;
        }
        
        // Parse token amount
        if (!decimal.TryParse(tokenAmountInput.text, out decimal tokenAmount))
        {
            Debug.LogError("Invalid token amount");
            return;
        }
        
        try
        {
            TradeOffer offer = await tradeManager.CreateTradeOfferAsync(
                targetAddress,
                tokenAmount,
                selectedItemsToOffer,
                selectedItemsToRequest
            );
            
            // Hide the panel
            tradePanel.SetActive(false);
            
            // Show success message
            Debug.Log($"Trade offer created with ID: {offer.TradeId}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to create trade offer: {e.Message}");
            // Show error message to user
        }
    }
    
    private void HandleTradeOfferReceived(TradeOffer offer)
    {
        // Show notification
        Debug.Log($"New trade offer received from {offer.OfferPlayerAddress}");
        
        // You would typically show a notification here
        // For this example, we'll just open the offer panel directly
        ShowTradeOfferPanel(offer);
    }
    
    private void ShowTradeOfferPanel(TradeOffer offer)
    {
        currentViewingOffer = offer;
        
        // Set offer details
        offerPlayerText.text = $"From: {ShortenAddress(offer.OfferPlayerAddress)}";
        tokenAmountText.text = $"Tokens offered: {offer.TokenAmount} RLY";
        
        // Populate offered items
        foreach (Transform child in offeredItemsContainer)
        {
            Destroy(child.gameObject);
        }
        
        foreach (var item in offer.ItemsOffered)
        {
            GameObject itemObject = Instantiate(itemPrefab, offeredItemsContainer);
            TradeItemUI itemUI = itemObject.GetComponent<TradeItemUI>();
            
            if (itemUI != null)
            {
                itemUI.Initialize(item, false, null);
            }
        }
        
        // Populate requested items
        foreach (Transform child in requestedItemsListContainer)
        {
            Destroy(child.gameObject);
        }
        
        foreach (var item in offer.ItemsRequested)
        {
            GameObject itemObject = Instantiate(itemPrefab, requestedItemsListContainer);
            TradeItemUI itemUI = itemObject.GetComponent<TradeItemUI>();
            
            if (itemUI != null)
            {
                itemUI.Initialize(item, false, null);
            }
        }
        
        // Show the panel
        tradeOfferPanel.SetActive(true);
    }
    
    private async void OnAcceptOfferClicked()
    {
        if (currentViewingOffer == null)
        {
            Debug.LogError("No current trade offer to accept");
            return;
        }
        
        // Disable buttons during processing
        acceptOfferButton.interactable = false;
        rejectOfferButton.interactable = false;
        
        try
        {
            bool success = await tradeManager.AcceptTradeOfferAsync(currentViewingOffer.TradeId);
            
            if (success)
            {
                // Trade successful, close panel
                tradeOfferPanel.SetActive(false);
                currentViewingOffer = null;
                
                // Show success message
                Debug.Log("Trade completed successfully!");
            }
            else
            {
                // Trade failed
                Debug.LogError("Trade execution failed");
                
                // Re-enable buttons
                acceptOfferButton.interactable = true;
                rejectOfferButton.interactable = true;
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error accepting trade: {e.Message}");
            
            // Re-enable buttons
            acceptOfferButton.interactable = true;
            rejectOfferButton.interactable = true;
        }
    }
    
    private async void OnRejectOfferClicked()
    {
        if (currentViewingOffer == null)
        {
            Debug.LogError("No current trade offer to reject");
            return;
        }
        
        try
        {
            await tradeManager.RejectTradeOfferAsync(currentViewingOffer.TradeId, "Offer rejected by user");
            
            // Close panel
            tradeOfferPanel.SetActive(false);
            currentViewingOffer = null;
            
            Debug.Log("Trade offer rejected");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error rejecting trade: {e.Message}");
        }
    }
    
    private void HandleTradeCompleted(TradeOffer offer)
    {
        Debug.Log($"Trade {offer.TradeId} completed successfully!");
        
        // Update UI as needed
        if (currentViewingOffer != null && currentViewingOffer.TradeId == offer.TradeId)
        {
            tradeOfferPanel.SetActive(false);
            currentViewingOffer = null;
        }
    }
    
    private void HandleTradeFailed(TradeOffer offer, string reason)
    {
        Debug.LogError($"Trade {offer.TradeId} failed: {reason}");
        
        // Update UI as needed
        if (currentViewingOffer != null && currentViewingOffer.TradeId == offer.TradeId)
        {
            // Display error message
            // Re-enable buttons
            acceptOfferButton.interactable = true;
            rejectOfferButton.interactable = true;
        }
    }
    
    // Helper methods
    private string GetItemName(string itemId)
    {
        // In a real game, you would look up item details in your item database
        // This is just a placeholder
        switch (itemId)
        {
            case "sword_basic": return "Basic Sword";
            case "shield_basic": return "Basic Shield";
            case "potion_health": return "Health Potion";
            case "gem_ruby": return "Ruby Gem";
            default: return itemId;
        }
    }
    
    private string GetItemType(string itemId)
    {
        // In a real game, you would look up item details in your item database
        if (itemId.StartsWith("sword") || itemId.StartsWith("shield"))
            return "equipment";
        else if (itemId.StartsWith("potion"))
            return "consumable";
        else if (itemId.StartsWith("gem"))
            return "resource";
        else
            return "misc";
    }
    
    private string ShortenAddress(string address)
    {
        if (string.IsNullOrEmpty(address) || address.Length < 10)
            return address;
            
        return $"{address.Substring(0, 6)}...{address.Substring(address.Length - 4)}";
    }
}
```

### Step 2: Create a Trade Item UI Component

```csharp
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class TradeItemUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI itemNameText;
    [SerializeField] private TextMeshProUGUI quantityText;
    [SerializeField] private Image itemImage;
    [SerializeField] private Toggle itemToggle;
    
    private GameItem item;
    private Action<GameItem, bool> onSelectionChanged;
    
    public void Initialize(GameItem gameItem, bool selectable, Action<GameItem, bool> selectionCallback)
    {
        this.item = gameItem;
        
        // Set UI elements
        itemNameText.text = gameItem.ItemName;
        quantityText.text = $"x{gameItem.Quantity}";
        
        // In a real game, you would load the item image here
        // itemImage.sprite = ResourceManager.LoadItemSprite(gameItem.ItemId);
        
        // Set up toggle
        if (selectable)
        {
            itemToggle.gameObject.SetActive(true);
            onSelectionChanged = selectionCallback;
            itemToggle.onValueChanged.AddListener(OnToggleChanged);
        }
        else
        {
            itemToggle.gameObject.SetActive(false);
        }
    }
    
    private void OnToggleChanged(bool isOn)
    {
        onSelectionChanged?.Invoke(item, isOn);
    }
    
    private void OnDestroy()
    {
        // Clean up event listener
        if (itemToggle != null)
        {
            itemToggle.onValueChanged.RemoveListener(OnToggleChanged);
        }
    }
}
```

## Part 3: Implementing Security Measures

When implementing a P2P trading system, security is critical. Here are some essential security measures to implement:

### Step 1: Server Validation for Trades

In a real application, your game server should validate trades before allowing them to execute:

```csharp
// Example server-side validation pseudocode (this would be on your backend)
public class TradeValidator
{
    public ValidationResult ValidateTrade(TradeOffer offer)
    {
        // 1. Verify both players exist
        if (!PlayerExists(offer.OfferPlayerAddress) || !PlayerExists(offer.TargetPlayerAddress))
            return new ValidationResult(false, "One or both players do not exist");
        
        // 2. Verify the offerer owns all offered items
        foreach (var item in offer.ItemsOffered)
        {
            if (!PlayerHasItem(offer.OfferPlayerAddress, item.ItemId, item.Quantity))
                return new ValidationResult(false, "Offering player doesn't own all offered items");
        }
        
        // 3. Verify the target owns all requested items
        foreach (var item in offer.ItemsRequested)
        {
            if (!PlayerHasItem(offer.TargetPlayerAddress, item.ItemId, item.Quantity))
                return new ValidationResult(false, "Target player doesn't own all requested items");
        }
        
        // 4. Verify token balances if applicable
        if (offer.TokenAmount > 0)
        {
            if (!PlayerHasSufficientTokens(offer.OfferPlayerAddress, offer.TokenAmount))
                return new ValidationResult(false, "Offering player has insufficient token balance");
        }
        
        // 5. Check for rate limiting (prevent spam trades)
        if (IsRateLimited(offer.OfferPlayerAddress))
            return new ValidationResult(false, "Too many trade offers in a short period");
        
        // 6. Check for suspicious activity
        if (IsSuspiciousActivity(offer))
            return new ValidationResult(false, "Trade flagged for suspicious activity");
        
        return new ValidationResult(true, "Trade is valid");
    }
}

public struct ValidationResult
{
    public bool IsValid;
    public string Message;
    
    public ValidationResult(bool isValid, string message)
    {
        IsValid = isValid;
        Message = message;
    }
}
```

### Step 2: Transaction Atomicity

Ensure trades are atomic (all-or-nothing) to prevent partial trades:

```csharp
// Pseudocode for atomic trade execution
public async Task<bool> ExecuteTradeAtomically(TradeOffer offer)
{
    // Start a transaction or use a locking mechanism
    using (var transaction = await dbContext.BeginTransactionAsync())
    {
        try
        {
            // 1. Lock both player inventories
            await LockPlayerInventory(offer.OfferPlayerAddress);
            await LockPlayerInventory(offer.TargetPlayerAddress);
            
            // 2. Revalidate the trade (make sure nothing changed since validation)
            if (!await RevalidateTrade(offer))
            {
                await transaction.RollbackAsync();
                return false;
            }
            
            // 3. Execute blockchain transaction for tokens if applicable
            if (offer.TokenAmount > 0)
            {
                string txHash = await ExecuteTokenTransfer(
                    offer.OfferPlayerAddress,
                    offer.TargetPlayerAddress,
                    offer.TokenAmount
                );
                
                // Wait for confirmation
                bool confirmed = await WaitForConfirmation(txHash);
                if (!confirmed)
                {
                    await transaction.RollbackAsync();
                    return false;
                }
            }
            
            // 4. Transfer offered items
            foreach (var item in offer.ItemsOffered)
            {
                await TransferItem(
                    offer.OfferPlayerAddress,
                    offer.TargetPlayerAddress,
                    item.ItemId,
                    item.Quantity
                );
            }
            
            // 5. Transfer requested items
            foreach (var item in offer.ItemsRequested)
            {
                await TransferItem(
                    offer.TargetPlayerAddress,
                    offer.OfferPlayerAddress,
                    item.ItemId,
                    item.Quantity
                );
            }
            
            // 6. Commit the transaction
            await transaction.CommitAsync();
            return true;
        }
        catch (Exception e)
        {
            // Log error and rollback
            await transaction.RollbackAsync();
            return false;
        }
        finally
        {
            // Unlock inventories
            await UnlockPlayerInventory(offer.OfferPlayerAddress);
            await UnlockPlayerInventory(offer.TargetPlayerAddress);
        }
    }
}
```

### Step 3: Rate Limiting and Anti-Spam

Implement rate limiting to prevent trade spam:

```csharp
public class TradeRateLimiter
{
    private Dictionary<string, Queue<DateTime>> tradeTimes = new Dictionary<string, Queue<DateTime>>();
    private const int MAX_TRADES_PER_MINUTE = 5;
    
    public bool IsRateLimited(string playerAddress)
    {
        if (!tradeTimes.TryGetValue(playerAddress, out var times))
        {
            times = new Queue<DateTime>();
            tradeTimes[playerAddress] = times;
        }
        
        // Remove old timestamps
        while (times.Count > 0 && (DateTime.Now - times.Peek()).TotalMinutes > 1)
        {
            times.Dequeue();
        }
        
        // Check if within rate limit
        if (times.Count >= MAX_TRADES_PER_MINUTE)
        {
            return true; // Rate limited
        }
        
        // Add new timestamp
        times.Enqueue(DateTime.Now);
        return false;
    }
}
```

## Best Practices for P2P Trading

1. **Always Validate on Server** - Never trust client-side validation alone
2. **Implement Escrow Systems** - Hold assets in escrow during trade negotiation
3. **Add Confirmation Steps** - Require explicit confirmation before finalizing trades
4. **Monitor for Abuse** - Watch for suspicious patterns that might indicate scams or exploits
5. **Keep Trade History** - Maintain a log of all trades for dispute resolution
6. **Provide Clear UI** - Make it obvious what assets are being traded in both directions
7. **Include Fallback Mechanisms** - Implement systems to resolve incomplete trades
8. **Protect Against Asset Duplication** - Use locking mechanisms to prevent asset duplication

## Conclusion

This tutorial has covered the fundamentals of implementing a secure P2P trading system in your Unity game using the Rally Protocol SDK. We've built:

- A core trading system with offer creation, negotiation, and execution
- A UI for players to create and respond to trade offers
- Security measures to ensure trades are valid and atomic

This implementation can be extended with additional features such as:

- Trade offer expiration times
- Counter-offers during negotiation
- Trade history and analytics
- Trade rating systems
- Integration with NFT marketplaces

## Next Steps

- Learn about [implementing NFTs](./implementing-nfts.md) that can be traded between players
- Explore the [GSN Integration](../features/gsn-integration.md) for gasless trading
- Implement [Advanced Token Economy](./advanced-token-economy.md) mechanisms to create a balanced trading environment
