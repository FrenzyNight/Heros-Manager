using System.Collections.Generic;
using UnityEngine;
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
    public GameDataCatalog Catalog { get; private set; }
    public IReadOnlyList<GameDataIssue> ValidationIssues { get; private set; } = Array.Empty<GameDataIssue>();

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
        Catalog = new GameDataCatalog(stageDatas.Values, stageDayDatas.Values, fenceDatas.Values,
            heroStateDatas.Values, cutsceneDatas.Values, tutorialDatas.Values, miniGameDatas.Values,
            jStateDatas.Values, jResultDatas.Values);
        ValidationIssues = GameDataRules.Validate(this, Catalog);
        foreach (GameDataIssue issue in ValidationIssues)
            if (issue.IsError)
                throw new InvalidOperationException("Invalid gameplay tables: " + issue.Message);
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
            if (row == null)
                throw new InvalidOperationException($"Null row in {path}");
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
        return GetString(_code, Application.systemLanguage);
    }

    public string GetString(string _code, SystemLanguage language)
    {
        if (string.IsNullOrEmpty(_code) || !stringDatas.TryGetValue(_code, out StringData value))
            return $"[{_code}]";
        if (language != SystemLanguage.Korean && !GameDataRules.IsAbsentReference(value.ENG))
            return value.ENG;
        return value.KOR ?? $"[{_code}]";
    }
}
