using System;
using System.Linq;
using NUnit.Framework;
using SeaBattle.Domain.Builders;
using SeaBattle.Domain.Commands;
using SeaBattle.Domain.Enums;

namespace SeaBattle.Domain.Tests
{
    public class AttackPositionCommandTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void ShipOneCellLengthAttacked_NextPositionsNotGenerated()
        {
            // Arrange
            var attacker = new PlayerBuilder()
                .WithField(3, 3)
                .Build();
            var defender = new PlayerBuilder()
                .WithField(3, 3)
                .WithShipAtPos(1, 1, 1, Orientation.Horizontal)
                .Build();

            // Act
            var result = new AttackByPositionCommand(1, 1, attacker.PlayerId)
                .Execute(attacker, defender, out _);

            // Assert
            Assert.IsTrue(result);
            Assert.IsEmpty(attacker.NextPositions);
        }
        
        

        [Test]
        public void ShipTwoCellLengthAttacked_NextPositionsGenerated()
        {
            // Arrange
            var attacker = new PlayerBuilder()
                .WithField(3, 3)
                .Build();
            var defender = new PlayerBuilder()
                .WithField(3, 3)
                .WithShipAtPos(1, 1, 2, Orientation.Horizontal)
                .Build();

            // Act
            var result = new AttackByPositionCommand(1, 1, attacker.PlayerId)
                .Execute(attacker, defender, out _);

            // Assert
            Assert.IsTrue(result);
            Assert.IsNotEmpty(attacker.NextPositions);
        }
        
        [Test]
        public void EmptyCellAttacked_NextPositionsNotGenerated()
        {
            // Arrange
            var attacker = new PlayerBuilder()
                .WithField(3, 3)
                .Build();
            var defender = new PlayerBuilder()
                .WithField(3, 3)
                .WithShipAtPos(1, 1, 1, Orientation.Horizontal)
                .Build();

            // Act
            var result = new AttackByPositionCommand(0, 0, attacker.PlayerId)
                .Execute(attacker, defender, out _);

            // Assert
            Assert.IsFalse(result);
            Assert.IsEmpty(attacker.NextPositions);
        }

        [Test]
        public void ShipCellAttacked_NextPositionsGenerated_RndCmdSelectPosFromListOfNextPos()
        {
            // Arrange
            var attacker = new PlayerBuilder()
                .WithField(3, 3)
                .Build();
            var defender = new PlayerBuilder()
                .WithField(3, 3)
                .WithShipAtPos(1, 1, 2, Orientation.Horizontal)
                .Build();
            new AttackByPositionCommand(1, 1, attacker.PlayerId)
                .Execute(attacker, defender, out _);

            // Act
            new AttackByRandomPositionCommand(attacker.PlayerId)
                .Execute(attacker, defender, out var changesList);

            // Assert
            Assert.IsTrue(new[]
            {
                (x: 0, y: 1),
                (x: 1, y: 0), (x: 1, y: 2),
                (x: 2, y: 1)
            }.Intersect(changesList
                .First(changes => changes.PlayerId == defender.PlayerId && changes.FieldId == defender.OwnField.FieldId)
                .AffectedCells.Select(cell => (x: cell.X, y: cell.Y))).Any());
        }

        [Test]
        public void ShipTwoCellsDestroyed_PreviousHitsCleared()
        {
            // Arrange
            var attacker = new PlayerBuilder()
                .WithField(5, 5)
                .Build();
            var defender = new PlayerBuilder()
                .WithField(5, 5)
                .WithShipAtPos(0, 0, 3, Orientation.Horizontal)
                .WithShipAtPos(4, 0, 2, Orientation.Horizontal)
                .Build();

            // Act
            new AttackByPositionCommand(0, 0, attacker.PlayerId)
                .Execute(attacker, defender, out _);
            new AttackByPositionCommand(0, 1, attacker.PlayerId)
                .Execute(attacker, defender, out _);
            new AttackByPositionCommand(0, 2, attacker.PlayerId)
                .Execute(attacker, defender, out _);

            // Assert
            Assert.IsEmpty(attacker.PreviousHits);
        }

        [Test]
        public void ShipTwoCellsAttacked_NextPositionsGenerated()
        {
            // Arrange
            var attacker = new PlayerBuilder()
                .WithField(5, 5)
                .Build();
            var defender = new PlayerBuilder()
                .WithField(5, 5)
                .WithShipAtPos(0, 0, 3, Orientation.Horizontal)
                .WithShipAtPos(4, 0, 2, Orientation.Horizontal)
                .Build();

            // Act
            new AttackByPositionCommand(0, 0, attacker.PlayerId)
                .Execute(attacker, defender, out _);
            new AttackByPositionCommand(0, 1, attacker.PlayerId)
                .Execute(attacker, defender, out _);
            new AttackByPositionCommand(0, 2, attacker.PlayerId)
                .Execute(attacker, defender, out _);
            new AttackByPositionCommand(4, 0, attacker.PlayerId)
                .Execute(attacker, defender, out _);

            // Assert
            Assert.AreEqual(new[] {new Position(x: 3, y: 0), new Position(x: 4, y: 1)}, attacker.NextPositions);
        }
    }
}