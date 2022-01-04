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

    public HeroInfo[] heroInfos = new HeroInfo[4];

    public Dictionary<string, MiniGameData> miniGameData = new Dictionary<string, MiniGameData>();

    void Start()
    {
        Setup();
    }

    void Setup()
    {
        nowTime = 0f;
        maxTime = 360f;
        day = 1;

        for (int i = 0; i < heroInfos.Length; i++)
        {
            HeroInfo hInfo = new HeroInfo(100f, 100f, 0f, 0f);
            heroInfos[i] = hInfo;
        }
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
        for (int i = 0; i < LoadGameData.Instance.miniGameDatas.Count; i++)
        {
            MiniGameData mData = LoadGameData.Instance.miniGameDatas[i];
            if (mData.stageCode == _stageCode)
                miniGameData.Add(mData.code, mData);
        }

        switch (_stageCode)
        {
            case "":
                state = State.Game1;
                break;
        }
    }

    //temp
    public void GetItems(MiniGameData _mData)
    {
        if (_mData.type != 1)
            return;

        //플레이어 보유 아이템(목재) += _mData.wood;
    }
}
