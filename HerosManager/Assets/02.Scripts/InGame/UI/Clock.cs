using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Clock : MonoBehaviour
{
    public Text DayText;
    public Image ClockFillImg;

    public float nowTime;
    public float maxTime;
    public int day;

    void Start()
    {
        Setup();
        DayDesplay();
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
        ClockFillImg.fillAmount = nowTime / maxTime;
        if (nowTime >= maxTime)
        {
            nowTime = 0f;
            day++;
            DayDesplay();
        }
    }

    void DayDesplay()
    {
        DayText.text = string.Format("{0:D2}", day) + "일";
    }
}
