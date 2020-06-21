using System.Collections.Generic;
using ProjectArena.Engine.Helpers;
using ProjectArena.Engine.Objects;

namespace ProjectArena.Engine
{
    public interface IPlayerParentRef
    {
        string Id { get; }

        string UserId { get; }

        int? Team { get; }

        List<Actor> KeyActors { get; }

        int TurnsSkipped { get; }

        PlayerStatus Status { get; }

        void SkipTurn();

        bool ActThisTurn();

        void Defeat(bool leave);

        void Victory();
    }
}
