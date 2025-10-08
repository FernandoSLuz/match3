using System.Collections.Generic;
using Lighthouse.Match3.Domain.BoardState;
using Lighthouse.Match3.Domain.MatchFinder;
using Lighthouse.Match3.Domain.Spawner;

namespace Lighthouse.Match3.Domain.Resolver
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

                var removals = new System.Collections.Generic.List<BoardState.TileRemovalInfo>();
                foreach (var group in matches)
                {
                    removals.AddRange(board.RemovePositionsWithInfo(group));
                }

                var moves = gravity.Apply(board);
                var spawnInfos = FillWithInfo(board);
                cascades.Add(CascadeResult.From(removals, moves, spawnInfos));
            }

            return new ResolutionResult(cascades);
        }

        private System.Collections.Generic.List<TileSpawn> FillWithInfo(Board board)
        {
            var spawns = new System.Collections.Generic.List<TileSpawn>();
            for (var x = 0; x < board.Width; x++)
            {
                for (var y = 0; y < board.Height; y++)
                {
                    if (board.GetAt(x, y) == null)
                    {
                        var tile = spawner.CreateTile(x, y);
                        board.SetAt(x, y, tile);
                        spawns.Add(new TileSpawn(tile, new BoardPosition(x, y)));
                    }
                }
            }
            return spawns;
        }
    }

    public struct CascadeResult
    {
        public System.Collections.Generic.List<BoardState.TileRemovalInfo> Removals;
        public System.Collections.Generic.List<TileMove> Moves;
        public System.Collections.Generic.List<TileSpawn> Spawns;

        public static CascadeResult From(System.Collections.Generic.List<BoardState.TileRemovalInfo> removals, System.Collections.Generic.List<TileMove> moves, System.Collections.Generic.List<TileSpawn> spawns)
        {
            return new CascadeResult { Removals = removals, Moves = moves, Spawns = spawns };
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
            foreach (var c in Cascades) sum += c.Removals.Count;
            return sum;
        }
    }
}


