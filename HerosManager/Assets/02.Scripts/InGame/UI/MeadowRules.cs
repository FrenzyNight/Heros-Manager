using System;

/// <summary>Deterministic growth and expedition rules shared by playable campaign stages.</summary>
public static class MeadowRules
{
    public const int CouragePerLevel = 25;
    public const int PowerPerLevel = 15;
    public const int FailureDamage = 40;
    public const int MaximumFoodRecovery = 12;

    public static bool IsAbleToAct(float hp) => IsFinite(hp) && hp > 0;
    public static int Level(float courage) => 1 + (int)(NormalizeCourage(courage) / CouragePerLevel);

    // Courage records lifetime earned progress. Spending or losing it would permit
    // repeatedly crossing the same level boundary and minting permanent power.
    public static float EarnCourage(float current, float earned)
    {
        current = NormalizeCourage(current);
        return IsFinite(earned) && earned > 0 ? Math.Min(1000000f, current + earned) : current;
    }

    public static int GrowthPower(float before, float after) =>
        Math.Max(0, Level(after) - Level(before)) * PowerPerLevel;

    public static float NormalizeCourage(float value) => IsFinite(value) ? Math.Max(0, Math.Min(1000000f, value)) : 0;
    public static int FoodRecovery(int portions) => (int)Math.Min(MaximumFoodRecovery, Math.Max(0, portions) * 2L);

    public static float ResultWeight(string resultId, float weight, bool preparedJourney) =>
        preparedJourney && resultId != null && resultId.EndsWith("_Fail", StringComparison.Ordinal) ? 0 : weight;

    public static float ReturnedHealth(float hp, float damageDelta, int foodPortions, float maximumHp)
    {
        if (!IsAbleToAct(hp)) return 0;
        float remaining = Math.Max(0, hp + damageDelta);
        // Meals sustain survivors; they cannot revive a casualty from this expedition.
        return remaining > 0 ? Math.Min(maximumHp, remaining + FoodRecovery(foodPortions)) : 0;
    }

    public static MeadowHeroReward Reward(string resultId, float stress, float power, float hp, float courage)
    {
        bool failure = resultId != null && resultId.EndsWith("_Fail", StringComparison.Ordinal);
        bool perfect = resultId != null && resultId.EndsWith("_Perfect", StringComparison.Ordinal);
        return failure
            ? new MeadowHeroReward(stress, Math.Min(0, power), Math.Min(-FailureDamage, hp), 0)
            : new MeadowHeroReward(stress, power, hp, Math.Max(perfect ? 15 : 10, courage));
    }

    static bool IsFinite(float value) => !float.IsNaN(value) && !float.IsInfinity(value);
}

public readonly struct MeadowHeroReward
{
    public readonly float Stress, Power, Hp, Courage;
    public MeadowHeroReward(float stress, float power, float hp, float courage)
    { Stress = stress; Power = power; Hp = hp; Courage = courage; }
}

public readonly struct MeadowJourneyResult
{
    public string ResultId { get; }
    public bool IsSuccess => !string.IsNullOrEmpty(ResultId) && !ResultId.EndsWith("_Fail", StringComparison.Ordinal);
    public bool IsPerfect => ResultId != null && ResultId.EndsWith("_Perfect", StringComparison.Ordinal);
    public int FoodRecoveryPerHero { get; }
    public int ActiveHeroesAtDeparture { get; }
    public MeadowJourneyResult(string resultId, int recovery, int activeHeroes)
    { ResultId = resultId; FoodRecoveryPerHero = recovery; ActiveHeroesAtDeparture = activeHeroes; }
}
