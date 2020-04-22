using AgribattleArena.Engine.Objects;

namespace AgribattleArena.Engine.Helpers.DelegateLists
{
    public class ActiveDecorationActions
    {
        public delegate void Action(ISceneParentRef scene, ActiveDecoration activeDecoration);

        public static void DoSelfDamage(ISceneParentRef scene, ActiveDecoration activeDecoration)
        {
            activeDecoration.Damage(activeDecoration.Mod, activeDecoration.Native.Tags);
        }
    }
}
