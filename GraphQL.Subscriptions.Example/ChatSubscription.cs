using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using GraphQL.Types;
using GraphQL.Subscription;
using GraphQL.Resolvers;
using WebSocketManager;
using Newtonsoft.Json.Linq;

namespace GraphQL.Subscriptions.Example
{
    public class ChatSubscription : ObjectGraphType<object>
    {

        public ChatSubscription(WebSocketConnectionManager connectionManager)
        {
            Name = "subscription";
            AddField(new EventStreamFieldType
            {
                Name = "joinChat",
                Type = typeof(MessageType),
                Arguments = new QueryArguments {
                    new QueryArgument<StringGraphType>{Name = "groupName"}
                },
                Resolver = new FuncFieldResolver<Message>(x =>
                {
                    var message = x.Source as Message;
                    return message;
                }),
                Subscriber = new EventStreamResolver<Message>(x =>
                {
                    var grp = x.GetArgument<string>("groupName");
                    if (string.IsNullOrWhiteSpace(grp))
                    {
                        grp = "all";
                    }
                    var context = JObject.FromObject(x.UserContext);
                    string socketId = context.SelectToken("$..socketId").ToString();
                    string opId = context.SelectToken("$..opId").ToString();
                    connectionManager.AddToGroup(socketId, opId, grp);
                    return null;
                })
            });
        }
    }
}
