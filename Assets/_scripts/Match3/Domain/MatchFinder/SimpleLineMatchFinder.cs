using System.Collections.Generic;
using Lighthouse.Match3.Domain.BoardState;
using Lighthouse.Match3.Domain.TileModel;

namespace Lighthouse.Match3.Domain.MatchFinder
{
    public class SimpleLineMatchFinder : IMatchFinder
    {
        public List<HashSet<BoardPosition>> FindMatches(Board board)
        {
            var results = new List<HashSet<BoardPosition>>();

            // Horizontal
            for (var y = 0; y < board.Height; y++)
            {
                var run = new List<BoardPosition>();
                TileColor? current = null;
                for (var x = 0; x < board.Width; x++)
                {
                    var tile = board.GetAt(x, y);
                    var color = tile?.Color ?? TileColor.None;
                    if (tile != null && color != TileColor.None && color == current)
                    {
                        run.Add(new BoardPosition(x, y));
                    }
                    else
                    {
                        if (run.Count >= 3)
                        {
                            results.Add(new HashSet<BoardPosition>(run));
                        }
                        run.Clear();
                        if (tile != null && color != TileColor.None)
                        {
                            current = color;
                            run.Add(new BoardPosition(x, y));
                        }
                        else
                        {
                            current = null;
                        }
                    }
                }
                if (run.Count >= 3) results.Add(new HashSet<BoardPosition>(run));
            }

            // Vertical
            for (var x = 0; x < board.Width; x++)
            {
                var run = new List<BoardPosition>();
                TileColor? current = null;
                for (var y = 0; y < board.Height; y++)
                {
                    var tile = board.GetAt(x, y);
                    var color = tile?.Color ?? TileColor.None;
                    if (tile != null && color != TileColor.None && color == current)
                    {
                        run.Add(new BoardPosition(x, y));
                    }
                    else
                    {
                        if (run.Count >= 3)
                        {
                            results.Add(new HashSet<BoardPosition>(run));
                        }
                        run.Clear();
                        if (tile != null && color != TileColor.None)
                        {
                            current = color;
                            run.Add(new BoardPosition(x, y));
                        }
                        else
                        {
                            current = null;
                        }
                    }
                }
                if (run.Count >= 3) results.Add(new HashSet<BoardPosition>(run));
            }

            return results;
        }
    }
}


