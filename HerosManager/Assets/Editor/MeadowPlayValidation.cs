using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>Scene integration, not a test of human input skill or four minutes of camp balancing.</summary>
[InitializeOnLoad]
public static class MeadowPlayValidation
{
    const string Prefix = "HerosManager.MeadowPlay.";
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

    static MeadowPlayValidation()
    {
        if (SessionState.GetBool(Prefix + "Active", false) || SessionState.GetBool(Prefix + "Exiting", false))
            Attach();
    }

    [MenuItem("Tools/Heros Manager/Validate Meadow Campaign Play")]
    public static void Run()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode)
            throw new InvalidOperationException("Run meadow validation outside Play mode.");
        ValidationSaveSession.Begin(Prefix);
        Directory.CreateDirectory("Logs");
        File.WriteAllText("Logs/meadow-play-errors.txt", string.Empty);
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
        // The common storage session survives reloads independently of runner initialization.
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
        File.AppendAllText("Logs/meadow-play-errors.txt", message + "\n" + stack + "\n");
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
            Require(errors == 0, "Unity logged errors; see Logs/meadow-play-errors.txt.");
            if (sequence == null)
            {
                started = EditorApplication.timeSinceStartup;
                sequence = Campaign().GetEnumerator();
            }
            Require(EditorApplication.timeSinceStartup - started < 175, "Campaign validation exceeded 175 seconds.");
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
        return $" scene={SceneManager.GetActiveScene().name}, day={Clock.Instance.day}, meadow={MeadowCampaign.IsCreateInstance()}, " +
            $"modalBuilt={(MeadowCampaign.IsCreateInstance() && MeadowCampaign.Instance.ModalRoot != null)}, ready={adventure.IsJourneyReady}, " +
            $"clicked={adventure.isClicked}, panel={adventure.AdvPanel.activeInHierarchy}, black={adventure.BlackPanel.activeSelf}, " +
            $"tutorial={TutorialMgr.Instance.isTutorial}, savedOutcome={SaveDataManager.Instance.saveGameData.meadowOutcome}";
    }

    static Gate Until(string name, Func<bool> ready, double timeout = 15) =>
        new Gate { Name = name, Ready = ready, Timeout = timeout };

    static Gate Delay(double seconds)
    {
        double end = EditorApplication.timeSinceStartup + seconds;
        return Until("frame/animation delay", () => EditorApplication.timeSinceStartup >= end, seconds + 5);
    }

    static IEnumerable<Gate> Campaign()
    {
        yield return Until("Title scene", () => SceneManager.GetActiveScene().name == "Title" &&
            UnityEngine.Object.FindAnyObjectByType<TitleManager>() != null);
        yield return Delay(0.25);
        Require(SaveStoragePaths.IsIsolated, "Validation must never use the player's save directory.");
        var title = UnityEngine.Object.FindAnyObjectByType<TitleManager>();
        title.SendMessage("TitleEff", SendMessageOptions.RequireReceiver);
        yield return Until("Title menu callbacks", () => title.MenuReady);
        title.MenuTrans.GetChild(0).GetComponent<Button>().onClick.Invoke();
        yield return Until("Intro", () => SceneManager.GetActiveScene().name == "Intro" &&
            UnityEngine.Object.FindAnyObjectByType<IntroManager>() != null);
        yield return Delay(0.2);
        UnityEngine.Object.FindAnyObjectByType<IntroManager>().SkipBtn.onClick.Invoke();
        yield return ReadyDay(1);
        Require(Clock.Instance.day == 1 && CampGemWallet.Instance.GetAmount() == 0, "New game starts on day one without gems.");
        Require(ItemManager.Instance.GetAmount(InventoryRules.Meat) == 10 &&
            ItemManager.Instance.GetAmount(InventoryRules.Water) == 10, "First departure uses the authored initial stock.");
        float originalPower = HeroStateManager.Instance.heroStates[0].power;
        for (int day = 1; day <= 4; day++)
        {
            yield return ReadyDay(day);
            DismissTutorial();
            PrepareGoodJourney();
            int[] before = ItemManager.Instance.CaptureItems();
            int[] provisions = AdventureManager.Instance.ItemNumTexts.Select(text => int.Parse(text.text)).ToArray();
            AdventureManager.Instance.StartAdvBtn.onClick.Invoke();
            Require(AdventureManager.Instance.IsJourneyInProgress, "Departure button starts exactly one journey.");
            Require(ItemManager.Instance.GetAmount(InventoryRules.Meat) == before[2] - provisions[0] &&
                ItemManager.Instance.GetAmount(InventoryRules.Water) == before[1] - provisions[1], "Departure consumes selected supplies.");
            AdventureManager.Instance.StartAdvBtn.onClick.Invoke();
            Require(ItemManager.Instance.GetAmount(InventoryRules.Meat) == before[2] - provisions[0], "Duplicate departure cannot charge twice.");
            yield return Until("departure animation", () => !AdventureManager.Instance.AdvPanel.activeSelf);
            DismissTutorial();
            if (day == 1)
            {
                DismissTutorial();
                bool padValidated = false;
                MeadowCampaign.Instance.StartCoroutine(GamepadSceneValidation.RunAsync(() => padValidated = true));
                yield return Until("gamepad UI validation", () => padValidated);
                // Deterministic reward callbacks provide the stock for subsequent days.
                // This deliberately does not claim to solve six games through keyboard input.
                foreach (var gate in CollectCampaignSupplies()) yield return gate;
            }
            Require(AdventureManager.Instance.ResolveJourney(0.5f), "Prepared journey resolves.");
            Require(AdventureManager.Instance.LastJourneyResult.IsSuccess && !AdventureManager.Instance.ResolveJourney(0.5f),
                "Prepared journey succeeds and cannot reward twice.");
            if (day == 1)
            {
                DismissTutorial();
                var hero = HeroStateManager.Instance.heroStates[0];
                float woundedHp = hero.hp;
                int gems = CampGemWallet.Instance.GetAmount();
                Require(woundedHp < hero.MaximumHp && MeadowCampaign.Instance.CanRecover, "Returned party can use collected gems.");
                Require(MeadowCampaign.Instance.RecoverParty(), "Recovery callback succeeds.");
                Require(CampGemWallet.Instance.GetAmount() == gems - MeadowCampaignRules.RecoveryGemCost &&
                    Mathf.Approximately(hero.hp, Mathf.Min(hero.MaximumHp, woundedHp + MeadowCampaignRules.RecoveryHealth)),
                    "Recovery charges the exact gem cost and heals the survivor.");
                Require(!MeadowCampaign.Instance.RecoverParty(), "Insufficient gems/full health cannot pay for another recovery.");
            }
            DismissTutorial();
            Clock.Instance.nowTime = Clock.Instance.DayDurationSeconds;
            yield return Until("daily report", () => MeadowCampaign.Instance.IsModalOpen);
            Require(Clock.Instance.isProduct && Clock.Instance.isStop, "Report stops the current day.");
            Debug.Log($"MEADOW PLAY DAY {day}: outcome={MeadowCampaign.Instance.Outcome}, exp={HeroStateManager.Instance.heroStates[0].exp}");
            if (day < 4)
            {
                Require(MeadowCampaign.Instance.Outcome == MeadowOutcome.Active, "First three successes continue.");
                ModalButton("다음 날").onClick.Invoke();
                yield return ReadyDay(day + 1);
                VerifyDisk(MeadowOutcome.Active, day + 1);
            }
        }
        Require(MeadowCampaign.Instance.Outcome == MeadowOutcome.Victory && InGameMgr.Instance.IsComplete,
            "The fourth successful expedition ends the meadow campaign.");
        Require(HeroStateManager.Instance.heroStates.All(hero => hero.Level >= 2 && hero.exp >= 40) &&
            HeroStateManager.Instance.heroStates[0].power > originalPower, "Four successes produce saved courage and power growth.");
        SaveGameData victory = VerifyDisk(MeadowOutcome.Victory, 4);
        File.WriteAllText("Logs/meadow-victory.json", JsonUtility.ToJson(victory, true));
        Require(victory.gems > 0, "Unspent cave gems survive the terminal checkpoint.");
        ModalButton("제목으로").onClick.Invoke();
        yield return Until("return to Title", () => SceneManager.GetActiveScene().name == "Title");
        yield return Delay(0.25);
        title = UnityEngine.Object.FindAnyObjectByType<TitleManager>();
        title.SendMessage("TitleEff", SendMessageOptions.RequireReceiver);
        yield return Until("continue menu", () => title.MenuReady);
        // Also verify the authored Continue button has its runtime callback component.
        yield return Until("continue callback", () => title.MenuTrans.GetChild(1)
            .GetComponent<UnityEngine.EventSystems.EventTrigger>() != null);
        Require(title.MenuTrans.GetChild(1).GetComponent<Button>().interactable, "Saved campaign enables continue.");
        title.MenuTrans.GetChild(1).GetComponent<Button>().onClick.Invoke();
        yield return Until("restored victory screen", () => SceneManager.GetActiveScene().name == "InGame" &&
            MeadowCampaign.IsCreateInstance() && MeadowCampaign.Instance.IsModalOpen);
        Require(MeadowCampaign.Instance.Outcome == MeadowOutcome.Victory && InGameMgr.Instance.IsComplete &&
            !AdventureManager.Instance.AdvPanel.activeSelf, "Continue restores terminal UI instead of replaying day four.");
        Require(CampGemWallet.Instance.GetAmount() == victory.gems &&
            HeroStateManager.Instance.heroStates[0].exp == victory.hero1Stat[3], "Continue restores currency and growth.");
        var completed = MeadowCampaign.Instance;
        ModalButton("새 원정 · 1일부터").onClick.Invoke();
        yield return Until("restart scene", () => MeadowCampaign.IsCreateInstance() && MeadowCampaign.Instance != completed);
        yield return ReadyDay(1);
        Require(MeadowCampaign.Instance.Outcome == MeadowOutcome.Active && CampGemWallet.Instance.GetAmount() == 0 &&
            HeroStateManager.Instance.heroStates.All(hero => hero.exp == 0 && hero.hp == hero.MaximumHp),
            "Restart resets outcome, currency, health and courage.");

        for (int day = 1; day <= 3; day++)
        {
            yield return ReadyDay(day);
            DismissTutorial();
            Require(AdventureManager.Instance.ItemNumTexts.All(text => text.text == "0"), "Failure run selects no supplies.");
            AdventureManager.Instance.StartAdvBtn.onClick.Invoke();
            yield return Until("unprepared departure", () => !AdventureManager.Instance.AdvPanel.activeSelf);
            DismissTutorial();
            Require(AdventureManager.Instance.ResolveJourney(0.99f) && !AdventureManager.Instance.LastJourneyResult.IsSuccess,
                "Unprepared deterministic sample resolves as failure.");
            Clock.Instance.nowTime = Clock.Instance.DayDurationSeconds;
            yield return Until("failure report", () => MeadowCampaign.Instance.IsModalOpen);
            if (day < 3)
            {
                Require(MeadowCampaign.Instance.Outcome == MeadowOutcome.Active, "Living survivors can attempt another day.");
                ModalButton("다음 날").onClick.Invoke();
            }
        }
        Require(MeadowCampaign.Instance.Outcome == MeadowOutcome.Defeat && InGameMgr.Instance.IsComplete &&
            HeroStateManager.Instance.heroStates.All(hero => hero.hp == 0), "Three unsupported failures incapacitate the party and end the campaign.");
        SaveGameData defeat = VerifyDisk(MeadowOutcome.Defeat, 3);
        File.WriteAllText("Logs/meadow-defeat.json", JsonUtility.ToJson(defeat, true));
        Debug.Log("MEADOW PLAY SCOPE: actual scenes/buttons, deterministic reward callbacks and journey samples; clock advanced to day boundaries. Human input skill and full-duration camp balance were not tested.");
    }

    static Gate ReadyDay(int day) => Until("day " + day + " preparation", () =>
        SceneManager.GetActiveScene().name == "InGame" && MeadowCampaign.IsCreateInstance() &&
        MeadowCampaign.Instance.ModalRoot != null && Clock.Instance.day == day &&
        AdventureManager.Instance.IsJourneyReady && !AdventureManager.Instance.isClicked &&
        AdventureManager.Instance.AdvPanel.activeInHierarchy && !AdventureManager.Instance.BlackPanel.activeSelf, 20);

    static void DismissTutorial()
    {
        if (FirstRunGuide.IsCreateInstance()) FirstRunGuide.Instance.SkipPractice();
        // Completing collection guidance can enqueue camp guidance; consume that authored chain too.
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
        for (int i = 0; i < 100 && adventure.ResultImg.sprite != adventure.GoodImg; i++)
            adventure.AddItemBtn(3);
        Require(adventure.ResultImg.sprite == adventure.GoodImg && adventure.StartAdvBtn.interactable,
            "Collected stock must reach the prepared/safe journey bracket through supply controls.");
    }

    static IEnumerable<Gate> CollectCampaignSupplies()
    {
        var games = MiniGameMgr.Instance;
        games.Setup("CS_S1_Forest");
        yield return Delay(0.1);
        var cave = UnityEngine.Object.FindAnyObjectByType<CaveManager>();
        var wood = UnityEngine.Object.FindAnyObjectByType<WoodManager>();
        Require(cave != null && cave.isActiveAndEnabled && wood != null, "Forest pair is active.");
        int gemBefore = CampGemWallet.Instance.GetAmount();
        int perPickup = (int)LoadGameData.Instance.miniGameDatas["game1cage_jem"].GetJemAmount;
        for (int i = 0; i < 4; i++)
        {
            var pickup = cave.Spawn(cave.JemPrefab, cave.RealSpawnPoint, Quaternion.identity);
            Require(cave.CollectJem(pickup) && !cave.CollectJem(pickup), "Each spawned gem pays exactly once.");
        }
        Require(games.PendingGems == perPickup * 4, "Cave rewards use the table amount.");
        for (int i = 0; i < 100; i++) wood.AddItem(wood.item1ID, wood.woodNormalRes);
        games.CloseMiniGame();
        games.CloseMiniGame();
        Require(CampGemWallet.Instance.GetAmount() == gemBefore + perPickup * 4, "Duplicate close cannot duplicate gems.");
        DismissTutorial();
        games.Setup("CS_S1_Grassland");
        yield return Delay(0.1);
        Require(games.PendingGems == 0, "Re-entry resets pending gems.");
        var hunt = UnityEngine.Object.FindAnyObjectByType<HuntManager>();
        var herb = UnityEngine.Object.FindAnyObjectByType<HubManager>();
        for (int i = 0; i < 100; i++) { hunt.KillFox(); herb.GetHub(); }
        games.CloseMiniGame();
        games.Setup("CS_S1_Lake");
        yield return Delay(0.1);
        var fish = UnityEngine.Object.FindAnyObjectByType<FishManager>();
        var water = UnityEngine.Object.FindAnyObjectByType<WaterManager>();
        fish.GetFish();
        for (int i = 0; i < 100; i++) water.AddItem(water.item1ID, water.waterRes);
        games.CloseMiniGame();
        DismissTutorial();
        var cook = UnityEngine.Object.FindAnyObjectByType<CookInteractionMgr>();
        var fire = cook.FireInterMgr.GetComponent<FireInteractionMgr>();
        fire.ClickButton();
        Require(fire.isActive, "Collected wood starts the campfire.");
        int[] costsBefore = ItemManager.Instance.CaptureItems();
        cook.ClickButton();
        Require(cook.isActive && !cook.isGrace, "Cooking starts with the collected ingredients.");
        int[] costsAfter = ItemManager.Instance.CaptureItems();
        Require(costsAfter[1] == costsBefore[1] - cook.needItem3Amount &&
            costsAfter[2] == costsBefore[2] - cook.needItem2Amount &&
            costsAfter[3] == costsBefore[3] - cook.needItem1Amount, "Cooking charges the configured water, meat and herb costs.");
        cook.ClickButton();
        Require(ItemManager.Instance.CaptureItems().SequenceEqual(costsAfter), "Clicking active cooking cannot charge again.");
        cook.activeTime = 0;
        yield return Until("cooked meal", () => cook.isGrace);
        int foodBefore = ItemManager.Instance.GetAmount(InventoryRules.Food);
        cook.ClickButton();
        Require(ItemManager.Instance.GetAmount(InventoryRules.Food) == foodBefore + cook.getItemAmount,
            "Completed cooking pays food through its callback.");
        cook.SendMessage("GetFood", SendMessageOptions.RequireReceiver);
        Require(ItemManager.Instance.GetAmount(InventoryRules.Food) == foodBefore + cook.getItemAmount,
            "A duplicate completion callback cannot award another meal.");
        Debug.Log("MEADOW PLAY SUPPLIES: six minigame reward paths exercised; synthetic successful pickups fund the campaign, not an initial inventory override.");
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
        string path = Path.Combine(SaveStoragePaths.Directory, "SaveGameData.Json");
        Require(JsonSaveStore.TryLoad(path, SaveDataManager.IsPlayableSave, out SaveGameData saved, out string error),
            "Checkpoint exists on disk: " + error);
        Require(saved.stage == 1 && saved.day == day && saved.meadowOutcome == (int)outcome &&
            saved.gems == CampGemWallet.Instance.GetAmount() &&
            saved.hero1Stat.SequenceEqual(HeroStateManager.Instance.heroStates[0].CaptureStats()),
            "Disk checkpoint preserves day, outcome, gems and hero growth.");
        return saved;
    }

    static void Require(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
    }

    static void Finish(bool success)
    {
        Debug.Log(success ? "MEADOW PLAY VALIDATION PASSED" : "MEADOW PLAY VALIDATION FAILED");
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
