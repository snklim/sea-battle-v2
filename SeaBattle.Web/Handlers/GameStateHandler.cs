using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SeaBattle.Domain;
using SeaBattle.Domain.Commands;
using SeaBattle.Web.Events;
using SeaBattle.Web.Managers;
using SeaBattle.Web.Models;

namespace SeaBattle.Web.Handlers
{
    public class GameStateHandler : INotificationHandler<MoveEvent>
    {
        private readonly GameManager _gameManager;
        private readonly IMediator _mediator;

        public GameStateHandler(GameManager gameManager, IMediator mediator)
        {
            _gameManager = gameManager;
            _mediator = mediator;
        }

        public async Task Handle(MoveEvent notification, CancellationToken cancellationToken)
        {
            await ProcessMessageAsync(notification.GameId, notification.PlayerId, notification.Action, notification.PosX, notification.PosY);
        }
        
        private async Task ProcessMessageAsync(Guid gameId, Guid playerId, string action, int posX = -1, int posY = -1)
        {
            var gameDetails = _gameManager.GetAll().FirstOrDefault(x => x.Game.GameId == gameId);

            if (gameDetails == null)
            {
                return;
            }

            if (action == "start")
            {
                await ProcessStartAsync(gameDetails.Game);
            }

            if (action == "attack")
            {
                var withBot = gameDetails.WithBot;

                await ProcessAttackAsync(gameDetails.Game, new AttackByPositionCommand(posX, posY, playerId));

                if (gameDetails.Game.AttackerChanged && withBot)
                    AttackByBot(gameDetails.Game);
            }

            if (action == "bot_attack")
            {
                await ProcessAttackAsync(gameDetails.Game, new AttackByRandomPositionCommand(playerId));

                if (!gameDetails.Game.AttackerChanged)
                    AttackByBot(gameDetails.Game);
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
                await _mediator.Publish(new GameEvent
                {
                    GameId = game.GameId,
                    PlayerId = changesGroup.Key,
                    ChangesList = changesGroup.ToArray(),
                    Message = game.GameIsOver
                        ? changesGroup.Key == game.Attacker.PlayerId ? "YOU WIN" : "YOU LOSE"
                        : changesGroup.Key == game.Attacker.PlayerId
                            ? "YOUR TURN"
                            : "OPPONENT TURN"
                });
            }
        }
    }
}