using System;
using SeaBattle.Domain.Enums;

namespace SeaBattle.Domain.Cells
{
    public class BorderCell : Cell
    {
        public BorderCell(int x, int y, Guid fieldId) : base(x, y, CellType.Border, fieldId)
        {
            
        }
    }
}