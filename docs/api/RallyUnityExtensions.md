# RallyUnityExtensions

This document covers the Unity-specific extension methods provided by the Rally Protocol Unity SDK. These extensions are designed to make integrating Rally Protocol functionality more seamless within Unity projects.

## Overview

The `RallyUnityExtensions` class provides extension methods that enhance the base Rally Protocol functionality with Unity-specific features. These extensions primarily help with task management, Unity UI integration, and platform-specific behavior.

## Extension Methods

### Task Extensions

#### WaitForCompletion

```csharp
public static T WaitForCompletion<T>(this Task<T> task)
```

Synchronously waits for a task to complete and returns its result. This is useful in contexts where async/await cannot be used directly.

**Warning:** This method should be used with caution as it can cause deadlocks if not used properly. It's best to use it in editor scripts or initialization code, not in gameplay code.

**Example:**

```csharp
// Get a reference to the Rally Network
IRallyNetwork rlyNetwork = RallyUnityManager.Instance.RlyNetwork;

// In a context where async/await can't be used (like OnValidate or editor scripts)
Account account = rlyNetwork.GetAccountAsync().WaitForCompletion();
```

#### ContinueWithUnity

```csharp
public static Task<T> ContinueWithUnity<T>(this Task<T> task, Action<T> action)
```

Continues a task on the Unity main thread, allowing for safe UI updates from background tasks.

**Example:**

```csharp
async void CheckBalanceAndUpdateUI()
{
    // Start the balance check
    Task<decimal> balanceTask = rlyNetwork.GetDisplayBalanceAsync();
    
    // Continue on the Unity main thread to update UI
    await balanceTask.ContinueWithUnity(balance => {
        // This code runs on the Unity main thread
        balanceText.text = $"Balance: {balance} RLY";
    });
}
```

### Transform Extensions

#### SetRallyWorldPosition

```csharp
public static void SetRallyWorldPosition(this Transform transform, Vector3 position)
```

Sets a world position with Rally Protocol-specific behavior (such as special animation or effects).

**Example:**

```csharp
// Move a token reward to the player
tokenRewardObject.transform.SetRallyWorldPosition(player.transform.position);
```

### UI Extensions

#### SetupRallyButton

```csharp
public static void SetupRallyButton(this Button button, Action onClick, bool interactable = true)
```

Configures a Unity Button with Rally Protocol styling and behavior.

**Example:**

```csharp
// Set up a claim tokens button
claimButton.SetupRallyButton(() => {
    StartCoroutine(ClaimTokensCoroutine());
});

// Coroutine for claiming tokens
private IEnumerator ClaimTokensCoroutine()
{
    // Show loading state
    loadingPanel.SetActive(true);
    
    // Start the async operation
    Task<string> claimTask = rlyNetwork.ClaimRlyAsync();
    
    // Wait for it to complete
    while (!claimTask.IsCompleted)
    {
        yield return null;
    }
    
    // Hide loading state
    loadingPanel.SetActive(false);
    
    // Handle result
    if (claimTask.IsFaulted)
    {
        Debug.LogError($"Failed to claim tokens: {claimTask.Exception}");
    }
    else
    {
        Debug.Log($"Tokens claimed! Transaction: {claimTask.Result}");
    }
}
```

#### ConfigureRallyInputField

```csharp
public static void ConfigureRallyInputField(this TMP_InputField inputField, Action<string> onValueChanged)
```

Configures a TextMeshPro input field with Rally Protocol validation and styling.

**Example:**

```csharp
// Configure an address input field
addressInputField.ConfigureRallyInputField(address => {
    // Validate the address
    bool isValid = Regex.IsMatch(address, "^0x[a-fA-F0-9]{40}$");
    submitButton.interactable = isValid;
});
```

### BigInteger Extensions

#### ToRlyDisplayString

```csharp
public static string ToRlyDisplayString(this BigInteger value, int decimals = 18)
```

Converts a BigInteger token amount to a human-readable string with proper formatting.

**Example:**

```csharp
// Get exact token balance
BigInteger exactBalance = await rlyNetwork.GetExactBalanceAsync();

// Convert to display string
string displayBalance = exactBalance.ToRlyDisplayString();
balanceText.text = $"Balance: {displayBalance} RLY";
```

### String Extensions

#### ToShortenedAddress

```csharp
public static string ToShortenedAddress(this string address)
```

Shortens an Ethereum address for display purposes (e.g., "0x1234...5678").

**Example:**

```csharp
string userAddress = await rlyNetwork.AccountManager.GetPublicAddressAsync();
if (userAddress != null)
{
    addressLabel.text = userAddress.ToShortenedAddress();
}
```

### Component Extensions

#### AddRallyComponent

```csharp
public static T AddRallyComponent<T>(this GameObject gameObject) where T : Component
```

Adds a Rally Protocol component to a GameObject with automatic initialization.

**Example:**

```csharp
// Add a token display component
var tokenDisplay = gameObject.AddRallyComponent<RallyTokenDisplay>();
```

## Platform-Specific Extensions

### iOS Extensions

```csharp
#if UNITY_IOS
public static void ConfigureForIOS(this IRallyNetwork network)
```

Configures the Rally Network with iOS-specific optimizations.

**Example:**

```csharp
#if UNITY_IOS
rlyNetwork.ConfigureForIOS();
#endif
```

### Android Extensions

```csharp
#if UNITY_ANDROID
public static void ConfigureForAndroid(this IRallyNetwork network)
```

Configures the Rally Network with Android-specific optimizations.

**Example:**

```csharp
#if UNITY_ANDROID
rlyNetwork.ConfigureForAndroid();
#endif
```

## Coroutine Helpers

### RunRallyTask

```csharp
public static Coroutine RunRallyTask<T>(this MonoBehaviour monoBehaviour, Task<T> task, Action<T> onComplete, Action<Exception> onError = null)
```

Runs a Rally Protocol task as a coroutine, handling success and error callbacks.

**Example:**

```csharp
// In a MonoBehaviour class
void TransferTokens()
{
    this.RunRallyTask(
        rlyNetwork.TransferAsync("0xRecipientAddress", 10.5m),
        txHash => {
            // Success callback
            Debug.Log($"Transfer successful! Transaction: {txHash}");
        },
        error => {
            // Error callback
            Debug.LogError($"Transfer failed: {error.Message}");
        }
    );
}
```

## Event System Integration

### AddRallyListener

```csharp
public static void AddRallyListener(this UnityEvent unityEvent, Action callback)
```

Adds a listener to a UnityEvent with Rally Protocol-specific behavior.

**Example:**

```csharp
// Set up a button click event
claimButton.onClick.AddRallyListener(() => {
    this.RunRallyTask(
        rlyNetwork.ClaimRlyAsync(),
        txHash => Debug.Log($"Claimed tokens! Transaction: {txHash}"),
        error => Debug.LogError($"Failed to claim tokens: {error.Message}")
    );
});
```

## Unity Editor Extensions

The SDK also provides editor extensions to enhance the development experience:

```csharp
#if UNITY_EDITOR
public static void ValidateRallyConfiguration(this RallyUnityManager manager)
```

Validates the Rally Protocol configuration in the Unity Editor.

## Best Practices

1. **Use the main thread extensions** for UI updates to avoid threading issues
2. **Leverage coroutine helpers** for better Unity integration with async operations
3. **Apply platform-specific extensions** when targeting mobile platforms
4. **Wrap long-running operations** in coroutines to avoid blocking the main thread
5. **Handle exceptions properly** in all task-based operations

## Related Documentation

- [RallyUnityManager](./RallyUnityManager.md) - Unity-specific manager for Rally Protocol
- [Task-Based Programming](./task-based-programming.md) - Guide to task-based programming with Rally Protocol
- [Unity UI Integration](./unity-ui-integration.md) - Detailed guide on UI integration
