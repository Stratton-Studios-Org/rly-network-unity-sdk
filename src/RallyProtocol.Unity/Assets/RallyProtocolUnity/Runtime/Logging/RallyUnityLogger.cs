using System;
using System.Collections;
using System.Collections.Generic;

using Microsoft.Extensions.Logging;

using RallyProtocol.Logging;

using UnityEngine;

using IUnityLogger = UnityEngine.ILogger;
using UnityLogger = UnityEngine.Logger;

namespace RallyProtocolUnity.Logging
{

    public class RallyUnityLogger : IRallyLogger
    {

        #region Fields

        private static RallyUnityLogger defaultInstance;

        protected LoggerExternalScopeProvider scopeProvider;
        protected IUnityLogger unityLogger;

        #endregion

        #region Properties

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

        public IExternalScopeProvider ScopeProvider => this.scopeProvider;
        public IUnityLogger UnityLogger => this.unityLogger;

        #endregion

        #region Constructors

        public RallyUnityLogger()
        {
            this.scopeProvider = new();
            this.unityLogger = new UnityLogger(Debug.unityLogger);
        }

        #endregion

        #region Public Methods

        public void Log(string message)
        {
            this.unityLogger.Log(message);
        }

        public void LogError(string message)
        {
            this.unityLogger.LogError(string.Empty, message);
        }

        public void LogException(Exception exception)
        {
            this.unityLogger.LogException(exception);
        }

        public void LogWarning(string message)
        {
            this.unityLogger.LogWarning(string.Empty, message);
        }

        #endregion

        #region Microsoft.Extensions.Logging.ILogger

        public static LogType? ToLogType(LogLevel logLevel)
        {
            switch (logLevel)
            {
                case LogLevel.Debug:
                case LogLevel.Information:
                case LogLevel.Trace:
                    return LogType.Log;

                case LogLevel.Warning:
                    return LogType.Warning;

                case LogLevel.Error:
                case LogLevel.Critical:
                    return LogType.Error;

                default:
                case LogLevel.None:
                    return null;
            }
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            this.unityLogger.Log(state);
            return this.scopeProvider.Push(state);
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            if (!this.unityLogger.logEnabled)
            {
                return false;
            }

            LogType? logType = ToLogType(logLevel);
            if (logType == null)
            {
                return false;
            }

            return this.unityLogger.IsLogTypeAllowed(logType.Value);
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            LogType? logType = ToLogType(logLevel);
            if (logType == null)
            {
                return;
            }

            this.unityLogger.Log(logType.Value, formatter(state, exception));
        }

        #endregion

    }

}