using System;
using SeaBattle.Domain.Enums;

namespace SeaBattle.Domain
{
    public class Cell
    {
        public Cell(int x, int y)
        {
            X = x;
            Y = y;
        }

        public bool IsShipDestroyed { get; set; }
        public bool Attacked { get; set; }
        public int X { get; }
        public int Y { get; }
        public CellType CellType { get; set; } = CellType.Empty;
        public Guid ShipId { get; set; }
    }
}