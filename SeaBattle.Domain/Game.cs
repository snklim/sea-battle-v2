using System.Collections.Generic;
using System.Linq;
using SeaBattle.Domain.Cells;
using SeaBattle.Domain.Commands;
using SeaBattle.Domain.Enums;

namespace SeaBattle.Domain
{
    public class Game
    {
        public Player Attacker { get; set; }
        public Player Defender { get; set; }
        public IEnumerable<Cell> Next(AttackPositionCommand command)
        {
            if (!command.Execute(Attacker, Defender, out var affectedCells))
            {
                (Attacker, Defender) = (Defender, Attacker);
            }

            return affectedCells;
        }

        public bool GameIsOver => Attacker.OwnField.AllShipsDestroyed || Defender.OwnField.AllShipsDestroyed;
    }
}