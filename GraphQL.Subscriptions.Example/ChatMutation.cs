using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using GraphQL.Types;
using WebSocketManager;
using Newtonsoft.Json.Linq;

namespace GraphQL.Subscriptions.Example
{
    public class ChatMutation : ObjectGraphType<object>
    {
        public ChatMutation(IWebSocketWriter websocketWriter)
        {
            Name = "Mutation";

            Field<MessageType>(
                name: "addMessage",
                arguments: new QueryArguments {
                    new QueryArgument<MessageInputType> {
                        Name = "message"
                    },
                    new QueryArgument<StringGraphType> {
                        Name = "group",
                        Description = "If no group name is given, message is sent to all."
                    }
                },
                resolve: x =>
                {
                    var newMsg = x.GetArgument<Message>("message");
                    var grpName = x.GetArgument<String>("group");
                    newMsg.GroupName = grpName;
                    websocketWriter.SendToGroup(grpName, new GQLMessage
                    {
                        Type = GQLMessageTypes.GQL_DATA,
                        //wrap message in a data element to match what subscription-transport-ws.SubscriptionClient is expecting
                        Payload = JObject.FromObject(new { data = newMsg })
                    }).Wait();
                    return newMsg;
                }
            );
        }
    }
}
