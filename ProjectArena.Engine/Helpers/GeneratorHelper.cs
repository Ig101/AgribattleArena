using System.Linq;
using ProjectArena.Engine.Natives;
using ProjectArena.Engine.Objects;
using ProjectArena.Engine.Objects.Immaterial.Buffs;

namespace ProjectArena.Engine.Helpers
{
    public static class GeneratorHelper
    {
        public static Player ConvertExternalPlayerFromGeneration(ISceneForSceneGenerator scene, ForExternalUse.Generation.ObjectInterfaces.IPlayer player, int? team)
        {
            return scene.CreatePlayer(player.Id, player.UserId, team);
        }

        public static Actor ConvertExternalActorFromGeneration(ISceneForSceneGenerator scene, Player player, Tile target, ForExternalUse.Generation.ObjectInterfaces.IActor actor, float? z)
        {
            Actor newActor = scene.CreateActor(
                player,
                actor.ExternalId,
                actor.NativeId,
                new RoleModelNative(scene.NativeManager, null, actor.Strength, actor.Willpower, actor.Constitution, actor.Speed, actor.ActionPointsIncome, actor.AttackingSkillName, actor.SkillNames.ToArray()),
                target,
                z);
            if (actor.StartBuffs != null)
            {
                foreach (string buffName in actor.StartBuffs)
                {
                    newActor.BuffManager.AddBuff(new Buff(newActor.BuffManager, scene.NativeManager.GetBuffNative(buffName), null, null));
                }
            }

            return newActor;
        }
    }
}
