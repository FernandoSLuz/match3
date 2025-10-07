using MatchTree.Core.Domain.BoardState;

namespace MatchTree.Core.Domain.Resolver
{
    public interface IGravity
    {
        // Returns number of tiles moved
        int Apply(Board board);
    }
}


