using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Amulet : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    bool isMouseOn;

    public string JemID;
    public string JemStringID;
    public string JemExplainString;
    public string JemExplainStringID;
    public string JemImageID;
    public int JemLevel;
    public float value1, value2;
    public int JemMaxLevel;
    public 

    void Start()
    {
        isMouseOn = false;
    }

    public void Setup()
    {
        JemStringID = LoadGameData.Instance.jemDatas[JemID].JemNameString;
        JemExplainStringID = LoadGameData.Instance.jemDatas[JemID].JemExplainString;
        JemImageID = LoadGameData.Instance.jemDatas[JemID].JemImageID;
        JemLevel = LoadGameData.Instance.jemDatas[JemID].StartLevel;
        JemMaxLevel = LoadGameData.Instance.jemDatas[JemID].MaxLevel;
        value1 = LoadGameData.Instance.jemDatas[JemID].Value1;
        value2 = LoadGameData.Instance.jemDatas[JemID].Value2;

        JemExplainString = LoadGameData.Instance.GetString(JemStringID);
    }

    void Update()
    {
        if (isMouseOn)
            AmuletManager.Instance.AmuletNotice.transform.position = Input.mousePosition;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isMouseOn = true;

        AmuletManager.Instance.NoticeText.text = "";
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isMouseOn = false;
        AmuletManager.Instance.AmuletNotice.SetActive(isMouseOn);
    }
}
