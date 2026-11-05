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
    private bool nightStarted, advancingDay;
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
        if (!LoadGameData.Instance.Catalog.TryGetStageDay(InGameMgr.Instance.stageData.StageDayGroupID, day, out stageDayData))
        {
            isStop = true;
            Debug.LogError($"No day data for stage {InGameMgr.Instance.stage}, day {day}.", this);
            return;
        }
        nowTime = 0;
        isHeroBack = isProduct = nightStarted = false;
        DayText.text = string.Format(LoadGameData.Instance.GetString("Day_t1"), day);
        ClockFillImg.fillAmount = 0;
        EventMgr.Instance.EventButtonDeActive();
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
            foreach (var interaction in campInteractions) interaction.NextDayAction();
            if (!GameProgression.TryGetNextDay(LoadGameData.Instance.Catalog, game.stage, day, out var stage, out var nextDay))
            {
                ProductMgr.Instance.BlackPanel.SetActive(false);
                ProductMgr.Instance.UIInProduct();
                game.CompleteGame();
                game.SaveStageData();
                return;
            }
            if (stage.Index != game.stage) game.NextStage();
            else FenceMgr.Instance.Invade();
            day = nextDay.Day;
            LoadStageDayData();
            NextDayAct?.Invoke();
            game.SaveStageData();
            ProductMgr.Instance.UIInProduct();
        }
        finally { advancingDay = false; }
    }

    private void Update()
    {
        if (isStop || isProduct || InGameMgr.Instance.IsComplete) return;
        nowTime = Mathf.Min(nowTime + Time.deltaTime, DayDurationSeconds);
        ClockFillImg.fillAmount = nowTime / DayDurationSeconds;
        if (!isHeroBack && nowTime >= heroBackTime)
        {
            isHeroBack = true;
            AdventureMgr.EndJourney();
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
        isProduct = true;
        GatheringManager.Instance.CloseGatheringPannel();
        MiniGameMgr.Instance.CloseMiniGame();
        FenceMgr.Instance.ClosePanel();
        EventMgr.Instance.ClosePanel();
        AmuletManager.Instance.CloseList();
        EventMgr.Instance.EventButtonDeActive();
        InGameMgr.Instance.state = State.None;
        ProductMgr.Instance.UIOutProduct();
    }

    private IEnumerator HeroBackTutorialDelay()
    {
        yield return new WaitForSeconds(0.5f);
        if (!isProduct && !InGameMgr.Instance.IsComplete)
            TutorialMgr.Instance.OpenTutorial("Open_Tutorial_Back_1");
    }
}
