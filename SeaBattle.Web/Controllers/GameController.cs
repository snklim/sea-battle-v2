using System;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SeaBattle.Domain.Builders;
using SeaBattle.Web.Events;
using SeaBattle.Web.Handlers;
using SeaBattle.Web.Managers;
using SeaBattle.Web.Models;
using SeaBattle.Web.ViewModels;

namespace SeaBattle.Web.Controllers
{
    [Authorize]
    public class GameController : Controller
    {
        private readonly GameManager _gameManager;
        private readonly UserManager<User> _userManager;
        private readonly IMediator _mediator;

        public GameController(GameManager gameManager, UserManager<User> userManager, IMediator mediator)
        {
            _gameManager = gameManager;
            _userManager = userManager;
            _mediator = mediator;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View(_gameManager.GetAll()
                .Where(gameDetails => !gameDetails.GameIsOver)
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
        public async Task<IActionResult> Create(CreateGameViewModel model)
        {
            var game = new GameBuilder()
                .WithFieldSize(10, 10)
                .WithShips(new[] {4, 3, 3, 2, 2, 2, 1, 1, 1, 1})
                .Build();

            _gameManager.Add(new GameDetails
            {
                Game = game,
                FirstPlayer = new PlayerDetails
                {
                    PlayerId = game.FirstPlayer.PlayerId,
                    UserName = User.Identity.Name
                },
                SecondPlayer = new PlayerDetails
                {
                    PlayerId = game.SecondPlayer.PlayerId,
                    UserName = model.User
                }
            });
            
            await _mediator.Publish(new InfoEvent
            {
                FromUser = User.Identity.Name,
                ToUser = model.User,
                Message = $"Game was created by {User.Identity.Name}."
            });

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Play(Guid gameId, Guid playerId)
        {
            return View(new PlayViewModel
            {
                GameId = gameId,
                PlayerId = playerId
            });
        }
    }
}