using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;

public class LoadGameData : Singleton<LoadGameData>
{
    public List<MiniGameData> miniGameDatas = new List<MiniGameData>();

    public void LoadCSVDatas()
    {
        LoadMiniGameData("CSVData/MiniGameData");
    }

    void LoadMiniGameData(string _path)
    {
        List<Dictionary<string, object>> data = CSVReader.Read(_path);
        for (int i = 0; i < data.Count; i++)
        {
            MiniGameData miniGameData = new MiniGameData();
            miniGameData.code = ConvertToString(data[i]["ObjectCode"]);
            miniGameData.stageCode = ConvertToString(data[i]["StageCode"]);
            miniGameData.type = ConvertToInt(data[i]["Object_type"]);
            miniGameData.wood = ConvertToInt(data[i]["Wood"]);
            miniGameData.water = ConvertToInt(data[i]["Water"]);
            miniGameData.meat = ConvertToInt(data[i]["Meat"]);
            miniGameData.hub = ConvertToInt(data[i]["Hub"]);
            miniGameData.jem = ConvertToInt(data[i]["Jem"]);
            miniGameData.probability = ConvertToFloat(data[i]["Probability"]);
            miniGameData.stun = ConvertToFloat(data[i]["Stun"]);
            miniGameData.invincibility = ConvertToFloat(data[i]["Invincibility"]);
            miniGameData.speed = ConvertToFloat(data[i]["Velocity"]);
            miniGameData.cooltime = ConvertToFloat(data[i]["CoolTime"]);
            miniGameData.environment = ConvertToInt(data[i]["GameEnvironment"]);
            miniGameData.value1 = ConvertToFloat(data[i]["value1"]);
            miniGameData.value2 = ConvertToFloat(data[i]["value2"]);
            miniGameData.value3 = ConvertToFloat(data[i]["value3"]);
            miniGameData.value4 = ConvertToFloat(data[i]["value4"]);
            miniGameData.value5 = ConvertToFloat(data[i]["value5"]);
            miniGameData.value6 = ConvertToFloat(data[i]["value6"]);

            miniGameDatas.Add(miniGameData);
        }
    }

    int ConvertToInt(object obj)
    {
        string _body = obj.ToString();
        //_body = Regex.Replace(_body, @"[^0-9]", "");
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

[System.Serializable]
public class MiniGameData
{
    public string code;
    public string stageCode;
    public int type;
    public int wood;
    public int water;
    public int meat;
    public int hub;
    public int jem;
    public float probability;   //확률
    public float stun;          //스턴
    public float invincibility; //무적
    public float speed;
    public float cooltime;
    public int environment;
    public float value1;
    public float value2;
    public float value3;
    public float value4;
    public float value5;
    public float value6;
}