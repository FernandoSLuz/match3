using UnityEngine;
using MatchTree.Core.Tools;

namespace MatchTree.Core.Presentation.Views
{
    [RequireComponent(typeof(Camera))]
    [ExecuteAlways]
    public class CameraFit2D : MonoBehaviour
    {
        public SimulationRunner Runner;
        public SimpleBoardView View; // Required for TileSize and origin
        public float Padding = 0.5f; // World units padding around the board
        public bool CenterOnBoard = true;

        private Camera cam;
        private float lastAspect = -1f;
        private int lastWidth = -1;
        private int lastHeight = -1;

        void Awake()
        {
            cam = GetComponent<Camera>();
            if (cam != null) cam.orthographic = true;
        }

        void OnEnable()
        {
            TryFitIfChanged(true);
        }

        void LateUpdate()
        {
            TryFitIfChanged(false);
        }

        private void TryFitIfChanged(bool force)
        {
            if (cam == null) cam = GetComponent<Camera>();
            if (cam == null) return;

            if (!TryGetBoardSize(out var w, out var h)) return;

            var aspect = cam.aspect;
            if (force || aspect != lastAspect || w != lastWidth || h != lastHeight)
            {
                FitToBoard(w, h);
                lastAspect = aspect;
                lastWidth = w;
                lastHeight = h;
            }
        }

        private bool TryGetBoardSize(out int width, out int height)
        {
            width = 0;
            height = 0;
            if (Runner == null || Runner.Board == null) return false;
            width = Runner.Board.Width;
            height = Runner.Board.Height;
            return true;
        }

        private void FitToBoard(int width, int height)
        {
            if (View == null) return;
            var size = View.TileSize;
            var worldWidth = width * size + 2f * Padding;
            var worldHeight = height * size + 2f * Padding;

            var sizeY = worldHeight * 0.5f;
            var sizeX = worldWidth * 0.5f / Mathf.Max(0.0001f, cam.aspect);
            cam.orthographicSize = Mathf.Max(sizeY, sizeX);

            if (!CenterOnBoard) return;

            Transform origin = View != null ? View.transform : (Runner != null ? Runner.transform : null);
            if (origin == null) return;

            var centerLocal = new Vector3((width - 1) * size * 0.5f, (height - 1) * size * 0.5f, 0f);
            var centerWorld = origin.TransformPoint(centerLocal);
            var p = cam.transform.position;
            cam.transform.position = new Vector3(centerWorld.x, centerWorld.y, p.z);
        }
    }
}


