using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SeaBattle.Domain;
using SeaBattle.Domain.Enums;
using SeaBattle.Web.Data;
using SeaBattle.Web.Events;
using SeaBattle.Web.Models;

namespace SeaBattle.Web.Managers
{
    public class GameManager : INotificationHandler<GameEvent>
    {
        private readonly ApplicationContext _context;

        public GameManager(ApplicationContext context)
        {
            _context = context;
        }

        public void Add(GameDetails gameDetails)
        {
            _context.Games.Add(new SeaBattleGame
            {
                GameId = gameDetails.Game.GameId,
                FirstPlayerId = gameDetails.FirstPlayer.PlayerId,
                SecondPlayerId = gameDetails.SecondPlayer.PlayerId,
                AttackerId = gameDetails.Game.AttackerId,
                DefenderId = gameDetails.Game.DefenderId,
                GameIsOver = gameDetails.Game.GameIsOver
            });

            _context.Players.Add(new SeaBattlePlayer
            {
                PlayerId = gameDetails.FirstPlayer.PlayerId,
                GameId = gameDetails.Game.GameId,
                UserName = gameDetails.FirstPlayer.UserName,
                OwnFieldId = gameDetails.Game.FirstPlayer.OwnField.FieldId,
                EnemyFieldId = gameDetails.Game.FirstPlayer.EnemyField.FieldId
            });

            _context.Players.Add(new SeaBattlePlayer
            {
                PlayerId = gameDetails.SecondPlayer.PlayerId,
                GameId = gameDetails.Game.GameId,
                UserName = gameDetails.SecondPlayer.UserName,
                OwnFieldId = gameDetails.Game.SecondPlayer.OwnField.FieldId,
                EnemyFieldId = gameDetails.Game.SecondPlayer.EnemyField.FieldId
            });

            _context.Fields.Add(new SeaBattleField
            {
                FieldId = gameDetails.Game.FirstPlayer.OwnField.FieldId,
                PlayerId = gameDetails.FirstPlayer.PlayerId,
                SizeX = gameDetails.Game.FirstPlayer.OwnField.SizeX,
                SizeY = gameDetails.Game.FirstPlayer.OwnField.SizeY
            });

            _context.Fields.Add(new SeaBattleField
            {
                FieldId = gameDetails.Game.FirstPlayer.EnemyField.FieldId,
                PlayerId = gameDetails.FirstPlayer.PlayerId,
                SizeX = gameDetails.Game.FirstPlayer.EnemyField.SizeX,
                SizeY = gameDetails.Game.FirstPlayer.EnemyField.SizeY
            });

            _context.Fields.Add(new SeaBattleField
            {
                FieldId = gameDetails.Game.SecondPlayer.OwnField.FieldId,
                PlayerId = gameDetails.SecondPlayer.PlayerId,
                SizeX = gameDetails.Game.SecondPlayer.OwnField.SizeX,
                SizeY = gameDetails.Game.SecondPlayer.OwnField.SizeY
            });

            _context.Fields.Add(new SeaBattleField
            {
                FieldId = gameDetails.Game.SecondPlayer.EnemyField.FieldId,
                PlayerId = gameDetails.SecondPlayer.PlayerId,
                SizeX = gameDetails.Game.SecondPlayer.EnemyField.SizeX,
                SizeY = gameDetails.Game.SecondPlayer.EnemyField.SizeY
            });

            foreach (var cell in gameDetails.Game.FirstPlayer.OwnField.GetCells()
                .Concat(gameDetails.Game.FirstPlayer.EnemyField.GetCells())
                .Concat(gameDetails.Game.SecondPlayer.OwnField.GetCells())
                .Concat(gameDetails.Game.SecondPlayer.EnemyField.GetCells()))
            {
                _context.Cells.Add(new SeaBattleCell
                {
                    FieldId = cell.FieldId,
                    PosX = cell.X,
                    PosY = cell.Y,
                    IsShipDestroyed = cell.IsShipDestroyed,
                    Attacked = cell.Attacked,
                    CellType = (int) cell.CellType,
                    ShipId = cell.ShipId
                });
            }

            foreach (var shipKeyValue in gameDetails.Game.FirstPlayer.OwnField.Ships)
            {
                foreach (var position in shipKeyValue.Value.Ship)
                {
                    _context.Ships.Add(new SeaBattleShip
                    {
                        ShipId = shipKeyValue.Key,
                        FieldId = gameDetails.Game.FirstPlayer.OwnField.FieldId,
                        PosX = position.X,
                        PosY = position.Y,
                        Type = 1
                    });
                }

                foreach (var position in shipKeyValue.Value.Border)
                {
                    _context.Ships.Add(new SeaBattleShip
                    {
                        ShipId = shipKeyValue.Key,
                        FieldId = gameDetails.Game.FirstPlayer.OwnField.FieldId,
                        PosX = position.X,
                        PosY = position.Y,
                        Type = 2
                    });
                }
            }

            foreach (var shipKeyValue in gameDetails.Game.SecondPlayer.OwnField.Ships)
            {
                foreach (var position in shipKeyValue.Value.Ship)
                {
                    _context.Ships.Add(new SeaBattleShip
                    {
                        ShipId = shipKeyValue.Key,
                        FieldId = gameDetails.Game.SecondPlayer.OwnField.FieldId,
                        PosX = position.X,
                        PosY = position.Y,
                        Type = 1
                    });
                }

                foreach (var position in shipKeyValue.Value.Border)
                {
                    _context.Ships.Add(new SeaBattleShip
                    {
                        ShipId = shipKeyValue.Key,
                        FieldId = gameDetails.Game.SecondPlayer.OwnField.FieldId,
                        PosX = position.X,
                        PosY = position.Y,
                        Type = 2
                    });
                }
            }

            foreach (var position in gameDetails.Game.FirstPlayer.NextPositions)
            {
                _context.Positions.Add(new SeaBattlePosition
                {
                    FieldId = gameDetails.Game.FirstPlayer.EnemyField.FieldId,
                    Type = 1,
                    PosX = position.X,
                    PosY = position.Y
                });
            }

            foreach (var position in gameDetails.Game.FirstPlayer.PreviousHits)
            {
                _context.Positions.Add(new SeaBattlePosition
                {
                    FieldId = gameDetails.Game.FirstPlayer.EnemyField.FieldId,
                    Type = 2,
                    PosX = position.X,
                    PosY = position.Y
                });
            }

            foreach (var position in gameDetails.Game.SecondPlayer.NextPositions)
            {
                _context.Positions.Add(new SeaBattlePosition
                {
                    FieldId = gameDetails.Game.SecondPlayer.EnemyField.FieldId,
                    Type = 1,
                    PosX = position.X,
                    PosY = position.Y
                });
            }

            foreach (var position in gameDetails.Game.SecondPlayer.PreviousHits)
            {
                _context.Positions.Add(new SeaBattlePosition
                {
                    FieldId = gameDetails.Game.SecondPlayer.EnemyField.FieldId,
                    Type = 2,
                    PosX = position.X,
                    PosY = position.Y
                });
            }

            _context.SaveChanges();
        }

        public IEnumerable<GameDetails> GetAll(Guid? gameId = null)
        {
            var gamesDb = _context.Games.Select(x => x);
            if (gameId.HasValue)
            {
                gamesDb = gamesDb.Where(x => x.GameId == gameId.Value);
            }

            var games = gamesDb.Select(game => new GameDetails
            {
                GameIsOver = game.GameIsOver,
                Game = new Game
                {
                    GameId = game.GameId,
                    AttackerId = game.AttackerId,
                    DefenderId = game.DefenderId,
                    FirstPlayer = new Player
                    {
                        PlayerId = game.FirstPlayerId
                    },
                    SecondPlayer = new Player
                    {
                        PlayerId = game.SecondPlayerId
                    }
                },
                FirstPlayer = new PlayerDetails
                {
                    PlayerId = game.FirstPlayerId,
                    UserName = _context.Players.First(player => player.PlayerId == game.FirstPlayerId).UserName
                },
                SecondPlayer = new PlayerDetails
                {
                    PlayerId = game.SecondPlayerId,
                    UserName = _context.Players.First(player => player.PlayerId == game.SecondPlayerId).UserName
                }
            }).ToList();

            if (gameId.HasValue)
            {
                games.ForEach(gameDetails =>
                {
                    gameDetails.Game.FirstPlayer.OwnField = _context.Players
                        .Where(player => player.PlayerId == gameDetails.FirstPlayer.PlayerId)
                        .SelectMany(player =>
                            _context.Fields.Where(field => field.FieldId == player.OwnFieldId)
                                .Select(field => GetField(field,
                                    _context.Cells.Where(cell => cell.FieldId == field.FieldId)
                                )))
                        .First();
                    gameDetails.Game.FirstPlayer.EnemyField = _context.Players
                        .Where(player => player.PlayerId == gameDetails.FirstPlayer.PlayerId)
                        .SelectMany(player =>
                            _context.Fields.Where(field => field.FieldId == player.EnemyFieldId)
                                .Select(field => GetField(field,
                                    _context.Cells.Where(cell => cell.FieldId == field.FieldId)
                                )))
                        .First();

                    gameDetails.Game.SecondPlayer.OwnField = _context.Players
                        .Where(player => player.PlayerId == gameDetails.SecondPlayer.PlayerId)
                        .SelectMany(player =>
                            _context.Fields.Where(field => field.FieldId == player.OwnFieldId)
                                .Select(field => GetField(field,
                                    _context.Cells.Where(cell => cell.FieldId == field.FieldId)
                                )))
                        .First();
                    gameDetails.Game.SecondPlayer.EnemyField = _context.Players
                        .Where(player => player.PlayerId == gameDetails.SecondPlayer.PlayerId)
                        .SelectMany(player =>
                            _context.Fields.Where(field => field.FieldId == player.EnemyFieldId)
                                .Select(field => GetField(field,
                                    _context.Cells.Where(cell => cell.FieldId == field.FieldId)
                                )))
                        .First();

                    gameDetails.Game.FirstPlayer.OwnField.Ships = _context.Ships
                        .Where(x => x.FieldId == gameDetails.Game.FirstPlayer.OwnField.FieldId)
                        .ToArray()
                        .GroupBy(x => x.ShipId)
                        .ToDictionary(x => x.Key, x => new ShipDetails
                        {
                            Ship = x.Where(x1 => x1.Type == 1)
                                .Select(x1 => new Position(x1.PosX, x1.PosY))
                                .ToList(),
                            Border = x.Where(x1 => x1.Type == 2)
                                .Select(x1 => new Position(x1.PosX, x1.PosY))
                                .ToList()
                        });

                    gameDetails.Game.SecondPlayer.OwnField.Ships = _context.Ships
                        .Where(x => x.FieldId == gameDetails.Game.SecondPlayer.OwnField.FieldId)
                        .ToArray()
                        .GroupBy(x => x.ShipId)
                        .ToDictionary(x => x.Key, x => new ShipDetails
                        {
                            Ship = x.Where(x1 => x1.Type == 1)
                                .Select(x1 => new Position(x1.PosX, x1.PosY))
                                .ToList(),
                            Border = x.Where(x1 => x1.Type == 2)
                                .Select(x1 => new Position(x1.PosX, x1.PosY))
                                .ToList()
                        });

                    _context.Positions
                        .Where(x => x.FieldId == gameDetails.Game.FirstPlayer.EnemyField.FieldId)
                        .ToList()
                        .ForEach(x =>
                        {
                            if (x.Type == 1)
                                gameDetails.Game.FirstPlayer.NextPositions.Add(new Position(x.PosX, x.PosY));
                            else if (x.Type == 2)
                                gameDetails.Game.FirstPlayer.PreviousHits.Add(new Position(x.PosX, x.PosY));
                        });

                    _context.Positions
                        .Where(x => x.FieldId == gameDetails.Game.SecondPlayer.EnemyField.FieldId)
                        .ToList()
                        .ForEach(x =>
                        {
                            if (x.Type == 1)
                                gameDetails.Game.SecondPlayer.NextPositions.Add(new Position(x.PosX, x.PosY));
                            else if (x.Type == 2)
                                gameDetails.Game.SecondPlayer.PreviousHits.Add(new Position(x.PosX, x.PosY));
                        });
                });
            }

            return games;
        }

        private static Field GetField(SeaBattleField fieldDb, IEnumerable<SeaBattleCell> cellsDb)
        {
            var field = new Field(fieldDb.SizeX, fieldDb.SizeY)
            {
                FieldId = fieldDb.FieldId
            };
            foreach (var cellDb in cellsDb)
            {
                var cell = field[cellDb.PosX, cellDb.PosY];
                cell.Attacked = cellDb.Attacked;
                cell.CellType = (CellType) cellDb.CellType;
                cell.ShipId = cellDb.ShipId;
                cell.IsShipDestroyed = cellDb.IsShipDestroyed;
            }

            return field;
        }

        public async Task Handle(GameEvent notification, CancellationToken cancellationToken)
        {
            var gameDb = await _context.Games.FirstAsync(game => game.GameId == notification.GameId,
                cancellationToken: cancellationToken);
            gameDb.AttackerId = notification.AttackerId;
            gameDb.DefenderId = notification.DefenderId;
            gameDb.GameIsOver = notification.GameIsOver;
            var firstPlayerDb = await _context.Players.FirstAsync(x => x.PlayerId == gameDb.FirstPlayerId,
                cancellationToken: cancellationToken);
            var secondPlayerDb = await _context.Players.FirstAsync(x => x.PlayerId == gameDb.SecondPlayerId,
                cancellationToken: cancellationToken);
            foreach (var changes in notification.ChangesList)
            {
                foreach (var cell in changes.AffectedCells)
                {
                    var cellDb = _context.Cells.First(x =>
                        x.FieldId == changes.FieldId && x.PosX == cell.X && x.PosY == cell.Y);
                    cellDb.Attacked = cell.Attacked;
                    cellDb.IsShipDestroyed = cell.IsShipDestroyed;
                    cellDb.CellType = (int) cell.CellType;
                }
            }

            _context.Positions.RemoveRange(_context.Positions.Where(x => x.FieldId == firstPlayerDb.EnemyFieldId));

            _context.Positions.RemoveRange(_context.Positions.Where(x => x.FieldId == secondPlayerDb.EnemyFieldId));

            foreach (var position in notification.FirstPlayerNextPositions)
            {
                _context.Positions.Add(new SeaBattlePosition
                {
                    FieldId = firstPlayerDb.EnemyFieldId,
                    Type = 1,
                    PosX = position.X,
                    PosY = position.Y
                });
            }

            foreach (var position in notification.FirstPlayerPreviousHits)
            {
                _context.Positions.Add(new SeaBattlePosition
                {
                    FieldId = firstPlayerDb.EnemyFieldId,
                    Type = 2,
                    PosX = position.X,
                    PosY = position.Y
                });
            }

            foreach (var position in notification.SecondPlayerNextPositions)
            {
                _context.Positions.Add(new SeaBattlePosition
                {
                    FieldId = secondPlayerDb.EnemyFieldId,
                    Type = 1,
                    PosX = position.X,
                    PosY = position.Y
                });
            }

            foreach (var position in notification.SecondPlayerPreviousHits)
            {
                _context.Positions.Add(new SeaBattlePosition
                {
                    FieldId = secondPlayerDb.EnemyFieldId,
                    Type = 2,
                    PosX = position.X,
                    PosY = position.Y
                });
            }

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}