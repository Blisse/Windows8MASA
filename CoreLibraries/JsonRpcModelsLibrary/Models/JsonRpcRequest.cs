using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace JsonRpcModelsLibrary.Models
{
    [JsonObject]
    public class JsonRpcRequest
    {
        [JsonProperty("jsonrpc", Required = Required.Always)]
        public String JsonRpc { get; private set; }

        [JsonProperty("method", Required = Required.Always)]
        public String Method { get; private set; }

        [JsonProperty("params", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate, NullValueHandling = NullValueHandling.Include)]
        public JToken Params { get; private set; }

        [JsonProperty("id", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public String Id { get; private set; }

        public JsonRpcRequest()
        {
            JsonRpc = JsonRpcConstants.Version;
        }

        protected JsonRpcRequest(String method) : this()
        {
            Method = method;
        }

        protected JsonRpcRequest(String method, Object param)
            : this(method)
        {
            if (param != null)
            {
                Params = JObject.FromObject(param);   
            }
        }

        public static class Factory
        {
            public static JsonRpcRequest CreateJsonRpcRequest(String method)
            {
                return new JsonRpcRequest(method)
                {
                    Id = Guid.NewGuid().ToString()
                };
            }

            public static JsonRpcRequest CreateJsonRpcRequest(String method, Object param)
            {
                return new JsonRpcRequest(method, param)
                {
                    Id = Guid.NewGuid().ToString()
                };
            }

            public static JsonRpcRequest CreateJsonRpcNotification(String method)
            {
                return new JsonRpcRequest(method);
            }

            public static JsonRpcRequest CreateJsonRpcNotification(String method, Object param)
            {
                return new JsonRpcRequest(method, param);
            }
        }

        [JsonIgnore]
        public bool IsNotification
        {
            get { return Id == null; }
        }
    }
}
