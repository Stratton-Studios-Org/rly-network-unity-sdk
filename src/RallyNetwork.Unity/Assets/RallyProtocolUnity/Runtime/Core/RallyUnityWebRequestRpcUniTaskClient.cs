using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using Cysharp.Threading.Tasks;

using Microsoft.Extensions.Logging;

using Nethereum.JsonRpc.Client;
using Nethereum.JsonRpc.Client.RpcMessages;

using Newtonsoft.Json;

using UnityEngine.Networking;

using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace RallyProtocolUnity
{

    public class UnityWebRequestRpcTaskClient : ClientBase, IClientRequestHeaderSupport
    {

        #region Fields

        private readonly Uri _baseUrl;

        private readonly JsonSerializerSettings _jsonSerializerSettings;

        private readonly ILogger _log;

        #endregion

        #region Properties

        public Dictionary<string, string> RequestHeaders { get; set; } = new Dictionary<string, string>();

        #endregion

        #region Constructors

        public UnityWebRequestRpcTaskClient(Uri baseUrl, JsonSerializerSettings jsonSerializerSettings = null, ILogger log = null)
        {
            _baseUrl = baseUrl;
            if (jsonSerializerSettings == null)
            {
                jsonSerializerSettings = DefaultJsonSerializerSettingsFactory.BuildDefaultJsonSerializerSettings();
            }

            _jsonSerializerSettings = jsonSerializerSettings;
            _log = log;
            this.SetBasicAuthenticationHeaderFromUri(baseUrl);
        }

        #endregion

        #region Protected Methods

        protected override async Task<RpcResponseMessage[]> SendAsync(RpcRequestMessage[] requests)
        {
            RpcLogger logger = new RpcLogger(_log);
            string absoluteUri = _baseUrl.AbsoluteUri;
            string text = JsonConvert.SerializeObject(requests, _jsonSerializerSettings);
            Encoding.UTF8.GetBytes(text);
            string value = await SendAsyncInternally(text, absoluteUri, string.Empty, logger);
            try
            {
                return JsonConvert.DeserializeObject<RpcResponseMessage[]>(value, _jsonSerializerSettings);
            }
            catch (Exception innerException)
            {
                throw new RpcClientUnknownException("Error occurred when trying to send rpc request(s): ", innerException);
            }
        }

        protected override async Task<RpcResponseMessage> SendAsync(RpcRequestMessage request, string route = null)
        {
            RpcLogger logger = new RpcLogger(_log);
            string absoluteUri = new Uri(_baseUrl, route).AbsoluteUri;
            string rpcRequestJson = JsonConvert.SerializeObject(request, _jsonSerializerSettings);
            string value = await SendAsyncInternally(rpcRequestJson, absoluteUri, request.Method, logger);
            try
            {
                RpcResponseMessage rpcResponseMessage = JsonConvert.DeserializeObject<RpcResponseMessage>(value, _jsonSerializerSettings);
                logger.LogResponse(rpcResponseMessage);
                return rpcResponseMessage;
            }
            catch (Exception innerException)
            {
                throw new RpcClientUnknownException("Error occurred when trying to send rpc request(s): " + request.Method, innerException);
            }
        }

        #endregion

        #region Private Methods

        private async Task<string> SendAsyncInternally(string rpcRequestJson, string uri, string rpcRequestMethod, RpcLogger logger)
        {
            if (rpcRequestMethod == null)
            {
                rpcRequestMethod = string.Empty;
            }

            await UniTask.SwitchToMainThread();
            byte[] bytes = Encoding.UTF8.GetBytes(rpcRequestJson);
            using UnityWebRequest unityRequest = new UnityWebRequest(uri, "POST");
            UploadHandlerRaw uploadHandlerRaw = new UploadHandlerRaw(bytes);
            unityRequest.SetRequestHeader("Content-Type", "application/json");
            uploadHandlerRaw.contentType = "application/json";
            unityRequest.uploadHandler = uploadHandlerRaw;
            unityRequest.downloadHandler = new DownloadHandlerBuffer();
            if (RequestHeaders != null)
            {
                foreach (KeyValuePair<string, string> requestHeader in RequestHeaders)
                {
                    unityRequest.SetRequestHeader(requestHeader.Key, requestHeader.Value);
                }
            }

            logger.LogRequest(rpcRequestJson);
            try
            {
                await unityRequest.SendWebRequest();
            }
            catch (Exception innerException)
            {
                logger.LogException(innerException);
            }

            if (unityRequest.error != null)
            {
                throw new RpcClientUnknownException("Error occurred when trying to send rpc request(s): " + rpcRequestMethod, new Exception(unityRequest.error));
            }

            byte[] data = unityRequest.downloadHandler.data;
            return Encoding.UTF8.GetString(data);
        }

        #endregion

    }

}