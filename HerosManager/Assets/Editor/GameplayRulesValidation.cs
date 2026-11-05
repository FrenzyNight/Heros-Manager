using System;
using UnityEngine;

/// <summary>Runs deterministic domain regression tests without entering Play mode.</summary>
public static class GameplayRulesValidation
{
    public static void Run()
    {
        using var validation = ValidationSaveSession.Open("GameplayRulesValidation.");
        DataRulesValidation.Run();
        CampRulesValidation.Run();
        MiniGameRulesValidation.Run();
        MeadowRulesValidation.Run();
        CampaignProgressionValidation.Run();
        CaveRewardValidation.Run();
        GamepadInputValidation.Run();
        DialogueRevealValidation.Run();
        ValidateMeadowCampaign();
        ValidateProgression();
        ValidateChoices();
        Debug.Log("GAMEPLAY RULES PASSED");
    }

    private static void ValidateMeadowCampaign()
    {
        Require(MeadowCampaignRules.Evaluate(1, 4, true, false) == MeadowOutcome.Active, "A surviving party can recover from an early defeat.");
        Require(MeadowCampaignRules.Evaluate(4, 4, false, true) == MeadowOutcome.Active, "Day four cannot win before the expedition resolves.");
        Require(MeadowCampaignRules.Evaluate(4, 1, true, true) == MeadowOutcome.Victory, "A successful final expedition completes the meadow.");
        Require(MeadowCampaignRules.Evaluate(4, 4, true, false) == MeadowOutcome.Defeat, "The final objective cannot be bypassed by waiting.");
        Require(MeadowCampaignRules.Evaluate(2, 0, true, true) == MeadowOutcome.Defeat, "Party incapacity overrides success.");
    }

    private static void ValidateProgression()
    {
        var catalog = new GameDataCatalog(
            new[] {
                new StageData { Index = 7, StageID = "Last", StageDayGroupID = "B" },
                new StageData { Index = 2, StageID = "First", StageDayGroupID = "A" }
            },
            new[] {
                new StageDayData { StageDayGroupID = "A", Day = 9 },
                new StageDayData { StageDayGroupID = "B", Day = 4 },
                new StageDayData { StageDayGroupID = "A", Day = 3 }
            }, Array.Empty<FenceData>());
        Require(GameProgression.TryGetNextDay(catalog, 2, 3, out var stage, out var day) &&
                stage.Index == 2 && day.Day == 9, "Sparse days advance by configured order.");
        Require(GameProgression.TryGetNextDay(catalog, 2, 9, out stage, out day) &&
                stage.Index == 7 && day.Day == 4, "Stage changes select the first authored day.");
        Require(!GameProgression.TryGetNextDay(catalog, 7, 4, out _, out _), "Final day has no successor.");
        Require(!GameProgression.TryGetNextDay(catalog, 99, 1, out _, out _), "Unknown stages cannot advance.");
    }

    private static void ValidateChoices()
    {
        var choice = new ChoiceData {
            RewardProb = 0.5f, RewardType = 2,
            Hero1ID = "Hero_1", H1Reward1Type = "hp", H1Reward1Amount = 10,
            Hero2ID = "Hero_2", H2Reward1Type = "stress", H2Reward1Amount = -5,
            FailHero1ID = "Hero_3", FailH1Reward1Type = "power", FailH1Reward1Amount = -7,
            FailHero2ID = "Hero_4", FailH2Reward1Type = "exp", FailH2Reward1Amount = 3,
            SuccessRewardStringID = "Success", FailRewardStringID = "Failure"
        };
        var outcome = EventChoiceRules.Resolve(choice, 50);
        Require(outcome.Success && outcome.StringId == "Success" && outcome.Changes.Count == 2 &&
                outcome.Changes[1].HeroId == "Hero_2" && outcome.Changes[1].Amount == -5, "Success rewards and boundary.");
        outcome = EventChoiceRules.Resolve(choice, 51);
        Require(!outcome.Success && outcome.StringId == "Failure" &&
                outcome.Changes[0].HeroId == "Hero_3" && outcome.Changes[1].Stat == "exp", "Failure uses distinct recipients.");
        choice.RewardProb = 0;
        Require(!EventChoiceRules.Resolve(choice, 1).Success, "Zero probability never succeeds.");
        choice.RewardProb = 1;
        Require(EventChoiceRules.Resolve(choice, 100).Success, "Full probability always succeeds.");
        choice.RewardType = 0;
        Require(EventChoiceRules.Resolve(choice, 1).Changes.Count == 0, "Text-only choices do not grant stats.");
        choice.NeeditemID = "-1";
        choice.NeedAmount = -1;
        Require(InventoryRules.CanAfford(new int[5], new[] { EventChoiceRules.GetCost(choice) }),
            "Legacy absent costs must remain selectable with an empty inventory.");
        bool rejected = false;
        try { EventChoiceRules.Resolve(choice, 0); }
        catch (ArgumentOutOfRangeException) { rejected = true; }
        Require(rejected, "Invalid random rolls must be rejected.");

        bool existed = LoadGameData.IsCreateInstance();
        var tables = LoadGameData.Instance;
        try
        {
            Require(tables.GetString("Stage_t1", SystemLanguage.English) == tables.stringDatas["Stage_t1"].KOR,
                "Missing English sentinel must fall back to Korean, not display -1.");
            foreach (var row in tables.laundryDatas.Values)
                Require(row.L1_JenTimeMin > 0 && row.L1_JenTimeMax >= row.L1_JenTimeMin,
                    "Laundry spawn times must survive JSON deserialization.");
            foreach (var row in tables.choiceDatas.Values)
            {
                EventChoiceRules.Resolve(row, 1);
                EventChoiceRules.Resolve(row, 100);
            }
        }
        finally { if (!existed) UnityEngine.Object.DestroyImmediate(tables.gameObject); }
    }

    private static void Require(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException("Gameplay rules: " + message);
    }
}
