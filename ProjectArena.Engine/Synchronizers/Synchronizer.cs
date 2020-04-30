using System.Collections.Generic;
using System.Linq;
using ProjectArena.Engine.ForExternalUse.Synchronization;
using ProjectArena.Engine.ForExternalUse.Synchronization.ObjectInterfaces;
using ProjectArena.Engine.Helpers;
using ProjectArena.Engine.Objects.Abstract;
using ProjectArena.Engine.Synchronizers.SynchronizationObjects;

namespace ProjectArena.Engine.Synchronizers
{
    public class Synchronizer : ISynchronizer
    {
        private readonly Point tileLength;

        public int? TempDecoration { get; }

        public int? TempActor { get; }

        public int RandomCounter { get; }

        public IEnumerable<IPlayer> Players { get; }

        public IEnumerable<IActor> ChangedActors { get; }

        public IEnumerable<IActiveDecoration> ChangedDecorations { get; }

        public IEnumerable<ISpecEffect> ChangedEffects { get; }

        public IEnumerable<int> DeletedActors { get; }

        public IEnumerable<int> DeletedDecorations { get; }

        public IEnumerable<int> DeletedEffects { get; }

        public IEnumerable<ITile> ChangedTiles { get; }

        public ITile[,] TileSet
        {
            get
            {
                Tile[,] tiles = new Tile[tileLength.X, tileLength.Y];
                foreach (Tile tile in ChangedTiles)
                {
                    tiles[tile.X, tile.Y] = tile;
                }

                return tiles;
            }
        }

        public Synchronizer(
            TileObject tempObject,
            List<Player> players,
            List<Objects.Actor> changedActors,
            List<Objects.ActiveDecoration> changedDecorations,
            List<Objects.SpecEffect> changedEffects,
            List<Objects.Actor> deletedActors,
            List<Objects.ActiveDecoration> deletedDecorations,
            List<Objects.SpecEffect> deletedEffects,
            Point tileLength,
            List<Objects.Tile> changedTiles,
            int randomCounter)
        {
            if (tempObject is Objects.Actor actor)
            {
                this.TempActor = actor.Id;
            }

            if (tempObject is Objects.ActiveDecoration decoration)
            {
                this.TempDecoration = decoration.Id;
            }

            this.RandomCounter = randomCounter;
            this.tileLength = tileLength;
            this.Players = players.Select(x => new SynchronizationObjects.Player(x));
            this.ChangedActors = changedActors.Select(x => new Actor(x));
            this.ChangedDecorations = changedDecorations.Select(x => new ActiveDecoration(x));
            this.ChangedEffects = changedEffects.Select(x => new SpecEffect(x));
            List<ITile> tempChangedTiles = new List<ITile>();
            this.ChangedTiles = changedTiles.Select(x => new Tile(x));
            this.DeletedActors = deletedActors.Select(x => x.Id);
            this.DeletedDecorations = deletedDecorations.Select(x => x.Id);
            this.DeletedEffects = deletedEffects.Select(x => x.Id);
        }
    }
}
