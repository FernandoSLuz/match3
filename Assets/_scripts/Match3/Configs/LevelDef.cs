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

        [System.Serializable]
        public struct Tile
        {
            public string Id;
            public string Alias;
            public TileColor Color;
            public Sprite Sprite;
        }

        [Header("Sprite mapping per color")]
        public Tile[] Tiles;

        // Computed set of available colors for this level, derived from Tiles.
        public TileColor[] Colors
        {
            get
            {
                if (Tiles == null || Tiles.Length == 0)
                {
                    return new[] { TileColor.Red, TileColor.Blue, TileColor.Green, TileColor.Yellow, TileColor.Purple };
                }
                var list = new System.Collections.Generic.List<TileColor>();
                for (var i = 0; i < Tiles.Length; i++)
                {
                    var c = Tiles[i].Color;
                    if (!list.Contains(c)) list.Add(c);
                }
                return list.ToArray();
            }
        }

        public Sprite GetTileSprite(TileColor color)
        {
            if (Tiles == null) return null;
            for (var i = 0; i < Tiles.Length; i++)
            {
                if (Tiles[i].Color == color) return Tiles[i].Sprite;
            }
            return null;
        }

        // Back-compat helper for callers expecting GetSprite
        public Sprite GetSprite(TileColor color) => GetTileSprite(color);
    }
}


