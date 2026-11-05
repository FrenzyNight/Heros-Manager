using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SaveGameData
{
    public int stage;
    public int day;
    public int[] items;
    public float[] hero1Stat;
    public float[] hero2Stat;
    public float[] hero3Stat;
    public float[] hero4Stat;
    public int fenceLevel;
    public int gems;
    // Zero is the legacy/default active run; terminal runs reopen their result screen.
    public int meadowOutcome;
    public bool onboardingComplete;
    // JsonUtility may serialize a null array as []; this marker distinguishes legacy
    // saves with an uninitialized queue from a deliberately exhausted queue.
    public bool hasEventSnapshot;
    public string[] pendingEventIds;

    public SaveGameData Clone()
    {
        return new SaveGameData
        {
            stage = stage, day = day, fenceLevel = fenceLevel,
            gems = gems, meadowOutcome = meadowOutcome, onboardingComplete = onboardingComplete,
            items = items == null ? null : (int[])items.Clone(),
            hasEventSnapshot = hasEventSnapshot,
            pendingEventIds = pendingEventIds == null ? null : (string[])pendingEventIds.Clone(),
            hero1Stat = CopyStats(hero1Stat), hero2Stat = CopyStats(hero2Stat),
            hero3Stat = CopyStats(hero3Stat), hero4Stat = CopyStats(hero4Stat)
        };
    }

    private static float[] CopyStats(float[] values) => values == null ? null : (float[])values.Clone();

    public static bool IsValid(SaveGameData data)
    {
        if (data == null || data.stage < 1 || data.day < 1 || data.fenceLevel < 1 ||
            data.items == null || data.items.Length != 5 || data.gems < 0 ||
            data.meadowOutcome < 0 || data.meadowOutcome > 2 ||
            (data.meadowOutcome != 0 && data.stage != 1))
            return false;

        foreach (int count in data.items)
            if (count < 0)
                return false;

        return ValidEvents(data) && ValidStats(data.hero1Stat) && ValidStats(data.hero2Stat) &&
               ValidStats(data.hero3Stat) && ValidStats(data.hero4Stat);
    }

    private static bool ValidEvents(SaveGameData data)
    {
        if (!data.hasEventSnapshot) return true;
        if (data.pendingEventIds == null) return false;
        var unique = new HashSet<string>(StringComparer.Ordinal);
        foreach (string id in data.pendingEventIds)
            if (string.IsNullOrWhiteSpace(id) || id == "-1" || !unique.Add(id))
                return false;
        return true;
    }

    private static bool ValidStats(float[] stats)
    {
        // Old saves have no hero snapshots; consumers use table defaults in that case.
        if (stats == null || stats.Length == 0)
            return true;
        if (stats.Length != 4)
            return false;
        foreach (float stat in stats)
            if (float.IsNaN(stat) || float.IsInfinity(stat))
                return false;
        return true;
    }
}

[Serializable]
public class SaveOptionData
{
    public bool disableVibration;
    public bool isWindow = true;
    public float EffVolume = 1f;
    public float BgmVolume = 1f;

    public SaveOptionData Clone()
    {
        return new SaveOptionData { isWindow = isWindow, EffVolume = EffVolume, BgmVolume = BgmVolume, disableVibration = disableVibration };
    }

    public void Normalize()
    {
        EffVolume = NormalizeVolume(EffVolume);
        BgmVolume = NormalizeVolume(BgmVolume);
    }

    public static bool IsValid(SaveOptionData data)
    {
        return data != null && !float.IsNaN(data.EffVolume) && !float.IsInfinity(data.EffVolume) &&
               !float.IsNaN(data.BgmVolume) && !float.IsInfinity(data.BgmVolume);
    }

    private static float NormalizeVolume(float volume)
    {
        return float.IsNaN(volume) || float.IsInfinity(volume) ? 1f : Mathf.Clamp01(volume);
    }
}
