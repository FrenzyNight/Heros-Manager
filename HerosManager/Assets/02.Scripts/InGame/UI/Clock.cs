using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Clock : Singleton<Clock>
{
    StageDayData stageDayData;

    int day = 1;
    float nowTime;
    public bool isStop;

    public Text DayText;
    public Image ClockFillImg;

    public AdventureManager AdventureMgr;

    void Start()
    {
        LoadStageDayData();
    }

    void LoadStageDayData()
    {
        foreach (var data in LoadGameData.Instance.stageDayDatas)
        {
            if (data.Value.StageID == InGameMgr.Instance.stageData.StageID)
            {
                string dayStr = data.Value.StageDayID;
                if ((int)dayStr[dayStr.Length - 1] == day)
                {
                    stageDayData = data.Value;
                    break;
                }
            }
        }

        if (stageDayData == null)
        {
            Debug.LogError("StageDayData is Null");
            return;
        }

        nowTime = 0f;
        isStop = false;

        DayText.text = LoadGameData.Instance.GetString(stageDayData.StageDayStringID);
    }

    void NextDay()
    {
        day++;
        int stageDayCnt = 0;
        foreach (var data in LoadGameData.Instance.stageDayDatas)
        {
            if (data.Value.StageID == InGameMgr.Instance.stageData.StageID)
            {
                stageDayCnt++;
            }
        }

        if (day > stageDayCnt)
        {
            InGameMgr.Instance.NextStage();
        }

        LoadStageDayData();
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
        ClockFillImg.fillAmount = nowTime / (stageDayData.Time * 60f);
        if (nowTime >= (stageDayData.HeroBackTime * 60f))
        {
            AdventureMgr.EndJourney();
        }
        if (nowTime >= (stageDayData.Time * 60f))
        {
            NextDay();
        }
    }
}
