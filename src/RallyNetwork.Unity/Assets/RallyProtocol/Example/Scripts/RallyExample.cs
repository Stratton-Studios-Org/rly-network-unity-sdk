using System;
using System.Collections;
using System.Collections.Generic;

using RallyProtocol.Accounts;
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
                await this.rlyNetwork.ClaimRly();
            }
            catch (Exception ex)
            {
                Debug.LogError("Claiming RLY failed");
                Debug.LogException(ex);
            }
        }

    }

}