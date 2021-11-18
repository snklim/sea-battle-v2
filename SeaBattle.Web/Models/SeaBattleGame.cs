using System;

namespace SeaBattle.Web.Models
{
    public class SeaBattleGame
    {
        public Guid GameId { get; set; }
        public Guid FirstPlayerId { get; set; }
        public Guid SecondPlayerId { get; set; }
        public Guid AttackerId { get; set; }
        public Guid DefenderId { get; set; }
        public bool GameIsOver { get; set; }
    }
}