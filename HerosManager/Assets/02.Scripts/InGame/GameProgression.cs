using System;

/// <summary>Progression follows configured indices and days, not dictionary order or row counts.</summary>
public static class GameProgression
{
    public static bool TryGetNextDay(GameDataCatalog catalog, int stageIndex, int day,
        out StageData nextStage, out StageDayData nextDay)
    {
        if (catalog == null) throw new ArgumentNullException(nameof(catalog));
        nextStage = null;
        nextDay = null;
        if (!catalog.TryGetStage(stageIndex, out var current)) return false;
        foreach (var candidate in catalog.GetStageDays(current.StageDayGroupID))
            if (candidate.Day > day)
            {
                nextStage = current;
                nextDay = candidate;
                return true;
            }
        if (!catalog.TryGetNextStage(stageIndex, out nextStage)) return false;
        var days = catalog.GetStageDays(nextStage.StageDayGroupID);
        if (days.Count == 0) return false;
        nextDay = days[0];
        return true;
    }
}
