namespace ProjectArena.Engine.VarManagers
{
    public interface IVarManager
    {
        float TurnTimeLimit { get; }

        bool CanEndTurnPrematurely { get; }

        int MaxActionPoints { get; }

        int ConstitutionMod { get; }

        float WillpowerMod { get; }

        float StrengthMod { get; }

        float SpeedMod { get; }

        float MinimumInitiative { get; }
    }
}
