using System;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

/// <summary>Renders the authored camp at fixed clock samples, using an isolated save.</summary>
[InitializeOnLoad]
public static class LightingPlayValidation
{
    const string Key = "HerosManager.LightingReview.";
    static int step, errors;
    static double next, started;
    static Quaternion pausedSun;
    static float pausedFire;
    static readonly float[] Samples = { 0, 0.25f, 0.47f, 0.72f, 0.72f };
    static readonly string[] Names = { "morning", "day", "sunset", "night", "fire" };

    static LightingPlayValidation()
    {
        if (SessionState.GetBool(Key + "Active", false) || SessionState.GetBool(Key + "Exiting", false)) Attach();
    }

    [MenuItem("Tools/Heros Manager/Validate and Capture Camp Lighting")]
    public static void Run()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode) throw new InvalidOperationException("Exit Play mode first.");
        ValidationSaveSession.Begin(Key);
        SessionState.SetBool(Key + "Active", true);
        SessionState.SetBool(Key + "Exiting", false);
        SessionState.SetBool(Key + "Baseline", Array.IndexOf(Environment.GetCommandLineArgs(), "-lightingBaseline") >= 0);
        SessionState.SetInt(Key + "ExitCode", 1);
        Attach();
        EditorSceneManager.OpenScene("Assets/01.Scenes/Title.unity");
        EditorApplication.isPlaying = true;
    }

    static void Attach()
    {
        step = 0; errors = 0;
        started = EditorApplication.timeSinceStartup;
        next = started + 2;
        EditorApplication.update -= Tick;
        EditorApplication.update += Tick;
        Application.logMessageReceived -= OnLog;
        Application.logMessageReceived += OnLog;
    }

    static void OnLog(string message, string trace, LogType type)
    {
        if (type == LogType.Exception || type == LogType.Error || type == LogType.Assert) errors++;
    }

    static void Tick()
    {
        if (!ValidationSaveSession.IsOwner(Key))
        { EditorApplication.update -= Tick; Application.logMessageReceived -= OnLog; return; }
        if (!SessionState.GetBool(Key + "Active", false))
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode) return;
            EditorApplication.update -= Tick;
            Application.logMessageReceived -= OnLog;
            ValidationSaveSession.CompleteRunner(Key);
            SessionState.SetBool(Key + "Exiting", false);
            if (Application.isBatchMode) EditorApplication.Exit(SessionState.GetInt(Key + "ExitCode", 1));
            return;
        }
        if (!EditorApplication.isPlaying || EditorApplication.timeSinceStartup < next) return;
        try
        {
            if (errors > 0 || EditorApplication.timeSinceStartup - started > 80) throw new Exception("Lighting review logged errors or timed out.");
            if (step == 0)
            {
                var data = SaveDataManager.Instance.CreateNewGameData();
                data.onboardingComplete = true;
                if (!SaveDataManager.Instance.TrySaveGameSnapshot(data)) throw new Exception("Isolated save failed.");
                SceneManager.LoadScene("InGame");
                step++; next = EditorApplication.timeSinceStartup + 3; return;
            }
            if (step == 1)
            {
                AdventureManager.Instance.AdvPanel.SetActive(false);
                AdventureManager.Instance.BlackPanel.SetActive(false);
                InGameMgr.Instance.state = State.Camp;
                Clock.Instance.enabled = false;
                Time.timeScale = 0;
                Camera.main.aspect = 16f / 9f;
                if (!SessionState.GetBool(Key + "Baseline", false)) ValidateTwilight();
                step++; next = EditorApplication.timeSinceStartup + 1; return;
            }
            int index = (step - 2) / 2;
            if (index >= Samples.Length)
            {
                if (!SessionState.GetBool(Key + "Baseline", false))
                {
                    var fire = UnityEngine.Object.FindAnyObjectByType<FireInteractionMgr>();
                    fire.NextDayAction();
                    if (fire.CampObjMgr.GetComponent<FireMgr>().FireObj.activeInHierarchy)
                        throw new Exception("Fire light remained active after the daily reset.");
                }
                Finish(true); return;
            }
            if (step % 2 == 0)
            {
                Clock.Instance.nowTime = Clock.Instance.DayDurationSeconds * Samples[index];
                if (index == 4)
                {
                    var fire = UnityEngine.Object.FindAnyObjectByType<FireInteractionMgr>();
                    ItemManager.Instance.AddItem(fire.NeedItemID, fire.needAmount);
                    fire.ClickButton();
                    foreach (var particles in fire.CampObjMgr.GetComponent<FireMgr>().FireObj.GetComponentsInChildren<ParticleSystem>())
                        particles.Simulate(1.5f, false, true);
                }
                var lighting = UnityEngine.Object.FindAnyObjectByType<LightController>();
                lighting.SendMessage("Update");
                pausedSun = lighting.sunLight.transform.rotation;
                pausedFire = FindFireLight().intensity;
            }
            else
            {
                var lighting = UnityEngine.Object.FindAnyObjectByType<LightController>();
                if (Quaternion.Angle(pausedSun, lighting.sunLight.transform.rotation) > 0.01f ||
                    Mathf.Abs(pausedFire - FindFireLight().intensity) > 0.001f)
                    throw new Exception("Lighting moved while the game was paused.");
                if (index == 4 && !FindFireLight().isActiveAndEnabled)
                    throw new Exception("Lighting did not follow the real fire interaction.");
                Capture(Names[index]);
            }
            step++; next = EditorApplication.timeSinceStartup + 0.5;
        }
        catch (Exception exception) { Debug.LogException(exception); Finish(false); }
    }

    static Light FindFireLight() => UnityEngine.Object.FindAnyObjectByType<FireMgr>()
        .FireObj.GetComponentInChildren<Light>(true);

    static void ValidateTwilight()
    {
        var lighting = UnityEngine.Object.FindAnyObjectByType<LightController>();
        Clock.Instance.nowTime = Clock.Instance.DayDurationSeconds * 0.499f;
        lighting.SendMessage("Update");
        var before = lighting.sunLight.transform.rotation;
        float intensity = lighting.sunLight.intensity;
        Clock.Instance.nowTime = Clock.Instance.DayDurationSeconds * 0.501f;
        lighting.SendMessage("Update");
        if (Quaternion.Angle(before, lighting.sunLight.transform.rotation) > 2f ||
            Mathf.Abs(intensity - lighting.sunLight.intensity) > 0.05f)
            throw new Exception("The sun jumped across the half-day boundary.");
        if (!Camera.main.GetUniversalAdditionalCameraData().renderPostProcessing)
            throw new Exception("Main stage color grading is not connected.");
        if (FindFireLight().isActiveAndEnabled)
            throw new Exception("Fire light was active before lighting the fire.");
        Debug.Log("LIGHTING: twilight continuity, grading and unlit fire verified.");
    }

    static void Capture(string name)
    {
        string folder = "Logs/lighting-" + (SessionState.GetBool(Key + "Baseline", false) ? "before" : "after");
        Directory.CreateDirectory(folder);
        var texture = new RenderTexture(1280, 720, 24, RenderTextureFormat.ARGB32);
        var pixels = new Texture2D(1280, 720, TextureFormat.RGB24, false);
        var previous = RenderTexture.active;
        var overlays = new System.Collections.Generic.List<Canvas>();
        try
        {
            texture.Create();
            foreach (var canvas in UnityEngine.Object.FindObjectsByType<Canvas>(FindObjectsSortMode.None))
                if (canvas.enabled && canvas.renderMode == RenderMode.ScreenSpaceOverlay)
                { overlays.Add(canvas); canvas.enabled = false; }
            // Use the normal camera-stack path, including initialization and volume evaluation.
            RenderPipeline.SubmitRenderRequest(Camera.main, new RenderPipeline.StandardRequest { destination = texture });
            if (!SessionState.GetBool(Key + "Baseline", false) &&
                VolumeManager.instance.stack.GetComponent<Tonemapping>().mode.value != TonemappingMode.Neutral)
                throw new Exception("The scene lighting profile did not reach the camera volume stack.");
            RenderTexture.active = texture;
            pixels.ReadPixels(new Rect(0, 0, 1280, 720), 0, 0);
            pixels.Apply();
            File.WriteAllBytes(Path.Combine(folder, name + ".png"), pixels.EncodeToPNG());
            Debug.Log("LIGHTING CAPTURE: " + name);
        }
        finally
        {
            foreach (var canvas in overlays) if (canvas != null) canvas.enabled = true;
            RenderTexture.active = previous;
            texture.Release();
            UnityEngine.Object.DestroyImmediate(texture);
            UnityEngine.Object.DestroyImmediate(pixels);
        }
    }

    static void Finish(bool passed)
    {
        Debug.Log(passed ? "LIGHTING PLAY VALIDATION PASSED" : "LIGHTING PLAY VALIDATION FAILED");
        SessionState.SetInt(Key + "ExitCode", passed ? 0 : 1);
        SessionState.SetBool(Key + "Active", false);
        SessionState.SetBool(Key + "Exiting", true);
        EditorApplication.isPlaying = false;
    }
}
