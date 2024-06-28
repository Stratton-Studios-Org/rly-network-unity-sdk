using System.Collections;
using System.Collections.Generic;

using RallyProtocol.Accounts;

using RallyProtocolUnity.Logging;
using RallyProtocolUnity.Storage;

using UnityEngine;

namespace RallyProtocolUnity.Accounts
{

    public class RallyUnityAccountManager : RallyAccountManager
    {

        protected static RallyUnityAccountManager defaultInstance;

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

        public RallyUnityAccountManager() : base(RallyUnityKeyManager.Default, RallyUnityLogger.Default)
        {

        }

    }

}