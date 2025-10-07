using MatchTree.Core.Domain.BoardState;
using MatchTree.Core.Domain.TileModel;

namespace MatchTree.Core.Domain.Resolver
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

 

