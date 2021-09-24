using System.Collections.Generic;
using SeaBattle.Domain.Enums;

namespace SeaBattle.Domain.Builders
{
    public class GameBuilder
    {
        private Game _game;
        private FieldBuilder _attackerFieldBuilder;
        private FieldBuilder _defenderFieldBuilder;

        public GameBuilder WithFieldSize(int sizeX, int sizeY)
        {
            _attackerFieldBuilder = new FieldBuilder().WithSize(sizeX, sizeY);
            _defenderFieldBuilder = new FieldBuilder().WithSize(sizeX, sizeY);
            return this;
        }

        public GameBuilder WithShips(IEnumerable<int> ships)
        {
            foreach (var ship in ships)
            {
                _attackerFieldBuilder.WithShip(ship);
                _defenderFieldBuilder.WithShip(ship);
            }

            return this;
        }

        public GameBuilder WithShipAtPositionOnAttackerField(int x, int y, int length, Orientation orientation)
        {
            _attackerFieldBuilder.WithShipAtPosition(x, y, length, orientation);
            return this;
        }
        
        public GameBuilder WithShipAtPositionOnDefenderField(int x, int y, int length, Orientation orientation)
        {
            _defenderFieldBuilder.WithShipAtPosition(x, y, length, orientation);
            return this;
        }

        public Game Build()
        {
            _game = new Game
            {
                AttackerField = _attackerFieldBuilder.Build(),
                DefenderField = _defenderFieldBuilder.Build()
            };
            return _game;
        }
    }
}