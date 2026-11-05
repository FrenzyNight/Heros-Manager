public enum MeadowOutcome { Active, Victory, Defeat }

/// <summary>The first playable campaign ends in the meadow, independently of later authored stages.</summary>
public static class MeadowCampaignRules
{
    public const int FinalDay = 4;
    public const int RecoveryGemCost = 3;
    public const int RecoveryHealth = 20;

    public static MeadowOutcome Evaluate(int day, int ableHeroes, bool journeyResolved, bool success)
    {
        if (ableHeroes <= 0) return MeadowOutcome.Defeat;
        if (!journeyResolved || day < FinalDay) return MeadowOutcome.Active;
        return success ? MeadowOutcome.Victory : MeadowOutcome.Defeat;
    }
}
