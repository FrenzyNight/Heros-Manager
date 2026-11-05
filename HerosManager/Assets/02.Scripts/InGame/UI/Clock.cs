using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[DefaultExecutionOrder(100)]
public class Clock : Singleton<Clock>
{
    public GameObject[] CampObjs;
    public StageDayData stageDayData;
    public int day;
    public float nowTime, dayTime;
    public bool isStop;
    public Text DayText;
    public Image ClockFillImg;
    public Action NextDayAct;
    public AdventureManager AdventureMgr;
    public bool isHeroBack, isProduct;
    [HideInInspector] public bool isFirstHeroBack;

    private float heroBackTime;
    private bool nightStarted, advancingDay, daySettled;
    private CampInteractionMgr[] campInteractions;
    public float DayDurationSeconds => Mathf.Max(0.01f, dayTime * 60f);

    private void Start()
    {
        day = SaveDataManager.Instance.saveGameData.day;
        dayTime = LoadGameData.Instance.defineDatas["Define_Day_Time"].value1;
        heroBackTime = LoadGameData.Instance.defineDatas["Define_HeroBack_Time"].value1 * 60f;
        if (dayTime <= 0 || float.IsNaN(dayTime) || float.IsInfinity(dayTime))
            throw new InvalidOperationException("Define_Day_Time must be finite and positive.");
        heroBackTime = Mathf.Clamp(heroBackTime, 0, DayDurationSeconds);
        isFirstHeroBack = true;
        var interactions = new List<CampInteractionMgr>();
        foreach (var camp in CampObjs)
            if (camp != null && camp.TryGetComponent<CampInteractionMgr>(out var interaction))
                interactions.Add(interaction);
        campInteractions = interactions.ToArray();
        LoadStageDayData();
    }

    public void LoadStageDayData()
    {
        StopAllCoroutines();
        if (!LoadGameData.Instance.Catalog.TryGetStageDay(InGameMgr.Instance.stageData.StageDayGroupID, day, out stageDayData))
        {
            isStop = true;
            Debug.LogError($"No day data for stage {InGameMgr.Instance.stage}, day {day}.", this);
            return;
        }
        nowTime = 0;
        isHeroBack = isProduct = nightStarted = daySettled = false;
        isStop = false;
        DayText.text = string.Format(LoadGameData.Instance.GetString("Day_t1"), day);
        ClockFillImg.fillAmount = 0;
        EventMgr.Instance.EventButtonDeActive();
        if (CampaignProgressionRules.IsPlayableStage(InGameMgr.Instance.stage) && SaveDataManager.Instance.saveGameData.meadowOutcome != 0)
        {
            isStop = true;
            return;
        }
        if (MeadowCampaign.IsCreateInstance()) MeadowCampaign.Instance.BeginDay();
        AdventureMgr.ReadyAdventure(stageDayData);
    }

    public void NextDay()
    {
        var game = InGameMgr.Instance;
        if (game.IsComplete || advancingDay || !isProduct) return;
        advancingDay = true;
        isProduct = false;
        try
        {
            Time.timeScale = 0;
            // Finish yesterday's activities before loading the next stage's configuration.
            SettleDay();
            if (!GameProgression.TryGetNextDay(LoadGameData.Instance.Catalog, game.stage, day, out var stage, out var nextDay))
            {
                ProductMgr.Instance.BlackPanel.SetActive(false);
                ProductMgr.Instance.UIInProduct();
                game.CompleteGame();
                game.SaveStageData();
                return;
            }
            if (stage.Index != game.stage) game.NextStage();
            day = nextDay.Day;
            LoadStageDayData();
            NextDayAct?.Invoke();
            if (!game.SaveStageData() && MeadowCampaign.IsCreateInstance())
                MeadowCampaign.Instance.ShowSaveFailure();
            ProductMgr.Instance.UIInProduct();
        }
        finally { advancingDay = false; }
    }

    public void SettleDay()
    {
        if (daySettled) return;
        daySettled = true;
        foreach (var interaction in campInteractions) interaction.NextDayAction();
        if (GameProgression.TryGetNextDay(LoadGameData.Instance.Catalog, InGameMgr.Instance.stage,
            day, out var nextStage, out _) && nextStage.Index == InGameMgr.Instance.stage)
            FenceMgr.Instance.Invade();
    }

    public bool TryFinishDay()
    {
        if (isProduct || InGameMgr.Instance.IsComplete || !AdventureMgr.HasJourneyResult) return false;
        isProduct = true;
        StopAllCoroutines();
        TutorialMgr.Instance.isCollect = false;
        GatheringManager.Instance.isFirst = false;
        TutorialMgr.Instance.CloseTutorial();
        GatheringManager.Instance.CloseGatheringPannel();
        MiniGameMgr.Instance.CloseMiniGame();
        FenceMgr.Instance.ClosePanel();
        EventMgr.Instance.ClosePanel();
        AmuletManager.Instance.CloseList();
        EventMgr.Instance.EventButtonDeActive();
        InGameMgr.Instance.state = State.None;
        if (MeadowCampaign.IsCreateInstance())
        {
            SettleDay();
            MeadowCampaign.Instance.ShowDayReport();
        }
        else ProductMgr.Instance.UIOutProduct();
        return true;
    }

    private void Update()
    {
        if (isStop || isProduct || InGameMgr.Instance.IsComplete || FirstRunGuide.HoldClock) return;
        nowTime = Mathf.Min(nowTime + Time.deltaTime, DayDurationSeconds);
        ClockFillImg.fillAmount = nowTime / DayDurationSeconds;
        if (!isHeroBack && nowTime >= heroBackTime)
        {
            isHeroBack = true;
            AdventureMgr.EndJourney();
            if (MeadowCampaign.IsCreateInstance() && !HeroStateManager.Instance.HasAbleHeroes)
            {
                TryFinishDay();
                return;
            }
            if (isFirstHeroBack)
            {
                isFirstHeroBack = false;
                StartCoroutine(HeroBackTutorialDelay());
            }
        }
        if (isStop) return;
        if (!nightStarted && nowTime >= DayDurationSeconds / 2f)
        {
            nightStarted = true;
            EventMgr.Instance.EventButtonActive();
        }
        if (nowTime < DayDurationSeconds) return;
        TryFinishDay();
    }

    private IEnumerator HeroBackTutorialDelay()
    {
        yield return new WaitForSeconds(0.5f);
        if (!isProduct && !InGameMgr.Instance.IsComplete)
            TutorialMgr.Instance.OpenTutorial("Open_Tutorial_Back_1");
    }
}
