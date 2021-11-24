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

    public float nowTime;
    public float maxTime;
    public int day;

    Dictionary<string, MiniGameData> miniGameData = new Dictionary<string, MiniGameData>();

    void Start()
    {
        Setup();
    }

    void Setup()
    {
        nowTime = 0f;
        maxTime = 360f;
        day = 1;
    }

    void Update()
    {
        Timer();
    }

    void Timer()
    {
        nowTime += Time.deltaTime;
    }

    public void EnterMiniGame(string _stageCode)
    {
        miniGameData.Clear();
        string key = LoadGameData.Instance.miniGameDic[_stageCode].code;
        MiniGameData mData = LoadGameData.Instance.miniGameDic[_stageCode];
        miniGameData.Add(key, mData);

        switch (_stageCode)
        {
            case "":
                state = State.Game1;
                break;
        }
    }
}
