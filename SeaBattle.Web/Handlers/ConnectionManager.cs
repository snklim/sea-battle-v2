using System;
using System.Collections.Generic;
using System.Net.WebSockets;

namespace SeaBattle.Web.Handlers
{
    public class ConnectionManager
    {
        private readonly Dictionary<Guid, Dictionary<Guid, WebSocket>> _socketsPerGame = new();

        public void Set(Guid gameId, Guid playerId, WebSocket webSocket)
        {
            if (!_socketsPerGame.ContainsKey(gameId))
            {
                _socketsPerGame.Add(gameId, new Dictionary<Guid, WebSocket>());
            }

            _socketsPerGame[gameId][playerId] = webSocket;
        }

        public bool TryGet(Guid gameId, Guid playerId, out WebSocket webSocket)
        {
            webSocket = null;
            if (_socketsPerGame.TryGetValue(gameId, out var socketsPerPlayer) &&
                socketsPerPlayer.TryGetValue(playerId, out webSocket)) return true;
            webSocket = null;
            return false;

        }
    }
}