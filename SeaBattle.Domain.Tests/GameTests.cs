using System.Linq;
using NUnit.Framework;
using SeaBattle.Domain.Builders;
using SeaBattle.Domain.Commands;
using SeaBattle.Domain.Enums;

namespace SeaBattle.Domain.Tests
{
    public class GameTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void CallNext_Missed_OneCellAffected()
        {
            // Arrange
            var game = new GameBuilder()
                .WithFieldSize(5, 5)
                .WithShipAtPositionOnAttackerField(2, 2, 1, Orientation.Horizontal)
                .Build();

            // Act
            var cells = game.Next(new AttackByPositionCommand(0,0, game.AttackerField.FieldId)).ToArray();

            // Assert
            Assert.AreEqual(1, cells.Length);
            Assert.AreEqual(CellType.Empty, cells[0].CellType);
        }
        
        [Test]
        public void CallNext_CellKilled_NineCellsAffected()
        {
            // Arrange
            var game = new GameBuilder()
                .WithFieldSize(5, 5)
                .WithShipAtPositionOnAttackerField(2, 2, 2, Orientation.Horizontal)
                .WithShipAtPositionOnDefenderField(2, 2, 2, Orientation.Horizontal)
                .Build();

            // Act
            var cells = game.Next(new AttackByPositionCommand(2, 2, game.AttackerField.FieldId)).ToArray();

            // Assert
            Assert.AreEqual(1, cells.Length);
            Assert.AreEqual(CellType.Ship, cells[0].CellType);
        }
        
        [Test]
        public void CallNext_ShipKilled_NineCellsAffected()
        {
            // Arrange
            var game = new GameBuilder()
                .WithFieldSize(5, 5)
                .WithShipAtPositionOnAttackerField(2, 2, 1, Orientation.Horizontal)
                .WithShipAtPositionOnDefenderField(2, 2, 1, Orientation.Horizontal)
                .Build();

            // Act
            var cells = game.Next(new AttackByPositionCommand(2,2, game.AttackerField.FieldId)).ToArray();

            // Assert
            Assert.AreEqual(9, cells.Length);
        }

        [Test]
        public void HitTheShipCell_NextPositionGenerated()
        {
            // Arrange
            var game = new GameBuilder()
                .WithFieldSize(3, 4)
                .WithShipAtPositionOnDefenderField(1, 1, 2, Orientation.Horizontal)
                .Build();
            
            // Act
            game.Next(new AttackByPositionCommand(1, 1, game.AttackerField.FieldId));
            
            // Assert
            Assert.IsNotEmpty(game.DefenderField.NextPositions);
            Assert.IsEmpty(game.AttackerField.NextPositions);
        }
        
        [Test]
        public void MissedTheShipCell_NextPositionNotGenerated()
        {
            // Arrange
            var game = new GameBuilder()
                .WithFieldSize(3, 4)
                .WithShipAtPositionOnDefenderField(1, 1, 2, Orientation.Horizontal)
                .Build();
            
            // Act
            game.Next(new AttackByPositionCommand(0, 0, game.AttackerField.FieldId));
            
            // Assert
            Assert.IsEmpty(game.DefenderField.NextPositions);
            Assert.IsEmpty(game.AttackerField.NextPositions);
        }
    }
}