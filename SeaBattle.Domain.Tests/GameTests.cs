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
            var attackerPlayerId = game.Attacker.PlayerId;
            var defenderPlayerId = game.Defender.PlayerId;
            var attackerFieldId = game.Attacker.EnemyField.FieldId;
            var defenderFieldId = game.Defender.OwnField.FieldId;

            // Act
            var changesList = game.Next(new AttackByPositionCommand(0,0, game.Attacker.PlayerId)).ToArray();

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
            var attackerFieldId = game.Attacker.EnemyField.FieldId;
            var defenderFieldId = game.Defender.OwnField.FieldId;

            // Act
            var changesList = game.Next(new AttackByPositionCommand(2, 2, game.Attacker.PlayerId)).ToArray();

            // Assert
            Assert.AreEqual(1, changesList
                .First(changes => changes.PlayerId == game.Attacker.PlayerId && 
                                  changes.FieldId == attackerFieldId).AffectedCells
                .Count);
            Assert.AreEqual(1, changesList
                .First(changes => changes.PlayerId == game.Defender.PlayerId && 
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
            var attackerFieldId = game.Attacker.EnemyField.FieldId;
            var defenderFieldId = game.Defender.OwnField.FieldId;

            // Act
            var changesList = game.Next(new AttackByPositionCommand(2,2, game.Attacker.PlayerId)).ToArray();

            // Assert
            Assert.AreEqual(9, changesList
                .First(changes => changes.PlayerId == game.Attacker.PlayerId && 
                                  changes.FieldId == attackerFieldId).AffectedCells
                .Count);
            Assert.AreEqual(9, changesList
                .First(changes => changes.PlayerId == game.Defender.PlayerId && 
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
            game.Next(new AttackByPositionCommand(1, 1, game.Attacker.PlayerId));
            
            // Assert
            Assert.IsNotEmpty(game.Attacker.NextPositions);
            Assert.IsEmpty(game.Defender.NextPositions);
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
            game.Next(new AttackByPositionCommand(0, 0, game.Attacker.PlayerId));
            
            // Assert
            Assert.IsEmpty(game.Defender.NextPositions);
            Assert.IsEmpty(game.Attacker.NextPositions);
        }
    }
}