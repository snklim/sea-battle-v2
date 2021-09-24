using System.Collections.Generic;
using System.Linq;
using SeaBattle.Domain.Commands;
using SeaBattle.Domain.Enums;

namespace SeaBattle.Domain
{
    public class Game
    {
        public Field AttackerField { get; set; }
        public Field DefenderField { get; set; }
        public IEnumerable<Cell> Next(AttackPositionCommand command)
        {
            if (command.FieldId != AttackerField.FieldId) return Enumerable.Empty<Cell>();
            
            if (!command.Execute(DefenderField, out var affectedCells))
            {
                (AttackerField, DefenderField) = (DefenderField, AttackerField);
            }

            return affectedCells;
        }

        public bool GameIsOver => AttackerField.AllShipsDestroyed || DefenderField.AllShipsDestroyed;
    }
}