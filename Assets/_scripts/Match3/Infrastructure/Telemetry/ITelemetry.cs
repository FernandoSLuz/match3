namespace Lighthouse.Match3.Infrastructure.Telemetry
{
    public interface ITelemetry
    {
        void LevelStart(string levelId, int seed, int width, int height);
        void MoveAttempted(int turn, int fromX, int fromY, int toX, int toY, bool valid);
        void MatchResolved(int turn, int cascade, int totalTilesRemoved);
        void LevelEnd(string result, int turnsUsed, int timeTotalMs);
    }
}


