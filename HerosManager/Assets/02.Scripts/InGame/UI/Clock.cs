﻿using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Clock : Singleton<Clock>
{
    public StageDayData stageDayData;

    public int day = 1;
    float nowTime;
    float dayTime;
    float heroBackTime;
    public bool isStop;

    public Text DayText;
    public Image ClockFillImg;

    public Action NextDayAct;

    public AdventureManager AdventureMgr;

    void Start()
    {
        Setup();
        LoadStageDayData();
    }

    void Setup()
    {
        dayTime = LoadGameData.Instance.defineDatas["Define_Day_Time"].value;
        heroBackTime = LoadGameData.Instance.defineDatas["Define_HeroBack_Time"].value;
    }

    void LoadStageDayData()
    {
        foreach (var data in LoadGameData.Instance.stageDayDatas)
        {
            if (data.Value.StageDayGroupID == InGameMgr.Instance.stageData.StageDayGroupID &&
                data.Value.Day == day)
            {
                stageDayData = data.Value;
                break;
            }
        }

        nowTime = 0f;

        DayText.text = string.Format(LoadGameData.Instance.GetString(stageDayData.StageDayStringID), day);
        ClockFillImg.fillAmount = 0;

        AdventureMgr.ReadyAdventure(stageDayData);
    }

    void NextDay()
    {
        Time.timeScale = 0f;

        int cnt = 0;
        foreach (var data in LoadGameData.Instance.stageDayDatas)
        {
            if (data.Value.StageDayGroupID == InGameMgr.Instance.stageData.StageDayGroupID)
                cnt++;
        }

        //마지막날
        if (day >= cnt)
        {
            day = 1;
            InGameMgr.Instance.NextStage();
            LoadStageDayData();
        }
        else
        {
            day++;
            FenceMgr.Instance.Invade();
            LoadStageDayData();
            NextDayAct();
        }
    }

    void Update()
    {
        Timer();
    }

    void Timer()
    {
        if (isStop)
            return;

        nowTime += Time.deltaTime;
        ClockFillImg.fillAmount = nowTime / (dayTime * 60f);
        if (nowTime >= (heroBackTime * 60f))
        {
            AdventureMgr.EndJourney();
        }
        if (nowTime >= (dayTime * 60f))
        {
            NextDay();
        }
    }
}
