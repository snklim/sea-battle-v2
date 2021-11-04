using System;
using MediatR;

namespace SeaBattle.Web.Events
{
    public class MoveEvent : INotification
    {
        public string Action { get; set; }
        public Guid GameId { get; init; }
        public Guid PlayerId { get; init; }
        public int PosX { get; init; }
        public int PosY { get; init; }
    }
}