using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

/// <summary>
/// Builds deterministic lookup indexes once after loading the tables.
/// Rows retain their legacy DTO types; callers must treat table rows as read-only.
/// </summary>
public sealed class GameDataCatalog
{
    private readonly Dictionary<int, StageData> stagesByIndex = new Dictionary<int, StageData>();
    private readonly Dictionary<int, StageData> nextStages = new Dictionary<int, StageData>();
    private readonly Dictionary<(string, int), StageDayData> daysByGroupAndDay = new Dictionary<(string, int), StageDayData>();
    private readonly Dictionary<(string, int), FenceData> fencesByGroupAndLevel = new Dictionary<(string, int), FenceData>();
    private readonly GroupIndex<StageDayData> days;
    private readonly GroupIndex<FenceData> fences;
    private readonly GroupIndex<HeroStateData> heroStates;
    private readonly GroupIndex<CutsceneData> cutscenes;
    private readonly GroupIndex<TutorialData> tutorials;
    private readonly GroupIndex<MiniGameData> miniGameObjects;
    private readonly GroupIndex<JStateData> journeyStates;
    private readonly GroupIndex<JResultData> journeyResults;

    public IReadOnlyList<StageData> Stages { get; }

    public GameDataCatalog(
        IEnumerable<StageData> stages,
        IEnumerable<StageDayData> stageDays,
        IEnumerable<FenceData> stageFences,
        IEnumerable<HeroStateData> heroStateRows = null,
        IEnumerable<CutsceneData> cutsceneRows = null,
        IEnumerable<TutorialData> tutorialRows = null,
        IEnumerable<MiniGameData> miniGameRows = null,
        IEnumerable<JStateData> journeyStateRows = null,
        IEnumerable<JResultData> journeyResultRows = null)
    {
        if (stages == null) throw new ArgumentNullException(nameof(stages));
        if (stageDays == null) throw new ArgumentNullException(nameof(stageDays));
        if (stageFences == null) throw new ArgumentNullException(nameof(stageFences));
        var orderedStages = new List<StageData>();
        foreach (StageData row in stages)
        {
            if (row == null || row.Index < 1 || !stagesByIndex.TryAdd(row.Index, row))
                throw new InvalidOperationException("Stage indexes must be unique positive integers.");
            orderedStages.Add(row);
        }
        orderedStages.Sort((a, b) => a.Index.CompareTo(b.Index));
        for (int i = 0; i + 1 < orderedStages.Count; i++)
            nextStages.Add(orderedStages[i].Index, orderedStages[i + 1]);
        Stages = orderedStages.AsReadOnly();

        var dayRows = new List<StageDayData>();
        foreach (StageDayData row in stageDays)
        {
            if (row == null || string.IsNullOrWhiteSpace(row.StageDayGroupID) || row.Day < 1 ||
                !daysByGroupAndDay.TryAdd((row.StageDayGroupID, row.Day), row))
                throw new InvalidOperationException("Stage days must have a group and a unique positive day within that group.");
            dayRows.Add(row);
        }
        days = new GroupIndex<StageDayData>(dayRows, row => row.StageDayGroupID, (a, b) => a.Day.CompareTo(b.Day));

        var fenceRows = new List<FenceData>();
        foreach (FenceData row in stageFences)
        {
            if (row == null || string.IsNullOrWhiteSpace(row.FenceGroupID) || row.FenceLevel < 1 ||
                !fencesByGroupAndLevel.TryAdd((row.FenceGroupID, row.FenceLevel), row))
                throw new InvalidOperationException("Fences must have a group and a unique positive level within that group.");
            fenceRows.Add(row);
        }
        fences = new GroupIndex<FenceData>(fenceRows, row => row.FenceGroupID, (a, b) => a.FenceLevel.CompareTo(b.FenceLevel));
        heroStates = new GroupIndex<HeroStateData>(heroStateRows, row => row.HeroStateGroupID,
            (a, b) => HeroStatOrder(a.HeroStateID).CompareTo(HeroStatOrder(b.HeroStateID)));
        // Cutscene ordering is authored in the source table; unlike tutorials it has no sequence field.
        cutscenes = new GroupIndex<CutsceneData>(cutsceneRows, row => row.OpenID);
        var orderedTutorials = new List<TutorialData>();
        var tutorialKeys = new HashSet<(string, int, int)>();
        foreach (TutorialData row in tutorialRows ?? Array.Empty<TutorialData>())
        {
            if (row == null || row.Index < 1 || !tutorialKeys.Add((row.OpenID, row.TutorialType, row.Index)))
                throw new InvalidOperationException("Tutorial sequence indexes must be unique within an open ID and section.");
            orderedTutorials.Add(row);
        }
        tutorials = new GroupIndex<TutorialData>(orderedTutorials, row => row.OpenID, (a, b) =>
        {
            int section = a.TutorialType.CompareTo(b.TutorialType);
            return section != 0 ? section : a.Index.CompareTo(b.Index);
        });
        miniGameObjects = new GroupIndex<MiniGameData>(miniGameRows, row => row.MinigameID);
        journeyStates = new GroupIndex<JStateData>(journeyStateRows, row => row.JStateGroupID,
            (a, b) => b.ValueMin.CompareTo(a.ValueMin));
        journeyResults = new GroupIndex<JResultData>(journeyResultRows, row => row.JResultGroupID);
    }

    public bool TryGetStage(int index, out StageData stage) => stagesByIndex.TryGetValue(index, out stage);
    public bool TryGetNextStage(int currentIndex, out StageData stage) => nextStages.TryGetValue(currentIndex, out stage);
    public bool TryGetStageDay(string groupId, int day, out StageDayData value) =>
        daysByGroupAndDay.TryGetValue((groupId, day), out value);
    public bool TryGetFence(string groupId, int level, out FenceData value) =>
        fencesByGroupAndLevel.TryGetValue((groupId, level), out value);
    public IReadOnlyList<StageDayData> GetStageDays(string groupId) => days.Get(groupId);
    public IReadOnlyList<FenceData> GetFences(string groupId) => fences.Get(groupId);
    public IReadOnlyList<HeroStateData> GetHeroStates(string groupId) => heroStates.Get(groupId);
    public IReadOnlyList<CutsceneData> GetCutscenes(string openId) => cutscenes.Get(openId);
    public IReadOnlyList<TutorialData> GetTutorials(string openId) => tutorials.Get(openId);
    public IReadOnlyList<MiniGameData> GetMiniGameObjects(string miniGameId) => miniGameObjects.Get(miniGameId);
    public IReadOnlyList<JStateData> GetJourneyStates(string groupId) => journeyStates.Get(groupId);
    public IReadOnlyList<JResultData> GetJourneyResults(string groupId) => journeyResults.Get(groupId);

    internal static int HeroStatOrder(string id)
    {
        if (id == null) return int.MaxValue;
        if (id.EndsWith("_Stress", StringComparison.Ordinal)) return 0;
        if (id.EndsWith("_Power", StringComparison.Ordinal)) return 1;
        if (id.EndsWith("_Hp", StringComparison.Ordinal)) return 2;
        if (id.EndsWith("_Exp", StringComparison.Ordinal)) return 3;
        return int.MaxValue;
    }

    private sealed class GroupIndex<T> where T : class
    {
        private static readonly IReadOnlyList<T> Empty = Array.AsReadOnly(Array.Empty<T>());
        private readonly Dictionary<string, ReadOnlyCollection<T>> groups = new Dictionary<string, ReadOnlyCollection<T>>(StringComparer.Ordinal);

        public GroupIndex(IEnumerable<T> rows, Func<T, string> getGroup, Comparison<T> order = null)
        {
            var building = new Dictionary<string, List<T>>(StringComparer.Ordinal);
            foreach (T row in rows ?? Array.Empty<T>())
            {
                if (row == null)
                    throw new InvalidOperationException($"A {typeof(T).Name} table contains a null row.");
                string group = getGroup(row);
                if (string.IsNullOrWhiteSpace(group))
                    throw new InvalidOperationException($"A {typeof(T).Name} row has no group identifier.");
                if (!building.TryGetValue(group, out List<T> list))
                    building.Add(group, list = new List<T>());
                list.Add(row);
            }
            foreach (var pair in building)
            {
                if (order != null)
                    pair.Value.Sort(order);
                groups.Add(pair.Key, pair.Value.AsReadOnly());
            }
        }

        public IReadOnlyList<T> Get(string group)
        {
            return group != null && groups.TryGetValue(group, out ReadOnlyCollection<T> rows) ? rows : Empty;
        }
    }
}
