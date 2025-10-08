using Lighthouse.Match3.Domain.BoardState;
using Lighthouse.Match3.Domain.TileModel;

namespace Lighthouse.Match3.Domain.Resolver
{
    public struct TileMove
    {
        public Tile Tile;
        public BoardPosition From;
        public BoardPosition To;

        public TileMove(Tile tile, BoardPosition from, BoardPosition to)
        {
            Tile = tile;
            From = from;
            To = to;
        }
    }
}

 

