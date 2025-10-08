using Lighthouse.Match3.Domain.TileModel;

namespace Lighthouse.Match3.Domain.BoardState
{
    public struct TileRemovalInfo
    {
        public Tile Tile;
        public BoardPosition Position;

        public TileRemovalInfo(Tile tile, BoardPosition position)
        {
            Tile = tile;
            Position = position;
        }
    }
}

 

