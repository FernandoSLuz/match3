using MatchTree.Core.Domain.BoardState;

namespace MatchTree.Core.Domain.Resolver
{
    public class VerticalGravity : IGravity
    {
        public System.Collections.Generic.List<TileMove> Apply(Board board)
        {
            var moves = new System.Collections.Generic.List<TileMove>();
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
                            var from = new BoardPosition(x, y);
                            var to = new BoardPosition(x, writeY);
                            board.SetAt(x, writeY, tile);
                            board.ClearAt(x, y);
                            moves.Add(new TileMove(tile, from, to));
                        }
                        writeY++;
                    }
                }
            }
            return moves;
        }
    }
}


