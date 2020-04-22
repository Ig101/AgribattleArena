using System;
using System.Collections.Generic;
using System.Text;

namespace AgribattleArena.Engine.ForExternalUse.Synchronization.ObjectInterfaces
{
    public interface ITile
    {
        int X { get; }

        int Y { get; }

        int? TempActorId { get; }

        float Height { get; }

        string NativeId { get; }

        string OwnerId { get; }
    }
}
