using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SeaBattle.Domain;
using SeaBattle.Domain.Commands;
using SeaBattle.Web.Models;
using SeaBattle.Web.Services;

namespace SeaBattle.Web.Handlers
{
    public abstract class WebSocketHandler
    {
        public async Task OnConnected(WebSocket webSocket)
        {
            await Task.Yield();
        }

        public async Task OnDisconnected(WebSocket webSocket)
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