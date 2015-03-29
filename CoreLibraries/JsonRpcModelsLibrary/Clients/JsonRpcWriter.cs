using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using JsonRpcModelsLibrary.Models;
using Newtonsoft.Json.Linq;

namespace JsonRpcModelsLibrary.Clients
{
    public class JsonRpcWriter
    {
        private readonly Stream _stream;
        private readonly JsonRpcStreamMode _streamMode;

        private JsonRpcWriter(JsonRpcStreamMode streamMode = JsonRpcStreamMode.Unknown)
        {
            _streamMode = streamMode;
        }

        public JsonRpcWriter(Stream stream, JsonRpcStreamMode mode = JsonRpcStreamMode.Unknown) : this(mode)
        {
            _stream = stream;
        }
        public async Task WriteJsonRpcObjectAsync(StreamContent streamContent, CancellationToken token)
        {
            switch (_streamMode)
            {
                case JsonRpcStreamMode.Unknown:
                    await WriteUnknownJsonRpcObjectAsync(streamContent, token);
                    break;
                case JsonRpcStreamMode.ObjectSizePrefixed:
                    await WriteObjectSizePrefixedJsonRpcObjectAsync(streamContent, token);
                    break;
                default:
                    Debug.Assert(false, "Must have read implementation for all stream modes.");
                    break;
            }
        }

        private async Task WriteUnknownJsonRpcObjectAsync(StreamContent content, CancellationToken token)
        {
            await _stream.WriteAsync(content.ContentBytes, 0, (int)content.ContentLength, token);
        }

        private async Task WriteObjectSizePrefixedJsonRpcObjectAsync(StreamContent content, CancellationToken token)
        {
            byte[] contentLengthBytes = BitConverter.GetBytes(content.ContentLength);
            await _stream.WriteAsync(contentLengthBytes, 0, contentLengthBytes.Length, token);
            await _stream.WriteAsync(content.ContentBytes, 0, (int)content.ContentLength, token);
        }
    }
}
