using System;
using System.Collections.Generic;
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
            var posX = parts.Length > 4 && int.TryParse(parts[3], out var x) ? x : -1;
            var posY = parts.Length > 4 && int.TryParse(parts[4], out var y) ? y : -1;

            _connectionManager.Set(gameId, playerId, webSocket);

            await ProcessMessageAsync(gameId, playerId, action, posX, posY);
        }

        private async Task ProcessMessageAsync(Guid gameId, Guid playerId, string action, int posX = -1, int posY = -1)
        {
            var gameDetails = _gameService.GetAll().FirstOrDefault(x => x.game.GameId == gameId);

            if (action == "start")
            {
                await ProcessStartAsync(gameDetails.game);
            }

            if (action == "attack")
            {
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

                await ProcessMessageAsync(game.GameId, game.Attacker.PlayerId, "bot_attack");
            });
        }

        private async Task ProcessStartAsync(Game game)
        {
            var changesList = new[]
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
            };

            await UpdateConsumersAsync(game, changesList);
        }

        private async Task ProcessAttackAsync(Game game, AttackCommand command)
        {
            var changesList = game.Next(command);

            await UpdateConsumersAsync(game, changesList);
        }

        private async Task UpdateConsumersAsync(Game game, IReadOnlyCollection<Changes> changesList)
        {
            foreach (var changesGroup in changesList.GroupBy(x => x.PlayerId))
            {
                if (_connectionManager.TryGet(game.GameId, changesGroup.Key, out var webSocket))
                {
                    await SendMessageAsync(webSocket,
                        System.Text.Json.JsonSerializer.Serialize(new GameDetails
                        {
                            ChangesList = changesGroup.ToArray(),
                            Message = game.GameIsOver
                                ? changesGroup.Key == game.Attacker.PlayerId ? "YOU WIN" : "YOU LOSE"
                                : changesGroup.Key == game.Attacker.PlayerId
                                    ? "YOUR TURN"
                                    : "OPPONENT TURN"
                        }));
                }
            }
        }
    }
}