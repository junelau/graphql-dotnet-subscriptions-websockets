using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using GraphQL.Types;

namespace GraphQL.Subscriptions.Example
{
    public class ChatSchema : Schema
    {
        public ChatSchema(IDependencyResolver resolver) : base(resolver)
        {
            Query = resolver.Resolve<ChatQuery>();
            Mutation = resolver.Resolve<ChatMutation>();
            Subscription = resolver.Resolve<ChatSubscription>();

        }
    }
}
