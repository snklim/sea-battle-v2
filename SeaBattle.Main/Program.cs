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
            Print(new[]
            {
                new Changes
                {
                    PlayerId = game.FirstPlayer.PlayerId,
                    FieldId = game.FirstPlayer.OwnField.FieldId,
                    AffectedCells = game.FirstPlayer.OwnField.GetCells().ToArray()
                },
                new Changes
                {
                    PlayerId = game.FirstPlayer.PlayerId,
                    FieldId = game.FirstPlayer.EnemyField.FieldId,
                    AffectedCells = game.FirstPlayer.EnemyField.GetCells().ToArray()
                },
                new Changes
                {
                    PlayerId = game.SecondPlayer.PlayerId,
                    FieldId = game.SecondPlayer.OwnField.FieldId,
                    AffectedCells = game.SecondPlayer.OwnField.GetCells().ToArray()
                },
                new Changes
                {
                    PlayerId = game.SecondPlayer.PlayerId,
                    FieldId = game.SecondPlayer.EnemyField.FieldId,
                    AffectedCells = game.SecondPlayer.EnemyField.GetCells().ToArray()
                }
            });

            while (!game.GameIsOver)
            {
                Console.ReadLine();
            
                var changesList = game.Next(new AttackByRandomPositionCommand(game.FirstPlayer.PlayerId)).ToArray();
                Print(changesList);
            
                Console.SetCursorPosition(0, 22);
            }
        }

        static void Print(IReadOnlyCollection<Changes> changesList)
        {
            foreach (var changes in changesList)
            {
                var key = $"{changes.PlayerId}_{changes.FieldId}";
                if (!ShiftPerField.TryGetValue(key, out var shift))
                {
                    var shiftX = 11 * (ShiftPerField.Count / 2);
                    var shiftY = 11 * (ShiftPerField.Count % 2);
                    shift = (shiftX, shiftY);
                    ShiftPerField.Add(key, shift);
                }

                foreach (var cell in changes.AffectedCells)
                {
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
        }

        private static readonly Dictionary<string, (int shiftX, int shiftY)> ShiftPerField = new ();
    }
}