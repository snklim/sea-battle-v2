using System;
using System.Collections.Generic;
using System.Linq;
using SeaBattle.Domain.Cells;
using SeaBattle.Domain.Enums;

namespace SeaBattle.Domain
{
    public class Player
    {
        public Guid PlayerId { get; } = Guid.NewGuid();
        public Field OwnField { get; init; }
        public Field EnemyField { get; init; }
        
        public List<(int x, int y)> NextPositions { get; } = new();
        public List<(int x, int y)> PreviousHits { get; } = new();

        public bool Attack(Player defender, int posX, int posY, out IEnumerable<Cell> affectedCells)
        {
            var hit = defender.Defend(posX, posY, out var affectedCell, out var affectedCellList);

            var enemyAffectedCellList = new List<Cell>();

            affectedCellList
                .ForEach(cell =>
                {
                    var pos = (x: cell.X, y: cell.Y);
                    NextPositions.Remove(pos);
                    PreviousHits.Remove(pos);
                    var enemyAffectedCell = cell.CellType == CellType.Ship
                        ? new ShipCell(cell.X, cell.Y, EnemyField.FieldId, new ShipDetails())
                        : cell.CellType == CellType.Border
                            ? new BorderCell(cell.X, cell.Y, EnemyField.FieldId)
                            : EnemyField[cell.X, cell.Y];
                    enemyAffectedCell.Attacked = true;
                    EnemyField[cell.X, cell.Y] = enemyAffectedCell;
                    enemyAffectedCellList.Add(enemyAffectedCell);
                });

            if (hit && !affectedCell.IsShipDestroyed)
            {
                GenerateNextPositions(posX, posY);
            }

            affectedCellList.AddRange(enemyAffectedCellList);

            affectedCells = affectedCellList.ToArray();

            return hit;
        }
        
        private void GenerateNextPositions(int positionX, int positionY)
        {
            var nextPositions = AvailablePositions
                .Intersect(new[]
                {
                    (x: positionX - 1, y: positionY),
                    (x: positionX, y: positionY + 1),
                    (x: positionX + 1, y: positionY),
                    (x: positionX, y: positionY - 1)
                });
                    
            PreviousHits.Add((x: positionX, y: positionY));
                    
            NextPositions.AddRange(nextPositions);

            if (PreviousHits.Count >= 2)
            {
                var positionPairs = PreviousHits
                    .Join(PreviousHits, pos => true, pos => true,
                        (pos1, pos2) => (pos1, pos2))
                    .ToList();
                var filteredNextPositions = positionPairs.All(x => x.pos1.x == x.pos2.x)
                    ? NextPositions.Where(pos => pos.x == positionPairs.First().pos1.x).ToList()
                    : NextPositions.Where(pos => pos.y == positionPairs.First().pos1.y).ToList();

                NextPositions.Clear();
                NextPositions.AddRange(filteredNextPositions);
            }
        }

        private bool Defend(int posX, int posY, out Cell affectedCell, out List<Cell> affectedCells)
        {
            affectedCell = OwnField[posX, posY];
            affectedCells = affectedCell.Attack().ToList();
            return affectedCell.CellType == CellType.Ship;
        }
        
        public List<(int x, int y)> AvailablePositions => EnemyField.GetCells()
            .Where(cell => !cell.Attacked)
            .Select(cell => (x: cell.X, y: cell.Y))
            .ToList();
    }
}