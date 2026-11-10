using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>Captures all playable gathering screens and checks their imported sprite references.</summary>
[InitializeOnLoad]
public static class MiniGameArtValidation
{
    const string Prefix = "HerosManager.MiniGameArt.";
    const string Output = "Logs/art-after";
    static IEnumerator<Gate> sequence;
    static Gate waiting;
    static double deadline, started;
    static int errors;
    static readonly HashSet<string> checkedClips = new();
    static readonly HashSet<string> capturedGames = new();
    static readonly StringBuilder report = new();

    sealed class Gate
    {
        internal string Name;
        internal Func<bool> Ready;
        internal double Timeout;
    }

    sealed class CanvasState
    {
        internal Canvas Canvas;
        internal RenderMode Mode;
        internal Camera Camera;
        internal float Distance;
    }

    static MiniGameArtValidation()
    {
        if (SessionState.GetBool(Prefix + "Active", false) || SessionState.GetBool(Prefix + "Exiting", false)) Attach();
    }

    [MenuItem("Tools/Heros Manager/Validate and Capture Minigame Art")]
    public static void Run()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode)
            throw new InvalidOperationException("Run minigame art validation outside Play mode.");
        ValidationSaveSession.Begin(Prefix);
        Directory.CreateDirectory(Output);
        File.WriteAllText(Output + "/errors.txt", string.Empty);
        SessionState.SetBool(Prefix + "Active", true);
        SessionState.SetBool(Prefix + "Exiting", false);
        SessionState.SetInt(Prefix + "ExitCode", 1);
        SessionState.SetInt(Prefix + "Errors", 0);
        sequence = null;
        waiting = null;
        Attach();
        EditorSceneManager.OpenScene("Assets/01.Scenes/Title.unity");
        EditorApplication.isPlaying = true;
    }

    static void Attach()
    {
        EditorApplication.update -= Tick;
        EditorApplication.update += Tick;
        Application.logMessageReceived -= OnLog;
        Application.logMessageReceived += OnLog;
        errors = SessionState.GetInt(Prefix + "Errors", 0);
    }

    static void OnLog(string message, string trace, LogType type)
    {
        if (type != LogType.Error && type != LogType.Exception && type != LogType.Assert) return;
        errors++;
        SessionState.SetInt(Prefix + "Errors", errors);
        File.AppendAllText(Output + "/errors.txt", message + "\n" + trace + "\n");
    }

    static void Tick()
    {
        if (!ValidationSaveSession.IsOwner(Prefix))
        { EditorApplication.update -= Tick; Application.logMessageReceived -= OnLog; return; }
        if (SessionState.GetBool(Prefix + "Exiting", false))
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode) return;
            EditorApplication.update -= Tick;
            Application.logMessageReceived -= OnLog;
            ValidationSaveSession.CompleteRunner(Prefix);
            SessionState.SetBool(Prefix + "Exiting", false);
            if (Application.isBatchMode) EditorApplication.Exit(SessionState.GetInt(Prefix + "ExitCode", 1));
            return;
        }
        if (!EditorApplication.isPlaying) return;
        try
        {
            Require(errors == 0, "Unity logged errors; see " + Output + "/errors.txt.");
            if (sequence == null)
            {
                started = EditorApplication.timeSinceStartup;
                checkedClips.Clear();
                capturedGames.Clear();
                report.Clear();
                sequence = Review().GetEnumerator();
            }
            Require(EditorApplication.timeSinceStartup - started < 120, "Minigame art validation exceeded 120 seconds.");
            if (waiting != null)
            {
                Require(EditorApplication.timeSinceStartup < deadline, "Timed out: " + waiting.Name);
                if (!waiting.Ready()) return;
                waiting = null;
            }
            if (!sequence.MoveNext()) { Finish(true); return; }
            waiting = sequence.Current;
            deadline = EditorApplication.timeSinceStartup + waiting.Timeout;
        }
        catch (Exception exception) { Debug.LogException(exception); Finish(false); }
    }

    static Gate Until(string name, Func<bool> ready, double timeout = 20) =>
        new Gate { Name = name, Ready = ready, Timeout = timeout };

    static Gate Delay(double seconds)
    {
        double end = EditorApplication.timeSinceStartup + seconds;
        return Until("animation/spawn delay", () => EditorApplication.timeSinceStartup >= end, seconds + 5);
    }

    static IEnumerable<Gate> Review()
    {
        ValidationSaveSession.RequireActive();
        yield return Until("Title", () => SceneManager.GetActiveScene().name == "Title" &&
            UnityEngine.Object.FindAnyObjectByType<TitleManager>() != null);
        yield return Delay(0.25);
        Require(SaveStoragePaths.IsIsolated, "Art validation requires an isolated save profile.");
        for (int stage = 1; stage <= 2; stage++)
        {
            var fixture = SaveDataManager.Instance.CreateNewGameData();
            fixture.stage = stage;
            fixture.day = 1;
            fixture.onboardingComplete = true;
            fixture.hasEventSnapshot = true;
            fixture.pendingEventIds = Array.Empty<string>();
            Require(SaveDataManager.Instance.TrySaveGameSnapshot(fixture), "Save the isolated stage fixture.");
            SceneManager.LoadScene("InGame");
            int expectedStage = stage;
            yield return Until("stage " + stage + " camp", () => SceneManager.GetActiveScene().name == "InGame" &&
                InGameMgr.IsCreateInstance() && InGameMgr.Instance.stage == expectedStage &&
                AdventureManager.Instance.IsJourneyReady && !AdventureManager.Instance.BlackPanel.activeSelf);
            yield return Delay(0.25);
            // This is a visual fixture, not a replacement for journey/balance validation.
            GatheringManager.Instance.isFirst = false;
            TutorialMgr.Instance.CloseTutorial();
            AdventureManager.Instance.AdvPanel.SetActive(false);
            AdventureManager.Instance.BlackPanel.SetActive(false);
            InGameMgr.Instance.state = State.Camp;
            Clock.Instance.enabled = false;
            Time.timeScale = 1;

            string[] areas = stage == 1
                ? new[] { "CS_S1_Forest", "CS_S1_Grassland", "CS_S1_Lake" }
                : new[] { "CS_S2_Ruins", "CS_S2_Desertfield", "CS_S2_Desertlake", "CS_S2_Oasis" };
            foreach (string area in areas)
            {
                MiniGameMgr.Instance.Setup(area);
                yield return Delay(0.65);
                Require(MiniGameMgr.Instance.CurrentArea == area && MiniGameMgr.Instance.MiniGamePanel.activeInHierarchy,
                    "The collection session did not open: " + area);
                ValidateVisibleGames(area);
                Capture(area + "-early");
                yield return Delay(1.75);
                ValidateVisibleGames(area);
                Capture(area + "-late");
                MiniGameMgr.Instance.CloseMiniGame();
                yield return Delay(0.15);
                Require(!MiniGameMgr.Instance.MiniGamePanel.activeSelf, "Collection did not close: " + area);
            }
        }
        Require(capturedGames.Count == 13, "Expected all six meadow and seven desert minigames, got " + capturedGames.Count);
        File.WriteAllText(Output + "/sprite-report.txt", report.ToString());
        Debug.Log("MINIGAME ART SCOPE: 13 games in 7 collection areas, 14 UI captures, active sprites and animation frame references. No human-input or full-duration balance claim.");
    }

    static void ValidateVisibleGames(string area)
    {
        ValidationSaveSession.RequireActive();
        var data = LoadGameData.Instance.collectSpaceDatas[area];
        foreach (string code in new[] { data.MiniGameID1, data.MiniGameID2 })
        {
            if (string.IsNullOrWhiteSpace(code) || code == "-1") continue;
            var game = MiniGameMgr.Instance.miniGames.Single(entry => entry.code == code);
            Require(game.obj.activeInHierarchy && game.obj.GetComponentsInChildren<MiniGameSetMgr>().Length == 1,
                "The expected minigame controller is inactive: " + code);
            capturedGames.Add(code);
            int sprites = 0;
            foreach (var graphic in game.obj.GetComponentsInChildren<Image>())
            {
                if (!graphic.enabled || graphic.sprite == null) continue; // Solid-color UI images intentionally have no sprite.
                ValidateSprite(graphic.sprite, code + "/" + graphic.name);
                sprites++;
            }
            foreach (var renderer in game.obj.GetComponentsInChildren<SpriteRenderer>())
            {
                if (!renderer.enabled) continue;
                Require(renderer.sprite != null, "Missing SpriteRenderer sprite in " + code + "/" + renderer.name);
                ValidateSprite(renderer.sprite, code + "/" + renderer.name);
                sprites++;
            }
            Require(sprites > 0, "No visible imported sprites in " + code);
            foreach (var animator in game.obj.GetComponentsInChildren<Animator>(true))
            {
                if (animator.runtimeAnimatorController == null) continue;
                foreach (var clip in animator.runtimeAnimatorController.animationClips)
                {
                    string path = AssetDatabase.GetAssetPath(clip);
                    if (!checkedClips.Add(path)) continue;
                    foreach (var binding in AnimationUtility.GetObjectReferenceCurveBindings(clip))
                    {
                        if (binding.propertyName != "m_Sprite") continue;
                        foreach (var key in AnimationUtility.GetObjectReferenceCurve(clip, binding))
                        {
                            Require(key.value is Sprite, "Missing animation sprite at " + path + ":" + key.time);
                            ValidateSprite((Sprite)key.value, path + ":" + key.time);
                        }
                    }
                }
            }
        }
    }

    static void ValidateSprite(Sprite sprite, string owner)
    {
        Require(sprite.texture != null && sprite.rect.width > 0 && sprite.rect.height > 0 && sprite.pixelsPerUnit > 0,
            "Invalid imported sprite: " + owner);
        report.AppendLine(owner + " | " + AssetDatabase.GetAssetPath(sprite) + " | " + sprite.rect + " | PPU=" + sprite.pixelsPerUnit);
    }

    static void Capture(string name)
    {
        ValidationSaveSession.RequireActive();
        var camera = Camera.main;
        Require(camera != null, "UI capture requires the main camera.");
        const int width = 1280, height = 720;
        var texture = new RenderTexture(width, height, 24, RenderTextureFormat.ARGB32);
        var pixels = new Texture2D(width, height, TextureFormat.RGB24, false);
        var previousActive = RenderTexture.active;
        var previousTarget = camera.targetTexture;
        float previousAspect = camera.aspect;
        int previousMask = camera.cullingMask;
        var overlays = new List<CanvasState>();
        try
        {
            texture.Create();
            camera.targetTexture = texture;
            camera.aspect = (float)width / height;
            foreach (var canvas in UnityEngine.Object.FindObjectsByType<Canvas>(FindObjectsSortMode.None))
            {
                if (!canvas.enabled || !canvas.isRootCanvas || canvas.renderMode != RenderMode.ScreenSpaceOverlay) continue;
                overlays.Add(new CanvasState { Canvas = canvas, Mode = canvas.renderMode, Camera = canvas.worldCamera, Distance = canvas.planeDistance });
                canvas.renderMode = RenderMode.ScreenSpaceCamera;
                canvas.worldCamera = camera;
                canvas.planeDistance = camera.nearClipPlane + 0.05f;
                foreach (var graphic in canvas.GetComponentsInChildren<Graphic>()) camera.cullingMask |= 1 << graphic.gameObject.layer;
            }
            Require(overlays.Count > 0, "Capture must include the active minigame UI.");
            Canvas.ForceUpdateCanvases();
            RenderPipeline.SubmitRenderRequest(camera, new RenderPipeline.StandardRequest { destination = texture });
            RenderTexture.active = texture;
            pixels.ReadPixels(new Rect(0, 0, width, height), 0, 0);
            pixels.Apply();
            File.WriteAllBytes(Output + "/" + name + ".png", pixels.EncodeToPNG());
            Debug.Log("MINIGAME ART CAPTURE: " + name);
        }
        finally
        {
            foreach (var state in overlays)
            {
                if (state.Canvas == null) continue;
                state.Canvas.renderMode = state.Mode;
                state.Canvas.worldCamera = state.Camera;
                state.Canvas.planeDistance = state.Distance;
            }
            camera.cullingMask = previousMask;
            camera.targetTexture = previousTarget;
            camera.aspect = previousAspect;
            Canvas.ForceUpdateCanvases();
            RenderTexture.active = previousActive;
            texture.Release();
            UnityEngine.Object.DestroyImmediate(texture);
            UnityEngine.Object.DestroyImmediate(pixels);
        }
    }

    static void Require(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
    }

    static void Finish(bool success)
    {
        Debug.Log(success ? "MINIGAME ART VALIDATION PASSED" : "MINIGAME ART VALIDATION FAILED");
        SessionState.SetInt(Prefix + "ExitCode", success ? 0 : 1);
        SessionState.SetBool(Prefix + "Active", false);
        SessionState.SetBool(Prefix + "Exiting", true);
        sequence?.Dispose();
        sequence = null;
        EditorApplication.isPlaying = false;
    }
}
