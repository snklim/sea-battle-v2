using System;
using System.Collections.Generic;
using System.Linq;
using SeaBattle.Domain.Cells;

namespace SeaBattle.Domain.Commands
{
    public class AttackByRandomPositionCommand : AttackPositionCommand
    {
        private static readonly Random Rnd = new Random();

        public AttackByRandomPositionCommand(Guid fieldId) : base(fieldId)
        {

        }

        public override bool Execute(Field field, out IEnumerable<Cell> affectedCell)
        {   
            if (field.NextPositions.Any())
            {
                var position = field.NextPositions[Rnd.Next(field.NextPositions.Count)];
                return AttackInternal(field, position.x, position.y, out affectedCell);
            }

            var availablePositions = field.AvailablePositions;
            if (availablePositions.Any())
            {
                var position = availablePositions[Rnd.Next(availablePositions.Count)];
                return AttackInternal(field, position.x, position.y, out affectedCell);
            }

            affectedCell = Enumerable.Empty<Cell>();

            return false;
        }
    }
}