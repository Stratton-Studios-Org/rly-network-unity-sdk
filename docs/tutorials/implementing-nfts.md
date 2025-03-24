# Implementing NFTs in Your Unity Game

This tutorial guides you through implementing Non-Fungible Tokens (NFTs) in your Unity game using the Rally Protocol SDK. You'll learn how to create, mint, display, and transfer NFTs within your game ecosystem.

## Prerequisites

Before starting this tutorial, ensure you have:

- Completed the [Getting Started](../guides/getting-started.md) guide
- Set up player accounts using the [Account Creation](../examples/account-creation.md) example
- Basic understanding of NFT concepts and ERC-721/ERC-1155 standards
- Familiarity with Unity UI and asset handling

## Overview

This tutorial will cover:

1. **Understanding NFTs in Games** - Key concepts and use cases
2. **Setting Up NFT Contracts** - Working with existing contracts or deploying your own
3. **Minting NFTs** - Creating new NFTs for players
4. **Displaying NFTs** - Showing NFTs in your game UI
5. **Transferring NFTs** - Enabling players to trade or gift NFTs
6. **NFT Metadata and Properties** - Managing NFT characteristics and attributes

## NFT Concepts for Game Developers

### What Are NFTs?

Non-Fungible Tokens (NFTs) are unique digital assets stored on a blockchain. Unlike fungible tokens (such as RLY), each NFT has distinct properties and cannot be exchanged on a 1:1 basis with other tokens.

### NFT Standards

Two primary standards are used for NFTs:

- **ERC-721**: Each token is completely unique
- **ERC-1155**: Semi-fungible tokens that can represent both unique items and multiple identical items

### NFT Use Cases in Games

- Unique in-game items (weapons, armor, cosmetics)
- Character customizations and skins
- Land or property ownership
- Achievement badges and trophies
- Access passes to special events or areas
- Collectible cards or figures

## Part 1: Setting Up NFT Contracts

### Using Existing Contracts

The simplest approach is to use existing NFT contracts. The Rally Protocol SDK can interact with any ERC-721 or ERC-1155 contract.

```csharp
using RallyProtocol.Unity;
using System.Threading.Tasks;
using UnityEngine;

public class NFTManager : MonoBehaviour
{
    private IRallyNetwork rlyNetwork;
    
    // Contract addresses for your NFTs
    private string erc721ContractAddress = "0xYourERC721ContractAddress";
    private string erc1155ContractAddress = "0xYourERC1155ContractAddress";
    
    private void Awake()
    {
        // Get reference to RallyNetwork
        rlyNetwork = RallyUnityManager.Instance.RlyNetwork;
    }
    
    // Check if player owns a specific ERC-721 NFT
    public async Task<bool> CheckERC721Ownership(string tokenId)
    {
        try
        {
            string playerAddress = rlyNetwork.AccountManager.GetAccountAddress();
            
            // Create contract query
            string ownerOfFunctionSignature = "ownerOf(uint256)";
            object[] parameters = new object[] { tokenId };
            
            // Call contract to check ownership
            string owner = await rlyNetwork.CallContractFunctionAsync<string>(
                erc721ContractAddress,
                ownerOfFunctionSignature,
                parameters
            );
            
            // Check if the owner matches the player's address
            return owner.ToLower() == playerAddress.ToLower();
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error checking NFT ownership: {e.Message}");
            return false;
        }
    }
    
    // Check if player owns a specific ERC-1155 NFT
    public async Task<int> CheckERC1155Balance(string tokenId)
    {
        try
        {
            string playerAddress = rlyNetwork.AccountManager.GetAccountAddress();
            
            // Create contract query
            string balanceOfFunctionSignature = "balanceOf(address,uint256)";
            object[] parameters = new object[] { playerAddress, tokenId };
            
            // Call contract to check balance
            int balance = await rlyNetwork.CallContractFunctionAsync<int>(
                erc1155ContractAddress,
                balanceOfFunctionSignature,
                parameters
            );
            
            return balance;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error checking NFT balance: {e.Message}");
            return 0;
        }
    }
}
```

### Deploying Your Own NFT Contract

For more control, you can deploy your own NFT contract. This requires a backend component or a one-time deployment script. Here's an example of interacting with a newly deployed contract:

```csharp
// This would typically be done on your backend or through a deployment tool
public async Task<string> DeployNFTContract()
{
    // Contract deployment code would go here
    // This is a simplified example
    
    string contractAddress = "0xNewlyDeployedContractAddress";
    return contractAddress;
}
```

## Part 2: Minting NFTs

Minting is the process of creating a new NFT. Depending on your game design, you might mint NFTs:

- When players complete achievements
- As rewards for in-game activities
- Through a marketplace or shop
- At specific game milestones

### Minting an ERC-721 NFT

```csharp
public async Task<bool> MintERC721NFT(string to, string tokenId, string tokenURI)
{
    try
    {
        // Create mint function call
        string mintFunctionSignature = "mint(address,uint256,string)";
        object[] parameters = new object[] { to, tokenId, tokenURI };
        
        // Execute the transaction
        string txHash = await rlyNetwork.SendContractTransactionAsync(
            erc721ContractAddress,
            mintFunctionSignature,
            parameters
        );
        
        Debug.Log($"NFT minting transaction sent: {txHash}");
        
        // In a production environment, you would wait for confirmation
        // bool confirmed = await WaitForTransactionConfirmation(txHash);
        // return confirmed;
        
        return true;
    }
    catch (System.Exception e)
    {
        Debug.LogError($"Error minting NFT: {e.Message}");
        return false;
    }
}
```

### Minting an ERC-1155 NFT

```csharp
public async Task<bool> MintERC1155NFT(string to, string tokenId, int amount, byte[] data)
{
    try
    {
        // Create mint function call
        string mintFunctionSignature = "mint(address,uint256,uint256,bytes)";
        object[] parameters = new object[] { to, tokenId, amount, data };
        
        // Execute the transaction
        string txHash = await rlyNetwork.SendContractTransactionAsync(
            erc1155ContractAddress,
            mintFunctionSignature,
            parameters
        );
        
        Debug.Log($"NFT minting transaction sent: {txHash}");
        
        // In a production environment, you would wait for confirmation
        // bool confirmed = await WaitForTransactionConfirmation(txHash);
        // return confirmed;
        
        return true;
    }
    catch (System.Exception e)
    {
        Debug.LogError($"Error minting NFT: {e.Message}");
        return false;
    }
}
```

### Handling NFT Metadata

NFT metadata is typically stored as a JSON file at a URL specified in the token URI. This metadata includes information about the NFT such as name, description, image URL, and attributes.

```json
{
  "name": "Legendary Sword of Truth",
  "description": "A powerful sword forged in the depths of Mount Doom",
  "image": "https://your-game-assets.com/legendary_sword.png",
  "attributes": [
    {
      "trait_type": "Damage",
      "value": 100
    },
    {
      "trait_type": "Element",
      "value": "Fire"
    },
    {
      "trait_type": "Rarity",
      "value": "Legendary"
    }
  ]
}
```

You can store this metadata:

- On IPFS (recommended for decentralization)
- On your game server
- On a third-party NFT metadata hosting service

## Part 3: Displaying NFTs in Your Game

### Creating an NFT Inventory System

```csharp
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using RallyProtocol.Unity;
using System.Threading.Tasks;

public class NFTInventoryUI : MonoBehaviour
{
    [SerializeField] private NFTManager nftManager;
    [SerializeField] private Transform nftItemContainer;
    [SerializeField] private GameObject nftItemPrefab;
    [SerializeField] private Button refreshButton;
    [SerializeField] private TextMeshProUGUI statusText;
    
    private IRallyNetwork rlyNetwork;
    
    // Sample NFT data structure
    [System.Serializable]
    public class NFTItem
    {
        public string TokenId;
        public string Name;
        public string Description;
        public string ImageURL;
        public Dictionary<string, string> Attributes;
        public NFTType Type;
        
        public enum NFTType
        {
            ERC721,
            ERC1155
        }
    }
    
    private void Start()
    {
        rlyNetwork = RallyUnityManager.Instance.RlyNetwork;
        
        refreshButton.onClick.AddListener(LoadNFTs);
        
        // Initial load
        LoadNFTs();
    }
    
    private async void LoadNFTs()
    {
        statusText.text = "Loading NFTs...";
        
        // Clear container
        foreach (Transform child in nftItemContainer)
        {
            Destroy(child.gameObject);
        }
        
        try
        {
            // Get player's address
            string playerAddress = rlyNetwork.AccountManager.GetAccountAddress();
            
            // Get NFTs owned by the player
            List<NFTItem> playerNFTs = await GetPlayerNFTs(playerAddress);
            
            if (playerNFTs.Count == 0)
            {
                statusText.text = "No NFTs found";
                return;
            }
            
            // Display each NFT
            foreach (NFTItem nft in playerNFTs)
            {
                GameObject nftObject = Instantiate(nftItemPrefab, nftItemContainer);
                NFTItemUI nftUI = nftObject.GetComponent<NFTItemUI>();
                
                if (nftUI != null)
                {
                    await nftUI.Initialize(nft);
                }
            }
            
            statusText.text = $"Loaded {playerNFTs.Count} NFTs";
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error loading NFTs: {e.Message}");
            statusText.text = "Error loading NFTs";
        }
    }
    
    private async Task<List<NFTItem>> GetPlayerNFTs(string playerAddress)
    {
        // This is where you would query your NFT contracts to get the player's NFTs
        // This is a simplified example
        
        List<NFTItem> nfts = new List<NFTItem>();
        
        // Example: Query for specific token IDs the player might own
        string[] tokenIdsToCheck = { "1", "2", "3", "4", "5" };
        
        foreach (string tokenId in tokenIdsToCheck)
        {
            bool ownsERC721 = await nftManager.CheckERC721Ownership(tokenId);
            
            if (ownsERC721)
            {
                // Fetch metadata for this NFT
                NFTItem nft = await FetchNFTMetadata(tokenId, NFTItem.NFTType.ERC721);
                if (nft != null)
                {
                    nfts.Add(nft);
                }
            }
            
            int erc1155Balance = await nftManager.CheckERC1155Balance(tokenId);
            
            if (erc1155Balance > 0)
            {
                NFTItem nft = await FetchNFTMetadata(tokenId, NFTItem.NFTType.ERC1155);
                if (nft != null)
                {
                    nft.Description += $" (Quantity: {erc1155Balance})";
                    nfts.Add(nft);
                }
            }
        }
        
        return nfts;
    }
    
    private async Task<NFTItem> FetchNFTMetadata(string tokenId, NFTItem.NFTType nftType)
    {
        try
        {
            // Get token URI from contract
            string tokenURI = await GetTokenURI(tokenId, nftType);
            
            // Fetch metadata from URI
            // In a real implementation, you would make an HTTP request to the tokenURI
            // This is simplified for the example
            
            // Simulate metadata retrieval
            await Task.Delay(100);
            
            // Create NFT item with sample data
            NFTItem nft = new NFTItem
            {
                TokenId = tokenId,
                Name = $"NFT #{tokenId}",
                Description = "A sample NFT item",
                ImageURL = "https://your-game-assets.com/nft_image.png",
                Attributes = new Dictionary<string, string>
                {
                    { "Rarity", "Common" },
                    { "Type", "Weapon" }
                },
                Type = nftType
            };
            
            return nft;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error fetching NFT metadata: {e.Message}");
            return null;
        }
    }
    
    private async Task<string> GetTokenURI(string tokenId, NFTItem.NFTType nftType)
    {
        string contractAddress = nftType == NFTItem.NFTType.ERC721 ? 
            nftManager.ERC721ContractAddress : nftManager.ERC1155ContractAddress;
            
        string functionSignature = nftType == NFTItem.NFTType.ERC721 ? 
            "tokenURI(uint256)" : "uri(uint256)";
            
        object[] parameters = new object[] { tokenId };
        
        try
        {
            string uri = await rlyNetwork.CallContractFunctionAsync<string>(
                contractAddress,
                functionSignature,
                parameters
            );
            
            return uri;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error getting token URI: {e.Message}");
            return "";
        }
    }
}
```

### Creating an NFT Item UI Component

```csharp
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Threading.Tasks;
using System.Collections.Generic;

public class NFTItemUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private Image nftImage;
    [SerializeField] private Transform attributesContainer;
    [SerializeField] private GameObject attributePrefab;
    [SerializeField] private Button transferButton;
    
    private NFTInventoryUI.NFTItem nftData;
    
    public async Task Initialize(NFTInventoryUI.NFTItem nft)
    {
        this.nftData = nft;
        
        // Set basic info
        nameText.text = nft.Name;
        descriptionText.text = nft.Description;
        
        // Load image
        await LoadNFTImage(nft.ImageURL);
        
        // Display attributes
        DisplayAttributes(nft.Attributes);
        
        // Set up transfer button
        transferButton.onClick.AddListener(OnTransferButtonClicked);
    }
    
    private async Task LoadNFTImage(string imageUrl)
    {
        // In a real implementation, you would load the image from the URL
        // This is simplified for the example
        
        // Simulate image loading
        await Task.Delay(100);
        
        // For a real implementation, you would use something like:
        // UnityWebRequest request = UnityWebRequestTexture.GetTexture(imageUrl);
        // await request.SendWebRequest();
        // if (request.result == UnityWebRequest.Result.Success)
        // {
        //     Texture2D texture = DownloadHandlerTexture.GetContent(request);
        //     nftImage.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one * 0.5f);
        // }
    }
    
    private void DisplayAttributes(Dictionary<string, string> attributes)
    {
        // Clear container
        foreach (Transform child in attributesContainer)
        {
            Destroy(child.gameObject);
        }
        
        // Add attribute items
        foreach (var attribute in attributes)
        {
            GameObject attributeObject = Instantiate(attributePrefab, attributesContainer);
            TextMeshProUGUI attributeText = attributeObject.GetComponent<TextMeshProUGUI>();
            
            if (attributeText != null)
            {
                attributeText.text = $"{attribute.Key}: {attribute.Value}";
            }
        }
    }
    
    private void OnTransferButtonClicked()
    {
        // Open transfer dialog
        NFTTransferDialog.Instance.ShowDialog(nftData);
    }
}
```

## Part 4: Transferring NFTs

NFT transfers allow players to trade or gift NFTs to other players. This functionality should be implemented with care to prevent unintended transfers.

### Creating a Transfer Dialog

```csharp
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using RallyProtocol.Unity;
using System.Threading.Tasks;

public class NFTTransferDialog : MonoBehaviour
{
    public static NFTTransferDialog Instance { get; private set; }
    
    [SerializeField] private GameObject dialogPanel;
    [SerializeField] private TextMeshProUGUI nftNameText;
    [SerializeField] private TMP_InputField recipientAddressInput;
    [SerializeField] private Button transferButton;
    [SerializeField] private Button cancelButton;
    [SerializeField] private TextMeshProUGUI statusText;
    
    private NFTInventoryUI.NFTItem currentNFT;
    private IRallyNetwork rlyNetwork;
    private NFTManager nftManager;
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
        
        // Hide dialog by default
        dialogPanel.SetActive(false);
        
        // Set up button listeners
        transferButton.onClick.AddListener(OnTransferConfirmed);
        cancelButton.onClick.AddListener(() => dialogPanel.SetActive(false));
    }
    
    private void Start()
    {
        rlyNetwork = RallyUnityManager.Instance.RlyNetwork;
        nftManager = FindObjectOfType<NFTManager>();
    }
    
    public void ShowDialog(NFTInventoryUI.NFTItem nft)
    {
        currentNFT = nft;
        
        // Reset fields
        nftNameText.text = nft.Name;
        recipientAddressInput.text = "";
        statusText.text = "";
        
        // Show dialog
        dialogPanel.SetActive(true);
    }
    
    private async void OnTransferConfirmed()
    {
        string recipientAddress = recipientAddressInput.text;
        
        // Validate recipient address
        if (string.IsNullOrEmpty(recipientAddress))
        {
            statusText.text = "Please enter a recipient address";
            return;
        }
        
        // Disable button during processing
        transferButton.interactable = false;
        statusText.text = "Processing transfer...";
        
        try
        {
            bool success = await TransferNFT(recipientAddress, currentNFT);
            
            if (success)
            {
                statusText.text = "Transfer successful!";
                
                // Close dialog after a delay
                await Task.Delay(2000);
                dialogPanel.SetActive(false);
                
                // Refresh NFT inventory
                FindObjectOfType<NFTInventoryUI>()?.LoadNFTs();
            }
            else
            {
                statusText.text = "Transfer failed. Please try again.";
                transferButton.interactable = true;
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error transferring NFT: {e.Message}");
            statusText.text = "Error: " + e.Message;
            transferButton.interactable = true;
        }
    }
    
    private async Task<bool> TransferNFT(string to, NFTInventoryUI.NFTItem nft)
    {
        try
        {
            string from = rlyNetwork.AccountManager.GetAccountAddress();
            
            if (nft.Type == NFTInventoryUI.NFTItem.NFTType.ERC721)
            {
                // For ERC-721, we use transferFrom
                string transferFunctionSignature = "transferFrom(address,address,uint256)";
                object[] parameters = new object[] { from, to, nft.TokenId };
                
                string txHash = await rlyNetwork.SendContractTransactionAsync(
                    nftManager.ERC721ContractAddress,
                    transferFunctionSignature,
                    parameters
                );
                
                Debug.Log($"ERC-721 transfer transaction sent: {txHash}");
            }
            else if (nft.Type == NFTInventoryUI.NFTItem.NFTType.ERC1155)
            {
                // For ERC-1155, we use safeTransferFrom with a quantity of 1
                string transferFunctionSignature = "safeTransferFrom(address,address,uint256,uint256,bytes)";
                object[] parameters = new object[] { from, to, nft.TokenId, 1, new byte[0] };
                
                string txHash = await rlyNetwork.SendContractTransactionAsync(
                    nftManager.ERC1155ContractAddress,
                    transferFunctionSignature,
                    parameters
                );
                
                Debug.Log($"ERC-1155 transfer transaction sent: {txHash}");
            }
            
            // In a production environment, you would wait for confirmation
            // bool confirmed = await WaitForTransactionConfirmation(txHash);
            // return confirmed;
            
            return true;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error transferring NFT: {e.Message}");
            return false;
        }
    }
}
```

## NFT Game Integration Best Practices

1. **Keep Game Logic Separate**: Don't make core gameplay depend on blockchain operations, which can be slow or occasionally fail.

2. **Use Local Caching**: Cache NFT data locally to provide a responsive UI without constant blockchain calls.

3. **Provide Clear Feedback**: Always show clear status updates for blockchain operations.

4. **Handle Network Issues**: Implement retry mechanisms and fallbacks for network issues.

5. **Design for Gas Costs**: If using Ethereum mainnet, consider gas costs in your game design or implement gasless transactions using GSN.

6. **Consider User Experience**: Make the blockchain complexity invisible to players when possible.

7. **Security Measures**: Implement confirmation dialogs for transfers and other important operations.

8. **Pre-fetch Data**: Load NFT data in the background to minimize waiting times.

## Technical Integration Diagram

Here's a simplified flow of how NFTs integrate with your game:

```
Game Client                   Rally Protocol SDK                Blockchain
    |                               |                               |
    |-- Check for NFTs ------------>|-- Query NFT Contracts ------->|
    |<-- Return NFT Data -----------|<-- Return Owner/Balance ------|
    |                               |                               |
    |-- Display NFTs to Player      |                               |
    |                               |                               |
    |-- Player Initiates Transfer ->|                               |
    |                               |-- Submit Transfer TX -------->|
    |<-- TX Submitted Notification -|<-- TX Hash ------------------|
    |                               |                               |
    |-- Show Pending UI             |                               |
    |                               |-- Check TX Status ----------->|
    |<-- TX Confirmed Notification -|<-- Confirmation --------------|
    |                               |                               |
    |-- Update UI                   |                               |
```

## Conclusion

By following this tutorial, you've learned how to implement NFTs in your Unity game using the Rally Protocol SDK. You now have the knowledge to:

1. Interact with NFT contracts (ERC-721 and ERC-1155)
2. Mint new NFTs as rewards or collectibles
3. Display NFTs in your game UI
4. Enable players to transfer NFTs

NFTs can add a new dimension of ownership and value to your game items, allowing players to truly own their in-game assets.

## Next Steps

- Explore [Player-to-Player Trading](./implementing-p2p-trading.md) to create a marketplace for NFTs
- Learn about [Advanced Token Economy](./advanced-token-economy.md) to create a balanced ecosystem
- Implement [GSN Integration](../features/gsn-integration.md) for gasless NFT operations
