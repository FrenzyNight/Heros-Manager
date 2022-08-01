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
    public bool isProduct;

    void Start()
    {
        Setup();
        LoadStageDayData();
    }

    void Setup()
    {
        day = SaveDataManager.Instance.saveGameData.day;

        dayTime = LoadGameData.Instance.defineDatas["Define_Day_Time"].value;
        heroBackTime = LoadGameData.Instance.defineDatas["Define_HeroBack_Time"].value;
        isHeroBack = false;
        isProduct = false;
        //isChicken = false;
    }

    public void LoadStageDayData()
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

    public void NextDay()
    {
        nowTime = 0;
        Time.timeScale = 0f;

        ProductMgr.Instance.UIInProduct();
        isProduct =false;

        isHeroBack = false;
        //isChicken = false;

        //Panel Exit
        

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


        //EventMgr.Instance.EventButton.GetComponent<Button>().interactable = false;
        EventMgr.Instance.EventButtonDeActive();
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

        /*
        if(nowTime > 0 && !isChicken)
        {
            AdventureMgr.audioSource.Play();
            isChicken = true;
        }
        */

        if (nowTime >= (dayTime * 60f) && !isProduct)
        {
            GatheringManager.Instance.CloseGatheringPannel();
            MiniGameMgr.Instance.CloseMiniGame();
            FenceMgr.Instance.ClosePanel();
            EventMgr.Instance.ClosePanel();
            AmuletManager.Instance.CloseList();
            //nowTime = 0;
            isProduct = true;
            ProductMgr.Instance.UIOutProduct();
            //NextDay();
        }

        if(nowTime >= (dayTime * 60f)/2) //night
        {
            //EventMgr.Instance.EventButton.GetComponent<Button>().interactable = true;
            EventMgr.Instance.EventButtonActive();
        }
    }
}
