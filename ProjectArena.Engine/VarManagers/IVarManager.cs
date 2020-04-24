namespace ProjectArena.Engine.VarManagers
{
    public interface IVarManager
    {
        float TurnTimeLimit { get; }

        float TurnTimeLimitAfterSkip { get; }

        int SkippedTurnsLimit { get; }

        int MaxActionPoints { get; }

        int ConstitutionMod { get; }

        float WillpowerMod { get; }

        float StrengthMod { get; }

        float SpeedMod { get; }
    }
}
