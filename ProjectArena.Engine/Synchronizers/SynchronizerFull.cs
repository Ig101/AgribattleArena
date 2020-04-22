using System.Collections.Generic;
using System.Linq;
using AgribattleArena.Engine.ForExternalUse.Synchronization;
using AgribattleArena.Engine.ForExternalUse.Synchronization.ObjectInterfaces;
using AgribattleArena.Engine.Objects.Abstract;
using AgribattleArena.Engine.Synchronizers.SynchronizationObjects;

namespace AgribattleArena.Engine.Synchronizers
{
    public class SynchronizerFull : ISynchronizer
    {
        public int RandomCounter { get; }

        public IActiveDecoration TempDecoration { get; }

        public IActor TempActor { get; }

        public IEnumerable<IPlayer> Players { get; }

        public IEnumerable<IActor> ChangedActors { get; }

        public IEnumerable<IActiveDecoration> ChangedDecorations { get; }

        public IEnumerable<ISpecEffect> ChangedEffects { get; }

        public IEnumerable<IActor> DeletedActors => new List<IActor>();

        public IEnumerable<IActiveDecoration> DeletedDecorations => new List<IActiveDecoration>();

        public IEnumerable<ISpecEffect> DeletedEffects => new List<ISpecEffect>();

        public IEnumerable<ITile> ChangedTiles => TileSet.Cast<ITile>().ToList();

        public ITile[,] TileSet { get; }

        public SynchronizerFull(
            TileObject tempObject,
            List<Player> players,
            List<Objects.Actor> actors,
            List<Objects.ActiveDecoration> decorations,
            List<Objects.SpecEffect> effects,
            Objects.Tile[][] tiles,
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
            this.Players = players.Select(x => new SynchronizationObjects.Player(x));
            this.ChangedDecorations = decorations.Select(x => new ActiveDecoration(x));
            this.ChangedActors = actors.Select(x => new Actor(x));
            this.ChangedEffects = effects.Select(x => new SpecEffect(x));
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
