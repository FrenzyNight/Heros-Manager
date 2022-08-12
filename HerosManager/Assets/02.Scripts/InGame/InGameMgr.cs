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

public class InGameMgr : Singleton<InGameMgr>
{
    public StageData stageData;
    public int stage;

    public Action NextStageAct;

    public State state = State.Camp;

    public Pause pause;

    public CutSceneManager cutSceneMgr;

    void Awake()
    {
        //temp
        LoadGameData.Instance.LoadTitleDatas();

        stage = SaveDataManager.Instance.saveGameData.stage;

        LoadStageData();
    }

    void LoadStageData()
    {
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
        stage++;

        //게임 End
        if (stage > LoadGameData.Instance.stageDatas.Count)
        {
            Debug.LogError("Game End");
            return;
        }

        LoadStageData();
        NextStageAct();
    }

    public void SaveStageData()
    {
        SaveGameData savegameData = SaveDataManager.Instance.saveGameData;
        savegameData.stage = stage;
        savegameData.day = Clock.Instance.day;
        for (int i = 0; i < ItemManager.Instance.items.Length; i++)
        {
            //saveData.items[i] = ItemManager.Instance.items[i].num;
            //임시로 주석처리함
        }
        savegameData.fenceLevel = FenceMgr.Instance.fenceLevel;

        SaveDataManager.Instance.SaveGameDatas();
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
