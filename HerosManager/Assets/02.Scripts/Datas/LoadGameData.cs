using System.Collections.Generic;
using UnityEngine;
using System.IO;
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

    public void LoadTitleDatas()
    {
        LoadEventData("/EventTable.Json");
        LoadChoiceData("/ChoiceTable.Json");
        LoadCookData("/CookTable.Json");
        LoadCutsceneData("/CutsceneTable.Json");
        LoadBonfireData("/BonfireTable.Json");
        LoadCollectSpaceData("/CollectSpaceTable.Json");
        LoadCollectData("/CollectTable.Json");
        LoadDefineData("/DefineTable.Json");
        LoadFenceData("/FenceTable.Json");
        LoadFieldData("/FieldTable.Json");
        LoadHeroStateData("/HeroStateTable.Json");
        LoadHeroData("/HeroTable.Json");
        LoadInvadeData("/InvadeTable.Json");
        LoadItemData("/ItemTable.Json");
        LoadJourneyData("/JourneyTable.Json");
        LoadJResultData("/JResultTable.Json");
        LoadJStateData("/JStateTable.Json");
        LoadMiniGameData("/MiniGameTable.Json");
        LoadStageDayData("/StageDayTable.Json");
        LoadStageData("/StageTable.Json");
        LoadStringData("/StringTable.Json");
        LoadJemData("/JemTable.Json");
        LoadLaundryData("/LaundryTable.Json");
    }

    void LoadCookData(string _path)
    {
        cookDatas.Clear();

        string path = Application.dataPath + _path;
        if (!File.Exists(path))
            return;

        string json = File.ReadAllText(path);

        var cookDataList = JsonUtilityHelper.FromJson<CookData>(json);
        foreach (var data in cookDataList)
        {
            cookDatas.Add(data.CookID, data);
        }

        //List<Dictionary<string, object>> data = CSVReader.Read(_path);
        //for(int i=0; i<data.Count; i++)
        //{
        //    CookData cookData = new CookData();
        //    cookData.CookID = CSVConvert.ToString(data[i]["CookID"]);
        //    cookData.NeedItem1ID = CSVConvert.ToString(data[i]["NeedItem1ID"]);
        //    cookData.NeedItem2ID = CSVConvert.ToString(data[i]["NeedItem2ID"]);
        //    cookData.NeedItem3ID = CSVConvert.ToString(data[i]["NeedItem3ID"]);
        //    cookData.NeedItem1Amount = CSVConvert.ToInt(data[i]["NeedItem1Amount"]);
        //    cookData.NeedItem2Amount = CSVConvert.ToInt(data[i]["NeedItem2Amount"]);
        //    cookData.NeedItem3Amount = CSVConvert.ToInt(data[i]["NeedItem3Amount"]);
        //    cookData.GetItemID = CSVConvert.ToString(data[i]["GetItemID"]);
        //    cookData.GetItemAmount = CSVConvert.ToInt(data[i]["GetItemAmount"]);
        //    cookData.CookTime = CSVConvert.ToFloat(data[i]["CookTime"]);
        //    cookData.GetTime = CSVConvert.ToFloat(data[i]["GetTime"]);

        //    cookDatas.Add(cookData.CookID, cookData);
        //}
    }

    void LoadCutsceneData(string _path)
    {
        cutsceneDatas.Clear();

        string path = Application.dataPath + _path;
        if (!File.Exists(path))
            return;

        string json = File.ReadAllText(path);

        var cutsceneDataList = JsonUtilityHelper.FromJson<CutsceneData>(json);
        foreach (var data in cutsceneDataList)
        {
            cutsceneDatas.Add(data.CutSceneID, data);
        }


        //List<Dictionary<string, object>> data = CSVReader.Read(_path);
        //for (int i = 0; i < data.Count; i++)
        //{
        //    CutsceneData cutsceneData = new CutsceneData();
        //    cutsceneData.CutSceneID = CSVConvert.ToString(data[i]["CutSceneID"]);
        //    cutsceneData.OpenID = CSVConvert.ToString(data[i]["OpenID"]);
        //    cutsceneData.StringType = CSVConvert.ToInt(data[i]["StringType"]);
        //    cutsceneData.StringID = CSVConvert.ToString(data[i]["StringID"]);
        //    cutsceneData.StringDelay = CSVConvert.ToFloat(data[i]["StringDelay"]);
        //    cutsceneData.value1 = CSVConvert.ToFloat(data[i]["value(1)"]);
        //    cutsceneData.ImageType = CSVConvert.ToInt(data[i]["ImageType"]);
        //    cutsceneData.ImageResourceID = CSVConvert.ToString(data[i]["ImageResourceID"]);
        //    cutsceneData.ImageDelay = CSVConvert.ToFloat(data[i]["ImageDelay"]);
        //    cutsceneData.value2 = CSVConvert.ToFloat(data[i]["value(2)"]);
        //    cutsceneData.SoundType = CSVConvert.ToInt(data[i]["SoundType"]);
        //    cutsceneData.SoundResourceID = CSVConvert.ToString(data[i]["SoundResourceID"]);
        //    cutsceneData.SoundDelay = CSVConvert.ToFloat(data[i]["SoundDelay"]);
        //    cutsceneData.value3 = CSVConvert.ToFloat(data[i]["value(3)"]);

        //    cutsceneDatas.Add(cutsceneData.CutSceneID, cutsceneData);
        //}
    }

    void LoadBonfireData(string _path)
    {
        bonfireDatas.Clear();

        string path = Application.dataPath + _path;
        if (!File.Exists(path))
            return;

        string json = File.ReadAllText(path);

        var bonfireDataList = JsonUtilityHelper.FromJson<BonfireData>(json);
        foreach (var data in bonfireDataList)
        {
            bonfireDatas.Add(data.BonfireID, data);
        }


        //List<Dictionary<string, object>> data = CSVReader.Read(_path);
        //for (int i = 0; i < data.Count; i++)
        //{
        //    BonfireData bonfireData = new BonfireData();
        //    bonfireData.BonfireID = CSVConvert.ToString(data[i]["BonfireID"]);
        //    bonfireData.NeedItemID = CSVConvert.ToString(data[i]["NeedItemID"]);
        //    bonfireData.NeedAmount = CSVConvert.ToInt(data[i]["NeedAmount"]);
        //    bonfireData.Time1 = CSVConvert.ToFloat(data[i]["Time1"]);
        //    bonfireData.Time2 = CSVConvert.ToFloat(data[i]["Time2"]);
        //    bonfireData.Time3 = CSVConvert.ToFloat(data[i]["Time3"]);
        //    bonfireData.Hero1_Stress = CSVConvert.ToFloat(data[i]["Hero1_Stress"]);
        //    bonfireData.Hero2_Stress = CSVConvert.ToFloat(data[i]["Hero2_Stress"]);
        //    bonfireData.Hero3_Stress = CSVConvert.ToFloat(data[i]["Hero3_Stress"]);
        //    bonfireData.Hero4_Stress = CSVConvert.ToFloat(data[i]["Hero4_Stress"]);

        //    bonfireDatas.Add(bonfireData.BonfireID, bonfireData);
        //}
    }

    void LoadCollectSpaceData(string _path)
    {
        collectSpaceDatas.Clear();

        string path = Application.dataPath + _path;
        if (!File.Exists(path))
            return;

        string json = File.ReadAllText(path);

        var collectSpaceDataList = JsonUtilityHelper.FromJson<CollectSpaceData>(json);
        foreach (var data in collectSpaceDataList)
        {
            collectSpaceDatas.Add(data.CollectSpaceID, data);
        }


        //List<Dictionary<string, object>> data = CSVReader.Read(_path);
        //for (int i = 0; i < data.Count; i++)
        //{
        //    CollectSpaceData collectSpaceData = new CollectSpaceData();
        //    collectSpaceData.CollectSpaceID = CSVConvert.ToString(data[i]["CollectSpaceID"]);
        //    collectSpaceData.MiniGameLeftID1 = CSVConvert.ToString(data[i]["MiniGameID1"]);
        //    collectSpaceData.MiniGameHelpStringID1 = CSVConvert.ToString(data[i]["MiniGameHelpStringID1"]);
        //    collectSpaceData.MiniGameRightID2 = CSVConvert.ToString(data[i]["MiniGameID2"]);
        //    collectSpaceData.MiniGameHelpStringID2 = CSVConvert.ToString(data[i]["MiniGameHelpStringID2"]);
        //    //collectSpaceData.MiniGameCenterID = CSVConvert.ToString(data[i]["MiniGameCenterID"]);
        //    //collectSpaceData.MiniGameHelpStringID3 = CSVConvert.ToString(data[i]["MiniGameHelpStringID3"]);
        //    collectSpaceData.FunctionID = CSVConvert.ToString(data[i]["FunctionID"]);

        //    collectSpaceDatas.Add(collectSpaceData.CollectSpaceID, collectSpaceData);
        //}
    }

    void LoadCollectData(string _path)
    {
        collectDatas.Clear();

        string path = Application.dataPath + _path;
        if (!File.Exists(path))
            return;

        string json = File.ReadAllText(path);

        var collectDataList = JsonUtilityHelper.FromJson<CollectData>(json);
        foreach (var data in collectDataList)
        {
            collectDatas.Add(data.CollectID, data);
        }


        //List<Dictionary<string, object>> data = CSVReader.Read(_path);
        //for (int i = 0; i < data.Count; i++)
        //{
        //    CollectData collectData = new CollectData();
        //    collectData.CollectID = CSVConvert.ToString(data[i]["CollectID"]);
        //    collectData.CollectSpaceID1 = CSVConvert.ToString(data[i]["CollectSpaceID1"]);
        //    collectData.StringID1 = CSVConvert.ToString(data[i]["StringID1"]);
        //    collectData.CollectSpaceID2 = CSVConvert.ToString(data[i]["CollectSpaceID2"]);
        //    collectData.StringID2 = CSVConvert.ToString(data[i]["StringID2"]);
        //    collectData.CollectSpaceID3 = CSVConvert.ToString(data[i]["CollectSpaceID3"]);
        //    collectData.StringID3 = CSVConvert.ToString(data[i]["StringID3"]);
        //    collectData.RanPos = CSVConvert.ToInt(data[i]["RanPos"]);
        //    collectData.CollectSpaceID4 = CSVConvert.ToString(data[i]["CollectSpaceID4"]);
        //    collectData.StringID4 = CSVConvert.ToString(data[i]["StringID4"]);
        //    collectData.StartTime = CSVConvert.ToFloat(data[i]["StartTime"]);
        //    collectData.EndTime = CSVConvert.ToFloat(data[i]["EndTime"]);
        //    collectData.ContinueTime = CSVConvert.ToFloat(data[i]["ContinueTime"]);

        //    collectDatas.Add(collectData.CollectID, collectData);
        //}
    }

    void LoadDefineData(string _path)
    {
        defineDatas.Clear();

        string path = Application.dataPath + _path;
        if (!File.Exists(path))
            return;

        string json = File.ReadAllText(path);

        var defineDataList = JsonUtilityHelper.FromJson<DefineData>(json);
        foreach (var data in defineDataList)
        {
            defineDatas.Add(data.DefineID, data);
        }


        //List<Dictionary<string, object>> data = CSVReader.Read(_path);
        //for (int i = 0; i < data.Count; i++)
        //{
        //    DefineData defineData = new DefineData();
        //    defineData.Index = CSVConvert.ToInt(data[i]["Index"]);
        //    defineData.DefineID = CSVConvert.ToString(data[i]["DefineID"]);
        //    defineData.value = CSVConvert.ToFloat(data[i]["value"]);

        //    defineDatas.Add(defineData.DefineID, defineData);
        //}
    }

    void LoadFenceData(string _path)
    {
        fenceDatas.Clear();

        string path = Application.dataPath + _path;
        if (!File.Exists(path))
            return;

        string json = File.ReadAllText(path);

        var fenceDataList = JsonUtilityHelper.FromJson<FenceData>(json);
        foreach (var data in fenceDataList)
        {
            fenceDatas.Add(data.FenceID, data);
        }


        //List<Dictionary<string, object>> data = CSVReader.Read(_path);
        //for (int i = 0; i < data.Count; i++)
        //{
        //    FenceData fenceData = new FenceData();
        //    fenceData.FenceID = CSVConvert.ToString(data[i]["FenceID"]);
        //    fenceData.FenceGroupID = CSVConvert.ToString(data[i]["FenceGroupID"]);
        //    fenceData.FenceLevel = CSVConvert.ToInt(data[i]["FenceLevel"]);
        //    fenceData.InvadeProb = CSVConvert.ToFloat(data[i]["InvadeProb"]);
        //    fenceData.NeedItemID = CSVConvert.ToString(data[i]["NeedItemID"]);
        //    fenceData.Amount = CSVConvert.ToInt(data[i]["Amount"]);
        //    fenceData.NeedTime = CSVConvert.ToFloat(data[i]["NeedTime"]);

        //    fenceDatas.Add(fenceData.FenceID, fenceData);
        //}
    }

    void LoadFieldData(string _path)
    {
        fieldDatas.Clear();

        string path = Application.dataPath + _path;
        if (!File.Exists(path))
            return;

        string json = File.ReadAllText(path);

        var fieldDataList = JsonUtilityHelper.FromJson<FieldData>(json);
        foreach (var data in fieldDataList)
        {
            fieldDatas.Add(data.FieldID, data);
        }


        //List<Dictionary<string, object>> data = CSVReader.Read(_path);
        //for (int i = 0; i < data.Count; i++)
        //{
        //    FieldData fieldData = new FieldData();
        //    fieldData.FieldID = CSVConvert.ToString(data[i]["FieldID"]);
        //    fieldData.NeedItemID = CSVConvert.ToString(data[i]["NeedItemID"]);
        //    fieldData.NeedAmount = CSVConvert.ToInt(data[i]["NeedAmount"]);
        //    fieldData.Time1 = CSVConvert.ToFloat(data[i]["Time1"]);
        //    fieldData.Time2 = CSVConvert.ToFloat(data[i]["Time2"]);
        //    fieldData.Repeat = CSVConvert.ToInt(data[i]["Repeat"]);
        //    fieldData.GetItemID = CSVConvert.ToString(data[i]["GetItemID"]);
        //    fieldData.GetAmount = CSVConvert.ToInt(data[i]["GetAmount"]);

        //    fieldDatas.Add(fieldData.FieldID, fieldData);
        //}
    }

    void LoadHeroStateData(string _path)
    {
        heroStateDatas.Clear();

        string path = Application.dataPath + _path;
        if (!File.Exists(path))
            return;

        string json = File.ReadAllText(path);

        var heroStateDataList = JsonUtilityHelper.FromJson<HeroStateData>(json);
        foreach (var data in heroStateDataList)
        {
            heroStateDatas.Add(data.HeroStateID, data);
        }


        //List<Dictionary<string, object>> data = CSVReader.Read(_path);
        //for (int i = 0; i < data.Count; i++)
        //{
        //    HeroStateData heroStateData = new HeroStateData();
        //    heroStateData.HeroStateID = CSVConvert.ToString(data[i]["HeroStateID"]);
        //    heroStateData.HeroStateGroupID = CSVConvert.ToString(data[i]["HeroStateGroupID"]);
        //    heroStateData.HeroStateStringID = CSVConvert.ToString(data[i]["HeroStateStringID"]);
        //    heroStateData.FirstGive = CSVConvert.ToFloat(data[i]["FirstGive"]);
        //    heroStateData.Min = CSVConvert.ToFloat(data[i]["Min"]);
        //    heroStateData.Max = CSVConvert.ToFloat(data[i]["Max"]);

        //    heroStateDatas.Add(heroStateData.HeroStateID, heroStateData);
        //}
    }

    void LoadHeroData(string _path)
    {
        heroDatas.Clear();

        string path = Application.dataPath + _path;
        if (!File.Exists(path))
            return;

        string json = File.ReadAllText(path);

        var heroDataList = JsonUtilityHelper.FromJson<HeroData>(json);
        foreach (var data in heroDataList)
        {
            heroDatas.Add(data.HeroID, data);
        }


        //List<Dictionary<string, object>> data = CSVReader.Read(_path);
        //for (int i = 0; i < data.Count; i++)
        //{
        //    HeroData heroData = new HeroData();
        //    heroData.HeroID = CSVConvert.ToString(data[i]["HeroID"]);
        //    heroData.HeroStringID = CSVConvert.ToString(data[i]["HeroStringID"]);
        //    heroData.HeroStateGroupID = CSVConvert.ToString(data[i]["HeroStateGroupID"]);

        //    heroDatas.Add(heroData.HeroID, heroData);
        //}
    }

    void LoadInvadeData(string _path)
    {
        invadeDatas.Clear();

        string path = Application.dataPath + _path;
        if (!File.Exists(path))
            return;

        string json = File.ReadAllText(path);

        var invadeDataList = JsonUtilityHelper.FromJson<InvadeData>(json);
        foreach (var data in invadeDataList)
        {
            invadeDatas.Add(data.InvadeID, data);
        }

        //List<Dictionary<string, object>> data = CSVReader.Read(_path);
        //for (int i = 0; i < data.Count; i++)
        //{
        //    InvadeData invadeData = new InvadeData();
        //    invadeData.InvadeID = CSVConvert.ToString(data[i]["InvadeID"]);
        //    invadeData.StealObject = CSVConvert.ToInt(data[i]["StealObject"]);
        //    invadeData.ItemID1 = CSVConvert.ToString(data[i]["ItemID1"]);
        //    invadeData.RandMin1 = CSVConvert.ToInt(data[i]["RandMin1"]);
        //    invadeData.RandMax1 = CSVConvert.ToInt(data[i]["RandMax1"]);
        //    invadeData.ItemID2 = CSVConvert.ToString(data[i]["ItemID2"]);
        //    invadeData.RandMin2 = CSVConvert.ToInt(data[i]["RandMin2"]);
        //    invadeData.RandMax2 = CSVConvert.ToInt(data[i]["RandMax2"]);
        //    invadeData.ItemID3 = CSVConvert.ToString(data[i]["ItemID3"]);
        //    invadeData.RandMin3 = CSVConvert.ToInt(data[i]["RandMin3"]);
        //    invadeData.RandMax3 = CSVConvert.ToInt(data[i]["RandMax3"]);
        //    invadeData.ItemID4 = CSVConvert.ToString(data[i]["ItemID4"]);
        //    invadeData.RandMin4 = CSVConvert.ToInt(data[i]["RandMin4"]);
        //    invadeData.RandMax4 = CSVConvert.ToInt(data[i]["RandMax4"]);

        //    invadeDatas.Add(invadeData.InvadeID, invadeData);
        //}
    }

    void LoadItemData(string _path)
    {
        itemDatas.Clear();

        string path = Application.dataPath + _path;
        if (!File.Exists(path))
            return;

        string json = File.ReadAllText(path);

        var itemDataList = JsonUtilityHelper.FromJson<ItemData>(json);
        foreach (var data in itemDataList)
        {
            itemDatas.Add(data.ItemID, data);
        }


        //List<Dictionary<string, object>> data = CSVReader.Read(_path);
        //for (int i = 0; i < data.Count; i++)
        //{
        //    ItemData itemData = new ItemData();
        //    itemData.ItemID = CSVConvert.ToString(data[i]["ItemID"]);
        //    itemData.ItemStringID = CSVConvert.ToString(data[i]["ItemStringID"]);
        //    itemData.FirstGive = CSVConvert.ToInt(data[i]["FirstGive"]);

        //    itemDatas.Add(itemData.ItemID, itemData);
        //}
    }

    void LoadJourneyData(string _path)
    {
        journeyDatas.Clear();

        string path = Application.dataPath + _path;
        if (!File.Exists(path))
            return;

        string json = File.ReadAllText(path);

        var journeyDataList = JsonUtilityHelper.FromJson<JourneyData>(json);
        foreach (var data in journeyDataList)
        {
            journeyDatas.Add(data.JourneyID, data);
        }


        //List<Dictionary<string, object>> data = CSVReader.Read(_path);
        //for (int i = 0; i < data.Count; i++)
        //{
        //    JourneyData journeyData = new JourneyData();
        //    journeyData.JourneyID = CSVConvert.ToString(data[i]["JourneyID"]);
        //    journeyData.Item1Min = CSVConvert.ToInt(data[i]["Item1Min"]);
        //    journeyData.Item2Min = CSVConvert.ToInt(data[i]["Item2Min"]);
        //    journeyData.StageTitleStringID = CSVConvert.ToString(data[i]["StageTitleStringID"]);
        //    journeyData.MeatValue = CSVConvert.ToFloat(data[i]["MeatValue"]);
        //    journeyData.WaterValue = CSVConvert.ToFloat(data[i]["WaterValue"]);
        //    journeyData.HubValue = CSVConvert.ToFloat(data[i]["HubValue"]);
        //    journeyData.FoodValue = CSVConvert.ToFloat(data[i]["FoodValue"]);
        //    journeyData.HStressValue = CSVConvert.ToFloat(data[i]["HStressValue"]);
        //    journeyData.HPowerValue = CSVConvert.ToFloat(data[i]["HPowerValue"]);
        //    journeyData.JStateGroupID = CSVConvert.ToString(data[i]["JStateGroupID"]);

        //    journeyDatas.Add(journeyData.JourneyID, journeyData);
        //}
    }

    void LoadJResultData(string _path)
    {
        jResultDatas.Clear();

        string path = Application.dataPath + _path;
        if (!File.Exists(path))
            return;

        string json = File.ReadAllText(path);

        var jResultDataList = JsonUtilityHelper.FromJson<JResultData>(json);
        foreach (var data in jResultDataList)
        {
            jResultDatas.Add(data.JResultID, data);
        }


        //List<Dictionary<string, object>> data = CSVReader.Read(_path);
        //for (int i = 0; i < data.Count; i++)
        //{
        //    JResultData jResultData = new JResultData();
        //    jResultData.JResultID = CSVConvert.ToString(data[i]["JResultID"]);
        //    jResultData.JResultGroupID = CSVConvert.ToString(data[i]["JResultGroupID"]);
        //    jResultData.JRanStringID = CSVConvert.ToString(data[i]["JRanStringID"]);
        //    jResultData.Prob = CSVConvert.ToFloat(data[i]["Prob"]);
        //    jResultData.Hero1_Stress = CSVConvert.ToFloat(data[i]["Hero1_Stress"]);
        //    jResultData.Hero1_Power = CSVConvert.ToFloat(data[i]["Hero1_Power"]);
        //    jResultData.Hero1_Hp = CSVConvert.ToFloat(data[i]["Hero1_Hp"]);
        //    jResultData.Hero1_Exp = CSVConvert.ToFloat(data[i]["Hero1_Exp"]);
        //    jResultData.Hero2_Stress = CSVConvert.ToFloat(data[i]["Hero2_Stress"]);
        //    jResultData.Hero2_Power = CSVConvert.ToFloat(data[i]["Hero2_Power"]);
        //    jResultData.Hero2_Hp = CSVConvert.ToFloat(data[i]["Hero2_Hp"]);
        //    jResultData.Hero2_Exp = CSVConvert.ToFloat(data[i]["Hero2_Exp"]);
        //    jResultData.Hero3_Stress = CSVConvert.ToFloat(data[i]["Hero3_Stress"]);
        //    jResultData.Hero3_Power = CSVConvert.ToFloat(data[i]["Hero3_Power"]);
        //    jResultData.Hero3_Hp = CSVConvert.ToFloat(data[i]["Hero3_Hp"]);
        //    jResultData.Hero3_Exp = CSVConvert.ToFloat(data[i]["Hero3_Exp"]);
        //    jResultData.Hero4_Stress = CSVConvert.ToFloat(data[i]["Hero4_Stress"]);
        //    jResultData.Hero4_Power = CSVConvert.ToFloat(data[i]["Hero4_Power"]);
        //    jResultData.Hero4_Hp = CSVConvert.ToFloat(data[i]["Hero4_Hp"]);
        //    jResultData.Hero4_Exp = CSVConvert.ToFloat(data[i]["Hero4_Exp"]);

        //    jResultDatas.Add(jResultData.JResultID, jResultData);
        //}
    }

    void LoadJStateData(string _path)
    {
        jStateDatas.Clear();

        string path = Application.dataPath + _path;
        if (!File.Exists(path))
            return;

        string json = File.ReadAllText(path);

        var jStateDataList = JsonUtilityHelper.FromJson<JStateData>(json);
        foreach (var data in jStateDataList)
        {
            jStateDatas.Add(data.JStateID, data);
        }


        //List<Dictionary<string, object>> data = CSVReader.Read(_path);
        //for (int i = 0; i < data.Count; i++)
        //{
        //    JStateData jStateData = new JStateData();
        //    jStateData.JStateID = CSVConvert.ToString(data[i]["JStateID"]);
        //    jStateData.JStateGroupID = CSVConvert.ToString(data[i]["JStateGroupID"]);
        //    jStateData.JStateStringID = CSVConvert.ToString(data[i]["JStateStringID"]);
        //    jStateData.ValueMin = CSVConvert.ToFloat(data[i]["ValueMin"]);
        //    jStateData.JResultGroupID = CSVConvert.ToString(data[i]["JResultGroupID"]);

        //    jStateDatas.Add(jStateData.JStateID, jStateData);
        //}
    }

    void LoadMiniGameData(string _path)
    {
        miniGameDatas.Clear();

        string path = Application.dataPath + _path;
        if (!File.Exists(path))
            return;

        string json = File.ReadAllText(path);

        var miniGameDataList = JsonUtilityHelper.FromJson<MiniGameData>(json);
        foreach (var data in miniGameDataList)
        {
            miniGameDatas.Add(data.ObjectID, data);
        }


        //List<Dictionary<string, object>> data = CSVReader.Read(_path);
        //for (int i = 0; i < data.Count; i++)
        //{
        //    MiniGameData miniGameData = new MiniGameData();
        //    miniGameData.ObjectID = CSVConvert.ToString(data[i]["ObjectID"]);
        //    miniGameData.MinigameID = CSVConvert.ToString(data[i]["MinigameID"]);
        //    miniGameData.GetItemID1 = CSVConvert.ToString(data[i]["GetItemID1"]);
        //    miniGameData.GetAmount1 = CSVConvert.ToInt(data[i]["GetAmount1"]);
        //    miniGameData.GetItemID2 = CSVConvert.ToString(data[i]["GetItemID2"]);
        //    miniGameData.GetAmount2 = CSVConvert.ToInt(data[i]["GetAmount2"]);
        //    miniGameData.GetJemAmount = CSVConvert.ToInt(data[i]["GetJemAmount"]);
        //    miniGameData.Probability = CSVConvert.ToFloat(data[i]["Probability"]);
        //    miniGameData.Stun = CSVConvert.ToFloat(data[i]["Stun"]);
        //    miniGameData.Invincibility = CSVConvert.ToFloat(data[i]["Invincibility"]);
        //    miniGameData.Velocity = CSVConvert.ToFloat(data[i]["Velocity"]);
        //    miniGameData.CoolTime = CSVConvert.ToFloat(data[i]["CoolTime"]);
        //    miniGameData.GameEnvironment = CSVConvert.ToInt(data[i]["GameEnvironment"]);
        //    miniGameData.value1 = CSVConvert.ToFloat(data[i]["value1"]);
        //    miniGameData.value2 = CSVConvert.ToFloat(data[i]["value2"]);
        //    miniGameData.value3 = CSVConvert.ToFloat(data[i]["value3"]);
        //    miniGameData.value4 = CSVConvert.ToFloat(data[i]["value4"]);
        //    miniGameData.value5 = CSVConvert.ToFloat(data[i]["value5"]);
        //    miniGameData.value6 = CSVConvert.ToFloat(data[i]["value6"]);

        //    miniGameDatas.Add(miniGameData.ObjectID, miniGameData);
        //}
    }

    void LoadStageDayData(string _path)
    {
        stageDayDatas.Clear();

        string path = Application.dataPath + _path;
        if (!File.Exists(path))
            return;

        string json = File.ReadAllText(path);

        var stageDayDataList = JsonUtilityHelper.FromJson<StageDayData>(json);
        foreach (var data in stageDayDataList)
        {
            stageDayDatas.Add(data.StageDayID, data);
        }


        //List<Dictionary<string, object>> data = CSVReader.Read(_path);
        //for (int i = 0; i < data.Count; i++)
        //{
        //    StageDayData stageDayData = new StageDayData();
        //    stageDayData.StageDayID = CSVConvert.ToString(data[i]["StageDayID"]);
        //    stageDayData.Day = CSVConvert.ToInt(data[i]["Day"]);
        //    stageDayData.StageDayGroupID = CSVConvert.ToString(data[i]["StageDayGroupID"]);
        //    stageDayData.InvadeID = CSVConvert.ToString(data[i]["InvadeID"]);
        //    stageDayData.JourneyID = CSVConvert.ToString(data[i]["JourneyID"]);

        //    stageDayDatas.Add(stageDayData.StageDayID, stageDayData);
        //}
    }

    void LoadStageData(string _path)
    {
        stageDatas.Clear();

        string path = Application.dataPath + _path;
        if (!File.Exists(path))
            return;

        string json = File.ReadAllText(path);

        var stageDataList = JsonUtilityHelper.FromJson<StageData>(json);
        foreach (var data in stageDataList)
        {
            stageDatas.Add(data.StageID, data);
        }


        //List<Dictionary<string, object>> data = CSVReader.Read(_path);
        //for (int i = 0; i < data.Count; i++)
        //{
        //    StageData stageData = new StageData();
        //    stageData.StageID = CSVConvert.ToString(data[i]["StageID"]);
        //    stageData.Index = CSVConvert.ToInt(data[i]["Index"]);
        //    stageData.StageDayGroupID = CSVConvert.ToString(data[i]["StageDayGroupID"]);
        //    stageData.StageStringID = CSVConvert.ToString(data[i]["StageStringID"]);
        //    stageData.BonfireID = CSVConvert.ToString(data[i]["BonfireID"]);
        //    stageData.FenceGroupID = CSVConvert.ToString(data[i]["FenceGroupID"]);
        //    stageData.CollectID = CSVConvert.ToString(data[i]["CollectID"]);
        //    stageData.FieldID = CSVConvert.ToString(data[i]["FieldID"]);

        //    stageDatas.Add(stageData.StageID, stageData);
        //}
    }

    void LoadChoiceData(string _path)
    {
        choiceDatas.Clear();

        string path = Application.dataPath + _path;
        if (!File.Exists(path))
            return;

        string json = File.ReadAllText(path);

        var choiceDataList = JsonUtilityHelper.FromJson<ChoiceData>(json);
        foreach (var data in choiceDataList)
        {
            choiceDatas.Add(data.ChoiceID, data);
        }


        //List<Dictionary<string, object>> data = CSVReader.Read(_path);
        //for(int i=0;i<data.Count;i++)
        //{
        //    ChoiceData choiceData = new ChoiceData();

        //    choiceData.ChoiceID = CSVConvert.ToString(data[i]["ChoiceID"]);
        //    choiceData.ChoiceStringID = CSVConvert.ToString(data[i]["ChoiceStringID"]);
        //    choiceData.NeedItemID = CSVConvert.ToString(data[i]["NeeditemID"]);
        //    choiceData.NeedAmount = CSVConvert.ToInt(data[i]["NeedAmount"]);
        //    choiceData.RewardProb = CSVConvert.ToFloat(data[i]["RewardProb"]);
        //    choiceData.RewardType = CSVConvert.ToInt(data[i]["RewardType"]);
        //    choiceData.SuccessRewardStringID = CSVConvert.ToString(data[i]["SuccessRewardStringID"]);
        //    choiceData.Hero1ID = CSVConvert.ToString(data[i]["Hero1ID"]);
        //    choiceData.H1Reward1Type = CSVConvert.ToString(data[i]["H1Reward1Type"]);
        //    choiceData.H1Reward1Amount = CSVConvert.ToInt(data[i]["H1Reward1Amount"]);
        //    choiceData.H1Reward2Type = CSVConvert.ToString(data[i]["H1Reward2Type"]);
        //    choiceData.H1Reward2Amount = CSVConvert.ToInt(data[i]["H1Reward2Amount"]);
        //    choiceData.Hero2ID = CSVConvert.ToString(data[i]["Hero2ID"]);
        //    choiceData.H2Reward1Type = CSVConvert.ToString(data[i]["H2Reward1Type"]);
        //    choiceData.H2Reward1Amount = CSVConvert.ToInt(data[i]["H2Reward1Amount"]);
        //    choiceData.H2Reward2Type = CSVConvert.ToString(data[i]["H2Reward2Type"]);
        //    choiceData.H2Reward2Amount = CSVConvert.ToInt(data[i]["H2Reward2Amount"]);
        //    choiceData.FailRewardStringID = CSVConvert.ToString(data[i]["FailRewardStringID"]);
        //    choiceData.FailHero1ID = CSVConvert.ToString(data[i]["FailHero1ID"]);
        //    choiceData.FailH1Reward1Type = CSVConvert.ToString(data[i]["FailH1Reward1Type"]);
        //    choiceData.FailH1Reward1Amount = CSVConvert.ToInt(data[i]["FailH1Reward1Amount"]);
        //    choiceData.FailH1Reward2Type = CSVConvert.ToString(data[i]["FailH1Reward2Type"]);
        //    choiceData.FailH1Reward2Amount = CSVConvert.ToInt(data[i]["FailH1Reward2Amount"]);
        //    choiceData.FailHero2ID = CSVConvert.ToString(data[i]["FailHero2ID"]);
        //    choiceData.FailH2Reward1Type = CSVConvert.ToString(data[i]["FailH2Reward1Type"]);
        //    choiceData.FailH2Reward1Amount = CSVConvert.ToInt(data[i]["FailH2Reward1Amount"]);
        //    choiceData.FailH2Reward2Type = CSVConvert.ToString(data[i]["FailH2Reward2Type"]);
        //    choiceData.FailH2Reward2Amount = CSVConvert.ToInt(data[i]["FailH2Reward2Amount"]);
        //    choiceData.AddEventID = CSVConvert.ToString(data[i]["AddEventID"]);

        //    choiceDatas.Add(choiceData.ChoiceID, choiceData);
        //}
    }

    void LoadJemData(string _path)
    {
        jemDatas.Clear();

        string path = Application.dataPath + _path;
        if (!File.Exists(path))
            return;

        string json = File.ReadAllText(path);

        var jemDataList = JsonUtilityHelper.FromJson<JemData>(json);
        foreach (var data in jemDataList)
        {
            jemDatas.Add(data.JemID, data);
        }


        //List<Dictionary<string,object>> data = CSVReader.Read(_path);
        //for(int i=0; i<data.Count;i++)
        //{
        //    JemData jemData = new JemData();
        //    jemData.JemID = CSVConvert.ToString(data[i]["JemID"]);
        //    jemData.JemImageID = CSVConvert.ToString(data[i]["JemImageID"]);
        //    jemData.JemNameString = CSVConvert.ToString(data[i]["JemNameString"]);
        //    jemData.JemExplainString = CSVConvert.ToString(data[i]["JemExplainString"]);
        //    jemData.JemType = CSVConvert.ToInt(data[i]["JemType"]);
        //    jemData.StartLevel = CSVConvert.ToInt(data[i]["StartLevel"]);
        //    jemData.MaxLevel = CSVConvert.ToInt(data[i]["MaxLevel"]);
        //    jemData.Value1 = CSVConvert.ToFloat(data[i]["Value(1)"]);
        //    jemData.Value2 = CSVConvert.ToFloat(data[i]["Value(2)"]);
        //    jemData.LevelUpAmount = CSVConvert.ToFloat(data[i]["LevelUpAmount"]);
        //    jemData.CutsceneOpenID = CSVConvert.ToString(data[i]["CutsceneOpenID"]);

        //    jemDatas.Add(jemData.JemID, jemData);
        //}
    }

    void LoadLaundryData(string _path)
    {
        laundryDatas.Clear();

        string path = Application.dataPath + _path;
        if (!File.Exists(path))
            return;

        string json = File.ReadAllText(path);

        var laundryDataList = JsonUtilityHelper.FromJson<LaundryData>(json);
        foreach (var data in laundryDataList)
        {
            laundryDatas.Add(data.LaundryID, data);
        }
    }

    void LoadEventData(string _path)
    {
        eventDatas.Clear();

        string path = Application.dataPath + _path;
        if (!File.Exists(path))
            return;

        string json = File.ReadAllText(path);

        var eventDataList = JsonUtilityHelper.FromJson<EventData>(json);
        foreach (var data in eventDataList)
        {
            eventDatas.Add(data.EventID, data);
        }


        //List<Dictionary<string, object>> data = CSVReader.Read(_path);
        //for(int i = 0; i < data.Count; i++)
        //{
        //    EventData eventData = new EventData();
        //    eventData.EventID = CSVConvert.ToString(data[i]["EventID"]);
        //    eventData.OpenType = CSVConvert.ToInt(data[i]["opentype"]);
        //    eventData.EventType = CSVConvert.ToInt(data[i]["EventType"]);
        //    eventData.H1ImgName = CSVConvert.ToString(data[i]["H1ImageName"]);
        //    eventData.H2ImgName = CSVConvert.ToString(data[i]["H2ImageName"]);
        //    eventData.ChoiceNum = CSVConvert.ToInt(data[i]["ChoiceNum"]);
        //    eventData.EventStringID = CSVConvert.ToString(data[i]["EventStringID"]);
        //    eventData.Choice1ID = CSVConvert.ToString(data[i]["Choice1ID"]);
        //    eventData.Choice2ID = CSVConvert.ToString(data[i]["Choice2ID"]);
        //    eventData.Choice3ID = CSVConvert.ToString(data[i]["Choice3ID"]);

        //    eventDatas.Add(eventData.EventID, eventData);
        //}
    }

    void LoadStringData(string _path)
    {
        stringDatas.Clear();

        string path = Application.dataPath + _path;
        if (!File.Exists(path))
            return;

        string json = File.ReadAllText(path);

        var stringDataList = JsonUtilityHelper.FromJson<StringData>(json);
        foreach (var data in stringDataList)
        {
            stringDatas.Add(data.StringID, data);
        }


        //List<Dictionary<string, object>> data = CSVReader.Read(_path);
        //for (int i = 0; i < data.Count; i++)
        //{
        //    StringData stringData = new StringData();
        //    stringData.StringID = CSVConvert.ToString(data[i]["StringID"]);
        //    stringData.KOR = CSVConvert.ToString(data[i]["KOR"]);
        //    stringData.ENG = CSVConvert.ToString(data[i]["ENG"]);

        //    stringDatas.Add(stringData.StringID, stringData);
        //}
    }

    public string GetString(string _code)
    {
        if (!stringDatas.ContainsKey(_code))
            return "String Code is Null";

        string rtn;

        if (Application.systemLanguage == SystemLanguage.Korean)
        {
            rtn = stringDatas[_code].KOR;
        }
        else
        {
            rtn = stringDatas[_code].ENG;
        }

        return rtn;
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