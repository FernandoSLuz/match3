using MatchTree.Core.Domain.BoardState;

namespace MatchTree.Core.Domain.Resolver
{
    public class VerticalGravity : IGravity
    {
        public int Apply(Board board)
        {
            var moved = 0;
            for (var x = 0; x < board.Width; x++)
            {
                var writeY = 0;
                for (var y = 0; y < board.Height; y++)
                {
                    var tile = board.GetAt(x, y);
                    if (tile != null)
                    {
                        if (y != writeY)
                        {
                            board.SetAt(x, writeY, tile);
                            board.ClearAt(x, y);
                            moved++;
                        }
                        writeY++;
                    }
                }
            }
            return moved;
        }
    }
}


