using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameMgr : Singleton<InGameMgr>
{
    public enum State
    {
        Camp,
        Pannel,
        Game1,
        Game2,
        Game3,
    }
    public State state = State.Camp;

    public float nowTime;
    public float maxTime;

    void Start()
    {
        Setup();
    }

    void Setup()
    {
        nowTime = 0f;
        maxTime = 360f;
    }

    void Update()
    {
        Timer();
    }

    void Timer()
    {
        nowTime += Time.deltaTime;
    }
}
