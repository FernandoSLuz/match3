using MatchTree.Core.Domain.BoardState;

namespace MatchTree.Core.Application.EventBus
{
    public struct LevelStartEvent : IGameEvent
    {
        public string LevelId;
        public int Seed;
        public int Width;
        public int Height;
    }

    public struct MoveAttemptedEvent : IGameEvent
    {
        public int TurnIndex;
        public BoardPosition From;
        public BoardPosition To;
        public bool Valid;
    }

    public struct SwapVisualEvent : IGameEvent
    {
        public int TurnIndex;
        public BoardPosition From;
        public BoardPosition To;
        public bool Revert; // if true: animate out-and-back without changing board
    }

    public struct TilesMatchedEvent : IGameEvent
    {
        public int TurnIndex;
        public int CascadeIndex;
        public System.Collections.Generic.List<BoardPosition> Positions;
    }

    public struct TilesRemovedEvent : IGameEvent
    {
        public int TurnIndex;
        public int CascadeIndex;
        public System.Collections.Generic.List<BoardPosition> Positions;
    }

    public struct TilesMovedEvent : IGameEvent
    {
        public int TurnIndex;
        public int CascadeIndex;
        public System.Collections.Generic.List<(BoardPosition from, BoardPosition to)> Moves;
    }

    public struct TilesSpawnedEvent : IGameEvent
    {
        public int TurnIndex;
        public int CascadeIndex;
        public System.Collections.Generic.List<BoardPosition> Positions;
    }

    public struct TileSelectedEvent : IGameEvent
    {
        public BoardPosition Position;
    }

    public struct TileDeselectedEvent : IGameEvent
    {
        public BoardPosition Position;
    }

    public struct CascadeEndEvent : IGameEvent
    {
        public int Index;
        public int TilesFallen;
        public int TimeMs;
    }

    public struct LevelEndEvent : IGameEvent
    {
        public string Result;
        public int TurnsUsed;
        public int TimeTotalMs;
    }

    public struct AnimationCompleteEvent : IGameEvent
    {
    }
}


