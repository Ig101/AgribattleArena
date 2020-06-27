using System.Collections.Generic;
using ProjectArena.Engine.Helpers;

namespace ProjectArena.Engine.ForExternalUse.Synchronization.ObjectInterfaces
{
    public interface IActiveDecoration
    {
        int Id { get; }

        string NativeId { get; }

        string Visualization { get; }

        float Health { get; }

        string OwnerId { get; }

        int? Team { get; }

        bool IsAlive { get; }

        int X { get; }

        int Y { get; }

        float Z { get; }

        float MaxHealth { get; }

        bool HealthRevealed { get; }
    }
}
