using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SeaBattle.Domain.Builders;
using SeaBattle.Web.Models;
using SeaBattle.Web.Services;
using SeaBattle.Web.ViewModels;

namespace SeaBattle.Web.Controllers
{
    [Authorize]
    public class GameController : Controller
    {
        private readonly GameService _gameService;
        private readonly UserManager<User> _userManager;

        public GameController(GameService gameService, UserManager<User> userManager)
        {
            _gameService = gameService;
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View(_gameService.GetAll()
                .Where(gameDetails => !gameDetails.Game.GameIsOver)
                .ToArray());
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new CreateGameViewModel
            {
                Users = new[] {new SelectListItem(GameDetails.Bot, GameDetails.Bot)}
                    .Concat(_userManager.Users
                        .Where(user => user.UserName != User.Identity.Name)
                        .Select(user => new SelectListItem(user.UserName, user.UserName)))
                    .ToList()
            });
        }

        [HttpPost]
        public IActionResult Create(CreateGameViewModel model)
        {
            var game = new GameBuilder()
                .WithFieldSize(10, 10)
                .WithShips(new[] {4, 3, 3, 2, 2, 2, 1, 1, 1, 1})
                .Build();

            _gameService.Add(new GameDetails
            {
                Game = game,
                FirstPlayer = new PlayerDetails
                {
                    PlayerId = game.Attacker.PlayerId,
                    UserName = User.Identity.Name
                },
                SecondPlayer = new PlayerDetails
                {
                    PlayerId = game.Defender.PlayerId,
                    UserName = model.User
                }
            });

            return RedirectToAction("Index");
        }
    }
}