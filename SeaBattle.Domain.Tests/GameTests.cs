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
                .WithShipAtPositionOnDefenderField(2, 2, 1, Orientation.Horizontal)
                .Build();
            var attackerPlayerId = game.AttackerId;
            var defenderPlayerId = game.DefenderId;
            var (attackerFieldId, defenderFieldId) = game.AttackerId == game.FirstPlayer.PlayerId
                ? (game.FirstPlayer.EnemyField.FieldId, game.SecondPlayer.OwnField.FieldId)
                : (game.SecondPlayer.OwnField.FieldId, game.SecondPlayer.OwnField.FieldId);

            // Act
            var changesList = game.Next(new AttackByPositionCommand(0,0, game.FirstPlayer.PlayerId)).ToArray();

            // Assert
            Assert.AreEqual(1,
                changesList
                    .First(changes => changes.PlayerId == attackerPlayerId && 
                                      changes.FieldId == attackerFieldId).AffectedCells
                    .Count);
            Assert.AreEqual(1,
                changesList
                    .First(changes => changes.PlayerId == defenderPlayerId && 
                                      changes.FieldId == defenderFieldId).AffectedCells
                    .Count);
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
            var (attackerFieldId, defenderFieldId) = game.AttackerId == game.FirstPlayer.PlayerId
                ? (game.FirstPlayer.EnemyField.FieldId, game.SecondPlayer.OwnField.FieldId)
                : (game.SecondPlayer.OwnField.FieldId, game.SecondPlayer.OwnField.FieldId);

            // Act
            var changesList = game.Next(new AttackByPositionCommand(2, 2, game.FirstPlayer.PlayerId)).ToArray();

            // Assert
            Assert.AreEqual(1, changesList
                .First(changes => changes.PlayerId == game.FirstPlayer.PlayerId && 
                                  changes.FieldId == attackerFieldId).AffectedCells
                .Count);
            Assert.AreEqual(1, changesList
                .First(changes => changes.PlayerId == game.SecondPlayer.PlayerId && 
                                  changes.FieldId == defenderFieldId).AffectedCells
                .Count);
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
            var (attackerFieldId, defenderFieldId) = game.AttackerId == game.FirstPlayer.PlayerId
                ? (game.FirstPlayer.EnemyField.FieldId, game.SecondPlayer.OwnField.FieldId)
                : (game.SecondPlayer.OwnField.FieldId, game.SecondPlayer.OwnField.FieldId);

            // Act
            var changesList = game.Next(new AttackByPositionCommand(2,2, game.FirstPlayer.PlayerId)).ToArray();

            // Assert
            Assert.AreEqual(9, changesList
                .First(changes => changes.PlayerId == game.FirstPlayer.PlayerId && 
                                  changes.FieldId == attackerFieldId).AffectedCells
                .Count);
            Assert.AreEqual(9, changesList
                .First(changes => changes.PlayerId == game.SecondPlayer.PlayerId && 
                                  changes.FieldId == defenderFieldId).AffectedCells
                .Count);
        }

        [Test]
        public void HitTheShipCell_NextPositionGenerated()
        {
            // Arrange
            var game = new GameBuilder()
                .WithFieldSize(3, 4)
                .WithShipAtPositionOnAttackerField(1, 1, 2, Orientation.Horizontal)
                .WithShipAtPositionOnDefenderField(1, 1, 2, Orientation.Horizontal)
                .Build();
            
            // Act
            game.Next(new AttackByPositionCommand(1, 1, game.FirstPlayer.PlayerId));
            
            // Assert
            Assert.IsNotEmpty(game.FirstPlayer.NextPositions);
            Assert.IsEmpty(game.SecondPlayer.NextPositions);
        }
        
        [Test]
        public void MissedTheShipCell_NextPositionNotGenerated()
        {
            // Arrange
            var game = new GameBuilder()
                .WithFieldSize(3, 4)
                .WithShipAtPositionOnAttackerField(1, 1, 2, Orientation.Horizontal)
                .WithShipAtPositionOnDefenderField(1, 1, 2, Orientation.Horizontal)
                .Build();
            
            // Act
            game.Next(new AttackByPositionCommand(0, 0, game.FirstPlayer.PlayerId));
            
            // Assert
            Assert.IsEmpty(game.SecondPlayer.NextPositions);
            Assert.IsEmpty(game.FirstPlayer.NextPositions);
        }
    }
}