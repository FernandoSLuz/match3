using Lighthouse.Match3.Domain.TileModel;

namespace Lighthouse.Match3.Domain.Spawner
{
    public interface ISpawner
    {
        Tile CreateTile(int x, int y);
    }
}


