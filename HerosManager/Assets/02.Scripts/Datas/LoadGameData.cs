using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;

public class LoadGameData : Singleton<LoadGameData>
{
    public Dictionary<string, BonfireData> bonfireDatas = new Dictionary<string, BonfireData>();
    public Dictionary<string, FenceData> fenceDatas = new Dictionary<string, FenceData>();
    public Dictionary<string, FieldData> fieldDatas = new Dictionary<string, FieldData>();
    public Dictionary<string, HeroStateData> heroStateDatas = new Dictionary<string, HeroStateData>();
    public Dictionary<string, HeroData> heroDatas = new Dictionary<string, HeroData>();
    public Dictionary<string, InvadeData> invadeDatas = new Dictionary<string, InvadeData>();
    public Dictionary<string, ItemData> itemDatas = new Dictionary<string, ItemData>();
    public Dictionary<string, JourneyData> journeyDatas = new Dictionary<string, JourneyData>();
    public Dictionary<string, JRwithHeroData> jRwithHeroDatas = new Dictionary<string, JRwithHeroData>();
    public Dictionary<string, JStateData> jStateDatas = new Dictionary<string, JStateData>();
    public Dictionary<string, JVCutData> jVCutDatas = new Dictionary<string, JVCutData>();
    public Dictionary<string, JVwithObjectData> jVwithObjectDatas = new Dictionary<string, JVwithObjectData>();
    public Dictionary<string, LaundryData> laundryDatas = new Dictionary<string, LaundryData>();
    public Dictionary<string, MiniGameData> miniGameDatas = new Dictionary<string, MiniGameData>();
    public Dictionary<string, StageDayData> stageDayDatas = new Dictionary<string, StageDayData>();
    public Dictionary<string, StageData> stageDatas = new Dictionary<string, StageData>();
    public Dictionary<string, StringData> stringDatas = new Dictionary<string, StringData>();

    public void LoadCSVDatas()
    {
        LoadBonfireData("CSVData/BonfireTable");
        LoadFenceData("CSVData/FenceTable");
        LoadFieldData("CSVData/FieldTable");
        LoadHeroStateData("CSVData/HeroStateTable");
        LoadHeroData("CSVData/HeroTable");
        LoadInvadeData("CSVData/InvadeTable");
        LoadItemData("CSVData/ItemTable");
        LoadJourneyData("CSVData/JourneyTable");
        LoadJRwithHeroData("CSVData/JRwithHeroTable");
        LoadJStateData("CSVData/JStateTable");
        LoadJVCutData("CSVData/JVCutTable");
        LoadJVwithObjectData("CSVData/JVwithObjectTable");
        LoadLaundryData("CSVData/LaundryTable");
        LoadMiniGameData("CSVData/MiniGameTable");
        LoadStageDayData("CSVData/StageDayTable");
        LoadStageData("CSVData/StageTable");
        LoadStringData("CSVData/StringTable");
    }

    void LoadBonfireData(string _path)
    {
        bonfireDatas.Clear();
        List<Dictionary<string, object>> data = CSVReader.Read(_path);
        for (int i = 0; i < data.Count; i++)
        {
            BonfireData bonfireData = new BonfireData();
            bonfireData.BonfireID = CSVConvert.ToString(data[i]["BonfireID"]);
            bonfireData.StageID = CSVConvert.ToString(data[i]["StageID"]);
            bonfireData.NeedItemID = CSVConvert.ToString(data[i]["NeedItemID"]);
            bonfireData.NeedAmount = CSVConvert.ToInt(data[i]["NeedAmount"]);
            bonfireData.Time1 = CSVConvert.ToFloat(data[i]["Time1"]);
            bonfireData.Time2 = CSVConvert.ToFloat(data[i]["Time2"]);
            bonfireData.Time3 = CSVConvert.ToFloat(data[i]["Time3"]);
            bonfireData.Hero1_Stress = CSVConvert.ToFloat(data[i]["Hero1_Stress"]);
            bonfireData.Hero2_Stress = CSVConvert.ToFloat(data[i]["Hero2_Stress"]);
            bonfireData.Hero3_Stress = CSVConvert.ToFloat(data[i]["Hero3_Stress"]);
            bonfireData.Hero4_Stress = CSVConvert.ToFloat(data[i]["Hero4_Stress"]);

            bonfireDatas.Add(bonfireData.BonfireID, bonfireData);
        }
    }

    void LoadFenceData(string _path)
    {
        fenceDatas.Clear();
        List<Dictionary<string, object>> data = CSVReader.Read(_path);
        for (int i = 0; i < data.Count; i++)
        {
            FenceData fenceData = new FenceData();
            fenceData.FenceID = CSVConvert.ToString(data[i]["FenceID"]);
            fenceData.StageID = CSVConvert.ToString(data[i]["StageID"]);
            fenceData.FenceLevel = CSVConvert.ToInt(data[i]["FenceLevel"]);
            fenceData.InvadeProb = CSVConvert.ToFloat(data[i]["InvadeProb"]);
            fenceData.NeedItemID = CSVConvert.ToString(data[i]["NeedItemID"]);
            fenceData.Amount = CSVConvert.ToInt(data[i]["Amount"]);
            fenceData.NeedTime = CSVConvert.ToFloat(data[i]["NeedTime"]);

            fenceDatas.Add(fenceData.FenceID, fenceData);
        }
    }

    void LoadFieldData(string _path)
    {
        fieldDatas.Clear();
        List<Dictionary<string, object>> data = CSVReader.Read(_path);
        for (int i = 0; i < data.Count; i++)
        {
            FieldData fieldData = new FieldData();
            fieldData.FieldID = CSVConvert.ToString(data[i]["FieldID"]);
            fieldData.StageID = CSVConvert.ToString(data[i]["StageID"]);
            fieldData.NeedItemID = CSVConvert.ToString(data[i]["NeedItemID"]);
            fieldData.NeedAmount = CSVConvert.ToInt(data[i]["NeedAmount"]);
            fieldData.Time1 = CSVConvert.ToFloat(data[i]["Time1"]);
            fieldData.Time2 = CSVConvert.ToFloat(data[i]["Time2"]);
            fieldData.Repeat = CSVConvert.ToInt(data[i]["Repeat"]);
            fieldData.GetItemID = CSVConvert.ToString(data[i]["GetItemID"]);
            fieldData.GetAmount = CSVConvert.ToInt(data[i]["GetAmount"]);


            fieldDatas.Add(fieldData.FieldID, fieldData);
        }
    }

    void LoadHeroStateData(string _path)
    {
        heroStateDatas.Clear();
        List<Dictionary<string, object>> data = CSVReader.Read(_path);
        for (int i = 0; i < data.Count; i++)
        {
            HeroStateData heroStateData = new HeroStateData();
            heroStateData.HeroStateID = CSVConvert.ToString(data[i]["HeroStateID"]);
            heroStateData.FirstGive = CSVConvert.ToFloat(data[i]["FirstGive"]);
            heroStateData.Min = CSVConvert.ToFloat(data[i]["Min"]);
            heroStateData.Max = CSVConvert.ToFloat(data[i]["Max"]);

            heroStateDatas.Add(heroStateData.HeroStateID, heroStateData);
        }
    }

    void LoadHeroData(string _path)
    {
        heroDatas.Clear();
        List<Dictionary<string, object>> data = CSVReader.Read(_path);
        for (int i = 0; i < data.Count; i++)
        {
            HeroData heroData = new HeroData();
            heroData.HeroID = CSVConvert.ToString(data[i]["HeroID"]);
            heroData.HeroStringID = CSVConvert.ToString(data[i]["HeroStringID"]);
            heroData.StressID = CSVConvert.ToString(data[i]["StressID"]);
            heroData.StressStringID = CSVConvert.ToString(data[i]["StressStringID"]);
            heroData.PowerID = CSVConvert.ToString(data[i]["PowerID"]);
            heroData.PowerStringID = CSVConvert.ToString(data[i]["PowerStringID"]);
            heroData.HpID = CSVConvert.ToString(data[i]["HpID"]);
            heroData.HpStringID = CSVConvert.ToString(data[i]["HpStringID"]);
            heroData.ExpID = CSVConvert.ToString(data[i]["ExpID"]);
            heroData.ExpStringID = CSVConvert.ToString(data[i]["ExpStringID"]);

            heroDatas.Add(heroData.HeroID, heroData);
        }
    }

    void LoadInvadeData(string _path)
    {
        invadeDatas.Clear();
        List<Dictionary<string, object>> data = CSVReader.Read(_path);
        for (int i = 0; i < data.Count; i++)
        {
            InvadeData invadeData = new InvadeData();
            invadeData.InvadeID = CSVConvert.ToString(data[i]["InvadeID"]);
            invadeData.StageDayID = CSVConvert.ToString(data[i]["StageDayID"]);
            invadeData.InvadeStringID = CSVConvert.ToString(data[i]["InvadeStringID"]);
            invadeData.StealObject = CSVConvert.ToInt(data[i]["StealObject"]);
            invadeData.RandMin = CSVConvert.ToInt(data[i]["RandMin"]);
            invadeData.RandMax = CSVConvert.ToInt(data[i]["RandMax"]);

            invadeDatas.Add(invadeData.InvadeID, invadeData);
        }
    }

    void LoadItemData(string _path)
    {
        itemDatas.Clear();
        List<Dictionary<string, object>> data = CSVReader.Read(_path);
        for (int i = 0; i < data.Count; i++)
        {
            ItemData itemData = new ItemData();
            itemData.ItemID = CSVConvert.ToString(data[i]["ItemID"]);
            itemData.ItemStringID = CSVConvert.ToString(data[i]["ItemStringID"]);
            itemData.FirstGive = CSVConvert.ToInt(data[i]["FirstGive"]);

            itemDatas.Add(itemData.ItemID, itemData);
        }
    }

    void LoadJourneyData(string _path)
    {
        journeyDatas.Clear();
        List<Dictionary<string, object>> data = CSVReader.Read(_path);
        for (int i = 0; i < data.Count; i++)
        {
            JourneyData journeyData = new JourneyData();
            journeyData.JourneyID = CSVConvert.ToString(data[i]["JourneyID"]);
            journeyData.StageDayID = CSVConvert.ToString(data[i]["StageDayID"]);
            journeyData.Item1Min = CSVConvert.ToInt(data[i]["Item1Min"]);
            journeyData.Item2Min = CSVConvert.ToInt(data[i]["Item2Min"]);
            journeyData.StageTitleStringID = CSVConvert.ToString(data[i]["StageTitleStringID"]);
            journeyData.JVwithObjectID = CSVConvert.ToString(data[i]["JVwithObjectID"]);
            journeyData.JVCutID = CSVConvert.ToString(data[i]["JVCutID"]);
            journeyData.JRwithHeroID = CSVConvert.ToString(data[i]["JRwithHeroID"]);

            journeyDatas.Add(journeyData.JourneyID, journeyData);
        }
    }

    void LoadJRwithHeroData(string _path)
    {
        jRwithHeroDatas.Clear();
        List<Dictionary<string, object>> data = CSVReader.Read(_path);
        for (int i = 0; i < data.Count; i++)
        {
            JRwithHeroData jRwithHeroData = new JRwithHeroData();
            jRwithHeroData.JRwithHeroID = CSVConvert.ToString(data[i]["JRwithHeroID"]);
            jRwithHeroData.JRStateID = CSVConvert.ToString(data[i]["JRStateID"]);
            jRwithHeroData.JRanStringID = CSVConvert.ToString(data[i]["JRanStringID"]);

            jRwithHeroData.Hero1_Stress = CSVConvert.ToFloat(data[i]["Hero1_Stress"]);
            jRwithHeroData.Hero1_Power = CSVConvert.ToFloat(data[i]["Hero1_Power"]);
            jRwithHeroData.Hero1_Hp = CSVConvert.ToFloat(data[i]["Hero1_Hp"]);
            jRwithHeroData.Hero1_Exp = CSVConvert.ToFloat(data[i]["Hero1_Exp"]);

            jRwithHeroData.Hero2_Stress = CSVConvert.ToFloat(data[i]["Hero2_Stress"]);
            jRwithHeroData.Hero2_Power = CSVConvert.ToFloat(data[i]["Hero2_Power"]);
            jRwithHeroData.Hero2_Hp = CSVConvert.ToFloat(data[i]["Hero2_Hp"]);
            jRwithHeroData.Hero2_Exp = CSVConvert.ToFloat(data[i]["Hero2_Exp"]);

            jRwithHeroData.Hero3_Stress = CSVConvert.ToFloat(data[i]["Hero3_Stress"]);
            jRwithHeroData.Hero3_Power = CSVConvert.ToFloat(data[i]["Hero3_Power"]);
            jRwithHeroData.Hero3_Hp = CSVConvert.ToFloat(data[i]["Hero3_Hp"]);
            jRwithHeroData.Hero3_Exp = CSVConvert.ToFloat(data[i]["Hero3_Exp"]);

            jRwithHeroData.Hero4_Stress = CSVConvert.ToFloat(data[i]["Hero4_Stress"]);
            jRwithHeroData.Hero4_Power = CSVConvert.ToFloat(data[i]["Hero4_Power"]);
            jRwithHeroData.Hero4_Hp = CSVConvert.ToFloat(data[i]["Hero4_Hp"]);
            jRwithHeroData.Hero4_Exp = CSVConvert.ToFloat(data[i]["Hero4_Exp"]);

            jRwithHeroDatas.Add(jRwithHeroData.JRwithHeroID, jRwithHeroData);
        }
    }

    void LoadJStateData(string _path)
    {
        jStateDatas.Clear();
        List<Dictionary<string, object>> data = CSVReader.Read(_path);
        for (int i = 0; i < data.Count; i++)
        {
            JStateData jStateData = new JStateData();
            jStateData.JStateID = CSVConvert.ToString(data[i]["JStateID"]);
            jStateData.JStateStringID = CSVConvert.ToString(data[i]["JStateStringID"]);
            jStateData.ResultPerfectProb = CSVConvert.ToFloat(data[i]["ResultPerfectProb"]);
            jStateData.ResultNormalProb = CSVConvert.ToFloat(data[i]["ResultNormalProb"]);
            jStateData.ResultFailProb = CSVConvert.ToFloat(data[i]["ResultFailProb"]);

            jStateDatas.Add(jStateData.JStateID, jStateData);
        }
    }

    void LoadJVCutData(string _path)
    {
        jVCutDatas.Clear();
        List<Dictionary<string, object>> data = CSVReader.Read(_path);
        for (int i = 0; i < data.Count; i++)
        {
            JVCutData jVCutData = new JVCutData();
            jVCutData.JVCutID = CSVConvert.ToString(data[i]["JVCutID"]);
            jVCutData.StateGoodMin = CSVConvert.ToFloat(data[i]["StateGoodMin"]);
            jVCutData.StateNormalMin = CSVConvert.ToFloat(data[i]["StateNormalMin"]);
            jVCutData.StateBadMin = CSVConvert.ToFloat(data[i]["StateBadMin"]);
            jVCutData.StateDangerMin = CSVConvert.ToFloat(data[i]["StateDangerMin"]);

            jVCutDatas.Add(jVCutData.JVCutID, jVCutData);
        }
    }

    void LoadJVwithObjectData(string _path)
    {
        jVwithObjectDatas.Clear();
        List<Dictionary<string, object>> data = CSVReader.Read(_path);
        for (int i = 0; i < data.Count; i++)
        {
            JVwithObjectData jVwithObjectData = new JVwithObjectData();
            jVwithObjectData.JVwithObjectID = CSVConvert.ToString(data[i]["JVwithObjectID"]);
            jVwithObjectData.MeatValue = CSVConvert.ToFloat(data[i]["MeatValue"]);
            jVwithObjectData.WaterValue = CSVConvert.ToFloat(data[i]["WaterValue"]);
            jVwithObjectData.HubValue = CSVConvert.ToFloat(data[i]["HubValue"]);
            jVwithObjectData.FoodValue = CSVConvert.ToFloat(data[i]["FoodValue"]);
            jVwithObjectData.HStressValue = CSVConvert.ToFloat(data[i]["HStressValue"]);
            jVwithObjectData.HPowerValue = CSVConvert.ToFloat(data[i]["HPowerValue"]);

            jVwithObjectDatas.Add(jVwithObjectData.JVwithObjectID, jVwithObjectData);
        }
    }

    void LoadLaundryData(string _path)
    {
        laundryDatas.Clear();
        List<Dictionary<string, object>> data = CSVReader.Read(_path);
        for (int i = 0; i < data.Count; i++)
        {
            LaundryData laundryData = new LaundryData();
            laundryData.LaundryID = CSVConvert.ToString(data[i]["LaundryID"]);
            laundryData.StageID = CSVConvert.ToString(data[i]["StageID"]);
            laundryData.NeedItemID = CSVConvert.ToString(data[i]["NeedItemID"]);
            laundryData.NeedAmount = CSVConvert.ToInt(data[i]["NeedAmount"]);
            laundryData.StressAdd = CSVConvert.ToFloat(data[i]["StressAdd"]);
            laundryData.StringID = CSVConvert.ToString(data[i]["StringID"]);
            laundryData.L1_GermCount = CSVConvert.ToInt(data[i]["L1_GermCount"]);
            laundryData.L2_RinseCount = CSVConvert.ToInt(data[i]["L2_RinseCount"]);
            laundryData.L3_MinTime = CSVConvert.ToFloat(data[i]["L3_MinTime"]);
            laundryData.L3_MaxTime = CSVConvert.ToFloat(data[i]["L3_MaxTime"]);
            laundryData.L3_TotalTime = CSVConvert.ToFloat(data[i]["L3_TotalTime"]);

            laundryDatas.Add(laundryData.LaundryID, laundryData);
        }
    }

    void LoadMiniGameData(string _path)
    {
        miniGameDatas.Clear();
        List<Dictionary<string, object>> data = CSVReader.Read(_path);
        for (int i = 0; i < data.Count; i++)
        {
            MiniGameData miniGameData = new MiniGameData();
            miniGameData.ObjectCode = CSVConvert.ToString(data[i]["ObjectCode"]);
            miniGameData.StageCode = CSVConvert.ToString(data[i]["StageCode"]);
            miniGameData.Object_type = CSVConvert.ToInt(data[i]["Object_type"]);
            miniGameData.Wood = CSVConvert.ToInt(data[i]["Wood"]);
            miniGameData.Water = CSVConvert.ToInt(data[i]["Water"]);
            miniGameData.Meat = CSVConvert.ToInt(data[i]["Meat"]);
            miniGameData.Hub = CSVConvert.ToInt(data[i]["Hub"]);
            miniGameData.Jem = CSVConvert.ToInt(data[i]["Jem"]);
            miniGameData.Probability = CSVConvert.ToFloat(data[i]["Probability"]);
            miniGameData.Stun = CSVConvert.ToFloat(data[i]["Stun"]);
            miniGameData.Invincibility = CSVConvert.ToFloat(data[i]["Invincibility"]);
            miniGameData.Velocity = CSVConvert.ToFloat(data[i]["Velocity"]);
            miniGameData.CoolTime = CSVConvert.ToFloat(data[i]["CoolTime"]);
            miniGameData.GameEnvironment = CSVConvert.ToInt(data[i]["GameEnvironment"]);
            miniGameData.value1 = CSVConvert.ToFloat(data[i]["value1"]);
            miniGameData.value2 = CSVConvert.ToFloat(data[i]["value2"]);
            miniGameData.value3 = CSVConvert.ToFloat(data[i]["value3"]);
            miniGameData.value4 = CSVConvert.ToFloat(data[i]["value4"]);
            miniGameData.value5 = CSVConvert.ToFloat(data[i]["value5"]);
            miniGameData.value6 = CSVConvert.ToFloat(data[i]["value6"]);

            miniGameDatas.Add(miniGameData.ObjectCode, miniGameData);
        }
    }

    void LoadStageDayData(string _path)
    {
        stageDayDatas.Clear();
        List<Dictionary<string, object>> data = CSVReader.Read(_path);
        for (int i = 0; i < data.Count; i++)
        {
            StageDayData stageDayData = new StageDayData();
            stageDayData.StageDayID = CSVConvert.ToString(data[i]["StageDayID"]);
            stageDayData.StageID = CSVConvert.ToString(data[i]["StageID"]);
            stageDayData.StageDayStringID = CSVConvert.ToString(data[i]["StageDayStringID"]);
            stageDayData.Time = CSVConvert.ToFloat(data[i]["Time"]);
            stageDayData.HeroBackTime = CSVConvert.ToFloat(data[i]["HeroBackTime"]);

            stageDayDatas.Add(stageDayData.StageDayID, stageDayData);
        }
    }

    void LoadStageData(string _path)
    {
        stageDatas.Clear();
        List<Dictionary<string, object>> data = CSVReader.Read(_path);
        for (int i = 0; i < data.Count; i++)
        {
            StageData stageData = new StageData();
            stageData.StageID = CSVConvert.ToString(data[i]["StageID"]);
            stageData.StageStringID = CSVConvert.ToString(data[i]["StageStringID"]);

            stageDatas.Add(stageData.StageID, stageData);
        }
    }

    void LoadStringData(string _path)
    {
        stringDatas.Clear();
        List<Dictionary<string, object>> data = CSVReader.Read(_path);
        for (int i = 0; i < data.Count; i++)
        {
            StringData stringData = new StringData();
            stringData.StringID = CSVConvert.ToString(data[i]["StringID"]);
            stringData.KOR = CSVConvert.ToString(data[i]["KOR"]);
            stringData.ENG = CSVConvert.ToString(data[i]["ENG"]);

            stringDatas.Add(stringData.StringID, stringData);
        }
    }

    public string GetString(string _code)
    {
        if (!stringDatas.ContainsKey(_code))
            return "String Code is Null";

        if (Application.systemLanguage == SystemLanguage.Korean)
        {
            return stringDatas[_code].KOR;
        }
        else
        {
            return stringDatas[_code].ENG;
        }
    }
}

[System.Serializable]
public class BonfireData
{
    public string BonfireID;
    public string StageID;
    public string NeedItemID;
    public int NeedAmount;
    public float Time1;
    public float Time2;
    public float Time3;
    public float Hero1_Stress;
    public float Hero2_Stress;
    public float Hero3_Stress;
    public float Hero4_Stress;
}

[System.Serializable]
public class FenceData
{
    public string FenceID;
    public string StageID;
    public int FenceLevel;
    public float InvadeProb;
    public string NeedItemID;
    public int Amount;
    public float NeedTime;
}

[System.Serializable]
public class FieldData
{
    public string FieldID;
    public string StageID;
    public string NeedItemID;
    public int NeedAmount;
    public float Time1;
    public float Time2;
    public int Repeat;
    public string GetItemID;
    public int GetAmount;
}

[System.Serializable]
public class HeroStateData
{
    public string HeroStateID;
    public float FirstGive;
    public float Min;
    public float Max;
}

[System.Serializable]
public class HeroData
{
    public string HeroID;
    public string HeroStringID;
    public string StressID;
    public string StressStringID;
    public string PowerID;
    public string PowerStringID;
    public string HpID;
    public string HpStringID;
    public string ExpID;
    public string ExpStringID;
}

[System.Serializable]
public class InvadeData
{
    public string InvadeID;
    public string StageDayID;
    public string InvadeStringID;
    public int StealObject;
    public int RandMin;
    public int RandMax;
}

[System.Serializable]
public class ItemData
{
    public string ItemID;
    public string ItemStringID;
    public int FirstGive;
}

[System.Serializable]
public class JourneyData
{
    public string JourneyID;
    public string StageDayID;
    public int Item1Min;
    public int Item2Min;
    public string StageTitleStringID;
    public string JVwithObjectID;
    public string JVCutID;
    public string JRwithHeroID;
}

[System.Serializable]
public class JRwithHeroData
{
    public string JRwithHeroID;
    public string JRStateID;
    public string JRanStringID;
    public float Hero1_Stress;
    public float Hero1_Power;
    public float Hero1_Hp;
    public float Hero1_Exp;
    public float Hero2_Stress;
    public float Hero2_Power;
    public float Hero2_Hp;
    public float Hero2_Exp;
    public float Hero3_Stress;
    public float Hero3_Power;
    public float Hero3_Hp;
    public float Hero3_Exp;
    public float Hero4_Stress;
    public float Hero4_Power;
    public float Hero4_Hp;
    public float Hero4_Exp;
}

[System.Serializable]
public class JStateData
{
    public string JStateID;
    public string JStateStringID;
    public float ResultPerfectProb;
    public float ResultNormalProb;
    public float ResultFailProb;
}

[System.Serializable]
public class JVCutData
{
    public string JVCutID;
    public float StateGoodMin;
    public float StateNormalMin;
    public float StateBadMin;
    public float StateDangerMin;
}

[System.Serializable]
public class JVwithObjectData
{
    public string JVwithObjectID;
    public float MeatValue;
    public float WaterValue;
    public float HubValue;
    public float FoodValue;
    public float HStressValue;
    public float HPowerValue;
}

[System.Serializable]
public class LaundryData
{
    public string LaundryID;
    public string StageID;
    public string NeedItemID;
    public int NeedAmount;
    public float StressAdd;
    public string StringID;
    public int L1_GermCount;
    public int L2_RinseCount;
    public float L3_MinTime;
    public float L3_MaxTime;
    public float L3_TotalTime;
}

[System.Serializable]
public class MiniGameData
{
    public string ObjectCode;
    public string StageCode;
    public int Object_type;
    public int Wood;
    public int Water;
    public int Meat;
    public int Hub;
    public int Jem;
    public float Probability;
    public float Stun;
    public float Invincibility;
    public float Velocity;
    public float CoolTime;
    public int GameEnvironment;
    public float value1;
    public float value2;
    public float value3;
    public float value4;
    public float value5;
    public float value6;
}

[System.Serializable]
public class StageDayData
{
    public string StageDayID;
    public string StageID;
    public string StageDayStringID;
    public float Time;
    public float HeroBackTime;
}

[System.Serializable]
public class StageData
{
    public string StageID;
    public string StageStringID;
}

[System.Serializable]
public class StringData
{
    public string StringID;
    public string KOR;
    public string ENG;
}

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