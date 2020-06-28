using ProjectArena.Engine.ForExternalUse.Synchronization.ObjectInterfaces;

namespace ProjectArena.Engine.Synchronizers.SynchronizationObjects
{
    internal class Tile : ITile
    {
        public int X { get; }

        public int Y { get; }

        public string OwnerId { get; }

        public int? Team { get; }

        public int? TempActorId { get; }

        public float Height { get; }

        public string NativeId { get; }

        public bool Unbearable { get; }

        public bool Revealed { get; }

        public Tile(Objects.Tile tile)
        {
            this.X = tile.X;
            this.Y = tile.Y;
            this.OwnerId = tile.Owner?.Id;
            this.Team = tile.Owner?.Team;
            this.TempActorId = tile.TempObject?.Id;
            this.Height = tile.Height;
            this.NativeId = tile.Native.Id;
            this.Unbearable = tile.Native.Unbearable;
            this.Revealed = tile.Revealed;
        }
    }
}
