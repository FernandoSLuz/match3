using Lighthouse.Match3.Domain.BoardState;

namespace Lighthouse.Match3.Domain.Resolver
{
    public interface IGravity
    {
        // Returns list of moves applied by gravity for animation purposes
        System.Collections.Generic.List<TileMove> Apply(Board board);
    }
}


