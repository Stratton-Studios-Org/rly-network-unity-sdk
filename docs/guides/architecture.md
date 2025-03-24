# Rally Protocol Unity SDK Architecture

This document provides an overview of the Rally Protocol Unity SDK architecture, helping you understand how the different components work together.

## Architecture Overview

The Rally Protocol Unity SDK is designed with a layered architecture that abstracts blockchain complexity and provides a clean interface for Unity developers.

```
┌───────────────────────────────────────────────────────────────┐
│                      Your Unity Game                          │
└───────────────┬───────────────────────────────┬───────────────┘
                │                               │
                ▼                               ▼
┌───────────────────────────┐     ┌───────────────────────────┐
│    RallyUnityManager      │     │  Unity UI Components      │
│  (Singleton Access Point) │     │  (Optional UI Elements)   │
└───────────────┬───────────┘     └───────────────────────────┘
                │                               
                ▼                               
┌───────────────────────────┐     ┌───────────────────────────┐
│      IRallyNetwork        │◄────┤ RallyUnityNetworkFactory  │
│   (Core API Interface)    │     │ (Creates Network Instance) │
└───────────┬───────────────┘     └───────────────────────────┘
            │                                   ▲
            │                                   │
            ▼                                   │
┌─────────────────────────────────────────────────────────────┐
│                                                             │
│  ┌─────────────────┐    ┌─────────────────┐                 │
│  │IRallyAccountMgr │    │ Web3 Interface  │                 │
│  │(Account Ops)    │    │(Blockchain Ops) │                 │
│  └─────────────────┘    └─────────────────┘                 │
│                                                             │
│  ┌─────────────────┐    ┌─────────────────┐                 │
│  │ Token Ops       │    │ GSN Service     │                 │
│  │(RLY Management) │    │(Gasless Txns)   │                 │
│  └─────────────────┘    └─────────────────┘                 │
│                                                             │
└─────────────────────────────────────────────────────────────┘
                │
                ▼
┌───────────────────────────────────────────────────────────────┐
│                   Platform-Specific Layer                      │
│  ┌─────────────────┐    ┌─────────────────┐    ┌────────────┐ │
│  │ iOS Plugin      │    │ Android Plugin  │    │Unity Editor│ │
│  └─────────────────┘    └─────────────────┘    └────────────┘ │
└───────────────────────────────────────────────────────────────┘
                │
                ▼
┌───────────────────────────────────────────────────────────────┐
│                      Blockchain Layer                          │
└───────────────────────────────────────────────────────────────┘
```

## Core Components

### RallyUnityManager

The `RallyUnityManager` is a singleton that serves as the primary access point to the SDK's functionality. It initializes and holds a reference to the `IRallyNetwork` instance.

```csharp
// Access the Rally Network instance
IRallyNetwork rlyNetwork = RallyUnityManager.Instance.RlyNetwork;
```

### RallyUnityNetworkFactory

The `RallyUnityNetworkFactory` creates instances of `IRallyNetwork` based on different configuration options:

```csharp
// Various ways to create a network instance
IRallyNetwork network1 = RallyUnityNetworkFactory.Create(); // Default
IRallyNetwork network2 = RallyUnityNetworkFactory.Create(networkType, apiKey); // Specific
IRallyNetwork network3 = RallyUnityNetworkFactory.Create(customConfig, apiKey); // Custom
```

### IRallyNetwork

The `IRallyNetwork` interface is the core API that provides access to blockchain functionality:

- Account management
- Token operations (balance checks, transfers)
- GSN transaction relaying
- Web3 access

### IRallyAccountManager

The `IRallyAccountManager` handles all account-related operations:

- Creating and loading accounts
- Secure key storage
- Cloud backup of keys

## Data Flow

### Initialization Flow

```
1. Application Start
   │
   ▼
2. Access RallyUnityManager.Instance
   │
   ▼
3. RallyUnityManager.Initialize()
   │
   ▼
4. RallyUnityNetworkFactory.Create()
   │
   ▼
5. Load Settings Preset
   │
   ▼
6. Create IRallyNetwork instance
   │
   ▼
7. Ready for API calls
```

### Transaction Flow

```
1. User Action in Game
   │
   ▼
2. Call to RallyUnityManager.Instance.RlyNetwork
   │
   ▼
3. IRallyNetwork method called (e.g., TransferAsync)
   │
   ▼
4. Account validation and preparation
   │
   ▼
5. Transaction creation
   │
   ▼
6. GSN relaying (gasless transaction)
   │
   ▼
7. Blockchain processes transaction
   │
   ▼
8. Transaction hash returned to application
```

## Configuration System

The SDK uses a configuration system based on preset files:

```
┌────────────────────┐     ┌────────────────────┐
│ Unity Editor       │     │ Settings Preset    │
│ (Setup Window)     │────▶│ (ScriptableObject) │
└────────────────────┘     └─────────┬──────────┘
                                     │
                                     ▼
┌────────────────────┐     ┌────────────────────┐
│ RallyNetworkFactory│◀────┤ Resources Folder   │
│ (Loads Settings)   │     │ (Stores Preset)    │
└────────┬───────────┘     └────────────────────┘
         │
         ▼
┌────────────────────┐
│ RallyNetworkConfig │
│ (Runtime Config)   │
└────────────────────┘
```

## Platform-Specific Components

The SDK supports only iOS and Android for deployment, with Unity Editor support for development on Windows, macOS, and Linux.

The SDK includes platform-specific plugins for iOS and Android that handle:

- Native cryptography operations
- Secure storage integration
- Platform-specific Web3 provider implementations

Note: Standalone builds for Windows, macOS, and Linux are not supported for deployment.

## Integration Points

When integrating the SDK, you'll primarily interact with:

1. `RallyUnityManager.Instance.RlyNetwork` - For all blockchain operations
2. `RallyUnityNetworkFactory` - If you need custom network configurations
3. SDK setup window - For initial configuration

## Extension Points

The SDK can be extended at several points:

- Custom network configurations via `RallyNetworkConfig`
- Direct Web3 access via `rlyNetwork.Web3`
- Custom logging via the logging system
