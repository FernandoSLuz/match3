using UnityEngine;
using MatchTree.Core.Tools;

namespace MatchTree.Core.Presentation.Input
{
    public class SimpleClickInput : MonoBehaviour
    {
        public SimulationRunner Runner;
        public float TileSize = 1f;

        private Vector2Int? selected;

        void Update()
        {
            if (Runner == null) return;
            if (UnityEngine.Input.GetMouseButtonDown(0))
            {
                var world = Camera.main.ScreenToWorldPoint(UnityEngine.Input.mousePosition);
                var x = Mathf.RoundToInt(world.x / TileSize);
                var y = Mathf.RoundToInt(world.y / TileSize);
                if (selected == null)
                {
                    selected = new Vector2Int(x, y);
                }
                else
                {
                    var from = selected.Value;
                    selected = null;
                    Runner.EnqueueSwap(from.x, from.y, x, y);
                }
            }
        }
    }
}


