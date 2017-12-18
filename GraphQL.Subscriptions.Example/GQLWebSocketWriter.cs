using System;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GraphQL.Subscriptions.Example;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using WebSocketManager;

namespace GraphQL.Subscriptions.Example
{


    public class GQLWebSocketWriter : IWebSocketWriter
    {
        public GQLWebSocketWriter(WebSocketConnectionManager connectionManager, ILogger<GQLWebSocketWriter> logger)
        {
            _connectionManager = connectionManager;
            _logger = logger;
        }
        public JsonSerializerSettings _jsonSerializerSettings = new JsonSerializerSettings()
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };
        private readonly WebSocketConnectionManager _connectionManager;
        private readonly ILogger<GQLWebSocketWriter> _logger;

        public override async Task SendMessageAsync(WebSocket socket, object message)
        {
            if (socket.State != WebSocketState.Open)
                return;
            var encodedMessage = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject((GQLMessage)message, _jsonSerializerSettings));
            await socket.SendAsync(buffer: new ArraySegment<byte>(array: encodedMessage,
                                                                  offset: 0,
                                                                  count: encodedMessage.Length),
                                   messageType: WebSocketMessageType.Text,
                                   endOfMessage: true,
                                   cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        public override async Task SendToGroup(string groupName, object message)
        {
            // for graphql subscriptions transport, each connection can have multiple operations (opId), 
            // which represent the client's many subscriptions to different channels.
            var groupSockets = _connectionManager.SocketOperationsInGroup(groupName);
            if (!groupSockets.Any())
            {
                _logger.LogInformation($"Group {groupName} does not have any members.");
                return;
            }
            WebSocket socket; String[] so;
            GQLMessage msgToSend = (GQLMessage)message;
            foreach (string socketOperation in groupSockets)
            {
                so = socketOperation.Split('~');
                msgToSend.Id = so[1];
                socket = _connectionManager.GetSocketById(so[0]);
                if (socket != null)
                { // socket might have been disconnected / removed and _groups list is out of sync
                    await SendMessageAsync(socket, msgToSend);
                }
            }
        }
    }
}