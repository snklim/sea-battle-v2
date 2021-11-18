using System.Collections.Generic;
using SeaBattle.Domain.Enums;

namespace SeaBattle.Domain.Builders
{
    public class GameBuilder
    {
        private Game _game;
        private PlayerBuilder _attackerBuilder;
        private PlayerBuilder _defenderBuilder;

        public GameBuilder WithFieldSize(int sizeX, int sizeY)
        {
            _attackerBuilder = new PlayerBuilder().WithField(sizeX, sizeY);
            _defenderBuilder = new PlayerBuilder().WithField(sizeX, sizeY);
            return this;
        }

        public GameBuilder WithShips(IEnumerable<int> ships)
        {
            foreach (var ship in ships)
            {
                _attackerBuilder.WithShip(ship);
                _defenderBuilder.WithShip(ship);
            }

            return this;
        }

        public GameBuilder WithShipAtPositionOnAttackerField(int x, int y, int length, Orientation orientation)
        {
            _attackerBuilder.WithShipAtPos(x, y, length, orientation);
            return this;
        }
        
        public GameBuilder WithShipAtPositionOnDefenderField(int x, int y, int length, Orientation orientation)
        {
            _defenderBuilder.WithShipAtPos(x, y, length, orientation);
            return this;
        }

        public Game Build()
        {
            var (firstPlayer, secondPlayer) = (_attackerBuilder.Build(), _defenderBuilder.Build());
            _game = new Game
            {
                FirstPlayer = firstPlayer,
                SecondPlayer = secondPlayer,
                AttackerId = firstPlayer.PlayerId,
                DefenderId = secondPlayer.PlayerId
            };
            return _game;
        }
    }
}