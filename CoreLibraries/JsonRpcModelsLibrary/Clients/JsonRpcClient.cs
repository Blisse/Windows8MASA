using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using Windows.Networking;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
using JsonRpcModelsLibrary.Annotations;
using JsonRpcModelsLibrary.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using JsonRpcModelsLibrary.Readers;
using JsonRpcModelsLibrary.Utilities;
using Newtonsoft.Json.Linq;

namespace JsonRpcModelsLibrary.Clients
{
    public interface IJsonRpcRequest
    {
        Object GetContent();
        String GetCommand();
    }

    public class Response
    {
        public JToken Content { get; set; }
        public bool IsError { get { return !String.IsNullOrWhiteSpace(Error); } }
        public String Error { get; set; }
    }

    public sealed class JsonRpcClient : INotifyPropertyChanged
    {
        private bool _isConnected;
        public bool IsConnected
        {
            get { return _isConnected; }
            set
            {
                if (value == _isConnected)
                {
                    return;
                }

                _isConnected = value;
                RaisePropertyChanged();
            }
        }

        public readonly String HostName;
        public readonly String Port;
        private DataWriter _dataWriter;
        private StreamSocket _streamSocket;
        private readonly ConcurrentQueue<StreamCommand> _sendToClientQueue;
        private readonly ConcurrentDictionary<String, TaskCompletionSource<Response>> _incompleteJsonRpcRequests;
        private readonly Dictionary<String, Func<JToken, Task<Response>>> _subscribedRequestCallbacks;
        private readonly Dictionary<String, Action<JToken>> _subscribedNotificationCallbacks;

        /// <summary>
        /// Create a new JsonRpcClient, binding the client to a specific host IP and port number
        /// </summary>
        /// <param name="host">Host IP Address</param>
        /// <param name="port">Port to connect to</param>
        public JsonRpcClient(String host, String port)
        {
            HostName = host;
            Port = port;

            _sendToClientQueue = new ConcurrentQueue<StreamCommand>();
            _incompleteJsonRpcRequests = new ConcurrentDictionary<String, TaskCompletionSource<Response>>();
            _subscribedRequestCallbacks = new Dictionary<String, Func<JToken, Task<Response>>>();
            _subscribedNotificationCallbacks = new Dictionary<String, Action<JToken>>();

            IsConnected = false;
        }

        /// <summary>
        /// Establish a connection to the Host and Port, setting IsConnected to true if the connection is successful.
        /// This does not start the service.
        /// </summary>
        /// <returns></returns>
        public async Task ConnectAsync()
        {
            try
            {
                if (_streamSocket != null)
                {
                    _streamSocket.Dispose();
                    _streamSocket = null;
                }

                _streamSocket = new StreamSocket();
                await _streamSocket.ConnectAsync(new HostName(HostName), Port);

                _dataWriter = new DataWriter(_streamSocket.OutputStream);

                IsConnected = true;
            }
            catch (Exception e)
            {
                Debug.WriteLine("Caught exception: {0}", e);
            }
        }

        /// <summary>
        /// Start receiving and sending messages. No action will occur until this method is called.
        /// </summary>
        public void Start()
        {
            var receiveMessagesTask = Task.Run(() => ReceiveMessagesFromClientAsync());
            var sendMessagesTask = Task.Run(() => SendQueuedMessagesToClientAsync());   
        }

        /// <summary>
        /// Stop receiving and sending messages. Messages not sent yet will still be queued.
        /// </summary>
        public void Stop()
        {
            IsConnected = false;
        }

        public Task<Response> SendRequestAsync(IJsonRpcRequest userJsonRpcRequest)
        {
            var jsonRpcRequest = JsonRpcRequest.Factory.CreateJsonRpcRequest(userJsonRpcRequest.GetCommand(), userJsonRpcRequest.GetContent());
            var jsonRpcJObject = JObject.FromObject(jsonRpcRequest);
            var streamCommand = JObjectStreamCommand.Factory.Create(jsonRpcJObject);

            var requestTaskCompletionSource = new TaskCompletionSource<Response>();

            _sendToClientQueue.Enqueue(streamCommand);

            if (!_incompleteJsonRpcRequests.TryAdd(jsonRpcRequest.Id, requestTaskCompletionSource))
            {
                // this shouldn't throw
                throw new Exception();
            }

            return requestTaskCompletionSource.Task;
        }

        public void SendNotification(String command, Object content)
        {
            var jsonRpcRequest = JsonRpcRequest.Factory.CreateJsonRpcNotification(command, content);
            var jsonRpcJObject = JObject.FromObject(jsonRpcRequest);
            var streamCommand = JObjectStreamCommand.Factory.Create(jsonRpcJObject);

            _sendToClientQueue.Enqueue(streamCommand);
        }

        public void SubscribeForRequests(String command, Func<JToken, Task<Response>> requestCallback)
        {
            _subscribedRequestCallbacks.Add(command, requestCallback);
        }

        public void SubscribeForNotifications(String command, Action<Object> notificationCallback)
        {
            _subscribedNotificationCallbacks.Add(command, notificationCallback);
        }

        private async Task ReceiveMessagesFromClientAsync()
        {
            try
            {
                IJsonReader reader = new JsonBracketReader(_streamSocket.InputStream.AsStreamForRead());

                while (IsConnected)
                {
                    var streamJObject = await reader.ReadJObjectAsync();
                    HandleReceivedJObject(streamJObject);
                }
            }
            catch (IOException ioException)
            {
                // occurs when stream timeouts (not disconnected)
                Debug.WriteLine("Caught and handling exception: {0}", ioException);
                HandleStreamClosed(ReceiveMessagesFromClientAsync);
            }
            catch (Exception e)
            {
                Debug.WriteLine("Caught exception: {0}", e);
                IsConnected = false;
            }
        }

        private async Task SendQueuedMessagesToClientAsync()
        {
            StreamCommand command = null;

            try
            {
                while (IsConnected)
                {
                    if (_sendToClientQueue.TryDequeue(out command))
                    {
                        _dataWriter.WriteUInt32(command.ContentLength);
                        _dataWriter.WriteBytes(command.ContentBytes);
                        await _dataWriter.StoreAsync();
                    }
                    else
                    {
                        await Task.Delay(250);
                    }
                }
            }
            catch (IOException ioException)
            {
                // occurs when stream timeouts (not disconnected)
                Debug.WriteLine("Caught and handling exception: {0}", ioException);
                if (command != null)
                {
                    // re-queue command to try sending again
                    _sendToClientQueue.Enqueue(command);
                }
                HandleStreamClosed(SendQueuedMessagesToClientAsync);
            }
            catch (Exception e)
            {
                Debug.WriteLine("Caught exception: {0}", e);
                IsConnected = false;
            }
        }

        private async void HandleStreamClosed(Func<Task> taskToRestart)
        {
            await ConnectAsync();
            var restartTask = Task.Run(() => taskToRestart);
        }

        private async void HandleReceivedJObject(JObject jObject)
        {
            if (jObject.IsJsonRpcRequest())
            {
                JsonRpcRequest jsonRpcRequest = jObject.ToObject<JsonRpcRequest>();
                String method = jsonRpcRequest.Method;

                if (jsonRpcRequest.IsNotification)
                {
                    if (_subscribedNotificationCallbacks.ContainsKey(method))
                    {
                        _subscribedNotificationCallbacks[method].Invoke(jsonRpcRequest.Params);
                    }
                }
                else
                {
                    if (_subscribedRequestCallbacks.ContainsKey(method))
                    {
                        Response response = await _subscribedRequestCallbacks[method].Invoke(jsonRpcRequest.Params);
                        JsonRpcResponse jsonRpcResponse = JsonRpcResponse.Factory.CreateJsonRpcResponse(jsonRpcRequest, response);
                        StreamCommand responseCommand = JObjectStreamCommand.Factory.Create(JObject.FromObject(jsonRpcResponse));
                        _sendToClientQueue.Enqueue(responseCommand);
                    }
                }
            }
            else if (jObject.IsJsonRpcResponse())
            {
                JsonRpcResponse jsonRpcResponse = jObject.ToObject<JsonRpcResponse>();
                TaskCompletionSource<Response> requestTaskCompletionSource;

                if (_incompleteJsonRpcRequests.TryGetValue(jsonRpcResponse.Id, out requestTaskCompletionSource))
                {
                    Response response = new Response();
                    if (jObject.IsErrorJsonRpcResponse())
                    {
                        response.Error = jsonRpcResponse.Error.Message;
                    }
                    else
                    {
                        response.Content = jsonRpcResponse.Result;
                    }

                    requestTaskCompletionSource.SetResult(response);
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
