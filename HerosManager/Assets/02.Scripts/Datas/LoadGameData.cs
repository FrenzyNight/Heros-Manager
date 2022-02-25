using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;

public class LoadGameData : Singleton<LoadGameData>
{
    //public Dictionary<string, HeroStateData> heroStateDatas = new Dictionary<string, HeroStateData>();
    //public Dictionary<string, ItemData> itemDatas = new Dictionary<string, ItemData>();
    //public Dictionary<string, JourneyData> journeyDatas = new Dictionary<string, JourneyData>();
    //public Dictionary<string, JRwithHeroData> jRwithHeroDatas = new Dictionary<string, JRwithHeroData>();
    //public Dictionary<string, JStateData> jStateDatas = new Dictionary<string, JStateData>();
    //public Dictionary<string, JVCutData> jVCutDatas = new Dictionary<string, JVCutData>();
    //public Dictionary<string, JVwithObjectData> jVwithObjectDatas = new Dictionary<string, JVwithObjectData>();
    //public Dictionary<string, StageDayData> stageDayDatas = new Dictionary<string, StageDayData>();
    //public Dictionary<string, StageData> stageDatas = new Dictionary<string, StageData>();
    //public Dictionary<string, StringData> stringDatas = new Dictionary<string, StringData>();


    #region 예전
    public List<MiniGameData> miniGameDatas = new List<MiniGameData>();
    public Dictionary<string, JourneyData> journeyDatas = new Dictionary<string, JourneyData>();
    public Dictionary<string, ItemData> itemDatas = new Dictionary<string, ItemData>();
    public Dictionary<string, HerosData> herosDatas = new Dictionary<string, HerosData>();

    public void LoadCSVDatas()
    {
        //LoadHeroStateData("CSVData/HeroStateTable");
        //LoadItemData("CSVData/ItemTable");
        //LoadJourneyData("CSVData/JourneyTable");
        //LoadJRwithHeroData("CSVData/JRwithHeroTable");
        //LoadJStateData("CSVData/JStateTable");
        //LoadJVCutData("CSVData/JVCutTable");
        //LoadJVwithObjectData("CSVData/JVwithObjectTable");
        //LoadStageDayData("CSVData/StageDayTable");
        //LoadStageData("CSVData/StageTable");
        //LoadStringData("CSVData/StringTable");


        LoadMiniGameData("CSVData/MiniGameData");
        LoadJourneyData("CSVData/JourneyData");
        LoadItemData("CSVData/ItemData");
        LoadHeroData("CSVData/HerosData");
    }

    //void LoadHeroStateData(string _path)
    //{
    //    heroStateDatas.Clear();
    //    List<Dictionary<string, object>> data = CSVReader.Read(_path);
    //    for (int i = 0; i < data.Count; i++)
    //    {
    //        HeroStateData heroStateData = new HeroStateData();
    //        heroStateData.HeroStateID = CSVConvert.ToString(data[i]["HeroStateID"]);
    //        heroStateData.FirstGive = CSVConvert.ToFloat(data[i]["FirstGive"]);
    //        heroStateData.Min = CSVConvert.ToFloat(data[i]["Min"]);
    //        heroStateData.Max = CSVConvert.ToFloat(data[i]["Max"]);

    //        heroStateDatas.Add(heroStateData.HeroStateID, heroStateData);
    //    }
    //}

    //void LoadItemData(string _path)
    //{
    //    itemDatas.Clear();
    //    List<Dictionary<string, object>> data = CSVReader.Read(_path);
    //    for (int i = 0; i < data.Count; i++)
    //    {
    //        ItemData itemData = new ItemData();
    //        itemData.ItemID = CSVConvert.ToString(data[i]["ItemID"]);
    //        itemData.ItemStringID = CSVConvert.ToString(data[i]["ItemStringID"]);
    //        itemData.FirstGive = CSVConvert.ToInt(data[i]["FirstGive"]);

    //        itemDatas.Add(itemData.ItemID, itemData);
    //    }
    //}

    //void LoadJourneyData(string _path)
    //{
    //    journeyDatas.Clear();
    //    List<Dictionary<string, object>> data = CSVReader.Read(_path);
    //    for (int i = 0; i < data.Count; i++)
    //    {
    //        JourneyData journeyData = new JourneyData();
    //        journeyData.JourneyID = CSVConvert.ToString(data[i]["JourneyID"]);
    //        journeyData.StageDayID = CSVConvert.ToString(data[i]["StageDayID"]);
    //        journeyData.Item1Min = CSVConvert.ToInt(data[i]["Item1Min"]);
    //        journeyData.Item2Min = CSVConvert.ToInt(data[i]["Item2Min"]);
    //        journeyData.StageTitleStringID = CSVConvert.ToString(data[i]["StageTitleStringID"]);
    //        journeyData.JVwithObjectID = CSVConvert.ToString(data[i]["JVwithObjectID"]);
    //        journeyData.JVCutID = CSVConvert.ToString(data[i]["JVCutID"]);
    //        journeyData.JRwithHeroID = CSVConvert.ToString(data[i]["JRwithHeroID"]);

    //        journeyDatas.Add(journeyData.JourneyID, journeyData);
    //    }
    //}

    //void LoadJRwithHeroData(string _path)
    //{
    //    jRwithHeroDatas.Clear();
    //    List<Dictionary<string, object>> data = CSVReader.Read(_path);
    //    for (int i = 0; i < data.Count; i++)
    //    {
    //        JRwithHeroData jRwithHeroData = new JRwithHeroData();
    //        jRwithHeroData.JRwithHeroID = CSVConvert.ToString(data[i]["JRwithHeroID"]);
    //        jRwithHeroData.JRStateID = CSVConvert.ToString(data[i]["JRStateID"]);
    //        jRwithHeroData.JRanStringID = CSVConvert.ToString(data[i]["JRanStringID"]);

    //        jRwithHeroData.Hero1_Stress = CSVConvert.ToFloat(data[i]["Hero1_Stress"]);
    //        jRwithHeroData.Hero1_Power = CSVConvert.ToFloat(data[i]["Hero1_Power"]);
    //        jRwithHeroData.Hero1_Hp = CSVConvert.ToFloat(data[i]["Hero1_Hp"]);
    //        jRwithHeroData.Hero1_Exp = CSVConvert.ToFloat(data[i]["Hero1_Exp"]);

    //        jRwithHeroData.Hero2_Stress = CSVConvert.ToFloat(data[i]["Hero2_Stress"]);
    //        jRwithHeroData.Hero2_Power = CSVConvert.ToFloat(data[i]["Hero2_Power"]);
    //        jRwithHeroData.Hero2_Hp = CSVConvert.ToFloat(data[i]["Hero2_Hp"]);
    //        jRwithHeroData.Hero2_Exp = CSVConvert.ToFloat(data[i]["Hero2_Exp"]);

    //        jRwithHeroData.Hero3_Stress = CSVConvert.ToFloat(data[i]["Hero3_Stress"]);
    //        jRwithHeroData.Hero3_Power = CSVConvert.ToFloat(data[i]["Hero3_Power"]);
    //        jRwithHeroData.Hero3_Hp = CSVConvert.ToFloat(data[i]["Hero3_Hp"]);
    //        jRwithHeroData.Hero3_Exp = CSVConvert.ToFloat(data[i]["Hero3_Exp"]);

    //        jRwithHeroData.Hero4_Stress = CSVConvert.ToFloat(data[i]["Hero4_Stress"]);
    //        jRwithHeroData.Hero4_Power = CSVConvert.ToFloat(data[i]["Hero4_Power"]);
    //        jRwithHeroData.Hero4_Hp = CSVConvert.ToFloat(data[i]["Hero4_Hp"]);
    //        jRwithHeroData.Hero4_Exp = CSVConvert.ToFloat(data[i]["Hero4_Exp"]);

    //        jRwithHeroDatas.Add(jRwithHeroData.JRwithHeroID, jRwithHeroData);
    //    }
    //}

    //void LoadJStateData(string _path)
    //{
    //    jStateDatas.Clear();
    //    List<Dictionary<string, object>> data = CSVReader.Read(_path);
    //    for (int i = 0; i < data.Count; i++)
    //    {
    //        JStateData jStateData = new JStateData();
    //        jStateData.JStateID = CSVConvert.ToString(data[i]["JStateID"]);
    //        jStateData.JStateStringID = CSVConvert.ToString(data[i]["JStateStringID"]);
    //        jStateData.ResultPerfectProb = CSVConvert.ToFloat(data[i]["ResultPerfectProb"]);
    //        jStateData.ResultNormalProb = CSVConvert.ToFloat(data[i]["ResultNormalProb"]);
    //        jStateData.ResultFailProb = CSVConvert.ToFloat(data[i]["ResultFailProb"]);

    //        jStateDatas.Add(jStateData.JStateID, jStateData);
    //    }
    //}

    //void LoadJVCutData(string _path)
    //{
    //    jVCutDatas.Clear();
    //    List<Dictionary<string, object>> data = CSVReader.Read(_path);
    //    for (int i = 0; i < data.Count; i++)
    //    {
    //        JVCutData jVCutData = new JVCutData();
    //        jVCutData.JVCutID = CSVConvert.ToString(data[i]["JVCutID"]);
    //        jVCutData.StateGoodMin = CSVConvert.ToFloat(data[i]["StateGoodMin"]);
    //        jVCutData.StateNormalMin = CSVConvert.ToFloat(data[i]["StateNormalMin"]);
    //        jVCutData.StateBadMin = CSVConvert.ToFloat(data[i]["StateBadMin"]);
    //        jVCutData.StateDangerMin = CSVConvert.ToFloat(data[i]["StateDangerMin"]);

    //        jVCutDatas.Add(jVCutData.JVCutID, jVCutData);
    //    }
    //}

    //void LoadJVwithObjectData(string _path)
    //{
    //    jVwithObjectDatas.Clear();
    //    List<Dictionary<string, object>> data = CSVReader.Read(_path);
    //    for (int i = 0; i < data.Count; i++)
    //    {
    //        JVwithObjectData jVwithObjectData = new JVwithObjectData();
    //        jVwithObjectData.JVwithObjectID = CSVConvert.ToString(data[i]["JVwithObjectID"]);
    //        jVwithObjectData.MeatValue = CSVConvert.ToFloat(data[i]["MeatValue"]);
    //        jVwithObjectData.WaterValue = CSVConvert.ToFloat(data[i]["WaterValue"]);
    //        jVwithObjectData.HubValue = CSVConvert.ToFloat(data[i]["HubValue"]);
    //        jVwithObjectData.FoodValue = CSVConvert.ToFloat(data[i]["FoodValue"]);
    //        jVwithObjectData.HStressValue = CSVConvert.ToFloat(data[i]["HStressValue"]);
    //        jVwithObjectData.HPowerValue = CSVConvert.ToFloat(data[i]["HPowerValue"]);

    //        jVwithObjectDatas.Add(jVwithObjectData.JVwithObjectID, jVwithObjectData);
    //    }
    //}

    //void LoadStageDayData(string _path)
    //{
    //    stageDayDatas.Clear();
    //    List<Dictionary<string, object>> data = CSVReader.Read(_path);
    //    for (int i = 0; i < data.Count; i++)
    //    {
    //        StageDayData stageDayData = new StageDayData();
    //        stageDayData.StageDayID = CSVConvert.ToString(data[i]["StageDayID"]);
    //        stageDayData.StageID = CSVConvert.ToString(data[i]["StageID"]);
    //        stageDayData.StageDayStringID = CSVConvert.ToString(data[i]["StageDayStringID"]);
    //        stageDayData.Time = CSVConvert.ToFloat(data[i]["Time"]);
    //        stageDayData.HeroBackTime = CSVConvert.ToFloat(data[i]["HeroBackTime"]);

    //        stageDayDatas.Add(stageDayData.StageDayID, stageDayData);
    //    }
    //}

    //void LoadStageData(string _path)
    //{
    //    stageDatas.Clear();
    //    List<Dictionary<string, object>> data = CSVReader.Read(_path);
    //    for (int i = 0; i < data.Count; i++)
    //    {
    //        StageData stageData = new StageData();
    //        stageData.StageID = CSVConvert.ToString(data[i]["StageID"]);
    //        stageData.StageStringID = CSVConvert.ToString(data[i]["StageStringID"]);

    //        stageDatas.Add(stageData.StageID, stageData);
    //    }
    //}

    //void LoadStringData(string _path)
    //{
    //    stringDatas.Clear();
    //    List<Dictionary<string, object>> data = CSVReader.Read(_path);
    //    for (int i = 0; i < data.Count; i++)
    //    {
    //        StringData stringData = new StringData();
    //        stringData.StringID = CSVConvert.ToString(data[i]["StringID"]);
    //        stringData.KOR = CSVConvert.ToString(data[i]["KOR"]);
    //        stringData.ENG = CSVConvert.ToString(data[i]["ENG"]);

    //        stringDatas.Add(stringData.StringID, stringData);
    //    }
    //}





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
    #endregion
}

//[System.Serializable]
//public class HeroStateData
//{
//    public string HeroStateID;
//    public float FirstGive;
//    public float Min;
//    public float Max;
//}

//[System.Serializable]
//public class ItemData
//{
//    public string ItemID;
//    public string ItemStringID;
//    public int FirstGive;
//}

//[System.Serializable]
//public class JourneyData
//{
//    public string JourneyID;
//    public string StageDayID;
//    public int Item1Min;
//    public int Item2Min;
//    public string StageTitleStringID;
//    public string JVwithObjectID;
//    public string JVCutID;
//    public string JRwithHeroID;
//}

//[System.Serializable]
//public class JRwithHeroData
//{
//    public string JRwithHeroID;
//    public string JRStateID;
//    public string JRanStringID;
//    public float Hero1_Stress;
//    public float Hero1_Power;
//    public float Hero1_Hp;
//    public float Hero1_Exp;
//    public float Hero2_Stress;
//    public float Hero2_Power;
//    public float Hero2_Hp;
//    public float Hero2_Exp;
//    public float Hero3_Stress;
//    public float Hero3_Power;
//    public float Hero3_Hp;
//    public float Hero3_Exp;
//    public float Hero4_Stress;
//    public float Hero4_Power;
//    public float Hero4_Hp;
//    public float Hero4_Exp;
//}

//[System.Serializable]
//public class JStateData
//{
//    public string JStateID;
//    public string JStateStringID;
//    public float ResultPerfectProb;
//    public float ResultNormalProb;
//    public float ResultFailProb;
//}

//[System.Serializable]
//public class JVCutData
//{
//    public string JVCutID;
//    public float StateGoodMin;
//    public float StateNormalMin;
//    public float StateBadMin;
//    public float StateDangerMin;
//}

//[System.Serializable]
//public class JVwithObjectData
//{
//    public string JVwithObjectID;
//    public float MeatValue;
//    public float WaterValue;
//    public float HubValue;
//    public float FoodValue;
//    public float HStressValue;
//    public float HPowerValue;
//}

//[System.Serializable]
//public class StageDayData
//{
//    public string StageDayID;
//    public string StageID;
//    public string StageDayStringID;
//    public float Time;
//    public float HeroBackTime;
//}

//[System.Serializable]
//public class StageData
//{
//    public string StageID;
//    public string StageStringID;
//}

//[System.Serializable]
//public class StringData
//{
//    public string StringID;
//    public string KOR;
//    public string ENG;
//}

public class CSVConvert
{
    public static int ToInt(object _obj)
    {
        string body = _obj.ToString();
        //body = Regex.Replace(body, @"[^0-9]", "");
        return int.Parse(body);
    }

    public static float ToFloat(object _obj)
    {
        string body = _obj.ToString();
        //body = Regex.Replace(body, @"[^0-9]", "");
        return float.Parse(body);
    }

    public static string ToString(object _obj)
    {
        return _obj.ToString();
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