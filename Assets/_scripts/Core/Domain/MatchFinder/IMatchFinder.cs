using System.Collections.Generic;
using MatchTree.Core.Domain.BoardState;

namespace MatchTree.Core.Domain.MatchFinder
{
    public interface IMatchFinder
    {
        // Returns sets of positions that make valid matches
        List<HashSet<BoardPosition>> FindMatches(Board board);
    }
}


