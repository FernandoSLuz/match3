using Lighthouse.Match3.Domain.BoardState;

namespace Lighthouse.Match3.Domain.MoveValidator
{
    public interface IMoveValidator
    {
        bool IsValidSwap(Board board, BoardPosition a, BoardPosition b);
    }
}


