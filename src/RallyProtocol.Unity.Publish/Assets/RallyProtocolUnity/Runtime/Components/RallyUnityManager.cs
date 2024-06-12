using System.Collections;
using System.Collections.Generic;

using RallyProtocol.Accounts;
using RallyProtocol.Networks;

using RallyProtocolUnity.Accounts;
using RallyProtocolUnity.Networks;

using UnityEngine;

namespace RallyProtocolUnity.Components
{

    [AddComponentMenu(AddComponentMenuNameBase + "/Manager (Rally Protocol)")]
    public class RallyUnityManager : RallyBehaviour
    {

        #region Fields

        private static RallyUnityManager instance;

        protected bool initialized = false;
        protected IRallyNetwork rlyNetwork;

        #endregion

        #region Properties

        public static RallyUnityManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new GameObject(nameof(RallyUnityManager)).AddComponent<RallyUnityManager>();
                    instance.Initialize();
                }

                return instance;
            }
        }

        public IRallyNetwork RlyNetwork => this.rlyNetwork;

        public bool IsInitialized => this.initialized;

        #endregion

        #region Unity Messages

        private void Awake()
        {
            Initialize();
        }

        #endregion

        #region Public Methods

        public void Initialize()
        {
            if (IsInitialized)
            {
                return;
            }

            this.initialized = true;
            DontDestroyOnLoad(gameObject);
            this.rlyNetwork = RallyUnityNetworkFactory.Create();
        }

        #endregion

    }

}