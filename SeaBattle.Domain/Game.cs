using System;
using System.Collections.Generic;
using System.Linq;
using SeaBattle.Domain.Cells;
using SeaBattle.Domain.Commands;
using SeaBattle.Domain.Enums;

namespace SeaBattle.Domain
{
    public class Game
    {
        public Guid GameId { get; } = Guid.NewGuid();
        public Player Attacker { get; set; }
        public Player Defender { get; set; }
        public bool AttackerChanged { get; private set; }

        public IReadOnlyCollection<Changes> Next(AttackCommand command)
        {
            if (command.AttackerId != Attacker.PlayerId || GameIsOver)
            {
                return Array.Empty<Changes>();
            }

            AttackerChanged = !command.Execute(Attacker, Defender, out var changesList);

            if (GameIsOver)
            {
                var changes = new Changes
                {
                    PlayerId = Defender.PlayerId,
                    FieldId = Defender.EnemyField.FieldId,
                    AffectedCells = Defender.AvailablePositions
                        .Select(pos => Attacker.OwnField[pos.X, pos.Y].ToCellDto())
                        .Where(cell => cell.CellType == CellType.Ship)
                        .ToArray()
                };

                var changesListList = changesList.ToList();
                changesListList.Add(changes);

                changesList = changesListList;
            }

            if (AttackerChanged)
            {
                (Attacker, Defender) = (Defender, Attacker);
            }

            return changesList;
        }

        public bool GameIsOver => Attacker.OwnField.AllShipsDestroyed || Defender.OwnField.AllShipsDestroyed;
    }
}