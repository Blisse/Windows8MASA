using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using JsonRpcModelsLibrary.Models;
using Newtonsoft.Json.Linq;

namespace JsonRpcModelsLibrary.Clients
{
    public class JsonRpcReader
    {
        private readonly Stream _stream;
        private readonly JsonRpcStreamMode _streamMode;

        private JsonRpcReader(JsonRpcStreamMode streamMode = JsonRpcStreamMode.Unknown)
        {
            _streamMode = streamMode;
        }

        public JsonRpcReader(Stream stream, JsonRpcStreamMode mode = JsonRpcStreamMode.Unknown)
            : this(mode)
        {
            _stream = stream;
        }

        public async Task<JToken> ReadJsonRpcObjectAsync(CancellationToken token)
        {
            JToken result = null;
            switch (_streamMode)
            {
                case JsonRpcStreamMode.Unknown:
                    result = await ReadUnknownJsonRpcObjectAsync(token);
                    break;
                case JsonRpcStreamMode.ObjectSizePrefixed:
                    result = await ReadObjectSizePrefixedJsonRpcObjectAsync(token);
                    break;
                default:
                    Debug.Assert(false, "Must have read implementation for all stream modes.");
                    break;
            }
            return result;
        }

        private async Task<JToken> ReadUnknownJsonRpcObjectAsync(CancellationToken token)
        {
            char leadingCharacter = await ReadCharAsync(token);
            while (true)
            {
                if (leadingCharacter == '[')
                {
                    return await ReadUntilParenthesesClosedAsync('[', ']', token);
                }
                if (leadingCharacter == '{')
                {
                    return await ReadUntilParenthesesClosedAsync('{', '}', token);
                }

                leadingCharacter = await ReadCharAsync(token);
            }
        }

        private async Task<char> ReadCharAsync(CancellationToken token)
        {
            byte[] buffer = new byte[2];
            await _stream.ReadAsync(buffer, 0, 2, token);
            char c = BitConverter.ToChar(buffer, 0);
            return c;
        }

        private async Task<JToken> ReadUntilParenthesesClosedAsync(char open, char close, CancellationToken token)
        {
            StringBuilder currentJsonString = new StringBuilder();
            currentJsonString.Append(open);

            int bracketsCount = 1;
            while (bracketsCount > 0)
            {
                char character = await ReadCharAsync(token);
                currentJsonString.Append(character);
                if (character == open)
                {
                    bracketsCount += 1;
                }
                else if (character == close)
                {
                    bracketsCount -= 1;
                }
            }
            String jsonString = currentJsonString.ToString();
            JToken jsonRpcObject = JToken.Parse(jsonString);
            return jsonRpcObject;
        }

        private async Task<JToken> ReadObjectSizePrefixedJsonRpcObjectAsync(CancellationToken token)
        {
            byte[] objectSizeBuffer = new byte[JsonRpcConstants.StreamPrefixSize];
            await _stream.ReadAsync(objectSizeBuffer, 0, (int)JsonRpcConstants.StreamPrefixSize, token);
            uint objectSize = BitConverter.ToUInt32(objectSizeBuffer, 0);

            Debug.Assert(objectSize < UInt32.MaxValue);
            byte[] objectBuffer = new byte[objectSize];
            await _stream.ReadAsync(objectBuffer, 0, (int)objectSize, token);
            string data = BitConverter.ToString(objectBuffer);
            JToken jsonData = JToken.Parse(data);
            return jsonData;
        }
    }
}
