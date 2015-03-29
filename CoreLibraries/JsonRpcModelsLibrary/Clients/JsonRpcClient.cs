using System.IO;
using System.Threading;
using Windows.Networking;
using Windows.Networking.Sockets;
using JsonRpcModelsLibrary.Models;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace JsonRpcModelsLibrary.Clients
{
    public sealed class JsonRpcClient
    {
        public enum JsonRpcClientStatus
        {
            Initialized,
            Connecting,
            Connected,
            Faulted,
            Stopped
        }

        public JsonRpcClientStatus Status { get; private set; }
        public String HostName { get; private set; }
        public String Port { get; private set; }

        private StreamSocket _streamSocket;
        private Task _readStreamTask;
        private Task _writeStreamTask;
        private readonly CancellationTokenSource _connectionCancellationTokenSource;
        private readonly ConcurrentQueue<StreamContent> _outgoingJsonRpcRequests;
        private readonly ConcurrentDictionary<String, TaskCompletionSource<JsonRpcResponse>> _incompleteJsonRpcRequests;

        public JsonRpcClient()
        {
            Status = JsonRpcClientStatus.Initialized;

            _connectionCancellationTokenSource = new CancellationTokenSource();
            _outgoingJsonRpcRequests = new ConcurrentQueue<StreamContent>();
            _incompleteJsonRpcRequests = new ConcurrentDictionary<String, TaskCompletionSource<JsonRpcResponse>>();
        }

        public async Task ConnectAsync(String host, String port)
        {
            await CloseAsync();

            HostName = host;
            Port = port;

            _streamSocket = new StreamSocket();
            await _streamSocket.ConnectAsync(new HostName(HostName), Port);

            CancellationToken token = _connectionCancellationTokenSource.Token;

            _readStreamTask = Task.Run(() => ReadJsonRpcStreamAsync(token), token);
            _writeStreamTask = Task.Run(() => WriteJsonRpcStreamAsync(token), token);   
        }

        public async Task CloseAsync()
        {
            if (Status != JsonRpcClientStatus.Initialized)
            {
                if (_streamSocket != null)
                {
                    _streamSocket.Dispose();
                    _streamSocket = null;
                }

                _connectionCancellationTokenSource.Cancel();
                await Task.WhenAll(_readStreamTask, _writeStreamTask);
                _readStreamTask = null;
                _writeStreamTask = null;
            }
        }

        public async Task<JsonRpcResponse> SendRequestAsync(JsonRpcRequest jsonRpcRequest)
        {
            Debug.Assert(jsonRpcRequest != null && jsonRpcRequest.Id != null && jsonRpcRequest.Id != String.Empty);

            JTokenStreamContent streamContent = new JTokenStreamContent(JToken.FromObject(jsonRpcRequest));
            TaskCompletionSource<JsonRpcResponse> sendJsonRpcRequestTask = new TaskCompletionSource<JsonRpcResponse>(jsonRpcRequest);
            _outgoingJsonRpcRequests.Enqueue(streamContent);
            _incompleteJsonRpcRequests.AddOrUpdate(jsonRpcRequest.Id, sendJsonRpcRequestTask, (k, v) =>
            {
                Debug.Assert(false, "Should not send requests with the same id.");
                return sendJsonRpcRequestTask;
            });
            return await sendJsonRpcRequestTask.Task;
        }

        public void SendNotificationAsync(JsonRpcRequest jsonRpcRequest)
        {
            Debug.Assert(jsonRpcRequest != null && jsonRpcRequest.Id != null && jsonRpcRequest.Id != String.Empty);
            Debug.Assert(jsonRpcRequest.IsNotification == true);

            JTokenStreamContent streamContent = new JTokenStreamContent(JToken.FromObject(jsonRpcRequest));
            _outgoingJsonRpcRequests.Enqueue(streamContent);
        }

        private async Task ReadJsonRpcStreamAsync(CancellationToken token)
        {
            JsonRpcReader reader = new JsonRpcReader(_streamSocket.InputStream.AsStreamForRead());
            try
            {
                while (!token.IsCancellationRequested)
                {
                    JToken streamJObject = await reader.ReadJsonRpcObjectAsync(token);
                    if (streamJObject["method"] != null) // always true for requests
                    {
                        JsonRpcRequest request = streamJObject.ToObject<JsonRpcRequest>();

                    }
                    else // response
                    {
                        JsonRpcResponse response = streamJObject.ToObject<JsonRpcResponse>();
                        _incompleteJsonRpcRequests[response.Id].SetResult(response);
                    }
                }
            }
            catch (OperationCanceledException)
            {
            }
            catch (IOException ioException)
            {
                // occurs when stream timeouts (not disconnected properly)
                Debug.WriteLine("Caught and handling exception: {0}", ioException);
                Status = JsonRpcClientStatus.Faulted;
            }

            if (Status == JsonRpcClientStatus.Faulted)
            {
                await ConnectAsync(HostName, Port);
            }
            else
            {
                Status = JsonRpcClientStatus.Stopped;
            }
        }

        private async Task WriteJsonRpcStreamAsync(CancellationToken token)
        {
            JsonRpcWriter writer = new JsonRpcWriter(_streamSocket.OutputStream.AsStreamForWrite());
            StreamContent content = null;
            try
            {
                while (!token.IsCancellationRequested)
                {
                    if (_outgoingJsonRpcRequests.TryDequeue(out content))
                    {
                        await writer.WriteJsonRpcObjectAsync(content, token);
                    }
                    
                    await Task.Delay(400, token);
                }
            }
            catch (OperationCanceledException)
            {
            }
            catch (IOException ioException)
            {
                // occurs when stream timeouts (not disconnected)
                Debug.WriteLine("Caught and handling exception: {0}", ioException);
                Status = JsonRpcClientStatus.Faulted;
            }

            if (Status == JsonRpcClientStatus.Faulted)
            {
                await ConnectAsync(HostName, Port);
            }
            else
            {
                Status = JsonRpcClientStatus.Stopped;
            }
        }
    }
}
