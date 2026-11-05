using System;
using UnityEngine;

public enum State
{
    None,
    Camp,
    Gathering,
    Fence,
    MiniGame,
    Cutscene,
    Event,
    Adventure,
    Laundry
}

[DefaultExecutionOrder(-300)]
public class InGameMgr : Singleton<InGameMgr>
{
    public StageData stageData;
    public int stage;

    public Action NextStageAct;

    public State state = State.Camp;

    public Pause pause;

    public CutSceneManager cutSceneMgr;

    public bool IsComplete { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        if (_instance != this)
            return;

        Time.timeScale = 1f;
        LoadGameData.Instance.LoadTitleDatas();
        var saves = SaveDataManager.Instance;
        if (saves.saveGameData == null || saves.saveGameData.stage < 1 ||
            saves.saveGameData.items == null || saves.saveGameData.items.Length != 5)
        {
            if (!saves.LoadGameDatas())
                saves.NewGameData();
        }
        stage = saves.saveGameData.stage;
        LoadStageData();
    }

    void LoadStageData()
    {
        stageData = null;
        foreach (var data in LoadGameData.Instance.stageDatas)
        {
            if (data.Value.Index == stage)
            {
                stageData = data.Value;
                break;
            }
        }

        if (stageData == null)
        {
            Debug.LogError("StageData is Null");
            return;
        }
    }

    public void NextStage()
    {
        StageData nextStage = null;
        foreach (var entry in LoadGameData.Instance.stageDatas.Values)
            if (entry.Index == stage + 1)
                nextStage = entry;

        if (nextStage == null)
        {
            IsComplete = true;
            Clock.Instance.isStop = true;
            Debug.Log("The final configured stage is complete.");
            if (pause != null)
                pause.PauseBtn();
            return;
        }
        stage = nextStage.Index;
        stageData = nextStage;
        NextStageAct?.Invoke();
    }

    public void SaveStageData()
    {
        SaveGameData savegameData = SaveDataManager.Instance.saveGameData;
        savegameData.stage = stage;
        savegameData.day = Clock.Instance.day;
        savegameData.items = ItemManager.Instance.CaptureItems();
        HeroStateManager.Instance.CaptureStats(savegameData);
        savegameData.fenceLevel = FenceMgr.Instance.fenceLevel;

        SaveDataManager.Instance.TrySaveGameDatas();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (state == State.Camp)
            {
                pause.PauseBtn();
            }
            else if (state == State.Gathering)
            {
                GatheringManager.Instance.CloseGatheringPannel();
            }
            else if (state == State.MiniGame)
            {
                MiniGameMgr.Instance.CloseMiniGame();
            }
            else if (state == State.Fence)
            {
                FenceMgr.Instance.ClosePanel();
            }
            else if (state == State.Event)
            {
                EventMgr.Instance.ClosePanel();
            }
            else if(state == State.Laundry)
            {
                 
            }
        }

        switch (state)
        {
            case State.Cutscene:
                if (Input.anyKeyDown)
                {
                    bool isLast = cutSceneMgr.NextCutscene();
                    if (isLast)
                    {

                    }
                }
                break;
        }
    }
}
