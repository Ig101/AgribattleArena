using System.Collections.Generic;
using ProjectArena.Engine.Helpers;

namespace ProjectArena.Engine.ForExternalUse.Synchronization.ObjectInterfaces
{
    public interface IActiveDecoration
    {
        int Id { get; }

        string NativeId { get; }

        float Mod { get; }

        float InitiativePosition { get; }

        float Health { get; }

        string OwnerId { get; }

        bool IsAlive { get; }

        int X { get; }

        int Y { get; }

        float Z { get; }

        float MaxHealth { get; }

        List<TagSynergy> Armor { get; }
    }
}
