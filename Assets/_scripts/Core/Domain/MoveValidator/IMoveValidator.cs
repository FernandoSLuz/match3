using MatchTree.Core.Domain.BoardState;

namespace MatchTree.Core.Domain.MoveValidator
{
    public interface IMoveValidator
    {
        bool IsValidSwap(Board board, BoardPosition a, BoardPosition b);
    }
}


