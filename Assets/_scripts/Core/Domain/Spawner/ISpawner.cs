using MatchTree.Core.Domain.TileModel;

namespace MatchTree.Core.Domain.Spawner
{
    public interface ISpawner
    {
        Tile CreateTile(int x, int y);
    }
}


