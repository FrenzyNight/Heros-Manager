using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[DefaultExecutionOrder(200)]
public sealed class MeadowCampaign : Singleton<MeadowCampaign>
{
    public MeadowOutcome Outcome { get; private set; }
    public bool IsModalOpen => modal != null && modal.activeSelf;
    public GameObject ModalRoot => modal;
    bool IsDesert => InGameMgr.Instance.stage == CampaignProgressionRules.DesertStage;
    string ChapterName => IsDesert ? "사막" : "초원";
    string DayObjective => !IsDesert ? "목표: 4일째 원정 성공 · 전원 행동 불능이면 패배" :
        Clock.Instance.day == 1 ? "오아시스 길 찾기 · 호수에서 내일의 물을 확보하세요" :
        Clock.Instance.day == 2 ? "유적 보급로 개척 · 미로 원석과 약초로 회복을 준비하세요" :
        Clock.Instance.day == 3 ? "모래바람 돌파 · 요리와 용사 성장으로 마지막 보급을 준비하세요" :
        "목표: 사막 횡단 원정 성공 · 생존 용사 1명 이상";
    public bool CanRecover => Outcome == MeadowOutcome.Active && !Clock.Instance.isProduct &&
        !AdventureManager.Instance.IsJourneyInProgress &&
        HeroStateManager.Instance.heroStates.Any(hero => hero.IsAbleToAct && hero.hp < hero.MaximumHp) &&
        CampGemWallet.Instance.GetAmount() >= MeadowCampaignRules.RecoveryGemCost;

    GameObject hud, modal;
    Text status, report, heading, gemHint;
    Button recovery, finish, primary, secondary;
    Font font;
    RectTransform canvasRect, cardRect;
    ScrollRect reportScroll;
    Vector2 lastCanvasSize;
    int[] morningItems;
    float[][] morningHeroes;
    int morningGems;
    bool saveFailed, leaving, helpOpen, previousClockStop, initialized;
    float previousTimeScale;

    System.Collections.IEnumerator Start()
    {
        // Added during InGameMgr.Awake: runtime-added components can start before
        // scene-owned managers despite their execution-order attributes.
        yield return null;
        BuildUi();
        initialized = true;
        Outcome = (MeadowOutcome)SaveDataManager.Instance.saveGameData.meadowOutcome;
        if (Outcome != MeadowOutcome.Active)
        {
            InGameMgr.Instance.FinishMeadow();
            AdventureManager.Instance.AdvPanel.SetActive(false);
            AdventureManager.Instance.BlackPanel.SetActive(false);
            TutorialMgr.Instance.CloseTutorial();
            ShowTerminal(true);
        }
        else if (!HeroStateManager.Instance.HasAbleHeroes)
        {
            Outcome = MeadowOutcome.Defeat;
            InGameMgr.Instance.FinishMeadow();
            saveFailed = !InGameMgr.Instance.SaveStageData();
            ShowTerminal(false);
        }
    }

    public void BeginDay()
    {
        morningItems = ItemManager.Instance.CaptureItems();
        morningHeroes = HeroStateManager.Instance.heroStates.Select(hero => hero.CaptureStats()).ToArray();
        morningGems = CampGemWallet.Instance.GetAmount();
    }

    void Update()
    {
        if (!initialized || hud == null) return;
        UpdateUiLayout();
        if (Outcome == MeadowOutcome.Active && !Clock.Instance.isProduct &&
            !HeroStateManager.Instance.HasAbleHeroes && AdventureManager.Instance.HasJourneyResult)
        {
            CloseHelp();
            Clock.Instance.TryFinishDay();
        }
        gemHint.gameObject.SetActive(InGameMgr.Instance.state == State.MiniGame && !TutorialMgr.Instance.isTutorial);
        if (gemHint.gameObject.activeSelf)
            gemHint.text = $"채집 원석 +{MiniGameMgr.Instance.PendingGems} · 나갈 때 지급 · 3개로 생존 용사 체력 +20";
        bool tutorial = TutorialMgr.Instance.isTutorial;
        bool camp = InGameMgr.Instance.state == State.Camp;
        bool preparing = AdventureManager.Instance.AdvPanel.activeInHierarchy;
        hud.SetActive(!IsModalOpen && !tutorial && !InGameMgr.Instance.pause.IsOpen &&
            Outcome == MeadowOutcome.Active && (camp || preparing));
        if (!hud.activeSelf) return;
        status.text = $"{ChapterName} {Clock.Instance.day} / 4일  ·  원석 {CampGemWallet.Instance.GetAmount()}\n" + DayObjective;
        recovery.interactable = CanRecover;
        finish.interactable = true;
    }

    void ShowHelp()
    {
        if (IsModalOpen) return;
        helpOpen = true;
        previousClockStop = Clock.Instance.isStop;
        previousTimeScale = Time.timeScale;
        Clock.Instance.isStop = true;
        Time.timeScale = 0;
        heading.text = $"{ChapterName} 4일 원정 안내";
        report.text = "매일 보급 준비 → 원정과 채집 → 귀환 → 캠프 관리 → 하루 결산\n\n" +
            "승리: 4일째 원정 성공 + 생존 용사 1명 이상\n패배: 전원 체력 0 또는 4일째 원정 실패\n\n" +
            "고기·물 최소량을 지키고 평가를 ‘순탄’으로 맞추면 실패 0%.\n성공 시 용사도 +10 이상, 용사도 25마다 전투력 +15.\n실패 시 체력 -40. 체력 0인 용사는 출전·회복 불가.\n\n" +
            "요리: 약초 2 + 고기 2 + 물 2 → 음식 3 (불 필요)\n보급 음식 1개당 귀환 체력 +2/명, 최대 +12. 치명상은 회복 불가.\n동굴 원석 3개 → 원정 전·귀환 후 생존 용사 전원 체력 +20.\n\n" +
            "하루는 4분입니다. 밤에는 불과 빨래를 관리해 스트레스를 줄이세요.\n매일 아침 자동 저장 · 진행 중 종료하면 그날 아침부터 재개합니다.";
        if (IsDesert)
            report.text = "초원의 전투력·용사도·자원·울타리를 이어받았습니다.\n" +
                "물·고기 최소 보급은 매일 9 / 10 / 11 / 12개씩입니다.\n" +
                "호수: 물·고기 / 사막 들판: 약초·사냥 / 유적: 나무·원석.\n" +
                "오아시스는 매일 잠깐 열립니다. 기본 채집만으로도 진행할 수 있습니다.\n" +
                "미로 보물마다 약초 2 + 원석 1. 원석 3개로 부상자를 회복하세요.\n\n" + report.text.Replace("동굴 원석", "미로 원석");
        ConfigureButton(primary, "확인", () => { CloseHelp(); });
        ConfigureButton(secondary, "돌아가기", () => { CloseHelp(); });
        OpenModal();
    }

    public bool CloseHelp()
    {
        if (!helpOpen) return false;
        helpOpen = false;
        modal.SetActive(false);
        Clock.Instance.isStop = previousClockStop;
        Time.timeScale = previousTimeScale;
        return true;
    }

    public bool RecoverParty()
    {
        if (!CanRecover || IsModalOpen || TutorialMgr.Instance.isTutorial) return false;
        if (!CampGemWallet.Instance.TrySpend(MeadowCampaignRules.RecoveryGemCost)) return false;
        foreach (var hero in HeroStateManager.Instance.heroStates)
            if (hero.IsAbleToAct) hero.AddStat(0, 0, MeadowCampaignRules.RecoveryHealth, 0);
        GamepadInput.Pulse(0.2f, 0.4f, 0.15f);
        return true;
    }

    public void ShowDayReport()
    {
        if (IsModalOpen || Outcome != MeadowOutcome.Active) return;
        var adventure = AdventureManager.Instance;
        Outcome = MeadowCampaignRules.Evaluate(Clock.Instance.day, HeroStateManager.Instance.AbleHeroCount,
            adventure.HasJourneyResult, adventure.LastJourneyResult.IsSuccess);
        Clock.Instance.isStop = true;
        Time.timeScale = 0;
        if (Outcome != MeadowOutcome.Active)
        {
            InGameMgr.Instance.FinishMeadow();
            saveFailed = !InGameMgr.Instance.SaveStageData();
            ShowTerminal(false);
            GamepadInput.Pulse(Outcome == MeadowOutcome.Victory ? 0.35f : 0.65f, 0.4f, 0.4f);
            return;
        }
        heading.text = $"{ChapterName} {Clock.Instance.day}일 결산";
        report.text = BuildReport() + "\n다음 날을 누르면 아침 상태가 저장됩니다.\n남은 작업은 종료되며, 보급을 다시 준비합니다.";
        ConfigureButton(primary, "다음 날", ContinueDay);
        ConfigureButton(secondary, "제목으로 · 아침부터 재개", ReturnToTitle);
        OpenModal();
    }

    void ContinueDay()
    {
        if (leaving || Outcome != MeadowOutcome.Active) return;
        leaving = true;
        modal.SetActive(false);
        ProductMgr.Instance.UIOutProduct();
        leaving = false;
    }

    void ShowTerminal(bool restored)
    {
        heading.text = Outcome == MeadowOutcome.Victory ? $"{ChapterName} 원정 완료" : $"{ChapterName} 원정 실패";
        string reason = Outcome == MeadowOutcome.Victory
            ? IsDesert ? "사막의 보급로를 열고 4일간의 횡단을 마쳤습니다." : "4일째 원정을 성공하고 초원을 지켜냈습니다."
            : HeroStateManager.Instance.HasAbleHeroes ? "4일째 원정에 실패했습니다. 보급과 성장을 준비해 다시 도전하세요."
            : "모든 용사가 행동 불능이 되었습니다. 음식과 원석 회복으로 부상을 관리해 주세요.";
        report.text = reason + "\n\n" + (restored ? PartySummary() : BuildReport());
        if (Outcome == MeadowOutcome.Victory && !IsDesert)
            report.text += "\n다음 지역: 사막 4일 · 성장·자원·울타리 계승\n" +
                "출발 보급: 물 12 · 고기 12 · 음식 6 · 원석 3 (1회)\n" +
                "이동 중 생존 용사 체력 +30 · 행동 불능 용사는 부활하지 않습니다.\n" +
                "사막 첫날 보급은 물·고기 각각 9개. 음식으로 평가와 귀환 체력을 보완하세요.\n" +
                "생존자가 적으면 전투력이 부족할 수 있습니다. 출발 전 보급 평가를 확인하세요.";
        else if (Outcome == MeadowOutcome.Victory)
            report.text += "\n초원부터 사막까지 8일 원정을 완수했습니다.\n현재 개발된 지역은 여기까지입니다. 마법 숲은 아직 열리지 않습니다.";
        if (saveFailed) report.text += "\n저장 실패: 제목으로 돌아가기 전에 아래 버튼으로 다시 저장해 주세요.";
        if (saveFailed) ConfigureButton(primary, "저장 다시 시도", RetrySave);
        else if (Outcome == MeadowOutcome.Victory && !IsDesert)
            ConfigureButton(primary, "사막으로 이어가기", () => { AdvanceToDesert(); });
        else ConfigureButton(primary, "새 원정 · 1일부터", () => { Restart(); });
        ConfigureButton(secondary, "제목으로", ReturnToTitle);
        secondary.interactable = !saveFailed;
        OpenModal();
    }

    public void ShowSaveFailure()
    {
        saveFailed = true;
        Clock.Instance.isStop = true;
        Time.timeScale = 0;
        heading.text = "진행을 저장하지 못했습니다";
        report.text = "현재 플레이 상태는 유지 중입니다. 저장 위치와 디스크 공간을 확인한 뒤 다시 시도해 주세요.";
        ConfigureButton(primary, "저장 다시 시도", RetrySave);
        ConfigureButton(secondary, "저장 없이 계속", ContinueWithoutSave);
        OpenModal();
    }

    void RetrySave()
    {
        if (!InGameMgr.Instance.SaveStageData()) return;
        saveFailed = false;
        if (Outcome != MeadowOutcome.Active) ShowTerminal(true);
        else ContinueWithoutSave();
    }

    void ContinueWithoutSave()
    {
        saveFailed = false;
        modal.SetActive(false);
        Clock.Instance.isStop = TutorialMgr.Instance.isTutorial;
        // The next day's preparation screen owns timeScale until departure.
    }

    public bool Restart()
    {
        if (leaving) return false;
        if (!SaveDataManager.Instance.TrySaveGameSnapshot(SaveDataManager.Instance.CreateNewGameData()))
        {
            report.text += "\n새 원정을 저장하지 못했습니다. 현재 결과는 유지됩니다.";
            return false;
        }
        leaving = true;
        Time.timeScale = 1;
        SceneManager.LoadScene("InGame");
        return true;
    }

    public bool AdvanceToDesert()
    {
        if (leaving || saveFailed || Outcome != MeadowOutcome.Victory || IsDesert) return false;
        var saves = SaveDataManager.Instance;
        if (!CampaignProgressionRules.TryAdvanceToDesert(saves.CaptureGameData(), out var next)) return false;
        if (!saves.TrySaveGameSnapshot(next))
        {
            report.text = "사막 출발 상태를 저장하지 못했습니다. 초원 완료 기록과 보상은 그대로 유지됩니다.\n" +
                "저장 위치와 디스크 공간을 확인한 뒤 다시 시도해 주세요.";
            return false;
        }
        leaving = true;
        Time.timeScale = 1;
        SceneManager.LoadScene("InGame");
        return true;
    }

    void ReturnToTitle()
    {
        if (leaving || saveFailed) return;
        leaving = true;
        Time.timeScale = 1;
        SceneManager.LoadScene("Title");
    }

    string BuildReport()
    {
        var result = AdventureManager.Instance.LastJourneyResult;
        var text = new StringBuilder(result.IsPerfect ? "원정: 대성공\n" : result.IsSuccess ? "원정: 성공\n" : "원정: 실패\n");
        string[] names = { "나무", "물", "고기", "약초", "음식" };
        int[] current = ItemManager.Instance.CaptureItems();
        text.Append("하루 자원 변화 (보급·작업·침입 포함)\n");
        for (int i = 0; i < current.Length; i++)
            text.Append($"{names[i]} {Signed(current[i] - (morningItems?[i] ?? current[i]))}  ");
        text.Append($"\n원석 {Signed(CampGemWallet.Instance.GetAmount() - morningGems)} · 보유 {CampGemWallet.Instance.GetAmount()}\n\n");
        text.Append(PartySummary());
        if (Clock.Instance.day < MeadowCampaignRules.FinalDay)
            text.Append("\n음식은 귀환 체력을 회복합니다. 원석 3개로 생존 용사 전원 체력 +20.\n보급 평가 ‘순탄’은 실패 0%. 용사도 25마다 전투력 +15.");
        return text.ToString();
    }

    string PartySummary()
    {
        var text = new StringBuilder();
        var heroes = HeroStateManager.Instance.heroStates;
        for (int i = 0; i < heroes.Length; i++)
        {
            var hero = heroes[i];
            string delta = morningHeroes != null && i < morningHeroes.Length ? $" ({Signed(hero.hp - morningHeroes[i][2])})" : "";
            text.Append($"{hero.NameText.text} · Lv.{hero.Level}  체력 {hero.hp:0}{delta}  전투력 {hero.power:0}  용사도 {hero.exp:0}\n");
        }
        return text.ToString();
    }

    static string Signed(float value) => value.ToString("+0;-0;0");

    void BuildUi()
    {
        font = Clock.Instance.DayText.font;
        var canvasObject = new GameObject("Meadow campaign UI", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        canvasObject.transform.SetParent(transform, false);
        var canvas = canvasObject.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 1000;
        var scaler = canvasObject.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;
        canvasRect = (RectTransform)canvas.transform;
        hud = Panel(canvas.transform, "Meadow objective", GameUiStyle.Surface, new Vector2(800, 156));
        GameUiStyle.Shadow(hud.GetComponent<Image>());
        var hudRect = (RectTransform)hud.transform;
        hudRect.anchorMin = hudRect.anchorMax = new Vector2(0.5f, 1f);
        hudRect.pivot = new Vector2(0.5f, 1f);
        hudRect.anchoredPosition = new Vector2(0, -18);
        status = Label(hud.transform, "Objective", 25, new Vector2(752, 70), new Vector2(0, 28));
        recovery = MakeButton(hud.transform, "원석 3 · 체력 회복 +20", new Vector2(310, 44), new Vector2(-175, -36), () => RecoverParty());
        finish = MakeButton(hud.transform, "목표·성장 규칙", new Vector2(360, 44), new Vector2(175, -36), ShowHelp);
        gemHint = Label(canvas.transform, "Cave reward", 24, new Vector2(1100, 45), new Vector2(0, 430));
        gemHint.gameObject.SetActive(false);
        modal = Panel(canvas.transform, "Meadow report modal", new Color(0.02f, 0.04f, 0.05f, 0.95f), Vector2.zero);
        var modalRect = (RectTransform)modal.transform;
        modalRect.anchorMin = Vector2.zero; modalRect.anchorMax = Vector2.one;
        modalRect.offsetMin = modalRect.offsetMax = Vector2.zero;
        var card = Panel(modal.transform, "Report card", GameUiStyle.Card, new Vector2(1240, 860));
        cardRect = (RectTransform)card.transform;
        GameUiStyle.Shadow(card.GetComponent<Image>());
        heading = Label(card.transform, "Heading", 46, new Vector2(1140, 80), new Vector2(0, 330));
        heading.color = GameUiStyle.Accent;
        heading.fontStyle = FontStyle.Bold;
        var viewport = Panel(card.transform, "Report viewport", Color.clear, Vector2.zero);
        var viewportRect = (RectTransform)viewport.transform;
        viewportRect.anchorMin = Vector2.zero; viewportRect.anchorMax = Vector2.one;
        viewportRect.offsetMin = new Vector2(40, 142); viewportRect.offsetMax = new Vector2(-40, -126);
        viewport.AddComponent<RectMask2D>();
        report = Label(viewport.transform, "Report", 27, new Vector2(1120, 550), Vector2.zero);
        report.alignment = TextAnchor.UpperLeft;
        report.resizeTextForBestFit = false;
        report.verticalOverflow = VerticalWrapMode.Overflow;
        report.rectTransform.anchorMin = new Vector2(0, 1); report.rectTransform.anchorMax = Vector2.one;
        report.rectTransform.pivot = new Vector2(0.5f, 1);
        report.rectTransform.sizeDelta = Vector2.zero;
        report.gameObject.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        reportScroll = viewport.AddComponent<ScrollRect>();
        reportScroll.viewport = viewportRect;
        reportScroll.content = report.rectTransform;
        reportScroll.horizontal = false;
        reportScroll.movementType = ScrollRect.MovementType.Clamped;
        reportScroll.scrollSensitivity = 35;
        var scrollLabel = Label(card.transform, "Scroll help", 20, new Vector2(1000, 30), Vector2.zero);
        scrollLabel.text = "본문 스크롤: 마우스 휠 / 패드 RS 위·아래";
        scrollLabel.color = new Color(0.73f, 0.82f, 0.78f);
        scrollLabel.rectTransform.anchorMin = new Vector2(0, 0); scrollLabel.rectTransform.anchorMax = new Vector2(1, 0);
        scrollLabel.rectTransform.sizeDelta = new Vector2(-48, 30);
        scrollLabel.rectTransform.anchoredPosition = new Vector2(0, 119);
        primary = MakeButton(card.transform, "", new Vector2(470, 72), new Vector2(-270, -340), null);
        secondary = MakeButton(card.transform, "", new Vector2(470, 72), new Vector2(270, -340), null);
        GameUiStyle.ButtonStyle(primary, true);
        UpdateUiLayout();
        modal.SetActive(false);
    }

    void OpenModal()
    {
        hud.SetActive(false);
        modal.SetActive(true);
        Canvas.ForceUpdateCanvases();
        UpdateUiLayout();
        Canvas.ForceUpdateCanvases();
        reportScroll.StopMovement();
        reportScroll.verticalNormalizedPosition = 1f;
        UnityEngine.EventSystems.EventSystem.current?.SetSelectedGameObject(primary.gameObject);
    }

    static GameObject Panel(Transform parent, string name, Color color, Vector2 size)
    {
        var obj = new GameObject(name, typeof(RectTransform), typeof(Image));
        obj.transform.SetParent(parent, false);
        obj.GetComponent<RectTransform>().sizeDelta = size;
        obj.GetComponent<Image>().color = color;
        return obj;
    }

    Text Label(Transform parent, string name, int size, Vector2 dimensions, Vector2 position)
    {
        var obj = new GameObject(name, typeof(RectTransform), typeof(Text));
        obj.transform.SetParent(parent, false);
        var rect = obj.GetComponent<RectTransform>();
        rect.sizeDelta = dimensions; rect.anchoredPosition = position;
        var label = obj.GetComponent<Text>();
        label.font = font;
        GameUiStyle.TextStyle(label, size, Mathf.Max(20, size - 4), GameUiStyle.Paper);
        label.alignment = TextAnchor.MiddleCenter;
        label.raycastTarget = false;
        return label;
    }

    Button MakeButton(Transform parent, string text, Vector2 size, Vector2 position, UnityEngine.Events.UnityAction action)
    {
        var obj = Panel(parent, text, Color.white, size);
        obj.GetComponent<RectTransform>().anchoredPosition = position;
        var button = obj.AddComponent<Button>();
        GameUiStyle.ButtonStyle(button);
        var label = Label(obj.transform, "Label", 24, size - new Vector2(28, 10), Vector2.zero);
        label.text = text;
        label.rectTransform.anchorMin = Vector2.zero; label.rectTransform.anchorMax = Vector2.one;
        label.rectTransform.sizeDelta = new Vector2(-28, -10);
        if (action != null) button.onClick.AddListener(action);
        return button;
    }

    public void ScrollReport(float direction)
    {
        if (!IsModalOpen || reportScroll == null || Mathf.Abs(direction) < 0.1f) return;
        float overflow = report.rectTransform.rect.height - reportScroll.viewport.rect.height;
        if (overflow > 0)
            reportScroll.verticalNormalizedPosition = Mathf.Clamp01(reportScroll.verticalNormalizedPosition + direction * Time.unscaledDeltaTime * 440f / overflow);
    }

    void UpdateUiLayout()
    {
        if (canvasRect == null || cardRect == null || primary == null) return;
        Vector2 available = canvasRect.rect.size;
        if (available == lastCanvasSize || available.x <= 240 || available.y <= 320) return;
        lastCanvasSize = available;
        cardRect.sizeDelta = new Vector2(Mathf.Min(1240, available.x - 48), Mathf.Min(860, available.y - 144));
        heading.rectTransform.anchorMin = new Vector2(0, 1); heading.rectTransform.anchorMax = Vector2.one;
        heading.rectTransform.sizeDelta = new Vector2(-80, 84);
        heading.rectTransform.anchoredPosition = new Vector2(0, -64);
        float buttonWidth = (cardRect.rect.width - 104) * 0.5f;
        foreach (var button in new[] { primary, secondary })
        {
            var rect = (RectTransform)button.transform;
            rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0);
            rect.sizeDelta = new Vector2(buttonWidth, 72);
            rect.anchoredPosition = new Vector2((button == primary ? -1 : 1) * (buttonWidth + 24) * 0.5f, 64);
        }
        float hudWidth = Mathf.Min(800, available.x - 32);
        ((RectTransform)hud.transform).sizeDelta = new Vector2(hudWidth, 156);
        status.rectTransform.sizeDelta = new Vector2(hudWidth - 40, 70);
        foreach (var button in new[] { recovery, finish })
        {
            var rect = (RectTransform)button.transform;
            rect.sizeDelta = new Vector2((hudWidth - 60) * 0.5f, 48);
            rect.anchoredPosition = new Vector2((button == recovery ? -1 : 1) * (hudWidth - 20) * 0.25f, -40);
        }
        gemHint.rectTransform.anchorMin = new Vector2(0, 1); gemHint.rectTransform.anchorMax = Vector2.one;
        gemHint.rectTransform.sizeDelta = new Vector2(-48, 62);
        gemHint.rectTransform.anchoredPosition = new Vector2(0, -48);
    }

    static void ConfigureButton(Button button, string text, UnityEngine.Events.UnityAction action)
    {
        button.GetComponentInChildren<Text>().text = text;
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(action);
        button.interactable = true;
    }

}
