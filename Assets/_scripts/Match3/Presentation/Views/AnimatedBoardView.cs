using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lighthouse.Match3.Tools;
using Lighthouse.Shared.EventBus;
using Lighthouse.Match3.Domain.BoardState;
using Lighthouse.Match3.Domain.TileModel;
using Lighthouse.Match3.Configs;
using Lighthouse.Match3.Application.Events;

namespace Lighthouse.Match3.Presentation.Views
{
    public class AnimatedBoardView : MonoBehaviour
    {
        public SimulationRunner Runner;
        public TransitionConfig Transitions;
        public float TileSize = 1f;
        public Sprite Red;
        public Sprite Blue;
        public Sprite Green;
        public Sprite Yellow;
        public Sprite Purple;

        private GameObject[,] visuals;
        private EventBus eventBus;

        void Start()
        {
            if (Runner == null)
            {
                Debug.LogError("AnimatedBoardView requires SimulationRunner");
                enabled = false;
                return;
            }
            if (Transitions == null)
            {
                Debug.LogError("AnimatedBoardView requires TransitionConfig");
                enabled = false;
                return;
            }

            StartCoroutine(BindBusAndSubscribe());
        }

        private IEnumerator BindBusAndSubscribe()
        {
            while (Runner == null || Runner.Bus == null) yield return null;
            eventBus = Runner.Bus;
            eventBus.Subscribe<LevelStartEvent>(OnLevelStart);
            eventBus.Subscribe<SwapVisualEvent>(OnSwapVisual);
            eventBus.Subscribe<TilesRemovedEvent>(OnTilesRemoved);
            eventBus.Subscribe<TilesMovedEvent>(OnTilesMoved);
            eventBus.Subscribe<TilesSpawnedEvent>(OnTilesSpawned);
            SyncToBoard();
        }

        private void OnLevelStart(LevelStartEvent e)
        {
            visuals = null;
            SyncToBoard();
        }

        private void EnsureGrid(Board board)
        {
            if (visuals != null && visuals.GetLength(0) == board.Width && visuals.GetLength(1) == board.Height) return;
            if (visuals != null)
            {
                foreach (var obj in visuals) if (obj != null) Destroy(obj);
            }
            visuals = new GameObject[board.Width, board.Height];
            for (var x = 0; x < board.Width; x++)
            {
                for (var y = 0; y < board.Height; y++)
                {
                    var go = new GameObject($"tile_{x}_{y}");
                    go.transform.SetParent(transform, false);
                    var sr = go.AddComponent<SpriteRenderer>();
                    sr.sortingOrder = 0;
                    visuals[x, y] = go;
                }
            }
        }

        private void SyncToBoard()
        {
            var board = Runner.Board;
            if (board == null) return;
            EnsureGrid(board);
            for (var x = 0; x < board.Width; x++)
            {
                for (var y = 0; y < board.Height; y++)
                {
                    var tile = board.GetAt(x, y);
                    var go = visuals[x, y];
                    if (tile == null)
                    {
                        go.SetActive(false);
                        continue;
                    }
                    go.SetActive(true);
                    var sr = go.GetComponent<SpriteRenderer>();
                    sr.sprite = SpriteFor(tile.Color);
                    go.transform.localPosition = Pos(x, y);
                    go.transform.localScale = Vector3.one;
                    sr.color = Color.white;
                }
            }
        }

        private Vector3 Pos(int x, int y) => new Vector3(x * TileSize, y * TileSize, 0f);

        private Sprite SpriteFor(TileColor color)
        {
            switch (color)
            {
                case TileColor.Red: return Red;
                case TileColor.Blue: return Blue;
                case TileColor.Green: return Green;
                case TileColor.Yellow: return Yellow;
                case TileColor.Purple: return Purple;
                default: return null;
            }
        }

        // Selection visuals
        private BoardPosition? selectedPos;
        private Coroutine wobbleRoutine;

        private void OnEnable()
        {
            // Extra safety: subscribe selection events even if Late-bound
            StartCoroutine(EnsureSelectionSubscriptions());
        }

        private IEnumerator EnsureSelectionSubscriptions()
        {
            while (Runner == null || Runner.Bus == null) yield return null;
            Runner.Bus.Subscribe<TileSelectedEvent>(OnTileSelected);
            Runner.Bus.Subscribe<TileDeselectedEvent>(OnTileDeselected);
        }

        private void OnTileSelected(TileSelectedEvent e)
        {
            selectedPos = e.Position;
            if (wobbleRoutine != null) StopCoroutine(wobbleRoutine);
            wobbleRoutine = StartCoroutine(WobbleSelected());
        }

        private void OnTileDeselected(TileDeselectedEvent e)
        {
            if (selectedPos.HasValue && selectedPos.Value.X == e.Position.X && selectedPos.Value.Y == e.Position.Y)
            {
                selectedPos = null;
                if (wobbleRoutine != null) StopCoroutine(wobbleRoutine);
                wobbleRoutine = null;
                ResetScaleAt(e.Position);
                SyncToBoard();
            }
        }

        private IEnumerator WobbleSelected()
        {
            while (selectedPos.HasValue)
            {
                var p = selectedPos.Value;
                if (visuals != null && p.X >= 0 && p.Y >= 0 && p.X < visuals.GetLength(0) && p.Y < visuals.GetLength(1))
                {
                    var go = visuals[p.X, p.Y];
                    if (go != null)
                    {
                        // Continuous wobble loop while selected
                        float t = 0f;
                        float baseScale = 1.08f;
                        float wobbleAmp = 0.05f;
                        float wobbleSpeed = 6.0f; // radians/sec
                        while (selectedPos.HasValue && selectedPos.Value.X == p.X && selectedPos.Value.Y == p.Y)
                        {
                            t += Time.deltaTime * wobbleSpeed;
                            var scale = baseScale + Mathf.Sin(t) * wobbleAmp;
                            go.transform.localScale = new Vector3(scale, scale, 1f);
                            yield return null;
                        }
                    }
                }
                yield return null;
            }
            // Reset all scales when deselected
            if (visuals != null)
            {
                for (var x = 0; x < visuals.GetLength(0); x++)
                {
                    for (var y = 0; y < visuals.GetLength(1); y++)
                    {
                        var go = visuals[x, y];
                        if (go != null) go.transform.localScale = Vector3.one;
                    }
                }
            }
        }

        private void ResetScaleAt(BoardPosition pos)
        {
            if (visuals == null) return;
            if (pos.X < 0 || pos.Y < 0 || pos.X >= visuals.GetLength(0) || pos.Y >= visuals.GetLength(1)) return;
            var go = visuals[pos.X, pos.Y];
            if (go != null) go.transform.localScale = Vector3.one;
        }

        private void OnSwapVisual(SwapVisualEvent e)
        {
            StartCoroutine(AnimateSwap(e.From, e.To, e.Revert));
        }

        private IEnumerator AnimateSwap(BoardPosition a, BoardPosition b, bool revert)
        {
            var board = Runner.Board;
            if (board != null) EnsureGrid(board);

            var aGo = visuals[a.X, a.Y];
            var bGo = visuals[b.X, b.Y];
            // Ensure both visibles and opaque
            var aSr = aGo.GetComponent<SpriteRenderer>();
            var bSr = bGo.GetComponent<SpriteRenderer>();
            aGo.SetActive(true); aSr.color = Color.white;
            bGo.SetActive(true); bSr.color = Color.white;
            var aStart = aGo.transform.localPosition;
            var bStart = bGo.transform.localPosition;
            var aEnd = Pos(b.X, b.Y);
            var bEnd = Pos(a.X, a.Y);
            var duration = Transitions.SwapDuration;

            float t = 0f;
            while (t < duration)
            {
                t += Time.deltaTime;
                var k = Mathf.Clamp01(t / duration);
                aGo.transform.localPosition = Vector3.Lerp(aStart, aEnd, k);
                bGo.transform.localPosition = Vector3.Lerp(bStart, bEnd, k);
                yield return null;
            }

            aGo.transform.localPosition = aEnd;
            bGo.transform.localPosition = bEnd;

            if (!revert)
            {
                // Swap references so mapping matches board state
                var tmp = visuals[b.X, b.Y];
                visuals[b.X, b.Y] = visuals[a.X, a.Y];
                visuals[a.X, a.Y] = tmp;
            }
            else
            {
                // animate back
                var backDuration = Transitions.SwapDuration;
                float tt = 0f;
                while (tt < backDuration)
                {
                    tt += Time.deltaTime;
                    var kk = Mathf.Clamp01(tt / backDuration);
                    aGo.transform.localPosition = Vector3.Lerp(aEnd, aStart, kk);
                    bGo.transform.localPosition = Vector3.Lerp(bEnd, bStart, kk);
                    yield return null;
                }
                aGo.transform.localPosition = aStart;
                bGo.transform.localPosition = bStart;
            }

            eventBus.Publish(new AnimationCompleteEvent());
        }

        private void OnTilesRemoved(TilesRemovedEvent e)
        {
            StartCoroutine(AnimateShakeThenFade(e.Positions));
        }

        private IEnumerator AnimateShakeThenFade(List<BoardPosition> positions)
        {
            var board = Runner.Board;
            if (board != null) EnsureGrid(board);

            // Shake
            var shakeDuration = Transitions.ShakeDuration;
            var shakeMagnitude = Transitions.ShakeMagnitude;
            float t = 0f;
            var originals = new Dictionary<BoardPosition, Vector3>();
            foreach (var p in positions)
            {
                originals[p] = visuals[p.X, p.Y].transform.localPosition;
            }

            while (t < shakeDuration)
            {
                t += Time.deltaTime;
                foreach (var p in positions)
                {
                    var basePos = originals[p];
                    var offset = new Vector3(
                        Random.Range(-shakeMagnitude, shakeMagnitude),
                        Random.Range(-shakeMagnitude, shakeMagnitude),
                        0f);
                    visuals[p.X, p.Y].transform.localPosition = basePos + offset;
                }
                yield return null;
            }

            foreach (var p in positions)
            {
                visuals[p.X, p.Y].transform.localPosition = originals[p];
            }

            // Fade
            var fadeDuration = Transitions.FadeOutDuration;
            t = 0f;
            var srs = new List<SpriteRenderer>();
            foreach (var p in positions) srs.Add(visuals[p.X, p.Y].GetComponent<SpriteRenderer>());

            while (t < fadeDuration)
            {
                t += Time.deltaTime;
                var k = Mathf.Clamp01(t / fadeDuration);
                var c = Color.Lerp(Color.white, new Color(1, 1, 1, 0), k);
                foreach (var sr in srs) sr.color = c;
                yield return null;
            }

            foreach (var sr in srs) sr.color = new Color(1, 1, 1, 0);
            foreach (var p in positions) visuals[p.X, p.Y].SetActive(false);

            eventBus.Publish(new AnimationCompleteEvent());
        }

        private void OnTilesMoved(TilesMovedEvent e)
        {
            StartCoroutine(AnimateMoves(e.Moves));
        }

        private IEnumerator AnimateMoves(List<(BoardPosition from, BoardPosition to)> moves)
        {
            var board = Runner.Board;
            if (board != null) EnsureGrid(board);

            var durationPerCell = Transitions.FallDurationPerCell;
            var items = new List<(GameObject go, Vector3 start, Vector3 end, float duration)>();

            // Collect all origins to avoid hiding a tile that's also moving this frame
            var movingFrom = new System.Collections.Generic.HashSet<BoardPosition>();
            for (var i = 0; i < moves.Count; i++) movingFrom.Add(moves[i].from);

            foreach (var m in moves)
            {
                // Prepare moving GO and hide destination during transit
                var go = visuals[m.from.X, m.from.Y];
                if (go != null)
                {
                    var sr = go.GetComponent<SpriteRenderer>();
                    go.SetActive(true);
                    if (sr != null) sr.color = Color.white;
                }
                var destGo = visuals[m.to.X, m.to.Y];
                // Only hide destination if it's not also the origin of another move
                if (destGo != null && !movingFrom.Contains(m.to) && destGo != go)
                {
                    destGo.SetActive(false);
                }
                var start = go.transform.localPosition;
                var end = Pos(m.to.X, m.to.Y);
                var dist = Mathf.Abs(m.to.X - m.from.X) + Mathf.Abs(m.to.Y - m.from.Y);
                var d = Mathf.Max(0.01f, dist * durationPerCell);
                items.Add((go, start, end, d));
            }

            float t = 0f;
            var maxDuration = 0f;
            foreach (var it in items) if (it.duration > maxDuration) maxDuration = it.duration;

            while (t < maxDuration)
            {
                t += Time.deltaTime;
                foreach (var it in items)
                {
                    var k = Mathf.Clamp01(t / it.duration);
                    it.go.transform.localPosition = Vector3.Lerp(it.start, it.end, k);
                }
                yield return null;
            }

            foreach (var it in items) it.go.transform.localPosition = it.end;

            // Update mapping so visuals match board cells
            for (var i = 0; i < moves.Count; i++)
            {
                var m = moves[i];
                var fromGo = visuals[m.from.X, m.from.Y];
                var toGo = visuals[m.to.X, m.to.Y];
                visuals[m.to.X, m.to.Y] = fromGo;
                visuals[m.from.X, m.from.Y] = toGo;
                // Ensure final active states
                if (visuals[m.to.X, m.to.Y] != null)
                {
                    visuals[m.to.X, m.to.Y].SetActive(true);
                    var sr = visuals[m.to.X, m.to.Y].GetComponent<SpriteRenderer>();
                    if (sr != null) sr.color = Color.white;
                    visuals[m.to.X, m.to.Y].transform.localPosition = Pos(m.to.X, m.to.Y);
                }
                if (visuals[m.from.X, m.from.Y] != null)
                {
                    // Keep placeholder at from (may remain inactive until spawn)
                    visuals[m.from.X, m.from.Y].transform.localPosition = Pos(m.from.X, m.from.Y);
                }
            }

            eventBus.Publish(new AnimationCompleteEvent());
        }

        private void OnTilesSpawned(TilesSpawnedEvent e)
        {
            StartCoroutine(AnimateSpawns(e.Positions));
        }

        private IEnumerator AnimateSpawns(List<BoardPosition> positions)
        {
            var board = Runner.Board;
            if (board == null) yield break;
            EnsureGrid(board);

            var startOffset = new Vector3(0, TileSize * 1.0f, 0);
            var duration = Transitions.FallDurationPerCell;
            var srs = new List<SpriteRenderer>();

            foreach (var p in positions)
            {
                var go = visuals[p.X, p.Y];
                var tile = board.GetAt(p.X, p.Y);
                if (tile == null) continue;

                var sr = go.GetComponent<SpriteRenderer>();
                srs.Add(sr);
                sr.sprite = SpriteFor(tile.Color);
                go.SetActive(true);
                go.transform.localPosition = Pos(p.X, p.Y) + startOffset;
                sr.color = new Color(1, 1, 1, 0);
            }

            float t = 0f;
            while (t < duration)
            {
                t += Time.deltaTime;
                var k = Mathf.Clamp01(t / duration);
                foreach (var p in positions)
                {
                    var go = visuals[p.X, p.Y];
                    go.transform.localPosition = Vector3.Lerp(Pos(p.X, p.Y) + startOffset, Pos(p.X, p.Y), k);
                }
                var col = new Color(1, 1, 1, Mathf.Lerp(0, 1, k));
                foreach (var sr in srs) sr.color = col;
                yield return null;
            }

            foreach (var p in positions) visuals[p.X, p.Y].transform.localPosition = Pos(p.X, p.Y);
            foreach (var sr in srs) sr.color = Color.white;
            // Ensure spawned cells are active and fully opaque
            foreach (var p in positions)
            {
                var go = visuals[p.X, p.Y];
                if (go == null) continue;
                go.SetActive(true);
                var sr = go.GetComponent<SpriteRenderer>();
                if (sr != null)
                {
                    sr.color = Color.white;
                    var boardTile = board.GetAt(p.X, p.Y);
                    if (boardTile != null) sr.sprite = SpriteFor(boardTile.Color);
                }
            }

            eventBus.Publish(new AnimationCompleteEvent());
        }
    }
}
