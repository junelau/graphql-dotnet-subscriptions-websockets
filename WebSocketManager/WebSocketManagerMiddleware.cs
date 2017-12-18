using System;
using System.IO;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace WebSocketManager
{
    public class WebSocketManagerMiddleware
    {
        private readonly RequestDelegate _next;
        private IWebSocketReceiver _webSocketReceiver { get; set; }

        private readonly ILogger<WebSocketManagerMiddleware> _logger;

        public string _acceptSubProtocol { get; set; }

        public WebSocketManagerMiddleware(RequestDelegate next, ILogger<WebSocketManagerMiddleware> logger,
                                          IWebSocketReceiver webSocketReceiver, string acceptSubProtocol)
        {
            _next = next;
            _webSocketReceiver = webSocketReceiver;
            _logger = logger;
            _acceptSubProtocol = acceptSubProtocol;
        }

        public async Task Invoke(HttpContext context)
        {
            if (!context.WebSockets.IsWebSocketRequest)
                return;

            var socket = await context.WebSockets.AcceptWebSocketAsync(_acceptSubProtocol).ConfigureAwait(false);
            await _webSocketReceiver.OnConnected(socket).ConfigureAwait(false);

            await Receive(socket, async (result, serializedInvocationDescriptor) =>
            {
                if (result.MessageType == WebSocketMessageType.Text)
                {
                    await _webSocketReceiver.ReceiveAsync(socket, result, serializedInvocationDescriptor).ConfigureAwait(false);
                    return;
                }

                else if (result.MessageType == WebSocketMessageType.Close)
                {
                    await _webSocketReceiver.OnDisconnected(socket);
                }
            });
        }

        private async Task Receive(WebSocket socket, Action<WebSocketReceiveResult, string> handleMessage)
        {
            while (socket.State == WebSocketState.Open)
            {
                ArraySegment<Byte> buffer = new ArraySegment<byte>(new Byte[1024 * 4]);
                string serializedInvocationDescriptor = null;
                WebSocketReceiveResult result = null;
                using (var ms = new MemoryStream())
                {
                    do
                    {
                        result = await socket.ReceiveAsync(buffer, CancellationToken.None).ConfigureAwait(false);
                        ms.Write(buffer.Array, buffer.Offset, result.Count);
                    }
                    while (!result.EndOfMessage);

                    ms.Seek(0, SeekOrigin.Begin);

                    using (var reader = new StreamReader(ms, Encoding.UTF8))
                    {
                        serializedInvocationDescriptor = await reader.ReadToEndAsync().ConfigureAwait(false);
                    }
                }
                _logger.LogInformation(JsonConvert.SerializeObject(result));
                handleMessage(result, serializedInvocationDescriptor);
            }
        }
    }
}