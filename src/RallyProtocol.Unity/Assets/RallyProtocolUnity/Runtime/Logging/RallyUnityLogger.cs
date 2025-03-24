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
    /// <summary>
    /// Unity-specific implementation of the Rally Protocol logging system.
    /// Provides a bridge between the Rally SDK logging infrastructure and Unity's built-in logging system.
    /// </summary>
    /// <remarks>
    /// This class implements both the Rally Protocol <see cref="IRallyLogger"/> interface and the 
    /// Microsoft.Extensions.Logging <see cref="ILogger"/> interface, providing comprehensive logging 
    /// capabilities within the Unity environment.
    /// </remarks>
    public class RallyUnityLogger : IRallyLogger
    {
        #region Fields

        /// <summary>
        /// The singleton instance of the RallyUnityLogger.
        /// </summary>
        private static RallyUnityLogger defaultInstance;

        /// <summary>
        /// Provider for logging scopes, used to implement the Microsoft.Extensions.Logging ILogger interface.
        /// </summary>
        protected LoggerExternalScopeProvider scopeProvider;

        /// <summary>
        /// The Unity logger instance that is used for actual log output.
        /// </summary>
        protected IUnityLogger unityLogger;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the default singleton instance of the RallyUnityLogger.
        /// If no instance exists, one will be created automatically.
        /// </summary>
        /// <value>The singleton instance of <see cref="RallyUnityLogger"/>.</value>
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

        /// <summary>
        /// Gets the scope provider for structured logging.
        /// </summary>
        /// <value>The <see cref="IExternalScopeProvider"/> used for logging scopes.</value>
        public IExternalScopeProvider ScopeProvider => this.scopeProvider;

        /// <summary>
        /// Gets the Unity logger instance used for actual log output.
        /// This can be used to configure Unity-specific logging settings.
        /// </summary>
        /// <value>The <see cref="IUnityLogger"/> instance.</value>
        public IUnityLogger UnityLogger => this.unityLogger;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RallyUnityLogger"/> class.
        /// Creates a logger that uses Unity's built-in Debug.unityLogger for output.
        /// </summary>
        public RallyUnityLogger()
        {
            this.scopeProvider = new();
            this.unityLogger = new UnityLogger(Debug.unityLogger);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Logs an informational message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        public void Log(string message)
        {
            this.unityLogger.Log(message);
        }

        /// <summary>
        /// Logs an error message.
        /// </summary>
        /// <param name="message">The error message to log.</param>
        public void LogError(string message)
        {
            this.unityLogger.LogError(string.Empty, message);
        }

        /// <summary>
        /// Logs an exception.
        /// </summary>
        /// <param name="exception">The exception to log.</param>
        public void LogException(Exception exception)
        {
            this.unityLogger.LogException(exception);
        }

        /// <summary>
        /// Logs a warning message.
        /// </summary>
        /// <param name="message">The warning message to log.</param>
        public void LogWarning(string message)
        {
            this.unityLogger.LogWarning(string.Empty, message);
        }

        #endregion

        #region Microsoft.Extensions.Logging.ILogger

        /// <summary>
        /// Converts a Microsoft.Extensions.Logging LogLevel to a Unity LogType.
        /// </summary>
        /// <param name="logLevel">The LogLevel to convert.</param>
        /// <returns>The corresponding Unity LogType, or null if there is no appropriate mapping.</returns>
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

        /// <summary>
        /// Begins a logical operation scope.
        /// </summary>
        /// <typeparam name="TState">The type of the scope state.</typeparam>
        /// <param name="state">The state to associate with the scope.</param>
        /// <returns>An IDisposable that ends the logical operation scope on disposal.</returns>
        public IDisposable BeginScope<TState>(TState state)
        {
            this.unityLogger.Log(state);
            return this.scopeProvider.Push(state);
        }

        /// <summary>
        /// Checks if the given LogLevel is enabled.
        /// </summary>
        /// <param name="logLevel">The LogLevel to check.</param>
        /// <returns>true if the LogLevel is enabled; otherwise, false.</returns>
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

        /// <summary>
        /// Writes a log entry.
        /// </summary>
        /// <typeparam name="TState">The type of the object to be written.</typeparam>
        /// <param name="logLevel">The LogLevel of the log entry.</param>
        /// <param name="eventId">The event id associated with the log.</param>
        /// <param name="state">The entry to be written.</param>
        /// <param name="exception">The exception related to this entry.</param>
        /// <param name="formatter">Function to create a string message of the state and exception.</param>
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