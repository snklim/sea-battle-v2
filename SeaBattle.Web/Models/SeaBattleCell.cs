using System;
using System.ComponentModel.DataAnnotations;

namespace SeaBattle.Web.Models
{
    public class SeaBattleCell
    {
        public Guid FieldId { get; set; }
        public int PosX { get; set; }
        public int PosY { get; set; }
        public bool IsShipDestroyed { get; set; }
        public bool Attacked { get; set; }
        public int CellType { get; set; }
        public Guid ShipId { get; set; }
    }
}