using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AdventureManager : MonoBehaviour
{
    [Header("Info")]
    public Image MainImg;
    public Text ExplainTxt;
    public Text DayText;

    [Header("Item")]
    int meat;
    int water;
    int food;
    int hub;
    public Text MeatNumTxt;
    public Text WaterNumTxt;
    public Text FoodNumTxt;
    public Text HubNumTxt;
    public Text MeatMinTxt;
    public Text WaterMinTxt;

    [Header("State")]
    public Text RateTxt;
    public Text StateTxt;

    string codeKey;

    float perfect;
    float success;
    float fail;

    enum JourneyState
    {
        none,
        perfect,
        success,
        fail
    }
    JourneyState journeyState = JourneyState.none;

    public void Setup()
    {
        codeKey = "Stage" + InGameMgr.Instance.stage + "_" + (Clock.Instance.day - 1) + "day";
        perfect = 0f;
        success = 0f;
        fail = 0f;
        journeyState = JourneyState.none;

        this.gameObject.SetActive(true);
        HeroStateManager.Instance.SlideAll(false);

        //MainImg.sprite = ;
        //ExplainTxt.text = ;
        DayText.text = (Clock.Instance.day - 1) + "일차";

        meat = 0;
        water = 0;
        food = 0;
        hub = 0;
        MeatNumTxt.text = meat.ToString();
        WaterNumTxt.text = water.ToString();
        FoodNumTxt.text = food.ToString();
        HubNumTxt.text = hub.ToString();
        MeatMinTxt.text = "최소 개수 : " + LoadGameData.Instance.journeyDatas[codeKey].meatMin + "개";
        WaterMinTxt.text = "최소 개수 : " + LoadGameData.Instance.journeyDatas[codeKey].waterMin + "개";

        CheckJourneyState();
    }

    void CheckJourneyState()
    {
        string str = CheckJourneyValue();
        StateText(str);
        RateText(str);
    }

    string CheckJourneyValue()
    {
        string state = "";

        if (meat < LoadGameData.Instance.journeyDatas[codeKey].meatMin)
            return "위험";

        if (water < LoadGameData.Instance.journeyDatas[codeKey].waterMin)
            return "위험";

        float value = (meat * LoadGameData.Instance.itemDatas["meat"].journeyValue)
            + (water * LoadGameData.Instance.itemDatas["water"].journeyValue)
            + (food * LoadGameData.Instance.itemDatas["food"].journeyValue)
            + (hub * LoadGameData.Instance.itemDatas["hub"].journeyValue);

        for (int i = 0; i < HeroStateManager.Instance.heroInfos.Length; i++)
        {
            string powerKey = "Heros" + (i + 1) + "Power";
            string stressKey = "Heros" + (i + 1) + "Stress";

            HeroInfo heroInfo = HeroStateManager.Instance.heroInfos[i];
            value += (heroInfo.power * LoadGameData.Instance.herosDatas[powerKey].journeyValue);
            value -= (heroInfo.stress * LoadGameData.Instance.herosDatas[stressKey].journeyValue);
        }

        if (value < LoadGameData.Instance.journeyDatas[codeKey].state_bad)
            state = "위험";
        else if (value >= LoadGameData.Instance.journeyDatas[codeKey].state_bad
            && value < LoadGameData.Instance.journeyDatas[codeKey].state_normal)
            state = "부실";
        else if (value >= LoadGameData.Instance.journeyDatas[codeKey].state_normal
            && value < LoadGameData.Instance.journeyDatas[codeKey].state_good)
            state = "보통";
        else if (value >= LoadGameData.Instance.journeyDatas[codeKey].state_good)
            state = "순탄";

        print(value);
        print(state);

        return state;
    }

    void RateText(string _state)
    {
        switch (_state)
        {
            case "위험":
                perfect = LoadGameData.Instance.journeyDatas["ResultPerfect"].state_danger;
                success = LoadGameData.Instance.journeyDatas["ResultSuccess"].state_danger;
                fail = LoadGameData.Instance.journeyDatas["ResultFail"].state_danger;
                break;
            case "부실":
                perfect = LoadGameData.Instance.journeyDatas["ResultPerfect"].state_bad;
                success = LoadGameData.Instance.journeyDatas["ResultSuccess"].state_bad;
                fail = LoadGameData.Instance.journeyDatas["ResultFail"].state_bad;
                break;
            case "보통":
                perfect = LoadGameData.Instance.journeyDatas["ResultPerfect"].state_normal;
                success = LoadGameData.Instance.journeyDatas["ResultSuccess"].state_normal;
                fail = LoadGameData.Instance.journeyDatas["ResultFail"].state_normal;
                break;
            case "순탄":
                perfect = LoadGameData.Instance.journeyDatas["ResultPerfect"].state_good;
                success = LoadGameData.Instance.journeyDatas["ResultSuccess"].state_good;
                fail = LoadGameData.Instance.journeyDatas["ResultFail"].state_good;
                break;
        }

        string str = "대성공 확률 : " + (int)(perfect * 100f) + "%\n성공 확률 : " + (int)(success * 100f)
            + "%\n실패 확률 : " + (int)(fail * 100f) + "%";
        RateTxt.text = str;
    }

    void StateText(string _state)
    {
        Color textColor = Color.clear;
        Color outLineColor = Color.clear;
        switch (_state)
        {
            case "위험":
                textColor = Color.red;
                outLineColor = Color.yellow;
                outLineColor.a = 0.5f;
                break;
            case "부실":
                textColor = new Color(1f, 0.5f, 0f, 1f);
                outLineColor = Color.blue;
                outLineColor.a = 0.5f;
                break;
            case "보통":
                textColor = Color.yellow;
                outLineColor = Color.green;
                outLineColor.a = 0.5f;
                break;
            case "순탄":
                textColor = Color.green;
                outLineColor = Color.white;
                outLineColor.a = 0.5f;
                break;
        }

        StateTxt.text = _state;
        StateTxt.color = textColor;
        StateTxt.GetComponent<Outline>().effectColor = outLineColor;
    }

    public void StartBtn()
    {
        float rand = Random.Range(0, 100) / 100f;
        if (rand < perfect)
            journeyState = JourneyState.perfect;
        else if (rand < perfect + success)
            journeyState = JourneyState.success;
        else
            journeyState = JourneyState.fail;

        ItemManager.Instance.AddItem(0, -water, -meat, -hub, -food);

        this.gameObject.SetActive(false);
    }

    public void EndJourney()
    {
        if (journeyState == JourneyState.none)
            return;

        string str = "";
        int cse = 0;
        switch (journeyState)
        {
            case JourneyState.perfect:
                str = "용사들이 의기양양한 모습으로 옵니다!";
                cse = 1;
                break;
            case JourneyState.success:
                str = "용사들이 도착했습니다.";
                cse = 2;
                break;
            case JourneyState.fail:
                str = "용사들이 많이 지쳐 보입니다...";
                cse = 3;
                break;
        }
        Notice.Instance.InstNoticeText(str);
        HeroStateManager.Instance.SetJourneyState(cse);

        journeyState = JourneyState.none;
    }

    #region 아이템 버튼
    public void AddMeatBtn()
    {
        if (ItemManager.Instance.meat <= meat)
            return;

        meat++;
        MeatNumTxt.text = meat.ToString();
        CheckJourneyState();
    }

    public void SubMeatBtn()
    {
        if (meat <= 0)
            return;

        meat--;
        MeatNumTxt.text = meat.ToString();
        CheckJourneyState();
    }

    public void AddWaterBtn()
    {
        if (ItemManager.Instance.water <= water)
            return;

        water++;
        WaterNumTxt.text = water.ToString();
        CheckJourneyState();
    }

    public void SubWaterBtn()
    {
        if (water <= 0)
            return;

        water--;
        WaterNumTxt.text = water.ToString();
        CheckJourneyState();
    }

    public void AddFoodBtn()
    {
        if (ItemManager.Instance.food <= food)
            return;

        food++;
        FoodNumTxt.text = food.ToString();
        CheckJourneyState();
    }

    public void SubFoodBtn()
    {
        if (food <= 0)
            return;

        food--;
        FoodNumTxt.text = food.ToString();
        CheckJourneyState();
    }

    public void AddHubBtn()
    {
        if (ItemManager.Instance.hub <= hub)
            return;

        hub++;
        HubNumTxt.text = hub.ToString();
        CheckJourneyState();
    }

    public void SubHubBtn()
    {
        if (hub <= 0)
            return;

        hub--;
        HubNumTxt.text = hub.ToString();
        CheckJourneyState();
    }
    #endregion
}
