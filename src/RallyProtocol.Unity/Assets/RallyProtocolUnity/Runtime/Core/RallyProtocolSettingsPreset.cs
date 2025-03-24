using System.Collections;
using System.Collections.Generic;

using RallyProtocol.Networks;

using UnityEngine;

namespace RallyProtocolUnity
{
    /// <summary>
    /// Scriptable object for storing Rally Protocol configuration settings.
    /// </summary>
    [CreateAssetMenu(menuName = "Rally Protocol/Settings Preset")]
    public class RallyProtocolSettingsPreset : ScriptableObject
    {

        #region Fields

        [SerializeField]
        protected string apiKey;
        [SerializeField]
        protected RallyNetworkType networkType = RallyNetworkType.BaseSepolia;
        [SerializeField]
        protected RallyNetworkConfig customNetworkConfig;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the API key for Rally Protocol services.
        /// </summary>
        public string ApiKey => this.apiKey;
        
        /// <summary>
        /// Gets the network type to use for the Rally Network.
        /// </summary>
        public RallyNetworkType NetworkType => this.networkType;
        
        /// <summary>
        /// Gets the custom network configuration when NetworkType is set to Custom.
        /// </summary>
        public RallyNetworkConfig CustomNetworkConfig => this.customNetworkConfig;

        #endregion

    }

}