using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using SeaBattle.Web.Events;
using SeaBattle.Web.Managers;

namespace SeaBattle.Web.Handlers
{
    public class InfoWebSocketHandler : WebSocketHandler, INotificationHandler<InfoEvent>
    {
        private readonly InfoConnectionManager _connectionManager;

        public InfoWebSocketHandler(InfoConnectionManager connectionManager)
        {
            _connectionManager = connectionManager;
        }
        
        public override async Task OnConnected(HttpContext context, WebSocket webSocket)
        {
            if (context.User.Identity.IsAuthenticated)
            {
                _connectionManager.Add(context.User.Identity.Name, webSocket);
            }
            await Task.Yield();
        }

        public override async Task OnDisconnected(HttpContext context, WebSocket webSocket)
        {
            if (context.User.Identity.IsAuthenticated)
            {
                _connectionManager.Remove(context.User.Identity.Name);
            }
            await Task.Yield();
        }

        public override async Task ReceiveAsync(WebSocket webSocket, WebSocketReceiveResult result, byte[] buffer)
        {
            await Task.Yield();
        }

        public async Task Handle(InfoEvent notification, CancellationToken cancellationToken)
        {
            if (_connectionManager.TryGet(notification.ToUser, out var webSocket))
            {
                await SendMessageAsync(webSocket, notification.Message);
            }
        }
    }
}