using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>Repeatable migration checks; no third-party testing dependency.</summary>
public static class MigrationValidation
{
    [MenuItem("Tools/Heros Manager/Save Unity 6 Scene and Rendering Upgrades")]
    public static void UpgradeAssets()
    {
        using var validation = ValidationSaveSession.Open("UpgradeAssets.");
        foreach (var entry in EditorBuildSettings.scenes.Where(scene => scene.enabled))
        {
            var scene = EditorSceneManager.OpenScene(entry.path, OpenSceneMode.Single);
            // Legacy auto-baking attempts to bake again during runtime scene loads.
            Lightmapping.bakeOnSceneLoad = Lightmapping.BakeOnSceneLoadMode.Never;
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
        }
        string[] pipelineAssets = AssetDatabase.FindAssets("t:RenderPipelineAsset")
            .Concat(AssetDatabase.FindAssets("t:ScriptableRendererData"))
            .Select(AssetDatabase.GUIDToAssetPath).ToArray();
        foreach (string path in pipelineAssets)
        {
            var asset = AssetDatabase.LoadMainAssetAtPath(path);
            EditorUtility.SetDirty(asset);
        }
        AssetDatabase.SaveAssets();
        AssetDatabase.ForceReserializeAssets(pipelineAssets);
        EditorSceneManager.OpenScene("Assets/01.Scenes/Title.unity");
        Debug.Log("UNITY 6 ASSET UPGRADE SAVED");
    }

    [MenuItem("Tools/Heros Manager/Validate Project")]
    public static void Validate()
    {
        using var validation = ValidationSaveSession.Open("ProjectValidation.");
        if (Application.unityVersion != "6000.4.7f1")
            throw new InvalidOperationException("Use Unity 6000.4.7f1 for this migration baseline.");
        var tablesObject = new GameObject("Validation tables");
        try
        {
            var tables = tablesObject.AddComponent<LoadGameData>();
            tables.LoadTitleDatas();
            Require(tables.stageDatas.Count > 0 && tables.itemDatas.Count == 5, "Core tables");
            foreach (var stage in tables.stageDatas.Values)
            {
                Require(tables.stageDayDatas.Values.Any(day => day.StageDayGroupID == stage.StageDayGroupID), stage.StageID);
                Require(tables.collectDatas.ContainsKey(stage.CollectID), stage.CollectID);
            }
            Require(JsonUtilityHelper.FromJson<ItemData>(" \n[{\"ItemID\":\"test\"}]").Length == 1, "JSON whitespace");
            Require(JsonUtilityHelper.FromJson<ItemData>("\uFEFF[]").Length == 0, "JSON BOM");
            Debug.Log($"TABLE VALIDATION PASSED: {tables.stageDatas.Count} stages, {tables.miniGameDatas.Count} mini-game rows.");
        }
        finally { UnityEngine.Object.DestroyImmediate(tablesObject); }

        int missing = 0;
        var setup = EditorSceneManager.GetSceneManagerSetup();
        try
        {
            foreach (var entry in EditorBuildSettings.scenes.Where(scene => scene.enabled))
            {
                var scene = EditorSceneManager.OpenScene(entry.path, OpenSceneMode.Single);
                foreach (GameObject root in scene.GetRootGameObjects())
                    foreach (Transform node in root.GetComponentsInChildren<Transform>(true))
                    {
                        int count = GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(node.gameObject);
                        if (count > 0)
                            Debug.LogError($"Missing scripts ({count}): {entry.path}/{node.name}");
                        missing += count;
                    }
            }
        }
        finally
        {
            if (setup.Any(scene => scene.isLoaded && scene.isActive))
                EditorSceneManager.RestoreSceneManagerSetup(setup);
            else
                EditorSceneManager.NewScene(NewSceneSetup.EmptyScene);
        }
        Require(missing == 0, $"Scene missing scripts: {missing}");
        Debug.Log("PROJECT VALIDATION PASSED");
    }

    [MenuItem("Tools/Heros Manager/Build Windows")]
    public static void BuildWindows()
    {
        using var validation = ValidationSaveSession.Open("BuildValidation.");
        string output = Path.GetFullPath("Builds/Windows/HerosManager.exe");
        Directory.CreateDirectory(Path.GetDirectoryName(output));
        var report = BuildPipeline.BuildPlayer(new BuildPlayerOptions
        {
            scenes = EditorBuildSettings.scenes.Where(scene => scene.enabled).Select(scene => scene.path).ToArray(),
            locationPathName = output,
            target = BuildTarget.StandaloneWindows64,
            options = BuildOptions.None
        });
        Require(report.summary.result == BuildResult.Succeeded,
            $"Build {report.summary.result}: {report.summary.totalErrors} errors");
        Debug.Log($"WINDOWS BUILD PASSED: {output}");
    }

    private static void Require(bool condition, string message)
    {
        if (!condition)
            throw new InvalidOperationException(message);
    }
}

[InitializeOnLoad]
public static class MigrationPlaySmoke
{
    private const string ActiveKey = "HerosManager.MigrationSmoke.Active";
    private const string StepKey = "HerosManager.MigrationSmoke.Step";
    private const string Prefix = "HerosManager.MigrationSmoke.";
    private static double nextStep;
    private static int errors;
    private static readonly string[] SmokeAreas =
    {
        "CS_S1_Forest", "CS_S1_Grassland", "CS_S1_Lake", "CS_S2_Ruins",
        "CS_S2_Desertfield", "CS_S2_Desertlake", "CS_S2_Oasis",
        "CS_S3_Magicspring", "CS_S3_Fairywarehouse", "CS_S3_Portal"
    };

    static MigrationPlaySmoke()
    {
        if (SessionState.GetBool(ActiveKey, false) || SessionState.GetBool(Prefix + "Exiting", false)) Attach();
    }

    static void Attach()
    {
        EditorApplication.update -= Update;
        EditorApplication.update += Update;
        Application.logMessageReceived -= OnLog;
        Application.logMessageReceived += OnLog;
        nextStep = EditorApplication.timeSinceStartup + 8;
    }

    public static void Run()
    {
        ValidationSaveSession.Begin(Prefix);
        Directory.CreateDirectory("Logs");
        File.WriteAllText("Logs/play-errors.txt", string.Empty);
        SessionState.SetBool(ActiveKey, true);
        SessionState.SetBool(Prefix + "Exiting", false);
        SessionState.SetInt(Prefix + "ExitCode", 1);
        SessionState.SetInt(StepKey, 0);
        errors = 0;
        Attach();
        EditorSceneManager.OpenScene("Assets/01.Scenes/Title.unity");
        EditorApplication.isPlaying = true;
    }

    private static void OnLog(string message, string stack, LogType type)
    {
        if (type == LogType.Exception || type == LogType.Error || type == LogType.Assert)
        {
            errors++;
            File.AppendAllText("Logs/play-errors.txt", message + "\n" + stack + "\n");
        }
    }

    private static void Update()
    {
        if (!ValidationSaveSession.IsOwner(Prefix))
        { EditorApplication.update -= Update; Application.logMessageReceived -= OnLog; return; }
        if (SessionState.GetBool(Prefix + "Exiting", false))
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode) return;
            ValidationSaveSession.CompleteRunner(Prefix);
            EditorApplication.update -= Update;
            Application.logMessageReceived -= OnLog;
            SessionState.SetBool(Prefix + "Exiting", false);
            if (Application.isBatchMode) EditorApplication.Exit(SessionState.GetInt(Prefix + "ExitCode", 1));
            return;
        }
        if (!EditorApplication.isPlaying || EditorApplication.timeSinceStartup < nextStep) return;
        try
        {
            int step = SessionState.GetInt(StepKey, 0);
            switch (step)
            {
                case 0:
                    if (SceneManager.GetActiveScene().name != "Title") throw new Exception("Title scene not loaded");
                    break;
                case 1:
                    SaveDataManager.Instance.NewGameData();
                    SceneManager.LoadScene("Intro");
                    break;
                case 2:
                    if (SceneManager.GetActiveScene().name != "Intro") throw new Exception("Intro scene not loaded");
                    break;
                case 3:
                    SceneManager.LoadScene("InGame");
                    break;
                case 4:
                    if (FirstRunGuide.IsCreateInstance()) FirstRunGuide.Instance.SkipPractice();
                    ValidateEvents();
                    break;
                case 5:
                    if (InGameMgr.Instance.stageData == null) throw new Exception("InGame stage missing");
                    GatheringManager.Instance.isFirst = false;
                    int before = ItemManager.Instance.GetItemInfo("Item_Wood").num;
                    MiniGameMgr.Instance.Setup("CS_S1_Forest");
                    MiniGameMgr.Instance.AddTempItem("Item_Wood", 7);
                    MiniGameMgr.Instance.CloseMiniGame();
                    MiniGameMgr.Instance.CloseMiniGame();
                    if (ItemManager.Instance.GetItemInfo("Item_Wood").num != before + 7)
                        throw new Exception("Mini-game rewards must be paid exactly once");
                    Time.timeScale = 0;
                    SceneManager.LoadScene("Title");
                    break;
                case 6:
                    if (Time.timeScale != 1) throw new Exception("Title did not restore time scale");
                    SaveDataManager.Instance.NewGameData();
                    SaveDataManager.Instance.saveGameData.hasEventSnapshot = true;
                    SaveDataManager.Instance.saveGameData.pendingEventIds = Array.Empty<string>();
                    SceneManager.LoadScene("InGame");
                    break;
                default:
                    if (InGameMgr.Instance.stageData == null) throw new Exception("InGame re-entry failed");
                    if (FirstRunGuide.IsCreateInstance()) FirstRunGuide.Instance.SkipPractice();
                    if (step == 7 && EventMgr.Instance.EventList.Count != 0)
                        throw new Exception("An exhausted saved event queue must not repopulate on scene entry");
                    MiniGameMgr.Instance.CloseMiniGame();
                    if (step - 7 < SmokeAreas.Length)
                    {
                        TutorialMgr.Instance.CloseTutorial();
                        GatheringManager.Instance.isFirst = false;
                        Time.timeScale = 1;
                        Clock.Instance.isStop = true;
                        MiniGameMgr.Instance.Setup(SmokeAreas[step - 7]);
                        if (InGameMgr.Instance.state != State.MiniGame)
                            throw new Exception("Minigame setup failed: " + SmokeAreas[step - 7]);
                        Debug.Log("PLAY AREA OPENED: " + SmokeAreas[step - 7]);
                        break;
                    }
                    ValidateStageTransitions();
                    Finish();
                    return;
            }
            SessionState.SetInt(StepKey, step + 1);
            nextStep = EditorApplication.timeSinceStartup + (step >= 7 ? 3 : 8);
        }
        catch (Exception exception)
        {
            Debug.LogException(exception);
            Finish();
        }
    }

    private static void ValidateEvents()
    {
        var manager = EventMgr.Instance;
        manager.isFirstEvent = false;
        manager.EventList.Clear();
        manager.ChoseEvent();
        manager.EventButtonActive();
        manager.EventButtonAction();
        if (manager.setedEventNum != 0) throw new Exception("Empty event pool must stay empty");

        var tables = LoadGameData.Instance;
        var row = tables.eventDatas.Values.First(data =>
            tables.choiceDatas.TryGetValue(data.Choice1ID, out var paid) && paid.NeedAmount > 0 &&
            tables.choiceDatas.TryGetValue(data.Choice2ID, out var free) && GameDataRules.IsAbsentReference(free.NeeditemID));
        var inventory = ItemManager.Instance;
        var balances = inventory.CaptureItems();
        foreach (var item in inventory.items) item.Setup(0);
        manager.EventList.Add(row.EventID);
        manager.ChoseEvent();
        manager.EventButtonAction();
        var panel = manager.EventPanels[0].GetComponent<EventPanel>();
        panel.ChoiceButtonAction(row.Choice1ID);
        if (panel.isClicked || manager.setedEventNum != 1) throw new Exception("Unaffordable event spent resources");
        manager.ClosePanel();
        var paidChoice = tables.choiceDatas[row.Choice1ID];
        var paidItem = inventory.GetItemInfo(paidChoice.NeeditemID);
        paidItem.Setup(paidChoice.NeedAmount);
        manager.EventButtonAction();
        if (!panel.OptionButtons[0].GetComponent<UnityEngine.UI.Button>().interactable || panel.isClicked)
            throw new Exception("Reopening an event must refresh affordability without resolving it");
        manager.ClosePanel();
        paidItem.Setup(0);
        manager.EventButtonAction();
        if (panel.OptionButtons[0].GetComponent<UnityEngine.UI.Button>().interactable)
            throw new Exception("Reopening an event must also disable a newly unaffordable choice");
        panel.ChoiceButtonAction(row.Choice2ID);
        panel.ChoiceButtonAction(row.Choice2ID);
        if (!panel.isClicked || manager.setedEventNum != 0 || inventory.CaptureItems().Any(value => value != 0))
            throw new Exception("Free event must resolve exactly once without spending");
        manager.ClosePanel();
        for (int i = 0; i < balances.Length; i++) inventory.items[i].Setup(balances[i]);
        int day = Clock.Instance.day;
        Clock.Instance.NextDay();
        if (Clock.Instance.day != day) throw new Exception("Day advance must reject duplicate callbacks");
    }

    private static void ValidateStageTransitions()
    {
        var game = InGameMgr.Instance;
        var stages = LoadGameData.Instance.Catalog.Stages;
        for (int i = 1; i < stages.Count; i++)
        {
            game.NextStage();
            if (game.stageData != stages[i]) throw new Exception("Stage transition selected wrong data");
            Clock.Instance.day = LoadGameData.Instance.Catalog.GetStageDays(game.stageData.StageDayGroupID)[0].Day;
            Clock.Instance.LoadStageDayData();
        }
        game.NextStage();
        game.NextStage();
        if (!game.IsComplete || !Clock.Instance.isStop) throw new Exception("Final stage must stop exactly once");
    }

    private static void Finish()
    {
        SessionState.SetBool(ActiveKey, false);
        SessionState.SetBool(Prefix + "Exiting", true);
        SessionState.SetInt(Prefix + "ExitCode", errors == 0 ? 0 : 1);
        Debug.Log($"PLAY SMOKE {(errors == 0 ? "PASSED" : "FAILED")}: {errors} errors");
        EditorApplication.isPlaying = false;
    }
}
