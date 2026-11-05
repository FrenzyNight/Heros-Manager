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
    private static double nextStep;
    private static int errors;

    static MigrationPlaySmoke()
    {
        if (!SessionState.GetBool(ActiveKey, false)) return;
        EditorApplication.update += Update;
        Application.logMessageReceived += OnLog;
        nextStep = EditorApplication.timeSinceStartup + 8;
    }

    public static void Run()
    {
        File.WriteAllText("Logs/play-errors.txt", string.Empty);
        SessionState.SetBool(ActiveKey, true);
        SessionState.SetInt(StepKey, 0);
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
        if (!EditorApplication.isPlaying || EditorApplication.timeSinceStartup < nextStep) return;
        try
        {
            int step = SessionState.GetInt(StepKey, 0);
            switch (step)
            {
                case 0:
                    ScreenCapture.CaptureScreenshot(Path.GetFullPath("Logs/smoke-title.png"));
                    break;
                case 1:
                    SaveDataManager.Instance.NewGameData();
                    SceneManager.LoadScene("Intro");
                    break;
                case 2:
                    ScreenCapture.CaptureScreenshot(Path.GetFullPath("Logs/smoke-intro.png"));
                    break;
                case 3:
                    SceneManager.LoadScene("InGame");
                    break;
                case 4:
                    ScreenCapture.CaptureScreenshot(Path.GetFullPath("Logs/smoke-ingame.png"));
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
                    SceneManager.LoadScene("InGame");
                    break;
                default:
                    if (InGameMgr.Instance.stageData == null) throw new Exception("InGame re-entry failed");
                    Finish();
                    return;
            }
            SessionState.SetInt(StepKey, step + 1);
            nextStep = EditorApplication.timeSinceStartup + 8;
        }
        catch (Exception exception)
        {
            Debug.LogException(exception);
            Finish();
        }
    }

    private static void Finish()
    {
        SessionState.SetBool(ActiveKey, false);
        EditorApplication.update -= Update;
        Application.logMessageReceived -= OnLog;
        Debug.Log($"PLAY SMOKE {(errors == 0 ? "PASSED" : "FAILED")}: {errors} errors");
        EditorApplication.Exit(errors == 0 ? 0 : 1);
    }
}
