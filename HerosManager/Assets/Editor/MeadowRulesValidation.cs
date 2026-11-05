using System;

/// <summary>Deterministic campaign balance boundaries; no scene or player-save access.</summary>
public static class MeadowRulesValidation
{
    public static void Run()
    {
        Require(MeadowRules.GrowthPower(24, MeadowRules.EarnCourage(24, 1)) == 15,
            "Crossing a courage threshold must grant one permanent growth bonus.");
        Require(MeadowRules.GrowthPower(25, MeadowRules.EarnCourage(25, 0)) == 0,
            "Restoring or repeating an unchanged courage value must not grant power.");
        Require(MeadowRules.GrowthPower(20, MeadowRules.EarnCourage(20, 60)) == 45,
            "A multi-level reward must grant every crossed level.");
        Require(MeadowRules.EarnCourage(100, 15) == 115,
            "Courage must continue growing beyond the legacy display cap.");
        Require(MeadowRules.EarnCourage(30, -20) == 30,
            "Courage cannot be drained and re-earned to repeat permanent growth.");
        Require(MeadowRules.EarnCourage(30, float.NaN) == 30,
            "Malformed rewards must not corrupt accumulated courage.");

        var failure = MeadowRules.Reward("JR_S1_Danger_Fail", 10, 3, -10, 0);
        Require(failure.Power == 0 && failure.Courage == 0 && failure.Hp == -40,
            "A failed expedition must damage health without rewarding power or courage.");
        float health = 100;
        for (int i = 0; i < 3; i++) health = MeadowRules.ReturnedHealth(health, failure.Hp, 0, 100);
        Require(health == 0 && !MeadowRules.IsAbleToAct(health),
            "Three unsupported failures must incapacitate a healthy hero.");
        Require(MeadowRules.ReturnedHealth(0, 20, 6, 100) == 0,
            "Meals and healing cannot revive an incapacitated hero.");
        Require(MeadowRules.ReturnedHealth(40, -40, 6, 100) == 0,
            "The return meal cannot undo lethal expedition damage.");
        Require(MeadowRules.ReturnedHealth(50, -5, 3, 100) == 51,
            "One cooked batch should sustain a surviving hero through an ordinary success.");
        Require(MeadowRules.FoodRecovery(int.MaxValue) == 12 && MeadowRules.FoodRecovery(-1) == 0,
            "Food recovery must be bounded without integer overflow.");
        Require(MeadowRules.ReturnedHealth(99, -1, 6, 100) == 100,
            "Food must not exceed maximum health.");

        float courage = 0, power = 100;
        health = 100;
        for (int day = 1; day <= 4; day++)
        {
            var reward = MeadowRules.Reward("JR_S1_Good_Normal", -5, 5, -5, 5);
            float next = MeadowRules.EarnCourage(courage, reward.Courage);
            power += reward.Power + MeadowRules.GrowthPower(courage, next);
            courage = next;
            health = MeadowRules.ReturnedHealth(health, reward.Hp, 0, 100);
        }
        Require(courage == 40 && power == 135 && health == 80 && MeadowRules.Level(courage) == 2,
            "Four ordinary successes must leave a living hero with visible permanent growth.");

        var candidates = new[] { "JR_S1_Good_Perfect", "JR_S1_Good_Normal", "JR_S1_Good_Fail" };
        for (int sample = 0; sample <= 100; sample++)
        {
            Require(WeightedSelection.TryPick(candidates,
                id => MeadowRules.ResultWeight(id, id.EndsWith("_Perfect") ? 0.3f : id.EndsWith("_Normal") ? 0.6f : 0.1f, true),
                sample / 100f, out string selected) && !selected.EndsWith("_Fail"),
                "Fully prepared meadow journeys must succeed for every random sample, including one.");
        }
        Require(MeadowRules.ResultWeight("JR_S1_Danger_Fail", 0.85f, false) == 0.85f,
            "Preparation must not erase risk from under-supplied expeditions.");
    }

    static void Require(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
    }
}
