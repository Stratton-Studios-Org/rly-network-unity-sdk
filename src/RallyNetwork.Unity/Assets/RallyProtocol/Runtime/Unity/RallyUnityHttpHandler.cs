using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

using Cysharp.Threading.Tasks;

using UnityEngine;
using UnityEngine.Networking;

namespace RallyProtocol
{

    public class RallyUnityHttpHandler : IRallyHttpHandler
    {

        public const string JsonContentType = "application/json";

        private static RallyUnityHttpHandler defaultInstance;

        public static RallyUnityHttpHandler Default
        {
            get
            {
                if (defaultInstance == null)
                {
                    defaultInstance = new();
                }

                return defaultInstance;
            }
        }

        public async Task<RallyHttpResponse> PostJson(string url, string json, Dictionary<string, string> headers = null)
        {
            UnityWebRequest request = UnityWebRequest.Post(url, json, JsonContentType);
            await request.SendWebRequest();
            return new(request.url, request.responseCode, request.downloadHandler.text, json, request.GetResponseHeaders(), request.error, request.result == UnityWebRequest.Result.Success);
        }

        public async Task<RallyHttpResponse> Get(string url, Dictionary<string, string> headers = null)
        {
            UnityWebRequest request = UnityWebRequest.Get(url);
            await request.SendWebRequest();
            return new(request.url, request.responseCode, request.downloadHandler.text, string.Empty, request.GetResponseHeaders(), request.error, request.result == UnityWebRequest.Result.Success);
        }

    }

}