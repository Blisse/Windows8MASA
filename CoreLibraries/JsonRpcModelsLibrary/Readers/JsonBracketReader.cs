using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace JsonRpcModelsLibrary.Readers
{
    public class JsonBracketReader : IJsonReader
    {
        private readonly StreamReader _streamReader;

        public JsonBracketReader(Stream stream)
        {
            _streamReader = new StreamReader(stream);
        }

        public async Task<JObject> ReadJObjectAsync()
        {
            var stringBuilder = new StringBuilder();
            int parenCount = 0;
            bool hasReadAnything = false;

            char[] buff = new char[1];

            while (!hasReadAnything || parenCount > 0)
            {
                int readCount = await _streamReader.ReadAsync(buff, 0, 1);

                if (readCount == 0)
                {
                    throw new Exception("ReadCount of 0 on the socket");
                }

                char c = buff[0];

                // Skip everything between brackets
                if (c != '{' && c != '}' && !hasReadAnything)
                {
                    continue;
                }

                stringBuilder.Append(c);

                hasReadAnything = true;

                if (c == '{')
                {
                    parenCount++;
                }
                else if (c == '}')
                {
                    parenCount--;
                }
            }

            return JObject.Parse(stringBuilder.ToString());
        }
    }
}
