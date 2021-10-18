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

        public AttackByPositionCommand(int positionX, int positionY, Guid attackerId) : base(attackerId)
        {
            _positionX = positionX;
            _positionY = positionY;
        }

        protected override bool ExecuteInternal(Player attacker, Player defender,
            out IReadOnlyCollection<Changes> changesList)
        {
            if (!attacker.AvailablePositions.Any(cell => cell.x == _positionX && cell.y == _positionY))
            {
                changesList = Array.Empty<Changes>();
                return true;
            }

            return attacker.Attack(defender, _positionX, _positionY, out changesList);
        }
    }
}