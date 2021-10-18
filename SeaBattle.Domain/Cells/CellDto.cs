using SeaBattle.Domain.Enums;

namespace SeaBattle.Domain.Cells
{
    public class CellDto
    {
        public int X { get; init; }
        public int Y { get; init; }
        public CellType CellType { get; init; }
        public bool Attacked { get; init; }
        public bool IsShipDestroyed { get; init; }

        public Cell ToCell()
        {
            if (CellType == CellType.Border)
                return new BorderCell(X, Y);
            if (CellType == CellType.Ship)
                return new ShipCell(X, Y, new ShipDetails());
            return new EmptyCell(X, Y);
        }
    }
}