using UnityEngine;
using Lighthouse.Match3.Domain.TileModel;

namespace Lighthouse.Match3.Configs
{
    [CreateAssetMenu(fileName = "LevelDef", menuName = "Lighthouse/LevelDef", order = 0)]
    public class LevelDef : ScriptableObject
    {
        public string Id = "level_001";
        public int Width = 8;
        public int Height = 8;
        public int Seed = 12345;
        public TileColor[] Colors = new[] { TileColor.Red, TileColor.Blue, TileColor.Green, TileColor.Yellow, TileColor.Purple };
    }
}


