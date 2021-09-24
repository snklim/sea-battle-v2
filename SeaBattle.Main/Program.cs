using System;
using System.Collections.Generic;
using System.Linq;
using SeaBattle.Domain;
using SeaBattle.Domain.Builders;
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
            Print(game.AttackerField.GetCells(), 0);
            Print(game.DefenderField.GetCells().Select(cell => new EmptyCell(cell.X, cell.Y, cell.FieldId)), 0);
            Print(game.AttackerField.GetCells().Select(cell => new EmptyCell(cell.X, cell.Y, cell.FieldId)), 11, true);
            Print(game.DefenderField.GetCells(), 11, true);

            while (!game.GameIsOver)
            {
                Console.ReadLine();

                var affectedCells = game.Next(new AttackByRandomPositionCommand(game.AttackerField.FieldId)).ToArray();
                Print(affectedCells);
                Print(affectedCells, 11, true);

                Console.SetCursorPosition(0, 22);
            }
        }

        static void Print(IEnumerable<Cell> cells, int shiftX = 0, bool swapFields = false)
        {
            foreach (var cell in cells)
            {
                if (!ShiftPerField.TryGetValue(cell.FieldId, out var shiftY))
                {
                    shiftY = 11 * ShiftPerField.Count;
                    ShiftPerField.Add(cell.FieldId, shiftY);
                }

                if (swapFields)
                {
                    if (shiftY == 11) shiftY = 0;
                    else shiftY = 11;
                }

                Console.SetCursorPosition(cell.Y + shiftY, cell.X + shiftX);
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

        private static readonly Dictionary<Guid, int> ShiftPerField = new Dictionary<Guid, int>();
    }
}