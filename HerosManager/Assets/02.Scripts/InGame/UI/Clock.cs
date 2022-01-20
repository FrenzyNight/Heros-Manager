using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Clock : Singleton<Clock>
{
    public Text DayText;
    public Image ClockFillImg;

    public float nowTime;
    public float maxTime;
    public int day;

    public bool isStop;

    public GameObject NextDayPannel;
    public AdventureManager AdventureMgr;

    void Start()
    {
        Setup();
    }

    void Setup()
    {
        nowTime = 0f;
        maxTime = 360f;
        day = 1;
        DayText.text = string.Format("{0:D2}", day) + "일";

        isStop = false;
    }

    void Update()
    {
        Timer();

        if (Input.GetKeyDown(KeyCode.Z))
        {
            StartCoroutine("NextDayCo");
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            AdventureMgr.EndJourney();
        }
    }

    void Timer()
    {
        if (isStop)
            return;

        nowTime += Time.deltaTime;
        ClockFillImg.fillAmount = nowTime / maxTime;
        if (day > 1 && nowTime >= maxTime / 3f)
        {
            AdventureMgr.EndJourney();
        }
        if (nowTime >= maxTime)
        {
            StartCoroutine("NextDayCo");
        }
    }

    IEnumerator NextDayCo()
    {
        isStop = true;

        NextDayPannel.SetActive(true);

        yield return new WaitForSeconds(4f);

        nowTime = 0f;
        day++;
        DayText.text = string.Format("{0:D2}", day) + "일";
        isStop = false;

        NextDayPannel.SetActive(false);
        AdventureMgr.Setup();
    }
}
