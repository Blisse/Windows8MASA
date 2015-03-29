using System;
using Newtonsoft.Json.Linq;

namespace JsonRpcModelsLibrary.Models
{
    public class JsonRpcReturnStatus
    {
        public Boolean IsError { get; set; }
        public JsonRpcError Error { get; set; }

        public Boolean IsCompleted { get; set; }
        public JToken Result { get; set; }
    }
}
