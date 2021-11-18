using System;

namespace SeaBattle.Web.Models
{
    public class SeaBattlePlayer
    {
        public Guid PlayerId { get; set; }
        public Guid GameId { get; set; }
        public string UserName { get; set; }
        public Guid OwnFieldId { get; set; }
        public Guid EnemyFieldId { get; set; }
    }
}