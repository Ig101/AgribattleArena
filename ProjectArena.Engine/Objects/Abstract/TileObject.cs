using System;
using System.Collections.Generic;
using ProjectArena.Engine.Helpers;
using ProjectArena.Engine.Natives;
using ProjectArena.Engine.Objects.Immaterial;

namespace ProjectArena.Engine.Objects.Abstract
{
    public abstract class TileObject : GameObject
    {
        public bool Affected { get; set; }

        public TaggingNative Native { get; }

        public DamageModel DamageModel { get; }

        public Tile TempTile { get; set; }

        public bool HealthRevealed { get; set; }

        public TileObject(Scene parent, Player owner, Tile tempTile, float z, DamageModel damageModel, TaggingNative native)
            : base(parent, owner, tempTile.X, tempTile.Y, z)
        {
            this.Native = native;
            this.TempTile = tempTile;
            this.DamageModel = damageModel;
            this.Affected = true;
            this.HealthRevealed = false;
        }

        public virtual bool Damage(float amount, IEnumerable<string> tags)
        {
            this.Affected = true;
            var realAmount = DamageModel.Damage(amount, tags);
            OnHitAction(realAmount);
            if (realAmount != 0)
            {
                HealthRevealed = true;
            }

            this.IsAlive = this.IsAlive && DamageModel.Health > 0;
            return !this.IsAlive;
        }

        public virtual void Kill()
        {
            this.IsAlive = false;
        }

        public void ChangePosition(Tile target)
        {
            float heightChange = target.Height - this.TempTile.Height;
            this.Affected = true;
            this.TempTile.Affected = true;
            target.Affected = true;
            this.TempTile.RemoveTempObject();
            target.ChangeTempObject(this);
            this.TempTile = target;
            this.X = target.X;
            this.Y = target.Y;
            this.Z += heightChange;
        }

        public abstract void OnHitAction(float amount);
    }
}
