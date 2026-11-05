using System;
using System.Collections.Generic;

public sealed class GameDataIssue
{
    public bool IsError { get; }
    public string Message { get; }

    public GameDataIssue(bool isError, string message)
    {
        IsError = isError;
        Message = message;
    }

    public override string ToString() => (IsError ? "Error: " : "Warning: ") + Message;
}

/// <summary>Validates references needed by gameplay separately from unfinished optional content.</summary>
public static class GameDataRules
{
    public static bool IsAbsentReference(string id) => string.IsNullOrWhiteSpace(id) || id == "-1";

    public static IReadOnlyList<GameDataIssue> Validate(LoadGameData tables, GameDataCatalog catalog)
    {
        if (tables == null) throw new ArgumentNullException(nameof(tables));
        if (catalog == null) throw new ArgumentNullException(nameof(catalog));
        var issues = new List<GameDataIssue>();
        if (catalog.Stages.Count == 0)
            Error(issues, "StageTable contains no playable stages.");

        foreach (StageData stage in catalog.Stages)
        {
            RequireReference(issues, stage.StageID, stage.BonfireID, tables.bonfireDatas);
            RequireReference(issues, stage.StageID, stage.CollectID, tables.collectDatas);
            RequireReference(issues, stage.StageID, stage.FieldID, tables.fieldDatas);
            if (catalog.GetStageDays(stage.StageDayGroupID).Count == 0)
                Error(issues, $"{stage.StageID} references an empty stage-day group '{stage.StageDayGroupID}'.");
            if (catalog.GetFences(stage.FenceGroupID).Count == 0)
                Error(issues, $"{stage.StageID} references an empty fence group '{stage.FenceGroupID}'.");
            OptionalReference(issues, stage.StageID, stage.StageStringID, tables.stringDatas);
        }
        foreach (StageDayData day in tables.stageDayDatas.Values)
        {
            RequireReference(issues, day.StageDayID, day.JourneyID, tables.journeyDatas);
            if (!IsAbsentReference(day.InvadeID))
                RequireReference(issues, day.StageDayID, day.InvadeID, tables.invadeDatas);
        }
        foreach (JourneyData journey in tables.journeyDatas.Values)
        {
            if (catalog.GetJourneyStates(journey.JStateGroupID).Count == 0)
                Error(issues, $"{journey.JourneyID} references an empty journey-state group '{journey.JStateGroupID}'.");
            OptionalReference(issues, journey.JourneyID, journey.StageTitleStringID, tables.stringDatas);
        }
        foreach (JStateData state in tables.jStateDatas.Values)
        {
            if (catalog.GetJourneyResults(state.JResultGroupID).Count == 0)
                Error(issues, $"{state.JStateID} references an empty journey-result group '{state.JResultGroupID}'.");
            if (!IsFinite(state.ValueMin))
                Error(issues, $"{state.JStateID} has a nonfinite threshold.");
            OptionalReference(issues, state.JStateID, state.JStateStringID, tables.stringDatas);
        }
        foreach (JResultData result in tables.jResultDatas.Values)
        {
            if (!IsFinite(result.Prob) || result.Prob < 0f)
                Error(issues, $"{result.JResultID} has an invalid probability weight.");
            OptionalReference(issues, result.JResultID, result.JRanStringID, tables.stringDatas);
        }
        foreach (HeroData hero in tables.heroDatas.Values)
        {
            IReadOnlyList<HeroStateData> states = catalog.GetHeroStates(hero.HeroStateGroupID);
            if (states.Count != 4)
                Error(issues, $"{hero.HeroID} must reference four stats: stress, power, hp and exp.");
            else
                for (int i = 0; i < states.Count; i++)
                    if (GameDataCatalog.HeroStatOrder(states[i].HeroStateID) != i)
                        Error(issues, $"{hero.HeroID} contains a missing, duplicate or unknown stat.");
            OptionalReference(issues, hero.HeroID, hero.HeroStringID, tables.stringDatas);
        }
        foreach (HeroStateData state in tables.heroStateDatas.Values)
        {
            if (!IsFinite(state.Min) || !IsFinite(state.Max) || !IsFinite(state.FirstGive) ||
                state.Min > state.Max || state.FirstGive < state.Min || state.FirstGive > state.Max)
                Error(issues, $"{state.HeroStateID} has invalid stat bounds or an initial value outside them.");
            OptionalReference(issues, state.HeroStateID, state.HeroStateStringID, tables.stringDatas);
        }
        foreach (FenceData fence in tables.fenceDatas.Values)
        {
            // The terminal fence level has no further upgrade cost.
            if (!IsAbsentReference(fence.NeedItemID))
                RequireReference(issues, fence.FenceID, fence.NeedItemID, tables.itemDatas);
            if (!IsFinite(fence.InvadeProb) || fence.InvadeProb < 0f || fence.InvadeProb > 1f)
                Error(issues, $"{fence.FenceID} has an invasion probability outside [0, 1].");
        }
        foreach (ItemData item in tables.itemDatas.Values)
        {
            if (item.FirstGive < 0)
                Error(issues, $"{item.ItemID} has a negative initial inventory.");
            OptionalReference(issues, item.ItemID, item.ItemStringID, tables.stringDatas);
        }
        foreach (BonfireData fire in tables.bonfireDatas.Values)
            RequireReference(issues, fire.BonfireID, fire.NeedItemID, tables.itemDatas);
        foreach (FieldData field in tables.fieldDatas.Values)
        {
            RequireReference(issues, field.FieldID, field.NeedItemID, tables.itemDatas);
            RequireReference(issues, field.FieldID, field.GetItemID, tables.itemDatas);
        }
        foreach (CookData recipe in tables.cookDatas.Values)
        {
            RequireReference(issues, recipe.CookID, recipe.GetItemID, tables.itemDatas);
            OptionalReference(issues, recipe.CookID, recipe.NeedItem1ID, tables.itemDatas);
            OptionalReference(issues, recipe.CookID, recipe.NeedItem2ID, tables.itemDatas);
            OptionalReference(issues, recipe.CookID, recipe.NeedItem3ID, tables.itemDatas);
        }
        foreach (CollectData collect in tables.collectDatas.Values)
        {
            OptionalReference(issues, collect.CollectID, collect.CollectSpaceID1, tables.collectSpaceDatas);
            OptionalReference(issues, collect.CollectID, collect.CollectSpaceID2, tables.collectSpaceDatas);
            OptionalReference(issues, collect.CollectID, collect.CollectSpaceID3, tables.collectSpaceDatas);
            OptionalReference(issues, collect.CollectID, collect.CollectSpaceID4, tables.collectSpaceDatas);
        }
        foreach (CollectSpaceData space in tables.collectSpaceDatas.Values)
        {
            CheckMiniGame(issues, catalog, space.CollectSpaceID, space.MiniGameID1);
            CheckMiniGame(issues, catalog, space.CollectSpaceID, space.MiniGameID2);
        }
        foreach (EventData eventData in tables.eventDatas.Values)
        {
            OptionalReference(issues, eventData.EventID, eventData.Choice1ID, tables.choiceDatas);
            OptionalReference(issues, eventData.EventID, eventData.Choice2ID, tables.choiceDatas);
            OptionalReference(issues, eventData.EventID, eventData.Choice3ID, tables.choiceDatas);
            OptionalReference(issues, eventData.EventID, eventData.EventStringID, tables.stringDatas);
        }
        foreach (ChoiceData choice in tables.choiceDatas.Values)
        {
            OptionalReference(issues, choice.ChoiceID, choice.NeeditemID, tables.itemDatas);
            OptionalReference(issues, choice.ChoiceID, choice.Hero1ID, tables.heroDatas);
            OptionalReference(issues, choice.ChoiceID, choice.Hero2ID, tables.heroDatas);
            OptionalReference(issues, choice.ChoiceID, choice.FailHero1ID, tables.heroDatas);
            OptionalReference(issues, choice.ChoiceID, choice.FailHero2ID, tables.heroDatas);
            OptionalReference(issues, choice.ChoiceID, choice.AddEventID, tables.eventDatas);
            OptionalReference(issues, choice.ChoiceID, choice.ChoiceStringID, tables.stringDatas);
            OptionalReference(issues, choice.ChoiceID, choice.SuccessRewardStringID, tables.stringDatas);
            OptionalReference(issues, choice.ChoiceID, choice.FailRewardStringID, tables.stringDatas);
        }
        foreach (CutsceneData scene in tables.cutsceneDatas.Values)
            OptionalReference(issues, scene.CutSceneID, scene.StringID, tables.stringDatas);
        foreach (TutorialData tutorial in tables.tutorialDatas.Values)
            OptionalReference(issues, tutorial.TutorialID, tutorial.StringID, tables.stringDatas);
        return issues.AsReadOnly();
    }

    private static void RequireReference<T>(List<GameDataIssue> issues, string owner, string id, IDictionary<string, T> target)
    {
        if (IsAbsentReference(id) || !target.ContainsKey(id))
            Error(issues, $"{owner} requires missing {typeof(T).Name} '{id}'.");
    }

    private static void OptionalReference<T>(List<GameDataIssue> issues, string owner, string id, IDictionary<string, T> target)
    {
        if (!IsAbsentReference(id) && !target.ContainsKey(id))
            issues.Add(new GameDataIssue(false, $"{owner} references unfinished {typeof(T).Name} '{id}'."));
    }

    private static void CheckMiniGame(List<GameDataIssue> issues, GameDataCatalog catalog, string owner, string id)
    {
        if (!IsAbsentReference(id) && catalog.GetMiniGameObjects(id).Count == 0)
            issues.Add(new GameDataIssue(false, $"{owner} references a minigame with no object table rows: '{id}'."));
    }

    private static bool IsFinite(float value) => !float.IsNaN(value) && !float.IsInfinity(value);
    private static void Error(List<GameDataIssue> issues, string message) => issues.Add(new GameDataIssue(true, message));
}
