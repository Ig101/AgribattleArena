using AgribattleArena.Engine.Helpers;

namespace AgribattleArena.Engine.Natives
{
    public class ActorNative : TaggingNative
    {
        public float DefaultZ { get; }

        public TagSynergy[] Armor { get; }

        public ActorNative(string id, string[] tags, float defaultZ, TagSynergy[] armor)
            : base(id, tags)
        {
            this.DefaultZ = defaultZ;
            this.Armor = armor;
        }
    }
}
