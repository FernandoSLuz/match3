using System.Collections.Generic;
using MatchTree.Core.Domain.BoardState;
using MatchTree.Core.Domain.MatchFinder;
using MatchTree.Core.Domain.Spawner;

namespace MatchTree.Core.Domain.Resolver
{
    public class ResolverPipeline
    {
        private readonly IMatchFinder matchFinder;
        private readonly IGravity gravity;
        private readonly ISpawner spawner;

        public ResolverPipeline(IMatchFinder matchFinder, IGravity gravity, ISpawner spawner)
        {
            this.matchFinder = matchFinder;
            this.gravity = gravity;
            this.spawner = spawner;
        }

        public ResolutionResult Resolve(Board board)
        {
            var cascades = new List<CascadeResult>();

            while (true)
            {
                var matches = matchFinder.FindMatches(board);
                if (matches.Count == 0) break;

                var removedCount = 0;
                foreach (var group in matches)
                {
                    removedCount += board.RemovePositions(group);
                }

                var moved = gravity.Apply(board);
                var filled = Fill(board);
                cascades.Add(new CascadeResult(removedCount, moved, filled));
            }

            return new ResolutionResult(cascades);
        }

        private int Fill(Board board)
        {
            var spawned = 0;
            for (var x = 0; x < board.Width; x++)
            {
                for (var y = 0; y < board.Height; y++)
                {
                    if (board.GetAt(x, y) == null)
                    {
                        var tile = spawner.CreateTile(x, y);
                        board.SetAt(x, y, tile);
                        spawned++;
                    }
                }
            }
            return spawned;
        }
    }

    public struct CascadeResult
    {
        public int TilesRemoved;
        public int TilesMoved;
        public int TilesSpawned;

        public CascadeResult(int removed, int moved, int spawned)
        {
            TilesRemoved = removed;
            TilesMoved = moved;
            TilesSpawned = spawned;
        }
    }

    public struct ResolutionResult
    {
        public List<CascadeResult> Cascades;

        public ResolutionResult(List<CascadeResult> cascades)
        {
            Cascades = cascades;
        }

        public int TotalRemoved()
        {
            var sum = 0;
            foreach (var c in Cascades) sum += c.TilesRemoved;
            return sum;
        }
    }
}


