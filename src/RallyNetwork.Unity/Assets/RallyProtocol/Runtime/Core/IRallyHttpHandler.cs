using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace RallyProtocol.Core
{

    public interface IRallyHttpHandler
    {

        public Task<RallyHttpResponse> PostJson(string url, string json, Dictionary<string, string> headers = null);

        public Task<RallyHttpResponse> Get(string url, Dictionary<string, string> headers = null);

    }

    /// <summary>
    /// A generic HTTP response.
    /// </summary>
    public class RallyHttpResponse
    {

        #region Fields

        protected string url;
        protected long responseCode;
        protected string responseText;
        protected string requestText;

        protected Dictionary<string, string> responseHeaders;
        protected string errorMessage;
        protected bool success;

        #endregion

        #region Properties

        public string Url => this.url;
        public long ResponseCode => this.responseCode;
        public string ResponseText => this.responseText;
        public string RequestText => this.requestText;

        public IReadOnlyDictionary<string, string> ResponseHeaders => this.responseHeaders;
        public string ErrorMessage => this.errorMessage;
        public bool IsCompletedSuccessfully => string.IsNullOrEmpty(ErrorMessage) && this.success;

        #endregion

        #region Constructors

        public RallyHttpResponse(string url, long responseCode, string responseText, string requestText, Dictionary<string, string> responseHeaders, string errorMessage, bool success)
        {
            this.url = url;
            this.responseCode = responseCode;
            this.responseText = responseText;
            this.requestText = requestText;
            this.responseHeaders = responseHeaders;
            this.errorMessage = errorMessage;
            this.success = success;
        }

        #endregion

        #region Public Methods

        public TData DeserializeJson<TData>()
        {
            return JsonConvert.DeserializeObject<TData>(ResponseText);
        }

        #endregion

    }

}