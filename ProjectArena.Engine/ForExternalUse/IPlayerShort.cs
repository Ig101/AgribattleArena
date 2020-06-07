using System.Collections.Generic;
using ProjectArena.Engine.Helpers;

namespace ProjectArena.Engine.ForExternalUse
{
    public interface IPlayerShort
    {
        string Id { get; }

        int? Team { get; }

        PlayerStatus Status { get; }

        bool Left { get; }

        IEnumerable<int> GetPlayerActors();

        bool TryRedeemPlayerStatusHash(out int? hash);
    }
}
