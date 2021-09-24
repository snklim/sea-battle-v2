using System;
using System.Collections.Generic;
using System.Linq;
using SeaBattle.Domain.Enums;

namespace SeaBattle.Domain.Builders
{
    public class FieldBuilder
    {
        private Field _field;
        private List<(int x, int y)> _availablePositions;

        private readonly Random _random = new Random();
        private readonly Orientation[] _orientations = {Orientation.Horizontal, Orientation.Vertical};

        public FieldBuilder WithSize(int sizeX, int sizeY)
        {
            _field = new Field(sizeX, sizeY);
            _availablePositions = new List<(int x, int y)>(_field.GetCells().Select(cell => (cell.X, cell.Y)));
            return this;
        }

        public FieldBuilder WithShip(int length)
        {
            var allCombinations = _availablePositions
                .Join(_orientations, position => true, orientation => true,
                    (position, orientation) => (position, orientation))
                .ToList();
            while (allCombinations.Count > 0)
            {
                var combination = allCombinations[_random.Next(allCombinations.Count)];
                if (TryGenerateShip(combination.position.x, combination.position.y, length, combination.orientation,
                    out var ship, out var border))
                {
                    PlaceShipOnField(ship, border);
                    
                    return this;
                }

                allCombinations.Remove(combination);
            }

            throw new ApplicationException("Cannot find place for ship");
        }

        public FieldBuilder WithShipAtPosition(int x, int y, int length,
            Orientation orientation = Orientation.Horizontal)
        {
            if (!TryGenerateShip(x, y, length, orientation, out var ship, out var border))
            {
                throw new ApplicationException("Position is invalid");
            }
            
            PlaceShipOnField(ship, border);
            
            return this;
        }

        private void PlaceShipOnField(List<(int x, int y)> ship, List<(int x, int y)> border)
        {
            var shipDetails = new ShipDetails();
            border.ForEach(cell =>
            {
                var borderCell = new BorderCell(cell.x, cell.y, _field.FieldId);
                _field[cell.x, cell.y] = borderCell;
                shipDetails.Border.Add(borderCell);
                _availablePositions.Remove(cell);
            });
            ship.ForEach(cell =>
            {
                var shipCell = new ShipCell(cell.x, cell.y, _field.FieldId, shipDetails);
                _field[cell.x, cell.y] = shipCell;
                shipDetails.Ship.Add(shipCell);
                _availablePositions.Remove(cell);
            });
        }

        private bool TryGenerateShip(int x, int y, int length, Orientation orientation,
            out List<(int x, int y)> ship, out List<(int x, int y)> border)
        {
            var borderLengthX = orientation == Orientation.Vertical ? length + 2 : 3;
            var borderLengthY = orientation == Orientation.Horizontal ? length + 2 : 3;
            var shipLengthX = orientation == Orientation.Vertical ? length : 1;
            var shipLengthY = orientation == Orientation.Horizontal ? length : 1;

            ship = GeneratePositions(x, y, shipLengthX, shipLengthY)
                .ToList();

            border = GeneratePositions(x - 1, y - 1, borderLengthX, borderLengthY)
                .Where(cell => _field.IsPositionValid(cell.x, cell.y))
                .Except(ship)
                .ToList();

            return ship.All(cell => _field.IsPositionValid(cell.x, cell.y) 
                                    && _field[cell.x, cell.y].CellType == CellType.Empty);
        }

        private IEnumerable<(int x, int y)> GeneratePositions(int startX, int startY, int lengthX, int lengthY)
        {
            return Enumerable.Range(startX, lengthX)
                .Join(Enumerable.Range(startY, lengthY),
                    x => true,
                    y => true,
                    (x, y) => (x, y));
        }

        public Field Build()
        {
            return _field;
        }
    }
}