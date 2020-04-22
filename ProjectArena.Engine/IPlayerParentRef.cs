using System.Collections.Generic;
using AgribattleArena.Engine.Helpers;
using AgribattleArena.Engine.Objects;

namespace AgribattleArena.Engine
{
    public interface IPlayerParentRef
    {
        string Id { get; }

        int? Team { get; }

        List<Actor> KeyActors { get; }

        int TurnsSkipped { get; }

        PlayerStatus Status { get; }

        void SkipTurn();

        bool ActThisTurn();

        void Defeat();

        void Victory();
    }
}
