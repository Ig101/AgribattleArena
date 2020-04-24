using System.Collections.Generic;
using ProjectArena.Engine.Helpers;
using ProjectArena.Engine.Objects;

namespace ProjectArena.Engine
{
    public class Player : IPlayerParentRef, ForExternalUse.IPlayerShort
    {
        public int? Team { get; }

        public ISceneParentRef Parent { get; }

        public string Id { get; }

        public List<Actor> KeyActors { get; }

        public int TurnsSkipped { get; private set; }

        public PlayerStatus Status { get; set; }

        public Player(ISceneParentRef parent, string id, int? team)
        {
            this.Team = team;
            this.Parent = parent;
            this.Id = id;
            this.KeyActors = new List<Actor>();
            this.TurnsSkipped = 0;
            this.Status = PlayerStatus.Playing;
        }

        public void SkipTurn()
        {
            TurnsSkipped++;
        }

        public bool ActThisTurn()
        {
            if (TurnsSkipped > 0)
            {
                TurnsSkipped = 0;
                return true;
            }

            return false;
        }

        public void Defeat()
        {
            Status = PlayerStatus.Defeated;
            Parent.GetPlayerActors(this).ForEach(x => x.Kill());
        }

        public void Victory()
        {
            Status = PlayerStatus.Victorious;
        }
    }
}
