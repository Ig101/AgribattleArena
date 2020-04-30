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

        public int? TempDecoration { get; }

        public int? TempActor { get; }

        public IEnumerable<IPlayer> Players { get; }

        public IEnumerable<IActor> ChangedActors { get; }

        public IEnumerable<IActiveDecoration> ChangedDecorations { get; }

        public IEnumerable<ISpecEffect> ChangedEffects { get; }

        public IEnumerable<int> DeletedActors => new List<int>();

        public IEnumerable<int> DeletedDecorations => new List<int>();

        public IEnumerable<int> DeletedEffects => new List<int>();

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
                this.TempActor = actor.Id;
            }

            if (tempObject is Objects.ActiveDecoration decoration)
            {
                this.TempDecoration = decoration.Id;
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
