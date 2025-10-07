using UnityEngine;
using MatchTree.Core.Domain.TileModel;

namespace MatchTree.Core.Configs
{
    [CreateAssetMenu(fileName = "SpawnerConfig", menuName = "MatchTree/SpawnerConfig", order = 1)]
    public class SpawnerConfig : ScriptableObject
    {
        public TileColor[] Colors = new[] { TileColor.Red, TileColor.Blue, TileColor.Green, TileColor.Yellow, TileColor.Purple };
    }
}


