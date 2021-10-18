using System.Collections.Generic;
using SeaBattle.Domain;
using SeaBattle.Domain.Builders;

namespace SeaBattle.Web.Services
{
    public interface IGameService
    {
        void Add(Game game);
        IEnumerable<Game> GetAll();
    }
    public class GameService : IGameService
    {
        private List<Game> _games = new();
        
        public void Add(Game game)
        {
            _games.Add(game);
        }

        public IEnumerable<Game> GetAll()
        {
            return _games.ToArray();
        }
    }
}