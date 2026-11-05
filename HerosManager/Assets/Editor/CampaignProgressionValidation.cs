using System;

/// <summary>Detached chapter transition boundaries; no scene or storage access.</summary>
public static class CampaignProgressionValidation
{
    public static void Run()
    {
        var source = CompletedMeadow();
        Require(CampaignProgressionRules.TryAdvanceToDesert(source, out var next), "A completed meadow must unlock the desert.");
        Require(next.stage == 2 && next.day == 1 && next.meadowOutcome == 0 && next.onboardingComplete,
            "The desert starts at its first active day without replaying onboarding.");
        Require(next.items[0] == 11 && next.items[1] == 34 && next.items[2] == 45 &&
                next.items[3] == 44 && next.items[4] == 61 && next.gems == 10 && next.fenceLevel == 3,
            "Travel rewards must go to the correct inventory slots while preserving purchased improvements.");
        Require(next.hero1Stat[2] == 70 && next.hero2Stat[2] == 100 && next.hero3Stat[2] == 0 &&
                next.hero4Stat[2] == 100 && next.hero1Stat[1] == 135 && next.hero1Stat[3] == 40,
            "Travel heals only survivors, caps health, and does not repeat permanent growth.");
        Require(next.hasEventSnapshot && next.pendingEventIds.Length == 2 &&
                next.pendingEventIds[0] == "Event_Norm4" && next.pendingEventIds[1] == "Event_Acis2",
            "Pending events and their order must survive the chapter boundary.");
        Require(source.stage == 1 && source.day == 4 && source.meadowOutcome == 1 && !source.onboardingComplete &&
                source.items[1] == 22 && source.items[2] == 33 && source.items[4] == 55 &&
                source.gems == 7 && source.hero1Stat[2] == 40,
            "Preparing a transition must not modify the completed source checkpoint.");
        Require(!ReferenceEquals(source, next) && !ReferenceEquals(source.items, next.items) &&
                !ReferenceEquals(source.hero1Stat, next.hero1Stat) && !ReferenceEquals(source.hero2Stat, next.hero2Stat) &&
                !ReferenceEquals(source.hero3Stat, next.hero3Stat) && !ReferenceEquals(source.hero4Stat, next.hero4Stat) &&
                !ReferenceEquals(source.pendingEventIds, next.pendingEventIds),
            "Every mutable transition snapshot array must be detached from its source.");
        next.pendingEventIds[0] = "Changed";
        next.hero4Stat[1] = 999;
        next.items[0] = 999;
        Require(source.pendingEventIds[0] == "Event_Norm4" && source.hero4Stat[1] == 135 && source.items[0] == 11,
            "Editing an uncommitted transition must not alter the previous checkpoint.");
        Reject(next, "A committed desert checkpoint cannot claim the travel reward again.");
        Require(CampaignProgressionRules.TryAdvanceToDesert(source, out var retry) && retry.gems == 10 && retry.items[1] == 34,
            "Retrying the same uncommitted checkpoint must produce the same reward without stacking it.");

        var emptyQueue = CompletedMeadow();
        emptyQueue.pendingEventIds = Array.Empty<string>();
        Require(CampaignProgressionRules.TryAdvanceToDesert(emptyQueue, out next) &&
                next.hasEventSnapshot && next.pendingEventIds.Length == 0,
            "Exhausted event queues must not be reseeded by travel.");
        var legacyQueue = CompletedMeadow();
        legacyQueue.hasEventSnapshot = false;
        legacyQueue.pendingEventIds = null;
        Require(CampaignProgressionRules.TryAdvanceToDesert(legacyQueue, out next) &&
                !next.hasEventSnapshot && next.pendingEventIds == null,
            "Legacy event initialization markers must remain unchanged.");

        var rich = CompletedMeadow();
        rich.items = new[] { int.MaxValue, int.MaxValue - 1, int.MaxValue, int.MaxValue, int.MaxValue - 2 };
        rich.gems = int.MaxValue - 1;
        Require(CampaignProgressionRules.TryAdvanceToDesert(rich, out next) &&
                next.items[1] == int.MaxValue && next.items[2] == int.MaxValue &&
                next.items[4] == int.MaxValue && next.gems == int.MaxValue && SaveGameData.IsValid(next),
            "Travel rewards must saturate instead of overflowing into negative balances.");

        Reject(null, "Missing checkpoints must be rejected.");
        foreach (int stage in new[] { -1, 0, 2, 3, int.MaxValue })
            RejectChanged(data => data.stage = stage, "Only meadow checkpoints may enter the desert.");
        foreach (int day in new[] { -1, 0, 1, 3, 5, int.MaxValue })
            RejectChanged(data => data.day = day, "Only the final completed meadow day may travel.");
        foreach (int outcome in new[] { -1, 0, 2, 3, int.MaxValue })
            RejectChanged(data => data.meadowOutcome = outcome, "Only a victory checkpoint may travel.");
        for (int slot = 0; slot < 5; slot++)
        {
            int index = slot;
            RejectChanged(data => data.items[index] = -1, "Negative inventory cannot be laundered through travel rewards.");
        }
        RejectChanged(data => data.gems = -1, "Negative gems must remain invalid.");
        RejectChanged(data => data.items = new int[4], "Incomplete inventory snapshots must be rejected.");
        RejectChanged(data => data.hero1Stat = null, "Every hero must be captured before travel.");
        RejectChanged(data => data.hero2Stat = Array.Empty<float>(), "Legacy missing hero snapshots cannot enter mid-campaign.");
        RejectChanged(data => data.hero3Stat[2] = float.NaN, "Nonfinite health must be rejected.");
        RejectChanged(data => data.hero4Stat[3] = float.PositiveInfinity, "Nonfinite growth must be rejected.");
        RejectChanged(data =>
        {
            data.hero1Stat[2] = data.hero2Stat[2] = data.hero3Stat[2] = data.hero4Stat[2] = 0;
        }, "A fully incapacitated party cannot travel even if its outcome flag is corrupt.");
        RejectChanged(data => data.pendingEventIds = new[] { "Event_Norm4", "Event_Norm4" },
            "Duplicate queued rewards must remain invalid at the chapter boundary.");
    }

    public static SaveGameData CompletedMeadow() => new SaveGameData
    {
        stage = 1, day = 4, meadowOutcome = 1, fenceLevel = 3, gems = 7,
        items = new[] { 11, 22, 33, 44, 55 },
        hero1Stat = new[] { 5f, 135f, 40f, 40f },
        hero2Stat = new[] { 10f, 135f, 90f, 40f },
        hero3Stat = new[] { 15f, 120f, 0f, 20f },
        hero4Stat = new[] { 0f, 135f, 100f, 40f },
        hasEventSnapshot = true,
        pendingEventIds = new[] { "Event_Norm4", "Event_Acis2" }
    };

    static void RejectChanged(Action<SaveGameData> change, string message)
    {
        var data = CompletedMeadow();
        change(data);
        Reject(data, message);
    }

    static void Reject(SaveGameData source, string message) =>
        Require(!CampaignProgressionRules.TryAdvanceToDesert(source, out var next) && next == null, message);

    static void Require(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException("Campaign progression validation failed: " + message);
    }
}
