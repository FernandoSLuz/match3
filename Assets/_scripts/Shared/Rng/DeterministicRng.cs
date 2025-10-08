using System;

namespace Lighthouse.Shared.Rng
{
    public class DeterministicRng : IRng
    {
        private readonly Random random;

        public DeterministicRng(int seed)
        {
            random = new Random(seed);
        }

        public int NextInt(int minInclusive, int maxExclusive)
        {
            return random.Next(minInclusive, maxExclusive);
        }
    }
}


