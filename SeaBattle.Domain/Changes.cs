using System;
using System.Collections.Generic;
using SeaBattle.Domain.Cells;

namespace SeaBattle.Domain
{
    public class Changes
    {
        public Guid FieldId { get; init; }
        public Guid PlayerId { get; init; }
        public IReadOnlyCollection<CellDto> AffectedCells { get; init; }
    }
}