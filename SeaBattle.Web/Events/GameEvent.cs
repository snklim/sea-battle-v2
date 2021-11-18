using System;
using System.Collections.Generic;
using MediatR;
using SeaBattle.Domain;

namespace SeaBattle.Web.Events
{
    public class GameEvent : INotification
    {
        public Guid GameId { get; set; }
        public Guid PlayerId { get; set; }
        public Changes[] ChangesList { get; set; }
        public string Message { get; set; }
        public Guid AttackerId { get; set; }
        public Guid DefenderId { get; set; }
        public bool GameIsOver { get; set; }
        public List<Position> FirstPlayerNextPositions { get; set; }
        public List<Position> FirstPlayerPreviousHits { get; set; }
        public List<Position> SecondPlayerNextPositions { get; set; }
        public List<Position> SecondPlayerPreviousHits { get; set; }
    }
}