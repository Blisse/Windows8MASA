using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using Newtonsoft.Json.Linq;

namespace JsonRpcModelsLibrary.Readers
{
    public class JsonStreamCommandReader : IJsonReader
    {
        private readonly DataReader _dataReader;

        public JsonStreamCommandReader(Stream stream)
        {
            _dataReader = new DataReader(stream.AsInputStream());
        }

        public async Task<JObject> ReadJObjectAsync()
        {
            uint size = await _dataReader.LoadAsync(sizeof(uint));
            if (size < sizeof(uint))
            {
                // socket was closed
                return null;
            }

            uint contentLength = _dataReader.ReadUInt32();

            uint actualContentLength = await _dataReader.LoadAsync(contentLength);
            if (actualContentLength != contentLength)
            {
                // socket closed before all data read
                return null;
            }

            var content = _dataReader.ReadString(contentLength);
            return JObject.Parse(content);
        }
    }
}
