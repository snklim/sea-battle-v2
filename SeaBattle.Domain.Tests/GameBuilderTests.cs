using NUnit.Framework;
using SeaBattle.Domain.Builders;
using SeaBattle.Domain.Commands;
using SeaBattle.Domain.Enums;

namespace SeaBattle.Domain.Tests
{
    public class GameBuilderTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void BuildGameWithFieldSize_GameWithFieldSizeCreated()
        {
            // Arrange
            var gameBuilder = new GameBuilder();
            var ships = new[] {1, 1};
            var sizeX = 3;
            var sizeY = 3;

            // Act
            var game = gameBuilder
                .WithFieldSize(sizeX, sizeY)
                .WithShips(ships)
                .Build();

            // Assert
            Assert.IsNotNull(game);
            Assert.IsNotNull(game.FirstPlayer.OwnField);
            Assert.IsNotNull(game.SecondPlayer.OwnField);
        }
        
        [Test]
        public void BuildGameWithFieldSize_GameWithFieldsAndShipsCreate()
        {
            // Arrange
            var gameBuilder = new GameBuilder();
            var positionX = 2;
            var positionY = 2;
            var shipLength = 1;
            var sizeX = 5;
            var sizeY = 5;

            // Act
            var game = gameBuilder
                .WithFieldSize(sizeX, sizeY)
                .WithShipAtPositionOnAttackerField(positionX, positionY, shipLength, Orientation.Horizontal)
                .WithShipAtPositionOnDefenderField(positionX, positionY, shipLength, Orientation.Horizontal)
                .Build();

            // Assert
            Assert.IsNotNull(game);
            Assert.IsNotNull(game.FirstPlayer.OwnField);
            Assert.IsNotNull(game.SecondPlayer.OwnField);
        }
        
        [Test]
        public void Game_AttackerMissed_SwapFields()
        {
            // Arrange
            var gameBuilder = new GameBuilder();
            var positionX = 2;
            var positionY = 2;
            var shipLength = 1;
            var sizeX = 5;
            var sizeY = 5;

            // Act
            var game = gameBuilder
                .WithFieldSize(sizeX, sizeY)
                .WithShipAtPositionOnAttackerField(positionX, positionY, shipLength, Orientation.Horizontal)
                .WithShipAtPositionOnDefenderField(positionX, positionY, shipLength, Orientation.Horizontal)
                .Build();
            var attacker = game.AttackerId;
            var defender = game.DefenderId;
            game.Next(new AttackByPositionCommand(0, 0, game.FirstPlayer.PlayerId));

            // Assert
            Assert.IsNotNull(game);
            Assert.AreEqual(attacker, game.DefenderId);
            Assert.AreEqual(defender, game.AttackerId);
        }
    }
}