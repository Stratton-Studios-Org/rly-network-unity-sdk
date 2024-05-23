using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

using Cysharp.Threading.Tasks;

using RallyProtocol.Logging;

using UnityEngine.Networking;

namespace RallyProtocol
{

    /// <summary>
    /// Uses <see cref="UnityWebRequest"/> to perform web requests.
    /// </summary>
    public class RallyUnityHttpHandler : IRallyHttpHandler
    {

        #region Constants

        public const string JsonContentType = "application/json";

        #endregion

        #region Fields

        private static RallyUnityHttpHandler defaultInstance;

        protected IRallyLogger logger;

        #endregion

        #region Properties

        public static RallyUnityHttpHandler Default
        {
            get
            {
                if (defaultInstance == null)
                {
                    defaultInstance = new(RallyUnityLogger.Default);
                }

                return defaultInstance;
            }
        }

        #endregion

        #region Constructors

        public RallyUnityHttpHandler(IRallyLogger logger)
        {
            this.logger = logger;
        }

        #endregion

        #region Public Methods

        public async Task<RallyHttpResponse> PostJson(string url, string json, Dictionary<string, string> headers = null)
        {
            UnityWebRequest request = UnityWebRequest.Post(url, json, JsonContentType);
            AddHeaders(request, headers);
            try
            {
                await request.SendWebRequest();
            }
            catch (Exception ex)
            {
                this.logger.LogException(ex);
                this.logger.LogError(json);
            }

            return new(request.url, request.responseCode, request.downloadHandler.text, json, request.GetResponseHeaders(), request.error, request.result == UnityWebRequest.Result.Success);
        }

        public async Task<RallyHttpResponse> Get(string url, Dictionary<string, string> headers = null)
        {
            UnityWebRequest request = UnityWebRequest.Get(url);
            AddHeaders(request, headers);
            await request.SendWebRequest();
            return new(request.url, request.responseCode, request.downloadHandler.text, string.Empty, request.GetResponseHeaders(), request.error, request.result == UnityWebRequest.Result.Success);
        }

        public void AddHeaders(UnityWebRequest request, Dictionary<string, string> headers)
        {
            if (headers == null)
            {
                return;
            }

            foreach (var header in headers)
            {
                request.SetRequestHeader(header.Key, header.Value);
            }
        }

        #endregion

    }

}