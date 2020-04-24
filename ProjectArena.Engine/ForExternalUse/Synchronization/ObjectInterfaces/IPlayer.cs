using System.Collections.Generic;
using ProjectArena.Engine.Helpers;

namespace ProjectArena.Engine.ForExternalUse.Synchronization.ObjectInterfaces
{
    public interface IPlayer
    {
        string Id { get; }

        int? Team { get; }

        List<int> KeyActorsSync { get; }

        int TurnsSkipped { get; }

        PlayerStatus Status { get; }
    }
}
