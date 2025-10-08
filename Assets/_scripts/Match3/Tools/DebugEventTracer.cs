using UnityEngine;
using Lighthouse.Shared.EventBus;
using Lighthouse.Match3.Application.Events;

namespace Lighthouse.Match3.Tools
{
    public class DebugEventTracer : MonoBehaviour
    {
        public SimulationRunner Runner;

        private int seq;
        private bool subscribed;

        void OnEnable()
        {
            if (Runner == null)
            {
                Debug.LogError("DebugEventTracer requires SimulationRunner");
                enabled = false;
                return;
            }
            TrySubscribe();
        }

        void Update()
        {
            if (!subscribed) TrySubscribe();
        }

        private void TrySubscribe()
        {
            var bus = Runner.Bus;
            if (bus == null) return;
            bus.Subscribe<LevelStartEvent>(OnLevelStart);
            bus.Subscribe<MoveAttemptedEvent>(OnMoveAttempted);
            bus.Subscribe<CascadeEndEvent>(OnCascadeEnd);
            bus.Subscribe<LevelEndEvent>(OnLevelEnd);
            subscribed = true;
        }

        private void OnLevelStart(LevelStartEvent e)
        {
            Debug.Log($"[{++seq}] level_start id={e.LevelId} seed={e.Seed} size={e.Width}x{e.Height}");
        }

        private void OnMoveAttempted(MoveAttemptedEvent e)
        {
            Debug.Log($"[{++seq}] move_attempted turn={e.TurnIndex} from={e.From} to={e.To} valid={e.Valid}");
        }

        private void OnCascadeEnd(CascadeEndEvent e)
        {
            Debug.Log($"[{++seq}] cascade_end index={e.Index} fallen={e.TilesFallen}");
        }

        private void OnLevelEnd(LevelEndEvent e)
        {
            Debug.Log($"[{++seq}] level_end result={e.Result} turns={e.TurnsUsed} timeMs={e.TimeTotalMs}");
        }
    }
}


