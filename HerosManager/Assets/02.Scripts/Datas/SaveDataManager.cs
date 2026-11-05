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
        return TrySaveGameSnapshot(saveGameData, _callback);
    }

    public SaveGameData CaptureGameData() => saveGameData?.Clone() ?? new SaveGameData();

    public bool TrySaveGameSnapshot(SaveGameData snapshot, Action _callback = null)
    {
        // Capture before validation and serialization; neither the caller nor a failed write
        // may partially change the current in-memory snapshot.
        SaveGameData captured = snapshot?.Clone();
        if (!TrySave(GameFileName, captured, IsPlayableSave))
            return false;
        saveGameData = captured;
        _callback?.Invoke();
        return true;
    }

    public void NewGameData()
    {
        saveGameData = CreateNewGameData();
    }

    public SaveGameData CreateNewGameData()
    {
        var tables = LoadGameData.Instance;
        tables.LoadTitleDatas();
        StageData firstStage = tables.Catalog.Stages[0];

        var fresh = new SaveGameData
        {
            stage = firstStage.Index,
            day = tables.Catalog.GetStageDays(firstStage.StageDayGroupID)[0].Day,
            fenceLevel = tables.Catalog.GetFences(firstStage.FenceGroupID)[0].FenceLevel,
            items = new int[ItemIds.Length]
        };
        for (int i = 0; i < ItemIds.Length; i++)
            fresh.items[i] = tables.itemDatas[ItemIds[i]].FirstGive;

        // Return a complete snapshot so a new game never inherits old hero statistics.
        return fresh;
    }

    public static bool IsPlayableSave(SaveGameData data)
    {
        if (!SaveGameData.IsValid(data))
            return false;

        var tables = LoadGameData.Instance;
        tables.LoadTitleDatas();
        return tables.Catalog.TryGetStage(data.stage, out StageData stage) &&
               tables.Catalog.TryGetStageDay(stage.StageDayGroupID, data.day, out _) &&
               tables.Catalog.TryGetFence(stage.FenceGroupID, data.fenceLevel, out _);
    }

    public void LoadOptionDatas(Action _callback = null)
    {
        if (TryLoad(OptionFileName, SaveOptionData.IsValid, out SaveOptionData loaded))
        {
            saveOptionData = loaded;
            saveOptionData.Normalize();
            GamepadInput.RumbleEnabled = !saveOptionData.disableVibration;
            _callback?.Invoke();
            return;
        }
        var defaults = new SaveOptionData();
        if (!TrySave(OptionFileName, defaults, SaveOptionData.IsValid))
            return;
        saveOptionData = defaults;
        GamepadInput.RumbleEnabled = !saveOptionData.disableVibration;
        _callback?.Invoke();
    }

    public void SaveOptionDatas(Action _callback = null)
    {
        TrySaveOptionDatas(_callback);
    }

    public bool TrySaveOptionDatas(Action _callback = null)
    {
        SaveOptionData captured = saveOptionData?.Clone() ?? new SaveOptionData();
        captured.Normalize();
        if (!TrySave(OptionFileName, captured, SaveOptionData.IsValid))
            return false;
        saveOptionData = captured;
        _callback?.Invoke();
        return true;
    }

    private static bool TryLoad<T>(string fileName, Func<T, bool> validate, out T value)
        where T : class, new()
    {
        string path = Path.Combine(SaveStoragePaths.Directory, fileName);
        if (JsonSaveStore.TryLoad(path, validate, out value, out string error))
            return true;
        if (!string.IsNullOrEmpty(error))
            Debug.LogWarning(error);

        // A damaged modern save must not silently restore the demonstration save in Assets.
        if (SaveStoragePaths.IsIsolated || File.Exists(path) || File.Exists(path + ".bak"))
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
        string path = Path.Combine(SaveStoragePaths.Directory, fileName);
        if (JsonSaveStore.TrySave(path, value, validate, out string error))
            return true;
        Debug.LogError(error);
        return false;
    }
}
