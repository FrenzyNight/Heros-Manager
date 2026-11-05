using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>Scene integration, not a test of human input skill or four minutes of camp balancing.</summary>
[InitializeOnLoad]
public static class GuidePlayValidation
{
    const string Prefix = "HerosManager.GuidePlay.";
    static IEnumerator<Gate> sequence;
    static Gate waiting;
    static double deadline, started;
    static int errors;
    static PlayerCtrl navigationPlayer;
    static CampObjectMgr navigationCamp;
    static Vector3 requestedApproach;

    sealed class Gate
    {
        internal Func<bool> Ready;
        internal string Name;
        internal double Timeout;
    }

    static GuidePlayValidation()
    {
        if (SessionState.GetBool(Prefix + "Active", false) || SessionState.GetBool(Prefix + "Exiting", false))
            Attach();
    }

    [MenuItem("Tools/Heros Manager/Validate First Run Guide Play")]
    public static void Run()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode)
            throw new InvalidOperationException("Run guide validation outside Play mode.");
        ValidationSaveSession.Begin(Prefix);
        Directory.CreateDirectory("Logs");
        File.WriteAllText("Logs/guide-play-errors.txt", string.Empty);
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
        File.AppendAllText("Logs/guide-play-errors.txt", message + "\n" + stack + "\n");
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
            Require(errors == 0, "Unity logged errors; see Logs/guide-play-errors.txt.");
            if (sequence == null)
            {
                started = EditorApplication.timeSinceStartup;
                sequence = Practice().GetEnumerator();
            }
            Require(EditorApplication.timeSinceStartup - started < 175, "Guide validation exceeded 175 seconds.");
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
        if (!FirstRunGuide.IsCreateInstance()) return " scene=" + SceneManager.GetActiveScene().name;
        var guide = FirstRunGuide.Instance;
        return $" scene={SceneManager.GetActiveScene().name}, step={guide.Step}, active={guide.IsPracticeActive}, " +
            $"hold={guide.HoldsDayClock}, target={guide.HighlightTarget?.name}, state={InGameMgr.Instance.state}, " +
            $"timeScale={Time.timeScale}, tutorial={TutorialMgr.Instance.isTutorial}" + NavigationDiagnostics();
    }

    static Gate Until(string name, Func<bool> ready, double timeout = 15) =>
        new Gate { Name = name, Ready = ready, Timeout = timeout };

    static Gate Delay(double seconds)
    {
        double end = EditorApplication.timeSinceStartup + seconds;
        return Until("frame/animation delay", () => EditorApplication.timeSinceStartup >= end, seconds + 5);
    }

    static IEnumerable<Gate> Practice()
    {
        yield return Until("Title", () => SceneManager.GetActiveScene().name == "Title" &&
            UnityEngine.Object.FindAnyObjectByType<TitleManager>() != null);
        yield return Delay(0.25);
        Require(SaveStoragePaths.IsIsolated, "Guide validation must use isolated saves.");
        var title = UnityEngine.Object.FindAnyObjectByType<TitleManager>();
        title.SendMessage("TitleEff", SendMessageOptions.RequireReceiver);
        yield return Until("new game menu", () => title.MenuReady);
        title.MenuTrans.GetChild(0).GetComponent<Button>().onClick.Invoke();
        yield return Until("Intro", () => SceneManager.GetActiveScene().name == "Intro" &&
            UnityEngine.Object.FindAnyObjectByType<IntroManager>() != null);
        yield return Delay(0.2);
        UnityEngine.Object.FindAnyObjectByType<IntroManager>().SkipBtn.onClick.Invoke();
        yield return GuideReady();
        var guide = FirstRunGuide.Instance;
        var adventure = AdventureManager.Instance;
        SaveGameData morning = SaveDataManager.Instance.CaptureGameData();
        Require(!morning.onboardingComplete && guide.Step == FirstRunStep.Supplies &&
            guide.HoldsDayClock && guide.HighlightTarget != null, "New players receive active supply practice and a real target.");
        var view = guide.GetComponent<FirstRunGuideView>();
        var coachCard = view.GetComponentsInChildren<Image>(true).Single(image => image.name == "Coach card");
        var pointer = view.GetComponentsInChildren<CanvasGroup>(true).Single(group => group.name == "Current objective");
        view.SetExpanded(false);
        yield return Delay(0.1);
        Require(!coachCard.gameObject.activeInHierarchy && pointer.gameObject.activeInHierarchy &&
            view.GetComponentsInChildren<Text>().Any(text => text.text == "실습 1/9"),
            "Folding the explanation retains the actionable target and current step.");
        view.SetExpanded(true);
        yield return Delay(0.1);
        var eventSystem = UnityEngine.EventSystems.EventSystem.current;
        var pointerEvent = new UnityEngine.EventSystems.PointerEventData(eventSystem) { position = coachCard.rectTransform.position };
        var hits = new List<UnityEngine.EventSystems.RaycastResult>();
        eventSystem.RaycastAll(pointerEvent, hits);
        Require(hits.Count > 0 && hits[0].gameObject == coachCard.gameObject,
            "The coach surface intercepts clicks rather than activating controls behind the card.");
        int[] inventory = ItemManager.Instance.CaptureItems();
        adventure.StartAdvBtn.onClick.Invoke();
        Require(!adventure.IsJourneyInProgress && inventory.SequenceEqual(ItemManager.Instance.CaptureItems()),
            "Invoking departure without minimum practice supplies cannot depart or spend.");

        var journey = LoadGameData.Instance.journeyDatas[Clock.Instance.stageDayData.JourneyID];
        for (int i = 0; i < journey.Item1Min; i++) adventure.AddItemBtn(0);
        for (int i = 0; i < journey.Item2Min - 1; i++) adventure.AddItemBtn(1);
        guide.TickProgress();
        adventure.StartAdvBtn.onClick.Invoke();
        Require(!adventure.IsJourneyInProgress && guide.Step == FirstRunStep.Supplies,
            "One missing water portion keeps the practice departure blocked.");
        adventure.AddItemBtn(1);
        guide.TickProgress();
        Require(guide.Step == FirstRunStep.Depart && adventure.StartAdvBtn.interactable,
            "Filling both minimums advances to the actual departure button.");
        adventure.StartAdvBtn.onClick.Invoke();
        yield return Until("departure animation", () => !adventure.AdvPanel.activeSelf);
        yield return Until("gather instruction", () => FirstRunGuide.Instance.Step == FirstRunStep.Gather);
        Require(Time.timeScale > 0, "Practice does not pause all game simulation after departure.");
        float heldTime = Clock.Instance.nowTime;
        yield return Delay(0.4);
        Require(Mathf.Approximately(Clock.Instance.nowTime, heldTime) && guide.HoldsDayClock,
            "Only the day clock remains held while camp/minigame simulation can run.");

        OpenArea("CS_S1_Grassland");
        yield return Delay(0.15);
        guide.TickProgress();
        Require(guide.Step != FirstRunStep.ReturnToCamp, "A wrong area cannot fulfill wood practice.");
        MiniGameMgr.Instance.ExitBtn.onClick.Invoke();
        yield return Delay(0.1);
        OpenArea("CS_S1_Forest");
        yield return Until("wood practice", () => guide.Step == FirstRunStep.Wood);
        int woodBefore = ItemManager.Instance.GetAmount(InventoryRules.Wood);
        // Early exit must not grant wood or falsely complete the objective.
        MiniGameMgr.Instance.ExitBtn.onClick.Invoke();
        yield return Delay(0.1);
        guide.TickProgress();
        Require(ItemManager.Instance.GetAmount(InventoryRules.Wood) == woodBefore &&
            guide.Step != FirstRunStep.MoveToFire && guide.Step != FirstRunStep.LightFire,
            "An early close can be retried without granting or completing the wood objective.");
        OpenArea("CS_S1_Forest");
        yield return Until("reopened forest", () => InGameMgr.Instance.state == State.MiniGame);
        yield return Delay(0.1);
        var wood = UnityEngine.Object.FindAnyObjectByType<WoodManager>();
        Require(wood != null && wood.isActiveAndEnabled, "The forest wood controller is live.");
        wood.SendMessage("CheckWoodDirection", (wood.woodDirection + 1) % 4, SendMessageOptions.RequireReceiver);
        guide.TickProgress();
        Require(wood.isStun && view.GetComponentsInChildren<Text>().Any(text => text.text.Contains("잠시 쉬는 중")),
            "Wrong wood input explains the temporary wait without losing progress.");
        yield return Until("wood recovery", () => !wood.isStun);
        for (int i = 0; i < 15; i++)
            wood.SendMessage("CheckWoodDirection", wood.woodDirection, SendMessageOptions.RequireReceiver);
        guide.TickProgress();
        Require(guide.Step == FirstRunStep.ReturnToCamp && ItemManager.Instance.GetAmount(InventoryRules.Wood) == woodBefore,
            "Wood earned in the live minigame advances guidance before session payout.");
        MiniGameMgr.Instance.ExitBtn.onClick.Invoke();
        yield return Until("return to camp", () => guide.Step == FirstRunStep.MoveToFire || guide.Step == FirstRunStep.LightFire);
        Require(ItemManager.Instance.GetAmount(InventoryRules.Wood) >= woodBefore + 15,
            "Closing the forest pays earned wood into the real inventory.");
        var fire = UnityEngine.Object.FindAnyObjectByType<FireInteractionMgr>(FindObjectsInactive.Include);
        var cook = UnityEngine.Object.FindAnyObjectByType<CookInteractionMgr>(FindObjectsInactive.Include);
        Require(fire != null && cook != null, "The authored fire and cooking controls exist.");
        var camp = fire.CampObjMgr.GetComponent<CampObjectMgr>();
        var player = UnityEngine.Object.FindAnyObjectByType<PlayerCtrl>();
        Require(player != null && camp != null, "Player navigation and camp trigger are connected.");
        WalkToCamp(player, camp);
        yield return Until("walk to the campfire", () => camp.ObjectUI.activeInHierarchy && guide.Step == FirstRunStep.LightFire, 20);
        int beforeFire = ItemManager.Instance.GetAmount(fire.NeedItemID);
        fire.ActiveButton.GetComponent<Button>().onClick.Invoke();
        yield return Until("lit campfire", () => fire.isActive && guide.Step == FirstRunStep.Cook);
        Require(ItemManager.Instance.GetAmount(fire.NeedItemID) == beforeFire - fire.needAmount,
            "Practice lights the fire through the real paid action.");

        // Spend away a required ingredient to exercise the recoverable failure path.
        int herbs = ItemManager.Instance.GetAmount(cook.needItem1ID);
        Require(ItemManager.Instance.TrySpend(cook.needItem1ID, herbs), "Fault injection removes only the selected ingredient.");
        int[] shortageInventory = ItemManager.Instance.CaptureItems();
        int meals = cook.MealsCollected;
        cook.ClickButton();
        guide.TickProgress();
        Require(!cook.isActive && guide.Step == FirstRunStep.Cook && guide.IsPracticeActive &&
            shortageInventory.SequenceEqual(ItemManager.Instance.CaptureItems()) && cook.MealsCollected == meals,
            "Missing ingredients preserve the step and do not partially spend or award a meal.");
        Require(guide.HighlightTarget != null && guide.HoldsDayClock, "Shortage recovery retains an actionable target and day-clock protection.");
        float protectedFireTime = fire.activeTime;
        yield return Delay(0.2);
        Require(ItemManager.Instance.GetAmount(cook.needItem1ID) == 0, "The guide does not mint replacement ingredients.");
        Require(Mathf.Approximately(fire.activeTime, protectedFireTime), "Practice protects campfire duration while recovering missing ingredients.");
        OpenArea("CS_S1_Grassland");
        yield return Delay(0.1);
        var herb = UnityEngine.Object.FindAnyObjectByType<HubManager>();
        Require(herb != null && herb.isActiveAndEnabled, "Ingredient recovery enters the actual herb minigame.");
        for (int i = 0; i < cook.needItem1Amount; i++) herb.GetHub();
        MiniGameMgr.Instance.ExitBtn.onClick.Invoke();
        yield return Delay(0.1);
        guide.TickProgress();
        Require(guide.Step == FirstRunStep.Cook && ItemManager.Instance.GetAmount(cook.needItem1ID) >= cook.needItem1Amount,
            "Earned ingredient payout resumes the same cooking step.");
        WalkToCamp(player, camp);
        yield return Until("cooking control visible", () => camp.ObjectUI.activeInHierarchy);
        cook.ActiveButton.GetComponent<Button>().onClick.Invoke();
        yield return Until("cooking started", () => cook.isActive && !cook.isGrace);
        Require(cook.MealsCollected == meals && guide.IsPracticeActive, "Starting cooking alone cannot complete practice.");
        OpenArea("CS_S1_Forest");
        cook.activeTime = 0;
        yield return Until("meal ready", () => cook.isGrace && guide.Step == FirstRunStep.CollectMeal);
        guide.TickProgress();
        Require(guide.HighlightTarget == MiniGameMgr.Instance.ExitBtn.transform &&
            view.GetComponentsInChildren<Text>().Any(text => text.text.Contains("첫 요리가 완성")),
            "A meal finished during gathering guides the player back to camp.");
        MiniGameMgr.Instance.ExitBtn.onClick.Invoke();
        yield return Delay(0.1);
        float protectedMealTime = cook.graceTime;
        yield return Delay(0.2);
        Require(Mathf.Approximately(cook.graceTime, protectedMealTime), "Practice protects the ready meal while waiting for collection.");
        int foodBefore = ItemManager.Instance.GetAmount(cook.getItemID);
        cook.ActiveButton.GetComponent<Button>().onClick.Invoke();
        yield return Until("practice completed", () => !guide.IsPracticeActive && guide.Step == FirstRunStep.Complete);
        Require(cook.MealsCollected == meals + cook.getItemAmount && ItemManager.Instance.GetAmount(cook.getItemID) == foodBefore + cook.getItemAmount,
            "Only the actual collection callback completes practice and grants food.");
        Require(!guide.HoldsDayClock, "Completing practice releases its clock hold.");
        VerifyMorningMetadata(morning);
        float releasedTime = Clock.Instance.nowTime;
        yield return Delay(0.25);
        Require(Clock.Instance.nowTime > releasedTime, "The day clock resumes after practice completion.");
        Require(view.GetComponentsInChildren<Text>().Any(text => text.text.Contains("내일 최소 보급")),
            "Completing practice hands off to current inventory and tomorrow's supplies.");

        SceneManager.LoadScene("Title");
        yield return Until("Title after completion", () => SceneManager.GetActiveScene().name == "Title");
        yield return Delay(0.25);
        title = UnityEngine.Object.FindAnyObjectByType<TitleManager>();
        title.SendMessage("TitleEff", SendMessageOptions.RequireReceiver);
        yield return Until("continue callback", () => title.MenuTrans.GetChild(1)
            .GetComponent<UnityEngine.EventSystems.EventTrigger>() != null);
        title.MenuTrans.GetChild(1).GetComponent<Button>().onClick.Invoke();
        yield return Until("completed onboarding restored", () => SceneManager.GetActiveScene().name == "InGame" &&
            FirstRunGuide.IsCreateInstance() && AdventureManager.Instance.IsJourneyReady);
        yield return Delay(0.2);
        Require(!FirstRunGuide.Instance.IsPracticeActive && SaveDataManager.Instance.saveGameData.onboardingComplete,
            "Continuing a completed practice does not start it again.");
        Require(morning.items.SequenceEqual(ItemManager.Instance.CaptureItems()),
            "Continuing returns to the morning inventory rather than a partial tutorial checkpoint.");

        // The independent skip case starts from a genuinely new save and scene.
        Require(SaveDataManager.Instance.TrySaveGameSnapshot(SaveDataManager.Instance.CreateNewGameData()), "Fresh skip case saves.");
        var previous = FirstRunGuide.Instance;
        SceneManager.LoadScene("InGame");
        yield return Until("new guide instance", () => FirstRunGuide.IsCreateInstance() && FirstRunGuide.Instance != previous);
        yield return GuideReady();
        morning = SaveDataManager.Instance.CaptureGameData();
        guide = FirstRunGuide.Instance;
        guide.SkipPractice();
        guide.SkipPractice();
        Require(!guide.IsPracticeActive && !guide.HoldsDayClock, "Repeated skip safely releases practice.");
        VerifyMorningMetadata(morning);
        previous = guide;
        SceneManager.LoadScene("InGame");
        yield return Until("skip restored", () => FirstRunGuide.IsCreateInstance() && FirstRunGuide.Instance != previous);
        yield return Delay(0.25);
        Require(!FirstRunGuide.Instance.IsPracticeActive && SaveDataManager.Instance.saveGameData.onboardingComplete,
            "Skipped practice stays skipped after scene re-entry.");
        Debug.Log("GUIDE PLAY SCOPE: scene callbacks, correct wood-direction callbacks, NavMesh movement and collection boundaries; human input skill and 40 real seconds of cooking were not tested.");
    }

    static Gate GuideReady() => Until("first-run practice", () =>
        SceneManager.GetActiveScene().name == "InGame" && FirstRunGuide.IsCreateInstance() &&
        FirstRunGuide.Instance.IsPracticeActive && FirstRunGuide.Instance.HighlightTarget != null && AdventureManager.Instance.IsJourneyReady &&
        !AdventureManager.Instance.isClicked && !AdventureManager.Instance.BlackPanel.activeSelf);

    static void OpenArea(string id)
    {
        var gather = GatheringManager.Instance;
        if (!gather.GatheringPanel.activeSelf) gather.GatheringBtn.onClick.Invoke();
        var area = gather.gatheringObjs.Single(entry => entry.code == id);
        Require(area.obj.activeInHierarchy, "Requested gathering area is visible: " + id);
        area.obj.GetComponent<Button>().onClick.Invoke();
        Require(InGameMgr.Instance.state == State.MiniGame, "Gathering area opens its real minigame: " + id);
    }

    static void VerifyMorningMetadata(SaveGameData morning)
    {
        string path = Path.Combine(SaveStoragePaths.Directory, "SaveGameData.Json");
        Require(JsonSaveStore.TryLoad(path, SaveDataManager.IsPlayableSave, out SaveGameData saved, out string error),
            "Practice metadata was written to disk: " + error);
        Require(saved.onboardingComplete && saved.stage == morning.stage && saved.day == morning.day &&
            saved.items.SequenceEqual(morning.items) && saved.gems == morning.gems &&
            SameStats(saved.hero1Stat, morning.hero1Stat) && SameStats(saved.hero2Stat, morning.hero2Stat) &&
            SameStats(saved.hero3Stat, morning.hero3Stat) && SameStats(saved.hero4Stat, morning.hero4Stat),
            "Onboarding persistence updates metadata without committing a partial day's resources or hero state.");
    }

    static bool SameStats(float[] left, float[] right) =>
        (left == null || left.Length == 0) && (right == null || right.Length == 0) ||
        left != null && right != null && left.SequenceEqual(right);

    static void WalkToCamp(PlayerCtrl player, CampObjectMgr camp)
    {
        navigationPlayer = player;
        navigationCamp = camp;
        var agent = player.GetComponent<NavMeshAgent>();
        Require(agent != null && agent.enabled && agent.isOnNavMesh, "Camp approach requires a live player agent." + NavigationDiagnostics());
        var triggers = camp.GetComponentsInChildren<Collider>(true)
            .Where(collider => collider.enabled && collider.gameObject.activeInHierarchy && collider.isTrigger &&
                collider.CompareTag("CampObject") && collider.GetComponentInParent<CampObjectMgr>() == camp).ToArray();
        var playerColliders = player.GetComponentsInChildren<Collider>(true)
            .Where(collider => collider.enabled && collider.gameObject.activeInHierarchy).ToArray();
        Require(triggers.Length > 0 && playerColliders.Length > 0, "Camp/player collision shapes must be connected." + NavigationDiagnostics());
        float bestDistance = float.PositiveInfinity;
        int sampled = 0, complete = 0, overlaps = 0;
        var path = new NavMeshPath();
        foreach (var trigger in triggers)
        {
            // This scene's Fire object origin is far from its offset collider center.
            // Pick a real floor point inside the interaction envelope, not the decorative mesh/origin.
            Bounds bounds = trigger.bounds;
            for (int x = -3; x <= 3; x++)
            for (int z = -3; z <= 3; z++)
            {
                Vector3 target = new Vector3(bounds.center.x + bounds.extents.x * x / 4f,
                    bounds.center.y, bounds.center.z + bounds.extents.z * z / 4f);
                float searchRadius = Mathf.Max(1f, bounds.extents.y + agent.height);
                if (!NavMesh.SamplePosition(target, out var hit, searchRadius, agent.areaMask)) continue;
                sampled++;
                if (!agent.CalculatePath(hit.position, path) || path.status != NavMeshPathStatus.PathComplete) continue;
                complete++;
                Vector3 offset = hit.position - player.transform.position;
                bool reachesTrigger = playerColliders.Any(collider =>
                    !Physics.GetIgnoreLayerCollision(collider.gameObject.layer, trigger.gameObject.layer) &&
                    !Physics.GetIgnoreCollision(collider, trigger) &&
                    Physics.ComputePenetration(collider, collider.transform.position + offset, collider.transform.rotation,
                        trigger, trigger.transform.position, trigger.transform.rotation, out _, out float depth) &&
                    depth > agent.stoppingDistance + 0.2f);
                if (!reachesTrigger) continue;
                overlaps++;
                float distance = 0;
                Vector3 previous = player.transform.position;
                foreach (Vector3 corner in path.corners) { distance += Vector3.Distance(previous, corner); previous = corner; }
                if (distance >= bestDistance) continue;
                bestDistance = distance;
                requestedApproach = hit.position;
            }
        }
        Debug.Log($"GUIDE CAMP APPROACH: samples={sampled}, completePaths={complete}, overlappingTargets={overlaps}, " +
            $"chosen={requestedApproach:F3}, pathLength={bestDistance:F2}" + NavigationDiagnostics());
        Require(!float.IsPositiveInfinity(bestDistance), "No reachable floor point overlaps the camp trigger; inspect scene geometry/trigger placement." + NavigationDiagnostics());
        // Use the gameplay movement callback. Do not warp the actor or force OpenUI.
        player.SendMessage("MoveTo", requestedApproach, SendMessageOptions.RequireReceiver);
        Debug.Log("GUIDE CAMP MOVE REQUESTED" + NavigationDiagnostics());
    }

    static string NavigationDiagnostics()
    {
        if (navigationPlayer == null || navigationCamp == null) return "";
        var player = navigationPlayer;
        var camp = navigationCamp;
        var agent = player.GetComponent<NavMeshAgent>();
        string agentState = agent == null ? "missing agent" :
            $"enabled={agent.enabled}, onMesh={agent.isOnNavMesh}, position={player.transform.position:F3}";
        if (agent != null && agent.enabled && agent.isOnNavMesh)
            agentState += $", destination={agent.destination:F3}, next={agent.nextPosition:F3}, pending={agent.pathPending}, " +
                $"status={agent.pathStatus}, hasPath={agent.hasPath}, remaining={agent.remainingDistance:F3}, " +
                $"velocity={agent.velocity:F3}, stopped={agent.isStopped}, stopDistance={agent.stoppingDistance:F3}";
        string shapes = string.Join("; ", camp.GetComponentsInChildren<Collider>(true)
            .Select(collider => ColliderDescription("camp", collider)).Concat(player.GetComponentsInChildren<Collider>(true)
            .Select(collider => ColliderDescription("player", collider))));
        return $"\nNavigation: requested={requestedApproach:F3}, {agentState}; campOrigin={camp.transform.position:F3}, " +
            $"UIactive={camp.ObjectUI.activeInHierarchy}\nCollision shapes: {shapes}";
    }

    static string ColliderDescription(string owner, Collider collider)
    {
        Rigidbody body = collider.attachedRigidbody;
        string rigid = body == null ? "no Rigidbody" :
            $"body={body.name}, kinematic={body.isKinematic}, detectCollisions={body.detectCollisions}, position={body.position:F3}";
        return $"{owner}/{collider.name}:{collider.GetType().Name}, tag={collider.tag}, layer={collider.gameObject.layer}, " +
            $"enabled={collider.enabled}, active={collider.gameObject.activeInHierarchy}, trigger={collider.isTrigger}, " +
            $"center={collider.bounds.center:F3}, size={collider.bounds.size:F3}, {rigid}";
    }

    static void Require(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
    }

    static void Finish(bool success)
    {
        Debug.Log(success ? "GUIDE PLAY VALIDATION PASSED" : "GUIDE PLAY VALIDATION FAILED");
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
