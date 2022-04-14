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
    JVwithObjectData jVwithObjectData;
    JVCutData jVCutData;
    JRwithHeroData jRwithHeroData;
    JStateData jStateData;

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

        jVwithObjectData = LoadGameData.Instance.jVwithObjectDatas[journeyData.JVwithObjectID];
        jVCutData = LoadGameData.Instance.jVCutDatas[journeyData.JVCutID];

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


        CheckAdventureState();
    }

    void StartAdventure()
    {
        ItemManager.Instance.AddItem(0, -water, -meat, -hub, -food);

        string jRStateID = "";

        float rand = Random.Range(0f, 0.99f);
        if (rand < jStateData.ResultPerfectProb)
            jRStateID = "JR_Perfect";
        else if (rand < jStateData.ResultPerfectProb + jStateData.ResultNormalProb)
            jRStateID = "JR_Normal";
        else if (rand < jStateData.ResultPerfectProb + jStateData.ResultNormalProb + jStateData.ResultFailProb)
            jRStateID = "JR_Fail";

        foreach (var data in LoadGameData.Instance.jRwithHeroDatas)
        {
            if (data.JRwithHeroID == journeyData.JRwithHeroID)
            {
                if (data.JRStateID == jRStateID)
                {
                    jRwithHeroData = data;
                    break;
                }
            }
        }

        HeroStateManager.Instance.SlideAll(true);
    }

    public void EndJourney()
    {
        string msg = LoadGameData.Instance.GetString(jRwithHeroData.JRanStringID);
        Notice.Instance.InstNoticeText(msg);

        HeroStateManager.Instance.heroStates[0].AddStat
            (jRwithHeroData.Hero1_Stress, jRwithHeroData.Hero1_Power, jRwithHeroData.Hero1_Hp, jRwithHeroData.Hero1_Exp);
        HeroStateManager.Instance.heroStates[1].AddStat
            (jRwithHeroData.Hero2_Stress, jRwithHeroData.Hero2_Power, jRwithHeroData.Hero2_Hp, jRwithHeroData.Hero2_Exp);
        HeroStateManager.Instance.heroStates[2].AddStat
            (jRwithHeroData.Hero3_Stress, jRwithHeroData.Hero3_Power, jRwithHeroData.Hero3_Hp, jRwithHeroData.Hero3_Exp);
        HeroStateManager.Instance.heroStates[3].AddStat
            (jRwithHeroData.Hero4_Stress, jRwithHeroData.Hero4_Power, jRwithHeroData.Hero4_Hp, jRwithHeroData.Hero4_Exp);

    }

    public void AddSubItemBtn(int _idx, bool _isAdd)
    {
        int num = 1;
        if (!_isAdd)
            num *= -1;

        switch (_idx)
        {
            case 0:
                if (meat + num > ItemManager.Instance.GetItemInfo("Item_Meat").num ||
                    meat + num < 0)
                    return;

                meat += num;
                ItemNumTexts[_idx].text = meat.ToString();
                break;
            case 1:
                if (water + num > ItemManager.Instance.GetItemInfo("Item_Water").num ||
                    water + num < 0)
                    return;

                water += num;
                ItemNumTexts[_idx].text = water.ToString();
                break;
            case 2:
                if (food + num > ItemManager.Instance.GetItemInfo("Item_Food").num ||
                    food + num < 0)
                    return;

                food += num;
                ItemNumTexts[_idx].text = food.ToString();
                break;
            case 3:
                if (hub + num > ItemManager.Instance.GetItemInfo("Item_Hub").num ||
                    hub + num < 0)
                    return;

                hub += num;
                ItemNumTexts[_idx].text = hub.ToString();
                break;
        }

        CheckAdventureState();
    }

    void CheckAdventureState()
    {
        string jStateID = "";

        float cutValue = AdventureValue();
        if (cutValue >= jVCutData.StateGoodMin)
            jStateID = "State_Good";
        else if (cutValue >= jVCutData.StateNormalMin)
            jStateID = "State_Normal";
        else if (cutValue >= jVCutData.StateBadMin)
            jStateID = "State_Bad";
        else if (cutValue >= jVCutData.StateDangerMin)
            jStateID = "State_Danger";
        else
            jStateID = "State_Danger";

        jStateData = LoadGameData.Instance.jStateDatas[jStateID];
        ResultRateText.text = string.Format(rateStr,
            (int)(jStateData.ResultPerfectProb * 100), (int)(jStateData.ResultNormalProb * 100), (int)(jStateData.ResultFailProb * 100));
        ResultText.text = LoadGameData.Instance.GetString(jStateData.JStateStringID);
    }

    float AdventureValue()
    {
        float value = 0f;
        if (meat < journeyData.Item1Min)
            return 0;
        if (water < journeyData.Item2Min)
            return 0;

        float totalHeroStress = 0;
        float totalHeroPower = 0;
        for (int i = 0; i < HeroStateManager.Instance.heroStates.Length; i++)
        {
            totalHeroStress += HeroStateManager.Instance.heroStates[i].stress;
            totalHeroPower += HeroStateManager.Instance.heroStates[i].power;
        }

        value = (jVwithObjectData.MeatValue * meat) + (jVwithObjectData.WaterValue * water) +
            (jVwithObjectData.HubValue * hub) + (jVwithObjectData.FoodValue * food) +
            (jVwithObjectData.HStressValue * totalHeroStress) + (jVwithObjectData.HPowerValue * totalHeroPower);

        return value;
    }
}
