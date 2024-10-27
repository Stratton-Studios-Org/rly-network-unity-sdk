using System.Collections;
using System.Collections.Generic;

using RallyProtocol.Networks;

using UnityEngine;

namespace RallyProtocolUnity
{

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

        public string ApiKey => this.apiKey;
        public RallyNetworkType NetworkType => this.networkType;
        public RallyNetworkConfig CustomNetworkConfig => this.customNetworkConfig;

        #endregion

    }

}