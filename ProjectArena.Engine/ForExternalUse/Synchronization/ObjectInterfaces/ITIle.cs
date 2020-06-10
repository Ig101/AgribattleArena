using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectArena.Engine.ForExternalUse.Synchronization.ObjectInterfaces
{
    public interface ITile
    {
        int X { get; }

        int Y { get; }

        int? TempActorId { get; }

        float Height { get; }

        string NativeId { get; }

        string OwnerId { get; }

        int? Team { get; }

        bool Unbearable { get; }

        bool Revealed { get; }
    }
}
