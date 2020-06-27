using System;
using ProjectArena.Engine.Helpers;
using ProjectArena.Engine.Objects;

namespace ProjectArena.Engine.Natives
{
    public class ActorNative : TaggingNative
    {
        public float DefaultZ { get; }

        public string DefaultVisualization { get; }

        public string DefaultEnemyVisualization { get; }

        public TagSynergy[] Armor { get; }

        public Action<Scene, Actor, float> OnHitAction { get; set; }

        public Action<Scene, Actor> OnCastAction { get; set; }

        public Action<Scene, Actor> OnDeathAction { get; set; }

        public ActorNative(
            string id,
            string defaultVisualization,
            string defaultEnemyVisualization,
            string[] tags,
            float defaultZ,
            TagSynergy[] armor,
            Action<Scene, Actor, float> onHitAction,
            Action<Scene, Actor> onCastAction,
            Action<Scene, Actor> onDeathAction)
            : base(id, tags)
        {
            this.DefaultVisualization = defaultVisualization;
            this.DefaultEnemyVisualization = defaultEnemyVisualization;
            this.DefaultZ = defaultZ;
            this.Armor = armor;
            this.OnHitAction = onHitAction;
            this.OnCastAction = onCastAction;
            this.OnDeathAction = onDeathAction;
        }
    }
}
