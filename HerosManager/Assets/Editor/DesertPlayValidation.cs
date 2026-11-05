using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>Scene integration with deterministic supplies, journey samples and accelerated day boundaries.</summary>
[InitializeOnLoad]
public static class DesertPlayValidation
{
    const string Prefix = "HerosManager.DesertPlay.";
    static IEnumerator<Gate> sequence;
    static Gate waiting;
    static double deadline, started;
    static int errors;

    sealed class Gate
    {
        internal Func<bool> Ready;
        internal string Name;
        internal double Timeout;
    }

    sealed class VirtualPad : IGamepadSource
    {
        internal GamepadFrame Frame;
        public GamepadFrame Read() => Frame;
        public void SetVibration(float low, float high) { }
    }

    static DesertPlayValidation()
    {
        if (SessionState.GetBool(Prefix + "Active", false) || SessionState.GetBool(Prefix + "Exiting", false))
            Attach();
    }

    [MenuItem("Tools/Heros Manager/Validate Desert Campaign Play")]
    public static void Run()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode)
            throw new InvalidOperationException("Run desert validation outside Play mode.");
        ValidationSaveSession.Begin(Prefix);
        Directory.CreateDirectory("Logs");
        File.WriteAllText("Logs/desert-play-errors.txt", string.Empty);
        SessionState.SetBool(Prefix + "Active", true);
        SessionState.SetBool(Prefix + "Exiting", false);
        SessionState.SetInt(Prefix + "Errors", 0);
        sequence = null;
        waiting = null;
        Attach();
        EditorSceneManager.OpenScene("Assets/01.Scenes/Title.unity");
        EditorApplication.isPlaying = true;
    }

    static void Attach()
    {
        EditorApplication.update -= Update;
        EditorApplication.update += Update;
        Application.logMessageReceived -= OnLog;
        Application.logMessageReceived += OnLog;
        errors = SessionState.GetInt(Prefix + "Errors", 0);
    }

    static void OnLog(string message, string stack, LogType type)
    {
        if (type != LogType.Error && type != LogType.Assert && type != LogType.Exception) return;
        errors++;
        SessionState.SetInt(Prefix + "Errors", errors);
        File.AppendAllText("Logs/desert-play-errors.txt", message + "\n" + stack + "\n");
    }

    static void Update()
    {
        if (!ValidationSaveSession.IsOwner(Prefix))
        { EditorApplication.update -= Update; Application.logMessageReceived -= OnLog; return; }
        if (SessionState.GetBool(Prefix + "Exiting", false))
        {
            if (!EditorApplication.isPlayingOrWillChangePlaymode) Cleanup();
            return;
        }
        if (!EditorApplication.isPlaying) return;
        try
        {
            Require(errors == 0, "Unity logged errors; see Logs/desert-play-errors.txt.");
            if (sequence == null)
            {
                started = EditorApplication.timeSinceStartup;
                sequence = Campaign().GetEnumerator();
            }
            Require(EditorApplication.timeSinceStartup - started < 240, "Desert validation exceeded 240 seconds.");
            if (waiting != null)
            {
                Require(EditorApplication.timeSinceStartup < deadline, "Timed out: " + waiting.Name + Diagnostics());
                if (!waiting.Ready()) return;
                waiting = null;
            }
            if (!sequence.MoveNext()) { Finish(true); return; }
            waiting = sequence.Current;
            deadline = EditorApplication.timeSinceStartup + waiting.Timeout;
        }
        catch (Exception exception)
        {
            Debug.LogException(exception);
            Finish(false);
        }
    }

    static string Diagnostics()
    {
        if (!InGameMgr.IsCreateInstance()) return " scene=" + SceneManager.GetActiveScene().name;
        var adventure = AdventureManager.Instance;
        return $" scene={SceneManager.GetActiveScene().name}, stage={InGameMgr.Instance.stage}, day={Clock.Instance.day}, " +
            $"modal={(MeadowCampaign.IsCreateInstance() && MeadowCampaign.Instance.IsModalOpen)}, ready={adventure.IsJourneyReady}, " +
            $"panel={adventure.AdvPanel.activeInHierarchy}, black={adventure.BlackPanel.activeSelf}, tutorial={TutorialMgr.Instance.isTutorial}";
    }

    static Gate Until(string name, Func<bool> ready, double timeout = 20) =>
        new Gate { Name = name, Ready = ready, Timeout = timeout };

    static Gate Delay(double seconds)
    {
        double end = EditorApplication.timeSinceStartup + seconds;
        return Until("frame/animation delay", () => EditorApplication.timeSinceStartup >= end, seconds + 5);
    }

    static IEnumerable<Gate> Campaign()
    {
        ValidationSaveSession.RequireActive();
        yield return Until("Title", () => SceneManager.GetActiveScene().name == "Title" &&
            UnityEngine.Object.FindAnyObjectByType<TitleManager>() != null);
        yield return Delay(0.25);
        Require(SaveStoragePaths.IsIsolated, "Desert validation must use an isolated profile.");
        var fixture = SaveDataManager.Instance.CreateNewGameData();
        fixture.stage = 1;
        fixture.day = 4;
        fixture.meadowOutcome = (int)MeadowOutcome.Victory;
        fixture.onboardingComplete = true;
        fixture.items = new[] { 50, 24, 24, 12, 12 };
        fixture.gems = 5;
        fixture.fenceLevel = 2;
        fixture.hasEventSnapshot = true;
        fixture.pendingEventIds = Array.Empty<string>();
        fixture.hero1Stat = new[] { 10f, 135f, 60f, 40f };
        fixture.hero2Stat = new[] { 20f, 135f, 85f, 40f };
        fixture.hero3Stat = new[] { 30f, 135f, 100f, 40f };
        fixture.hero4Stat = new[] { 40f, 135f, 0f, 40f };
        Require(SaveDataManager.Instance.TrySaveGameSnapshot(fixture), "Save the meadow victory fixture in the isolated profile.");
        SceneManager.LoadScene("InGame");
        yield return Terminal(1, MeadowOutcome.Victory);
        Require(!AdventureManager.Instance.AdvPanel.activeSelf, "A legacy meadow victory reopens its terminal screen.");
        var meadow = MeadowCampaign.Instance;
        ModalButton("사막으로 이어가기").onClick.Invoke();
        Require(!meadow.AdvanceToDesert(), "A duplicate transition callback cannot award another departure package.");
        yield return ReadyDay(1);
        DismissTutorial();
        var arrival = VerifyDisk(MeadowOutcome.Active, 1);
        Require(arrival.items.SequenceEqual(new[] { 50, 36, 36, 12, 18 }) && arrival.gems == 8,
            "The transition awards exactly water 12, meat 12, food 6 and gems 3.");
        Require(arrival.fenceLevel == 2 && FenceMgr.Instance.fenceLevel == 2 && arrival.onboardingComplete &&
            arrival.hasEventSnapshot && arrival.pendingEventIds.Length == 0,
            "Fence, onboarding and the exhausted event queue survive the transition.");
        Require(arrival.hero1Stat.SequenceEqual(new[] { 10f, 135f, 90f, 40f }) &&
            arrival.hero2Stat[2] == 100 && arrival.hero3Stat[2] == 100 && arrival.hero4Stat[2] == 0,
            "Survivors heal by 30 with the health cap; dead heroes remain dead and growth is preserved.");
        Require(!MeadowCampaign.Instance.AdvanceToDesert(), "An active desert run cannot reapply transition rewards.");

        // Reload through the real Title Continue callback before any resource collection.
        SceneManager.LoadScene("Title");
        foreach (var gate in ContinueFromTitle()) yield return gate;
        yield return ReadyDay(1);
        DismissTutorial();
        var resumed = VerifyDisk(MeadowOutcome.Active, 1);
        Require(resumed.items.SequenceEqual(arrival.items) && resumed.gems == arrival.gems &&
            resumed.hero1Stat.SequenceEqual(arrival.hero1Stat), "Continue cannot duplicate the departure package or level growth.");
        float initialPower = HeroStateManager.Instance.heroStates[0].power;
        for (int day = 1; day <= 4; day++)
        {
            yield return ReadyDay(day);
            DismissTutorial();
            PrepareGoodJourney();
            if (day == 1)
            {
                CaptureUi("desert-preparation");
            }
            int[] before = ItemManager.Instance.CaptureItems();
            int meat = int.Parse(AdventureManager.Instance.ItemNumTexts[0].text);
            int water = int.Parse(AdventureManager.Instance.ItemNumTexts[1].text);
            AdventureManager.Instance.StartAdvBtn.onClick.Invoke();
            Require(AdventureManager.Instance.IsJourneyInProgress, "Desert departure starts from the actual supply button.");
            AdventureManager.Instance.StartAdvBtn.onClick.Invoke();
            Require(ItemManager.Instance.GetAmount(InventoryRules.Meat) == before[2] - meat &&
                ItemManager.Instance.GetAmount(InventoryRules.Water) == before[1] - water,
                "Repeated departure does not spend supplies twice.");
            yield return Until("departure animation", () => !AdventureManager.Instance.AdvPanel.activeSelf);
            DismissTutorial();
            foreach (var gate in VerifyDailyOasis(day)) yield return gate;
            if (day == 1)
                foreach (var gate in CollectDesertSupplies()) yield return gate;
            Require(AdventureManager.Instance.ResolveJourney(0.99f) && AdventureManager.Instance.LastJourneyResult.IsSuccess,
                "A fully prepared desert journey excludes failure even at a high random sample.");
            Require(!AdventureManager.Instance.ResolveJourney(0.99f), "A second resolution cannot grant growth twice.");
            DismissTutorial();
            Clock.Instance.nowTime = Clock.Instance.DayDurationSeconds;
            yield return Until("day report", () => MeadowCampaign.Instance.IsModalOpen);
            Require(Clock.Instance.isProduct && Clock.Instance.isStop, "The report stops the desert day.");
            Debug.Log($"DESERT PLAY DAY {day}: outcome={MeadowCampaign.Instance.Outcome}, courage={HeroStateManager.Instance.heroStates[0].exp}");
            if (day < 4)
            {
                Require(MeadowCampaign.Instance.Outcome == MeadowOutcome.Active, "The first three successful desert days continue.");
                ModalButton("다음 날").onClick.Invoke();
                yield return ReadyDay(day + 1);
                VerifyDisk(MeadowOutcome.Active, day + 1);
            }
        }
        Require(InGameMgr.Instance.IsComplete && MeadowCampaign.Instance.Outcome == MeadowOutcome.Victory,
            "Desert day four ends at an explicit victory, without entering unfinished forest content.");
        Require(HeroStateManager.Instance.heroStates[0].exp >= 80 &&
            HeroStateManager.Instance.heroStates[0].power > initialPower &&
            HeroStateManager.Instance.heroStates[3].hp == 0,
            "The desert continues courage/power growth while keeping fallen heroes incapacitated.");
        var victory = VerifyDisk(MeadowOutcome.Victory, 4);
        File.WriteAllText("Logs/desert-victory.json", JsonUtility.ToJson(victory, true));
        ModalButton("제목으로").onClick.Invoke();
        foreach (var gate in ContinueFromTitle()) yield return gate;
        yield return Terminal(2, MeadowOutcome.Victory);
        Require(!AdventureManager.Instance.AdvPanel.activeSelf && !MeadowCampaign.Instance.AdvanceToDesert(),
            "Continue restores desert victory and cannot grant another transition.");
        VerifyDisk(MeadowOutcome.Victory, 4);

        var finalFailure = victory.Clone();
        finalFailure.meadowOutcome = 0;
        finalFailure.hero1Stat[2] = finalFailure.hero2Stat[2] = finalFailure.hero3Stat[2] = 100;
        Require(SaveDataManager.Instance.TrySaveGameSnapshot(finalFailure), "Save an isolated final-day failure scenario.");
        SceneManager.LoadScene("InGame");
        yield return ReadyDay(4);
        DismissTutorial();
        AdventureManager.Instance.StartAdvBtn.onClick.Invoke();
        yield return Until("unsupported departure", () => !AdventureManager.Instance.AdvPanel.activeSelf);
        DismissTutorial();
        Require(AdventureManager.Instance.ResolveJourney(0.99f) && !AdventureManager.Instance.LastJourneyResult.IsSuccess,
            "An unprepared final desert journey can fail.");
        Require(HeroStateManager.Instance.HasAbleHeroes, "Final-day failure is tested with survivors, independently of all-dead defeat.");
        Clock.Instance.nowTime = Clock.Instance.DayDurationSeconds;
        yield return Terminal(2, MeadowOutcome.Defeat);
        var defeat = VerifyDisk(MeadowOutcome.Defeat, 4);
        File.WriteAllText("Logs/desert-defeat.json", JsonUtility.ToJson(defeat, true));
        ModalButton("제목으로").onClick.Invoke();
        foreach (var gate in ContinueFromTitle()) yield return gate;
        yield return Terminal(2, MeadowOutcome.Defeat);
        VerifyDisk(MeadowOutcome.Defeat, 4);
        Debug.Log("DESERT PLAY SCOPE: real scenes, transition/continue/supply/report buttons, seven minigame reward paths, virtual pad lumber input and deterministic journey samples. Synthetic pickups and food fund the run; accelerated clocks do not establish human-input skill, physical controller behavior or full-duration balance.");
    }

    static IEnumerable<Gate> ContinueFromTitle()
    {
        yield return Until("Title continuation", () => SceneManager.GetActiveScene().name == "Title" &&
            UnityEngine.Object.FindAnyObjectByType<TitleManager>() != null);
        yield return Delay(0.25);
        var title = UnityEngine.Object.FindAnyObjectByType<TitleManager>();
        title.SendMessage("TitleEff", SendMessageOptions.RequireReceiver);
        yield return Until("Title menu", () => title.MenuReady &&
            title.MenuTrans.GetChild(1).GetComponent<UnityEngine.EventSystems.EventTrigger>() != null);
        var button = title.MenuTrans.GetChild(1).GetComponent<Button>();
        Require(button.interactable, "A saved desert checkpoint enables Continue.");
        button.onClick.Invoke();
    }

    static Gate ReadyDay(int day) => Until("desert day " + day + " preparation", () =>
        SceneManager.GetActiveScene().name == "InGame" && InGameMgr.IsCreateInstance() && InGameMgr.Instance.stage == 2 &&
        MeadowCampaign.IsCreateInstance() && MeadowCampaign.Instance.ModalRoot != null && Clock.Instance.day == day &&
        AdventureManager.Instance.IsJourneyReady && !AdventureManager.Instance.isClicked &&
        AdventureManager.Instance.AdvPanel.activeInHierarchy && !AdventureManager.Instance.BlackPanel.activeSelf);

    static Gate Terminal(int stage, MeadowOutcome outcome) => Until("stage terminal " + stage + "/" + outcome, () =>
        SceneManager.GetActiveScene().name == "InGame" && InGameMgr.IsCreateInstance() && InGameMgr.Instance.stage == stage &&
        MeadowCampaign.IsCreateInstance() && MeadowCampaign.Instance.IsModalOpen && MeadowCampaign.Instance.Outcome == outcome);

    static void DismissTutorial()
    {
        if (FirstRunGuide.IsCreateInstance()) FirstRunGuide.Instance.SkipPractice();
        for (int i = 0; i < 3; i++)
        {
            TutorialMgr.Instance.CloseTutorial();
            GatheringManager.Instance.CloseGatheringPannel();
        }
    }

    static void PrepareGoodJourney()
    {
        var adventure = AdventureManager.Instance;
        var journey = LoadGameData.Instance.journeyDatas[Clock.Instance.stageDayData.JourneyID];
        for (int i = 0; i < journey.Item1Min; i++) adventure.AddItemBtn(0);
        for (int i = 0; i < journey.Item2Min; i++) adventure.AddItemBtn(1);
        for (int i = 0; i < 6; i++) adventure.AddItemBtn(2);
        // UI order is meat, water, food, herb; the inventory order is wood, water, meat, herb, food.
        int[] supplyOrder = { 0, 1, 3 };
        string[] inventoryForSupply = { InventoryRules.Meat, InventoryRules.Water, InventoryRules.Food, InventoryRules.Herb };
        for (int round = 0; round < 1000 && adventure.ResultImg.sprite != adventure.GoodImg; round++)
        {
            bool changed = false;
            foreach (int supply in supplyOrder)
            {
                if (adventure.ResultImg.sprite == adventure.GoodImg) break;
                int selected = int.Parse(adventure.ItemNumTexts[supply].text);
                if (selected >= ItemManager.Instance.GetAmount(inventoryForSupply[supply])) continue;
                adventure.AddItemBtn(supply);
                changed |= int.Parse(adventure.ItemNumTexts[supply].text) > selected;
            }
            if (!changed) break;
        }
        Require(adventure.ResultImg.sprite == adventure.GoodImg && adventure.StartAdvBtn.interactable,
            "Supplies selected through UI controls reach the safe journey bracket. " +
            $"stage={InGameMgr.Instance.stage}, day={Clock.Instance.day}, " +
            "stock[wood,water,meat,herb,food]=" + string.Join(",", ItemManager.Instance.CaptureItems()) +
            "; selected[meat,water,food,herb]=" + string.Join(",", adventure.ItemNumTexts.Select(text => text.text)) +
            "; heroes[stress,power,hp,courage]=" + string.Join(" / ",
                HeroStateManager.Instance.heroStates.Select(hero => string.Join(",", hero.CaptureStats()))));
    }

    static IEnumerable<Gate> VerifyDailyOasis(int day)
    {
        var gathering = GatheringManager.Instance;
        var area = gathering.gatheringObjs.Single(entry => entry.code == "CS_S2_Oasis");
        var collect = LoadGameData.Instance.collectDatas[InGameMgr.Instance.stageData.CollectID];
        var clock = Clock.Instance;
        float previousTime = clock.nowTime;
        bool previousStop = clock.isStop;
        Require(!area.obj.activeSelf, "Oasis is hidden at the beginning of desert day " + day + ".");
        // Freeze automatic journey/day updates while advancing the scheduled collection clock explicitly.
        clock.isStop = true;
        try
        {
            gathering.GatheringBtn.onClick.Invoke();
            clock.nowTime = collect.EndTime + 1;
            yield return Until("day " + day + " oasis appearance", () => area.obj.activeInHierarchy);
            var button = area.obj.GetComponent<Button>();
            Require(button != null && button.interactable, "The daily oasis appears as an actionable area button.");
            float appearedAt = clock.nowTime;
            yield return Delay(0.15);
            Require(area.obj.activeInHierarchy && clock.nowTime == appearedAt,
                "Pausing gameplay time does not consume the daily oasis visit.");
            button.onClick.Invoke();
            yield return Delay(0.1);
            Require(MiniGameMgr.Instance.CurrentArea == "CS_S2_Oasis" &&
                UnityEngine.Object.FindAnyObjectByType<OasisManager>() != null,
                "The visible daily oasis button opens its playable collection game.");
            MiniGameMgr.Instance.CloseMiniGame();
            clock.nowTime = appearedAt + collect.ContinueTime + 1;
            yield return Until("day " + day + " oasis closes", () => !area.obj.activeSelf);
            Debug.Log($"DESERT OASIS DAY {day}: appeared after configured window and closed after {collect.ContinueTime} gameplay seconds.");
        }
        finally
        {
            clock.nowTime = previousTime;
            clock.isStop = previousStop;
            gathering.CloseGatheringPannel();
        }
    }

    static IEnumerable<Gate> CollectDesertSupplies()
    {
        ValidationSaveSession.RequireActive();
        var games = MiniGameMgr.Instance;
        foreach (var gate in OpenDesertArea("CS_S2_Ruins", true)) yield return gate;
        var tree = UnityEngine.Object.FindAnyObjectByType<TreeManager>();
        var maze = UnityEngine.Object.FindAnyObjectByType<MazeManager>();
        Require(tree != null && maze != null && tree.isActiveAndEnabled && maze.isActiveAndEnabled, "Ruins activate both games.");
        int[] before = ItemManager.Instance.CaptureItems();
        int gems = CampGemWallet.Instance.GetAmount();
        ValidateTreePad(tree);
        maze.CollectTreasure();
        maze.CollectTreasure();
        int woodReward = (int)LoadGameData.Instance.miniGameDatas["game2tree_norm"].GetAmount1;
        var treasure = LoadGameData.Instance.miniGameDatas["game2maze"];
        CaptureUi("desert-ruins");
        games.CloseMiniGame();
        games.CloseMiniGame();
        Require(ItemManager.Instance.GetAmount(InventoryRules.Wood) == before[0] + woodReward &&
            ItemManager.Instance.GetAmount(InventoryRules.Herb) == before[3] + (int)treasure.GetAmount1 &&
            CampGemWallet.Instance.GetAmount() == gems + (int)treasure.GetJemAmount,
            "Repeated treasure callbacks in one frame and repeated session close cannot duplicate rewards.");

        foreach (var gate in OpenDesertArea("CS_S2_Desertfield")) yield return gate;
        Require(games.PendingGems == 0, "New collection sessions clear pending gems.");
        var hunt = UnityEngine.Object.FindAnyObjectByType<Hunt2Manager>();
        var herb = UnityEngine.Object.FindAnyObjectByType<Hub2Manager>();
        Require(hunt != null && herb != null && hunt.spawnObjects.Any(spawned =>
            spawned != null && spawned.activeInHierarchy && spawned.GetComponent<DesertAnimalMove>() != null),
            "Desertfield starts spawning live prey, not just cactus obstacles.");
        before = ItemManager.Instance.CaptureItems();
        for (int i = 0; i < 30; i++) { hunt.KillFox(); hunt.KillWeasel(); hunt.KillGoldFox(); herb.GetHub(); }
        games.CloseMiniGame();
        Require(ItemManager.Instance.GetAmount(InventoryRules.Meat) > before[2] &&
            ItemManager.Instance.GetAmount(InventoryRules.Herb) > before[3], "Desert hunt and herb callbacks grant resources.");

        foreach (var gate in OpenDesertArea("CS_S2_Desertlake")) yield return gate;
        var fish = UnityEngine.Object.FindAnyObjectByType<LakeFishManager>();
        var water = UnityEngine.Object.FindAnyObjectByType<LakeWaterManager>();
        Require(fish != null && water != null && fish.spawnObjects.Count > 0, "Desert lake starts fish spawning.");
        before = ItemManager.Instance.CaptureItems();
        fish.GetFish();
        fish.GetGoldFish();
        water.GetWater(1);
        for (int i = 0; i < 15; i++) water.GetWater(2);
        CaptureUi("desert-lake");
        games.CloseMiniGame();
        Require(ItemManager.Instance.GetAmount(InventoryRules.Meat) > before[2] &&
            ItemManager.Instance.GetAmount(InventoryRules.Water) > before[1], "Fishing and both bucket depths grant resources.");

        games.Setup("CS_S2_Oasis");
        yield return Delay(0.1);
        var oasis = UnityEngine.Object.FindAnyObjectByType<OasisManager>();
        Require(oasis != null && oasis.spawnObjects.Count > 0, "The oasis starts spawning collectible resources.");
        before = ItemManager.Instance.CaptureItems();
        oasis.GetWater(); oasis.GetMeat(); oasis.GetTree();
        games.CloseMiniGame();
        Require(ItemManager.Instance.GetAmount(InventoryRules.Wood) > before[0] &&
            ItemManager.Instance.GetAmount(InventoryRules.Water) > before[1] &&
            ItemManager.Instance.GetAmount(InventoryRules.Meat) > before[2], "Oasis callbacks grant all three resource types.");
        games.Setup("CS_S2_Oasis");
        yield return Delay(0.1);
        Require(games.PendingWood == 0 && games.PendingGems == 0 && oasis.spawnObjects.Count > 0,
            "Oasis re-entry starts fresh, without stale rewards or stopped spawning.");
        games.CloseMiniGame();
        DismissTutorial();
        // Food manufacture is covered separately by MeadowPlayValidation. This fixture isolates desert progression.
        ItemManager.Instance.AddItem("Item_Food", 100);
    }

    static IEnumerable<Gate> OpenDesertArea(string code, bool captureMenu = false)
    {
        var gathering = GatheringManager.Instance;
        gathering.GatheringBtn.onClick.Invoke();
        Require(gathering.GatheringPanel.activeInHierarchy, "The gathering button opens the area selection panel.");
        var area = gathering.gatheringObjs.SingleOrDefault(entry => entry.code == code);
        Require(area != null && area.obj != null, "The actual area menu contains " + code + ".");
        var button = area.obj.GetComponent<Button>();
        Require(area.obj.activeInHierarchy && button != null && button.interactable,
            "A visible actionable area button exists for " + code + ".");
        if (captureMenu)
        {
            yield return Delay(0.1);
            CaptureUi("desert-gathering");
        }
        button.onClick.Invoke();
        yield return Delay(0.1);
        Require(MiniGameMgr.Instance.CurrentArea == code && MiniGameMgr.Instance.MiniGamePanel.activeInHierarchy &&
            !gathering.GatheringPanel.activeInHierarchy,
            "The real area button closes selection and opens its configured minigames: " + code + ".");
    }

    sealed class CanvasCaptureState
    {
        internal Canvas Canvas;
        internal RenderMode Mode;
        internal Camera Camera;
        internal float Distance;
    }

    static void CaptureUi(string name)
    {
        ValidationSaveSession.RequireActive();
        var camera = Camera.main;
        Require(camera != null, "UI capture requires the main game camera.");
        const int width = 1920, height = 1080;
        var texture = new RenderTexture(width, height, 24, RenderTextureFormat.ARGB32);
        var pixels = new Texture2D(width, height, TextureFormat.RGB24, false);
        var previousActive = RenderTexture.active;
        var previousTarget = camera.targetTexture;
        int previousMask = camera.cullingMask;
        var overlays = new List<CanvasCaptureState>();
        try
        {
            texture.Create();
            camera.targetTexture = texture;
            foreach (var canvas in UnityEngine.Object.FindObjectsByType<Canvas>(FindObjectsSortMode.None))
            {
                if (!canvas.enabled || !canvas.isRootCanvas || canvas.renderMode != RenderMode.ScreenSpaceOverlay) continue;
                overlays.Add(new CanvasCaptureState
                {
                    Canvas = canvas, Mode = canvas.renderMode, Camera = canvas.worldCamera, Distance = canvas.planeDistance
                });
                canvas.renderMode = RenderMode.ScreenSpaceCamera;
                canvas.worldCamera = camera;
                canvas.planeDistance = camera.nearClipPlane + 0.05f;
                foreach (var graphic in canvas.GetComponentsInChildren<Graphic>())
                    camera.cullingMask |= 1 << graphic.gameObject.layer;
            }
            Require(overlays.Count > 0, "UI capture must include an active overlay canvas.");
            Canvas.ForceUpdateCanvases();
            RenderPipeline.SubmitRenderRequest(camera, new RenderPipeline.StandardRequest { destination = texture });
            RenderTexture.active = texture;
            pixels.ReadPixels(new Rect(0, 0, width, height), 0, 0);
            pixels.Apply();
            string path = Path.Combine("Logs", name + ".png");
            File.WriteAllBytes(path, pixels.EncodeToPNG());
            Require(File.Exists(path) && new FileInfo(path).Length > 8, "UI capture wrote a PNG file: " + path);
            Debug.Log("DESERT UI CAPTURE: " + path);
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
            Canvas.ForceUpdateCanvases();
            RenderTexture.active = previousActive;
            texture.Release();
            UnityEngine.Object.DestroyImmediate(texture);
            UnityEngine.Object.DestroyImmediate(pixels);
        }
    }

    static void ValidateTreePad(TreeManager tree)
    {
        Require(tree.spawnObjects.Count >= 4, "The lumber queue is populated.");
        tree.spawnObjects[0].tag = "NormalTree";
        var pad = new VirtualPad { Frame = new GamepadFrame { Connected = true } };
        var previous = GamepadInput.SetSourceForTests(pad);
        try
        {
            GamepadInput.AdvanceFrameForTests(0.016f);
            pad.Frame = new GamepadFrame { Connected = true, Buttons = GamepadButton.Right };
            GamepadInput.AdvanceFrameForTests(0.016f);
            tree.SendMessage("Update", SendMessageOptions.RequireReceiver);
            Require(MiniGameMgr.Instance.PendingWood == (int)LoadGameData.Instance.miniGameDatas["game2tree_norm"].GetAmount1,
                "A virtual D-pad right reaches the lumber gameplay action and grants table wood.");
            pad.Frame = new GamepadFrame { Connected = true };
            GamepadInput.AdvanceFrameForTests(0.016f);
        }
        finally { GamepadInput.SetSourceForTests(previous); }
    }

    static Button ModalButton(string label)
    {
        var button = MeadowCampaign.Instance.ModalRoot.GetComponentsInChildren<Button>(true)
            .SingleOrDefault(candidate => candidate.GetComponentInChildren<Text>().text == label);
        Require(button != null && button.interactable && button.gameObject.activeInHierarchy, "Visible actionable modal button: " + label);
        return button;
    }

    static SaveGameData VerifyDisk(MeadowOutcome outcome, int day)
    {
        ValidationSaveSession.RequireActive();
        string path = Path.Combine(SaveStoragePaths.Directory, "SaveGameData.Json");
        Require(JsonSaveStore.TryLoad(path, SaveDataManager.IsPlayableSave, out SaveGameData saved, out string error),
            "Checkpoint exists on disk: " + error);
        Require(saved.stage == 2 && saved.day == day && saved.meadowOutcome == (int)outcome &&
            saved.gems == CampGemWallet.Instance.GetAmount() &&
            saved.hero1Stat.SequenceEqual(HeroStateManager.Instance.heroStates[0].CaptureStats()),
            "Disk checkpoint preserves desert day, outcome, gems and growth.");
        return saved;
    }

    static void Require(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
    }

    static void Finish(bool success)
    {
        Debug.Log(success ? "DESERT PLAY VALIDATION PASSED" : "DESERT PLAY VALIDATION FAILED");
        SessionState.SetInt(Prefix + "ExitCode", success ? 0 : 1);
        SessionState.SetBool(Prefix + "Active", false);
        SessionState.SetBool(Prefix + "Exiting", true);
        sequence?.Dispose();
        sequence = null;
        EditorApplication.isPlaying = false;
    }

    static void Cleanup()
    {
        ValidationSaveSession.CompleteRunner(Prefix);
        EditorApplication.update -= Update;
        Application.logMessageReceived -= OnLog;
        SessionState.SetBool(Prefix + "Exiting", false);
        int code = SessionState.GetInt(Prefix + "ExitCode", 1);
        if (Application.isBatchMode) EditorApplication.Exit(code);
    }
}
