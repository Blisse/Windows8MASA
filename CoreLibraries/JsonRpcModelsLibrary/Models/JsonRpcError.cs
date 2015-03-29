using System;
using Windows.UI.Xaml.Controls;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace JsonRpcModelsLibrary.Models
{
    [JsonObject]
    public class JsonRpcError
    {
        [JsonProperty("code")]
        public int Code { get; set; }

        [JsonProperty("message")]
        public String Message { get; set; }

        [JsonProperty("data", NullValueHandling = NullValueHandling.Ignore)]
        public JToken Data { get; set; }

        public JsonRpcError()
        {
            
        }

        protected JsonRpcError(int code, String message)
        {
            Code = code;
            Message = message;
        }

        protected JsonRpcError(int code, String message, Object data) : this(code, message)
        {
            if (data != null)
            {
                Data = JToken.FromObject(data);
            }
        }

        public static class Factory
        {
            public static JsonRpcError CreateError(int code, String message, Object data = null)
            {
                return new JsonRpcError(code, message, data);
            }

            public static JsonRpcError CreateParseError(Object data = null)
            {
                return new JsonRpcError(JsonRpcConstants.ParseErrorCode, JsonRpcConstants.ParseErrorMessage, data);
            }

            public static JsonRpcError CreateInvalidRequestError(Object data = null)
            {
                return new JsonRpcError(JsonRpcConstants.InvalidRequestErrorCode, JsonRpcConstants.InvalidRequestErrorMessage, data);
            }

            public static JsonRpcError CreateMethodNotFoundError(Object data = null)
            {
                return new JsonRpcError(JsonRpcConstants.MethodNotFoundErrorCode, JsonRpcConstants.MethodNotFoundErrorMessage, data);
            }

            public static JsonRpcError CreateInvalidParamsError(Object data = null)
            {
                return new JsonRpcError(JsonRpcConstants.InvalidParamsErrorCode, JsonRpcConstants.InvalidParamsErrorMessage, data);
            }

            public static JsonRpcError CreateInternalError(Object data = null)
            {
                return new JsonRpcError(JsonRpcConstants.InternalErrorCode, JsonRpcConstants.InternalErrorMessage, data);
            }
        }
    }
}
