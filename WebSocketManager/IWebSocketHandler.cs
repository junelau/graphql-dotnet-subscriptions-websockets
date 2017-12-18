using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Microsoft.Extensions.Logging;

namespace WebSocketManager
{
    /// <summary>
    /// receives  communication events from client sockets. Note that this does not implement a writer
    ///</summary>
    public abstract class IWebSocketReceiver
    {
        public WebSocketConnectionManager _connectionManager { get; set; }

        public readonly ILogger _logger;

        public IWebSocketReceiver(WebSocketConnectionManager webSocketConnectionManager, ILogger<IWebSocketReceiver> logger)
        {
            _connectionManager = webSocketConnectionManager;
            _logger = logger;
        }

        public virtual Task OnConnected(WebSocket socket)
        {// save connection against user
            _logger.LogInformation($"Added socket connection id.");
            _connectionManager.AddSocket(socket);
            return Task.CompletedTask;
        }

        public virtual Task OnDisconnected(WebSocket socket)
        {
            return _connectionManager.RemoveSocket(_connectionManager.GetId(socket));
        }
        public abstract Task ReceiveAsync(WebSocket socket, WebSocketReceiveResult result, string serialisedPayload);
    }

    public abstract class IWebSocketWriter
    {
        public abstract Task SendMessageAsync(WebSocket socket, object message);
        public abstract Task SendToGroup(string groupName, object message);
    }
}