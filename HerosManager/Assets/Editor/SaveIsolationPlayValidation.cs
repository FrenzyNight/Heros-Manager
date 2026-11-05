using System;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

/// <summary>Exercises real Title saves, reload on/off, interruption, late writes and a fresh retry.</summary>
[InitializeOnLoad]
public static class SaveIsolationPlayValidation
{
    const string Prefix = "HerosManager.IsolationPlay.";
    static SaveIsolationPlayValidation()
    {
        if (SessionState.GetBool(Prefix + "Suite", false)) Attach();
    }

    public static void Run()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode) throw new InvalidOperationException("Exit Play mode first.");
        SessionState.SetBool(Prefix + "OriginalEnabled", EditorSettings.enterPlayModeOptionsEnabled);
        SessionState.SetInt(Prefix + "OriginalOptions", (int)EditorSettings.enterPlayModeOptions);
        SessionState.SetBool(Prefix + "Suite", true);
        SessionState.SetBool(Prefix + "Failed", false);
        SessionState.SetInt(Prefix + "Cycle", 0);
        Attach();
        BeginCycle();
    }

    static void Attach()
    {
        EditorApplication.update -= Tick;
        EditorApplication.update += Tick;
        Application.logMessageReceived -= OnLog;
        Application.logMessageReceived += OnLog;
    }

    static void OnLog(string message, string trace, LogType type)
    {
        if (type == LogType.Error || type == LogType.Exception || type == LogType.Assert)
            SessionState.SetBool(Prefix + "Failed", true);
    }

    static void BeginCycle()
    {
        string previous = SessionState.GetString(Prefix + "Root", "");
        string root = ValidationSaveSession.Begin(Prefix);
        if (root == previous) throw new Exception("A retry reused the previous test profile.");
        SessionState.SetString(Prefix + "Root", root);
        SessionState.SetBool(Prefix + "Active", true);
        SessionState.SetBool(Prefix + "Exiting", false);
        SessionState.SetInt(Prefix + "Phase", 0);
        SessionState.SetFloat(Prefix + "Started", (float)EditorApplication.timeSinceStartup);
        bool noReload = SessionState.GetInt(Prefix + "Cycle", 0) == 1;
        EditorSettings.enterPlayModeOptionsEnabled = noReload;
        EditorSettings.enterPlayModeOptions = EnterPlayModeOptions.DisableDomainReload;
        EditorSceneManager.OpenScene("Assets/01.Scenes/Title.unity");
        EditorApplication.isPlaying = true;
    }

    static void Tick()
    {
        try
        {
            if (SessionState.GetBool(Prefix + "Failed", false))
            {
                if (EditorApplication.isPlayingOrWillChangePlaymode) EditorApplication.isPlaying = false;
                else Finish(false);
                return;
            }
            if (EditorApplication.timeSinceStartup - SessionState.GetFloat(Prefix + "Started", 0) > 60)
                throw new Exception("Isolation Play cycle timed out.");
            if (SessionState.GetInt(Prefix + "Phase", 0) == 0)
            {
                if (!EditorApplication.isPlaying || EditorApplication.timeSinceStartup - SessionState.GetFloat(Prefix + "Started", 0) < 4) return;
                string root = SessionState.GetString(Prefix + "Root", "");
                if (!SaveStoragePaths.SamePath(root, SaveStoragePaths.Directory)) throw new Exception("Reload lost the isolated profile.");
                var saves = SaveDataManager.Instance;
                saves.LoadOptionDatas();
                if (!saves.TrySaveGameSnapshot(saves.CreateNewGameData()) || !saves.TrySaveOptionDatas() ||
                    !File.Exists(Path.Combine(root, "SaveOptionData.Json"))) throw new Exception("Real save callbacks did not use the owned profile.");
                SessionState.SetInt(Prefix + "Phase", 1);
                // Deliberately leave Active true: this is the same path as manually pressing Stop.
                EditorApplication.isPlaying = false;
                return;
            }
            if (EditorApplication.isPlayingOrWillChangePlaymode) return;
            if (SessionState.GetString(SaveStoragePaths.ValidationKey + "State", "") != "sealed" ||
                SessionState.GetBool(Prefix + "Active", true)) throw new Exception("Interrupted runner was not sealed and disarmed.");
            string file = Path.Combine(SessionState.GetString(Prefix + "Root", ""), "SaveOptionData.Json");
            if (JsonSaveStore.TrySave(file, new SaveOptionData(), SaveOptionData.IsValid, out _))
                throw new Exception("A callback saved after interruption.");
            Debug.Log("ISOLATION PLAY CYCLE PASSED: domain reload " + (SessionState.GetInt(Prefix + "Cycle", 0) == 0 ? "on" : "off"));
            SessionState.SetBool(Prefix + "Exiting", false);
            if (SessionState.GetInt(Prefix + "Cycle", 0) == 0)
            { SessionState.SetInt(Prefix + "Cycle", 1); BeginCycle(); }
            else Finish(true);
        }
        catch (Exception exception)
        {
            Debug.LogException(exception);
            SessionState.SetBool(Prefix + "Failed", true);
            if (EditorApplication.isPlayingOrWillChangePlaymode) EditorApplication.isPlaying = false;
            else Finish(false);
        }
    }

    static void Finish(bool passed)
    {
        try { ValidationSaveSession.Complete(); }
        catch (Exception exception) { Debug.LogException(exception); passed = false; }
        EditorSettings.enterPlayModeOptionsEnabled = SessionState.GetBool(Prefix + "OriginalEnabled", false);
        EditorSettings.enterPlayModeOptions = (EnterPlayModeOptions)SessionState.GetInt(Prefix + "OriginalOptions", 0);
        SessionState.SetBool(Prefix + "Suite", false);
        SessionState.SetBool(Prefix + "Active", false);
        SessionState.SetBool(Prefix + "Exiting", false);
        EditorApplication.update -= Tick;
        Application.logMessageReceived -= OnLog;
        Debug.Log(passed ? "SAVE ISOLATION PLAY VALIDATION PASSED" : "SAVE ISOLATION PLAY VALIDATION FAILED");
        if (Application.isBatchMode) EditorApplication.Exit(passed ? 0 : 1);
    }
}
