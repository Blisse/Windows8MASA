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

        // params will always appear
        [JsonProperty("params", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate, NullValueHandling = NullValueHandling.Include)]
        public JToken Params { get; private set; }

        // null Id means Notification.
        [JsonProperty("id", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate, NullValueHandling = NullValueHandling.Include)]
        public String Id { get; private set; }

        public JsonRpcRequest()
        {

        }

        private JsonRpcRequest(String method)
        {
            JsonRpc = JsonRpcConstants.Version;
            Method = method;
            Params = null;
            Id = null;
        }

        protected JsonRpcRequest(String method, Object param = null) : this(method)
        {
            if (param != null)
            {
                Params = JToken.FromObject(param);
            }
        }

        protected JsonRpcRequest(String id, String method, Object param = null) : this(method, param)
        {
            Id = id;
        }

        public static class Factory
        {
            public static JsonRpcRequest CreateNotification(String method, Object param = null)
            {
                return new JsonRpcRequest(method, param);
            }

            public static JsonRpcRequest CreateRequest(String method, Object param = null)
            {
                String id = Guid.NewGuid().ToString();
                return new JsonRpcRequest(id, method, param);
            }
        }

        [JsonIgnore]
        public bool IsNotification
        {
            get { return Id == null; }
        }
    }
}
