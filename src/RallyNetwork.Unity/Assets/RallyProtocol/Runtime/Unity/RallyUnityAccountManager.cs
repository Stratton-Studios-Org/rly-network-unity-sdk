using System.Collections;
using System.Collections.Generic;

using RallyProtocol.Logging;

using UnityEngine;

namespace RallyProtocol.Accounts
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