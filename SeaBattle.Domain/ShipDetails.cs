using System.Collections.Generic;
using System.Linq;
using SeaBattle.Domain.Cells;

namespace SeaBattle.Domain
{
    public class ShipDetails
    {
        public List<BorderCell> Border { get; } = new ();
        public List<ShipCell> Ship { get; } = new ();
        public int CellsAlive => Ship.Count(cell => !cell.Attacked);
    }
}