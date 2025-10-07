using UnityEngine;
using MatchTree.Core.Domain.TileModel;

namespace MatchTree.Core.Configs
{
    [CreateAssetMenu(fileName = "LevelDef", menuName = "MatchTree/LevelDef", order = 0)]
    public class LevelDef : ScriptableObject
    {
        public string Id = "level_001";
        public int Width = 8;
        public int Height = 8;
        public int Seed = 12345;
        public TileColor[] Colors = new[] { TileColor.Red, TileColor.Blue, TileColor.Green, TileColor.Yellow, TileColor.Purple };
    }
}


