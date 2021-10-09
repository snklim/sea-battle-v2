using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using SeaBattle.Domain.Cells;

namespace SeaBattle.Domain.Commands
{
    public abstract class AttackPositionCommand
    {
        public Guid AttackerId { get; }

        protected AttackPositionCommand(Guid attackerId)
        {
            AttackerId = attackerId;
        }

        public bool Execute(Player attacker, Player defender, out IReadOnlyCollection<Changes> changesList)
        {
            if (attacker.PlayerId != AttackerId)
            {
                changesList = Array.Empty<Changes>();
                return true;
            }

            return ExecuteInternal(attacker, defender, out changesList);
        }

        protected abstract bool ExecuteInternal(Player attacker, Player defender, out IReadOnlyCollection<Changes> changesList);
    }
}