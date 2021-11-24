using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;

public class LoadGameData : Singleton<LoadGameData>
{
    public Dictionary<string, MiniGameData> miniGameDic = new Dictionary<string, MiniGameData>();

    void Start()
    {
        LoadMiniGameData("CSVData/MiniGameData");
    }

    void LoadMiniGameData(string _path)
    {
        List<Dictionary<string, object>> data = CSVReader.Read(_path);
        for (int i = 0; i < data.Count; i++)
        {
            MiniGameData miniGameData = new MiniGameData();
            miniGameData.code = ConvertToString(data[i][""]);
            miniGameData.stageCode = ConvertToString(data[i][""]);
            miniGameData.value1 = ConvertToInt(data[i][""]);
            miniGameData.value2 = ConvertToInt(data[i][""]);

            miniGameDic.Add(miniGameData.stageCode, miniGameData);
        }
    }

    int ConvertToInt(object obj)
    {
        string _body = obj.ToString();
        _body = Regex.Replace(_body, @"[^0-9]", "");
        return int.Parse(_body);
    }

    string ConvertToString(object obj)
    {
        return obj.ToString();
    }

    float ConvertToFloat(object obj)
    {
        string _body = obj.ToString();
        //_body = Regex.Replace(_body, @"[^0-9]", "");
        return float.Parse(_body);
    }
}

public class MiniGameData
{
    public string code;
    public string stageCode;
    public int value1;
    public int value2;
}