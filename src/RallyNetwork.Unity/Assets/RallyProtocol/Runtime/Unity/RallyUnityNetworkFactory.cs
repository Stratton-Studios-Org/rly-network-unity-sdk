using System.Collections;
using System.Collections.Generic;

using RallyProtocol.Accounts;
using RallyProtocol.Core;
using RallyProtocol.Logging;

using UnityEngine;

namespace RallyProtocol.Networks
{

    public class RallyUnityNetworkFactory
    {

        public static IRallyWeb3Provider GetWeb3Provider()
        {
            return RallyUnityWeb3Provider.Default;
        }

        public static IRallyHttpHandler GetHttpHandler()
        {
            return RallyUnityHttpHandler.Default;
        }

        public static IRallyLogger GetLogger()
        {
            return RallyUnityLogger.Default;
        }

        public static IRallyAccountManager GetAccountManager()
        {
            return RallyUnityAccountManager.Default;
        }

        public static IRallyNetwork Create(RallyNetworkType type, string apiKey = null)
        {
            return RallyNetworkFactory.Create(GetWeb3Provider(), GetHttpHandler(), GetLogger(), GetAccountManager(), type, apiKey);
        }

        public static IRallyNetwork Create(RallyNetworkConfig config, string apiKey = null)
        {
            return RallyNetworkFactory.Create(GetWeb3Provider(), GetHttpHandler(), GetLogger(), GetAccountManager(), config, apiKey);
        }

    }

}