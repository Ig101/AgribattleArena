using AgribattleArena.Engine.ForExternalUse.Synchronization.ObjectInterfaces;

namespace AgribattleArena.Engine.Synchronizers.SynchronizationObjects
{
    internal class Skill : ISkill
    {
        public int Id { get; }

        public int Range { get; }

        public string NativeId { get; }

        public float Cd { get; }

        public float Mod { get; }

        public int Cost { get; }

        public float PreparationTime { get; }

        public Skill(Objects.Immaterial.Skill skill)
        {
            this.Id = skill.Id;
            this.Range = skill.Range;
            this.NativeId = skill.Native.Id;
            this.Cd = skill.Cd;
            this.Mod = skill.Mod;
            this.Cost = skill.Cost;
            this.PreparationTime = skill.PreparationTime;
        }
    }
}
