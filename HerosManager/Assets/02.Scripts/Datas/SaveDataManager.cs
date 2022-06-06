using System.IO;
using UnityEngine;
using System;

public class SaveDataManager : Singleton<SaveDataManager>
{
    public SaveGameData saveGameData = new SaveGameData();
    public SaveOptionData saveOptionData = new SaveOptionData();

    const string saveGamePath = "/SaveGameData.Json";
    const string saveOptionPath = "/SaveOptionData.Json";

    public bool LoadGameDatas(Action _callback = null)
    {
        string path = Application.dataPath + saveGamePath;
        if (!File.Exists(path))
            return false;

        string json = File.ReadAllText(path);
        saveGameData = JsonUtility.FromJson<SaveGameData>(json);

        if (_callback != null)
            _callback();

        return true;
    }

    public void SaveGameDatas(Action _callback = null)
    {
        string json = JsonUtility.ToJson(saveGameData);

        string path = Application.dataPath + saveGamePath;
        File.WriteAllText(path, json);

        if (_callback != null)
            _callback();
    }

    public void NewGameData()
    {
        saveGameData.stage = 1;
        saveGameData.day = 1;

        saveGameData.items = new int[5];
        saveGameData.items[0] = LoadGameData.Instance.itemDatas["Item_Wood"].FirstGive;
        saveGameData.items[1] = LoadGameData.Instance.itemDatas["Item_Water"].FirstGive;
        saveGameData.items[2] = LoadGameData.Instance.itemDatas["Item_Meat"].FirstGive;
        saveGameData.items[3] = LoadGameData.Instance.itemDatas["Item_Hub"].FirstGive;
        saveGameData.items[4] = LoadGameData.Instance.itemDatas["Item_Food"].FirstGive;

        saveGameData.fenceLevel = 1;
    }

    public void LoadOptionDatas(Action _callback = null)
    {
        string path = Application.dataPath + saveOptionPath;
        if (!File.Exists(path))
        {
            SaveOptionDatas();

            return;
        }

        string json = File.ReadAllText(path);
        saveOptionData = JsonUtility.FromJson<SaveOptionData>(json);

        if (_callback != null)
            _callback();
    }

    public void SaveOptionDatas(Action _callback = null)
    {
        string json = JsonUtility.ToJson(saveOptionData);

        string path = Application.dataPath + saveOptionPath;
        File.WriteAllText(path, json);

        if (_callback != null)
            _callback();
    }
}

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
}

public class SaveOptionData
{
    public bool isWindow;

    public float EffVolume;
    public float BgmVolume;

    public SaveOptionData()
    {
        isWindow = true;
        EffVolume = 1;
        BgmVolume = 1;
    }
}