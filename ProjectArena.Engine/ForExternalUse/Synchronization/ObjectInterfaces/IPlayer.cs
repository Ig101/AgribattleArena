using System.Collections.Generic;
using ProjectArena.Engine.Helpers;

namespace ProjectArena.Engine.ForExternalUse.Synchronization.ObjectInterfaces
{
    public interface IPlayer
    {
        string Id { get; }

        string UserId { get; }

        int? Team { get; }

        List<int> KeyActorsSync { get; }

        PlayerStatus Status { get; }
    }
}
