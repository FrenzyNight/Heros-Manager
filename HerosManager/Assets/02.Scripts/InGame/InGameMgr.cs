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

    void Start()
    {

    }

    void Update()
    {
        
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
}
