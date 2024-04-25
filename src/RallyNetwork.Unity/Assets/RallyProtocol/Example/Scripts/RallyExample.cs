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

        async void Start()
        {
            try
            {
                IRallyNetwork rlyNetwork = RallyNetworkFactory.Create(this.networkType, this.apiKey);

                // Create account
                Debug.Log("CreateAccount");
                await WalletManager.Default.CreateAccountAsync(new() { Overwrite = true });

                // Claim some RLY for the newly created account
                Debug.Log("ClaimRly");
                await rlyNetwork.ClaimRly();
            }
            catch (Exception ex)
            {
                Debug.LogError("Failed to claim");
                Debug.LogException(ex);
            }
        }

    }

}