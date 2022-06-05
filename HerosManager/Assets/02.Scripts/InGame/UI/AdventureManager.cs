using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class AdventureManager : MonoBehaviour
{
    public GameObject productPanel;
    public float timeScale;
    public bool isClicked;
    public GameObject[] Heros;

    [Header("Title")]
    public GameObject AdvPanel;
    public Image TitleCharImg;
    public float delayTime;
    public float CharTargetX;
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
    public Text ResultPerfectRateText;
    public Text ResultSuccesRateText;
    public Text ResultFailRateText;
    //public Text ResultText;
    public Button StartAdvBtn;
    public Sprite GoodImg, NormalImg, BadImg, DangerImg;

    public Image ResultImg;
    string perfectStr;
    string normalStr;
    string failStr;


    [Header("EX")]
    public GameObject ExImg;
    
    [HideInInspector]
    public Color color;
    

    JourneyData journeyData;
    List<JStateData> jStateDataList = new List<JStateData>();
    List<JResultData> jResultDataList = new List<JResultData>();

    public AudioSource audioSource;


    void Start()
    {
        isClicked = false;
        StartAdvBtn.onClick.AddListener(StartButton);
        audioSource = GetComponent<AudioSource>();

        WarningText.text = LoadGameData.Instance.GetString("Journey_a1");
        
        //StartAdvBtn.GetComponentInChildren<Text>().text = LoadGameData.Instance.GetString("Journey_t7");
    }

    void PanelOff()
    {
        productPanel.SetActive(false);
    }

    public void ReadyAdventure(StageDayData _stageDayData)
    {
        
        Time.timeScale = 0;

        //productPanel.SetActive(true);
        //productPanel.GetComponent<Image>().DOFade(0,1.5f);

        perfectStr = LoadGameData.Instance.GetString("Journey_t4");
        normalStr = LoadGameData.Instance.GetString("Journey_t5");
        failStr = LoadGameData.Instance.GetString("Journey_t6");


        journeyData = LoadGameData.Instance.journeyDatas[_stageDayData.JourneyID];
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

        DayText.text = string.Format(LoadGameData.Instance.GetString("Day_t1"), Clock.Instance.day);

        ItemNumTexts[0].text = meat.ToString();
        ItemNumTexts[1].text = water.ToString();
        ItemNumTexts[2].text = food.ToString();
        ItemNumTexts[3].text = hub.ToString();

        ItemTextColorSet();

        MinMeatText.text = string.Format(LoadGameData.Instance.GetString("Journey_t1"), journeyData.Item1Min);
        MinWaterText.text = string.Format(LoadGameData.Instance.GetString("Journey_t1"), journeyData.Item2Min);


        CheckAdventureState();
    }

    void StartButton()
    {
        if(isClicked)
            return;

        Time.timeScale = 1;
        StartCoroutine(StartAdventereCoroutine());
    }

    IEnumerator StartAdventereCoroutine()
    {
        TitleCharImg.transform.DOLocalMoveX(CharTargetX, delayTime);
        yield return new WaitForSeconds(delayTime);
        StartAdventure();
    }

    void StartAdventure()
    {
        foreach(GameObject hero in Heros)
        {
            hero.GetComponent<HeroMove>().SetAdv();
        }
        Time.timeScale = timeScale;
        TitleCharImg.transform.DOLocalMoveX(-CharTargetX, 0);
        AdvPanel.SetActive(false);
        isClicked = false;
        ItemManager.Instance.AddItem(0, -water, -meat, -hub, -food);
        HeroStateManager.Instance.SlideAll(true);
        InGameMgr.Instance.state = State.Camp;

        for(int i=0;i<4;i++)
        {
            Heros[i].GetComponent<HeroMove>().SetAdv();
        }

        EventMgr.Instance.ChoseEvent();
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

        for(int i=0;i<4;i++)
        {
            Heros[i].GetComponent<HeroMove>().ComeBackHome();
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

    public void ItemTextColorSet()
    {
        if(meat < journeyData.Item1Min)
        {
            ColorUtility.TryParseHtmlString("#f24c36", out color);
        }
        else
        {
            ColorUtility.TryParseHtmlString("#443b38", out color);
        }
        ItemNumTexts[0].color = color;

        if(water < journeyData.Item2Min)
        {
            ColorUtility.TryParseHtmlString("#f24c36", out color);
        }
        else 
        {
            ColorUtility.TryParseHtmlString("#443b38", out color);
        }
        ItemNumTexts[1].color = color;
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

        ItemTextColorSet();
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

        ItemTextColorSet();
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

        /*
        string str = "";
        str += string.Format(perfectStr, (int)(jResultDataList[0].Prob * 100)) + "\n" +
            string.Format(normalStr, (int)(jResultDataList[1].Prob * 100)) + "\n" +
            string.Format(failStr, (int)(jResultDataList[2].Prob * 100));
        */

        ResultPerfectRateText.text = (jResultDataList[0].Prob * 100).ToString();
        ResultSuccesRateText.text = (jResultDataList[1].Prob * 100).ToString();
        ResultFailRateText.text = (jResultDataList[2].Prob * 100).ToString();

        //ResultText.text = LoadGameData.Instance.GetString(jStateData.JStateStringID);

        if(jStateData.JStateStringID == "JS_t1") //순탄
        {
            ResultImg.sprite = GoodImg;
        }
        else if(jStateData.JStateStringID == "JS_t2") //보통
        {
            ResultImg.sprite = NormalImg;
        }
        else if(jStateData.JStateStringID == "JS_t3") // 어려움
        {
            ResultImg.sprite = BadImg;
        }
        else if(jStateData.JStateStringID == "JS_t4") // 위험
        {
            ResultImg.sprite = DangerImg;
        }
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
