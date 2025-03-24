using System.Collections;
using System.Collections.Generic;

using RallyProtocol.Accounts;
using RallyProtocol.Networks;

using RallyProtocolUnity.Accounts;
using RallyProtocolUnity.Networks;

using UnityEngine;

namespace RallyProtocolUnity.Components
{

    /// <summary>
    /// The main manager component for Rally Protocol in Unity.
    /// This singleton class provides centralized access to the Rally Network functionality.
    /// </summary>
    /// <remarks>
    /// The manager is automatically created when accessed through the <see cref="Instance"/> property,
    /// and is set to persist across scene changes with <see cref="Object.DontDestroyOnLoad"/>.
    /// It initializes a <see cref="IRallyNetwork"/> instance using the default configuration.
    /// </remarks>
    [AddComponentMenu(AddComponentMenuNameBase + "/Manager (Rally Protocol)")]
    public class RallyUnityManager : RallyBehaviour
    {

        #region Fields

        private static RallyUnityManager instance;

        protected bool initialized = false;
        protected IRallyNetwork rlyNetwork;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the singleton instance of the Rally Unity Manager.
        /// If no instance exists, one will be created automatically.
        /// </summary>
        /// <value>The singleton instance of <see cref="RallyUnityManager"/>.</value>
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

        /// <summary>
        /// Gets the Rally Network instance managed by this component.
        /// </summary>
        /// <value>The <see cref="IRallyNetwork"/> instance for blockchain interactions.</value>
        public IRallyNetwork RlyNetwork => this.rlyNetwork;

        /// <summary>
        /// Gets a value indicating whether the manager has been initialized.
        /// </summary>
        /// <value><c>true</c> if the manager is initialized; otherwise, <c>false</c>.</value>
        public bool IsInitialized => this.initialized;

        #endregion

        #region Unity Messages

        /// <summary>
        /// Called when the script instance is being loaded.
        /// Automatically initializes the manager.
        /// </summary>
        private void Awake()
        {
            Initialize();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Initializes the Rally Unity Manager if it has not been initialized already.
        /// Creates the Rally Network instance using the default configuration.
        /// </summary>
        /// <remarks>
        /// This method is called automatically when the singleton instance is accessed
        /// or when the component awakes. It can also be called manually if needed.
        /// </remarks>
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