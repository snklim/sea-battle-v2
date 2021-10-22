using System;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using SeaBattle.Domain;
using SeaBattle.Domain.Commands;
using SeaBattle.Web.Models;
using SeaBattle.Web.Services;

namespace SeaBattle.Web.Handlers
{
    public class GameWebSocketHandler : WebSocketHandler
    {
        private readonly ConnectionManager _connectionManager;
        private readonly GameService _gameService;

        public GameWebSocketHandler(ConnectionManager connectionManager, GameService gameService)
        {
            _connectionManager = connectionManager;
            _gameService = gameService;
        }

        public override async Task ReceiveAsync(WebSocket webSocket, WebSocketReceiveResult result, byte[] buffer)
        {
            var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
            var parts = message.Split(':');
            var action = parts[0];
            var gameId = Guid.Parse(parts[1]);
            var playerId = Guid.Parse(parts[2]);

            _connectionManager.Set(gameId, playerId, webSocket);

            await ProcessMessageAsync(gameId, playerId, action, parts);
        }

        private async Task ProcessMessageAsync(Guid gameId, Guid playerId, string action, string[] parts)
        {
            var gameDetails = _gameService.GetAll().FirstOrDefault(x => x.game.GameId == gameId);

            if (action == "start")
            {
                await ProcessStartAsync(gameDetails.game, playerId);
            }

            if (action == "attack")
            {
                var posX = int.Parse(parts[3]);
                var posY = int.Parse(parts[4]);
                var withBot = gameDetails.wtihBot;

                await ProcessAttackAsync(gameDetails.game, new AttackByPositionCommand(posX, posY, playerId));

                if (gameDetails.game.AttackerChanged && withBot)
                    AttackByBot(gameDetails.game);
            }

            if (action == "bot_attack")
            {
                await ProcessAttackAsync(gameDetails.game, new AttackByRandomPositionCommand(playerId));

                if (!gameDetails.game.AttackerChanged)
                    AttackByBot(gameDetails.game);
            }
        }

        private void AttackByBot(Game game)
        {
            Task.Run(async () =>
            {
                await Task.Delay(500);

                await ProcessMessageAsync(game.GameId, game.Attacker.PlayerId, "bot_attack", Array.Empty<string>());
            });
        }

        private async Task ProcessStartAsync(Game game, Guid playerId)
        {
            var gameDetails = new GameDetails
            {
                ChangesList = new[]
                {
                    new Changes
                    {
                        PlayerId = game.Attacker.PlayerId,
                        FieldId = game.Attacker.OwnField.FieldId,
                        AffectedCells = game.Attacker.OwnField.GetCells().ToArray()
                    },
                    new Changes
                    {
                        PlayerId = game.Attacker.PlayerId,
                        FieldId = game.Attacker.EnemyField.FieldId,
                        AffectedCells = game.Attacker.EnemyField.GetCells().ToArray()
                    },
                    new Changes
                    {
                        PlayerId = game.Defender.PlayerId,
                        FieldId = game.Defender.OwnField.FieldId,
                        AffectedCells = game.Defender.OwnField.GetCells().ToArray()
                    },
                    new Changes
                    {
                        PlayerId = game.Defender.PlayerId,
                        FieldId = game.Defender.EnemyField.FieldId,
                        AffectedCells = game.Defender.EnemyField.GetCells().ToArray()
                    }
                }.Where(changes => changes.PlayerId == playerId).ToArray(),
                Message = game.GameIsOver
                    ? game.Attacker.PlayerId == playerId ? "YOU WIN" : "YOU LOSE"
                    : playerId == game.Attacker.PlayerId
                        ? "YOUR TURN"
                        : "OPPONENT TURN"
            };

            if (_connectionManager.TryGet(game.GameId, playerId, out var webSocket))
            {
                await SendMessageAsync(webSocket,
                    System.Text.Json.JsonSerializer.Serialize(gameDetails));
            }
        }

        private async Task ProcessAttackAsync(Game game, AttackCommand command)
        {
            var changesList = game.Next(command);

            foreach (var changes in changesList.ToList())
            {

                if (_connectionManager.TryGet(game.GameId, changes.PlayerId, out var webSocket))
                {
                    await SendMessageAsync(webSocket,
                        System.Text.Json.JsonSerializer.Serialize(new GameDetails
                        {
                            ChangesList = new[] {changes},
                            Message = game.GameIsOver
                                ? changes.PlayerId == game.Attacker.PlayerId ? "YOU WIN" : "YOU LOSE"
                                : changes.PlayerId == game.Attacker.PlayerId
                                    ? "YOUR TURN"
                                    : "OPPONENT TURN"
                        }));
                }
            }
        }
    }
}