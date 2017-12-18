using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GraphQL.Subscriptions.Example
{
    /// <summary>
    /// Messages and payload interface as defined by subscriptions-transport-ws.SubscriptionsClient
    /// </summary>
    public class GQLMessage
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("type")]
        public string Type { get; set; } // of type GQLMessageTypes
        [JsonProperty("payload")]
        public JObject Payload { get; set; }
    }

    public class GQLMessageTypes // see subscription-transport-ws
    {
        public const string CONNECTION_INIT = "connection_init";
        public const string CONNECTION_ACK = "connection_ack";
        public const string GQL_START = "start";
        public const string GQL_DATA = "data";

        // the below are not used - but included for completeness, to reflect subscription-transport-ws definitions

        public const string GQL_CONNECTION_ERROR = "connection_error";
        public const string GQL_CONNECTION_KEEP_ALIVE = "ka";
        public const string GQL_CONNECTION_TERMINATE = "connection_terminate";


        public const string GQL_ERROR = "error";
        public const string GQL_COMPLETE = "complete";
        public const string GQL_STOP = "stop";
        public const string SUBSCRIPTION_START = "subscription_start";
        public const string SUBSCRIPTION_DATA = "subscription_data";
        public const string SUBSCRIPTION_SUCCESS = "subscription_success";
        public const string SUBSCRIPTION_FAIL = "subscription_fail";
        public const string SUBSCRIPTION_END = "subscription_end";
        public const string INIT = "init";
        public const string INIT_SUCCESS = "init_success";
        public const string INIT_FAIL = "init_fail";
        public const string KEEP_ALIVE = "keepalive";
    }

}