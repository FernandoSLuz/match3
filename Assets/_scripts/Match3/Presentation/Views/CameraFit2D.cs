using UnityEngine;
using Lighthouse.Match3.Tools;

namespace Lighthouse.Match3.Presentation.Views
{
    [ExecuteAlways]
    public class CameraFit2D : MonoBehaviour
    {
        public SimulationRunner Runner;
        public AnimatedBoardView BoardView;

        [SerializeField] private Camera targetCamera;

        [Header("Padding (world units)")]
        [Range(0f, 10f)] [SerializeField] private float paddingTop = 0.5f;
        [Range(0f, 10f)] [SerializeField] private float paddingBottom = 0.5f;
        [Range(0f, 10f)] [SerializeField] private float paddingLeft = 0.5f;
        [Range(0f, 10f)] [SerializeField] private float paddingRight = 0.5f;

        private Camera cam;
        private float lastAspect = -1f;
        private int lastWidth = -1;
        private int lastHeight = -1;
        private float lastTileSize = -1f;
        private float lastPadTop = -1f, lastPadBottom = -1f, lastPadLeft = -1f, lastPadRight = -1f;
        private bool lastOrtho = false;
        private float lastOrthoSize = -1f;
        private Vector3 initialLocalScale;

        void Awake()
        {
            CacheCamera();
            if (BoardView != null)
            {
                initialLocalScale = BoardView.transform.localScale;
            }
        }

        void OnEnable()
        {
            TryFitIfChanged(true);
        }

        void OnValidate()
        {
            // Ensure runtime and edit-time changes are reflected immediately.
            TryFitIfChanged(true);
        }

        void LateUpdate()
        {
            TryFitIfChanged(false);
        }

        private void CacheCamera()
        {
            cam = targetCamera != null ? targetCamera : Camera.main;
        }

        public Camera EffectiveCamera
        {
            get
            {
                if (cam == null) CacheCamera();
                return cam;
            }
        }

        private void TryFitIfChanged(bool force)
        {
            if (cam == null) CacheCamera();
            if (cam == null || BoardView == null) return;
            if (!TryGetBoardSize(out var w, out var h)) return;

            var aspect = cam.aspect;
            var tile = BoardView.TileSize;
            var ortho = cam.orthographic;
            var orthoSize = ortho ? cam.orthographicSize : -1f;

            bool changed = force
                || aspect != lastAspect
                || w != lastWidth
                || h != lastHeight
                || !Mathf.Approximately(tile, lastTileSize)
                || !Mathf.Approximately(paddingTop, lastPadTop)
                || !Mathf.Approximately(paddingBottom, lastPadBottom)
                || !Mathf.Approximately(paddingLeft, lastPadLeft)
                || !Mathf.Approximately(paddingRight, lastPadRight)
                || ortho != lastOrtho
                || (ortho && !Mathf.Approximately(orthoSize, lastOrthoSize));

            if (!changed) return;

            FitBoardToCamera(w, h);

            lastAspect = aspect;
            lastWidth = w;
            lastHeight = h;
            lastTileSize = tile;
            lastPadTop = paddingTop;
            lastPadBottom = paddingBottom;
            lastPadLeft = paddingLeft;
            lastPadRight = paddingRight;
            lastOrtho = ortho;
            lastOrthoSize = orthoSize;
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

        private void FitBoardToCamera(int boardWidthTiles, int boardHeightTiles)
        {
            var t = BoardView.transform;
            if (initialLocalScale == Vector3.zero) initialLocalScale = t.localScale;

            var tileSize = Mathf.Max(0.0001f, BoardView.TileSize);
            var boardWidth = Mathf.Max(0.0001f, boardWidthTiles * tileSize);
            var boardHeight = Mathf.Max(0.0001f, boardHeightTiles * tileSize);

            // Compute camera visible size (world units) at the board plane
            float viewWidth, viewHeight;
            float planeDistance = Mathf.Abs(Vector3.Dot(t.position - cam.transform.position, cam.transform.forward));
            if (cam.orthographic)
            {
                viewHeight = 2f * Mathf.Max(0.0001f, cam.orthographicSize);
                viewWidth = viewHeight * Mathf.Max(0.0001f, cam.aspect);
            }
            else
            {
                // Perspective camera support: fit at board plane along camera forward direction
                float frustumHeight = 2f * Mathf.Tan(cam.fieldOfView * 0.5f * Mathf.Deg2Rad) * Mathf.Max(0.0001f, planeDistance);
                viewHeight = Mathf.Max(0.0001f, frustumHeight);
                viewWidth = viewHeight * Mathf.Max(0.0001f, cam.aspect);
            }

            // Available size after padding (world units)
            float availableWidth = Mathf.Max(0.0001f, viewWidth - paddingLeft - paddingRight);
            float availableHeight = Mathf.Max(0.0001f, viewHeight - paddingTop - paddingBottom);

            // Uniform scale to fit within available rect
            float scaleX = availableWidth / boardWidth;
            float scaleY = availableHeight / boardHeight;
            float uniformScale = Mathf.Max(0.0001f, Mathf.Min(scaleX, scaleY));

            t.localScale = initialLocalScale * uniformScale;

            // Target center inside camera rect, offset by asymmetric padding
            // Center shift is (left - right)/2, (bottom - top)/2 relative to camera center
            float dx = (paddingLeft - paddingRight) * 0.5f;
            float dy = (paddingBottom - paddingTop) * 0.5f;

            Vector3 centerOnBoardPlane = cam.transform.position + cam.transform.forward * planeDistance;
            Vector3 desiredCenter = centerOnBoardPlane
                                     + cam.transform.right * dx
                                     + cam.transform.up * dy;
            desiredCenter.z = t.position.z; // keep board Z

            // Board center in world after scaling
            var centerLocal = new Vector3(
                (boardWidthTiles - 1) * tileSize * 0.5f,
                (boardHeightTiles - 1) * tileSize * 0.5f,
                0f);

            Vector3 currentCenter = t.TransformPoint(centerLocal);
            Vector3 delta = desiredCenter - currentCenter;
            t.position += delta;
        }
    }
}


