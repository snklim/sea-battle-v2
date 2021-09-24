using System;
using System.Collections.Generic;
using System.Linq;

namespace SeaBattle.Domain.Commands
{
    public abstract class AttackPositionCommand
    {
        public Guid FieldId { get; }
        
        protected AttackPositionCommand(Guid fieldId)
        {
            FieldId = fieldId;
        }
        
        public abstract bool Execute(Field field, out IEnumerable<Cell> affectedCell);

        protected bool AttackInternal(Field field, int positionX, int positionY,
            out IEnumerable<Cell> affectedCells)
        {
            var hit = field.Attack(positionX, positionY, out var affectedCell, out affectedCells);

            affectedCells.Select(cell => (x: cell.X, y: cell.Y))
                .ToList()
                .ForEach(pos =>
                {
                    field.NextPositions.Remove(pos);
                });

            if (hit)
            {
                if (affectedCell.IsShipDestroyed)
                {
                    field.PreviousHits.Clear();
                    field.NextPositions.Clear();
                }
                else
                {
                    var nextPositions = field.AvailablePositions
                        .Intersect(new[]
                        {
                            (x: positionX - 1, y: positionY),
                            (x: positionX, y: positionY + 1),
                            (x: positionX + 1, y: positionY),
                            (x: positionX, y: positionY - 1)
                        });
                    
                    field.PreviousHits.Add((x: positionX, y: positionY));
                    
                    field.NextPositions.AddRange(nextPositions);

                    if (field.PreviousHits.Count >= 2)
                    {
                        var positionPairs = field.PreviousHits
                            .Join(field.PreviousHits, pos => true, pos => true,
                                (pos1, pos2) => (pos1, pos2))
                            .ToList();
                        var filteredNextPositions = positionPairs.All(x => x.pos1.x == x.pos2.x)
                            ? field.NextPositions.Where(pos => pos.x == positionPairs.First().pos1.x).ToList()
                            : field.NextPositions.Where(pos => pos.y == positionPairs.First().pos1.y).ToList();

                        field.NextPositions.Clear();
                        field.NextPositions.AddRange(filteredNextPositions);
                    }
                }
            }

            return hit;
        }
    }
}