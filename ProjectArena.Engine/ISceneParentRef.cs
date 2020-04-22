using System.Collections.Generic;
using AgribattleArena.Engine.Helpers;
using AgribattleArena.Engine.NativeManagers;
using AgribattleArena.Engine.Natives;
using AgribattleArena.Engine.Objects;
using AgribattleArena.Engine.Objects.Abstract;
using AgribattleArena.Engine.VarManagers;

namespace AgribattleArena.Engine
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

        void EndTurn();

        bool DecorationCast(ActiveDecoration actor);

        Actor CreateActor(Player owner, string nativeName, string roleNativeName, Tile target, float? z);

        Actor CreateActor(Player owner, long? externalId, string nativeName, RoleModelNative roleModel, Tile target, float? z);

        ActiveDecoration CreateDecoration(Player owner, string nativeName, Tile target, float? z, int? health, TagSynergy[] armor, float? mod);

        SpecEffect CreateEffect(Player owner, string nativeName, Tile target, float? z, float? duration, float? mod);

        Tile ChangeTile(string nativeName, int x, int y, float? height, IPlayerParentRef initiator);

        List<Actor> GetPlayerActors(Player player);

        Actor ResurrectActor(Actor actor, Tile target, int health);
    }
}
