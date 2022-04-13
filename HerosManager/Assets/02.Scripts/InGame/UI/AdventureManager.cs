using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AdventureManager : MonoBehaviour
{
    [Header("Title")]
    public GameObject AdvPanel;
    public Image TitleImg;
    public Text TitleText;
    public Text DayText;

    [Header("Item")]
    int meat;
    int water;
    int food;
    int hub;
    public Text[] ItemNumTexts;

    public Text WarningText;
    public Text MinMeatText;
    public Text MinWaterText;
    string minStr;

    [Header("Info")]
    public Text ResultTitleText;
    public Text ResultRateText;
    public Text ResultText;
    public Button StartAdvBtn;
    string rateStr;

    [Header("EX")]
    public GameObject ExImg;
    

    JourneyData journeyData;

    void Start()
    {
        StartAdvBtn.onClick.AddListener(StartAdventure);

        WarningText.text = LoadGameData.Instance.GetString("Journey_a1");
        minStr = LoadGameData.Instance.GetString("Journey_t1") + "{0}";

        ResultTitleText.text = LoadGameData.Instance.GetString("Journey_t2");
        rateStr = LoadGameData.Instance.GetString("Journey_t3") + "{0}%\n" +
            LoadGameData.Instance.GetString("Journey_t4") + "{1}%\n" +
            LoadGameData.Instance.GetString("Journey_t5") + "{2}%";
        
        StartAdvBtn.GetComponentInChildren<Text>().text = LoadGameData.Instance.GetString("Journey_t6");
    }

    public void ReadyAdventure(StageDayData _stageDayData)
    {
        foreach (var data in LoadGameData.Instance.journeyDatas)
        {
            if (data.Value.StageDayID == _stageDayData.StageDayID)
            {
                journeyData = data.Value;
                break;
            }
        }

        meat = 0;
        water = 0;
        food = 0;
        hub = 0;

        InGameMgr.Instance.state = State.None;

        AdvPanel.SetActive(true);

        HeroStateManager.Instance.SlideAll(false);

        TitleText.text = LoadGameData.Instance.GetString(journeyData.StageTitleStringID);
        DayText.text = Clock.Instance.DayText.text;

        ItemNumTexts[0].text = meat.ToString();
        ItemNumTexts[1].text = water.ToString();
        ItemNumTexts[2].text = food.ToString();
        ItemNumTexts[3].text = hub.ToString();

        MinMeatText.text = string.Format(minStr, journeyData.Item1Min);
        MinWaterText.text = string.Format(minStr, journeyData.Item2Min);
    }

    void StartAdventure()
    {


        HeroStateManager.Instance.SlideAll(true);
    }

    public void EndJourney()
    {


    }

    public void AddSubItemBtn(int _idx)
    {
        switch (_idx)
        {
            case 0:
                break;
            case 1:
                break;
            case 2:
                break;
            case 3:
                break;
        }
    }
}
