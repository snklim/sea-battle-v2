using NUnit.Framework;

namespace SeaBattle.Domain.Tests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void CreateField_SetFieldSize()
        {
            // Arrange
            var sizeX = 2;
            var sizeY = 3;
            
            // Act
            var field = new Field(sizeX, sizeY);
            
            // Assert
            Assert.AreEqual(sizeX, field.SizeX);
            Assert.AreEqual(sizeY, field.SizeY);
        }
    }
}