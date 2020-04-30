using System.Collections.Generic;
using ProjectArena.Engine.ForExternalUse.Synchronization.ObjectInterfaces;

namespace ProjectArena.Engine.ForExternalUse.Synchronization
{
    public interface ISynchronizer
    {
        int RandomCounter { get; }

        int? TempActor { get; }

        int? TempDecoration { get; }

        IEnumerable<IPlayer> Players { get; }

        IEnumerable<IActor> ChangedActors { get; }

        IEnumerable<IActiveDecoration> ChangedDecorations { get; }

        IEnumerable<ISpecEffect> ChangedEffects { get; }

        IEnumerable<int> DeletedActors { get; }

        IEnumerable<int> DeletedDecorations { get; }

        IEnumerable<int> DeletedEffects { get; }

        IEnumerable<ITile> ChangedTiles { get; }

        ITile[,] TileSet { get; }
    }
}
