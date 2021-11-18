using System;
using System.Collections.Generic;
using System.Linq;
using SeaBattle.Domain.Enums;

namespace SeaBattle.Domain
{
    public class Field
    {
        private Cell[,] Cells { get; }
        public int SizeX { get; }
        public int SizeY { get; }
        public Guid FieldId { get; set; } = Guid.NewGuid();
        public Dictionary<Guid, ShipDetails> Ships { get; set; } = new();

        public Field(int sizeX, int sizeY)
        {
            Cells = new Cell[sizeX, sizeY];
            SizeX = sizeX;
            SizeY = sizeY;
            for (var x = 0; x < sizeX; x++)
            for (var y = 0; y < sizeY; y++)
            {
                Cells[x, y] = new Cell(FieldId, x, y);
            }
        }

        public Cell this[int x, int y]
        {
            get
            {
                if (IsPositionValid(x, y))
                {
                    return Cells[x, y];
                }

                throw new ApplicationException("Position is invalid");
            }
            set
            {
                if (IsPositionValid(x, y))
                {
                    Cells[x, y] = value;
                    return;
                }

                throw new ApplicationException("Position is invalid");
            }
        }

        public IEnumerable<Cell> Attack(int posX, int posY, out Cell attackedCell)
        { 
            attackedCell = this[posX, posY];

            if (attackedCell.Attacked) return Enumerable.Empty<Cell>();

            attackedCell.Attacked = true;

            if (attackedCell.CellType == CellType.Ship)
            {
                var shipDetails = Ships[attackedCell.ShipId];

                if (shipDetails.Ship.Select(pos => this[pos.X, pos.Y]).Count(cell => !cell.Attacked) == 0)
                {
                    attackedCell.IsShipDestroyed = true;
                    var affectedCells = new List<Cell>();

                    foreach (var cell in shipDetails.Border.Select(pos => this[pos.X, pos.Y]))
                    {
                        cell.Attacked = true;
                        cell.IsShipDestroyed = true;
                        affectedCells.Add(cell);
                    }

                    foreach (var cell in shipDetails.Ship.Select(pos => this[pos.X, pos.Y]))
                    {
                        cell.Attacked = true;
                        cell.IsShipDestroyed = true;
                        affectedCells.Add(cell);
                    }

                    return affectedCells;
                }
            }

            return new[] {attackedCell};
        }

        public bool IsPositionValid(int x, int y)
        {
            return 0 <= x && x < SizeX && 0 <= y && y < SizeY;
        }

        public IEnumerable<Cell> GetCells()
        {
            for (var x = 0; x < SizeX; x++)
            for (var y = 0; y < SizeY; y++)
            {
                yield return this[x, y];
            }
        }

        public bool AllShipsDestroyed => GetCells()
            .Where(cell => cell.CellType == CellType.Ship)
            .All(cell => cell.IsShipDestroyed);
    }
}