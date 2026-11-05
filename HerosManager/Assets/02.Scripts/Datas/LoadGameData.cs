using System.Collections.Generic;
using UnityEngine;
using System.Globalization;
using System;

public class LoadGameData : Singleton<LoadGameData>
{
    public Dictionary<string, EventData> eventDatas = new Dictionary<string, EventData>();
    public Dictionary<string, ChoiceData> choiceDatas = new Dictionary<string, ChoiceData>();
    public Dictionary<string, CookData> cookDatas = new Dictionary<string, CookData>();
    public Dictionary<string, CutsceneData> cutsceneDatas = new Dictionary<string, CutsceneData>();
    public Dictionary<string, BonfireData> bonfireDatas = new Dictionary<string, BonfireData>();
    public Dictionary<string, CollectSpaceData> collectSpaceDatas = new Dictionary<string, CollectSpaceData>();
    public Dictionary<string, CollectData> collectDatas = new Dictionary<string, CollectData>();
    public Dictionary<string, DefineData> defineDatas = new Dictionary<string, DefineData>();
    public Dictionary<string, FenceData> fenceDatas = new Dictionary<string, FenceData>();
    public Dictionary<string, FieldData> fieldDatas = new Dictionary<string, FieldData>();
    public Dictionary<string, HeroStateData> heroStateDatas = new Dictionary<string, HeroStateData>();
    public Dictionary<string, HeroData> heroDatas = new Dictionary<string, HeroData>();
    public Dictionary<string, InvadeData> invadeDatas = new Dictionary<string, InvadeData>();
    public Dictionary<string, ItemData> itemDatas = new Dictionary<string, ItemData>();
    public Dictionary<string, JourneyData> journeyDatas = new Dictionary<string, JourneyData>();
    public Dictionary<string, JResultData> jResultDatas = new Dictionary<string, JResultData>();
    public Dictionary<string, JStateData> jStateDatas = new Dictionary<string, JStateData>();
    public Dictionary<string, MiniGameData> miniGameDatas = new Dictionary<string, MiniGameData>();
    public Dictionary<string, StageDayData> stageDayDatas = new Dictionary<string, StageDayData>();
    public Dictionary<string, StageData> stageDatas = new Dictionary<string, StageData>();
    public Dictionary<string, StringData> stringDatas = new Dictionary<string, StringData>();
    public Dictionary<string, JemData> jemDatas = new Dictionary<string, JemData>();
    public Dictionary<string, LaundryData> laundryDatas = new Dictionary<string, LaundryData>();
    public Dictionary<string, TutorialData> tutorialDatas = new Dictionary<string, TutorialData>();

    public bool IsLoaded { get; private set; }

    public override void Init()
    {
        if (Application.isPlaying)
        {
            transform.SetParent(null);
            DontDestroyOnLoad(gameObject);
        }
        LoadTitleDatas();
    }

    public void LoadTitleDatas()
    {
        if (IsLoaded)
            return;
        LoadTable("Json/CookTable", cookDatas, row => row.CookID);
        LoadTable("Json/CutsceneTable", cutsceneDatas, row => row.CutSceneID);
        LoadTable("Json/BonfireTable", bonfireDatas, row => row.BonfireID);
        LoadTable("Json/CollectSpaceTable", collectSpaceDatas, row => row.CollectSpaceID);
        LoadTable("Json/CollectTable", collectDatas, row => row.CollectID);
        LoadTable("Json/DefineTable", defineDatas, row => row.DefineID);
        LoadTable("Json/FenceTable", fenceDatas, row => row.FenceID);
        LoadTable("Json/FieldTable", fieldDatas, row => row.FieldID);
        LoadTable("Json/HeroStateTable", heroStateDatas, row => row.HeroStateID);
        LoadTable("Json/HeroTable", heroDatas, row => row.HeroID);
        LoadTable("Json/InvadeTable", invadeDatas, row => row.InvadeID);
        LoadTable("Json/ItemTable", itemDatas, row => row.ItemID);
        LoadTable("Json/JourneyTable", journeyDatas, row => row.JourneyID);
        LoadTable("Json/JResultTable", jResultDatas, row => row.JResultID);
        LoadTable("Json/JStateTable", jStateDatas, row => row.JStateID);
        LoadTable("Json/MiniGameTable", miniGameDatas, row => row.ObjectID);
        LoadTable("Json/StageDayTable", stageDayDatas, row => row.StageDayID);
        LoadTable("Json/StageTable", stageDatas, row => row.StageID);
        LoadTable("Json/ChoiceTable", choiceDatas, row => row.ChoiceID);
        LoadTable("Json/JemTable", jemDatas, row => row.JemID);
        LoadTable("Json/LaundryTable", laundryDatas, row => row.LaundryID);
        LoadTable("Json/EventTable", eventDatas, row => row.EventID);
        LoadTable("Json/TutorialTable", tutorialDatas, row => row.TutorialID);
        LoadTable("Json/StringTable", stringDatas, row => row.StringID);
        IsLoaded = true;
    }

    private static void LoadTable<T>(string path, Dictionary<string, T> destination, Func<T, string> getId)
    {
        TextAsset asset = Resources.Load<TextAsset>(path);
        if (asset == null)
            throw new InvalidOperationException($"Missing game table: Resources/{path}");
        T[] rows = JsonUtilityHelper.FromJson<T>(asset.text);
        var loaded = new Dictionary<string, T>(StringComparer.Ordinal);
        foreach (T row in rows)
        {
            string id = getId(row);
            if (string.IsNullOrWhiteSpace(id) || !loaded.TryAdd(id, row))
                throw new InvalidOperationException($"Invalid or duplicate ID '{id}' in {path}");
        }
        destination.Clear();
        foreach (var row in loaded)
            destination.Add(row.Key, row.Value);
    }

    public string GetString(string _code)
    {
        if (string.IsNullOrEmpty(_code) || !stringDatas.TryGetValue(_code, out StringData value))
            return $"[{_code}]";
        if (Application.systemLanguage != SystemLanguage.Korean && !string.IsNullOrEmpty(value.ENG))
            return value.ENG;
        return value.KOR ?? $"[{_code}]";
    }
}

[System.Serializable]
public class CookData
{
    public string CookID;
    public string NeedItem1ID;
    public int NeedItem1Amount;
    public string NeedItem2ID;
    public int NeedItem2Amount;
    public string NeedItem3ID;
    public int NeedItem3Amount;
    public string GetItemID;
    public int GetItemAmount;
    public float CookTime;
    public float GetTime;
}

[System.Serializable]
public class CutsceneData
{
    public string CutSceneID;
    public string OpenID;
    public int StringType;
    public string StringID;
    public float StringDelay;
    public float value1;
    public int ImageType;
    public string ImageResourceID;
    public float ImageDelay;
    public float value2;
    public int SoundType;
    public string SoundResourceID;
    public float SoundDelay;
    public float value3;
}


[System.Serializable]
public class BonfireData
{
    public string BonfireID;
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
public class CollectSpaceData
{
    public string CollectSpaceID;
    public string MiniGameID1;
    public string MiniGameHelpStringID1;
    public string MiniGameID2;
    public string MiniGameHelpStringID2;
    public string FunctionID;
}

[System.Serializable]
public class CollectData
{
    public string CollectID;
    public string CollectSpaceID1;
    public string StringID1;
    public string CollectSpaceID2;
    public string StringID2;
    public string CollectSpaceID3;
    public string StringID3;
    public int RanPos;
    public string CollectSpaceID4;
    public string StringID4;
    public float StartTime;
    public float EndTime;
    public float ContinueTime;
}

[System.Serializable]
public class DefineData
{
    public int Index;
    public string DefineID;
    public float value1;
}

[System.Serializable]
public class FenceData
{
    public string FenceID;
    public string FenceGroupID;
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
    public string HeroStateGroupID;
    public string HeroStateStringID;
    public float FirstGive;
    public float Min;
    public float Max;
}

[System.Serializable]
public class HeroData
{
    public string HeroID;
    public string HeroStringID;
    public string HeroStateGroupID;
}

[System.Serializable]
public class InvadeData
{
    public string InvadeID;
    public string InvadeStringID;
    public int StealObject;
    public string ItemID1;
    public int RandMin1;
    public int RandMax1;
    public string ItemID2;
    public int RandMin2;
    public int RandMax2;
    public string ItemID3;
    public int RandMin3;
    public int RandMax3;
    public string ItemID4;
    public int RandMin4;
    public int RandMax4;
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
    public int Item1Min;
    public int Item2Min;
    public string StageTitleStringID;
    public float MeatValue;
    public float WaterValue;
    public float HubValue;
    public float FoodValue;
    public float HStressValue;
    public float HPowerValue;
    public string JStateGroupID;
}

[System.Serializable]
public class JResultData
{
    public string JResultID;
    public string JResultGroupID;
    public string JRanStringID;
    public float Prob;
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
    public string JStateGroupID;
    public string JStateStringID;
    public float ValueMin;
    public string JResultGroupID;
}

[System.Serializable]
public class MiniGameData
{
    public string ObjectID;
    public string MinigameID;
    public string GetItemID1;
    public int GetAmount1;
    public string GetItemID2;
    public int GetAmount2;
    public int GetJemAmount;
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
    public string StageDayGroupID;
    public int Day;
    public string InvadeID;
    public string JourneyID;
}

[System.Serializable]
public class EventData
{
    public string EventID;
    public int opentype;
    public int EventType;
    public string H1ImageName;
    public string H2ImageName;
    public int ChoiceNum;
    public string EventStringID;
    public string Choice1ID;
    public string Choice2ID;
    public string Choice3ID;
}

[System.Serializable]
public class ChoiceData
{
    public string ChoiceID;
    public string ChoiceStringID;
    public string NeeditemID;
    public int NeedAmount;
    public float RewardProb;
    public int RewardType;
    public string SuccessRewardStringID;
    public string Hero1ID;
    public string H1Reward1Type;
    public int H1Reward1Amount;
    public string H1Reward2Type;
    public int H1Reward2Amount;
    public string Hero2ID;
    public string H2Reward1Type;
    public int H2Reward1Amount;
    public string H2Reward2Type;
    public int H2Reward2Amount;
    public string FailRewardStringID;
    public string FailHero1ID;
    public string FailH1Reward1Type;
    public int FailH1Reward1Amount;
    public string FailH1Reward2Type;
    public int FailH1Reward2Amount;
    public string FailHero2ID;
    public string FailH2Reward1Type;
    public int FailH2Reward1Amount;
    public string FailH2Reward2Type;
    public int FailH2Reward2Amount;
    public string AddEventID;
}



[System.Serializable]
public class StageData
{
    public string StageID;
    public string StageDayGroupID;
    public int Index;
    public string StageStringID;
    public string BonfireID;
    public string FenceGroupID;
    public string CollectID;
    public string FieldID;
}

[System.Serializable]
public class JemData
{
    public string JemID;
    public string JemImageID;
    public string JemNameString;
    public string JemExplainString;
    public int JemType;
    public int StartLevel;
    public int MaxLevel;
    public float Value1;
    public float Value2;
    public float LevelUpAmount;
    public string CutsceneOpenID;
}

[System.Serializable]
public class LaundryData
{
    public string LaundryID;
    public string StageID;
    public int NeedWaterAmount;
    public int L1_GermCount;
    public float L1_GermTime;
    public float L1_JenTimeMin;
    public float L1_JenTimeMax;
    public int L2_RinseCount;
    public float L2_DelayTime;
    public float L3_MinTime;
    public float L3_MaxTime;
    public float L3_TotalTime;
    public int Hero1_Stress;
    public int Hero2_Stress;
    public int Hero3_Stress;
    public int Hero4_Stress;
}

[System.Serializable]
public class TutorialData
{
    public string TutorialID;
    public string OpenID;
    public int TutorialType;
    public int Index;
    public int ImageType;
    public string ImageID;
    public string StringID;
}

[System.Serializable]
public class StringData
{
    public string StringID;
    public string KOR;
    public string ENG;
}

public class JsonUtilityHelper
{
    public static T[] FromJson<T>(string json)
    {
        if (string.IsNullOrWhiteSpace(json))
            throw new ArgumentException("JSON table is empty.", nameof(json));
        json = json.TrimStart('\uFEFF', ' ', '\t', '\r', '\n');
        if (json.Length == 0)
            throw new ArgumentException("JSON table is empty.", nameof(json));
        string newJson = "";
        if (json[0] == '{')
        {
            newJson = json;
        }
        else
        {
            newJson = "{ \"array\": " + json + "}";
        }
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(newJson);
        if (wrapper == null || wrapper.array == null)
            throw new FormatException("Expected a JSON array or an object containing an array field.");
        return wrapper.array;
    }

    public static string ToJson<T>(T[] array)
    {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.array = array;
        return JsonUtility.ToJson(wrapper);
    }

    [Serializable]
    private class Wrapper<T>
    {
        public T[] array;
    }
}

public class CSVConvert
{
    public static int ToInt(object _obj)
    {
        string body = _obj.ToString();
        //body = Regex.Replace(body, @"[^0-9]", "");
        return int.Parse(body, CultureInfo.InvariantCulture);
    }

    public static float ToFloat(object _obj)
    {
        string body = _obj.ToString();
        //body = Regex.Replace(body, @"[^0-9]", "");
        return float.Parse(body, CultureInfo.InvariantCulture);
    }

    public static string ToString(object _obj)
    {
        return _obj.ToString();
    }
}
