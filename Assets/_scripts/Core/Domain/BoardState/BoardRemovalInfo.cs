using MatchTree.Core.Domain.TileModel;

namespace MatchTree.Core.Domain.BoardState
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

 

