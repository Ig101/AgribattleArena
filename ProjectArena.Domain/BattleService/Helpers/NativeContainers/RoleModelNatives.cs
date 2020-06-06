using ProjectArena.Engine.ForExternalUse;

namespace ProjectArena.Domain.BattleService.Helpers.NativeContainers
{
    public static class RoleModelNatives
    {
        public static void FillRoleModelNatives(this INativeManager nativeManager)
        {
            nativeManager.AddRoleModelNative(
                "mistspawn",
                12,
                2,
                12,
                20,
                6,
                "slash",
                new string[0]);
        }
    }
}