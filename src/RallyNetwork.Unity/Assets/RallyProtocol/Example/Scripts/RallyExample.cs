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

        protected IRallyNetwork rlyNetwork;

        void Start()
        {
            // You can set the logging filter using this in Unity
            RallyUnityLogger.Default.UnityLogger.filterLogType = LogType.Log;

            Debug.Log("Initializing Rally network...");

            // Create a Rally network instance from the Main settings preset created by Window > Rally Protocol > Setup window
            this.rlyNetwork = RallyUnityNetworkFactory.Create();
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