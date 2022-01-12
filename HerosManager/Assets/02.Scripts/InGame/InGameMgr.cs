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
    public State state = State.Camp;

    public Dictionary<string, MiniGameData> miniGameData = new Dictionary<string, MiniGameData>();

    public MiniGameMgr miniGameMgr;
    public Pause pause;

    void Awake()
    {
        //temp
        LoadGameData.Instance.LoadCSVDatas();
    }

    void Start()
    {

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
        for (int i = 0; i < LoadGameData.Instance.miniGameDatas.Count; i++)
        {
            MiniGameData mData = LoadGameData.Instance.miniGameDatas[i];
            if (mData.stageCode == _stageCode)
                miniGameData.Add(mData.code, mData);
        }

        miniGameMgr.StartMiniGame(_stageCode);
    }
}
