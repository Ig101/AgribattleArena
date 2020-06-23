using ProjectArena.Engine.Helpers;

namespace ProjectArena.Engine.Natives
{
    public class ActorNative : TaggingNative
    {
        public float DefaultZ { get; }

        public string DefaultVisualization { get; }

        public string DefaultEnemyVisualization { get; }

        public TagSynergy[] Armor { get; }

        public ActorNative(string id, string defaultVisualization, string defaultEnemyVisualization, string[] tags, float defaultZ, TagSynergy[] armor)
            : base(id, tags)
        {
            this.DefaultVisualization = defaultVisualization;
            this.DefaultEnemyVisualization = defaultEnemyVisualization;
            this.DefaultZ = defaultZ;
            this.Armor = armor;
        }
    }
}
