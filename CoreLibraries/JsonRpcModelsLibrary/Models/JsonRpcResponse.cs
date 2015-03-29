using System;
using System.Diagnostics;
using JsonRpcModelsLibrary.Clients;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace JsonRpcModelsLibrary.Models
{
    [JsonObject]
    public class JsonRpcResponse
    {
        [JsonProperty("jsonrpc", Required = Required.Always)]
        public String JsonRpc { get; private set; }

        // nulls are not sent over JSON, but nulls are set when converted to Objects
        [JsonProperty("error", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate, NullValueHandling = NullValueHandling.Ignore)]
        public JsonRpcError Error { get; private set; }

        // nulls are not sent over JSON, but nulls are set when converted to Objects
        [JsonProperty("result", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate, NullValueHandling = NullValueHandling.Ignore)]
        public JToken Result { get; private set; }

        // Id is always required because we should only respond to requests
        [JsonProperty("id", Required = Required.Always)]
        public String Id { get; private set; }

        public JsonRpcResponse()
        {
        }

        private JsonRpcResponse(JsonRpcRequest request)
        {
            Debug.Assert(request.IsNotification == false, "Cannot respond to a notification.");
            JsonRpc = JsonRpcConstants.Version;
            Error = null;
            Result = null;
            Id = request.Id;
        }

        protected JsonRpcResponse(JsonRpcRequest request, JsonRpcError error) : this(request)
        {
            Error = error;
        }

        protected JsonRpcResponse(JsonRpcRequest request, Object result) : this(request)
        {
            Result = JToken.FromObject(result);
        }

        public static class Factory
        {
            private static JsonRpcResponse CreateGoodResponse(JsonRpcRequest request, Object result)
            {
                return new JsonRpcResponse(request, result);
            }

            private static JsonRpcResponse CreateErrorResponse(JsonRpcRequest request, JsonRpcError error)
            {
                return new JsonRpcResponse(request, error);
            }
        }
    }
}
