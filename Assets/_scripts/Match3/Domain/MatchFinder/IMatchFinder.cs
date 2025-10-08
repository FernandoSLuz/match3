using System.Collections.Generic;
using Lighthouse.Match3.Domain.BoardState;

namespace Lighthouse.Match3.Domain.MatchFinder
{
    public interface IMatchFinder
    {
        // Returns sets of positions that make valid matches
        List<HashSet<BoardPosition>> FindMatches(Board board);
    }
}


