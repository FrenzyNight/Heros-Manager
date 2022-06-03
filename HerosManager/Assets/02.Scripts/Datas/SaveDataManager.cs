using System.IO;
using UnityEngine;
using System;

public class SaveDataManager : Singleton<SaveDataManager>
{
    public SaveData saveData = new SaveData();

    const string savePath = "/SaveData.Json";

    public bool isSaveStage = false;
    public bool isContinue = false;

    public void LoadDatas(Action _callback = null)
    {
        string path = Application.dataPath + savePath;
        if (!File.Exists(path))
            return;

        string json = File.ReadAllText(path);
        saveData = JsonUtility.FromJson<SaveData>(json);
        if (saveData.stage != 0)
            isSaveStage = true;

        if (_callback != null)
            _callback();
    }

    public void SaveDatas(Action _callback = null)
    {
        string json = JsonUtility.ToJson(saveData);

        string path = Application.dataPath + savePath;
        File.WriteAllText(path, json);

        if (_callback != null)
            _callback();
    }
}

[Serializable]
public class SaveData
{
    public float EffVolume;
    public float BgmVolume;

    public int stage = 0;
    public int day;

    public int[] items;

    public float[] hero1Stat;
    public float[] hero2Stat;
    public float[] hero3Stat;
    public float[] hero4Stat;

    public int fenceLevel;
}