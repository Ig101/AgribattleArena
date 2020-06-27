using ProjectArena.Engine.Helpers;

namespace ProjectArena.Engine.ForExternalUse.Synchronization.ObjectInterfaces
{
    public interface ISkill
    {
        int Id { get; }

        int Range { get; }

        string NativeId { get; }

        string Visualization { get; }

        string EnemyVisualization { get; }

        float Cd { get; }

        float Mod { get; }

        float PreparationTime { get; }

        Targets AvailableTargets { get; }

        bool Revealed { get; }
    }
}
