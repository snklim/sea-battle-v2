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
        public Guid FieldId { get; } = Guid.NewGuid();
        public List<(int x, int y)> NextPositions { get; } = new List<(int x, int y)>();
        public List<(int x, int y)> PreviousHits { get; } = new List<(int x, int y)>();

        public Field(int sizeX, int sizeY)
        {
            Cells = new Cell[sizeX, sizeY];
            SizeX = sizeX;
            SizeY = sizeY;
            for (var x = 0; x < sizeX; x++)
            for (var y = 0; y < sizeY; y++)
            {
                Cells[x, y] = new EmptyCell(x, y, FieldId);
            }
        }
        
        public Cell this[int x,int y]
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
                if (IsPositionValid(x,y))
                {
                    Cells[x, y] = value;
                    return;
                }

                throw new ApplicationException("Position is invalid");
            }
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

        public bool Attack(int posX, int posY, out Cell cell, out IEnumerable<Cell> affectedCells)
        {
            cell = this[posX, posY];
            affectedCells = cell.Attack();
            return cell.CellType == CellType.Ship;
        }

        public List<(int x, int y)> AvailablePositions => GetCells()
            .Where(cell => !cell.Attacked)
            .Select(cell => (x: cell.X, y: cell.Y))
            .ToList();

        public bool AllShipsDestroyed => GetCells()
            .Where(cell => cell.CellType == CellType.Ship)
            .All(cell => cell.IsShipDestroyed);
    }
}