using System;
using System.Collections.Generic;
using System.Linq;
using SeaBattle.Domain;
using SeaBattle.Domain.Builders;
using SeaBattle.Domain.Cells;
using SeaBattle.Domain.Commands;
using SeaBattle.Domain.Enums;

namespace SeaBattle.Main
{
    class Program
    {
        static void Main(string[] args)
        {
            var game = new GameBuilder()
                .WithFieldSize(10, 10)
                .WithShips(new[] {4, 3, 3, 2, 2, 2, 1, 1, 1, 1})
                .Build();

            Console.Clear();
            Print(game.Attacker.OwnField.GetCells());
            Print(game.Attacker.EnemyField.GetCells());
            Print(game.Defender.OwnField.GetCells());
            Print(game.Defender.EnemyField.GetCells());

            while (!game.GameIsOver)
            {
                Console.ReadLine();
            
                var affectedCells = game.Next(new AttackByRandomPositionCommand(game.Attacker.PlayerId)).ToArray();
                Print(affectedCells);
            
                Console.SetCursorPosition(0, 22);
            }
        }

        static void Print(IEnumerable<Cell> cells)
        {
            foreach (var cell in cells)
            {
                if (!ShiftPerField.TryGetValue(cell.FieldId, out var shift))
                {
                    var shiftX = 11 * (ShiftPerField.Count / 2);
                    var shiftY = 11 * (ShiftPerField.Count % 2);
                    shift = (shiftX, shiftY);
                    ShiftPerField.Add(cell.FieldId, shift);
                }

                Console.SetCursorPosition(cell.Y + shift.shiftY, cell.X + shift.shiftX);
                if (cell.Attacked)
                {
                    Console.BackgroundColor = ConsoleColor.Red;
                }
                else
                {
                    Console.ResetColor();
                }
                if (cell.CellType == CellType.Ship)
                    Console.Write('O');
                else
                    Console.Write('~');
            }
        }

        private static readonly Dictionary<Guid, (int shiftX, int shiftY)> ShiftPerField = new ();
    }
}