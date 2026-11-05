using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>Deterministic table-index tests and an audit of the currently authored content.</summary>
public static class DataRulesValidation
{
    public static void Run()
    {
        var first = new StageData { StageID = "First", Index = 2, StageDayGroupID = "Days", FenceGroupID = "Fences" };
        var last = new StageData { StageID = "Last", Index = 7 };
        var sourceStages = new List<StageData> { last, first };
        var catalog = new GameDataCatalog(sourceStages,
            new[]
            {
                new StageDayData { StageDayGroupID = "Days", Day = 9 },
                new StageDayData { StageDayGroupID = "Days", Day = 3 },
                new StageDayData { StageDayGroupID = "Other", Day = 3 }
            },
            new[]
            {
                new FenceData { FenceGroupID = "Fences", FenceLevel = 4 },
                new FenceData { FenceGroupID = "Fences", FenceLevel = 1 }
            },
            new[]
            {
                new HeroStateData { HeroStateGroupID = "Hero", HeroStateID = "Hero_Exp" },
                new HeroStateData { HeroStateGroupID = "Hero", HeroStateID = "Hero_Hp" },
                new HeroStateData { HeroStateGroupID = "Hero", HeroStateID = "Hero_Power" },
                new HeroStateData { HeroStateGroupID = "Hero", HeroStateID = "Hero_Stress" }
            },
            new[]
            {
                new CutsceneData { OpenID = "Opening", CutSceneID = "AuthoredFirst" },
                new CutsceneData { OpenID = "Opening", CutSceneID = "AuthoredSecond" }
            },
            new[]
            {
                new TutorialData { OpenID = "Help", TutorialType = 2, Index = 1 },
                new TutorialData { OpenID = "Help", TutorialType = 1, Index = 2 },
                new TutorialData { OpenID = "Help", TutorialType = 1, Index = 1 }
            });

        sourceStages.Clear();
        Require(catalog.Stages.Count == 2 && catalog.Stages[0] == first, "Indexes must not depend on source list order or membership changes.");
        Require(catalog.TryGetNextStage(2, out StageData next) && next == last, "Sparse stage indexes must advance in sorted order.");
        Require(!catalog.TryGetNextStage(7, out _) && !catalog.TryGetNextStage(999, out _), "Terminal and unknown stages have no successor.");
        Require(catalog.TryGetStageDay("Days", 3, out StageDayData day) && day.StageDayGroupID == "Days", "Day keys must include the group.");
        Require(catalog.GetStageDays("Days")[0].Day == 3 && catalog.GetStageDays("Days")[1].Day == 9, "Days must be sorted by authored day, not count.");
        Require(catalog.TryGetFence("Fences", 4, out _) && catalog.GetFences("Fences")[0].FenceLevel == 1, "Fence levels must be indexed and sorted.");
        Require(catalog.GetHeroStates("Hero")[0].HeroStateID == "Hero_Stress" &&
                catalog.GetHeroStates("Hero")[3].HeroStateID == "Hero_Exp", "Stat order must use semantic names, not table row order.");
        Require(catalog.GetTutorials("Help")[0].TutorialType == 1 &&
                catalog.GetTutorials("Help")[1].Index == 2 &&
                catalog.GetTutorials("Help")[2].TutorialType == 2, "Tutorial indexes restart within each section.");
        Require(catalog.GetCutscenes("Opening")[0].CutSceneID == "AuthoredFirst", "Cutscene rows must retain authored order.");
        Require(catalog.GetStageDays(null).Count == 0 && catalog.GetMiniGameObjects("Unknown").Count == 0 &&
                !catalog.TryGetStageDay(null, 3, out _), "Missing groups must yield empty results.");
        Require(((IList<StageData>)catalog.Stages).IsReadOnly, "Catalog collections must not expose mutable membership.");

        ExpectInvalid(() => new GameDataCatalog(new[] { first, first }, Array.Empty<StageDayData>(), Array.Empty<FenceData>()));
        ExpectInvalid(() => new GameDataCatalog(new[] { first },
            new[] { new StageDayData { StageDayGroupID = "A", Day = 1 }, new StageDayData { StageDayGroupID = "A", Day = 1 } },
            Array.Empty<FenceData>()));
        ExpectInvalid(() => new GameDataCatalog(new[] { first }, Array.Empty<StageDayData>(),
            new[] { new FenceData { FenceGroupID = "A", FenceLevel = 1 }, new FenceData { FenceGroupID = "A", FenceLevel = 1 } }));
        ExpectInvalid(() => new GameDataCatalog(new[] { first }, Array.Empty<StageDayData>(), Array.Empty<FenceData>(),
            tutorialRows: new[] { new TutorialData { OpenID = "A", TutorialType = 1, Index = 1 }, new TutorialData { OpenID = "A", TutorialType = 1, Index = 1 } }));
        Require(GameDataRules.IsAbsentReference("-1") && GameDataRules.IsAbsentReference(null) &&
                !GameDataRules.IsAbsentReference("Item_Wood"), "Legacy absence sentinels must not be reported as broken references.");

        bool existed = LoadGameData.IsCreateInstance();
        LoadGameData tables = LoadGameData.Instance;
        try
        {
            tables.LoadTitleDatas();
            int warnings = 0;
            foreach (GameDataIssue issue in tables.ValidationIssues)
            {
                Require(!issue.IsError, issue.Message);
                warnings++;
                Debug.LogWarning(issue.ToString());
            }
            // Broken required references are errors, while missing localization is a content warning.
            var missing = new StageData { StageID = "BrokenFixture", Index = 1, BonfireID = "Missing", StageStringID = "MissingString" };
            var brokenCatalog = new GameDataCatalog(new[] { missing }, Array.Empty<StageDayData>(), Array.Empty<FenceData>());
            bool sawRequiredError = false;
            bool sawOptionalWarning = false;
            foreach (GameDataIssue issue in GameDataRules.Validate(tables, brokenCatalog))
            {
                if (issue.IsError && issue.Message.Contains("BrokenFixture")) sawRequiredError = true;
                if (!issue.IsError && issue.Message.Contains("MissingString")) sawOptionalWarning = true;
            }
            Require(sawRequiredError && sawOptionalWarning, "Validation must distinguish required gameplay links from optional content.");
            Debug.Log($"DataRulesValidation: all catalog and data-rule checks passed; {warnings} content warnings.");
        }
        finally
        {
            if (!existed && tables != null)
                UnityEngine.Object.DestroyImmediate(tables.gameObject);
        }
    }

    private static void ExpectInvalid(Action build)
    {
        try { build(); }
        catch (InvalidOperationException) { return; }
        throw new InvalidOperationException("Data-rule validation failed: duplicate indexed keys were accepted.");
    }

    private static void Require(bool condition, string message)
    {
        if (!condition)
            throw new InvalidOperationException("Data-rule validation failed: " + message);
    }
}
