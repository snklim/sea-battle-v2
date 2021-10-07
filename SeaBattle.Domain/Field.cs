using System;
using System.Collections.Generic;
using System.Linq;
using SeaBattle.Domain.Cells;
using SeaBattle.Domain.Enums;

namespace SeaBattle.Domain
{
    public class Field
    {
        private Cell[,] Cells { get; }
        public int SizeX { get; }
        public int SizeY { get; }
        public Guid FieldId { get; } = Guid.NewGuid();
        public List<(int x, int y)> NextPositions { get; } = new();
        public List<(int x, int y)> PreviousHits { get; } = new();

        public Field(int sizeX, int sizeY)
        {
            Cells = new Cell[sizeX, sizeY];
            SizeX = sizeX;
            SizeY = sizeY;
            for (var x = 0; x < sizeX; x++)
            for (var y = 0; y < sizeY; y++)
            {
                Cells[x, y] = new EmptyCell(x, y, FieldId);
            }
        }
        
        public Cell this[int x,int y]
        {
            get
            {
                if (IsPositionValid(x, y))
                {
                    return Cells[x, y];
                }

                throw new ApplicationException("Position is invalid");
            }
            set
            {
                if (IsPositionValid(x,y))
                {
                    Cells[x, y] = value;
                    return;
                }

                throw new ApplicationException("Position is invalid");
            }
        }

        public bool IsPositionValid(int x, int y)
        {
            return 0 <= x && x < SizeX && 0 <= y && y < SizeY;
        }

        public IEnumerable<Cell> GetCells()
        {
            for (var x = 0; x < SizeX; x++)
            for (var y = 0; y < SizeY; y++)
            {
                yield return this[x, y];
            }
        }

        public List<(int x, int y)> AvailablePositions => GetCells()
            .Where(cell => !cell.Attacked)
            .Select(cell => (x: cell.X, y: cell.Y))
            .ToList();

        public bool AllShipsDestroyed => GetCells()
            .Where(cell => cell.CellType == CellType.Ship)
            .All(cell => cell.IsShipDestroyed);
        
        public bool Attack(int posX, int posY, out IEnumerable<Cell> affectedCells)
        {
            var affectedCell = this[posX, posY];
            affectedCells = affectedCell.Attack();
            var hit = affectedCell.CellType == CellType.Ship;

            affectedCells.Select(cell => (x: cell.X, y: cell.Y))
                .ToList()
                .ForEach(pos =>
                {
                    NextPositions.Remove(pos);
                    PreviousHits.Remove(pos);
                });

            if (hit && !affectedCell.IsShipDestroyed)
            {
                GenerateNextPositions(posX, posY);
            }

            return hit;
        }

        private void GenerateNextPositions(int positionX, int positionY)
        {
            var nextPositions = AvailablePositions
                .Intersect(new[]
                {
                    (x: positionX - 1, y: positionY),
                    (x: positionX, y: positionY + 1),
                    (x: positionX + 1, y: positionY),
                    (x: positionX, y: positionY - 1)
                });
                    
            PreviousHits.Add((x: positionX, y: positionY));
                    
            NextPositions.AddRange(nextPositions);

            if (PreviousHits.Count >= 2)
            {
                var positionPairs = PreviousHits
                    .Join(PreviousHits, pos => true, pos => true,
                        (pos1, pos2) => (pos1, pos2))
                    .ToList();
                var filteredNextPositions = positionPairs.All(x => x.pos1.x == x.pos2.x)
                    ? NextPositions.Where(pos => pos.x == positionPairs.First().pos1.x).ToList()
                    : NextPositions.Where(pos => pos.y == positionPairs.First().pos1.y).ToList();

                NextPositions.Clear();
                NextPositions.AddRange(filteredNextPositions);
            }
        }
    }
}