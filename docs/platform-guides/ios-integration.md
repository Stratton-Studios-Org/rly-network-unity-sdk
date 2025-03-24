# iOS Integration Guide for Rally Protocol Unity SDK

This guide provides the essential steps for integrating the Rally Protocol Unity SDK into your iOS build.

## Prerequisites

Before proceeding with iOS integration, ensure you have:

- Unity 2021.3 or newer
- Rally Protocol Unity SDK installed and configured
- Xcode 14.0 or newer (for building the iOS app)
- An Apple Developer account (for deployment)

## Step 1: Configure Unity Player Settings

1. Open your Unity project and navigate to **Edit > Project Settings > Player**
2. Select the **iOS tab** (iPhone icon)
3. Configure the following settings:

### Identification

- **Bundle Identifier**: Set to your unique app ID (e.g., `com.yourcompany.gamenamerly`)
- **Version**: Set your app version (e.g., `1.0.0`)
- **Build Number**: Set your build number (e.g., `1`)

### Configuration

- **Architecture**: Set to `ARM64` (32-bit is no longer supported by Apple)
- **Target minimum iOS Version**: Set to iOS 13.0 or higher (Rally Protocol requires iOS 13+)
- **Target SDK**: Set to `Device SDK`

### Other Settings

- Under **Optimization**, set **Scripting Backend** to `IL2CPP`
- Enable **Allow 'unsafe' Code** in the **Other Settings** section

## Step 2: Configure Rally Protocol Settings

1. Navigate to **Window > Rally Protocol > Configuration**
2. In the configuration window, ensure your API key is correctly set and select the appropriate network settings for your deployment (testnet or mainnet)

## Step 3: Build and Export to Xcode

1. Navigate to **File > Build Settings**
2. Select **iOS** as the platform
3. Click **Switch Platform** if not already selected
4. Click **Build** or **Build and Run**
5. Choose a location to save the Xcode project

## Step 4: Final Xcode Configurations

After opening the exported project in Xcode, make the following configurations:

1. **Signing & Capabilities**:
   - Select your development team
   - Set your provisioning profile

2. **Build Settings** (for newer Xcode versions):
   - Ensure **Enable Bitcode** is set to **No** (Bitcode has been deprecated)
   - Set **Other Linker Flags** to include `-Wl,-ld_classic` if needed

## Troubleshooting iOS-Specific Issues

### App Not Launching

**Issue**: App crashes immediately on launch.
**Solution**: Check the device logs in Xcode for specific error messages. Common causes include:

- Missing required libraries
- Incompatible architecture settings
- Invalid entitlements

### Transaction Failures

**Issue**: Transactions fail on iOS but work on other platforms.
**Solution**:

- Check network connectivity
- Verify that background execution is properly configured for long-running operations

## Conclusion

By following this guide, you should have successfully integrated the Rally Protocol Unity SDK into your iOS app. The SDK is designed to work with minimal configuration, requiring only the standard Unity iOS build settings.

For further assistance, consult the [Rally Protocol documentation](https://docs.rallyprotocol.com) or reach out to the Rally Protocol support team.

## Additional Resources

- [Apple App Store Review Guidelines](https://developer.apple.com/app-store/review/guidelines/)
- [Unity iOS Development Documentation](https://docs.unity3d.com/Manual/iphone-GettingStarted.html)
- [Rally Protocol Developer Portal](https://docs.rallyprotocol.com)
