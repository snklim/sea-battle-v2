using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using SeaBattle.Domain;
using SeaBattle.Domain.Builders;
using SeaBattle.Domain.Commands;
using SeaBattle.Web.Services;

namespace SeaBattle.Web.HostedServices
{
    public class BackgroundSocketProcessor : BackgroundService
    {
        private readonly IGameService _gameService;

        private static readonly
            BlockingCollection<(string message, BlockingCollection<GameDetails> gameDetailsProvider)> Tasks = new();

        private static readonly Dictionary<Guid, (BlockingCollection<GameDetails> gameDetailsProvider, bool withBot)> Updates = new();

        public BackgroundSocketProcessor(IGameService gameService)
        {
            _gameService = gameService;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            foreach (var task in Tasks.GetConsumingEnumerable())
            {
                var parts = task.message.Split(':');
                var gameDetailsProvider = task.gameDetailsProvider;
                var action = parts[0];
                if (!Guid.TryParse(parts[1], out var gameId)) continue;
                var playerId = Guid.Parse(parts[2]);

                var game = _gameService.GetAll().FirstOrDefault(game => game.GameId == gameId);

                if (game == null) continue;

                if (action == "start")
                {
                    var withBot = parts[3] == "withbot";
                    
                    Updates[playerId] = (gameDetailsProvider, withBot);

                    ProcessStart(game, playerId);
                }

                if (action == "attack")
                {
                    var posX = int.Parse(parts[3]);
                    var posY = int.Parse(parts[4]);
                    var withBot = Updates[playerId].withBot;

                    ProcessAttack(game, new AttackByPositionCommand(posX, posY, playerId));

                    if (game.AttackerChanged && withBot)
                        AttackByBot(game);
                }

                if (action == "bot_attack")
                {
                    ProcessAttack(game, new AttackByRandomPositionCommand(playerId));

                    if (!game.AttackerChanged)
                        AttackByBot(game);
                }
            }

            return Task.FromResult(new object());
        }

        private void AttackByBot(Game game)
        {
            Task.Run(async () =>
            {
                await Task.Delay(500);

                Tasks.Add(($"bot_attack:{game.GameId}:{game.Attacker.PlayerId}", Updates[game.Defender.PlayerId].gameDetailsProvider));
            });
        }

        private void ProcessStart(Game game, Guid playerId)
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
                    : playerId == game.Attacker.PlayerId ? "YOUR TURN" : "OPPONENT TURN"
            };

            Updates[playerId].gameDetailsProvider.Add(gameDetails);
        }

        private void ProcessAttack(Game game, AttackPositionCommand command)
        {
            var changesList = game.Next(command);

            changesList.ToList().ForEach(changes =>
            {
                if (Updates.TryGetValue(changes.PlayerId, out var provider))
                {
                    provider.gameDetailsProvider.Add(new GameDetails
                    {
                        ChangesList = new[] {changes},
                        Message = game.GameIsOver 
                                ? changes.PlayerId == game.Attacker.PlayerId ? "YOU WIN" : "YOU LOSE"
                                : changes.PlayerId == game.Attacker.PlayerId ? "YOUR TURN" : "OPPONENT TURN"
                    });
                }
            });
        }

        public static void AddSocket(WebSocket webSocket, TaskCompletionSource<object> tcs)
        {
            var gameDetailsProvider = new BlockingCollection<GameDetails>();

            Task.Run(async () =>
            {
                foreach (var gameDetails in gameDetailsProvider.GetConsumingEnumerable())
                {
                    var bytes = System.Text.Encoding.UTF8.GetBytes(
                        System.Text.Json.JsonSerializer.Serialize(gameDetails));

                    await webSocket.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true,
                        CancellationToken.None);
                }
            });

            Task.Run(async () =>
            {
                var buffer = new byte[1024 * 4];
                var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                while (!result.CloseStatus.HasValue)
                {
                    var message = System.Text.Encoding.UTF8.GetString(buffer, 0, result.Count);

                    Tasks.Add((message, gameDetailsProvider));

                    result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                }

                await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription,
                    CancellationToken.None);
            });
        }
    }

    public class GameDetails
    {
        public Changes[] ChangesList { get; set; }
        public string Message { get; set; }
    }
}