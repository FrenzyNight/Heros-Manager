using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum State
{
    Camp,
    Pannel,
    Game1,
    Game2,
    Game3,
}

public class InGameMgr : Singleton<InGameMgr>
{
    public StageData stageData;
    int stage = 1;

    public State state = State.Camp;

    public Dictionary<string, MiniGameData> miniGameData = new Dictionary<string, MiniGameData>();

    public MiniGameMgr miniGameMgr;
    public Pause pause;

    void Awake()
    {
        //temp
        LoadGameData.Instance.LoadCSVDatas();

        LoadStageData();
    }

    void LoadStageData()
    {
        string code = "";
        switch (stage)
        {
            case 1:
                code = "Stage_Grassland";
                break;
            case 2:
                code = "Stage_Desert";
                break;
            case 3:
                code = "Stage_Forest";
                break;
        }

        if (code == "")
        {
            Debug.LogError("StageData is Null");
            return;
        }

        stageData = LoadGameData.Instance.stageDatas[code];
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
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (state == State.Camp)
            {
                pause.PauseBtn();
            }
            else if (state == State.Pannel)
            {
                Gathering.Instance.CloseGatheringPannel();
            }
            else if (state == State.Game1 || state == State.Game2 || state == State.Game3)
            {
                miniGameMgr.CloseCurrentGame();
            }
        }
    }

    public void EnterMiniGame(string _stageCode)
    {
        miniGameData.Clear();


        miniGameMgr.StartMiniGame(_stageCode);
    }
}
