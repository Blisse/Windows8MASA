using Newtonsoft.Json.Linq;

namespace JsonRpcModelsLibrary.Models
{
    public class JsonRpcReturnStatus
    {
        public JsonRpcError Error { get; set; }

        public JToken Result { get; set; }
    }
}
