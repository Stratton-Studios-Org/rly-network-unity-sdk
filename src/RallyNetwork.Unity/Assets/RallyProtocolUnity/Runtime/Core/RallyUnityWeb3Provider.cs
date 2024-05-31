using System;
using System.Collections;
using System.Collections.Generic;

using Nethereum.JsonRpc.Client;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;

using RallyProtocol.Core;
using RallyProtocol.Networks;

using RallyProtocolUnity.Logging;

using UnityEngine;

namespace RallyProtocolUnity
{

    /// <summary>
    /// This implemention uses <see cref="UnityWebRequestRpcTaskClient"/> as RPC client for the <see cref="Web3"/> instance.
    /// </summary>
    public class RallyUnityWeb3Provider : IRallyWeb3Provider
    {

        #region Fields

        private static RallyUnityWeb3Provider defaultInstance;

        protected RallyNetworkConfig config;

        #endregion

        #region Properties

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

        #endregion

        #region Constructors

        public RallyUnityWeb3Provider()
        {
        }

        #endregion

        #region Public Methods

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
            UnityWebRequestRpcTaskClient unityClient = new(new Uri(config.Gsn.RpcUrl), null, RallyUnityLogger.Default);
            string apiKey = "";
            if (!string.IsNullOrEmpty(config.RelayerApiKey))
            {
                apiKey = config.RelayerApiKey;
            }

            unityClient.RequestHeaders["Authorization"] = $"Bearer {apiKey}";
            return unityClient;
        }

        #endregion

    }

}