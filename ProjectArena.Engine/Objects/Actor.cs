using System;
using System.Collections.Generic;
using System.Linq;
using AgribattleArena.Engine.Helpers;
using AgribattleArena.Engine.Natives;
using AgribattleArena.Engine.Objects.Abstract;
using AgribattleArena.Engine.Objects.Immaterial;
using AgribattleArena.Engine.Objects.Immaterial.Buffs;
using AgribattleArena.Engine.VarManagers;

namespace AgribattleArena.Engine.Objects
{
    public class Actor : TileObject, IActorParentRef, IActorDamageModelRef
    {
        private readonly ActorNative native;
        private readonly IVarManager varManager;
        private int actionPoints;

        public string[] Tags => native.Tags;

        public int SelfStrength { get; }

        public int SelfWillpower { get; }

        public int SelfConstitution { get; }

        public int SelfSpeed { get; }

        public int SelfActionPointsIncome { get; private set; }

        public long? ExternalId { get; }

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

        public float Initiative => (Speed * varManager.SpeedMod) + BuffManager.Initiative;

        public List<Skill> Skills { get; }

        public List<Buff> Buffs => BuffManager.Buffs;

        public TagSynergy[] DefaultArmor { get; }

        public List<TagSynergy> Armor => BuffManager.Armor;

        public List<TagSynergy> AttackModifiers => BuffManager.Attack;

        public Actor(ISceneParentRef parent, IPlayerParentRef owner, long? externalId, ITileParentRef tempTile, float? z, ActorNative native, RoleModelNative roleModelNative)
            : base(parent, owner, tempTile, z ?? native.DefaultZ, new DamageModel(), native)
        {
            this.varManager = parent.VarManager;
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
                Skills.Add(new Skill(this, skill, null, null, null, null));
            }

            this.AttackingSkill = new Skill(this, roleModelNative.AttackingSkill, 0, null, 1, null);
            this.BuffManager = new BuffManager(this);
            this.InitiativePosition += 1f / this.Initiative;
            this.DamageModel.SetupRoleModel(this);
        }

        public override void Update(float time)
        {
            this.InitiativePosition -= time;
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
                SpendActionPoints(1);
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

        public Skill AddSkill(string native, float? cd, float? mod, int? cost, int? range)
        {
            return AddSkill(new Skill(this, Parent.NativeManager.GetSkillNative(native), cd, mod, cost, range));
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
            return this.IsAlive && this.ActionPoints > 0 && !CheckStunnedState();
        }
    }
}
