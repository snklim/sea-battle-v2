using SeaBattle.Domain.Enums;

namespace SeaBattle.Domain.Cells
{
    public class CellDto
    {
        public int X { get; set; }
        public int Y { get; set; }
        public CellType CellType { get; set; }
        public bool Attacked { get; set; }
    }
}