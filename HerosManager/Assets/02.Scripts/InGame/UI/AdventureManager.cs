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

    [Header("Info")]
    public Text ResultTitleText;
    public Text ResultRateText;
    public Text ResultText;
    public Button StartAdvBtn;
    string perfectStr;
    string normalStr;
    string failStr;


    [Header("EX")]
    public GameObject ExImg;
    

    JourneyData journeyData;
    List<JStateData> jStateDataList = new List<JStateData>();
    List<JResultData> jResultDataList = new List<JResultData>();

    void Start()
    {
        StartAdvBtn.onClick.AddListener(StartAdventure);

        WarningText.text = LoadGameData.Instance.GetString("Journey_a1");

        ResultTitleText.text = LoadGameData.Instance.GetString("Journey_t3");
        perfectStr = LoadGameData.Instance.GetString("Journey_t4");
        normalStr = LoadGameData.Instance.GetString("Journey_t5");
        failStr = LoadGameData.Instance.GetString("Journey_t6");
        
        StartAdvBtn.GetComponentInChildren<Text>().text = LoadGameData.Instance.GetString("Journey_t7");
    }

    public void ReadyAdventure(StageDayData _stageDayData)
    {
        journeyData = LoadGameData.Instance.journeyDatas[Clock.Instance.stageDayData.JourneyID];
        jStateDataList.Clear();
        foreach (var data in LoadGameData.Instance.jStateDatas)
        {
            if (data.Value.JStateGroupID == journeyData.JStateGroupID)
                jStateDataList.Add(data.Value);
        }

        meat = 0;
        water = 0;
        food = 0;
        hub = 0;

        InGameMgr.Instance.state = State.None;

        AdvPanel.SetActive(true);

        HeroStateManager.Instance.SlideAll(false);

        TitleText.text = LoadGameData.Instance.GetString(journeyData.StageTitleStringID);
        DayText.text = string.Format(LoadGameData.Instance.GetString("Day_t1"), Clock.Instance.day);

        ItemNumTexts[0].text = meat.ToString();
        ItemNumTexts[1].text = water.ToString();
        ItemNumTexts[2].text = food.ToString();
        ItemNumTexts[3].text = hub.ToString();

        MinMeatText.text = string.Format(LoadGameData.Instance.GetString("Journey_t1"), journeyData.Item1Min);
        MinWaterText.text = string.Format(LoadGameData.Instance.GetString("Journey_t1"), journeyData.Item2Min);


        CheckAdventureState();
    }

    void StartAdventure()
    {
        Time.timeScale = 1;
        AdvPanel.SetActive(false);
        ItemManager.Instance.AddItem(0, -water, -meat, -hub, -food);
        HeroStateManager.Instance.SlideAll(true);
    }

    public void EndJourney()
    {
        JResultData jResultData = null;

        float rand = Random.Range(0f, 0.99f);
        float temp = 0;
        for (int i = 0; i < jResultDataList.Count; i++)
        {
            temp += jResultDataList[i].Prob;
            if (rand < temp)
            {
                jResultData = jResultDataList[i];
                break;
            }
        }

        string msg = LoadGameData.Instance.GetString(jResultData.JRanStringID);
        Notice.Instance.InstNoticeText(msg);

        HeroStateManager.Instance.heroStates[0].AddStat
            (jResultData.Hero1_Stress, jResultData.Hero1_Power, jResultData.Hero1_Hp, jResultData.Hero1_Exp);
        HeroStateManager.Instance.heroStates[1].AddStat
            (jResultData.Hero2_Stress, jResultData.Hero2_Power, jResultData.Hero2_Hp, jResultData.Hero2_Exp);
        HeroStateManager.Instance.heroStates[2].AddStat
            (jResultData.Hero3_Stress, jResultData.Hero3_Power, jResultData.Hero3_Hp, jResultData.Hero3_Exp);
        HeroStateManager.Instance.heroStates[3].AddStat
            (jResultData.Hero4_Stress, jResultData.Hero4_Power, jResultData.Hero4_Hp, jResultData.Hero4_Exp);

    }
    public void SubItemBtn(int _idx)
    {
        int num = -1;
        //if (!_isAdd)
         //   num = -1;

        switch (_idx)
        {
            case 0:
                if (meat + num < 0)
                    return;

                meat += num;
                ItemNumTexts[_idx].text = meat.ToString();
                break;
            case 1:
                if (water + num < 0)
                    return;

                water += num;
                ItemNumTexts[_idx].text = water.ToString();
                break;
            case 2:
                if (food + num < 0)
                    return;

                food += num;
                ItemNumTexts[_idx].text = food.ToString();
                break;
            case 3:
                if (hub + num < 0)
                    return;

                hub += num;
                ItemNumTexts[_idx].text = hub.ToString();
                break;
        }

        CheckAdventureState();
    }

    public void AddItemBtn(int _idx)
    {
        int num = 1;
        //if (!_isAdd)
         //   num = -1;

        switch (_idx)
        {
            case 0:
                if (meat + num > ItemManager.Instance.GetItemInfo("Item_Meat").num)
                    return;

                meat += num;
                ItemNumTexts[_idx].text = meat.ToString();
                break;
            case 1:
                if (water + num > ItemManager.Instance.GetItemInfo("Item_Water").num )
                    return;

                water += num;
                ItemNumTexts[_idx].text = water.ToString();
                break;
            case 2:
                if (food + num > ItemManager.Instance.GetItemInfo("Item_Food").num)
                    return;

                food += num;
                ItemNumTexts[_idx].text = food.ToString();
                break;
            case 3:
                if (hub + num > ItemManager.Instance.GetItemInfo("Item_Hub").num)
                    return;

                hub += num;
                ItemNumTexts[_idx].text = hub.ToString();
                break;
        }

        CheckAdventureState();
    }

    void CheckAdventureState()
    {
        JStateData jStateData = jStateDataList[jStateDataList.Count - 1];

        float cutValue = AdventureValue();
        for (int i = 0; i < jStateDataList.Count; i++)
        {
            if (cutValue >= jStateDataList[i].ValueMin)
            {
                jStateData = jStateDataList[i];
                break;
            }
        }

        jResultDataList.Clear();
        foreach (var data in LoadGameData.Instance.jResultDatas)
        {
            if (data.Value.JResultGroupID == jStateData.JResultGroupID)
                jResultDataList.Add(data.Value);
        }

        string str = "";
        str += string.Format(perfectStr, (int)(jResultDataList[0].Prob * 100)) + "\n" +
            string.Format(normalStr, (int)(jResultDataList[1].Prob * 100)) + "\n" +
            string.Format(failStr, (int)(jResultDataList[2].Prob * 100));

        ResultRateText.text = str;
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

        value = (journeyData.MeatValue * meat) + (journeyData.WaterValue * water) +
            (journeyData.HubValue * hub) + (journeyData.FoodValue * food) +
            (journeyData.HStressValue * totalHeroStress) + (journeyData.HPowerValue * totalHeroPower);

        return value;
    }
}
