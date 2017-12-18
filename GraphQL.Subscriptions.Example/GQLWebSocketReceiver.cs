using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GraphQL;
using GraphQL.Subscriptions.Example;
using GraphQL.Subscription;
using GraphQL.Types;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using WebSocketManager;

namespace GraphQL.Subscriptions.Example
{


    /// <summary>
    /// Implementation of GQL interfaces for sending and receiving ws messages.
    ///</summary>
    public class GQLWebSocketReceiver : IWebSocketReceiver
    {
        private readonly IWebSocketWriter _gqlSender;
        private readonly ISchema _schema;
        private readonly ISubscriptionExecuter _subscriptionExecuter;

        public GQLWebSocketReceiver(WebSocketConnectionManager webSocketConnectionManager, ILogger<IWebSocketReceiver> logger,
        ISchema schema, ISubscriptionExecuter subscriptionExecuter,
        IWebSocketWriter websocketWriter)
        : base(webSocketConnectionManager, logger)
        {
            _gqlSender = websocketWriter;
            _schema = schema;
            _subscriptionExecuter = subscriptionExecuter;
        }

        public override Task ReceiveAsync(WebSocket socket, WebSocketReceiveResult result, string serialisedWSPayload)
        {
            _logger.LogInformation("Received payload: " + serialisedWSPayload);
            var received = JsonConvert.DeserializeObject<GQLMessage>(serialisedWSPayload);

            switch (received.Type)
            {
                case GQLMessageTypes.CONNECTION_INIT:
                    _logger.LogInformation("Connection initialisation received. Acknowledge client.");
                    // acknowledge successful connection with client
                    return _gqlSender.SendMessageAsync(socket, new GQLMessage
                    {
                        Id = _connectionManager.GetId(socket),
                        Type = GQLMessageTypes.CONNECTION_ACK
                    });

                case GQLMessageTypes.GQL_START:
                    _logger.LogInformation("received subscription request. adding to group.");
                    var gql = new GraphQLDocument
                    {
                        Query = received.Payload.SelectToken("$..query").ToString(),
                        Variables = received.Payload.SelectToken("$..variables").ToObject<JObject>()
                    };
                    // include socket context. can also include with usercontext if using authentication
                    object context = new
                    {
                        socketId = _connectionManager.GetId(socket),
                        opId = received.Id
                    };
                    return this.Subscribe(gql, context);

                case GQLMessageTypes.GQL_STOP:
                    var id = _connectionManager.GetId(socket);
                    _logger.LogInformation($"stopping subscriptions for connection {id}");
                    return _connectionManager.RemoveSocket(id);
                default:
                    _logger.LogError("Unknown GQL Message Type received: " + received.Type);
                    return Task.CompletedTask;
            }
        }

        public async Task<SubscriptionExecutionResult> Subscribe(GraphQLDocument bdy, object context)
        {
            var executionOptions = new ExecutionOptions
            {
                Schema = _schema,
                Query = bdy.Query,
                OperationName = bdy.OperationName,
                Inputs = bdy.Variables.ToInputs(),
                ExposeExceptions = false,
                UserContext = context
            };

            // call ChatSubscription's subscriber
            var result = await _subscriptionExecuter.SubscribeAsync(executionOptions);
            return result;
        }
    }
}
