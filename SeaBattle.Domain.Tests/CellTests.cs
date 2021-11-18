using System;
using NUnit.Framework;
using SeaBattle.Domain.Enums;

namespace SeaBattle.Domain.Tests
{
    public class CellTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void CreateShipCell_WithPosition_CellCreatedAtPosition()
        {
            // Arrange
            var posX = 2;
            var posY = 3;
            var fieldId = Guid.NewGuid();

            // Act
            var cell = new Cell(fieldId, posX, posY) {CellType = CellType.Ship};

            // Assert
            Assert.AreEqual(posX, cell.X);
            Assert.AreEqual(posY, cell.Y);
            Assert.AreEqual(CellType.Ship, cell.CellType);
        }
    }
}