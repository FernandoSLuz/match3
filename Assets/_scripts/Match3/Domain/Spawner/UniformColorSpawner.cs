using System;
using Lighthouse.Match3.Domain.TileModel;

namespace Lighthouse.Match3.Domain.Spawner
{
    public class UniformColorSpawner : ISpawner
    {
        private readonly TileColor[] colors;
        private readonly Random rng;

        public UniformColorSpawner(int seed, TileColor[] availableColors)
        {
            rng = new Random(seed);
            colors = availableColors;
        }

        public Tile CreateTile(int x, int y)
        {
            var color = colors[rng.Next(colors.Length)];
            return new Tile(color, TileType.Normal);
        }
    }
}


