using System;
using System.Collections.Generic;

/// <summary>Resolves choices without scene objects or global random state.</summary>
public static class EventChoiceRules
{
    public static ItemCost GetCost(ChoiceData choice)
    {
        if (choice == null) throw new ArgumentNullException(nameof(choice));
        // Legacy tables encode an absent item and amount with -1.
        return GameDataRules.IsAbsentReference(choice.NeeditemID)
            ? new ItemCost(null, 0)
            : new ItemCost(choice.NeeditemID, choice.NeedAmount);
    }

    public static EventChoiceOutcome Resolve(ChoiceData choice, int roll)
    {
        if (choice == null) throw new ArgumentNullException(nameof(choice));
        if (roll < 1 || roll > 100) throw new ArgumentOutOfRangeException(nameof(roll));
        if (float.IsNaN(choice.RewardProb) || choice.RewardProb < 0 || choice.RewardProb > 1)
            throw new ArgumentException("Reward probability must be between zero and one.");
        if (choice.RewardType < 0 || choice.RewardType > 2)
            throw new ArgumentException("Unsupported choice reward type.");
        bool success = roll <= choice.RewardProb * 100f;
        var changes = new List<HeroStatChange>(4);
        if (choice.RewardType > 0)
        {
            if (success)
            {
                Add(changes, choice.Hero1ID, choice.H1Reward1Type, choice.H1Reward1Amount);
                Add(changes, choice.Hero1ID, choice.H1Reward2Type, choice.H1Reward2Amount);
            }
            else
            {
                Add(changes, choice.FailHero1ID, choice.FailH1Reward1Type, choice.FailH1Reward1Amount);
                Add(changes, choice.FailHero1ID, choice.FailH1Reward2Type, choice.FailH1Reward2Amount);
            }
        }
        if (choice.RewardType == 2)
        {
            if (success)
            {
                Add(changes, choice.Hero2ID, choice.H2Reward1Type, choice.H2Reward1Amount);
                Add(changes, choice.Hero2ID, choice.H2Reward2Type, choice.H2Reward2Amount);
            }
            else
            {
                Add(changes, choice.FailHero2ID, choice.FailH2Reward1Type, choice.FailH2Reward1Amount);
                Add(changes, choice.FailHero2ID, choice.FailH2Reward2Type, choice.FailH2Reward2Amount);
            }
        }
        return new EventChoiceOutcome(success,
            success ? choice.SuccessRewardStringID : choice.FailRewardStringID, changes.AsReadOnly());
    }

    private static void Add(List<HeroStatChange> changes, string heroId, string stat, int amount)
    {
        if (string.IsNullOrEmpty(stat) || stat == "-1" || amount == 0) return;
        if (stat != "stress" && stat != "power" && stat != "hp" && stat != "exp")
            throw new ArgumentException($"Unknown hero stat '{stat}'.");
        if (string.IsNullOrEmpty(heroId) || heroId == "-1")
            throw new ArgumentException("A reward must reference a hero.");
        changes.Add(new HeroStatChange(heroId, stat, amount));
    }
}

public readonly struct HeroStatChange
{
    public string HeroId { get; }
    public string Stat { get; }
    public int Amount { get; }
    public HeroStatChange(string heroId, string stat, int amount)
    {
        HeroId = heroId;
        Stat = stat;
        Amount = amount;
    }
}

public sealed class EventChoiceOutcome
{
    public bool Success { get; }
    public string StringId { get; }
    public IReadOnlyList<HeroStatChange> Changes { get; }
    internal EventChoiceOutcome(bool success, string stringId, IReadOnlyList<HeroStatChange> changes)
    {
        Success = success;
        StringId = stringId;
        Changes = changes;
    }
}
