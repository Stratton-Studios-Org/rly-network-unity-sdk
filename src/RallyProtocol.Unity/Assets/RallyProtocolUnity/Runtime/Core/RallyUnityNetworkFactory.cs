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

    public class RallyUnityNetworkFactory
    {

        #region Constants

        public const string MainSettingsResourcesPath = "RallyProtocol/Settings/Main";

        #endregion

        #region Public Methods

        public static IRallyWeb3Provider GetWeb3Provider()
        {
            return RallyUnityWeb3Provider.Default;
        }

        public static IRallyHttpHandler GetHttpHandler()
        {
            return RallyUnityHttpHandler.Default;
        }

        public static IRallyLogger GetLogger()
        {
            return RallyUnityLogger.Default;
        }

        public static IRallyAccountManager GetAccountManager()
        {
            return RallyUnityAccountManager.Default;
        }

        public static RallyProtocolSettingsPreset LoadMainSettingsPreset()
        {
            return Resources.Load<RallyProtocolSettingsPreset>(MainSettingsResourcesPath);
        }

        public static IRallyNetwork Create()
        {
            RallyProtocolSettingsPreset preset = LoadMainSettingsPreset();
            if (preset == null)
            {
                throw new System.Exception($"There are no default settings preset available at {MainSettingsResourcesPath}");
            }

            return Create(preset);
        }

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