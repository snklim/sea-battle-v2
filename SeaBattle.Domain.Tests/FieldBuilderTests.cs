using System;
using System.Linq;
using NUnit.Framework;
using SeaBattle.Domain.Builders;
using SeaBattle.Domain.Enums;

namespace SeaBattle.Domain.Tests
{
    public class FieldBuilderTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void CreateFieldWithSize_FieldWithSizeCreated()
        {
            // Arrange
            var fieldBuilder = new FieldBuilder();
            var sizeX = 2;
            var sizeY = 3;

            // Act
            var field = fieldBuilder
                .WithSize(sizeX, sizeY)
                .Build();

            // Assert
            Assert.AreEqual(sizeX, field.SizeX);
            Assert.AreEqual(sizeY, field.SizeY);
        }

        [Test]
        public void CreateFieldWithShip_ShipOneLength_FieldWithShipOneLengthCreated()
        {
            // Arrange
            var fieldBuilder = new FieldBuilder();
            var sizeX = 5;
            var sizeY = 5;
            var shipLength = 1;
            var posX = 2;
            var posY = 2;

            // Act
            var field = fieldBuilder
                .WithSize(sizeX, sizeY)
                .WithShipAtPosition(posX, posY, shipLength)
                .Build();

            // Assert
            Assert.AreEqual(CellType.Ship, field[posX, posY].CellType);
            Assert.IsTrue(new[]
            {
                field[posX - 1, posY - 1], field[posX - 1, posY], field[posX - 1, posY + 1],
                field[posX, posY - 1], field[posX, posY + 1],
                field[posX + 1, posY - 1], field[posX + 1, posY], field[posX + 1, posY + 1],
            }.All(cell => cell.CellType == CellType.Border));
            Assert.IsTrue(new[]
            {
                field[0, 0], field[0, 1], field[0, 2], field[0, 3], field[0, 4],
                field[1, 0], field[1, 4],
                field[2, 0], field[2, 4],
                field[3, 0], field[3, 4],
                field[4, 0], field[4, 1], field[4, 2], field[4, 3], field[4, 4]
            }.All(cell => cell.CellType == CellType.Empty));
        }
        
        [Test]
        public void CreateFieldWithShip_ShipTwoCellLength_FieldWithShipTwoCellLengthCreated()
        {
            // Arrange
            var fieldBuilder = new FieldBuilder();
            var sizeX = 3;
            var sizeY = 4;
            var shipLength = 2;
            var posX = 1;
            var posY = 1;

            // Act
            var field = fieldBuilder
                .WithSize(sizeX, sizeY)
                .WithShipAtPosition(posX, posY, shipLength)
                .Build();

            // Assert
            Assert.IsTrue(new[] {field[posX, posY], field[posX, posY + 1]}.All(cell => cell.CellType == CellType.Ship));
            Assert.IsTrue(new[]
            {
                field[posX - 1, posY - 1], field[posX - 1, posY], field[posX - 1, posY + 1], field[posX - 1, posY + 2],
                field[posX, posY - 1], field[posX, posY + 2],
                field[posX + 1, posY - 1], field[posX + 1, posY], field[posX + 1, posY + 1], field[posX + 1, posY + 2]
            }.All(cell => cell.CellType == CellType.Border));
        }

        [Test]
        public void CreateFieldWithTwoShip_ShipsNotIntersection_FieldWithTwoShipCreated()
        {
            // Arrange
            var fieldBuilder = new FieldBuilder();
            var sizeX = 5;
            var sizeY = 5;
            var shipLength = 1;
            var posX = 0;
            var posY = 0;
            var posShip2X = 4;
            var posShip2Y = 4;
            
            // Act
            var field = fieldBuilder
                .WithSize(sizeX, sizeY)
                .WithShipAtPosition(posX, posY, shipLength)
                .WithShipAtPosition(posShip2X, posShip2Y, shipLength)
                .Build();
            
            // Assert
            Assert.AreEqual(CellType.Ship, field[posX, posY].CellType);
            Assert.AreEqual(CellType.Ship, field[posShip2X, posShip2Y].CellType);
        }
        
        [Test]
        public void CreateFieldWithTwoShip_ShipsIntersection_FieldWithTwoShipNotCreated()
        {
            // Arrange
            var fieldBuilder = new FieldBuilder();
            var sizeX = 5;
            var sizeY = 5;
            var shipLength = 1;
            var posX = 2;
            var posY = 2;
            var posShip2X = 3;
            var posShip2Y = 3;
            
            // Act
            fieldBuilder
                .WithSize(sizeX, sizeY)
                .WithShipAtPosition(posX, posY, shipLength);
            
            // Assert
            Assert.Throws<ApplicationException>(() =>
                fieldBuilder.WithShipAtPosition(posShip2X, posShip2Y, shipLength));
        }

        [Test]
        public void CallWithShip_ShipTwoLengthAndFieldTwoLength_ShipPlaced()
        {
            // Arrange
            var fieldBuilder = new FieldBuilder();
            var sizeX = 2;
            var sizeY = 1;
            var shipLength = 2;
            
            // Act
            fieldBuilder
                .WithSize(sizeX, sizeY);

            // Assert
            Assert.DoesNotThrow(() => fieldBuilder.WithShip(shipLength));
        }

        [Test]
        public void CallWithShip_ShipTwoLengthAndFieldOneLength_ShipNotPlaced()
        {
            // Arrange
            var fieldBuilder = new FieldBuilder();
            var sizeX = 1;
            var sizeY = 1;
            var shipLength = 2;
            
            // Act
            fieldBuilder
                .WithSize(sizeX, sizeY);

            // Assert
            Assert.Throws<ApplicationException>(() => fieldBuilder.WithShip(shipLength));
        }

        [Test]
        public void CallWithShip_10ShipsAndField10CellLength_FieldCreated()
        {
            // Arrange
            var fieldBuilder = new FieldBuilder();
            var sizeX = 10;
            var sizeY = 10;
            var ships = new[] {4, 3, 3, 2, 2, 2, 1, 1, 1, 1};
            
            // Act
            fieldBuilder
                .WithSize(sizeX, sizeY);

            // Assert
            Assert.DoesNotThrow(() =>
            {
                foreach (var ship in ships)
                {
                    fieldBuilder = fieldBuilder.WithShip(ship);
                }
            });
        }
    }
}