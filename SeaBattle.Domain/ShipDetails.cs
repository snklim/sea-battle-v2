using System.Collections.Generic;
using System.Linq;

namespace SeaBattle.Domain
{
    public class ShipDetails
    {
        public List<Position> Border { get; } = new ();
        public List<Position> Ship { get; } = new ();
        //public int CellsAlive => Ship.Count(cell => !cell.Attacked);
    }
}