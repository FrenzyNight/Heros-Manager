using System;

/// <summary>Playable chapter boundaries and one-time intermission supplies.</summary>
public static class CampaignProgressionRules
{
    public const int DesertStage = 2;
    public const int TravelSupplies = 12;
    public const int TravelFood = 6;
    public const int TravelGems = 3;
    public const int TravelRecovery = 30;

    public static bool IsPlayableStage(int stage) => stage == 1 || stage == DesertStage;

    public static bool TryAdvanceToDesert(SaveGameData source, out SaveGameData next)
    {
        next = null;
        if (!SaveGameData.IsValid(source) || source.stage != 1 ||
            source.day != MeadowCampaignRules.FinalDay || source.meadowOutcome != (int)MeadowOutcome.Victory)
            return false;
        var heroes = new[] { source.hero1Stat, source.hero2Stat, source.hero3Stat, source.hero4Stat };
        bool survivor = false;
        foreach (var hero in heroes)
        {
            // Terminal checkpoints must capture every hero, including incapacitated ones.
            if (hero == null || hero.Length != 4) return false;
            survivor |= hero[2] > 0;
        }
        if (!survivor) return false;
        var candidate = source.Clone();
        candidate.stage = DesertStage;
        candidate.day = 1;
        candidate.meadowOutcome = (int)MeadowOutcome.Active;
        candidate.onboardingComplete = true;
        candidate.items[1] = AddSaturated(candidate.items[1], TravelSupplies);
        candidate.items[2] = AddSaturated(candidate.items[2], TravelSupplies);
        candidate.items[4] = AddSaturated(candidate.items[4], TravelFood);
        candidate.gems = AddSaturated(candidate.gems, TravelGems);
        foreach (var hero in new[] { candidate.hero1Stat, candidate.hero2Stat, candidate.hero3Stat, candidate.hero4Stat })
            if (hero[2] > 0) hero[2] = Math.Min(100f, hero[2] + TravelRecovery);
        // Preserve exhausted event queues and purchased fence levels across chapters.
        next = candidate;
        return true;
    }

    static int AddSaturated(int value, int amount) => (int)Math.Min(int.MaxValue, (long)value + amount);
}
