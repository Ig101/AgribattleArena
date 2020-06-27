namespace ProjectArena.Engine.VarManagers
{
    public interface IVarManager
    {
        int ConstitutionMod { get; }

        float WillpowerMod { get; }

        float StrengthMod { get; }

        float SpeedMod { get; }

        float MinimumInitiative { get; }
    }
}
