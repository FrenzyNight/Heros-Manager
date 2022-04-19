using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Clock : Singleton<Clock>
{
    public StageDayData stageDayData;

    int day = 1;
    float nowTime;
    float dayTime;
    float heroBackTime;
    public bool isStop;

    public Text DayText;
    public Image ClockFillImg;

    public Action NextDayAct;

    public AdventureManager AdventureMgr;
    public FenceMgr fenceMgr;

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
            CheckInvade();
            LoadStageDayData();
            NextDayAct();
        }
    }

    void CheckInvade()
    {
        InvadeData invadeData = LoadGameData.Instance.invadeDatas[stageDayData.InvadeID];

        string msg = "";
        if (fenceMgr.CheckInvade(invadeData))
        {
            msg = LoadGameData.Instance.GetString(invadeData.InvadeStringID);

            List<int> temp = new List<int>() { 1, 2, 3, 4 };
            for (int i = 0; i < invadeData.StealObject; i++)
            {
                int rand = UnityEngine.Random.Range(0, temp.Count);

                string itemCode = "";
                int itemCnt = 0;
                switch (temp[rand])
                {
                    case 1:
                        itemCode = invadeData.ItemID1;
                        itemCnt = UnityEngine.Random.Range(invadeData.RandMin1, invadeData.RandMax1 + 1);
                        break;
                    case 2:
                        itemCode = invadeData.ItemID2;
                        itemCnt = UnityEngine.Random.Range(invadeData.RandMin2, invadeData.RandMax2 + 1);
                        break;
                    case 3:
                        itemCode = invadeData.ItemID3;
                        itemCnt = UnityEngine.Random.Range(invadeData.RandMin3, invadeData.RandMax3 + 1);
                        break;
                    case 4:
                        itemCode = invadeData.ItemID4;
                        itemCnt = UnityEngine.Random.Range(invadeData.RandMin4, invadeData.RandMax4 + 1);
                        break;
                }
                ItemManager.Instance.AddItem(itemCode, -itemCnt);

                temp.RemoveAt(rand);
            }

            //연출 추가

        }
        else
        {
            msg = LoadGameData.Instance.GetString("Invade_a3");

            //연출 추가

        }

        Notice.Instance.InstNoticeText(msg);
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
