using System;
using System.Collections.Generic;
using System.Linq;
using SeaBattle.Domain.Cells;

namespace SeaBattle.Domain.Commands
{
    public class AttackByPositionCommand : AttackPositionCommand
    {
        private readonly int _positionX;
        private readonly int _positionY;

        public AttackByPositionCommand(int positionX, int positionY, Guid attackerFieldId) : base(attackerFieldId)
        {
            _positionX = positionX;
            _positionY = positionY;
        }

        protected override bool ExecuteInternal(Field field, out IEnumerable<Cell> affectedCell)
        {
            return field.Attack(_positionX, _positionY, out affectedCell);
        }
    }
}