namespace AgribattleArena.Engine.VarManagers
{
    public class VarManager : IVarManager, ForExternalUse.IVarManager
    {
        public float TurnTimeLimit { get; }

        public float TurnTimeLimitAfterSkip { get; }

        public int SkippedTurnsLimit { get; }

        public int MaxActionPoints { get; }

        public int ConstitutionMod { get; }

        public float WillpowerMod { get; }

        public float StrengthMod { get; }

        public float SpeedMod { get; }

        public VarManager(
            float turnTimeLimit,
            float turnTimeLimitAfterSkip,
            int skippedTurnsLimit,
            int maxActionPoints,
            int constitutionMod,
            float willpowerMod,
            float strengthMod,
            float speedMod)
        {
            this.TurnTimeLimit = turnTimeLimit;
            this.TurnTimeLimitAfterSkip = turnTimeLimitAfterSkip;
            this.SkippedTurnsLimit = skippedTurnsLimit;
            this.MaxActionPoints = maxActionPoints;
            this.ConstitutionMod = constitutionMod;
            this.WillpowerMod = willpowerMod;
            this.StrengthMod = strengthMod;
            this.SpeedMod = speedMod;
        }
    }
}
