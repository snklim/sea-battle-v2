using System;
using System.Collections.Generic;

namespace SeaBattle.Domain.Commands
{
    public abstract class AttackCommand
    {
        public Guid AttackerId { get; }

        protected AttackCommand(Guid attackerId)
        {
            AttackerId = attackerId;
        }

        public bool Execute(Player attacker, Player defender, out IReadOnlyCollection<Changes> changesList)
        {
            return ExecuteInternal(attacker, defender, out changesList);
        }

        protected abstract bool ExecuteInternal(Player attacker, Player defender,
            out IReadOnlyCollection<Changes> changesList);
    }
}