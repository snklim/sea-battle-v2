using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using SeaBattle.Domain.Cells;

namespace SeaBattle.Domain.Commands
{
    public abstract class AttackPositionCommand
    {
        private Guid AttackerFieldId { get; }

        protected AttackPositionCommand(Guid attackerFieldId)
        {
            AttackerFieldId = attackerFieldId;
        }

        public bool Execute(Field defenderField, out IEnumerable<Cell> affectedCell)
        {
            if (defenderField.FieldId == AttackerFieldId)
            {
                affectedCell = ImmutableArray<Cell>.Empty;
                return true;
            }

            return ExecuteInternal(defenderField, out affectedCell);
        }

        protected abstract bool ExecuteInternal(Field field, out IEnumerable<Cell> affectedCell);
    }
}