using System.Collections.Generic;
using System.Linq;
using SeaBattle.Domain.Cells;

namespace SeaBattle.Domain
{
    public class ShipDetails
    {
        public List<BorderCell> Border { get; } = new List<BorderCell>();
        public List<ShipCell> Ship { get; } = new List<ShipCell>();
        public int CellsAlive => Ship.Count(cell => !cell.Attacked);
    }
}