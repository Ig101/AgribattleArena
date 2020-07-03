namespace ProjectArena.Engine.VarManagers
{
    public class VarManager : IVarManager, ForExternalUse.IVarManager
    {
        public float TurnTimeLimit { get; }

        public bool CanEndTurnPrematurely { get; }

        public int MaxActionPoints { get; }

        public int ConstitutionMod { get; }

        public float WillpowerMod { get; }

        public float StrengthMod { get; }

        public float SpeedMod { get; }

        public float MinimumInitiative { get; }

        public VarManager(
            float turnTimeLimit,
            bool canEndTurnPrematurely,
            int maxActionPoints,
            int constitutionMod,
            float willpowerMod,
            float strengthMod,
            float speedMod,
            float minimumInitiative)
        {
            this.TurnTimeLimit = turnTimeLimit;
            this.CanEndTurnPrematurely = canEndTurnPrematurely;
            this.MaxActionPoints = maxActionPoints;
            this.ConstitutionMod = constitutionMod;
            this.WillpowerMod = willpowerMod;
            this.StrengthMod = strengthMod;
            this.SpeedMod = speedMod;
            this.MinimumInitiative = minimumInitiative;
        }
    }
}
