using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace JsonRpcModelsLibrary.Readers
{
    public interface IJsonReader
    {
        Task<JObject> ReadJObjectAsync();
    }
}
