namespace SeaBattle.Domain
{
    public struct Position
    {
        public Position(int x, int y)
        {
            X = x;
            Y = y;
        }
        
        public int X { get; }
        public int Y { get; }
    }
}