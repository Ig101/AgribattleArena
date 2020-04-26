using ProjectArena.Engine.Objects;
using ProjectArena.Engine.Objects.Immaterial.Buffs;

namespace ProjectArena.Engine.Helpers.DelegateLists
{
    public class BuffActions
    {
        public delegate void Action(ISceneParentRef scene, IActorParentRef actor, Buff buff, float time);

        public delegate void Applier(IBuffManagerParentRef manager, Buff buff);

        public delegate void OnPurgeAction(ISceneParentRef scene, IActorParentRef actor, Buff buff);

        public static void AddMaxHealth(IBuffManagerParentRef manager, Buff buff)
        {
            manager.AdditionMaxHealth += buff.Mod;
        }

        public static void AddStrength(IBuffManagerParentRef manager, Buff buff)
        {
            manager.AdditionStrength += buff.Mod;
        }

        public static void Stun(IBuffManagerParentRef manager, Buff buff)
        {
            manager.CanMove = false;
            manager.CanAct = false;
        }

        public static void DamageSelf(ISceneParentRef scene, IActorParentRef actor, Buff buff, float time)
        {
            if (time > 0)
            {
                actor.Damage(buff.Mod * time, buff.Native.Tags);
            }
        }

        public static void DamageSelfPurge(ISceneParentRef scene, IActorParentRef actor, Buff buff)
        {
            DamageSelf(scene, actor, buff, 1);
        }

        public static void AddTestAttackAndArmor(IBuffManagerParentRef manager, Buff buff)
        {
            manager.Attack.Add(new TagSynergy("test_self_tag", "test_target_tag", buff.Mod));
            manager.Armor.Add(new TagSynergy("test_target_tag", buff.Mod));
        }

        public static void AddSpellDamage(IBuffManagerParentRef manager, Buff buff)
        {
            manager.SkillPower += buff.Mod;
        }

        public static void ReducePureResistance(IBuffManagerParentRef manager, Buff buff)
        {
            manager.Armor.Add(new TagSynergy("pure", 1 + buff.Mod));
        }
    }
}
