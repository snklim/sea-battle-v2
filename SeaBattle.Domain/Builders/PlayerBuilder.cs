using SeaBattle.Domain.Enums;

namespace SeaBattle.Domain.Builders
{
    public class PlayerBuilder
    {
        private readonly FieldBuilder _ownFieldBuilder = new ();
        private readonly FieldBuilder _enemyFieldBuilder = new ();

        public PlayerBuilder WithField(int sizeX, int sizeY)
        {
            _ownFieldBuilder.WithSize(sizeX, sizeY);
            _enemyFieldBuilder.WithSize(sizeX, sizeY);
            return this;
        }

        public PlayerBuilder WithShip(int length)
        {
            _ownFieldBuilder.WithShip(length);
            return this;
        }

        public PlayerBuilder WithShipAtPos(int x, int y, int length, Orientation orientation)
        {
            _ownFieldBuilder.WithShipAtPosition(x, y, length, orientation);
            return this;
        }

        public Player Build()
        {
            return new Player
            {
                OwnField = _ownFieldBuilder.Build(),
                EnemyField = _enemyFieldBuilder.Build()
            };
        }
    }
}