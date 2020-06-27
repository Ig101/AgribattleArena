using System;
using System.Collections.Generic;
using ProjectArena.Engine.Helpers;
using ProjectArena.Engine.Objects;

namespace ProjectArena.Engine
{
    public class Player : ForExternalUse.IPlayerShort
    {
        public int? Team { get; }

        public Scene Parent { get; }

        public string Id { get; }

        public string UserId { get; }

        public Actor PlayerActor { get; }

        public List<Actor> KeyActors { get; }

        public PlayerStatus Status { get; set; }

        public int? StatusHash { get; set; }

        public bool StatusRedeemed { get; set; }

        public bool Left { get; set; }

        public Player(Scene parent, string id, string userId, int? team)
        {
            this.Team = team;
            this.Parent = parent;
            this.Id = id;
            this.UserId = userId;
            this.KeyActors = new List<Actor>();
            this.Status = PlayerStatus.Playing;
            this.Left = false;
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
