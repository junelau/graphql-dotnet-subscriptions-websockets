using System;
using GraphQL.Types;

namespace GraphQL.Subscriptions.Example
{
    public class Message
    {
        public string From { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Content { get; set; }
        public string GroupName { get; set; }
        public Message()
        {
            CreatedAt = DateTime.UtcNow;
        }
    }

    public class MessageType : ObjectGraphType<Message>
    {
        public MessageType()
        {
            Field(x => x.From);
            Field(x => x.CreatedAt);
            Field(x => x.Content);
            Field(x => x.GroupName, nullable: true);
        }
    }

    public class MessageInputType : InputObjectGraphType
    {
        public MessageInputType()
        {
            Field<StringGraphType>("from");
            Field<StringGraphType>("content");
        }
    }
}