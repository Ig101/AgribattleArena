using System.Collections.Generic;

namespace ProjectArena.Engine.ForExternalUse.Generation.ObjectInterfaces
{
    public interface IPlayer
    {
        string Id { get; }

        string UserId { get; }

        int? Team { get; }

        IActor PlayerActorGen { get; }

        List<IActor> KeyActorsGen { get; }
    }
}
