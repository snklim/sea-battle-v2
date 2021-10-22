using System.Collections.Generic;
using SeaBattle.Domain;

namespace SeaBattle.Web.Services
{
    public class GameService
    {
        private readonly List<(Game game, bool withBot)> _games = new();

        
        public void Add(Game game, bool withBot)
        {
            _games.Add((game, withBot));
        }

        public IEnumerable<(Game game, bool wtihBot)> GetAll()
        {
            return _games.ToArray();
        }
    }
}