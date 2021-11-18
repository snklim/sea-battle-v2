using System;
using System.Collections.Generic;
using System.Linq;
using SeaBattle.Domain.Commands;
using SeaBattle.Domain.Enums;

namespace SeaBattle.Domain
{
    public class Game
    {
        public Guid GameId { get; set; } = Guid.NewGuid();
        public Player FirstPlayer { get; init; }
        public Player SecondPlayer { get; init; }
        public bool AttackerChanged { get; private set; }
        public Guid AttackerId { get; set; }
        public Guid DefenderId { get; set; }

        public IReadOnlyCollection<Changes> Next(AttackCommand command)
        {
            var (attacker, defender) = FirstPlayer.PlayerId == AttackerId
                ? (FirstPlayer, SecondPlayer)
                : (SecondPlayer, FirstPlayer);
            
            if (command.AttackerId != attacker.PlayerId || GameIsOver)
            {
                return Array.Empty<Changes>();
            }

            AttackerChanged = !command.Execute(attacker, defender, out var changesList);

            if (GameIsOver)
            {
                var changes = new Changes
                {
                    PlayerId = defender.PlayerId,
                    FieldId = defender.EnemyField.FieldId,
                    AffectedCells = defender.AvailablePositions
                        .Select(pos => attacker.OwnField[pos.X, pos.Y])
                        .Where(cell => cell.CellType == CellType.Ship)
                        .ToArray()
                };

                var changesListList = changesList.ToList();
                changesListList.Add(changes);

                changesList = changesListList;
            }

            if (AttackerChanged)
            {
                (AttackerId, DefenderId) = (DefenderId, AttackerId);
            }

            return changesList;
        }

        public bool GameIsOver => FirstPlayer.OwnField.AllShipsDestroyed || SecondPlayer.OwnField.AllShipsDestroyed;
    }
}