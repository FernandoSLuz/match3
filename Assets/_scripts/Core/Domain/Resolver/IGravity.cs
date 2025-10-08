using MatchTree.Core.Domain.BoardState;

namespace MatchTree.Core.Domain.Resolver
{
    public interface IGravity
    {
        // Returns list of moves applied by gravity for animation purposes
        System.Collections.Generic.List<TileMove> Apply(Board board);
    }
}


