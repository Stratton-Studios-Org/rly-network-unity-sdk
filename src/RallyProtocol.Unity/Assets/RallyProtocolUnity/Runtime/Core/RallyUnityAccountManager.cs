using System.Collections;
using System.Collections.Generic;

using RallyProtocol.Accounts;

using RallyProtocolUnity.Logging;
using RallyProtocolUnity.Storage;

using UnityEngine;

namespace RallyProtocolUnity.Accounts
{
    /// <summary>
    /// Unity-specific implementation of the Rally Protocol account manager.
    /// Provides a singleton instance for centralized access to account management functionality.
    /// </summary>
    /// <remarks>
    /// This class extends the base <see cref="RallyAccountManager"/> with Unity-specific key management
    /// and logging capabilities. It uses the <see cref="RallyUnityKeyManager"/> for secure storage
    /// of account keys in the Unity environment.
    /// </remarks>
    public class RallyUnityAccountManager : RallyAccountManager
    {
        /// <summary>
        /// The singleton instance of the RallyUnityAccountManager.
        /// </summary>
        protected static RallyUnityAccountManager defaultInstance;

        /// <summary>
        /// Gets the default singleton instance of the RallyUnityAccountManager.
        /// If no instance exists, one will be created automatically.
        /// </summary>
        /// <value>The singleton instance of <see cref="RallyUnityAccountManager"/>.</value>
        public static RallyUnityAccountManager Default
        {
            get
            {
                if (defaultInstance == null)
                {
                    defaultInstance = new RallyUnityAccountManager();
                }

                return defaultInstance;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RallyUnityAccountManager"/> class.
        /// Uses the default Unity key manager and logger instances.
        /// </summary>
        public RallyUnityAccountManager() : base(RallyUnityKeyManager.Default, RallyUnityLogger.Default)
        {
        }
    }
}