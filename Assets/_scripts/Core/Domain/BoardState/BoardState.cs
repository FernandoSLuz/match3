using System.Collections.Generic;
using MatchTree.Core.Domain.TileModel;

namespace MatchTree.Core.Domain.BoardState
{
    public class Board
    {
        private readonly Tile[,] grid;

        public int Width { get; private set; }
        public int Height { get; private set; }

        public Board(int width, int height)
        {
            Width = width;
            Height = height;
            grid = new Tile[width, height];
        }

        public bool IsInside(int x, int y)
        {
            return x >= 0 && x < Width && y >= 0 && y < Height;
        }

        public Tile GetAt(int x, int y)
        {
            return grid[x, y];
        }

        public void SetAt(int x, int y, Tile tile)
        {
            grid[x, y] = tile;
        }

        public void ClearAt(int x, int y)
        {
            grid[x, y] = null;
        }

        public void Swap(BoardPosition a, BoardPosition b)
        {
            var tmp = grid[a.X, a.Y];
            grid[a.X, a.Y] = grid[b.X, b.Y];
            grid[b.X, b.Y] = tmp;
        }

        public IEnumerable<BoardPosition> AllPositions()
        {
            for (var y = 0; y < Height; y++)
            {
                for (var x = 0; x < Width; x++)
                {
                    yield return new BoardPosition(x, y);
                }
            }
        }

        public int RemovePositions(IEnumerable<BoardPosition> positions)
        {
            var removed = 0;
            foreach (var pos in positions)
            {
                if (grid[pos.X, pos.Y] != null)
                {
                    grid[pos.X, pos.Y] = null;
                    removed++;
                }
            }
            return removed;
        }

        public System.Collections.Generic.List<TileRemovalInfo> RemovePositionsWithInfo(IEnumerable<BoardPosition> positions)
        {
            var infos = new System.Collections.Generic.List<TileRemovalInfo>();
            foreach (var pos in positions)
            {
                var tile = grid[pos.X, pos.Y];
                if (tile != null)
                {
                    infos.Add(new TileRemovalInfo(tile, pos));
                    grid[pos.X, pos.Y] = null;
                }
            }
            return infos;
        }
    }
}


