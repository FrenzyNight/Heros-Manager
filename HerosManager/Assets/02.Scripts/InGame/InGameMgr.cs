using System;
using UnityEngine;

public enum State
{
    None,
    Camp,
    Gathering,
    Fence,
    MiniGame,
}

public class InGameMgr : Singleton<InGameMgr>
{
    public StageData stageData;
    public int stage;

    public Action NextStageAct;

    public State state = State.Camp;

    public Pause pause;

    void Awake()
    {
        //temp
        LoadGameData.Instance.LoadCSVDatas();

        if (SaveDataManager.Instance.isContinue)
            stage = SaveDataManager.Instance.saveData.stage;
        else
            stage = 1;

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
        SaveData saveData = SaveDataManager.Instance.saveData;
        saveData.stage = stage;
        saveData.day = Clock.Instance.day;

        SaveDataManager.Instance.SaveDatas();
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
        }
    }
}
