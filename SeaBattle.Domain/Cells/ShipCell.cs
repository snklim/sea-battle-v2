using System;
using System.Collections.Generic;
using System.Linq;
using SeaBattle.Domain.Enums;

namespace SeaBattle.Domain.Cells
{
    public class ShipCell : Cell
    {
        public ShipCell(int x, int y, ShipDetails shipDetails) : base(x, y, CellType.Ship)
        {
            ShipDetails = shipDetails;
        }

        public override IEnumerable<Cell> Attack()
        {
            if (Attacked) return Enumerable.Empty<Cell>();
            
            Attacked = true;
            
            if (ShipDetails.CellsAlive == 0)
            {
                var affectedCells = new List<Cell>();
                foreach (var cell in ShipDetails.Border)
                {
                    cell.Attacked = true;
                    affectedCells.Add(cell);
                }

                foreach (var cell in ShipDetails.Ship)
                {
                    cell.Attacked = true;
                    affectedCells.Add(cell);
                }

                return affectedCells;
            }

            return new[] {this};
        }

        private ShipDetails ShipDetails { get; }

        protected override bool IsShipDestroyed => ShipDetails.CellsAlive == 0;
    }
}