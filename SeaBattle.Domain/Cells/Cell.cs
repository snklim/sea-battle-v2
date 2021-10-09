using System;
using System.Collections.Generic;
using System.Linq;
using SeaBattle.Domain.Enums;

namespace SeaBattle.Domain.Cells
{
    public abstract class Cell
    {
        protected Cell(int x, int y, CellType cellType, Guid fieldId)
        {
            X = x;
            Y = y;
            CellType = cellType;
            FieldId = fieldId;
        }

        public virtual IEnumerable<Cell> Attack()
        {
            if (Attacked) return Enumerable.Empty<Cell>();
            Attacked = true;
            return new[] {this};
        }

        public virtual bool IsShipDestroyed => false;

        public bool Attacked { get; set; }
        public Guid FieldId { get; }
        public int X { get; }
        public int Y { get; }
        public CellType CellType { get; }

        public CellDto ToCellDto()
        {
            return CellType == CellType.Ship
                ? new CellDto {X = X, Y = Y, Attacked = true, CellType = CellType.Ship}
                : CellType == CellType.Border
                    ? new CellDto {X = X, Y = Y, Attacked = true, CellType = CellType.Border}
                    : new CellDto {X = X, Y = Y, Attacked = true, CellType = CellType.Empty};
        }
    }
}