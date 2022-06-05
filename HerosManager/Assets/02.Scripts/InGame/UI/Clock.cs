using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Clock : Singleton<Clock>
{
    public GameObject[] CampObjs;
    public StageDayData stageDayData;

    public int day;
    public float nowTime;
    public float dayTime;
    float heroBackTime;
    public bool isStop;

    public Text DayText;
    public Image ClockFillImg;

    public Action NextDayAct;

    public AdventureManager AdventureMgr;

    public bool isHeroBack;
    public bool isChicken;

    void Start()
    {
        Setup();
        LoadStageDayData();
    }

    void Setup()
    {
        if (SaveDataManager.Instance.isContinue)
            day = SaveDataManager.Instance.saveData.day;
        else
            day = 1;

        dayTime = LoadGameData.Instance.defineDatas["Define_Day_Time"].value;
        heroBackTime = LoadGameData.Instance.defineDatas["Define_HeroBack_Time"].value;
        isHeroBack = false;
        isChicken = false;
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

        DayText.text = string.Format(LoadGameData.Instance.GetString("Day_t1"), day);
        ClockFillImg.fillAmount = 0;

        AdventureMgr.ReadyAdventure(stageDayData);
    }

    void NextDay()
    {
        nowTime = 0;
        Time.timeScale = 0f;
        isHeroBack = false;
        isChicken = false;

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
            foreach(GameObject campobj in CampObjs)
            {
                campobj.GetComponent<CampInteractionMgr>().NextDayAction();
            }
            //NextDayAct();
        }


        EventMgr.Instance.EventButton.GetComponent<Button>().interactable = false;
        InGameMgr.Instance.SaveStageData();
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
        if (nowTime >= (heroBackTime * 60f) && !isHeroBack)
        {
            AdventureMgr.EndJourney();
            isHeroBack = true;
        }

        if(nowTime > 0 && !isChicken)
        {
            AdventureMgr.audioSource.Play();
            isChicken = true;
        }

        if (nowTime >= (dayTime * 60f))
        {
            NextDay();
        }

        if(nowTime >= (dayTime * 60f)/2) //night
        {
            EventMgr.Instance.EventButton.GetComponent<Button>().interactable = true;
        }
    }
}
