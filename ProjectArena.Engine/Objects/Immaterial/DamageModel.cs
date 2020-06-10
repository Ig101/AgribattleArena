using System;
using System.Collections.Generic;
using ProjectArena.Engine.Helpers;
using ProjectArena.Engine.Objects.Immaterial.Buffs;

namespace ProjectArena.Engine.Objects.Immaterial
{
    public class DamageModel
    {
        private readonly int? maxHealth;
        private readonly TagSynergy[] armor;
        private IActorDamageModelRef roleModel;
        private float health;

        public List<Buff> Buffs => roleModel?.Buffs;

        public int MaxHealth => roleModel?.MaxHealth ?? maxHealth.Value;

        public TagSynergy[] Armor => roleModel?.Armor.ToArray() ?? armor;

        public float Health
        {
            get { return health; } set { health = Math.Min(value, MaxHealth); }
        }

        public DamageModel(int maxHealth, TagSynergy[] armor)
        {
            this.roleModel = null;
            this.maxHealth = maxHealth;
            this.armor = armor;
            this.health = maxHealth;
        }

        public DamageModel()
        {
        }

        public void SetupRoleModel(IActorDamageModelRef model)
        {
            this.roleModel = model;
            this.health = MaxHealth;
        }

        public bool Damage(float amount, IEnumerable<string> tags)
        {
            if (tags != null && Armor != null)
            {
                foreach (string atackerTag in tags)
                {
                    foreach (TagSynergy synergy in Armor)
                    {
                        if (synergy.TargetTag == atackerTag)
                        {
                            amount *= synergy.Mod;
                        }
                    }
                }
            }

            Health -= amount;
            return amount != 0;
        }
    }
}
