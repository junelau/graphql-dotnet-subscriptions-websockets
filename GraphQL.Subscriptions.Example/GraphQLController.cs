using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using GraphQL;
using GraphQL.Types;
using GraphQL.Subscription;
using System;
using Microsoft.AspNetCore.Http;
using GraphQL.Subscriptions.Example;
using Microsoft.Extensions.Logging;
using System.Net.WebSockets;
using WebSocketManager;

namespace GraphQL.Subscriptions.Example
{
    public class GraphQLController : Controller
    {
        private IDocumentExecuter _docExecuter { get; set; }
        private ISubscriptionExecuter _subscriptionExecuter { get; set; }
        private ISchema _schema { get; set; }
        private HttpContext _httpContext { get; set; }
        private ILogger<GraphQLController> _logger { get; set; }

        public GraphQLController(ISchema schema, IDocumentExecuter documentExecuter, ILogger<GraphQLController> logger)
        {
            _schema = schema;
            _docExecuter = documentExecuter;
            _logger = logger;
        }

        // POST
        [HttpPost]
        public async Task<IActionResult> Index([FromBody] GraphQLDocument bdy)
        {

            var executionOptions = new ExecutionOptions
            {
                Schema = _schema,
                Query = bdy.Query,
                OperationName = bdy.OperationName,
                Inputs = bdy.Variables.ToInputs(),
                ExposeExceptions = false
            };

            ExecutionResult result = await _docExecuter
                    .ExecuteAsync(executionOptions)
                    .ConfigureAwait(false);

            if (result.Errors?.Count > 0)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

    }

}
