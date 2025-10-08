using UnityEngine;
using Lighthouse.Match3.Domain.TileModel;

namespace Lighthouse.Match3.Configs
{
    [CreateAssetMenu(fileName = "SpawnerConfig", menuName = "Lighthouse/SpawnerConfig", order = 1)]
    public class SpawnerConfig : ScriptableObject
    {
        public TileColor[] Colors = new[] { TileColor.Red, TileColor.Blue, TileColor.Green, TileColor.Yellow, TileColor.Purple };
    }
}


