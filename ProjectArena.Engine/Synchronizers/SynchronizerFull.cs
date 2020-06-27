using System;
using System.Collections.Generic;
using System.Linq;
using ProjectArena.Engine.ForExternalUse.Synchronization;
using ProjectArena.Engine.ForExternalUse.Synchronization.ObjectInterfaces;
using ProjectArena.Engine.Objects.Abstract;
using ProjectArena.Engine.Synchronizers.SynchronizationObjects;

namespace ProjectArena.Engine.Synchronizers
{
    public class SynchronizerFull : ISynchronizer
    {
        public int RandomCounter { get; }

        public IEnumerable<IPlayer> Players { get; }

        public IEnumerable<IActor> ChangedActors { get; }

        public IEnumerable<IActiveDecoration> ChangedDecorations { get; }

        public IEnumerable<int> DeletedActors => new List<int>();

        public IEnumerable<int> DeletedDecorations => new List<int>();

        public IEnumerable<ITile> ChangedTiles => TileSet.Cast<ITile>().ToList();

        public ITile[,] TileSet { get; }

        public SynchronizerFull(
            List<Player> players,
            List<Objects.Actor> actors,
            List<Objects.ActiveDecoration> decorations,
            Objects.Tile[][] tiles,
            int randomCounter)
        {
            this.RandomCounter = randomCounter;
            this.Players = players.Select(x => new SynchronizationObjects.Player(x));
            this.ChangedDecorations = decorations.Select(x => new ActiveDecoration(x));
            this.ChangedActors = actors.Select(x => new Actor(x));
            this.TileSet = new ITile[tiles.Length, tiles[0].Length];
            for (int x = 0; x < tiles.Length; x++)
            {
                for (int y = 0; y < tiles[x].Length; y++)
                {
                    this.TileSet[x, y] = new Tile(tiles[x][y]);
                }
            }
        }
    }
}
