using System.Collections.Generic;
using System.Linq;
using AgribattleArena.Engine.ForExternalUse.Synchronization;
using AgribattleArena.Engine.ForExternalUse.Synchronization.ObjectInterfaces;
using AgribattleArena.Engine.Helpers;
using AgribattleArena.Engine.Objects.Abstract;
using AgribattleArena.Engine.Synchronizers.SynchronizationObjects;

namespace AgribattleArena.Engine.Synchronizers
{
    public class Synchronizer : ISynchronizer
    {
        private Point tileLength;

        public IActiveDecoration TempDecoration { get; }

        public IActor TempActor { get; }

        public int RandomCounter { get; }

        public IEnumerable<IPlayer> Players { get; }

        public IEnumerable<IActor> ChangedActors { get; }

        public IEnumerable<IActiveDecoration> ChangedDecorations { get; }

        public IEnumerable<ISpecEffect> ChangedEffects { get; }

        public IEnumerable<IActor> DeletedActors { get; }

        public IEnumerable<IActiveDecoration> DeletedDecorations { get; }

        public IEnumerable<ISpecEffect> DeletedEffects { get; }

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
                this.TempActor = new Actor(actor);
            }

            if (tempObject is Objects.ActiveDecoration decoration)
            {
                this.TempDecoration = new ActiveDecoration(decoration);
            }

            this.RandomCounter = randomCounter;
            this.tileLength = tileLength;
            this.Players = players.Select(x => new SynchronizationObjects.Player(x));
            this.ChangedActors = changedActors.Select(x => new Actor(x));
            this.ChangedDecorations = changedDecorations.Select(x => new ActiveDecoration(x));
            this.ChangedEffects = changedEffects.Select(x => new SpecEffect(x));
            List<ITile> tempChangedTiles = new List<ITile>();
            this.ChangedTiles = changedTiles.Select(x => new Tile(x));
            this.DeletedActors = deletedActors.Select(x => new Actor(x));
            this.DeletedDecorations = deletedDecorations.Select(x => new ActiveDecoration(x));
            this.DeletedEffects = deletedEffects.Select(x => new SpecEffect(x));
        }
    }
}
