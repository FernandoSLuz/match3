using UnityEngine;
using Lighthouse.Match3.Tools;
using Lighthouse.Shared.EventBus;
using Lighthouse.Match3.Application.Events;

namespace Lighthouse.Match3.Presentation.Input
{
    public class SimpleClickInput : MonoBehaviour
    {
        public SimulationRunner Runner;
        public float TileSize = 1f;

        private Vector2Int? selected;
        private EventBus bus;

        void Start()
        {
			bus = Runner != null ? Runner.Bus : null;
        }

        void Update()
        {
			if (Runner == null) return;
			// Lazy bind to EventBus if it wasn't ready at Start
			if (bus == null && Runner.Bus != null) bus = Runner.Bus;
            if (UnityEngine.Input.GetMouseButtonDown(0))
            {
                var world = Camera.main.ScreenToWorldPoint(UnityEngine.Input.mousePosition);
                var x = Mathf.RoundToInt(world.x / TileSize);
                var y = Mathf.RoundToInt(world.y / TileSize);
				var board = Runner.Board;
				if (board == null) return;
				// Bounds check
				if (x < 0 || y < 0 || x >= board.Width || y >= board.Height) return;
                if (selected == null)
                {
                    selected = new Vector2Int(x, y);
                    if (bus != null) bus.Publish(new TileSelectedEvent { Position = new Lighthouse.Match3.Domain.BoardState.BoardPosition(x, y) });
                }
                else
                {
                    var from = selected.Value;
					// If clicking the same tile, toggle off selection
					if (from.x == x && from.y == y)
					{
                        selected = null;
                        if (bus != null) bus.Publish(new TileDeselectedEvent { Position = new Lighthouse.Match3.Domain.BoardState.BoardPosition(from.x, from.y) });
						return;
					}
					// Only allow adjacent (4-neighbour) swaps
					var manhattan = Mathf.Abs(from.x - x) + Mathf.Abs(from.y - y);
					if (manhattan == 1)
					{
						selected = null;
						if (bus != null) bus.Publish(new TileDeselectedEvent { Position = new Lighthouse.Match3.Domain.BoardState.BoardPosition(from.x, from.y) });
						Runner.EnqueueSwap(from.x, from.y, x, y);
					}
					else
					{
					// Change selection to the new tile (no swap). Ensure previous scaled back.
                    if (bus != null)
					{
                        bus.Publish(new TileDeselectedEvent { Position = new Lighthouse.Match3.Domain.BoardState.BoardPosition(from.x, from.y) });
                        bus.Publish(new TileSelectedEvent { Position = new Lighthouse.Match3.Domain.BoardState.BoardPosition(x, y) });
					}
					selected = new Vector2Int(x, y);
					}
                }
            }
        }
    }
}


