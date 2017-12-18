using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using GraphQL.Types;

namespace GraphQL.Subscriptions.Example
{
    public class ChatQuery : ObjectGraphType<object>
    {
        public ChatQuery()
        {
            Name = "Query";

            Field<ListGraphType<MessageType>>(
                name: "messages",
                arguments: new QueryArguments {
                    new QueryArgument<DateGraphType> { Name = "dateCursor" },
                    new QueryArgument<IntGraphType> { Name = "pageSize"}
                },
                resolve: x =>
                {
                    var dateCursor = x.GetArgument<DateTime?>("dateCursor");
                    var pageSize = x.GetArgument<int>("pageSize") == 0 ? 100 : x.GetArgument<int>("pageSize");
                    if (dateCursor != null)
                    {
                        return new List<Message>();
                        //return hubService.allMessages.Select(a => a.CreatedAt < dateCursor).Take(pageSize);
                    }
                    return new List<Message>();
                    //return hubService.allMessages;
                }
            );
        }
    }
}
