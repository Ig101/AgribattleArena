namespace ProjectArena.Engine.VarManagers
{
    public class VarManager : IVarManager, ForExternalUse.IVarManager
    {
        public int ConstitutionMod { get; }

        public float WillpowerMod { get; }

        public float StrengthMod { get; }

        public float SpeedMod { get; }

        public float MinimumInitiative { get; }

        public VarManager(
            int constitutionMod,
            float willpowerMod,
            float strengthMod,
            float speedMod,
            float minimumInitiative)
        {
            this.ConstitutionMod = constitutionMod;
            this.WillpowerMod = willpowerMod;
            this.StrengthMod = strengthMod;
            this.SpeedMod = speedMod;
            this.MinimumInitiative = minimumInitiative;
        }
    }
}
