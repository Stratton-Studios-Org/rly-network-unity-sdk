using System;
using System.Collections;
using System.Collections.Generic;

using Nethereum.JsonRpc.Client;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;

using RallyProtocol.Networks;

using UnityEngine;

namespace RallyProtocol.Core
{

    /// <summary>
    /// This implemention uses <see cref="UnityWebRequestRpcTaskClient"/> as RPC client for the <see cref="Web3"/> instance.
    /// </summary>
    public class RallyUnityWeb3Provider : IRallyWeb3Provider
    {

        private static RallyUnityWeb3Provider defaultInstance;

        public static RallyUnityWeb3Provider Default
        {
            get
            {
                if (defaultInstance == null)
                {
                    defaultInstance = new RallyUnityWeb3Provider();
                }

                return defaultInstance;
            }
        }

        protected RallyNetworkConfig config;

        public RallyUnityWeb3Provider()
        {
        }

        public Web3 GetWeb3(RallyNetworkConfig config)
        {
            IClient client = GetRpcClient(config);
            return new Web3(client);
        }

        public Web3 GetWeb3(Account account, RallyNetworkConfig config)
        {
            IClient client = GetRpcClient(config);
            return new Web3(account, client);
        }

        public IClient GetRpcClient(RallyNetworkConfig config)
        {
            UnityWebRequestRpcTaskClient unityClient = new(new Uri(config.Gsn.RpcUrl), null, null);
            string apiKey = "";
            if (!string.IsNullOrEmpty(config.RelayerApiKey))
            {
                apiKey = config.RelayerApiKey;
            }

            unityClient.RequestHeaders["Authorization"] = $"Bearer {apiKey}";
            return unityClient;
        }

    }

}