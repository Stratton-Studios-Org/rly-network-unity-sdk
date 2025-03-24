# Rally Protocol Unity SDK Troubleshooting Guide

This troubleshooting guide addresses common issues that developers may encounter when working with the Rally Protocol Unity SDK. It provides diagnostic steps and solutions for various scenarios.

## Table of Contents

- [Connection Issues](#connection-issues)
- [Authentication Errors](#authentication-errors)
- [Transaction Failures](#transaction-failures)
- [Wallet Integration Problems](#wallet-integration-problems)
- [Platform-Specific Issues](#platform-specific-issues)
- [Performance Problems](#performance-problems)
- [Update and Compatibility Issues](#update-and-compatibility-issues)
- [Error Codes Reference](#error-codes-reference)

## Connection Issues

### Unable to Connect to Rally Network

**Symptoms:**

- "Could not connect to Rally network" errors
- Operations fail with network errors
- SDK initialization fails

**Possible Causes:**

1. No internet connection
2. Incorrect API key
3. Network configuration issues
4. Rally Protocol services may be down

**Solutions:**

1. **Verify Internet Connection**

   ```csharp
   // Check if the device is connected to the internet
   if (Application.internetReachability == NetworkReachability.NotReachable)
   {
       Debug.LogError("No internet connection available");
       // Show user-friendly message suggesting to check connection
   }
   ```

2. **Validate API Key**
   - Ensure your API key is correctly set in the configuration
   - Check that the API key hasn't expired
   - Verify the API key has the necessary permissions

3. **Check Network Configuration**
   - Ensure the correct network (testnet/mainnet) is configured
   - Verify firewall settings aren't blocking connections
   - Try enabling the detailed logging to see specific connection errors:

   ```csharp
   // Enable detailed logging through the Unity logger
   if (Debug.unityLogger.logEnabled)
   {
       Debug.unityLogger.filterLogType = LogType.Log; // Shows all logs including debug
       Debug.Log("Detailed logging enabled for Rally Protocol SDK");
   }
   ```

4. **Service Status Check**
   - Check the Rally Protocol status page for service outages
   - Try connecting to a different endpoint if available

### Connection Timeout Issues

**Symptoms:**

- Operations take a long time to complete
- Requests eventually fail with timeout errors

**Solutions:**

1. **Implement Custom Network Configuration**

   ```csharp
   // Create a custom network configuration with adjusted timeout
   var customConfig = new RallyNetworkConfig
   {
       // Other config settings...
       HttpRequestTimeoutMs = 30000, // 30 seconds timeout
   };
   
   // Initialize with the custom configuration
   var rlyNetwork = RallyUnityNetworkFactory.Create(customConfig, "your-api-key");
   ```

2. **Implement Retry Logic**

   ```csharp
   private async Task<T> ExecuteWithRetry<T>(Func<Task<T>> operation, int maxRetries = 3)
   {
       int retryCount = 0;
       while (true)
       {
           try
           {
               return await operation();
           }
           catch (Exception ex) when (IsNetworkException(ex) && retryCount < maxRetries)
           {
               retryCount++;
               Debug.LogWarning($"Operation failed, retrying ({retryCount}/{maxRetries}): {ex.Message}");
               await Task.Delay(1000 * retryCount); // Exponential backoff
           }
       }
   }
   
   private bool IsNetworkException(Exception ex)
   {
       // Determine if exception is network-related
       return ex is TimeoutException || 
              ex is System.Net.WebException ||
              ex.Message.Contains("network");
   }
   ```

## Authentication Errors

### Failed to Authenticate User

**Symptoms:**

- "Authentication failed" errors
- Unable to perform actions requiring authentication
- User session errors

**Possible Causes:**

1. Invalid or expired credentials
2. Incorrect authentication flow
3. Account permissions issues

**Solutions:**

1. **Verify Credentials**
   - Ensure that user credentials are valid
   - Check if token has expired and needs refreshing

2. **Proper Authentication Sequence**

   ```csharp
   // Example of proper authentication flow
   try
   {
       // First initialize the SDK
       var rlyNetwork = RallyUnityNetworkFactory.Create();
       
       // Then verify the account is accessible
       var account = await rlyNetwork.GetAccountAsync();
       
       if (account == null)
       {
           Debug.LogError("Account not available - check initialization");
           // Prompt user to create or import an account
       }
   }
   catch (Exception ex)
   {
       Debug.LogError($"Authentication error: {ex.Message}");
   }
   ```

3. **Check SDK Logs**
   - Enable detailed logging to see specific authentication errors
   - Look for error codes and messages in the logs

### User Session Expired

**Symptoms:**

- Operations suddenly start failing after previously working
- "Session expired" or "Unauthorized" errors

**Solutions:**

1. **Reinitialize If Needed**

   ```csharp
   // If operations start failing, try reinitializing
   try
   {
       await rlyNetwork.SomeOperationAsync();
   }
   catch (Exception ex) when (ex.Message.Contains("session expired") || ex.Message.Contains("unauthorized"))
   {
       Debug.Log("Session appears to be expired, attempting to reinitialize");
       
       try
       {
           // Recreate the network with valid credentials
           rlyNetwork = RallyUnityNetworkFactory.Create();
           
           // Retry the operation
           await rlyNetwork.SomeOperationAsync();
       }
       catch (Exception reinitEx)
       {
           Debug.LogError("Could not reinitialize network: " + reinitEx.Message);
           // Show login UI
       }
   }
   ```

## Transaction Failures

### Transaction Fails to Submit

**Symptoms:**

- Transfers, claims, or other transactions fail to submit
- "Transaction failed" errors
- Gas errors

**Possible Causes:**

1. Insufficient funds for gas
2. Network congestion
3. Invalid transaction parameters
4. Contract interaction issues

**Solutions:**

1. **Check Balance for Gas**

   ```csharp
   // Check if user has enough balance for gas
   var balance = await rlyNetwork.GetBalanceAsync();
   float minRequired = 0.01f; // Minimum required for gas (adjust based on network)
   
   if (balance < minRequired)
   {
       Debug.LogWarning($"Insufficient balance for gas: {balance}. Minimum required: {minRequired}");
       // Inform user about insufficient funds
   }
   ```

2. **Use Appropriate Gas Settings**

   ```csharp
   // If your implementation supports gas settings through TransferOptions 
   // (Note: check actual implementation as this may vary)
   try {
       // Example of how you might set gas parameters if supported
       var transferOptions = new Dictionary<string, object>
       {
           ["gasLimit"] = 150000,
           ["gasMultiplier"] = 1.2f
       };
       
       await rlyNetwork.TransferAsync(destination, amount, transferOptions);
   }
   catch (Exception ex) {
       Debug.LogError($"Transfer failed: {ex.Message}");
   }
   ```

### Pending Transactions Never Confirm

**Symptoms:**

- Transactions stay in "pending" state indefinitely
- No confirmation or error is received

**Solutions:**

1. **Transaction Monitoring**

   ```csharp
   private async Task MonitorTransaction(string txHash, int timeoutSeconds = 180)
   {
       int pollIntervalSeconds = 5;
       int attempts = timeoutSeconds / pollIntervalSeconds;
       
       for (int i = 0; i < attempts; i++)
       {
           var status = await rlyNetwork.GetTransactionStatusAsync(txHash);
           
           if (status == "confirmed")
           {
               Debug.Log($"Transaction {txHash} confirmed successfully");
               return;
           }
           else if (status == "failed")
           {
               Debug.LogError($"Transaction {txHash} failed");
               throw new Exception($"Transaction failed: {txHash}");
           }
           
           Debug.Log($"Transaction {txHash} still pending. Checking again in {pollIntervalSeconds} seconds...");
           await Task.Delay(pollIntervalSeconds * 1000);
       }
       
       Debug.LogWarning($"Transaction {txHash} still pending after {timeoutSeconds} seconds");
       // At this point, you might want to let the user know that the transaction
       // is taking longer than expected but may still confirm later
   }
   ```

2. **Handling Stuck Transactions**
   - If a transaction is stuck, sometimes sending a new transaction with the same nonce
     but higher gas price can help (this is called "replacing" the transaction)
   - If your SDK supports it, implement a "speed up" function:

   ```csharp
   // Example function to speed up a stuck transaction
   public async Task<string> SpeedUpTransaction(string originalTxHash)
   {
       // This would depend on your SDK's capabilities
       return await rlyNetwork.SpeedUpTransactionAsync(
           originalTxHash, 
           gasPriceMultiplier: 1.5f
       );
   }
   ```

## Wallet Integration Problems

### Unable to Connect to External Wallet

**Symptoms:**

- External wallet connection fails
- "Connection rejected" or similar errors
- Timeout when trying to connect to a wallet

**Possible Causes:**

1. Wallet app not installed
2. Wallet doesn't support the connection method
3. User rejected the connection
4. Connection timeout

**Solutions:**

1. **Check Wallet Installation**

   ```csharp
   // For mobile platforms - check if the wallet app is installed
   public bool IsWalletAppInstalled(string walletScheme)
   {
   #if UNITY_ANDROID && !UNITY_EDITOR
       AndroidJavaClass intentClass = new AndroidJavaClass("android.content.Intent");
       AndroidJavaObject intentObject = new AndroidJavaObject("android.content.Intent");
       intentObject.Call<AndroidJavaObject>("setAction", intentClass.GetStatic<string>("ACTION_VIEW"));
       intentObject.Call<AndroidJavaObject>("setData", AndroidJavaObject.CallStatic<AndroidJavaObject>("android.net.Uri", "parse", walletScheme + "://"));
       
       AndroidJavaClass unityPlayerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
       AndroidJavaObject currentActivity = unityPlayerClass.GetStatic<AndroidJavaObject>("currentActivity");
       AndroidJavaObject packageManager = currentActivity.Call<AndroidJavaObject>("getPackageManager");
       
       AndroidJavaObject list = packageManager.Call<AndroidJavaObject>("queryIntentActivities", intentObject, 0);
       bool installed = list.Call<int>("size") > 0;
       
       return installed;
   #elif UNITY_IOS && !UNITY_EDITOR
       // iOS implementation would use Objective-C interop
       return CheckWalletInstalled_iOS(walletScheme);
   #else
       // For editor testing
       return true;
   #endif
   }
   ```

2. **Provide Clear Feedback**

   ```csharp
   public async Task ConnectToWallet()
   {
       try
       {
           // Show connecting UI
           ShowConnectingUI();
           
           // Set timeout for the connection
           var timeoutToken = new CancellationTokenSource(TimeSpan.FromSeconds(30));
           
           var result = await rlyNetwork.ConnectWalletAsync(timeoutToken.Token);
           
           if (result.Success)
           {
               Debug.Log($"Wallet connected: {result.WalletAddress}");
               ShowSuccessUI();
           }
           else
           {
               Debug.LogError($"Wallet connection failed: {result.ErrorMessage}");
               ShowErrorUI(result.ErrorMessage);
           }
       }
       catch (OperationCanceledException)
       {
           Debug.LogError("Wallet connection timed out");
           ShowTimeoutUI();
       }
       catch (Exception ex)
       {
           Debug.LogError($"Wallet connection error: {ex.Message}");
           ShowErrorUI(ex.Message);
       }
   }
   ```

3. **Alternative Connection Methods**
   - If direct connection fails, offer alternative methods:

   ```csharp
   // Example of offering different connection methods
   public void OfferConnectionOptions()
   {
       // Option 1: Direct connection (WalletConnect, etc.)
       if (IsWalletConnectSupported())
       {
           ShowOption("Connect directly", () => ConnectWalletDirect());
       }
       
       // Option 2: Deep link to wallet app
       if (CanDeeplinkToWallet())
       {
           ShowOption("Open wallet app", () => OpenWalletApp());
       }
       
       // Option 3: QR code scanning
       ShowOption("Scan QR code", () => ShowQRCodeForConnection());
       
       // Option 4: Manual address entry
       ShowOption("Enter address manually", () => ShowManualAddressEntryUI());
   }
   ```

### Incorrect Wallet Address Format

**Symptoms:**

- "Invalid address" errors
- Transactions fail with address validation errors

**Solutions:**

1. **Address Validation**

   ```csharp
   public bool IsValidEthereumAddress(string address)
   {
       // Basic validation - should start with "0x" and be 42 characters long
       if (!address.StartsWith("0x") || address.Length != 42)
       {
           return false;
       }
       
       // Check if it contains only hex characters after "0x"
       string hexPart = address.Substring(2);
       return System.Text.RegularExpressions.Regex.IsMatch(hexPart, "^[0-9a-fA-F]{40}$");
   }
   ```

2. **Address Normalization**

   ```csharp
   public string NormalizeAddress(string address)
   {
       // Ensure address has 0x prefix
       if (!address.StartsWith("0x"))
       {
           address = "0x" + address;
       }
       
       // Convert to lowercase for consistency
       return address.ToLowerInvariant();
   }
   ```

## Platform-Specific Issues

> **Important Note**: The Rally Protocol SDK only supports iOS and Android for deployment. The Unity Editor is supported on Windows, macOS, and Linux for development purposes only. Standalone builds for desktop platforms are not supported.

### iOS-Specific Problems

**Symptoms:**

- SDK works in editor but fails on iOS devices
- iOS builds fail with cryptography or network errors
- App Store submission rejections

**Solutions:**

1. **iOS Network Security Configuration**
   - Ensure your Info.plist has necessary configurations

   ```xml
   <!-- Add to Info.plist -->
   <key>NSAppTransportSecurity</key>
   <dict>
       <key>NSAllowsArbitraryLoads</key>
       <true/>
   </dict>
   ```

2. **iOS Entitlements Issues**
   - Check that your app has the correct entitlements for cryptography

3. **iOS Bitcode Compatibility**
   - If using older iOS targets, ensure Bitcode is disabled in Xcode settings

### Android-Specific Problems

**Symptoms:**

- SDK works in editor but fails on Android devices
- Permission issues on Android
- Crashes on specific Android versions

**Solutions:**

1. **Android Permissions**
   - Ensure your AndroidManifest.xml has necessary permissions

   ```xml
   <!-- Add to AndroidManifest.xml -->
   <uses-permission android:name="android.permission.INTERNET" />
   <uses-permission android:name="android.permission.ACCESS_NETWORK_STATE" />
   ```

2. **Android API Level Issues**

   ```csharp
   // Check Android API level and apply workarounds if needed
   private void ApplyAndroidWorkarounds()
   {
   #if UNITY_ANDROID && !UNITY_EDITOR
       using (var version = new AndroidJavaClass("android.os.Build$VERSION"))
       {
           int apiLevel = version.GetStatic<int>("SDK_INT");
           
           if (apiLevel < 21)
           {
               // Apply pre-Lollipop workarounds
               Debug.LogWarning("Running on older Android version - applying compatibility fixes");
               // ...workaround code...
           }
       }
   #endif
   }
   ```

3. **WebView Issues on Android**
   - If using WebView for wallet connections, ensure it's correctly configured:

   ```csharp
   #if UNITY_ANDROID && !UNITY_EDITOR
   // Configure WebView for proper security
   private void ConfigureWebView(AndroidJavaObject webView)
   {
       webView.Call("setWebContentsDebuggingEnabled", true);
       
       AndroidJavaObject settings = webView.Call<AndroidJavaObject>("getSettings");
       settings.Call("setJavaScriptEnabled", true);
       settings.Call("setDomStorageEnabled", true);
       settings.Call("setDatabaseEnabled", true);
       settings.Call("setAllowFileAccess", false);
       
       // Modern WebView on Android needs this for most wallet connections
       settings.Call("setAllowUniversalAccessFromFileURLs", true);
   }
   #endif
   ```

### WebGL-Specific Problems

**Symptoms:**

- SDK works in editor but fails in WebGL builds
- Console errors about CORS or security policies
- MetaMask or other browser wallet connection issues

**Solutions:**

1. **CORS Issues**
   - Configure your web server to allow cross-origin requests
   - Use a proxy if developing locally

2. **WebGL Security Limitations**

   ```csharp
   // Check if running in WebGL and apply specific handling
   private void ConfigureForWebGL()
   {
   #if UNITY_WEBGL
       Debug.Log("Running in WebGL - applying WebGL-specific configuration");
       
       // In WebGL, we may need different network initialization
       // Specific details would depend on your implementation
       
       // Other WebGL-specific settings
   #endif
   }
   ```

3. **Browser Wallet Integration**

   ```csharp
   #if UNITY_WEBGL && !UNITY_EDITOR
   // Example of JavaScript interop for wallet connection
   [DllImport("__Internal")]
   private static extern void ConnectToMetaMask();
   
   public void ConnectWalletForWebGL()
   {
       ConnectToMetaMask();
   }
   #endif
   ```

## Performance Problems

### Slow Operations

**Symptoms:**

- SDK operations take a long time to complete
- UI freezes during blockchain operations
- High CPU or memory usage

**Solutions:**

1. **Async Operations and Threading**

   ```csharp
   // Ensure operations run asynchronously and don't block the main thread
   public async void PerformBlockchainOperationSafely()
   {
       try
       {
           // Show loading indicator
           loadingIndicator.SetActive(true);
           
           // Run blockchain operation
           await UniTask.RunOnThreadPool(async () => {
               var result = await rlyNetwork.LongRunningOperationAsync();
               
               // Process result
               // Note: UI updates should be dispatched back to the main thread
               await UniTask.SwitchToMainThread();
               UpdateUI(result);
           });
       }
       catch (Exception ex)
       {
           Debug.LogError($"Operation failed: {ex.Message}");
           // Show error UI
       }
       finally
       {
           // Hide loading indicator
           loadingIndicator.SetActive(false);
       }
   }
   ```

2. **Caching Strategies**

   ```csharp
   // Example of a simple cache to reduce network calls
   public class BlockchainDataCache
   {
       private Dictionary<string, (object Data, DateTime Expiry)> cache = 
           new Dictionary<string, (object Data, DateTime Expiry)>();
       
       public void Set<T>(string key, T data, TimeSpan expiration)
       {
           cache[key] = (data, DateTime.UtcNow.Add(expiration));
       }
       
       public bool TryGet<T>(string key, out T data)
       {
           data = default;
           
           if (cache.TryGetValue(key, out var entry))
           {
               if (DateTime.UtcNow < entry.Expiry)
               {
                   data = (T)entry.Data;
                   return true;
               }
               else
               {
                   // Entry expired, remove it
                   cache.Remove(key);
               }
           }
           
           return false;
       }
       
       public void Clear()
       {
           cache.Clear();
       }
   }
   
   // Using the cache
   private BlockchainDataCache dataCache = new BlockchainDataCache();
   
   public async Task<decimal> GetBalanceWithCaching(string address)
   {
       string cacheKey = $"balance:{address}";
       
       if (dataCache.TryGet<decimal>(cacheKey, out var cachedBalance))
       {
           Debug.Log("Using cached balance");
           return cachedBalance;
       }
       
       var balance = await rlyNetwork.GetBalanceAsync(address);
       
       // Cache for 1 minute
       dataCache.Set(cacheKey, balance, TimeSpan.FromMinutes(1));
       
       return balance;
   }
   ```

3. **Optimizing Multiple Operations**

   ```csharp
   // Example of optimizing multiple related operations
   public async Task<List<decimal>> GetMultipleBalances(List<string> addresses)
   {
       // Instead of making separate calls for each address
       var tasks = addresses.Select(address => rlyNetwork.GetBalanceAsync(address));
       
       // Wait for all operations to complete
       return await Task.WhenAll(tasks);
   }
   ```

### Memory Leaks

**Symptoms:**

- Increasing memory usage over time
- Eventual out-of-memory crashes
- Performance degradation with extended use

**Solutions:**

1. **Event Unsubscription**

   ```csharp
   // Always unsubscribe from events when no longer needed
   private void OnEnable()
   {
       // Example of subscribing to component events
       RallyTransfer transferComponent = GetComponent<RallyTransfer>();
       RallyClaimRly claimComponent = GetComponent<RallyClaimRly>();
       
       if (transferComponent != null)
       {
           transferComponent.Transferring += OnTransferring;
           transferComponent.Transferred += OnTransferred;
       }
       
       if (claimComponent != null)
       {
           claimComponent.Claiming += OnClaiming;
           claimComponent.Claimed += OnClaimed;
       }
   }
   
   private void OnDisable()
   {
       // Important: Prevent memory leaks by unsubscribing
       RallyTransfer transferComponent = GetComponent<RallyTransfer>();
       RallyClaimRly claimComponent = GetComponent<RallyClaimRly>();
       
       if (transferComponent != null)
       {
           transferComponent.Transferring -= OnTransferring;
           transferComponent.Transferred -= OnTransferred;
       }
       
       if (claimComponent != null)
       {
           claimComponent.Claiming -= OnClaiming;
           claimComponent.Claimed -= OnClaimed;
       }
   }
   
   // Event handlers
   private void OnTransferring(object sender, EventArgs e)
   {
       Debug.Log("Transfer started");
   }
   
   private void OnTransferred(object sender, RallyTransferEventArgs e)
   {
       Debug.Log($"Transfer completed: {e.TransactionHash}");
   }
   
   private void OnClaiming(object sender, EventArgs e)
   {
       Debug.Log("Claiming started");
   }
   
   private void OnClaimed(object sender, RallyClaimRlyEventArgs e)
   {
       Debug.Log($"Claim completed: {e.TransactionHash}");
   }
   ```

2. **Resource Disposal**

   ```csharp
   // Proper disposal of resources
   public async Task UseResourcesAndDispose()
   {
       var client = new HttpClient();
       try
       {
           // Use the client
           var response = await client.GetAsync("https://api.rallyprotocol.com/endpoint");
           // Process response
       }
       finally
       {
           // Properly dispose resources
           client.Dispose();
       }
   }
   ```

3. **Memory Monitoring**

   ```csharp
   // Monitor for memory issues
   private void Start()
   {
       StartCoroutine(MonitorMemoryUsage());
   }
   
   private IEnumerator MonitorMemoryUsage()
   {
       while (true)
       {
           yield return new WaitForSeconds(30); // Check every 30 seconds
           
           long totalMemory = System.GC.GetTotalMemory(false);
           Debug.Log($"Current memory usage: {totalMemory / (1024 * 1024)} MB");
           
           if (totalMemory > 1000 * 1024 * 1024) // 1000 MB threshold
           {
               Debug.LogWarning("Memory usage is high - consider clearing caches");
               // Clear caches or take other actions to reduce memory usage
           }
       }
   }
   ```

## Update and Compatibility Issues

### SDK Version Mismatch

**Symptoms:**

- Errors about missing or incompatible methods
- Features don't work as expected after upgrading
- Unexpected behavior after SDK update

**Solutions:**

1. **Version Checking**

   ```csharp
   // Check for SDK compatibility at startup
   private void Start()
   {
       // Implement your own version checking mechanism
       // The SDK does not provide direct version access methods
       CheckSDKCompatibility();
   }
   
   private void CheckSDKCompatibility()
   {
       // Your version checking implementation here
       // For example, check features or compatibility markers
       bool isCompatible = ValidateSDKFeatures();
       
       if (!isCompatible)
       {
           Debug.LogError("Current SDK version is not compatible with this app. Please update to the latest version.");
           ShowVersionErrorUI();
       }
   }
   ```

2. **Migration Guide**
   - When upgrading, follow any migration guide provided with the SDK
   - Check for deprecated methods and their replacements
   - Update code to use new APIs as needed

3. **Dependency Conflicts**
   - Check for conflicts between the SDK and other packages
   - Look for duplicate libraries or conflicting versions
   - Use Unity's package manager to resolve conflicts

## Error Codes Reference

| Error Code | Description | Possible Solutions |
|------------|-------------|-------------------|
| RLY-1001   | Network connection failed | Check internet connection, verify API endpoints |
| RLY-1002   | Authentication failed | Verify credentials, check if account is active |
| RLY-1003   | Insufficient balance | Ensure account has enough tokens for the operation |
| RLY-2001   | Transaction rejected | Check parameters, verify gas settings |
| RLY-2002   | Contract execution failed | Verify contract address, check function parameters |
| RLY-3001   | Wallet connection failed | Ensure wallet is installed and accessible |
| RLY-4001   | Rate limit exceeded | Implement backoff strategy, reduce request frequency |
| RLY-5001   | Invalid signature | Check signing key, ensure data is formatted correctly |
| RLY-9001   | Internal SDK error | Check logs for details, update to latest SDK version |

## Debug Logging Tips

Enable detailed logging to help troubleshoot issues:

```csharp
// Set Unity logger to verbose mode
Debug.unityLogger.logEnabled = true;
Debug.unityLogger.filterLogType = LogType.Log;

// If you're implementing a custom logger, you can do something like:
public class CustomRallyLogger : IRallyLogger
{
    public void Log(string message)
    {
        Debug.Log($"[RLY] {message}");
        // Also log to file or custom system
        LogToFile($"[RLY] {message}");
    }
    
    public void LogError(string message)
    {
        Debug.LogError($"[RLY] {message}");
        // Maybe also send to error reporting service
        ReportError($"[RLY] {message}");
    }
    
    public void LogWarning(string message)
    {
        Debug.LogWarning($"[RLY] {message}");
    }
    
    public void LogException(Exception ex)
    {
        Debug.LogException(ex);
        // Send to error reporting service
        ReportError($"[RLY] Exception: {ex.Message}");
    }
    
    private void LogToFile(string message)
    {
        // Implement file logging
        System.IO.File.AppendAllText("rally_sdk_log.txt", $"{DateTime.Now}: {message}\n");
    }
    
    private void ReportError(string message)
    {
        // Implement error reporting to your service
        // e.g., Firebase Crashlytics, Sentry, etc.
    }
}

// Then use it with the network factory
var customLogger = new CustomRallyLogger();
var rlyNetwork = RallyNetworkFactory.Create(
    RallyUnityNetworkFactory.GetWeb3Provider(),
    RallyUnityNetworkFactory.GetHttpHandler(),
    customLogger,
    RallyUnityNetworkFactory.GetAccountManager(),
    RallyNetworkType.Testnet,
    "your-api-key"
);
```

## Getting Further Help

If you're still experiencing issues:

1. **Check the Documentation**
   - Review the [official Rally Protocol documentation](https://docs.rallyprotocol.com)
   - Look for code examples and tutorials

2. **Community Resources**
   - Visit the [Rally Protocol Discord server](https://discord.gg/rlynetwork)
   - Check the [GitHub repository](https://github.com/rally-dfs/rly-network-unity-sdk) for known issues

3. **Support Channels**
   - Contact support at <support@rallyprotocol.com>
   - Open a GitHub issue with detailed information about your problem

4. **Provide Complete Information**
   When seeking help, always include:
   - SDK version
   - Unity version
   - Platform and OS version
   - Complete error message and stack trace
   - Steps to reproduce the issue
   - Code snippets demonstrating the problem
