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

        void SetupEnemyActorsPrefix(string prefix);

        Tile[][] SetupEmptyTileSet(int width, int height);

        Player CreatePlayer(string id, int? team);

        Actor CreateActor(Player owner, string nativeName, string roleNativeName, Tile target, float? z);

        Actor CreateActor(Player owner, Guid? externalId, string nativeName, RoleModelNative roleModel, Tile target, float? z);

        ActiveDecoration CreateDecoration(Player owner, string nativeName, Tile target, TagSynergy[] armor, float? z, int? health, float? mod);

        SpecEffect CreateEffect(Player owner, string nativeName, Tile target, float? z, float? duration, float? mod);

        Tile CreateTile(string nativeName, int x, int y, int? height);
    }
}
