using System.Collections.Generic;
using ProjectArena.Engine.ForExternalUse.Synchronization.ObjectInterfaces;

namespace ProjectArena.Engine.ForExternalUse.Synchronization
{
    public interface ISynchronizer
    {
        int RandomCounter { get; }

        IActor TempActor { get; }

        IActiveDecoration TempDecoration { get; }

        IEnumerable<IPlayer> Players { get; }

        IEnumerable<IActor> ChangedActors { get; }

        IEnumerable<IActiveDecoration> ChangedDecorations { get; }

        IEnumerable<ISpecEffect> ChangedEffects { get; }

        IEnumerable<IActor> DeletedActors { get; }

        IEnumerable<IActiveDecoration> DeletedDecorations { get; }

        IEnumerable<ISpecEffect> DeletedEffects { get; }

        IEnumerable<ITile> ChangedTiles { get; }

        ITile[,] TileSet { get; }
    }
}
