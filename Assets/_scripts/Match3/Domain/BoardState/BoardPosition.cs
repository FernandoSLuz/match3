namespace Lighthouse.Match3.Domain.BoardState
{
    public struct BoardPosition
    {
        public int X;
        public int Y;

        public BoardPosition(int x, int y)
        {
            X = x;
            Y = y;
        }

        public override string ToString()
        {
            return $"({X},{Y})";
        }

        public override int GetHashCode()
        {
            return (X * 73856093) ^ (Y * 19349663);
        }

        public override bool Equals(object obj)
        {
            if (obj is BoardPosition other)
            {
                return other.X == X && other.Y == Y;
            }
            return false;
        }
    }
}


