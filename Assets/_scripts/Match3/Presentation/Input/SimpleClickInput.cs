using UnityEngine;
using Lighthouse.Match3.Tools;
using Lighthouse.Shared.EventBus;
using Lighthouse.Match3.Application.Events;
using Lighthouse.Match3.Presentation.Views;

namespace Lighthouse.Match3.Presentation.Input
{
    public class SimpleClickInput : MonoBehaviour
    {
        public SimulationRunner Runner;
        public AnimatedBoardView BoardView;
        public CameraFit2D Fitter; // source of camera and transform fit

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
                var cam = Fitter != null ? Fitter.EffectiveCamera : Camera.main;
                if (cam == null) return;
				var screen = UnityEngine.Input.mousePosition;
				var world = cam.ScreenToWorldPoint(new Vector3(screen.x, screen.y, Mathf.Abs(transform.position.z - cam.transform.position.z)));

				// Convert to board-local space to account for board scale/offset
                var t = BoardView != null ? BoardView.transform : null;
                var tileSize = BoardView != null ? BoardView.TileSize : 1f;
				Vector3 local = t != null ? t.InverseTransformPoint(world) : world;

				var x = Mathf.RoundToInt(local.x / tileSize);
				var y = Mathf.RoundToInt(local.y / tileSize);
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


