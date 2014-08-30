using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace JsonRpcModelsLibrary.Utilities
{
    public static class JObjectExtensions
    {
        public static Boolean IsJsonRpcRequest(this JObject jObject)
        {
            return jObject["method"] != null;
        }

        public static Boolean IsJsonRpcResponse(this JObject jObject)
        {
            return (jObject["result"] != null || jObject["error"] != null);
        }

        public static Boolean IsSuccessJsonRpcResponse(this JObject jObject)
        {
            return IsJsonRpcResponse(jObject) && jObject["result"] != null && jObject["error"] == null;
        }

        public static Boolean IsErrorJsonRpcResponse(this JObject jObject)
        {
            return IsJsonRpcResponse(jObject) && jObject["result"] == null && jObject["error"] != null;
        }
    }
}
