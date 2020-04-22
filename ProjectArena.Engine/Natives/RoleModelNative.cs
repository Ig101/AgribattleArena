using AgribattleArena.Engine.NativeManagers;

namespace AgribattleArena.Engine.Natives
{
    public class RoleModelNative
    {
        public string Id { get; }

        public SkillNative AttackingSkill { get; }

        public int DefaultStrength { get; }

        public int DefaultWillpower { get; }

        public int DefaultConstitution { get; }

        public int DefaultSpeed { get; }

        public SkillNative[] Skills { get; }

        public int DefaultActionPointsIncome { get; }

        public RoleModelNative(string id, int defaultStrength, int defaultWillpower, int defaultConstitution, int defaultSpeed, int defaultActionPointsIncome, SkillNative attackingSkill, SkillNative[] skills)
        {
            this.Id = id;
            this.AttackingSkill = attackingSkill;
            this.DefaultActionPointsIncome = defaultActionPointsIncome;
            this.DefaultConstitution = defaultConstitution;
            this.DefaultWillpower = defaultWillpower;
            this.DefaultStrength = defaultStrength;
            this.DefaultSpeed = defaultSpeed;
            this.Skills = skills;
        }

        public RoleModelNative(INativeManager nativeManager, string id, int defaultStrength, int defaultWillpower, int defaultConstitution, int defaultSpeed, int defaultActionPointsIncome, string attackingSkillId, string[] skillIds)
        {
            this.Id = id;
            this.AttackingSkill = nativeManager.GetSkillNative(attackingSkillId);
            this.DefaultActionPointsIncome = defaultActionPointsIncome;
            this.DefaultConstitution = defaultConstitution;
            this.DefaultWillpower = defaultWillpower;
            this.DefaultStrength = defaultStrength;
            this.DefaultSpeed = defaultSpeed;
            SkillNative[] skills = new SkillNative[skillIds.Length];
            for (int i = 0; i < skills.Length; i++)
            {
                skills[i] = nativeManager.GetSkillNative(skillIds[i]);
            }

            this.Skills = skills;
        }
    }
}
