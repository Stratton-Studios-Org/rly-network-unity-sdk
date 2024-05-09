using System.Collections;
using System.Collections.Generic;

using RallyProtocol.Accounts;
using RallyProtocol.Core;
using RallyProtocol.Logging;

using UnityEngine;

namespace RallyProtocol.Networks
{

    public class RallyNetworkFactory
    {

        public static IRallyNetwork Create(IRallyWeb3Provider web3Provider, IRallyHttpHandler httpHandler, IRallyLogger logger, IRallyAccountManager accountManager, RallyNetworkType type, string apiKey = null)
        {
            RallyNetworkConfig config;
            switch (type)
            {
                default:
                case RallyNetworkType.Local:
                    config = RallyNetworkConfig.Local;
                    break;
                case RallyNetworkType.Amoy:
                    config = RallyNetworkConfig.Amoy;
                    break;
                case RallyNetworkType.Polygon:
                    config = RallyNetworkConfig.Polygon;
                    break;
                case RallyNetworkType.Test:
                    config = RallyNetworkConfig.Test;
                    break;
            }

            return Create(web3Provider, httpHandler, logger, accountManager, config, apiKey);
        }

        public static IRallyNetwork Create(IRallyWeb3Provider web3Provider, IRallyHttpHandler httpHandler, IRallyLogger logger, IRallyAccountManager accountManager, RallyNetworkConfig config, string apiKey = null)
        {
            RallyEvmNetwork network = new(web3Provider, httpHandler, logger, accountManager, config);
            network.SetApiKey(apiKey);
            return network;
        }

    }

}