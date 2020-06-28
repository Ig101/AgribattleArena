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
    public class Actor : TileObject, IActorParentRef, IActorDamageModelRef
    {
        private readonly ActorNative native;
        private readonly IVarManager varManager;
        private int actionPoints;

        public string[] Tags => native.Tags;

        public string Visualization { get; set; }

        public string EnemyVisualization { get; set; }

        public int SelfStrength { get; }

        public int SelfWillpower { get; }

        public int SelfConstitution { get; }

        public int SelfSpeed { get; }

        public int SelfActionPointsIncome { get; private set; }

        public Guid? ExternalId { get; }

        public Skill AttackingSkill { get; }

        public BuffManager BuffManager { get; }

        public int Strength => SelfStrength + BuffManager.Strength;

        public int Willpower => SelfWillpower + BuffManager.Willpower;

        public int Constitution => SelfConstitution + BuffManager.Constitution;

        public int Speed => SelfSpeed + BuffManager.Speed;

        public int MaxHealth => (Constitution * varManager.ConstitutionMod) + BuffManager.MaxHealth;

        public int ActionPointsIncome
        {
            get { return SelfActionPointsIncome + BuffManager.ActionPointsIncome; } set { SelfActionPointsIncome = value; }
        }

        public int ActionPoints
        {
            get { return actionPoints; } set { actionPoints = value > varManager.MaxActionPoints ? varManager.MaxActionPoints : value < 0 ? 0 : value; }
        }

        public float SkillPower => (Willpower * varManager.WillpowerMod) + BuffManager.SkillPower;

        public float AttackPower => (Strength * varManager.StrengthMod) + BuffManager.AttackPower;

        public float Initiative => varManager.MinimumInitiative + (Speed * varManager.SpeedMod) + BuffManager.Initiative;

        public List<Skill> Skills { get; }

        public List<Buff> Buffs => BuffManager.Buffs;

        public TagSynergy[] DefaultArmor { get; }

        public List<TagSynergy> Armor => BuffManager.Armor;

        public List<TagSynergy> AttackModifiers => BuffManager.Attack;

        public Actor(ISceneParentRef parent, IPlayerParentRef owner, Guid? externalId, ITileParentRef tempTile, string visualization, string enemyVisualization, float? z, ActorNative native, RoleModelNative roleModelNative)
            : base(parent, owner, tempTile, z ?? native.DefaultZ, new DamageModel(), native)
        {
            this.varManager = parent.VarManager;
            this.Visualization = visualization ?? native.DefaultVisualization;
            this.EnemyVisualization = enemyVisualization ?? native.DefaultEnemyVisualization;
            this.ExternalId = externalId;
            this.native = native;
            this.SelfStrength = roleModelNative.DefaultStrength;
            this.SelfWillpower = roleModelNative.DefaultWillpower;
            this.SelfConstitution = roleModelNative.DefaultConstitution;
            this.SelfSpeed = roleModelNative.DefaultSpeed;
            this.SelfActionPointsIncome = roleModelNative.DefaultActionPointsIncome;
            this.DefaultArmor = native.Armor.ToArray();
            this.Skills = new List<Skill>();
            foreach (SkillNative skill in roleModelNative.Skills)
            {
                Skills.Add(new Skill(this, skill, null, null, null, null, null, null));
            }

            this.AttackingSkill = new Skill(this, roleModelNative.AttackingSkill, null, null, 0, null, null, null);
            this.BuffManager = new BuffManager(this);
            this.InitiativePosition += 1f / this.Initiative;
            this.DamageModel.SetupRoleModel(this);
        }

        public override void Update(float time)
        {
            this.InitiativePosition -= time;
            if (time > 0)
            {
                this.Affected = true;
            }

            BuffManager.Update(time);
            foreach (Skill skill in Skills)
            {
                skill.Update(time);
            }
        }

        public bool Attack(Tile target)
        {
            return AttackingSkill.Cast(target);
        }

        public bool Cast(int id, Tile target)
        {
            Skill skill = Skills.Find(x => x.Id == id);
            return skill.Cast(target);
        }

        public bool Wait()
        {
            return true;
        }

        public bool Move(Tile target)
        {
            if (target.TempObject == null && !target.Native.Unbearable && Math.Abs(target.Height - this.TempTile.Height) < 10 &&
                BuffManager.CanMove && ((target.X == X && Math.Abs(target.Y - Y) == 1) ||
                (target.Y == Y && Math.Abs(target.X - X) == 1)))
            {
                ChangePosition(target, true);
                return true;
            }

            return false;
        }

        public override void EndTurn()
        {
            if (Parent.TempTileObject == this)
            {
                this.InitiativePosition += 1f / this.Initiative;
            }
        }

        public override bool StartTurn()
        {
            if (Parent.TempTileObject == this)
            {
                this.Affected = true;
                this.ActionPoints += ActionPointsIncome;
                return CheckActionAvailability();
            }

            return false;
        }

        public override void OnDeathAction()
        {
            BuffManager.RemoveAllBuffs(false);
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

        public void SpendActionPoints(int amount)
        {
            this.ActionPoints -= amount;
            this.Affected = true;
        }

        private bool CheckStunnedState()
        {
            if (!BuffManager.CanAct && !BuffManager.CanMove)
            {
                this.actionPoints = 0;
                return true;
            }

            return false;
        }

        public bool CheckActionAvailability()
        {
            return this.IsAlive && !CheckStunnedState();
        }
    }
}
