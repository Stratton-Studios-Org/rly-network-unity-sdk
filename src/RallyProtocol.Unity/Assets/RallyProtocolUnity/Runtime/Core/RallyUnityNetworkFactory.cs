using System.Collections;
using System.Collections.Generic;

using RallyProtocol;
using RallyProtocol.Accounts;
using RallyProtocol.Core;
using RallyProtocol.Logging;
using RallyProtocol.Networks;

using RallyProtocolUnity;
using RallyProtocolUnity.Accounts;
using RallyProtocolUnity.Logging;

using UnityEngine;

namespace RallyProtocolUnity.Networks
{
    /// <summary>
    /// Factory class for creating Rally Network instances in Unity.
    /// Provides various methods to create and configure Rally Network instances with different settings.
    /// </summary>
    public class RallyUnityNetworkFactory
    {

        #region Constants

        /// <summary>
        /// The resource path for the main settings preset.
        /// </summary>
        public const string MainSettingsResourcesPath = "RallyProtocol/Settings/Main";

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets the default Web3 provider for Unity.
        /// </summary>
        /// <returns>The default <see cref="IRallyWeb3Provider"/> instance.</returns>
        public static IRallyWeb3Provider GetWeb3Provider()
        {
            return RallyUnityWeb3Provider.Default;
        }

        /// <summary>
        /// Gets the default HTTP handler for Unity.
        /// </summary>
        /// <returns>The default <see cref="IRallyHttpHandler"/> instance.</returns>
        public static IRallyHttpHandler GetHttpHandler()
        {
            return RallyUnityHttpHandler.Default;
        }

        /// <summary>
        /// Gets the default logger for Unity.
        /// </summary>
        /// <returns>The default <see cref="IRallyLogger"/> instance.</returns>
        public static IRallyLogger GetLogger()
        {
            return RallyUnityLogger.Default;
        }

        /// <summary>
        /// Gets the default account manager for Unity.
        /// </summary>
        /// <returns>The default <see cref="IRallyAccountManager"/> instance.</returns>
        public static IRallyAccountManager GetAccountManager()
        {
            return RallyUnityAccountManager.Default;
        }

        /// <summary>
        /// Loads the main settings preset from Resources.
        /// </summary>
        /// <returns>The main <see cref="RallyProtocolSettingsPreset"/> instance or null if not found.</returns>
        public static RallyProtocolSettingsPreset LoadMainSettingsPreset()
        {
            return Resources.Load<RallyProtocolSettingsPreset>(MainSettingsResourcesPath);
        }

        /// <summary>
        /// Creates a Rally Network instance using the main settings preset.
        /// </summary>
        /// <returns>A configured <see cref="IRallyNetwork"/> instance.</returns>
        /// <exception cref="System.Exception">Thrown when the main settings preset is not found.</exception>
        public static IRallyNetwork Create()
        {
            RallyProtocolSettingsPreset preset = LoadMainSettingsPreset();
            if (preset == null)
            {
                throw new System.Exception($"There are no default settings preset available at {MainSettingsResourcesPath}");
            }

            return Create(preset);
        }

        /// <summary>
        /// Creates a Rally Network instance using the main settings preset with a custom API key.
        /// </summary>
        /// <param name="apiKey">The API key to use for the Rally Network.</param>
        /// <returns>A configured <see cref="IRallyNetwork"/> instance.</returns>
        /// <exception cref="System.Exception">Thrown when the main settings preset is not found.</exception>
        public static IRallyNetwork Create(string apiKey)
        {
            RallyProtocolSettingsPreset preset = LoadMainSettingsPreset();
            if (preset == null)
            {
                throw new System.Exception($"There are no default settings preset available at {MainSettingsResourcesPath}");
            }

            return Create(preset, apiKey);
        }

        /// <summary>
        /// Creates a Rally Network instance using the specified settings preset.
        /// </summary>
        /// <param name="preset">The settings preset to use for configuration.</param>
        /// <returns>A configured <see cref="IRallyNetwork"/> instance.</returns>
        public static IRallyNetwork Create(RallyProtocolSettingsPreset preset)
        {
            if (preset.NetworkType == RallyNetworkType.Custom)
            {
                RallyNetworkConfig clonedCustomConfig = preset.CustomNetworkConfig.Clone();
                return Create(clonedCustomConfig, preset.ApiKey);
            }
            else
            {
                return Create(preset.NetworkType, preset.ApiKey);
            }
        }

        /// <summary>
        /// Creates a Rally Network instance using the specified settings preset and a custom API key.
        /// </summary>
        /// <param name="preset">The settings preset to use for configuration.</param>
        /// <param name="apiKey">The API key to use for the Rally Network.</param>
        /// <returns>A configured <see cref="IRallyNetwork"/> instance.</returns>
        public static IRallyNetwork Create(RallyProtocolSettingsPreset preset, string apiKey)
        {
            if (preset.NetworkType == RallyNetworkType.Custom)
            {
                RallyNetworkConfig clonedCustomConfig = preset.CustomNetworkConfig.Clone();
                return Create(clonedCustomConfig, apiKey);
            }
            else
            {
                return Create(preset.NetworkType, apiKey);
            }
        }

        /// <summary>
        /// Creates a Rally Network instance using the specified network type and API key.
        /// </summary>
        /// <param name="type">The network type to use for configuration.</param>
        /// <param name="apiKey">The API key to use for the Rally Network. If null, the API key from the main settings preset will be used if available.</param>
        /// <returns>A configured <see cref="IRallyNetwork"/> instance.</returns>
        public static IRallyNetwork Create(RallyNetworkType type, string apiKey = null)
        {
            if (apiKey == null)
            {
                RallyProtocolSettingsPreset preset = LoadMainSettingsPreset();
                if (preset != null)
                {
                    apiKey = preset.ApiKey;
                }
            }

            return RallyNetworkFactory.Create(GetWeb3Provider(), GetHttpHandler(), GetLogger(), GetAccountManager(), type, apiKey);
        }

        /// <summary>
        /// Creates a Rally Network instance using the specified network configuration and API key.
        /// </summary>
        /// <param name="config">The network configuration to use.</param>
        /// <param name="apiKey">The API key to use for the Rally Network. If null, the API key from the main settings preset will be used if available.</param>
        /// <returns>A configured <see cref="IRallyNetwork"/> instance.</returns>
        public static IRallyNetwork Create(RallyNetworkConfig config, string apiKey = null)
        {
            if (apiKey == null)
            {
                RallyProtocolSettingsPreset preset = LoadMainSettingsPreset();
                if (preset != null)
                {
                    apiKey = preset.ApiKey;
                }
            }

            return RallyNetworkFactory.Create(GetWeb3Provider(), GetHttpHandler(), GetLogger(), GetAccountManager(), config, apiKey);
        }

        #endregion

    }

}