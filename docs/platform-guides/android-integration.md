# Android Integration Guide for Rally Protocol Unity SDK

This guide provides the essential steps for integrating the Rally Protocol Unity SDK into your Android build.

## Prerequisites

Before proceeding with Android integration, ensure you have:

- Unity 2021.3 or newer
- Rally Protocol Unity SDK installed and configured
- Android SDK tools and platform installed through Unity Hub
- Android NDK installed through Unity Hub

## Step 1: Configure Unity Player Settings

1. Open your Unity project and navigate to **Edit > Project Settings > Player**
2. Select the **Android tab** (Android icon)
3. Configure the following settings:

### Identification

- **Package Name**: Set to your unique app ID (e.g., `com.yourcompany.gamenamerly`)
- **Version**: Set your app version (e.g., `1.0.0`)
- **Version Code**: Set your build number (e.g., `1`)

### Configuration

- **Minimum API Level**: Set to Android 5.0 (API level 21) or higher
- **Target API Level**: Use the latest stable API level
- **Scripting Backend**: Set to **IL2CPP**
- **ARM64 Architecture**: Enable this for better performance on modern devices

### Other Settings

- Under **Optimization**, ensure **Enable R8 Release** is checked for production builds
- In **Publishing Settings**, configure your keystore for signed APKs

## Step 2: Configure Rally Protocol Settings

1. Navigate to **Window > Rally Protocol > Configuration**
2. In the configuration window, ensure your API key is correctly set and select the appropriate network settings for your deployment (testnet or mainnet)

## Step 3: Add Internet Permissions

If needed, create a custom Android manifest to ensure internet permissions are set:

1. Create a folder structure in your project: `Assets/Plugins/Android`
2. Create a file named `AndroidManifest.xml` in that folder with the following content:

```xml
<?xml version="1.0" encoding="utf-8"?>
<manifest
    xmlns:android="http://schemas.android.com/apk/res/android"
    package="com.yourcompany.gamenamerly">
    
    <!-- Permissions -->
    <uses-permission android:name="android.permission.INTERNET" />
    <uses-permission android:name="android.permission.ACCESS_NETWORK_STATE" />
</manifest>
```

Note: This minimal manifest provides only the required permissions. Unity will automatically merge this with its default manifest.

## Step 4: Build and Run

1. Navigate to **File > Build Settings**
2. Select **Android** as the platform
3. Click **Switch Platform** if not already selected
4. Configure the build settings:
   - Choose **Gradle (New)** as the build system
   - Configure the keystore settings for signing your APK/AAB
5. Click **Build** or **Build And Run**

## Troubleshooting

### App Crashes on Launch

**Issue**: App crashes immediately on launch.
**Solution**: Check the device logs via ADB (`adb logcat`). Common causes include:

- Missing required libraries
- Architecture compatibility issues
- Missing Android manifest entries

### Performance Issues

**Issue**: App performance is sluggish, especially on lower-end devices.
**Solution**:

- Use IL2CPP instead of Mono
- Enable arm64 architecture for better performance on capable devices

## Conclusion

By following this guide, you should have successfully integrated the Rally Protocol Unity SDK into your Android app. The SDK is designed to work with minimal configuration, requiring only standard Unity Android build settings and internet permissions.

For further assistance, consult the [Rally Protocol documentation](https://docs.rallyprotocol.com) or reach out to the Rally Protocol support team.

## Additional Resources

- [Google Play Developer Distribution Agreement](https://play.google.com/about/developer-distribution-agreement.html)
- [Unity Android Documentation](https://docs.unity3d.com/Manual/android-GettingStarted.html)
- [Android Developer Documentation](https://developer.android.com/docs)
