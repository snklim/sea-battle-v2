using System.Linq;
using NUnit.Framework;
using SeaBattle.Domain.Builders;
using SeaBattle.Domain.Commands;

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
            var field = new FieldBuilder()
                .WithSize(3, 3)
                .WithShipAtPosition(1, 1, 1)
                .Build();

            // Act
            var result = new AttackByPositionCommand(1, 1, field.FieldId)
                .Execute(field, out var affectedCell);

            // Assert
            Assert.IsTrue(result);
            Assert.IsEmpty(field.NextPositions);
        }
        
        

        [Test]
        public void ShipTwoCellLengthAttacked_NextPositionsGenerated()
        {
            // Arrange
            var field = new FieldBuilder()
                .WithSize(3, 3)
                .WithShipAtPosition(1, 1, 2)
                .Build();

            // Act
            var result = new AttackByPositionCommand(1, 1, field.FieldId)
                .Execute(field, out var affectedCell);

            // Assert
            Assert.IsTrue(result);
            Assert.IsNotEmpty(field.NextPositions);
        }
        
        [Test]
        public void EmptyCellAttacked_NextPositionsNotGenerated()
        {
            // Arrange
            var field = new FieldBuilder()
                .WithSize(3, 3)
                .WithShipAtPosition(1, 1, 1)
                .Build();

            // Act
            var result = new AttackByPositionCommand(0, 0, field.FieldId)
                .Execute(field, out var affectedCell);

            // Assert
            Assert.IsFalse(result);
            Assert.IsEmpty(field.NextPositions);
        }

        [Test]
        public void ShipCellAttacked_NextPositionsGenerated_RndCmdSelectPosFromListOfNextPos()
        {
            // Arrange
            var field = new FieldBuilder()
                .WithSize(3, 3)
                .WithShipAtPosition(1, 1, 2)
                .Build();
            new AttackByPositionCommand(1, 1, field.FieldId)
                .Execute(field, out _);

            // Act
            new AttackByRandomPositionCommand(field.FieldId)
                .Execute(field, out var affectedCell);

            // Assert
            Assert.IsTrue(new[]
            {
                (x: 0, y: 1),
                (x: 1, y: 0), (x: 1, y: 2),
                (x: 2, y: 1)
            }.Intersect(affectedCell.Select(cell => (x: cell.X, y: cell.Y))).Any());
        }
        
        [Test]
        public void ShipTwoCellsDestroyed_PreviousHitsCleared()
        {
            // Arrange
            var field = new FieldBuilder()
                .WithSize(5, 5)
                .WithShipAtPosition(0, 0, 3)
                .WithShipAtPosition(4, 0, 2)
                .Build();

            // Act
            new AttackByPositionCommand(0, 0, field.FieldId)
                .Execute(field, out _);
            new AttackByPositionCommand(0, 1, field.FieldId)
                .Execute(field, out _);
            new AttackByPositionCommand(0, 2, field.FieldId)
                .Execute(field, out _);

            // Assert
            Assert.IsEmpty(field.PreviousHits);
        }
        
        [Test]
        public void ShipTwoCellsAttacked_NextPositionsGenerated()
        {
            // Arrange
            var field = new FieldBuilder()
                .WithSize(5, 5)
                .WithShipAtPosition(0, 0, 3)
                .WithShipAtPosition(4, 0, 2)
                .Build();

            // Act
            new AttackByPositionCommand(0, 0, field.FieldId)
                .Execute(field, out _);
            new AttackByPositionCommand(0, 1, field.FieldId)
                .Execute(field, out _);
            new AttackByPositionCommand(0, 2, field.FieldId)
                .Execute(field, out _);
            
            
            new AttackByPositionCommand(4, 0, field.FieldId)
                .Execute(field, out _);

            // Assert
            Assert.AreEqual(new[] {(x: 3, y: 0),(x: 4, y: 1)}, field.NextPositions);
        }
    }
}