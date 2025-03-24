# Installation Guide

This guide provides detailed instructions on how to install the Rally Protocol Unity SDK into your Unity project.

## Prerequisites

Before installing the Rally Protocol Unity SDK, ensure you have:

- Unity 2021.3 or later installed
- Basic knowledge of Unity development
- An API key from Rally Protocol (obtain one at [Rally Protocol Developer Portal](https://docs.rallyprotocol.com/))

## Installation Methods

### Method 1: Unity Package Manager (UPM)

Using the Unity Package Manager is the recommended way to install the Rally Protocol Unity SDK.

1. Open your Unity project
2. Go to **Window > Package Manager**
3. Click the **+** button in the top-left corner
4. Select **Add package from git URL...**
5. Enter the following URL: `https://github.com/rally-dfs/rly-network-unity-sdk.git`
6. Click **Add**

The Unity Package Manager will download and install the SDK automatically.

### Method 2: Import Unity Package

1. Download the latest Rally Protocol Unity SDK package from the [GitHub releases page](https://github.com/rally-dfs/rly-network-unity-sdk/releases)
2. Open your Unity project
3. Go to **Assets > Import Package > Custom Package...**
4. Navigate to the downloaded package file and select it
5. In the Import dialog, ensure all components are selected and click **Import**

### Method 3: Manual Installation

Advanced users may prefer to manually install the SDK:

1. Clone the repository: `git clone https://github.com/rally-dfs/rly-network-unity-sdk.git`
2. Copy the `src/RallyProtocol.Unity/Assets/RallyProtocolUnity` folder into your Unity project's `Assets` folder
3. Ensure all dependencies are properly configured

## Post-Installation Setup

After installing the SDK, you need to configure it for your project:

1. Open the Rally Protocol setup window via **Window > Rally Protocol > Setup**
2. Enter your API key in the provided field
3. Select the appropriate network type for your project (e.g., Polygon, Mumbai)
4. Click **Save Settings**

## Verifying Installation

To verify that the SDK is installed correctly:

1. Create a new C# script in your project
2. Add the following code to test the SDK's basic functionality:

    ```csharp
    using UnityEngine;
    using RallyProtocolUnity.Runtime.Core;

    public class RallyTest : MonoBehaviour
    {
        void Start()
        {
            // Check if the SDK is available
            IRallyNetwork rlyNetwork = RallyUnityManager.Instance.RlyNetwork;
            if (rlyNetwork != null)
            {
                Debug.Log("Rally Protocol SDK installed successfully!");
            }
            else
            {
                Debug.LogError("Rally Protocol SDK installation issue detected.");
            }
        }
    }
    ```

3. Attach this script to a GameObject in your scene
4. Run the scene and check the console output

## Troubleshooting

If you encounter issues during installation:

- Ensure your Unity version is compatible (2021.3 or later)
- Check that all dependencies are properly resolved
- Verify your API key is valid
- Look for errors in the Unity Console
- Restart Unity after installation

If problems persist, refer to the [GitHub issues page](https://github.com/rally-dfs/rly-network-unity-sdk/issues) or contact the Rally Protocol support team.

## Upgrading

To upgrade to a newer version of the SDK:

1. If using UPM, go to Package Manager, select the Rally Protocol package, and click **Update**
2. If using the Unity Package, download the latest version and import it, selecting to replace existing files
3. After upgrading, test your implementation to ensure compatibility with the new version
