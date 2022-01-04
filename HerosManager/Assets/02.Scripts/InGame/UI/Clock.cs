using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Clock : MonoBehaviour
{
    public Text DayText;
    public Image ClockFillImg;

    void Start()
    {
        DayDesplay();
    }

    void Update()
    {
        ClockFillImg.fillAmount = InGameMgr.Instance.nowTime / InGameMgr.Instance.maxTime;
        if (InGameMgr.Instance.nowTime >= InGameMgr.Instance.maxTime)
        {
            InGameMgr.Instance.nowTime = 0f;
            InGameMgr.Instance.day++;
            DayDesplay();
        }
    }

    void DayDesplay()
    {
        DayText.text = string.Format("{0:D2}", InGameMgr.Instance.day) + "일";
    }
}
