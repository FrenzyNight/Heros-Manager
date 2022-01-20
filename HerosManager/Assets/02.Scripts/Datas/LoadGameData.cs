using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;

public class LoadGameData : Singleton<LoadGameData>
{
    public List<MiniGameData> miniGameDatas = new List<MiniGameData>();
    public Dictionary<string, JourneyData> journeyDatas = new Dictionary<string, JourneyData>();
    public Dictionary<string, ItemData> itemDatas = new Dictionary<string, ItemData>();
    public Dictionary<string, HerosData> herosDatas = new Dictionary<string, HerosData>();

    public void LoadCSVDatas()
    {
        LoadMiniGameData("CSVData/MiniGameData");
        LoadJourneyData("CSVData/JourneyData");
        LoadItemData("CSVData/ItemData");
        LoadHeroData("CSVData/HerosData");
    }

    void LoadMiniGameData(string _path)
    {
        miniGameDatas.Clear();
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

    void LoadJourneyData(string _path)
    {
        journeyDatas.Clear();
        List<Dictionary<string, object>> data = CSVReader.Read(_path);
        for (int i = 0; i < data.Count; i++)
        {
            JourneyData journeyData = new JourneyData();
            journeyData.code = ConvertToString(data[i]["CodeName"]);
            journeyData.state_good = ConvertToFloat(data[i]["StateGood"]);
            journeyData.state_normal = ConvertToFloat(data[i]["StateNormal"]);
            journeyData.state_bad = ConvertToFloat(data[i]["StateBad"]);
            journeyData.state_danger = ConvertToFloat(data[i]["StateDanger"]);
            journeyData.meatMin = ConvertToInt(data[i]["MeatMin"]);
            journeyData.waterMin = ConvertToInt(data[i]["WaterMin"]);

            journeyDatas.Add(journeyData.code, journeyData);
        }
    }

    void LoadItemData(string _path)
    {
        itemDatas.Clear();
        List<Dictionary<string, object>> data = CSVReader.Read(_path);
        for (int i = 0; i < data.Count; i++)
        {
            ItemData itemData = new ItemData();
            itemData.code = ConvertToString(data[i]["CodeName"]);
            itemData.journeyValue = ConvertToFloat(data[i]["JourneyValue"]);
            itemData.startNum = ConvertToInt(data[i]["FirstGive"]);

            itemDatas.Add(itemData.code, itemData);
        }
    }

    void LoadHeroData(string _path)
    {
        herosDatas.Clear();
        List<Dictionary<string, object>> data = CSVReader.Read(_path);
        for (int i = 0; i < data.Count; i++)
        {
            HerosData herosData = new HerosData();
            herosData.code = ConvertToString(data[i]["CodeName"]);
            herosData.journeyValue = ConvertToFloat(data[i]["JourneyValue"]);
            herosData.startState = ConvertToFloat(data[i]["FirstGive"]);
            herosData.journeyPerfect = ConvertToFloat(data[i]["ResultPerfect"]);
            herosData.journeySuccess = ConvertToFloat(data[i]["ResultSuccess"]);
            herosData.journeyFail = ConvertToFloat(data[i]["ResultFail"]);

            herosDatas.Add(herosData.code, herosData);
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

[System.Serializable] 
public class JourneyData
{
    public string code;
    public float state_good;
    public float state_normal;
    public float state_bad;
    public float state_danger;
    public int meatMin;
    public int waterMin;
}

[System.Serializable]
public class ItemData
{
    public string code;
    public float journeyValue;
    public int startNum;
}

[System.Serializable]
public class HerosData
{
    public string code;
    public float journeyValue;
    public float startState;
    public float journeyPerfect;
    public float journeySuccess;
    public float journeyFail;
}