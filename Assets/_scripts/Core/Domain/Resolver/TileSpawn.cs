using MatchTree.Core.Domain.BoardState;
using MatchTree.Core.Domain.TileModel;

namespace MatchTree.Core.Domain.Resolver
{
    public struct TileSpawn
    {
        public Tile Tile;
        public BoardPosition Position;

        public TileSpawn(Tile tile, BoardPosition position)
        {
            Tile = tile;
            Position = position;
        }
    }
}

 

