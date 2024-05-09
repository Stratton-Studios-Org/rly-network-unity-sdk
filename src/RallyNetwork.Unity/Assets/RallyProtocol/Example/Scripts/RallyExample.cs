using System;
using System.Collections;
using System.Collections.Generic;

using RallyProtocol.Accounts;
using RallyProtocol.Logging;
using RallyProtocol.Networks;

using UnityEngine;

namespace RallyProtocol.Samples
{

    public class RallyExample : MonoBehaviour
    {

        [SerializeField]
        protected RallyProtocolSettingsPreset preset;

        protected IRallyNetwork rlyNetwork;

        void Start()
        {
            // You can set the logging filter using this in Unity
            RallyUnityLogger.Default.UnityLogger.filterLogType = LogType.Log;

            Debug.Log("Initializing Rally network...");
            this.rlyNetwork = RallyUnityNetworkFactory.Create(this.preset);
            Debug.Log("Initialized Rally network");
        }

        public async void CreateAccount()
        {
            try
            {
                Debug.Log("Creating account...");

                // Create a new account & overwrite everytime for testing purposes
                await RallyUnityAccountManager.Default.CreateAccountAsync(new() { Overwrite = true });
                Debug.Log("Account created successfully");
            }
            catch (Exception ex)
            {
                Debug.LogError("Account creation failed");
                Debug.LogException(ex);
            }
        }

        public async void ClaimRly()
        {
            try
            {
                Debug.Log("Claiming RLY...");
                await this.rlyNetwork.ClaimRly();
                Debug.Log("Claimed RLY successfully");
            }
            catch (Exception ex)
            {
                Debug.LogError("Claiming RLY failed");
                Debug.LogException(ex);
            }
        }

    }

}