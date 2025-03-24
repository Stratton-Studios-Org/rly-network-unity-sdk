# RallyProtocolSettingsPreset

The `RallyProtocolSettingsPreset` class is a ScriptableObject that provides configuration settings for the Rally Protocol in Unity applications. It stores essential configuration data like API key and network type that can be created and modified in the Unity Editor.

## Class Overview

```csharp
[CreateAssetMenu(menuName = "Rally Protocol/Settings Preset")]
public class RallyProtocolSettingsPreset : ScriptableObject
{
    // Properties
    public string ApiKey { get; }
    public RallyNetworkType NetworkType { get; }
    public RallyNetworkConfig CustomNetworkConfig { get; }
}
```

## Properties

### ApiKey

```csharp
public string ApiKey { get; }
```

Gets the API key for Rally Protocol services.

### NetworkType

```csharp
public RallyNetworkType NetworkType { get; }
```

Gets the network type to use for the Rally Network.

### CustomNetworkConfig

```csharp
public RallyNetworkConfig CustomNetworkConfig { get; }
```

Gets the custom network configuration when NetworkType is set to Custom.

## Usage

### Creating a Settings Preset Asset

1. In the Unity Editor, right-click in the Project window
2. Select "Create > Rally Protocol > Settings Preset"
3. Name your settings preset
4. Configure the settings in the Inspector

### Example: Using the Settings Preset

```csharp
using RallyProtocolUnity;
using UnityEngine;

public class RallyNetworkInitializer : MonoBehaviour
{
    [SerializeField] private RallyProtocolSettingsPreset settingsPreset;
    
    private void Start()
    {
        // Access settings from the preset
        string apiKey = settingsPreset.ApiKey;
        RallyNetworkType networkType = settingsPreset.NetworkType;
        
        // Use settings to initialize network
        // ...
    }
}
```

## Best Practices

1. **Create different presets for different environments**:
   - Create a development preset using testnet settings
   - Create a production preset using mainnet settings

2. **API Key Security**:
   - Do not commit settings assets with real API keys to version control
   - Consider using a runtime configuration system for sensitive data

3. **Custom Network Configuration**:
   - Only set NetworkType to Custom when you need to use a network not provided in the standard RallyNetworkType enum
   - When using Custom, ensure you properly configure CustomNetworkConfig with the correct values

## Related Classes

- [RallyNetworkConfig](./RallyNetworkConfig.md): The configuration class that the presets populate
- [RallyNetworkType](./RallyNetworkType.md): Enum of supported network types
- [RallyNetworkFactory](./RallyNetworkFactory.md): Factory for creating network instances using these presets
