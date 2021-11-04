using System.Collections.Generic;
using SeaBattle.Web.Models;

namespace SeaBattle.Web.Services
{
    public class GameService
    {
        private readonly List<GameDetails> _games = new();

        
        public void Add(GameDetails gameDetails)
        {
            _games.Add(gameDetails);
        }

        public IEnumerable<GameDetails> GetAll()
        {
            return _games.ToArray();
        }
    }
}