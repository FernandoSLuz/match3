using Lighthouse.Match3.Domain.BoardState;
using Lighthouse.Match3.Domain.TileModel;

namespace Lighthouse.Match3.Domain.Resolver
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

 

