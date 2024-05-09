using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace RallyProtocol.Logging
{

    public class RallyUnityLogger : IRallyLogger
    {

        private static RallyUnityLogger defaultInstance;

        public static RallyUnityLogger Default
        {
            get
            {
                if (defaultInstance == null)
                {
                    defaultInstance = new RallyUnityLogger();
                }

                return defaultInstance;
            }
        }

        public void Log(string message)
        {
            Debug.Log(message);
        }

        public void LogAssertion(string message)
        {
            Debug.LogAssertion(message);
        }

        public void LogError(string message)
        {
            Debug.LogError(message);
        }

        public void LogException(Exception exception)
        {
            Debug.LogError(exception);
        }

        public void LogWarning(string message)
        {
            Debug.LogWarning(message);
        }

    }

}