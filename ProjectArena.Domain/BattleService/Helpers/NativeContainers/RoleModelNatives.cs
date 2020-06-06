using ProjectArena.Engine.ForExternalUse;

namespace ProjectArena.Domain.BattleService.Helpers.NativeContainers
{
    public static class RoleModelNatives
    {
        public static void FillRoleModelNatives(this INativeManager nativeManager)
        {
            nativeManager.AddRoleModelNative(
                "mistspawn",
                8,
                0,
                8,
                20,
                6,
                "slash",
                new string[0]);
        }
    }
}