using ProjectArena.Engine.Natives;

namespace ProjectArena.Engine.NativeManagers
{
    public interface INativeManager
    {
        TileNative GetTileNative(string id);

        ActorNative GetActorNative(string id);

        ActiveDecorationNative GetDecorationNative(string id);

        BuffNative GetBuffNative(string id);

        SkillNative GetSkillNative(string id);

        RoleModelNative GetRoleModelNative(string id);

        SpecEffectNative GetEffectNative(string id);
    }
}
