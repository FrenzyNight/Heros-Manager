using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UniJSON;

public class AdventureManager : Singleton<AdventureManager>
{
    public GameObject BlackPanel;
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


    [Header("EX")]
    public GameObject ExImg;

    [Header("NightProduct")]
    public GameObject NightSky;
    public GameObject NightStar;
    public GameObject NightGob1, NightGob2;
    public GameObject NightFilter;
    
    [HideInInspector]
    public Color color;
    

    JourneyData journeyData;
    IReadOnlyList<JStateData> jStateDataList = System.Array.Empty<JStateData>();
    IReadOnlyList<JResultData> jResultDataList = System.Array.Empty<JResultData>();
    bool journeyReady;
    bool journeyInProgress;
    bool preparedCampaignJourney;
    string preparedDayId;
    readonly bool[] departingHeroes = new bool[4];
    int departureFood;
    int departureHeroCount;
    public bool HasJourneyResult { get; private set; }
    public MeadowJourneyResult LastJourneyResult { get; private set; }
    public bool IsJourneyReady => journeyReady;
    public bool IsJourneyInProgress => journeyInProgress;
    public int SelectedMeat => meat;
    public int SelectedWater => water;
    public int MinimumMeat => journeyData != null ? journeyData.Item1Min : 0;
    public int MinimumWater => journeyData != null ? journeyData.Item2Min : 0;
    public bool CanDepart => journeyReady && (!FirstRunGuide.SuppliesRequired ||
        (meat >= MinimumMeat && water >= MinimumWater)) && HeroStateManager.Instance.HasAbleHeroes &&
        WeightedSelection.TryPick(jResultDataList, ResultWeight, 0f, out _);

    [HideInInspector]
    public AudioSource audioSource;

    [HideInInspector] public bool isFirstAdventure;
    [HideInInspector] public bool isFirstMini;
   // [HideInInspector] public bool isFirstHeroBack;
    protected override void Awake()
    {
        base.Awake();
        audioSource = GetComponent<AudioSource>();
        isFirstAdventure = true;
        isFirstMini = true;
        //isFirstHeroBack = true;
    }

    void Start()
    {
        isClicked = false;
        StyleSupplyGuidance();
        
        StartAdvBtn.onClick.AddListener(StartButton);
        

        if (journeyReady) CheckAdventureState();
        else WarningText.text = LoadGameData.Instance.GetString("Journey_a1");
        
        //StartAdvBtn.GetComponentInChildren<Text>().text = LoadGameData.Instance.GetString("Journey_t7");
    }

    void PanelOff()
    {
        BlackPanel.SetActive(false);
    }

    void StyleSupplyGuidance()
    {
        // This space sits below the two supply rows on the existing illustrated panel.
        WarningText.rectTransform.sizeDelta = new Vector2(340, 88);
        WarningText.rectTransform.anchoredPosition = new Vector2(243, -78);
        GameUiStyle.TextStyle(WarningText, 19, 17, GameUiStyle.Paper);
        WarningText.alignment = TextAnchor.UpperCenter;
        GameUiStyle.TextStyle(MinMeatText, 17, 16, GameUiStyle.Paper);
        GameUiStyle.TextStyle(MinWaterText, 17, 16, GameUiStyle.Paper);
        foreach (var label in ItemNumTexts)
        {
            GameUiStyle.TextStyle(label, Mathf.Max(22, label.fontSize), 20, GameUiStyle.Ink);
            label.fontStyle = FontStyle.Bold;
        }
    }

    public void ReadyAdventure(StageDayData _stageDayData)
    {
        if (preparedDayId == _stageDayData.StageDayID && (journeyReady || journeyInProgress || HasJourneyResult))
            return;
        preparedDayId = _stageDayData.StageDayID;
        HasJourneyResult = false;
        LastJourneyResult = default;
        journeyReady = true;
        journeyInProgress = false;
        StopAllCoroutines();
        DOTween.Kill(this);
        Time.timeScale = 0;
        isClicked = true;

        //productPanel.SetActive(true);
        //productPanel.GetComponent<Image>().DOFade(0,1.5f);

        //perfectStr = LoadGameData.Instance.GetString("Journey_t4");
        //normalStr = LoadGameData.Instance.GetString("Journey_t5");
        //failStr = LoadGameData.Instance.GetString("Journey_t6");


        journeyData = LoadGameData.Instance.journeyDatas[_stageDayData.JourneyID];
        jStateDataList = LoadGameData.Instance.Catalog.GetJourneyStates(journeyData.JStateGroupID);

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

        if(Clock.Instance.day != 1)
        {
            StartCoroutine(NightProduct());
        }
        else
        {
            isClicked = false;
            BlackPanel.SetActive(false);
            ProductMgr.Instance.MainCam.GetComponent<AudioSource>().volume =
                SaveDataManager.Instance.saveOptionData.BgmVolume;
        }

        if(isFirstAdventure)
        {
            isFirstAdventure = false;
            TutorialMgr.Instance.OpenTutorial("Open_Tutorial_Journey_1");
        }

    }

    IEnumerator NightProduct()
    {
        Color color;
        color = NightFilter.GetComponent<Image>().color;
        color.a = 0.8f;
        NightFilter.GetComponent<Image>().color = color;

        color.a = 1f;

        NightSky.GetComponent<Image>().color = color;
        NightStar.GetComponent<Image>().color = color;
        NightGob1.GetComponent<RectTransform>().anchoredPosition = new Vector2(-250f,15f);
        NightGob2.GetComponent<RectTransform>().anchoredPosition = new Vector2(330f,15f);

        color = AdvPanel.GetComponent<Image>().color;
        color.a = 0f;
        AdvPanel.GetComponent<Image>().color = color;

        color.a = 0.6f;

        yield return new WaitForSecondsRealtime(3f);
        
        if(FenceMgr.Instance.isInvade)
        {
            NightGob1.GetComponent<RectTransform>().DOAnchorPosX(300, 1f).SetUpdate(true).SetId(this).SetLink(gameObject).OnComplete(() =>
            {
                NightGob2.GetComponent<RectTransform>().DOAnchorPosX(-400, 1f).SetDelay(1f).SetUpdate(true).SetId(this).SetLink(gameObject).OnComplete(() =>
                {
                    //yield return new WaitForSecondsRealtime(0.5f);
                    
                    BlackPanel.GetComponent<Image>().DOFade(0,1f).SetDelay(0.5f).SetUpdate(true).SetId(this).SetLink(gameObject).OnStart(() =>
                    {
                        audioSource.Play();
                        ProductMgr.Instance.MainCam.GetComponent<AudioSource>().DOFade(SaveDataManager.Instance.saveOptionData.BgmVolume, 0.5f).SetUpdate(true).SetId(this).SetLink(gameObject);
                        AdvPanel.GetComponent<Image>().color = color;
                    });
                    NightFilter.GetComponent<Image>().DOFade(0,1f).SetDelay(0.5f).SetUpdate(true).SetId(this).SetLink(gameObject);
                    NightSky.GetComponent<Image>().DOFade(0,1f).SetDelay(0.5f).SetUpdate(true).SetId(this).SetLink(gameObject);
                    NightStar.GetComponent<Image>().DOFade(0,1f).SetDelay(0.5f).SetUpdate(true).SetId(this).SetLink(gameObject).OnComplete(() =>
                    {
                        //
                        isClicked = false;
                        BlackPanel.SetActive(false);
                        

                    });
                });
            });
        }
        else
        {
            yield return new WaitForSecondsRealtime(0.5f);
            audioSource.Play();
            ProductMgr.Instance.MainCam.GetComponent<AudioSource>().DOFade(SaveDataManager.Instance.saveOptionData.BgmVolume, 0.5f).SetUpdate(true).SetId(this).SetLink(gameObject);
            BlackPanel.GetComponent<Image>().DOFade(0,1f).SetUpdate(true).SetId(this).SetLink(gameObject);
            NightFilter.GetComponent<Image>().DOFade(0,1f).SetUpdate(true).SetId(this).SetLink(gameObject);
            NightSky.GetComponent<Image>().DOFade(0,1f).SetUpdate(true).SetId(this).SetLink(gameObject);
            AdvPanel.GetComponent<Image>().color = color;
            NightStar.GetComponent<Image>().DOFade(0,1f).SetUpdate(true).SetId(this).SetLink(gameObject).OnComplete(() =>
            {
                //
                isClicked = false;
                BlackPanel.SetActive(false);
                
            });
        }
        
        
        yield return null;
    }

    void StartButton()
    {
        if (isClicked || !CanDepart || !StartAdvBtn.interactable || !AdvPanel.activeInHierarchy ||
            !ItemManager.Instance.TrySpend(new ItemCost(InventoryRules.Meat, meat),
                new ItemCost(InventoryRules.Water, water), new ItemCost(InventoryRules.Food, food),
                new ItemCost(InventoryRules.Herb, hub)))
            return;
        isClicked = true;
        journeyReady = false;
        journeyInProgress = true;
        departureFood = food;
        departureHeroCount = 0;
        for (int i = 0; i < departingHeroes.Length; i++)
        {
            departingHeroes[i] = i < HeroStateManager.Instance.heroStates.Length &&
                HeroStateManager.Instance.heroStates[i].IsAbleToAct;
            if (departingHeroes[i]) departureHeroCount++;
        }
        Time.timeScale = 1;
        StartCoroutine(StartAdventereCoroutine());
    }

    IEnumerator StartAdventereCoroutine()
    {
        TitleCharImg.transform.DOKill();
        TitleCharImg.transform.DOLocalMoveX(CharTargetX, delayTime).SetLink(gameObject);
        yield return new WaitForSeconds(delayTime);
        StartAdventure();
    }

    void StartAdventure()
    {
        for (int i = 0; i < Heros.Length && i < departingHeroes.Length; i++)
        {
            if (departingHeroes[i]) Heros[i].GetComponent<HeroMove>().SetAdv();
        }
        Time.timeScale = timeScale;
        var position = TitleCharImg.transform.localPosition;
        position.x = -CharTargetX;
        TitleCharImg.transform.localPosition = position;
        AdvPanel.SetActive(false);
        isClicked = false;
        HeroStateManager.Instance.SlideAll(true);
        InGameMgr.Instance.state = State.Camp;

        EventMgr.Instance.ChoseEvent();

        if (isFirstMini)
        {
            isFirstMini = false;
            TutorialMgr.Instance.isCollect = true;
            TutorialMgr.Instance.OpenTutorial("Open_Tutorial_Collect_1");
            
        }
    }

    public void EndJourney()
    {
        ResolveJourney(Random.value);
    }

    /// <summary>Applies the current expedition once. A supplied sample supports reproducible validation.</summary>
    public bool ResolveJourney(float unitSample)
    {
        if (!journeyInProgress)
            return false;
        if (!WeightedSelection.TryPick(jResultDataList, ResultWeight, unitSample, out JResultData jResultData))
        {
            Debug.LogError("The current journey has no valid weighted results.", this);
            return false;
        }
        journeyInProgress = false;
        HasJourneyResult = true;
        LastJourneyResult = new MeadowJourneyResult(jResultData.JResultID,
            MeadowRules.FoodRecovery(departureFood), departureHeroCount);

        for(int i=0;i<4;i++)
        {
            if (departingHeroes[i]) Heros[i].GetComponent<HeroMove>().ComeBackHome();
        }

        string msg = LoadGameData.Instance.GetString(jResultData.JRanStringID);
        Notice.Instance.InstNoticeText(msg);

        ApplyJourneyReward(0, jResultData, jResultData.Hero1_Stress, jResultData.Hero1_Power, jResultData.Hero1_Hp, jResultData.Hero1_Exp);
        ApplyJourneyReward(1, jResultData, jResultData.Hero2_Stress, jResultData.Hero2_Power, jResultData.Hero2_Hp, jResultData.Hero2_Exp);
        ApplyJourneyReward(2, jResultData, jResultData.Hero3_Stress, jResultData.Hero3_Power, jResultData.Hero3_Hp, jResultData.Hero3_Exp);
        ApplyJourneyReward(3, jResultData, jResultData.Hero4_Stress, jResultData.Hero4_Power, jResultData.Hero4_Hp, jResultData.Hero4_Exp);
        GamepadInput.Pulse(LastJourneyResult.IsSuccess ? 0.2f : 0.65f, 0.4f, 0.3f);
        return true;
    }

    void ApplyJourneyReward(int index, JResultData result, float stress, float power, float hp, float courage)
    {
        var hero = HeroStateManager.Instance.heroStates[index];
        if (!departingHeroes[index] || !hero.IsAbleToAct) return;
        var reward = CampaignProgressionRules.IsPlayableStage(InGameMgr.Instance.stage)
            ? MeadowRules.Reward(result.JResultID, stress, power, hp, courage)
            : new MeadowHeroReward(stress, power, hp, courage);
        float returnedHp = MeadowRules.ReturnedHealth(hero.hp, reward.Hp, departureFood, hero.MaximumHp);
        hero.AddStat(reward.Stress, reward.Power, returnedHp - hero.hp, reward.Courage);
    }

    float ResultWeight(JResultData result) => MeadowRules.ResultWeight(result.JResultID, result.Prob, preparedCampaignJourney);

    

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
        AdjustProvision(_idx, -1);
    }

    public void AddItemBtn(int _idx)
    {
        AdjustProvision(_idx, 1);
    }

    void AdjustProvision(int index, int delta)
    {
        if (isClicked || !journeyReady || index < 0 || index >= 4)
            return;
        int[] amounts = { meat, water, food, hub };
        string[] codes = { InventoryRules.Meat, InventoryRules.Water, InventoryRules.Food, InventoryRules.Herb };
        long next = (long)amounts[index] + delta;
        if (next < 0 || next > ItemManager.Instance.GetAmount(codes[index]))
            return;

        switch (index)
        {
            case 0: meat = (int)next; break;
            case 1: water = (int)next; break;
            case 2: food = (int)next; break;
            case 3: hub = (int)next; break;
        }
        ItemNumTexts[index].text = next.ToString();
        ItemTextColorSet();
        CheckAdventureState();
    }

    void CheckAdventureState()
    {
        if (jStateDataList.Count == 0)
        {
            StartAdvBtn.interactable = false;
            Debug.LogError($"No journey states for {journeyData.JStateGroupID}.", this);
            return;
        }
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

        jResultDataList = LoadGameData.Instance.Catalog.GetJourneyResults(jStateData.JResultGroupID);
        preparedCampaignJourney = CampaignProgressionRules.IsPlayableStage(InGameMgr.Instance.stage) &&
            jStateData.JStateStringID == "JS_t1" && meat >= MinimumMeat && water >= MinimumWater;

        /*
        string str = "";
        str += string.Format(perfectStr, (int)(jResultDataList[0].Prob * 100)) + "\n" +
            string.Format(normalStr, (int)(jResultDataList[1].Prob * 100)) + "\n" +
            string.Format(failStr, (int)(jResultDataList[2].Prob * 100));
        */

        ResultPerfectRateText.text = ResultPercentage("_Perfect");
        ResultSuccesRateText.text = ResultPercentage("_Normal");
        ResultFailRateText.text = ResultPercentage("_Fail");
        StartAdvBtn.interactable = CanDepart;
        string supplyWarning = meat < journeyData.Item1Min || water < journeyData.Item2Min
            ? "최소 보급 미달: 위험" : "최소 보급 충족 · 예상 확률을 확인하세요";
        WarningText.text = !HeroStateManager.Instance.HasAbleHeroes ? "출전할 수 있는 용사가 없습니다." :
            $"{supplyWarning}\n음식 귀환 회복 +{MeadowRules.FoodRecovery(food)}/명";
        WarningText.color = !HeroStateManager.Instance.HasAbleHeroes || meat < journeyData.Item1Min || water < journeyData.Item2Min
            ? GameUiStyle.WarningText : GameUiStyle.PositiveText;

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
            if (!HeroStateManager.Instance.heroStates[i].IsAbleToAct) continue;
            totalHeroStress += HeroStateManager.Instance.heroStates[i].stress;
            totalHeroPower += HeroStateManager.Instance.heroStates[i].power;
        }

        value = (journeyData.MeatValue * meat) + (journeyData.WaterValue * water) +
            (journeyData.HubValue * hub) + (journeyData.FoodValue * food) +
            (journeyData.HStressValue * totalHeroStress) + (journeyData.HPowerValue * totalHeroPower);

        return value;
    }

    string ResultPercentage(string suffix)
    {
        double total = 0;
        double category = 0;
        foreach (var result in jResultDataList)
        {
            float weight = ResultWeight(result);
            if (weight <= 0f || float.IsNaN(weight) || float.IsInfinity(weight))
                continue;
            total += weight;
            if (result.JResultID.EndsWith(suffix, System.StringComparison.Ordinal))
                category += weight;
        }
        return total > 0 ? (category / total * 100).ToString("0.#") : "0";
    }

    protected override void OnDestroy()
    {
        DOTween.Kill(this);
        if (StartAdvBtn != null)
            StartAdvBtn.onClick.RemoveListener(StartButton);
        if (TitleCharImg != null)
            TitleCharImg.transform.DOKill();
        base.OnDestroy();
    }
}
