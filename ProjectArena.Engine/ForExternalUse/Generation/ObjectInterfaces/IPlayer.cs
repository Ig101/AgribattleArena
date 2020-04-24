using System.Collections.Generic;

namespace ProjectArena.Engine.ForExternalUse.Generation.ObjectInterfaces
{
    public interface IPlayer
    {
        string Id { get; }

        int? Team { get; }

        List<IActor> KeyActorsGen { get; }
    }
}
