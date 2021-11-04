using System;
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
    }
}