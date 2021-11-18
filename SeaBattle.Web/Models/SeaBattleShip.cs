using System;

namespace SeaBattle.Web.Models
{
    public class SeaBattleShip
    {
        public Guid ShipId { get; set; }
        public Guid FieldId { get; set; }
        public int PosX { get; set; }
        public int PosY { get; set; }
        public int Type { get; set; }
    }
}