using System;
using SeaBattle.Domain.Enums;

namespace SeaBattle.Domain
{
    public class Cell
    {
        public Cell(Guid fieldId, int x, int y)
        {
            FieldId = fieldId;
            X = x;
            Y = y;
        }

        public Guid FieldId { get; set; }
        public bool IsShipDestroyed { get; set; }
        public bool Attacked { get; set; }
        public int X { get; }
        public int Y { get; }
        public CellType CellType { get; set; } = CellType.Empty;
        public Guid ShipId { get; set; }
    }
}