using System;
using SeaBattle.Domain.Enums;

namespace SeaBattle.Domain
{
    public class EmptyCell : Cell
    {
        public EmptyCell(int sizeX, int sizeY, Guid fieldId) : base(sizeX, sizeY, CellType.Empty, fieldId)
        {
            
        }
    }
}