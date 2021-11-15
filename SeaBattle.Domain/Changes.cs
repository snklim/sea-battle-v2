using System;
using System.Collections.Generic;

namespace SeaBattle.Domain
{
    public class Changes
    {
        public Guid FieldId { get; init; }
        public Guid PlayerId { get; init; }
        public IReadOnlyCollection<Cell> AffectedCells { get; init; }
    }
}