using System;
using System.Collections.Generic;
using System.Linq;
using SeaBattle.Domain.Cells;
using SeaBattle.Domain.Enums;

namespace SeaBattle.Domain
{
    public class Field
    {
        private Cell[,] Cells { get; }
        public int SizeX { get; }
        public int SizeY { get; }
        public Guid FieldId { get; } = Guid.NewGuid();

        public Field(int sizeX, int sizeY)
        {
            Cells = new Cell[sizeX, sizeY];
            SizeX = sizeX;
            SizeY = sizeY;
            for (var x = 0; x < sizeX; x++)
            for (var y = 0; y < sizeY; y++)
            {
                Cells[x, y] = new EmptyCell(x, y);
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

        public IEnumerable<CellDto> GetCells()
        {
            for (var x = 0; x < SizeX; x++)
            for (var y = 0; y < SizeY; y++)
            {
                yield return this[x, y].ToCellDto();
            }
        }

        public bool AllShipsDestroyed => GetCells()
            .Where(cell => cell.CellType == CellType.Ship)
            .All(cell => cell.IsShipDestroyed);
    }
}