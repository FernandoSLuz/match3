using UnityEngine;
using MatchTree.Core.Tools;
using MatchTree.Core.Domain.BoardState;
using MatchTree.Core.Domain.TileModel;

namespace MatchTree.Core.Presentation.Views
{
    public class SimpleBoardView : MonoBehaviour
    {
        public SimulationRunner Runner;
        public float TileSize = 1f;
        public Sprite Red;
        public Sprite Blue;
        public Sprite Green;
        public Sprite Yellow;
        public Sprite Purple;

        private GameObject[,] visuals;

        void Start()
        {
            if (Runner == null)
            {
                Debug.LogError("SimpleBoardView requires SimulationRunner");
                enabled = false;
                return;
            }
        }

        void LateUpdate()
        {
            Sync();
        }

        private void Sync()
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
                    go.transform.localPosition = new Vector3(x * TileSize, y * TileSize, 0f);
                }
            }
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
                    go.AddComponent<SpriteRenderer>();
                    visuals[x, y] = go;
                }
            }
        }

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

        
    }
}


