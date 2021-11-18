using System;
using System.Collections.Generic;
using System.Linq;
using SeaBattle.Domain.Enums;

namespace SeaBattle.Domain
{
    public class Player
    {
        public Guid PlayerId { get; set; } = Guid.NewGuid();

        public Field OwnField { get; set; }
        public Field EnemyField { get; set; }

        public List<Position> NextPositions { get; } = new();
        public List<Position> PreviousHits { get; } = new();

        public bool Attack(Player defender, int posX, int posY, out IReadOnlyCollection<Changes> changesList)
        {
            var hit = defender.Defend(posX, posY, out var affectedCell, out var enemyChanges);

            var enemyAffectedCellList = new List<Cell>();

            enemyChanges.AffectedCells.ToList().ForEach(cell =>
            {
                var pos = new Position(cell.X, cell.Y);
                NextPositions.Remove(pos);
                PreviousHits.Remove(pos);
                var enemyAffectedCell = EnemyField[cell.X, cell.Y];
                enemyAffectedCell.Attacked = true;
                enemyAffectedCell.CellType = cell.CellType;
                enemyAffectedCellList.Add(enemyAffectedCell);
            });

            if (hit && !affectedCell.IsShipDestroyed)
            {
                GenerateNextPositions(posX, posY);
            }

            changesList = new[]
            {
                enemyChanges, new Changes
                {
                    PlayerId = PlayerId,
                    FieldId = EnemyField.FieldId,
                    AffectedCells = enemyAffectedCellList.Select(cell => cell).ToArray()
                }
            };

            return hit;
        }

        private void GenerateNextPositions(int positionX, int positionY)
        {
            var nextPositions = AvailablePositions.Intersect(new[]
            {
                new Position(positionX - 1, positionY),
                new Position(positionX, positionY + 1),
                new Position(positionX + 1, positionY),
                new Position(positionX, positionY - 1)
            });

            PreviousHits.Add(new Position(positionX, positionY));

            NextPositions.AddRange(nextPositions);

            if (PreviousHits.Count >= 2)
            {
                var positionPairs = PreviousHits
                    .Join(PreviousHits, _ => true, _ => true,
                        (pos1, pos2) => (pos1, pos2))
                    .ToList();

                var filteredNextPositions = positionPairs.All(x => x.pos1.X == x.pos2.X)
                    ? NextPositions.Where(pos => pos.X == positionPairs.First().pos1.X).ToList()
                    : NextPositions.Where(pos => pos.Y == positionPairs.First().pos1.Y).ToList();

                NextPositions.Clear();
                NextPositions.AddRange(filteredNextPositions);
            }
        }

        private bool Defend(int posX, int posY, out Cell affectedCell, out Changes changes)
        {
            changes = new Changes
            {
                PlayerId = PlayerId,
                FieldId = OwnField.FieldId,
                AffectedCells = OwnField.Attack(posX, posY, out affectedCell).ToArray()
            };

            return affectedCell.CellType == CellType.Ship;
        }

        public List<Position> AvailablePositions => EnemyField.GetCells()
            .Where(cell => !cell.Attacked)
            .Select(cell => new Position(cell.X, cell.Y))
            .ToList();
    }
}