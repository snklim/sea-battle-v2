using System;
using SeaBattle.Domain.Enums;

namespace SeaBattle.Domain.Cells
{
    public class BorderCell : Cell
    {
        public BorderCell(int x, int y) : base(x, y, CellType.Border)
        {
            
        }
    }
}