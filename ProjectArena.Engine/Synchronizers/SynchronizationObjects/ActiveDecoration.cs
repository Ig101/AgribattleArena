using System.Collections.Generic;
using ProjectArena.Engine.ForExternalUse.Synchronization.ObjectInterfaces;
using ProjectArena.Engine.Helpers;

namespace ProjectArena.Engine.Synchronizers.SynchronizationObjects
{
    internal class ActiveDecoration : IActiveDecoration
    {
        public int Id { get; }

        public string NativeId { get; }

        public string Visualization { get; }

        public float Health { get; }

        public float MaxHealth { get; }

        public string OwnerId { get; }

        public int? Team { get; }

        public bool IsAlive { get; }

        public int X { get; }

        public int Y { get; }

        public float Z { get; }

        public bool HealthRevealed { get; }

        public ActiveDecoration(Objects.ActiveDecoration decoration)
        {
            this.Id = decoration.Id;
            this.Visualization = decoration.Visualization;
            this.NativeId = decoration.Native.Id;
            this.Health = decoration.DamageModel.Health;
            this.MaxHealth = decoration.DamageModel.MaxHealth;
            this.OwnerId = decoration.Owner?.Id;
            this.Team = decoration.Owner?.Team;
            this.IsAlive = decoration.IsAlive;
            this.X = decoration.X;
            this.Y = decoration.Y;
            this.Z = decoration.Z;
            this.HealthRevealed = decoration.HealthRevealed;
        }
    }
}
