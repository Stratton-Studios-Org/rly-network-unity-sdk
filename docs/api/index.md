# API Reference

This section contains detailed documentation for the Rally Protocol Unity SDK API.

## Platform Support

The SDK supports the following platforms:

- **iOS**: Fully supported for deployment
- **Android**: Fully supported for deployment
- **Unity Editor**: Supported for development on Windows, macOS, and Linux

> **Important Note**: Standalone builds for Windows, macOS, and Linux are not supported for deployment.

See [Versioning](./versioning.md) for detailed information on version and platform compatibility.

## Core Interfaces

- [IRallyNetwork](./IRallyNetwork.md) - The main interface for interacting with the Rally Protocol
- [IRallyAccountManager](./IRallyAccountManager.md) - Interface for managing blockchain accounts
- [RallyNetworkFactory](./RallyNetworkFactory.md) - Factory class for creating network instances

## Common Operations

- [Balance Checking](./balance-checking.md) - How to check token balances
- [Transaction Operations](./transaction-operations.md) - How to perform blockchain transactions
- [GSN Operations](./gsn-operations.md) - How to use the Gas Station Network for gasless transactions

## Configuration

- [Network Configuration](./network-configuration.md) - How to configure network settings
- [API Keys](./api-keys.md) - How to set up and use API keys
- [Storage Options](./storage-options.md) - Options for data storage and persistence
- [RallyNetworkConfig](./RallyNetworkConfig.md) - Configuration options for the Rally Network
- [RallyProtocolSettingsPreset](./RallyProtocolSettingsPreset.md) - Preset configuration settings

## Events and Callbacks

- [Event System](./event-system.md) - How to use the event system
- [Transaction Callbacks](./transaction-callbacks.md) - Callbacks for transaction operations

## Utility Classes

- [Web3 Utilities](./web3-utilities.md) - Utility functions for Web3 operations
- [Conversion Utilities](./conversion-utilities.md) - Utilities for unit conversions

## Advanced Topics

- [Custom Network Integration](./custom-network-integration.md) - How to integrate with custom networks
- [Security Best Practices](./security-best-practices.md) - Security best practices for using the SDK

## Factories

- [RallyNetworkFactory](./RallyNetworkFactory.md) - Factory for creating Rally Network instances

## Models

- [GsnTransactionDetails](./GsnTransactionDetails.md) - Details for Gas Station Network transactions
- [AccountOptions](./AccountOptions.md) - Options for account creation and management
- [StorageOptions](./StorageOptions.md) - Options for key storage

## Enums

- [RallyNetworkType](./RallyNetworkType.md) - Enum of supported network types

## Extensions

- [RallyUnityExtensions](./RallyUnityExtensions.md) - Unity-specific extension methods

## Exceptions

- [RallyException](./RallyException.md) - Base exception class for Rally Protocol
- [RallyAccountException](./RallyAccountException.md) - Exception related to account operations
- [RallyNetworkException](./RallyNetworkException.md) - Exception related to network operations

## Additional Documentation

- [Error Handling](./error-handling.md) - How to handle errors in the SDK
- [Versioning](./versioning.md) - Information about SDK versioning
- [Migration Guide](./migration-guide.md) - Guide for migrating between SDK versions
