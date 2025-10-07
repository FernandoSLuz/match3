using UnityEngine;

namespace MatchTree.Core.Infrastructure.Telemetry
{
    public class UnityDebugTelemetry : ITelemetry
    {
        public void LevelStart(string levelId, int seed, int width, int height)
        {
            Debug.Log($"level_start level={levelId} seed={seed} {width}x{height}");
        }

        public void MoveAttempted(int turn, int fromX, int fromY, int toX, int toY, bool valid)
        {
            Debug.Log($"move_attempted turn={turn} from=({fromX},{fromY}) to=({toX},{toY}) valid={valid}");
        }

        public void MatchResolved(int turn, int cascade, int totalTilesRemoved)
        {
            Debug.Log($"match_resolved turn={turn} cascade={cascade} removed={totalTilesRemoved}");
        }

        public void LevelEnd(string result, int turnsUsed, int timeTotalMs)
        {
            Debug.Log($"level_end result={result} turns={turnsUsed} time_ms={timeTotalMs}");
        }
    }
}


