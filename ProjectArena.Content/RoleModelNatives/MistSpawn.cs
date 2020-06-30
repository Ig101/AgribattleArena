using ProjectArena.Engine.ForExternalUse;

namespace ProjectArena.Content.RoleModelNatives
{
    public class MistSpawn : INative
    {
        public void Fill(INativeManager nativeManager)
        {
            nativeManager.AddRoleModelNative(
                "mistspawn",
                12,
                2,
                12,
                20,
                4,
                "slash",
                new string[0]);
        }
    }
}