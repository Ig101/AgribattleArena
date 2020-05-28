using System;
using System.Collections.Generic;
using System.Linq;
using ProjectArena.Engine.Helpers;

namespace ProjectArena.Engine.Objects.Immaterial.Buffs
{
    public class BuffManager : IBuffManagerParentRef
    {
        private float strength;
        private float willpower;
        private float constitution;
        private float speed;
        private float maxHealth;

        public float AdditionStrength
        {
            get
            {
                return strength;
            }

            set
            {
                strength = value;
                if (Parent.Strength <= 0)
                {
                    Parent.DamageModel.Health = 0;
                    Parent.IsAlive = false;
                }
            }
        }

        public float AdditionWillpower
        {
            get
            {
                return willpower;
            }

            set
            {
                willpower = value;
                if (Parent.Willpower <= 0)
                {
                    Parent.DamageModel.Health = 0;
                    Parent.IsAlive = false;
                }
            }
        }

        public float AdditionConstitution
        {
            get
            {
                return constitution;
            }

            set
            {
                float oldHealth = Parent.MaxHealth;
                constitution = value;
                float newHealth = Parent.MaxHealth;
                if (Parent.Constitution <= 0)
                {
                    Parent.DamageModel.Health = 0;
                    Parent.IsAlive = false;
                }
                else
                {
                    Parent.DamageModel.Health *= newHealth / oldHealth;
                }
            }
        }

        public float AdditionSpeed
        {
            get
            {
                return speed;
            }

            set
            {
                float oldInitiative = Parent.Initiative;
                speed = value;
                float newInitiative = Parent.Initiative;
                if (Parent.Speed <= 0)
                {
                    Parent.DamageModel.Health = 0;
                    Parent.IsAlive = false;
                }
                else
                {
                    Parent.InitiativePosition *= oldInitiative / newInitiative;
                }
            }
        }

        public float AdditionActionPointsIncome { get; set; }

        public float AdditionMaxHealth
        {
            get
            {
                return maxHealth;
            }

            set
            {
                float oldHealth = Parent.MaxHealth;
                maxHealth = value;
                float newHealth = Parent.MaxHealth;
                if (Parent.Constitution <= 0)
                {
                    Parent.DamageModel.Health = 0;
                    Parent.IsAlive = false;
                }
                else
                {
                    Parent.DamageModel.Health *= newHealth / oldHealth;
                }
            }
        }

        public float AdditionInitiative
        {
            get
            {
                return Initiative;
            }

            set
            {
                float oldInitiative = Parent.Initiative;
                Initiative = value;
                float newInitiative = Parent.Initiative;
                if (Parent.Speed <= 0)
                {
                    Parent.DamageModel.Health = 0;
                    Parent.IsAlive = false;
                }
                else
                {
                    Parent.InitiativePosition *= oldInitiative / newInitiative;
                }
            }
        }

        public int SkillCd { get; set; }

        public int SkillCost { get; set; }

        public int SkillRange { get; set; }

        public bool CanMove { get; set; }

        public bool CanAct { get; set; }

        public IActorParentRef Parent { get; set; }

        public float Initiative { get; private set; }

        public float AttackPower { get; set; }

        public float SkillPower { get; set; }

        public int MaxHealth => (int)maxHealth;

        public int ActionPointsIncome => (int)AdditionActionPointsIncome;

        public int Strength => (int)strength;

        public int Willpower => (int)willpower;

        public int Constitution => (int)constitution;

        public int Speed => (int)speed;

        public List<TagSynergy> Armor { get; }

        public List<TagSynergy> Attack { get; }

        public List<Buff> Buffs { get; }

        public BuffManager(IActorParentRef parent)
        {
            this.Parent = parent;
            Armor = new List<TagSynergy>();
            Attack = new List<TagSynergy>();
            Buffs = new List<Buff>();
            Armor.AddRange(parent.DefaultArmor);
            CanMove = true;
            CanAct = true;
        }

        public void RemoveAllBuffs(bool includeEternal)
        {
            if (includeEternal)
            {
                Buffs.ForEach(x => x.Purge());
                Buffs.Clear();
            }
            else
            {
                for (int i = 0; i < Buffs.Count; i++)
                {
                    if (!Buffs[i].Native.Eternal)
                    {
                        Buffs[i].Purge();
                        Buffs.RemoveAt(i);
                        i--;
                    }
                }
            }

            RecalculateBuffs();
        }

        public void RemoveAllBuffsIncludeEternal()
        {
            RecalculateBuffs();
        }

        private void RecalculateBuffs()
        {
            SkillCd = 0;
            SkillCost = 0;
            SkillRange = 0;
            CanMove = true;
            CanAct = true;
            AdditionStrength = 0;
            AdditionWillpower = 0;
            AdditionConstitution = 0;
            AdditionSpeed = 0;
            Armor.Clear();
            Armor.AddRange(Parent.DefaultArmor);
            Attack.Clear();
            AdditionActionPointsIncome = 0;
            AdditionMaxHealth = 0;
            SkillPower = 0;
            AttackPower = 0;
            AdditionInitiative = 0;
            foreach (Buff buff in Buffs)
            {
                buff.Native.Applier?.Invoke(this, buff);
            }
        }

        public Buff AddBuff(Buff buff)
        {
            Parent.Affected = true;
            List<Buff> neededBuffs = Buffs.FindAll(x => x.Native.Id == buff.Native.Id);
            if (buff.Native.Repeatable <= 1 && neededBuffs.Count > 0)
            {
                Buff tempVersion = neededBuffs.First();
                if (buff.Duration != null)
                {
                    if (buff.Native.SummarizeLength)
                    {
                        tempVersion.Duration += buff.Duration;
                    }
                    else
                    {
                        tempVersion.Duration = buff.Duration;
                    }
                }

                tempVersion.Mod = Math.Max(tempVersion.Mod, buff.Mod);
                buff = tempVersion;
            }
            else
            {
                if (neededBuffs.Count >= buff.Native.Repeatable)
                {
                    Buffs.Remove(neededBuffs.OrderBy(x => x.Duration).First());
                }

                Buffs.Add(buff);
            }

            RecalculateBuffs();
            return buff;
        }

        public Buff AddBuff(string native, float? mod, float? duration)
        {
            return AddBuff(new Buff(this, Parent.Parent.NativeManager.GetBuffNative(native), mod, duration));
        }

        public Buff RemoveBuff(Buff buff)
        {
            Parent.Affected = true;
            buff.Purge();
            Buffs.Remove(buff);
            RecalculateBuffs();
            return buff;
        }

        public Buff RemoveBuff(int id)
        {
            return RemoveBuff(Buffs.Find(x => x.Id == id));
        }

        public void RemoveBuffsByTagsCondition(Func<string[], bool> condition)
        {
            Parent.Affected = true;
            for (int i = 0; i < Buffs.Count; i++)
            {
                if (condition(Buffs[i].Native.Tags))
                {
                    Buffs[i].Purge();
                    Buffs.RemoveAt(i);
                    i--;
                }
            }

            RecalculateBuffs();
        }

        public void RemoveTileBuffs()
        {
            Parent.Affected = true;
            for (int i = 0; i < Buffs.Count; i++)
            {
                if (Buffs[i].Native.OnTile)
                {
                    Buffs[i].Purge();
                    Buffs.RemoveAt(i);
                    i--;
                }
            }

            RecalculateBuffs();
        }

        public void Update(float time)
        {
            int changedBuffs = 0;
            for (int i = 0; i < Buffs.Count; i++)
            {
                Buffs[i].Update(time);
                if (Buffs[i].Duration != null && Buffs[i].Duration <= 0)
                {
                    changedBuffs++;
                    Buffs.RemoveAt(i);
                    i--;
                }
            }

            if
            (changedBuffs > 0)
            {
                Parent.Affected = true;
                RecalculateBuffs();
            }
        }
    }
}
