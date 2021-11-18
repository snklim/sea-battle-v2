using System.Collections.Generic;

namespace SeaBattle.Domain
{
    public class ShipDetails
    {
        public List<Position> Border { get; set; } = new ();
        public List<Position> Ship { get; set; } = new ();
    }
}