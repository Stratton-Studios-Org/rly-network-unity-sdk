using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace RallyProtocol.Samples
{

    public class RallyExample : MonoBehaviour
    {

        [SerializeField]
        protected RallyNetworkType networkType;
        [TextArea]
        [SerializeField]
        protected string apiKey;

        protected IRallyNetwork rlyNetwork;

        void Start()
        {
            Debug.Log("Initializing Rally Network...");
            this.rlyNetwork = RallyNetworkFactory.Create(this.networkType, this.apiKey);
            Debug.Log($"Initialized Rally {this.networkType} network using API key: {this.apiKey}");
        }

        public async void CreateAccount()
        {
            try
            {
                Debug.Log("Creating account...");
                await WalletManager.Default.CreateAccountAsync(new() { Overwrite = true });
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