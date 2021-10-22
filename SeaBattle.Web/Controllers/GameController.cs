using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using SeaBattle.Domain;
using SeaBattle.Domain.Builders;
using SeaBattle.Web.Services;

namespace SeaBattle.Web.Controllers
{
    public class GameController : Controller
    {
        private readonly GameService _gameService;
        
        public GameController(GameService gameService)
        {
            _gameService = gameService;
        }
        
        [HttpGet]
        public IActionResult Index()
        {
            return View(_gameService.GetAll().ToArray());
        }

        [HttpGet]
        public IActionResult Create(bool withBot = false)
        {
            var game = new GameBuilder()
                .WithFieldSize(10, 10)
                .WithShips(new[] {4, 3, 3, 2, 2, 2, 1, 1, 1, 1})
                .Build();
            
            _gameService.Add(game, withBot);
            
            return RedirectToAction("Index");
        }
    }
}