# Common Issues and Solutions

This guide addresses the most common issues you might encounter when working with the Rally Protocol Unity SDK and provides solutions to help you resolve them.

## Account Management Issues

### Issue: Cannot Create Account

**Symptoms**:

- Account creation fails with timeout or error
- Unity console shows network-related errors during account creation

**Possible Causes**:

- Network connectivity issues
- Incorrect API key configuration
- Permission issues with storage

**Solutions**:

- Verify internet connection
- Check that your API key is correctly configured in the Rally Protocol settings
- Ensure proper permissions for data storage
- Try using the offline account creation method

```csharp
// Example: Creating an account with offline option
await accountManager.CreateAccountAsync(new()
{
    Overwrite = false,
    StorageOptions = new()
    {
        SaveToCloud = false
    }
});
```

### Issue: Cannot Access Existing Account

**Symptoms**:

- "Account not found" errors
- SDK fails to load previously created accounts

**Solutions**:

- Verify that account creation was successful previously
- Check storage permissions
- Ensure cloud sync settings are consistent with your implementation
- Try manual account recovery if available

## Token Operations Issues

### Issue: Failed Token Claims

**Symptoms**:

- Token claim transactions fail
- Error messages related to gas fees or network timeouts

**Solutions**:

- Verify network connectivity
- Check that the user is eligible for token claims
- Ensure Gas Station Network (GSN) is properly configured
- Try again later if network congestion may be an issue

### Issue: Token Transfer Failures

**Symptoms**:

- Transfer transactions consistently fail
- Error messages about insufficient funds or gas

**Solutions**:

- Verify the recipient address is valid
- Check that the user has sufficient token balance
- Ensure GSN configuration is correct
- Check that transfer amount is within allowed limits

## Transaction Troubleshooting

### Issue: Transaction Pending for Extended Periods

**Symptoms**:

- Transactions stay in "pending" state for abnormally long periods
- User interface appears stuck waiting for transaction confirmation

**Possible Causes**:

- Network congestion
- Gas price too low (for non-GSN transactions)
- RPC endpoint issues
- Transaction nonce conflicts

**Solutions**:

- Implement proper timeout handling in your application

```csharp
// Example: Transaction with timeout
public async Task<bool> ExecuteTransactionWithTimeout(Func<Task<string>> transactionFunc, int timeoutSeconds = 60)
{
    try
    {
        var timeoutTask = Task.Delay(timeoutSeconds * 1000);
        var transactionTask = transactionFunc();
        
        var completedTask = await Task.WhenAny(transactionTask, timeoutTask);
        
        if (completedTask == timeoutTask)
        {
            // Transaction timed out
            Debug.LogWarning("Transaction timed out");
            return false;
        }
        
        // Transaction completed within timeout
        string txHash = await transactionTask;
        Debug.Log($"Transaction successful: {txHash}");
        return true;
    }
    catch (Exception e)
    {
        Debug.LogError($"Transaction failed: {e.Message}");
        return false;
    }
}

// Usage
await ExecuteTransactionWithTimeout(async () => await rlyNetwork.TransferAsync(destinationAddress, amount));
```

- Provide clear feedback to users about transaction status
- Consider implementing a transaction queue system to prevent nonce conflicts

### Issue: Transaction Failed with Error

**Symptoms**:

- Transaction attempts result in error responses
- Error messages in console logs related to transaction execution

**Common Error Messages and Solutions**:

1. **"Insufficient funds for gas"**:
   - Make sure the account has enough ETH for gas fees
   - If using GSN, ensure the paymaster has sufficient funds and is correctly configured

2. **"Nonce too high"** or **"Nonce too low"**:
   - Reset the account's transaction count by calling:

   ```csharp
   await rlyNetwork.ResetTransactionCountAsync();
   ```

3. **"Contract execution reverted"**:
   - The transaction violated a rule in the smart contract
   - Check token allowances, balance limitations, or other restrictions

4. **"Known transaction"**:
   - A transaction with the same parameters was recently submitted
   - Implement deduplication or add a small random amount to make transactions unique

### Issue: GSN Relay Issues

**Symptoms**:

- Transactions fail with GSN-related errors
- Users can't perform gasless transactions

**Solutions**:

- Check GSN configuration:

```csharp
// Example of proper GSN configuration
var gsnConfig = new GSNConfigOptions
{
    RelayLookupWindowBlocks = 1000,
    RelayRegistrationLookupBlocks = 6000,
    PastEventQueryMaxBlocks = 10000,
    ActivityTimeout = 180 // seconds
};

await rlyNetwork.InitializeGSNAsync(gsnConfig);
```

- Ensure the Paymaster contract has sufficient funds
- Try connecting to a different GSN relay server
- Check if the user's account is authorized by the Paymaster

## Network and Integration Issues

### Issue: Network Connection Failures

**Symptoms**:

- Consistent timeouts when interacting with the blockchain
- API errors suggesting network connectivity problems

**Solutions**:

- Check internet connectivity
- Verify firewall settings aren't blocking blockchain connections
- Ensure the selected network (testnet/mainnet) is operational
- Try switching to a different network endpoint if available

### Issue: Platform-Specific Integration Problems

**Symptoms**:

- Features work on one platform but not another
- Native plugins fail to load

**Solutions**:

- Ensure all platform-specific plugins are correctly installed
- Check platform-specific configuration in your project settings
- See the [Android Integration Guide](../platform-guides/android-integration.md) or [iOS Integration Guide](../platform-guides/ios-integration.md) for platform-specific troubleshooting

## Unity Editor Issues

### Issue: SDK Not Functioning in Editor

**Symptoms**:

- SDK functions work in builds but not in the Unity Editor
- Error messages about missing dependencies

**Solutions**:

- Verify SDK is correctly configured for Editor use
- Check for any Editor-specific settings that need to be adjusted
- Try clearing the Library folder and restarting Unity
- Update to the latest SDK version

## Performance Issues

### Issue: Slow Transaction Times

**Symptoms**:

- Blockchain operations take unusually long to complete
- UI freezes during blockchain operations

**Solutions**:

- Implement asynchronous operations correctly
- Use threading best practices
- Consider implementing loading indicators for long-running operations
- Check network performance and connection quality

## Upgrade and Compatibility Issues

### Issue: Breaking Changes After SDK Update

**Symptoms**:

- Code that worked in previous SDK versions now fails
- Compile errors after updating

**Solutions**:

- Review the [Version History](../version-history.md) for breaking changes
- Follow the migration guides for your specific version upgrade
- Temporarily revert to the previous SDK version if needed

## Need More Help?

If you're experiencing issues not covered here, please:

1. Check the [Troubleshooting Guide](../troubleshooting.md) for more detailed information
2. Visit the [FAQ](../faq.md) for answers to common questions
3. File an issue on the [GitHub repository](https://github.com/rally-dfs/rly-network-unity-sdk/issues)
4. Contact Rally Protocol support for personalized assistance
