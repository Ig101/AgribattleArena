using ProjectArena.Engine.ForExternalUse.Synchronization.ObjectInterfaces;
using ProjectArena.Engine.Helpers;

namespace ProjectArena.Engine.Synchronizers.SynchronizationObjects
{
    internal class Skill : ISkill
    {
        public int Id { get; }

        public int Range { get; }

        public string NativeId { get; }

        public string Visualization { get; }

        public string EnemyVisualization { get; }

        public float Cd { get; }

        public float Mod { get; }

        public int Cost { get; }

        public float PreparationTime { get; }

        public Targets AvailableTargets { get; }

        public bool OnlyVisibleTargets { get; }

        public bool Revealed { get; }

        public Skill(Objects.Immaterial.Skill skill)
        {
            this.Id = skill.Id;
            this.Visualization = skill.Visualization;
            this.EnemyVisualization = skill.EnemyVisualization;
            this.Range = skill.Range;
            this.NativeId = skill.Native.Id;
            this.Cd = skill.Cd;
            this.Mod = skill.Mod;
            this.Cost = skill.Cost;
            this.PreparationTime = skill.PreparationTime;
            this.OnlyVisibleTargets = skill.Native.OnlyVisibleTargets;
            this.AvailableTargets = skill.Native.AvailableTargets;
            this.Revealed = skill.Revealed;
        }
    }
}
