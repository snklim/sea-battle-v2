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
            var gameDetails = _gameManager.GetAll(gameId).FirstOrDefault(x => x.Game.GameId == gameId);

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

                await ProcessMessageAsync(game.GameId, game.AttackerId, "bot_attack");
            });
        }

        private async Task ProcessStartAsync(Game game)
        {
            var changesList = new[]
            {
                new Changes
                {
                    PlayerId = game.FirstPlayer.PlayerId,
                    FieldId = game.FirstPlayer.OwnField.FieldId,
                    AffectedCells = game.FirstPlayer.OwnField.GetCells().ToArray()
                },
                new Changes
                {
                    PlayerId = game.FirstPlayer.PlayerId,
                    FieldId = game.FirstPlayer.EnemyField.FieldId,
                    AffectedCells = game.FirstPlayer.EnemyField.GetCells().ToArray()
                },
                new Changes
                {
                    PlayerId = game.SecondPlayer.PlayerId,
                    FieldId = game.SecondPlayer.OwnField.FieldId,
                    AffectedCells = game.SecondPlayer.OwnField.GetCells().ToArray()
                },
                new Changes
                {
                    PlayerId = game.SecondPlayer.PlayerId,
                    FieldId = game.SecondPlayer.EnemyField.FieldId,
                    AffectedCells = game.SecondPlayer.EnemyField.GetCells().ToArray()
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
            await _gameManager.Update(game);
            foreach (var changesGroup in changesList.GroupBy(x => x.PlayerId))
            {
                var changes = changesGroup.ToArray();
                await _gameManager.UpdateCells(changes);
                await _mediator.Publish(new GameEvent
                {
                    GameId = game.GameId,
                    PlayerId = changesGroup.Key,
                    ChangesList = changes,
                    Message = game.GameIsOver
                        ? changesGroup.Key == game.AttackerId ? "YOU WIN" : "YOU LOSE"
                        : changesGroup.Key == game.AttackerId
                            ? "YOUR TURN"
                            : "OPPONENT TURN"
                });
            }
        }
    }
}