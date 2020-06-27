using System;
using ProjectArena.Engine.ForExternalUse;
using ProjectArena.Engine.Helpers;
using ProjectArena.Engine.Natives;
using ProjectArena.Engine.Objects;

namespace ProjectArena.Engine
{
    public interface ISceneForSceneGenerator
    {
        VarManagers.IVarManager VarManager { get; }

        NativeManagers.INativeManager NativeManager { get; }

        Tile[][] SetupEmptyTileSet(int width, int height);

        Player CreatePlayer(string id, string userId, int? team);

        Actor CreateActor(Player owner, string nativeName, string roleNativeName, Tile target, string visualization, string enemyVisualization, float? z);

        Actor CreateActor(Player owner, Guid? externalId, string nativeName, RoleModelNative roleModel, Tile target, string visualization, string enemyVisualization, float? z);

        ActiveDecoration CreateDecoration(Player owner, string nativeName, Tile target, TagSynergy[] armor, string visualization, float? z, int? health, float? mod);

        Tile CreateTile(string nativeName, int x, int y, int? height);
    }
}
