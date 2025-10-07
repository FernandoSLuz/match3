using MatchTree.Core.Domain.BoardState;
using MatchTree.Core.Domain.TileModel;

namespace MatchTree.Core.Domain.Resolver
{
    public struct TileRemoval
    {
        public Tile Tile;
        public BoardPosition Position;

        public TileRemoval(Tile tile, BoardPosition position)
        {
            Tile = tile;
            Position = position;
        }
    }
}

 

