# Versioning

This document explains how versioning is managed within the Rally Protocol Unity SDK.

## SDK Version

The SDK follows Semantic Versioning (SemVer) principles, with version numbers in the format of `MAJOR.MINOR.PATCH`:

- **MAJOR** version increments for incompatible API changes
- **MINOR** version increments for backward-compatible new functionality
- **PATCH** version increments for backward-compatible bug fixes

## Version Compatibility

### Unity Version Compatibility

The SDK is compatible with Unity 2020.3 LTS and newer versions. Specific compatibility notes for different Unity versions:

- **Unity 2020.3 LTS**: Fully supported
- **Unity 2021.3 LTS**: Fully supported
- **Unity 2022.3 LTS**: Fully supported
- **Unity 2023.1+**: Supported, but new Unity features may not be utilized

### Platform Compatibility

The SDK supports the following platforms:

- **iOS**: Fully supported for deployment
- **Android**: Fully supported for deployment
- **Unity Editor**: Supported for development on Windows, macOS, and Linux

Note: Standalone builds for Windows, macOS, and Linux are not supported for deployment.

### .NET Version Compatibility

The SDK requires .NET 4.x or .NET Standard 2.0+ compatibility level in your Unity project settings.

## Upgrading the SDK

When upgrading to a new version of the SDK, follow these steps:

1. Back up your project
2. Remove the existing SDK folder (e.g., `Assets/RallyProtocol`)
3. Import the new SDK package
4. Review the release notes for any breaking changes
5. Test your integration thoroughly before deploying

### Handling Breaking Changes

Major version updates may include breaking changes. Common migration patterns include:

```csharp
// EXAMPLE: If a method was renamed in a new version

// Old code (before v2.0.0)
await network.SendTransactionAsync(recipient, amount);

// New code (after v2.0.0)
await network.TransferAsync(recipient, amount);
```

## Plugin Dependencies

The SDK has dependencies on several Unity packages and third-party libraries:

- Newtonsoft.Json (included with Unity 2020.3+)
- Nethereum (included in SDK)
- UniTask (included in SDK)

These dependencies are bundled with the SDK, so you don't need to manage them separately.

## Network Version Compatibility

The SDK is designed to work with specific versions of the Rally Protocol backend services:

- Rally Protocol API: v1.x
- Rally Protocol Contracts: v1.x

The SDK will automatically handle compatibility with these backend services.

## Version History

Major releases and their key features:

| Version | Release Date | Key Features |
|---------|--------------|--------------|
| 1.0.0   | 2024-04-02   | Initial release with core functionality |
| 1.1.0   | 2024-04-06   | Added support for GSN transactions |
| 1.2.0   | 2024-04-19   | Added balance polling and event system |
| 1.3.0   | 2024-05-31   | Improved error handling and retry mechanisms |

## Deprecation Policy

Features marked as deprecated will remain functional for at least one major version cycle before being removed. Deprecated features will generate warning messages in the console.

Example of handling deprecated methods:

```csharp
public class BackwardCompatibilityExample : MonoBehaviour
{
    private IRallyNetwork _network;
    
    void Start()
    {
        _network = RallyUnityManager.Instance.RlyNetwork;
    }
    
    // Example of handling deprecated methods
    public async Task<string> SendTokens(string recipient, decimal amount)
    {
        string txHash;
        
        // Check SDK version to determine which method to call
        string sdkVersion = RallyUnityManager.SdkVersion;
        
        if (string.Compare(sdkVersion, "2.0.0") < 0)
        {
            // Use old method for SDK versions before 2.0.0
            #pragma warning disable CS0618 // Disable obsolete warning
            txHash = await _network.SendTransactionAsync(recipient, amount.ToString());
            #pragma warning restore CS0618
        }
        else
        {
            // Use new method for SDK version 2.0.0 and above
            txHash = await _network.TransferAsync(recipient, amount);
        }
        
        return txHash;
    }
}
```

## Related Documentation

- [Migration Guide](./migration-guide.md) - For detailed migration instructions between major versions
- [Release Notes](https://github.com/rally-protocol/unity-sdk/releases) - For detailed changes in each release
