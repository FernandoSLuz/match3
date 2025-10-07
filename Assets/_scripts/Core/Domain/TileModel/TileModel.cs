using MatchTree.Core.Domain.TileModel;

namespace MatchTree.Core.Domain.TileModel
{
    public class Tile
    {
        public TileColor Color { get; private set; }
        public TileType Type { get; private set; }

        public Tile(TileColor color, TileType type = TileType.Normal)
        {
            Color = color;
            Type = type;
        }

        public void SetColor(TileColor color)
        {
            Color = color;
        }

        public void SetType(TileType type)
        {
            Type = type;
        }
    }
}


