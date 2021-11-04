using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace SeaBattle.Web.Handlers
{
    public abstract class WebSocketHandler
    {
        public virtual async Task OnConnected(HttpContext context, WebSocket webSocket)
        {
            await Task.Yield();
        }

        public virtual async Task OnDisconnected(HttpContext context, WebSocket webSocket)
        {
            await Task.Yield();
        }

        protected async Task SendMessageAsync(WebSocket socket, string message)
        {
            if (socket.State != WebSocketState.Open)
                return;

            await socket.SendAsync(buffer: new ArraySegment<byte>(array: Encoding.UTF8.GetBytes(message),
                    offset: 0,
                    count: message.Length),
                messageType: WebSocketMessageType.Text,
                endOfMessage: true,
                cancellationToken: CancellationToken.None);
        }

        public abstract Task ReceiveAsync(WebSocket webSocket, WebSocketReceiveResult result, byte[] buffer);
    }
}