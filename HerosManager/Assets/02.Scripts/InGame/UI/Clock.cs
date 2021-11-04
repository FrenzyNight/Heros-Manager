using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Clock : MonoBehaviour
{
    public Image ClockFillImg;

    //void Start()
    //{
        
    //}

    void Update()
    {
        ClockFillImg.fillAmount = InGameMgr.Instance.nowTime / InGameMgr.Instance.maxTime;
    }
}
