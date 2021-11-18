using System;

namespace SeaBattle.Web.Models
{
    public class SeaBattleField
    {
        public Guid FieldId { get; set; }
        public Guid PlayerId { get; set; }
        public int SizeX { get; set; }
        public int SizeY { get; set; }
    }
}