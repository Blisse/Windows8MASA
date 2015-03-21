using Newtonsoft.Json.Linq;

namespace JsonRpcModelsLibrary.Utilities
{
    public static class JTokenExtensions
    {
        public static bool IsJsonRpcErrorObject(this JToken jToken)
        {
            if (jToken is JArray)
            {
                // Errors are a single object
                return false;
            }

            if (jToken is JObject)
            {
                return jToken["code"] != null && jToken["message"] != null;
            }

            return false;
        }
    }
}
