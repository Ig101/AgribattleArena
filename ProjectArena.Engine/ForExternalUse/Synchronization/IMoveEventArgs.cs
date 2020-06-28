using System.Collections.Generic;

namespace ProjectArena.Engine.ForExternalUse.Synchronization
{
    public interface IMoveEventArgs
    {
        IScene Scene { get; }

        IEnumerable<IMoveInfo> MoveDefinition { get; }
    }
}