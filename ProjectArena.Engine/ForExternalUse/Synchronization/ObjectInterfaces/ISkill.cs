namespace AgribattleArena.Engine.ForExternalUse.Synchronization.ObjectInterfaces
{
    public interface ISkill
    {
        int Id { get; }

        int Range { get; }

        string NativeId { get; }

        float Cd { get; }

        float Mod { get; }

        int Cost { get; }

        float PreparationTime { get; }
    }
}
