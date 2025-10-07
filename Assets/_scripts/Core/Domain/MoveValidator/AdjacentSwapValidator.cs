using MatchTree.Core.Domain.BoardState;
using MatchTree.Core.Domain.MatchFinder;

namespace MatchTree.Core.Domain.MoveValidator
{
    public class AdjacentSwapValidator : IMoveValidator
    {
        private readonly IMatchFinder matchFinder;

        public AdjacentSwapValidator(IMatchFinder matchFinder)
        {
            this.matchFinder = matchFinder;
        }

        public bool IsValidSwap(Board board, BoardPosition a, BoardPosition b)
        {
            var dx = System.Math.Abs(a.X - b.X);
            var dy = System.Math.Abs(a.Y - b.Y);
            if (dx + dy != 1) return false;

            board.Swap(a, b);
            var matches = matchFinder.FindMatches(board);
            board.Swap(a, b);

            return matches.Count > 0;
        }
    }
}


