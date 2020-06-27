using System;
using System.Collections.Generic;
using System.Linq;
using ProjectArena.Engine.Helpers;
using ProjectArena.Engine.Natives;
using ProjectArena.Engine.Objects.Abstract;
using ProjectArena.Engine.Objects.Immaterial;
using ProjectArena.Engine.Objects.Immaterial.Buffs;
using ProjectArena.Engine.VarManagers;

namespace ProjectArena.Engine.Objects
{
    public class Actor : TileObject
    {
        private readonly IVarManager varManager;

        public OrderModel Order { get; set; }

        public new ActorNative Native => (ActorNative)base.Native;

        public bool Acted { get; set; }

        public string[] Tags => Native.Tags;

        public string Visualization { get; set; }

        public string EnemyVisualization { get; set; }

        public int SelfStrength { get; }

        public int SelfWillpower { get; }

        public int SelfConstitution { get; }

        public int SelfSpeed { get; }

        public Guid? ExternalId { get; }

        public Skill AttackingSkill { get; }

        public BuffManager BuffManager { get; }

        public int Strength => SelfStrength + BuffManager.Strength;

        public int Willpower => SelfWillpower + BuffManager.Willpower;

        public int Constitution => SelfConstitution + BuffManager.Constitution;

        public int Speed => SelfSpeed + BuffManager.Speed;

        public int MaxHealth => (Constitution * varManager.ConstitutionMod) + BuffManager.MaxHealth;

        public float SkillPower => (Willpower * varManager.WillpowerMod) + BuffManager.SkillPower;

        public float AttackPower => (Strength * varManager.StrengthMod) + BuffManager.AttackPower;

        public float Initiative => varManager.MinimumInitiative + (Speed * varManager.SpeedMod) + BuffManager.Initiative;

        public List<Skill> Skills { get; }

        public List<Buff> Buffs => BuffManager.Buffs;

        public TagSynergy[] DefaultArmor { get; }

        public List<TagSynergy> Armor => BuffManager.Armor;

        public List<TagSynergy> AttackModifiers => BuffManager.Attack;

        public Action<Scene, Actor> OnCastAction => Native.OnCastAction + BuffManager.OnCastAction;

        public Actor(Scene parent, Player owner, Guid? externalId, Tile tempTile, string visualization, string enemyVisualization, float? z, ActorNative native, RoleModelNative roleModelNative)
            : base(parent, owner, tempTile, z ?? native.DefaultZ, new DamageModel(), native)
        {
            this.varManager = parent.VarManager;
            this.Visualization = visualization ?? native.DefaultVisualization;
            this.EnemyVisualization = enemyVisualization ?? native.DefaultEnemyVisualization;
            this.ExternalId = externalId;
            this.SelfStrength = roleModelNative.DefaultStrength;
            this.SelfWillpower = roleModelNative.DefaultWillpower;
            this.SelfConstitution = roleModelNative.DefaultConstitution;
            this.SelfSpeed = roleModelNative.DefaultSpeed;
            this.DefaultArmor = native.Armor.ToArray();
            this.Skills = new List<Skill>();
            foreach (SkillNative skill in roleModelNative.Skills)
            {
                Skills.Add(new Skill(this, skill, null, null, null, null, null, null));
            }

            this.AttackingSkill = new Skill(this, roleModelNative.AttackingSkill, null, null, 0, null, null, null);
            this.BuffManager = new BuffManager(this);
            this.DamageModel.SetupRoleModel(this);
        }

        public void Update()
        {
            Acted = false;
            Affected = true;
            if (Order != null)
            {
                Order.Intended = false;
            }

            BuffManager.Update();
            foreach (Skill skill in Skills)
            {
                skill.Update();
            }
        }

        public bool Attack(Tile target)
        {
            var result = AttackingSkill.Cast(target);
            if (result)
            {
                Acted = true;
            }

            return result;
        }

        public bool Cast(int id, Tile target)
        {
            Skill skill = Skills.Find(x => x.Id == id);
            var result = skill.Cast(target);
            if (result)
            {
                Acted = true;
            }

            return result;
        }

        public bool Move(Tile target)
        {
            if (target.TempObject == null && !target.Native.Unbearable && Math.Abs(target.Height - this.TempTile.Height) < 10 &&
                BuffManager.CanMove && Math.Abs(target.X - X) <= 1 && Math.Abs(target.Y - Y) <= 1 && target != this.TempTile)
            {
                ChangePosition(target);
                return true;
            }

            return false;
        }

        public override void OnDeathAction()
        {
            TempTile.Native.OnDeathAction?.Invoke(Parent, TempTile, this);
            Native.OnDeathAction?.Invoke(Parent, this);
            BuffManager.OnDeathAction?.Invoke(Parent, this);
            BuffManager.RemoveAllBuffs(false);
        }

        public override void OnHitAction(float amount)
        {
            Native.OnHitAction?.Invoke(Parent, this, amount);
            BuffManager.OnHitAction?.Invoke(Parent, this, amount);
        }

        public Skill AddSkill(Skill skill)
        {
            Affected = true;
            Skills.Add(skill);
            return skill;
        }

        public Skill AddSkill(string native, string visualization, string enemyVisualization, float? cd, float? mod, int? cost, int? range)
        {
            return AddSkill(new Skill(this, Parent.NativeManager.GetSkillNative(native), visualization, enemyVisualization, cd, mod, cost, range));
        }

        public Skill RemoveSkill(Skill skill)
        {
            Affected = true;
            Skills.Remove(skill);
            return skill;
        }

        public Skill RemoveSkill(uint id)
        {
            return RemoveSkill(Skills.Find(x => x.Id == id));
        }

        public bool CheckActionAvailability()
        {
            return this.IsAlive && this.Acted && BuffManager.CanAct;
        }

        public bool CheckMoveAvailability()
        {
            return this.IsAlive && BuffManager.CanMove;
        }

        public void SetOrderModel(bool intended, int? skillId, int x, int y)
        {
            var skill = skillId.HasValue ? Skills.FirstOrDefault(s => s.Id == skillId) : null;
            if (Order == null)
            {
                Order = new OrderModel(intended, skill, x, y);
            }
            else
            {
                Order.Intended = intended;
                Order.Skill = skill;
                Order.X = x;
                Order.Y = y;
            }
        }

        public void SetOrderModel(bool intended, int? skillId, TileObject target)
        {
            var skill = skillId.HasValue ? Skills.FirstOrDefault(s => s.Id == skillId) : null;
            if (Order == null)
            {
                Order = new OrderModel(intended, skill, target);
            }
            else
            {
                Order.Intended = intended;
                Order.Skill = skill;
                Order.Target = target;
            }
        }
    }
}
