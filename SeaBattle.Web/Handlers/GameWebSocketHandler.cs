using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SeaBattle.Web.Events;
using SeaBattle.Web.Managers;
using SeaBattle.Web.Models;

namespace SeaBattle.Web.Handlers
{
    public class GameWebSocketHandler : WebSocketHandler, INotificationHandler<GameEvent>
    {
        private readonly GameConnectionManager _gameConnectionManager;
        private readonly IMediator _mediator;

        public GameWebSocketHandler(GameConnectionManager gameConnectionManager, IMediator mediator)
        {
            _gameConnectionManager = gameConnectionManager;
            _mediator = mediator;
        }

        public override async Task ReceiveAsync(WebSocket webSocket, WebSocketReceiveResult result, byte[] buffer)
        {
            var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
            var parts = message.Split(':');
            var action = parts[0];
            var gameId = Guid.Parse(parts[1]);
            var playerId = Guid.Parse(parts[2]);
            var posX = parts.Length > 4 && int.TryParse(parts[3], out var x) ? x : -1;
            var posY = parts.Length > 4 && int.TryParse(parts[4], out var y) ? y : -1;

            _gameConnectionManager.Set(gameId, playerId, webSocket);

            await _mediator.Publish(new MoveEvent
            {
                Action = action,
                GameId = gameId,
                PlayerId = playerId,
                PosX = posX,
                PosY = posY
            });
        }

        public async Task Handle(GameEvent notification, CancellationToken cancellationToken)
        {
            if (_gameConnectionManager.TryGet(notification.GameId, notification.PlayerId, out var webSocket))
            {
                await SendMessageAsync(webSocket,
                    System.Text.Json.JsonSerializer.Serialize(new GameState
                    {
                        ChangesList = notification.ChangesList,
                        Message = notification.Message
                    }));
            }
        }
    }
}