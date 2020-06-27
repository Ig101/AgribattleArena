using System;
using System.Collections.Generic;
using ProjectArena.Engine.ForExternalUse.Synchronization.ObjectInterfaces;

namespace ProjectArena.Engine.ForExternalUse.Synchronization
{
    public interface ISynchronizer
    {
        int RandomCounter { get; }

        IEnumerable<IPlayer> Players { get; }

        IEnumerable<IActor> ChangedActors { get; }

        IEnumerable<IActiveDecoration> ChangedDecorations { get; }

        IEnumerable<int> DeletedActors { get; }

        IEnumerable<int> DeletedDecorations { get; }

        IEnumerable<ITile> ChangedTiles { get; }

        ITile[,] TileSet { get; }
    }
}
