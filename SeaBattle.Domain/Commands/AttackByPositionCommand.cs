using System;
using System.Collections.Generic;
using System.Linq;

namespace SeaBattle.Domain.Commands
{
    public class AttackByPositionCommand : AttackPositionCommand
    {
        private readonly int _positionX;
        private readonly int _positionY;

        public AttackByPositionCommand(int positionX, int positionY, Guid fieldId) : base(fieldId)
        {
            _positionX = positionX;
            _positionY = positionY;
        }

        public override bool Execute(Field field, out IEnumerable<Cell> affectedCell)
        {
            return AttackInternal(field, _positionX, _positionY, out affectedCell);
        }
    }
}