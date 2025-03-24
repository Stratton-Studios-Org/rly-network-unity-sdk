# Configuration Guide

This guide explains how to configure the Rally Protocol Unity SDK for your project's needs.

## Basic Configuration

The Rally Protocol SDK is designed to be easily configurable through various methods. The basic configuration involves setting up your API key and selecting a network.

### Using the Setup Window

The easiest way to configure the SDK is through the built-in setup window:

1. Open the Rally Protocol setup window via **Window > Rally Protocol > Setup**
2. Enter your API key in the provided field
3. Select the network type (e.g., Polygon, Mumbai)
4. Configure additional options as needed
5. Click **Save Settings**

This creates a default settings preset that will be used automatically when initializing the SDK.

## Configuration Methods

### Method 1: Using the Default Settings

The simplest way to access a configured Rally Network instance is:

```csharp
// Get the network instance initialized with default settings
IRallyNetwork rlyNetwork = RallyUnityManager.Instance.RlyNetwork;
```

### Method 2: Using a Custom Settings Preset

You can create and use custom settings presets:

```csharp
// Load a custom settings preset
RallyProtocolSettingsPreset preset = Resources.Load<RallyProtocolSettingsPreset>("MyCustomPreset");
IRallyNetwork rlyNetwork = RallyUnityNetworkFactory.Create(preset);
```

To create a custom settings preset:

1. In the Project window, right-click and select **Create > Rally Protocol > Settings Preset**
2. Name your preset (e.g., "MyCustomPreset")
3. Configure the settings in the Inspector
4. Place the preset in a Resources folder to load it by name

### Method 3: Using Network Type and API Key

For simple configuration needs:

```csharp
string myApiKey = "your-api-key-here";
RallyNetworkType networkType = RallyNetworkType.Polygon;
IRallyNetwork rlyNetwork = RallyUnityNetworkFactory.Create(networkType, myApiKey);
```

### Method 4: Using a Custom Network Configuration

For advanced configuration needs:

```csharp
string myApiKey = "your-api-key-here";

// Create a configuration based on a predefined template
RallyNetworkConfig config = RallyNetworkConfig.Polygon;

// Customize the configuration
config.Gsn.RelayUrl = "https://custom-relay.example.com";
config.Gsn.ChainId = 137;
config.Gsn.PaymasterAddress = "0xYourPaymasterAddress";

// Create the network with custom configuration
IRallyNetwork rlyNetwork = RallyUnityNetworkFactory.Create(config, myApiKey);
```

## Configuration Parameters

### Network Configuration

The `RallyNetworkConfig` class contains these key properties:

- `ChainId`: The blockchain network chain ID
- `Name`: Human-readable name of the network
- `RlyTokenAddress`: The address of the RLY token contract
- `TokenFaucetAddress`: The address of the token faucet contract
- `Gsn`: Gas Station Network configuration

### GSN Configuration

The Gas Station Network (GSN) configuration includes:

- `RelayUrl`: The URL of the GSN relay server
- `PaymasterAddress`: The address of the paymaster contract
- `ForwarderAddress`: The address of the forwarder contract
- `ChainId`: The chain ID for the GSN network

### Storage Options

Configure how account keys are stored:

```csharp
// Configure storage options when creating an account
await accountManager.CreateAccountAsync(new()
{
    Overwrite = false,
    StorageOptions = new()
    {
        SaveToCloud = true,
        RejectOnCloudSaveFailure = false
    }
});
```

## Logging Configuration

Configure the SDK's logging level:

```csharp
// Set the logging filter
RallyUnityLogger.Default.UnityLogger.filterLogType = LogType.Log;

// For more detailed logs during development
RallyUnityLogger.Default.UnityLogger.filterLogType = LogType.Verbose;

// For production, minimize logs
RallyUnityLogger.Default.UnityLogger.filterLogType = LogType.Warning;
```

## Platform-Specific Configuration

The Rally Protocol SDK supports only iOS and Android for deployment, while the Unity Editor is supported on Windows, macOS, and Linux for development purposes.

### iOS Configuration

For iOS deployment, ensure you've properly configured:

1. Add necessary entries to your `Info.plist` for keychain access
2. Configure networking permissions for blockchain access

### Android Configuration

For Android deployment:

1. Ensure internet permissions are set in your `AndroidManifest.xml`
2. Configure any custom security settings for key storage

### Editor Development

When developing in the Unity Editor (Windows, macOS, or Linux):

1. The SDK provides full functionality for testing and development
2. Be aware that some platform-specific features may behave differently in the editor
3. Always test on actual iOS or Android devices before deployment
