# Token Transfers Example

This example demonstrates how to perform token transfers using the Rally Protocol Unity SDK.

## Overview

Token transfers are a fundamental blockchain operation that allows users to send tokens to other addresses. With the Rally Protocol Unity SDK, you can perform token transfers easily and with minimal code.

## Basic Token Transfer

### Step 1: Set Up the Rally Network

First, you need to ensure the Rally Network is properly initialized:

```csharp
using RallyProtocol.Unity;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class TokenTransferExample : MonoBehaviour
{
    [SerializeField] private InputField recipientAddressInput;
    [SerializeField] private InputField amountInput;
    [SerializeField] private Button transferButton;
    [SerializeField] private Text statusText;

    private IRallyNetwork _rallyNetwork;

    private void Start()
    {
        // Get a reference to the Rally Network
        _rallyNetwork = RallyUnityManager.Instance.RlyNetwork;
        
        // Set up the UI button
        transferButton.onClick.AddListener(OnTransferButtonClicked);
        
        // Update status
        UpdateStatus("Ready to transfer tokens");
    }
}
```

### Step 2: Implement the Transfer Method

Next, add a method to perform the token transfer:

```csharp
public async Task TransferTokensAsync(string recipientAddress, decimal amount)
{
    try
    {
        // Validate the recipient address
        if (string.IsNullOrEmpty(recipientAddress) || !recipientAddress.StartsWith("0x"))
        {
            UpdateStatus("Invalid recipient address. Must start with '0x'.");
            return;
        }

        // Validate the amount
        if (amount <= 0)
        {
            UpdateStatus("Amount must be greater than 0.");
            return;
        }

        // Check if we have enough balance
        decimal currentBalance = await _rallyNetwork.GetBalanceAsync();
        if (currentBalance < amount)
        {
            UpdateStatus($"Insufficient balance. You have {currentBalance} RLY.");
            return;
        }

        // Perform the transfer
        UpdateStatus("Transferring tokens...");
        string txHash = await _rallyNetwork.TransferAsync(recipientAddress, amount);
        
        UpdateStatus($"Transfer complete! Transaction hash: {txHash}");
    }
    catch (Exception ex)
    {
        UpdateStatus($"Transfer failed: {ex.Message}");
        Debug.LogError($"Token transfer error: {ex}");
    }
}
```

### Step 3: Connect the UI

Finally, connect the UI elements to the transfer method:

```csharp
private void OnTransferButtonClicked()
{
    // Disable the button while processing
    transferButton.interactable = false;
    
    // Get the values from the input fields
    string recipientAddress = recipientAddressInput.text;
    
    // Parse the amount
    if (!decimal.TryParse(amountInput.text, out decimal amount))
    {
        UpdateStatus("Invalid amount. Please enter a valid number.");
        transferButton.interactable = true;
        return;
    }
    
    // Perform the transfer
    TransferTokensAsync(recipientAddress, amount).ContinueWith(_ => 
    {
        // Re-enable the button when done
        UnityMainThreadDispatcher.Instance().Enqueue(() => 
        {
            transferButton.interactable = true;
        });
    });
}

private void UpdateStatus(string message)
{
    if (statusText != null)
    {
        statusText.text = message;
        Debug.Log(message);
    }
}
```

## Complete Example

Here's the complete example that you can copy and use in your project:

```csharp
using RallyProtocol.Unity;
using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class TokenTransferExample : MonoBehaviour
{
    [SerializeField] private InputField recipientAddressInput;
    [SerializeField] private InputField amountInput;
    [SerializeField] private Button transferButton;
    [SerializeField] private Text statusText;
    [SerializeField] private Text balanceText;

    private IRallyNetwork _rallyNetwork;

    private void Start()
    {
        // Get a reference to the Rally Network
        _rallyNetwork = RallyUnityManager.Instance.RlyNetwork;
        
        // Set up the UI button
        transferButton.onClick.AddListener(OnTransferButtonClicked);
        
        // Update status
        UpdateStatus("Ready to transfer tokens");
        
        // Update balance
        UpdateBalanceAsync().ContinueWith(_ => { });
    }

    private void OnEnable()
    {
        // Update balance whenever the component is enabled
        UpdateBalanceAsync().ContinueWith(_ => { });
    }

    private async Task UpdateBalanceAsync()
    {
        try
        {
            decimal balance = await _rallyNetwork.GetBalanceAsync();
            UnityMainThreadDispatcher.Instance().Enqueue(() => 
            {
                balanceText.text = $"Balance: {balance} RLY";
            });
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to update balance: {ex.Message}");
        }
    }

    public async Task TransferTokensAsync(string recipientAddress, decimal amount)
    {
        try
        {
            // Validate the recipient address
            if (string.IsNullOrEmpty(recipientAddress) || !recipientAddress.StartsWith("0x"))
            {
                UpdateStatus("Invalid recipient address. Must start with '0x'.");
                return;
            }

            // Validate the amount
            if (amount <= 0)
            {
                UpdateStatus("Amount must be greater than 0.");
                return;
            }

            // Check if we have enough balance
            decimal currentBalance = await _rallyNetwork.GetBalanceAsync();
            if (currentBalance < amount)
            {
                UpdateStatus($"Insufficient balance. You have {currentBalance} RLY.");
                return;
            }

            // Perform the transfer
            UpdateStatus("Transferring tokens...");
            string txHash = await _rallyNetwork.TransferAsync(recipientAddress, amount);
            
            UpdateStatus($"Transfer complete! Transaction hash: {txHash}");
            
            // Update the balance after transfer
            await UpdateBalanceAsync();
        }
        catch (Exception ex)
        {
            UpdateStatus($"Transfer failed: {ex.Message}");
            Debug.LogError($"Token transfer error: {ex}");
        }
    }

    private void OnTransferButtonClicked()
    {
        // Disable the button while processing
        transferButton.interactable = false;
        
        // Get the values from the input fields
        string recipientAddress = recipientAddressInput.text;
        
        // Parse the amount
        if (!decimal.TryParse(amountInput.text, out decimal amount))
        {
            UpdateStatus("Invalid amount. Please enter a valid number.");
            transferButton.interactable = true;
            return;
        }
        
        // Perform the transfer
        TransferTokensAsync(recipientAddress, amount).ContinueWith(_ => 
        {
            // Re-enable the button when done
            UnityMainThreadDispatcher.Instance().Enqueue(() => 
            {
                transferButton.interactable = true;
            });
        });
    }

    private void UpdateStatus(string message)
    {
        if (statusText != null)
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() => 
            {
                statusText.text = message;
            });
            Debug.Log(message);
        }
    }
}
```

## Unity Setup

1. Create a new Unity UI Canvas
2. Add the following UI elements:
   - Input field for recipient address
   - Input field for amount
   - Button for initiating the transfer
   - Text elements for status and balance display
3. Attach the `TokenTransferExample` script to a GameObject in your scene
4. Assign the UI elements to the script's public properties in the Inspector

## Important Considerations

### Gas Fees

The Rally Protocol handles gas fees for you, so users don't need to worry about having ETH for gas. This is handled through the Gas Station Network (GSN).

### Error Handling

The example includes basic error handling, but in a production application, you should consider:

- More detailed error messages
- Retry mechanisms for temporary failures
- Transaction receipt tracking
- Confirmation dialogs for large transfers

### Security Best Practices

- Always validate user input
- Confirm transfers with the user before processing
- Consider adding a transfer limit
- Add a confirmation step for large transfers

## Next Steps

After implementing token transfers, consider exploring these related features:

- [Checking Token Balances](./balance-checking.md)
- [Claiming RLY Tokens](./claim-rly.md)
- [Transaction History](./transaction-history.md)

## Related Documentation

- [Token Operations](../features/token-operations.md)
- [Gas Station Network Integration](../features/gsn-integration.md)
- [Transaction Security](../features/transaction-security.md)
