using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SeaBattle.Domain;
using SeaBattle.Web.Models;

namespace SeaBattle.Web.Managers
{
    public class GameManager
    {
        private static readonly Dictionary<Guid, string> Games = new();

        public void Add(GameDetails gameDetails)
        {
            Games[gameDetails.Game.GameId] = System.Text.Json.JsonSerializer.Serialize(gameDetails);
        }

        public IEnumerable<GameDetails> GetAll(Guid? gameId = null)
        {
            var games = Games.Values.Select(x => System.Text.Json.JsonSerializer.Deserialize<GameDetails>(x)).ToArray();
            return games;
        }

        public async Task Update(Game game)
        {
            var gameDetails = System.Text.Json.JsonSerializer.Deserialize<GameDetails>(Games[game.GameId]);
            gameDetails.Game = game;
            gameDetails.GameIsOver = game.GameIsOver;
            Games[gameDetails.Game.GameId] = System.Text.Json.JsonSerializer.Serialize(gameDetails);
            await Task.Yield();
        }
    }
}