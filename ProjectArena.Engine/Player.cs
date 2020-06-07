using System;
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

        public int? StatusHash { get; set; }

        public bool StatusRedeemed { get; set; }

        public bool Left { get; set; }

        public Player(ISceneParentRef parent, string id, int? team)
        {
            this.Team = team;
            this.Parent = parent;
            this.Id = id;
            this.KeyActors = new List<Actor>();
            this.TurnsSkipped = 0;
            this.Status = PlayerStatus.Playing;
            this.Left = false;
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

        public void Defeat(bool leave)
        {
            Status = leave ? PlayerStatus.Left : PlayerStatus.Defeated;
            StatusHash = Guid.NewGuid().GetHashCode();
            Parent.GetPlayerActors(this).ForEach(x => x.Kill());
            this.StatusRedeemed = false;
        }

        public void Victory()
        {
            Status = PlayerStatus.Victorious;
            StatusHash = Guid.NewGuid().GetHashCode();
            this.StatusRedeemed = false;
        }

        public IEnumerable<int> GetPlayerActors()
        {
            return Parent.GetPlayerActors(this.Id);
        }

        public bool TryRedeemPlayerStatusHash(out int? hash)
        {
            if (Status != PlayerStatus.Playing && !StatusRedeemed)
            {
                StatusRedeemed = true;
                hash = StatusHash;
                return true;
            }

            hash = StatusHash;
            return false;
        }
    }
}
