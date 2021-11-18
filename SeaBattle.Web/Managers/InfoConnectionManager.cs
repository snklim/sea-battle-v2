using System.Collections.Generic;
using System.Net.WebSockets;

namespace SeaBattle.Web.Managers
{
    public class InfoConnectionManager
    {
        private static readonly Dictionary<string, WebSocket> _connection = new();

        public void Add(string userName, WebSocket webSocket)
        {
            _connection[userName] = webSocket;
        }

        public bool TryGet(string userName, out WebSocket webSocket)
        {
            if (_connection.TryGetValue(userName, out webSocket))
            {
                return true;
            }
            
            return false;
        }

        public void Remove(string userName)
        {
            _connection.Remove(userName);
        }
    }
}