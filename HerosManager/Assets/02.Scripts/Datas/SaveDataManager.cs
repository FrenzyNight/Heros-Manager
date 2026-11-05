using System.IO;
using UnityEngine;
using System;

public class SaveDataManager : Singleton<SaveDataManager>
{
    public SaveGameData saveGameData = new SaveGameData();
    public SaveOptionData saveOptionData = new SaveOptionData();

    private const string GameFileName = "SaveGameData.Json";
    private const string OptionFileName = "SaveOptionData.Json";
    private static readonly string[] ItemIds =
    {
        "Item_Wood", "Item_Water", "Item_Meat", "Item_Hub", "Item_Food"
    };

    public override void Init()
    {
        if (Application.isPlaying)
        {
            transform.SetParent(null);
            DontDestroyOnLoad(gameObject);
        }
    }

    public bool LoadGameDatas(Action _callback = null)
    {
        if (!TryLoad(GameFileName, IsPlayableSave, out SaveGameData loaded))
            return false;
        saveGameData = loaded;
        _callback?.Invoke();
        return true;
    }

    public void SaveGameDatas(Action _callback = null)
    {
        TrySaveGameDatas(_callback);
    }

    public bool TrySaveGameDatas(Action _callback = null)
    {
        if (!TrySave(GameFileName, saveGameData, IsPlayableSave))
            return false;
        _callback?.Invoke();
        return true;
    }

    public void NewGameData()
    {
        var tables = LoadGameData.Instance;
        if (tables.itemDatas.Count == 0)
            tables.LoadTitleDatas();

        var fresh = new SaveGameData
        {
            stage = 1,
            day = 1,
            fenceLevel = 1,
            items = new int[ItemIds.Length]
        };
        for (int i = 0; i < ItemIds.Length; i++)
            fresh.items[i] = tables.itemDatas[ItemIds[i]].FirstGive;

        // Replace the entire snapshot so a new game never inherits old hero statistics.
        saveGameData = fresh;
    }

    public static bool IsPlayableSave(SaveGameData data)
    {
        if (!SaveGameData.IsValid(data))
            return false;

        var tables = LoadGameData.Instance;
        tables.LoadTitleDatas();
        StageData stage = null;
        foreach (var candidate in tables.stageDatas.Values)
            if (candidate.Index == data.stage)
            {
                stage = candidate;
                break;
            }
        if (stage == null)
            return false;

        bool hasDay = false;
        foreach (var candidate in tables.stageDayDatas.Values)
            if (candidate.StageDayGroupID == stage.StageDayGroupID && candidate.Day == data.day)
            {
                hasDay = true;
                break;
            }
        if (!hasDay)
            return false;

        foreach (var candidate in tables.fenceDatas.Values)
            if (candidate.FenceGroupID == stage.FenceGroupID && candidate.FenceLevel == data.fenceLevel)
                return true;
        return false;
    }

    public void LoadOptionDatas(Action _callback = null)
    {
        if (TryLoad(OptionFileName, SaveOptionData.IsValid, out SaveOptionData loaded))
        {
            saveOptionData = loaded;
            saveOptionData.Normalize();
            _callback?.Invoke();
            return;
        }
        saveOptionData = new SaveOptionData();
        TrySaveOptionDatas(_callback);
    }

    public void SaveOptionDatas(Action _callback = null)
    {
        TrySaveOptionDatas(_callback);
    }

    public bool TrySaveOptionDatas(Action _callback = null)
    {
        if (saveOptionData == null)
            saveOptionData = new SaveOptionData();
        saveOptionData.Normalize();
        if (!TrySave(OptionFileName, saveOptionData, SaveOptionData.IsValid))
            return false;
        _callback?.Invoke();
        return true;
    }

    private static bool TryLoad<T>(string fileName, Func<T, bool> validate, out T value)
        where T : class, new()
    {
        string path = Path.Combine(Application.persistentDataPath, fileName);
        if (JsonSaveStore.TryLoad(path, validate, out value, out string error))
            return true;
        if (!string.IsNullOrEmpty(error))
            Debug.LogWarning(error);

        // A damaged modern save must not silently restore the demonstration save in Assets.
        if (File.Exists(path) || File.Exists(path + ".bak"))
            return false;

        string legacyPath = Path.Combine(Application.dataPath, fileName);
        if (!JsonSaveStore.TryLoad(legacyPath, validate, out value, out error))
        {
            if (!string.IsNullOrEmpty(error))
                Debug.LogWarning(error);
            return false;
        }

        // Leave the old file intact, even when migration cannot be written.
        TrySave(fileName, value, validate);
        return true;
    }

    private static bool TrySave<T>(string fileName, T value, Func<T, bool> validate)
        where T : class, new()
    {
        string path = Path.Combine(Application.persistentDataPath, fileName);
        if (JsonSaveStore.TrySave(path, value, validate, out string error))
            return true;
        Debug.LogError(error);
        return false;
    }
}

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

    public static bool IsValid(SaveGameData data)
    {
        if (data == null || data.stage < 1 || data.day < 1 || data.fenceLevel < 1 ||
            data.items == null || data.items.Length != 5)
            return false;

        foreach (int count in data.items)
            if (count < 0)
                return false;

        return ValidStats(data.hero1Stat) && ValidStats(data.hero2Stat) &&
               ValidStats(data.hero3Stat) && ValidStats(data.hero4Stat);
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
    public bool isWindow = true;
    public float EffVolume = 1f;
    public float BgmVolume = 1f;

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
