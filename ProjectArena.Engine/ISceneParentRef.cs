using System;
using System.Collections.Generic;
using ProjectArena.Engine.Helpers;
using ProjectArena.Engine.NativeManagers;
using ProjectArena.Engine.Natives;
using ProjectArena.Engine.Objects;
using ProjectArena.Engine.Objects.Abstract;
using ProjectArena.Engine.VarManagers;

namespace ProjectArena.Engine
{
    public interface ISceneParentRef
    {
        IVarManager VarManager { get; }

        INativeManager NativeManager { get; }

        TileObject TempTileObject { get; }

        Tile[][] Tiles { get; }

        List<Actor> Actors { get; }

        List<ActiveDecoration> Decorations { get; }

        List<SpecEffect> SpecEffects { get; }

        IEnumerable<Player> Players { get; }

        int GetNextId();

        float GetNextRandom();

        IEnumerable<int> GetPlayerActors(string playerId);

        void EndTurn(bool firstTurn = false);

        bool DecorationCast(ActiveDecoration actor);

        Actor CreateActor(Player owner, string nativeName, string roleNativeName, Tile target, float? z);

        Actor CreateActor(Player owner, Guid? externalId, string nativeName, RoleModelNative roleModel, Tile target, float? z);

        ActiveDecoration CreateDecoration(Player owner, string nativeName, Tile target, TagSynergy[] armor, float? z, int? health, float? mod);

        SpecEffect CreateEffect(Player owner, string nativeName, Tile target, float? z, float? duration, float? mod);

        Tile ChangeTile(string nativeName, int x, int y, float? height, IPlayerParentRef initiator);

        List<Actor> GetPlayerActors(Player player);

        Actor ResurrectActor(Actor actor, Tile target, int health);
    }
}
