using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum FirstRunStep { Supplies, Depart, Gather, Wood, ReturnToCamp, MoveToFire, LightFire, Cook, CollectMeal, Complete }

/// <summary>Observes real gameplay; never grants resources or advances actions on the player's behalf.</summary>
[DefaultExecutionOrder(250)]
public sealed class FirstRunGuide : Singleton<FirstRunGuide>
{
    public FirstRunStep Step { get; private set; }
    public bool IsPracticeActive { get; private set; }
    public bool HoldsDayClock => IsPracticeActive;
    public Transform HighlightTarget { get; private set; }
    public static bool ReplacesLegacy => InGameMgr.IsCreateInstance() && InGameMgr.Instance.stage == 1;
    public static bool HoldClock => IsCreateInstance() && Instance.HoldsDayClock;
    public static bool SuppliesRequired => HoldClock;
    public static bool ProtectCampWork => HoldClock;

    FirstRunGuideView view;
    FireInteractionMgr fire;
    CookInteractionMgr cook;
    int initialMeals;
    bool ready, pendingSave;
    float completionMessageUntil;
    WoodManager wood;
    static readonly string[] WoodDirections = { "위 ↑ / W", "왼쪽 ← / A", "아래 ↓ / S", "오른쪽 → / D" };
    Transform lastFocusedTarget;
    readonly Transform[] supplyTargets = new Transform[2];

    public override void Init()
    {
        var saved = SaveDataManager.Instance.saveGameData;
        IsPracticeActive = saved.day == 1 && saved.meadowOutcome == 0 && !saved.onboardingComplete;
        Step = IsPracticeActive ? FirstRunStep.Supplies : FirstRunStep.Complete;
    }

    IEnumerator Start()
    {
        yield return null;
        fire = FindFirstObjectByType<FireInteractionMgr>();
        cook = FindFirstObjectByType<CookInteractionMgr>();
        initialMeals = cook != null ? cook.MealsCollected : 0;
        for (int i = 0; i < supplyTargets.Length; i++) supplyTargets[i] = FindSupplyTarget(i);
        view = gameObject.AddComponent<FirstRunGuideView>();
        view.Build(Clock.Instance.DayText.font, SkipPractice);
        view.SetExpanded(IsPracticeActive);
        ready = true;
        TickProgress();
    }

    void Update() => TickProgress();

    public void TickProgress()
    {
        if (!ready) return;
        var game = InGameMgr.Instance;
        var adventure = AdventureManager.Instance;
        if (IsPracticeActive && (Clock.Instance.day != 1 || game.IsComplete)) SkipPractice();
        if (IsPracticeActive)
        {
            bool supplied = adventure.SelectedMeat >= adventure.MinimumMeat && adventure.SelectedWater >= adventure.MinimumWater;
            switch (Step)
            {
                case FirstRunStep.Supplies:
                    if (supplied) Step = FirstRunStep.Depart;
                    break;
                case FirstRunStep.Depart:
                    if (adventure.IsJourneyInProgress && !adventure.AdvPanel.activeSelf) Step = FirstRunStep.Gather;
                    else if (adventure.IsJourneyReady && !supplied) Step = FirstRunStep.Supplies;
                    break;
                case FirstRunStep.Gather:
                    if (game.state == State.Gathering || game.state == State.MiniGame) Step = FirstRunStep.Wood;
                    break;
                case FirstRunStep.Wood:
                    if (WoodAvailable() >= fire.needAmount) Step = FirstRunStep.ReturnToCamp;
                    break;
                case FirstRunStep.ReturnToCamp:
                    if (game.state == State.Camp) Step = FirstRunStep.MoveToFire;
                    break;
                case FirstRunStep.MoveToFire:
                    if (AtFire() || fire.isActive) Step = FirstRunStep.LightFire;
                    break;
                case FirstRunStep.LightFire:
                    if (fire.isActive) Step = FirstRunStep.Cook;
                    break;
                case FirstRunStep.Cook:
                    if (cook.isActive) Step = FirstRunStep.CollectMeal;
                    break;
                case FirstRunStep.CollectMeal:
                    if (cook.MealsCollected > initialMeals) FinishPractice(true);
                    else if (!cook.isActive) Step = FirstRunStep.Cook;
                    break;
            }
            // Re-enable departure after an explicit skip or a change in the selected supplies.
            if (adventure.IsJourneyReady) adventure.StartAdvBtn.interactable = adventure.CanDepart;
        }
        RefreshContent();
        bool visible = !game.IsComplete && !game.pause.IsOpen && game.state != State.Cutscene &&
            !TutorialMgr.Instance.isTutorial && !(MeadowCampaign.IsCreateInstance() && MeadowCampaign.Instance.IsModalOpen);
        view.SetVisible(visible);
        if (visible && IsPracticeActive && GamepadInput.Connected && HighlightTarget != null &&
            HighlightTarget != lastFocusedTarget && HighlightTarget.TryGetComponent<Button>(out var button) &&
            button.IsActive() && button.IsInteractable() && EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(button.gameObject);
            lastFocusedTarget = HighlightTarget;
        }
    }

    int WoodAvailable() => ItemManager.Instance.GetAmount(InventoryRules.Wood) + MiniGameMgr.Instance.PendingWood;
    bool AtFire()
    {
        var camp = fire.CampObjMgr.GetComponent<CampObjectMgr>();
        return camp != null && camp.ObjectUI.activeInHierarchy;
    }

    public void SkipPractice()
    {
        if (!IsPracticeActive && !pendingSave) return;
        FinishPractice();
        if (AdventureManager.IsCreateInstance() && AdventureManager.Instance.IsJourneyReady)
            AdventureManager.Instance.StartAdvBtn.interactable = AdventureManager.Instance.CanDepart;
    }

    void FinishPractice(bool completedThroughPlay = false)
    {
        if (completedThroughPlay) completionMessageUntil = Time.unscaledTime + 12f;
        IsPracticeActive = false;
        Step = FirstRunStep.Complete;
        // Preserve the morning checkpoint. Saving tutorial preferences must not save a mid-journey inventory.
        var snapshot = SaveDataManager.Instance.CaptureGameData();
        snapshot.onboardingComplete = true;
        pendingSave = !SaveDataManager.Instance.TrySaveGameSnapshot(snapshot);
    }

    Transform Area(string code) => GatheringManager.Instance.gatheringObjs.FirstOrDefault(area => area.code == code)?.obj.transform;
    Transform GatheringTarget() => InGameMgr.Instance.state == State.Gathering ? Area("CS_S1_Forest") : GatheringManager.Instance.GatheringBtn.transform;
    string TravelHint => GamepadInput.Connected ? "RS로 이동 → LS/십자키로 버튼 선택 → A" : "바닥을 마우스 오른쪽 클릭해 이동한 뒤 버튼 클릭";

    Transform FindSupplyTarget(int index)
    {
        var adventure = AdventureManager.Instance;
        Transform amount = adventure.ItemNumTexts[index].transform;
        Button closest = null;
        float distance = float.PositiveInfinity;
        foreach (var button in adventure.AdvPanel.GetComponentsInChildren<Button>(true))
            for (int call = 0; call < button.onClick.GetPersistentEventCount(); call++)
                if (button.onClick.GetPersistentMethodName(call) == nameof(AdventureManager.AddItemBtn) &&
                    button.onClick.GetPersistentTarget(call) == adventure)
                {
                    float score = (button.transform.position - amount.position).sqrMagnitude;
                    if (score < distance) { distance = score; closest = button; }
                }
        return closest != null ? closest.transform : amount.parent;
    }

    void RefreshContent()
    {
        var adventure = AdventureManager.Instance;
        var state = InGameMgr.Instance.state;
        string title = "캠프 생활 안내", body = "", footer = "하루 4분 · 매일 아침 자동 저장";
        HighlightTarget = null;
        if (IsPracticeActive)
        {
            footer = "연습 중 하루 시간 정지 · 불과 음식 소진 방지";
            switch (Step)
            {
                case FirstRunStep.Supplies:
                    title = "출정할 식량을 챙겨요";
                    body = $"고기 {adventure.SelectedMeat}/{adventure.MinimumMeat} · 물 {adventure.SelectedWater}/{adventure.MinimumWater}\n\n각 자원 옆 + 버튼을 눌러 최소량을 채우세요.\n아래 예상 실패 확률이 어떻게 달라지는지 확인하세요.";
                    HighlightTarget = supplyTargets[adventure.SelectedMeat < adventure.MinimumMeat ? 0 : 1];
                    break;
                case FirstRunStep.Depart:
                    title = "준비 완료, 첫 원정!";
                    body = "최소 보급을 채웠습니다.\n평가가 ‘순탄’이면 실패 확률은 0%입니다.\n\n‘여정 출발’을 누르세요. 용사들이 떠난 동안 캠프를 준비합니다.";
                    HighlightTarget = adventure.StartAdvBtn.transform;
                    break;
                case FirstRunStep.Gather:
                    title = "직접 자원을 모아 봐요";
                    body = "화면 아래 배낭 모양의 채집 버튼을 누르세요.\n\n첫 목표는 모닥불에 쓸 나무 15개입니다. 채집 화면에서 ‘숲’을 선택하세요.";
                    HighlightTarget = GatheringTarget();
                    break;
                case FirstRunStep.Wood:
                    title = $"불에 쓸 나무 {WoodAvailable()}/{fire.needAmount}";
                    body = "숲의 나무에 홈이 파인 방향을 누르세요.\nW 위 · A 왼쪽 · S 아래 · D 오른쪽\n패드: LS/십자키\n\n틀려도 자원은 잃지 않습니다. 잠시 기다린 뒤 다시 시도하세요.";
                    HighlightTarget = state == State.Gathering ? Area("CS_S1_Forest") : state == State.Camp ? GatheringManager.Instance.GatheringBtn.transform : null;
                    if (state == State.MiniGame)
                    {
                        if (wood == null || !wood.isActiveAndEnabled) wood = FindFirstObjectByType<WoodManager>();
                        if (wood != null && wood.isActiveAndEnabled)
                        {
                            string direction = WoodDirections[Mathf.Clamp(wood.woodDirection, 0, 3)];
                            body = wood.isStun
                                ? $"방향이 달랐어요. 지금은 잠시 쉬는 중입니다.\n캐릭터가 일어나면 {direction} 방향으로 다시 시도하세요. 모은 나무는 그대로입니다."
                                : $"지금은 {direction} · 홈 방향을 한 번 누르세요. 패드: LS/십자키\n나무는 나갈 때 지급됩니다. 동굴: ↑ / 패드 A 점프 · ↓ / X 숙이기";
                        }
                        else body = "나무는 ‘숲’에서 얻습니다.\n닫기 / Esc / 패드 B로 돌아가 숲을 선택하세요.\n지금 모은 자원은 나갈 때 지급됩니다.";
                    }
                    break;
                case FirstRunStep.ReturnToCamp:
                    title = "채집한 자원을 챙겨요";
                    body = $"나무 {WoodAvailable()}개를 모았습니다!\n\n닫기 / Esc / 패드 B로 캠프로 돌아오세요.\n미니게임에서 모은 자원은 종료할 때 한 번 지급됩니다.";
                    HighlightTarget = state == State.MiniGame ? MiniGameMgr.Instance.ExitBtn.transform : GatheringManager.Instance.ExitBtn.transform;
                    break;
                case FirstRunStep.MoveToFire:
                    title = "모닥불 가까이 이동해요";
                    body = TravelHint + "\n\n빛나는 표시의 모닥불 가까이 가면 작업 버튼이 나타납니다.\n먼 곳의 작업은 먼저 걸어가야 할 수 있습니다.";
                    HighlightTarget = fire.CampObjMgr.transform;
                    break;
                case FirstRunStep.LightFire:
                    title = "모닥불을 피워요";
                    body = $"불 피우기 버튼으로 나무 {fire.needAmount}개를 사용하세요.\n\n불은 요리에 필요하고 밤의 스트레스도 막아 줍니다.\n일반 플레이에서는 불이 꺼지기 전에 연료를 보충하세요.";
                    HighlightTarget = AtFire() ? fire.ActiveButton.transform : fire.CampObjMgr.transform;
                    if (ItemManager.Instance.GetAmount(InventoryRules.Wood) < fire.needAmount)
                    { body = $"나무가 부족합니다. 숲에서 {fire.needAmount}개까지 모아 돌아오세요.\n자원은 자동으로 보충되지 않습니다."; HighlightTarget = GatheringTarget(); }
                    break;
                case FirstRunStep.Cook:
                    title = "첫 요리를 시작해요";
                    body = "모닥불의 요리 버튼을 누르세요.\n약초 2 + 고기 2 + 물 2 → 음식 3\n\n40초 동안 조리합니다. 음식은 다음 출정 때 챙겨 귀환한 용사의 체력을 회복시킵니다.";
                    HighlightTarget = AtFire() ? cook.ActiveButton.transform : fire.CampObjMgr.transform;
                    if (cook.buttonState == 2) body = "버린 음식을 먼저 정리한 뒤 다시 요리를 시작하세요.\n요리 재료는 시작할 때 소비됩니다.";
                    else if (!fire.isActive) { body = "불이 필요합니다. 나무 15개로 모닥불을 피운 뒤 요리하세요."; HighlightTarget = fire.ActiveButton.transform; }
                    else if (!HasRecipe())
                    {
                        body = $"재료가 부족합니다. 채집 후 다시 요리하세요.\n고기 {ItemManager.Instance.GetAmount(InventoryRules.Meat)}/2 · 물 {ItemManager.Instance.GetAmount(InventoryRules.Water)}/2 · 약초 {ItemManager.Instance.GetAmount(InventoryRules.Herb)}/2\n\n초원: 고기·약초 / 호수: 물\n단계와 연습 보호는 유지됩니다.";
                        HighlightTarget = GatheringManager.Instance.GatheringBtn.transform;
                        if (state == State.Gathering) HighlightTarget = Area(ItemManager.Instance.GetAmount(InventoryRules.Water) < 2 ? "CS_S1_Lake" : "CS_S1_Grassland");
                    }
                    break;
                case FirstRunStep.CollectMeal:
                    title = cook.isGrace ? "완성된 음식을 받아요" : $"요리 중 · {Mathf.CeilToInt(cook.activeTime)}초";
                    body = cook.isGrace ? "요리 버튼을 다시 눌러 음식 3개를 받으세요.\n완성만으로 가방에 들어오지는 않습니다.\n\n연습 중에는 타지 않습니다. 일반 플레이에서는 완성 후 20초 안에 받으세요."
                        : "조리가 끝나면 요리 버튼이 바뀝니다.\n기다리는 동안 채집을 더 해도 됩니다. 완성되면 이 안내가 알려 드립니다.\n\n음식은 다음 출정에 챙겨 용사들의 체력을 회복하세요.";
                    HighlightTarget = AtFire() ? cook.ActiveButton.transform : fire.CampObjMgr.transform;
                    break;
            }
        }
        else
        {
            CampGuidance(out title, out body);
            if (state == State.Gathering) { title = "무엇이 필요한가요?"; body = "숲: 나무·동굴 원석\n초원: 고기·약초\n호수: 물·물고기(고기)\n\n다음 보급과 요리 재료부터 확보하세요. 원석 3개로 생존 용사 체력을 +20 회복할 수 있습니다."; }
            if (state == State.Fence) { title = "야간 습격에 대비하기"; body = "울타리는 밤의 자원 손실을 줄입니다.\n강화에 필요한 나무와 시간을 확인하고 선택하세요.\n\n전부 투자하기 전에 모닥불 연료와 다음 날 보급을 남겨 두세요."; }
            if (state == State.Event) { title = "선택 전에 비용을 확인해요"; body = "선택지의 비용과 성공 확률을 비교하세요.\n실패해도 비용이 들 수 있습니다.\n\n내일의 최소 보급이 부족해지지 않도록 남은 자원을 확인하세요."; }
            if (state == State.Laundry) { title = "빨래로 스트레스 낮추기"; body = "1. 물을 사용해 시작\n2. 얼룩을 클릭 / 패드로 선택 후 A\n3. A·D / LB·RB를 번갈아 헹구기\n4. 화면에 표시된 버튼을 길게 눌러 말리기\n\n끝까지 완료해야 스트레스가 줄어듭니다."; }
            if (state != State.Camp && !adventure.AdvPanel.activeInHierarchy) HighlightTarget = null;
        }
        if (state == State.MiniGame && Step != FirstRunStep.Wood && Step != FirstRunStep.ReturnToCamp)
        {
            string area = MiniGameMgr.Instance.CurrentArea;
            title = "채집 조작 안내";
            body = area == "CS_S1_Forest" ? "벌목: 홈 방향 W/A/S/D 또는 LS/십자키\n동굴: ↑ / 패드 A 점프, ↓ / X 숙이기\n\n원석은 종료할 때 지급됩니다."
                : area == "CS_S1_Lake" ? "낚시: A / 패드 LB로 방향 전환\n물: ← → / RS 좌우로 균형 맞추기\n\n물은 출정·요리·빨래에 쓰입니다."
                : "약초: W/A/S/D / LS로 이동해 채집\n사냥: 마우스 조준·클릭 / RS 조준·RT 발사\n\n닫기 / Esc / 패드 B로 보상을 받고 나갑니다.";
        }
        if (state == State.Gathering)
        {
            if (IsPracticeActive && (Step == FirstRunStep.Gather || Step == FirstRunStep.Wood))
                body = "강조된 ‘숲’을 선택하세요. 홈 방향 W/A/S/D 또는 패드 LS/십자키로 나무를 모읍니다.\n나무 15개가 첫 목표입니다. 모은 자원은 채집 화면을 나갈 때 지급됩니다.";
            else
                body = "숲: 나무·원석 / 초원: 고기·약초 / 호수: 물·물고기(고기)\n필요한 재료의 장소를 선택하세요. 닫기 / Esc / 패드 B로 캠프에 돌아갑니다.";
        }
        if (IsPracticeActive && (state == State.Fence || state == State.Event || state == State.Laundry))
        {
            title = "실습은 이어서 할 수 있어요";
            body = "현재 작업을 마치거나 닫기 / Esc / 패드 B로 캠프에 돌아오세요.\n\n실습 단계와 모은 자원은 유지됩니다. 돌아오면 다음 목표를 다시 표시합니다.";
            HighlightTarget = null;
        }
        if ((state == State.MiniGame || state == State.Gathering) && cook.isGrace &&
            (!IsPracticeActive || Step == FirstRunStep.CollectMeal))
        {
            title = IsPracticeActive ? "첫 요리가 완성됐어요" : $"요리 완성 · {Mathf.CeilToInt(cook.graceTime)}초 안에 수령";
            body = "닫기 / Esc / 패드 B로 캠프로 돌아가 모닥불의 요리 버튼을 누르세요.\n" +
                (IsPracticeActive ? "음식은 직접 받아야 가방에 들어갑니다. 실습 중에는 타지 않습니다." : "음식이 타기 전에 수령하세요. 채집한 자원도 나갈 때 지급됩니다.");
            HighlightTarget = state == State.MiniGame ? MiniGameMgr.Instance.ExitBtn.transform : GatheringManager.Instance.ExitBtn.transform;
        }
        if (pendingSave) footer = "안내 완료 상태 저장 실패 · 아래 버튼으로 다시 시도";
        else if (!IsPracticeActive && adventure.IsJourneyReady) footer = "보급 준비 중 · 하루 시간 정지";
        else if (!IsPracticeActive) footer = $"하루 종료까지 {Mathf.CeilToInt(Clock.Instance.DayDurationSeconds - Clock.Instance.nowTime)}초 · 아침 자동 저장";
        string foldedLabel = IsPracticeActive ? $"실습 {(int)Step + 1}/9" : "플레이 안내";
        if (cook.isGrace) foldedLabel = "요리 완성 · 안내";
        else if (completionMessageUntil > Time.unscaledTime) foldedLabel = "실습 완료 · 안내";
        view.SetContent(IsPracticeActive ? $"첫 원정 실습 {(int)Step + 1} / 9" : "플레이 안내", title, body, footer,
            PracticeProgress(), IsPracticeActive, pendingSave, HighlightTarget,
            state == State.MiniGame || state == State.Gathering, foldedLabel);
    }

    float PracticeProgress()
    {
        if (!IsPracticeActive) return 1;
        float fraction = 0;
        var adventure = AdventureManager.Instance;
        if (Step == FirstRunStep.Supplies)
            fraction = (Mathf.Clamp01((float)adventure.SelectedMeat / Mathf.Max(1, adventure.MinimumMeat)) +
                Mathf.Clamp01((float)adventure.SelectedWater / Mathf.Max(1, adventure.MinimumWater))) * 0.5f;
        else if (Step == FirstRunStep.Wood) fraction = Mathf.Clamp01((float)WoodAvailable() / Mathf.Max(1, fire.needAmount));
        else if (Step == FirstRunStep.CollectMeal) fraction = cook.isGrace ? 0.9f : 0.8f * (1 - Mathf.Clamp01(cook.activeTime / Mathf.Max(1, cook.cookTime)));
        return ((int)Step + fraction) / 9f;
    }

    void CampGuidance(out string title, out string body)
    {
        var adventure = AdventureManager.Instance;
        if (adventure.IsJourneyReady)
        {
            title = "오늘의 보급을 준비하세요";
            body = $"고기 {adventure.SelectedMeat}/{adventure.MinimumMeat} · 물 {adventure.SelectedWater}/{adventure.MinimumWater}\n\n최소량을 채우고 평가가 ‘순탄’인지 확인하세요. 음식도 챙기면 귀환 시 생존 용사들의 체력을 회복합니다.";
            HighlightTarget = adventure.SelectedMeat < adventure.MinimumMeat ? supplyTargets[0] : adventure.SelectedWater < adventure.MinimumWater ? supplyTargets[1] : adventure.StartAdvBtn.transform;
            return;
        }
        if (cook.isGrace)
        {
            title = $"요리 완성 · {Mathf.CeilToInt(cook.graceTime)}초 안에 수령";
            body = $"모닥불에서 음식 {cook.getItemAmount}개를 받으세요.\n\n완성된 음식은 자동으로 가방에 들어오지 않습니다. 지금 수령하면 다음 출정에 사용할 수 있습니다.";
            HighlightTarget = AtFire() ? cook.ActiveButton.transform : fire.CampObjMgr.transform;
            return;
        }
        title = completionMessageUntil > Time.unscaledTime ? "첫 요리 완료! 이제 자유롭게" : "다음 원정에 대비하세요";
        body = completionMessageUntil > Time.unscaledTime ? "실습 완료! 하루 시간이 다시 흐릅니다.\n이제 불과 완성 음식의 시간을 확인하세요.\n\n" : "";
        if (LoadGameData.Instance.Catalog.TryGetStageDay(InGameMgr.Instance.stageData.StageDayGroupID, Clock.Instance.day + 1, out var nextDay) &&
            LoadGameData.Instance.journeyDatas.TryGetValue(nextDay.JourneyID, out var journey))
        {
            int meat = ItemManager.Instance.GetAmount(InventoryRules.Meat), water = ItemManager.Instance.GetAmount(InventoryRules.Water);
            body += $"내일 최소 보급: 고기 {meat}/{journey.Item1Min} · 물 {water}/{journey.Item2Min}\n";
            body += meat < journey.Item1Min ? "초원에서 고기를 먼저 모으세요." : water < journey.Item2Min ? "호수에서 물을 먼저 모으세요." : "최소 보급 확보! 요리와 야간 습격에 쓸 여유분도 남겨 두세요.";
        }
        else body += "마지막 원정입니다. 용사들의 귀환 결과를 확인하고 밤까지 캠프를 관리하세요.";
        body += $"\n모닥불 연료: 나무 {ItemManager.Instance.GetAmount(InventoryRules.Wood)}/{fire.needAmount}";
        if (Clock.Instance.nowTime >= Clock.Instance.DayDurationSeconds * 0.5f && !fire.isActive)
        {
            title = "밤이 됐어요 · 불을 확인하세요";
            body = $"나무 {fire.needAmount}개로 불을 피워 용사들의 스트레스를 막으세요.\n\n현재 나무 {ItemManager.Instance.GetAmount(InventoryRules.Wood)}개\n부족하면 숲에서 채집하세요. 내일 출정할 고기와 물도 남겨 두세요.";
            HighlightTarget = AtFire() ? fire.ActiveButton.transform : fire.CampObjMgr.transform;
        }
    }

    bool HasRecipe() => ItemManager.Instance.CanAfford(cook.needItem1ID, cook.needItem1Amount) &&
        ItemManager.Instance.CanAfford(cook.needItem2ID, cook.needItem2Amount) &&
        ItemManager.Instance.CanAfford(cook.needItem3ID, cook.needItem3Amount);
}
