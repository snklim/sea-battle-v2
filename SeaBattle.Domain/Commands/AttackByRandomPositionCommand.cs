using System;
using System.Collections.Generic;
using System.Linq;

namespace SeaBattle.Domain.Commands
{
    public class AttackByRandomPositionCommand : AttackCommand
    {
        private static readonly Random Rnd = new ();

        public AttackByRandomPositionCommand(Guid attackerId) : base(attackerId)
        {

        }

        protected override bool ExecuteInternal(Player attacker, Player defender, out IReadOnlyCollection<Changes> changesList)
        {   
            if (attacker.NextPositions.Any())
            {
                var position = attacker.NextPositions[Rnd.Next(attacker.NextPositions.Count)];
                return attacker.Attack(defender, position.X, position.Y, out changesList);
            }

            var availablePositions = attacker.AvailablePositions;
            if (availablePositions.Any())
            {
                var position = availablePositions[Rnd.Next(availablePositions.Count)];
                return attacker.Attack(defender, position.X, position.Y, out changesList);
            }

            changesList = Array.Empty<Changes>();

            return false;
        }
    }
}