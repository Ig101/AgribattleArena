using AgribattleArena.Engine.Objects.Abstract;

namespace AgribattleArena.Engine.Objects
{
    public interface ITileParentRef
    {
        ISceneParentRef Parent { get; }

        bool Affected { get; set; }

        float Height { get; set; }

        int X { get; }

        int Y { get; }

        TileObject TempObject { get; }

        bool ChangeTempObject(TileObject tileObject, bool trigger);

        bool RemoveTempObject();
    }
}
