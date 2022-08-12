using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class LaundryInteractionMgr : CampInteractionMgr
{
   
    //Laundry Data
    [HideInInspector] public string laundryID;
    [HideInInspector] public int needWaterAmount;
    [HideInInspector] public int l1CountNum;
    [HideInInspector] public float l1ExitTime;
    [HideInInspector] public float l1MinTime;
    [HideInInspector] public float l1MaxTime;
    [HideInInspector] public int l2CountNum;
    [HideInInspector] public float l2DelayTime;
    [HideInInspector] public float l3MinTime;
    [HideInInspector] public float l3MaxTime;
    [HideInInspector] public float l3TotalTime;
    [HideInInspector] public int hero1Stress;
    [HideInInspector] public int hero2Stress;
    [HideInInspector] public int hero3Stress;
    [HideInInspector] public int hero4Stress;

    [Header("LaundryUI")] 
    public Button laundryExitButton;
    public GameObject laundryMgr;
    public Button laundryInterButton;
    public GameObject laundryUIPanel;
    public Button laundryUIButton;
    public Text laundryUIButtonText;
    public GameObject laundryNeedItemUI;
    public Text laundryNeedItemText;
    public GameObject laundryGaugeUI;
    public GameObject laundryGauge;
    public GameObject[] laundryGauges;
    public GameObject laundryGuid;
    public Text laundryGuidText;
    [HideInInspector] public int laundryState;
    [HideInInspector] public bool isLaundryClear;

    [Header("L1 Objects")]
    public GameObject l1Panel;
    public GameObject[] l1GrimePrefab;
    public GameObject l1GrimeArea;
    public GameObject l1ClothObject;
    public GameObject l1ClearImage;
    [HideInInspector] public int l1Count;
    [HideInInspector] public List<GameObject> l1Grimes = new List<GameObject>();
    [HideInInspector] public BoxCollider2D areaCol;
    [HideInInspector] public Image laundryGageImg;
    
    [Header("L2 Objects")] 
    public GameObject l2Panel;
    public GameObject l2AniObj;
    public GameObject l2PressA;
    public GameObject l2PressD;
    public GameObject l2PressATextObj;
    public GameObject l2PressDTextObj;
    public Sprite l2OnPressImg;
    public Sprite l2OffPressImg;
    public float l2AniDelay;
    [HideInInspector] public RectTransform l2ButtonARt;
    [HideInInspector] public RectTransform l2ButtonDRt;
    [HideInInspector] public Animator l2Animator;
    [HideInInspector] public float l2AniTimeCount;
    [HideInInspector] public int l2Count;
    [HideInInspector] public int l2NextButton; // 0 == A . 1 == D
    [HideInInspector] public Image l2ButtonAImg;
    [HideInInspector] public Image l2ButtonDImg;
    [HideInInspector] public bool l2CanPress;

    [Header("L3 Objects")]
    public GameObject l3Panel;
    public GameObject l3TargetTextObj;
    public GameObject l3Button;
    public GameObject l3AniObj;
    public float l3AniDelay;
    [HideInInspector] public Image l3ButtonImg;
    [HideInInspector] public Text l3TargetText;
    [HideInInspector] public RectTransform l3ButtonTextRT;
    [HideInInspector] public KeyCode l3TargetKeyCode;
    [HideInInspector] public bool l3CanPress;
    [HideInInspector] public float l3TimeCount;
    [HideInInspector] public string l3TargetKey;
    [HideInInspector] public string[] l3KeyList = new string[] {"A","B", "C", "D", "E","F","G","H","I","J","K","L","M","N","O","P","Q","R","S","T","U","V","W","X","Y","Z"};
    [HideInInspector] public List<KeyCode> l3KeyCodeList = new List<KeyCode>();
    [HideInInspector] public Animator l3Animator;
    [HideInInspector] public float l3AniTimeCount;
    
    [Header("Laundry Clear")] 
    public GameObject laundryClearPanel;
    
    //Function
    private Coroutine _runningCoroutine = null;
    
    void Start()
    {
        Setup();
    }

    void Update()
    {
        if (Clock.Instance.isHeroBack && laundryState == -1)
        {
            laundryState = 0;
            ActiveLaundry();
        }

        if (laundryState == 1) //test
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                L1Clear();
            }
        }
        
        else if (laundryState == 2 && l2CanPress)
        {
            if (Input.GetKeyDown(KeyCode.A) && l2NextButton == 0)
            {
                L2PressKey(0);
            }
            
            else if (Input.GetKeyDown(KeyCode.D) && l2NextButton == 1)
            {
                L2PressKey(1);
            }

           
            if (l2AniTimeCount < l2AniDelay)
            {
                l2AniTimeCount += Time.deltaTime;
            }
            else
            {
                l2Animator.speed = 0f;
            }
        }
        
        else if (laundryState == 3 && l3CanPress)
        {
            if (Input.GetKey(l3TargetKeyCode))
            {
                l3AniTimeCount = 0;
                l3ButtonImg.sprite = l2OffPressImg;
                l3ButtonTextRT.anchoredPosition = new Vector2(0, 0);
                l3TimeCount += Time.deltaTime;
                laundryGageImg.fillAmount = l3TimeCount / l3TotalTime;

                if (l3TimeCount >= l3TotalTime)
                {
                    L3Clear();
                }
            }

            if (Input.GetKeyDown(l3TargetKeyCode))
            {
                l3Animator.speed = 0.1f;
                l3ButtonImg.sprite = l2OffPressImg;
                l3ButtonTextRT.anchoredPosition = new Vector2(0, 0);
            }
            else if (Input.GetKeyUp(l3TargetKeyCode))
            {
                l3ButtonImg.sprite = l2OnPressImg;
                l3ButtonTextRT.anchoredPosition = new Vector2(0, 5);
            }

            if (l3AniTimeCount < l3AniDelay)
            {
                l3AniTimeCount += Time.deltaTime;
            }
            else
            {
                l3Animator.speed = 0f;
            }
        }
    }

    public void ActiveLaundry()
    {
        laundryInterButton.interactable = true;
        laundryMgr.GetComponent<LaundryMgr>().OnWash();
    }

    public void DeactiveLaundry()
    {
        laundryInterButton.interactable = false;
        laundryMgr.GetComponent<LaundryMgr>().NonWash();
    }

    public void LaundryUIButtonClick()
    {
        switch (laundryState)
        {
            case 0:
                L1Active();
                break;
            case 1:
                L2Active();
                break;
            case 2:
                L3Active();
                break;
            case 3:
                ExitLaundryWindow();
                break;
            default:
                break;
        }
    }
    

    public override void Setup()
    {
        laundryMgr.GetComponent<LaundryMgr>().NonWash();
        isLaundryClear = false;
        laundryGageImg = laundryGauge.GetComponent<Image>();
        l1Count = 0;
        l2Count = 0;
        l3TimeCount = 0;
        laundryState = -1;
        
        switch (InGameMgr.Instance.stage)
        {
            case 1:
                laundryID = "Laundry_1";
                break;
            case 2:
                laundryID = "Laundry_2";
                break;
            case 3:
                laundryID = "Laundry_3";
                break;
            default:
                break;
        }

        needWaterAmount = LoadGameData.Instance.laundryDatas[laundryID].NeedWaterAmount;
        l1CountNum = LoadGameData.Instance.laundryDatas[laundryID].L1_GermCount;
        l1ExitTime = LoadGameData.Instance.laundryDatas[laundryID].L1_GermTime;
        l1MinTime = LoadGameData.Instance.laundryDatas[laundryID].L1_JenTimeMin;
        l1MaxTime = LoadGameData.Instance.laundryDatas[laundryID].L1_JenTimeMax;

        l2CountNum = LoadGameData.Instance.laundryDatas[laundryID].L2_RinseCount;
        l2DelayTime = LoadGameData.Instance.laundryDatas[laundryID].L2_DelayTime;

        l3MinTime = LoadGameData.Instance.laundryDatas[laundryID].L3_MinTime;
        l3MaxTime = LoadGameData.Instance.laundryDatas[laundryID].L3_MaxTime;
        l3TotalTime = LoadGameData.Instance.laundryDatas[laundryID].L3_TotalTime;

        hero1Stress = LoadGameData.Instance.laundryDatas[laundryID].Hero1_Stress;
        hero2Stress = LoadGameData.Instance.laundryDatas[laundryID].Hero2_Stress;
        hero3Stress = LoadGameData.Instance.laundryDatas[laundryID].Hero3_Stress;
        hero4Stress = LoadGameData.Instance.laundryDatas[laundryID].Hero4_Stress;

        laundryNeedItemUI.SetActive(true);
        laundryNeedItemText.text = "-" + needWaterAmount.ToString();
        laundryGaugeUI.SetActive(false);
        laundryGauge.GetComponent<Image>().fillAmount = 0;
        laundryUIButton.onClick.AddListener(LaundryUIButtonClick);

        laundryInterButton.onClick.AddListener(ClickButton);
        laundryExitButton.onClick.AddListener(ExitLaundryWindow);
        
        areaCol = l1GrimeArea.GetComponent<BoxCollider2D>();

        foreach (GameObject obj in laundryGauges)
        {
            obj.SetActive(false);
        }
        
        //l2
        l2ButtonAImg = l2PressA.GetComponent<Image>();
        l2ButtonDImg = l2PressD.GetComponent<Image>();
        l2CanPress = false;
        l2Animator = l2AniObj.GetComponent<Animator>();
        l2Animator.speed = 0f;
        l2ButtonARt = l2PressATextObj.GetComponent<RectTransform>();
        l2ButtonDRt = l2PressDTextObj.GetComponent<RectTransform>();
        
        //l3
        l3CanPress = false;
        l3TargetText = l3TargetTextObj.GetComponent<Text>();
        l3ButtonTextRT = l3TargetTextObj.GetComponent<RectTransform>();
        l3ButtonImg = l3Button.GetComponent<Image>();
        l3Animator = l3AniObj.GetComponent<Animator>();
        l3Animator.speed = 0f;
    }

    public void L1Active()
    {
        l1Count = 0;
        l1Panel.SetActive(true);
        l1ClothObject.SetActive(true);
        laundryState = 1;
        laundryUIButton.interactable = false;
        laundryNeedItemUI.SetActive(false);
        laundryGaugeUI.SetActive(true);
        laundryGuid.SetActive(false);
        _runningCoroutine = StartCoroutine(GrimeSpawn());
        
        laundryUIButtonText.text = LoadGameData.Instance.GetString("laundry_t2");
        laundryGuidText.text = LoadGameData.Instance.GetString("laundry_a2");

        ItemManager.Instance.AddItem("Item_Water",-needWaterAmount);
    }

    IEnumerator GrimeSpawn()
    {
        while (l1Count < l1CountNum)
        {
            float rndTime = Random.Range(l1MinTime, l1MaxTime);
            
            L1SpawnGrime();

            yield return new WaitForSeconds(rndTime);
        }
    }

    public void L1SpawnGrime()
    {
        int rnd = Random.Range(0, 2);
        GameObject grime = l1GrimePrefab[rnd];
        GameObject obj = Instantiate(grime,GrimeRandomPos() ,Quaternion.identity,l1GrimeArea.transform);

        //LaundryGrimeScript grimeScript = obj.GetComponent<LaundryGrimeScript>();
        
        obj.GetComponent<LaundryGrimeScript>().GrimeSet(gameObject ,l1ExitTime);
        
        //obj.GetComponent<Button>().onClick.AddListener(L1GrimeClick);
        l1Grimes.Add(obj);
        //random transform;
    }

    public Vector3 GrimeRandomPos()
    {
        Vector3 oriPos = l1GrimeArea.transform.position;

        float rangeX = areaCol.bounds.size.x;
        float rangeY = areaCol.bounds.size.y;
        
        rangeX = Random.Range( (rangeX / 2) * -1, rangeX / 2);
        rangeY = Random.Range( (rangeY / 2) * -1, rangeY / 2);

        Vector3 randomPos = new Vector3(rangeX, rangeY, 0);

        Vector3 spawnPos = oriPos + randomPos;

        return spawnPos;
    }

    public void L1GrimeClick()
    {
        l1Count += 1;
        //Debug.Log("Grime Click");

        laundryGageImg.fillAmount = (float)l1Count / l1CountNum;

        if (l1Count == l1CountNum)
        {
            L1Clear();
        }
    }

    public void L1Clear()
    {
        l1ClothObject.SetActive(false);
        l1ClearImage.SetActive(true);
        StopCoroutine(_runningCoroutine);
        
        foreach (GameObject obj in l1Grimes)
        {
            Destroy(obj);
        }
        l1Grimes.Clear();
        
        laundryGauges[0].SetActive(true);
        laundryUIButton.interactable = true;
    }

    public void L2Active()
    {
        laundryGuidText.text = LoadGameData.Instance.GetString("laundry_a3");
        
        l2Count = 0;
        l1ClearImage.SetActive(false);
        l1Panel.SetActive(false);
        laundryUIButton.interactable = false;

        l2ButtonAImg.sprite = l2OnPressImg;
        l2ButtonARt.anchoredPosition = new Vector2(0, 5);
        l2ButtonDImg.sprite = l2OffPressImg;
        l2ButtonDRt.anchoredPosition = new Vector2(0, 0);
        
        l2PressA.SetActive(true);
        l2PressD.SetActive(true);

        l2Panel.SetActive(true);
        l2NextButton = 0; // A
        laundryState = 2;
        laundryGageImg.fillAmount = 0;
        l2CanPress = true;
    }

    public void L2PressKey(int button)
    {
        l2CanPress = false;
        l2AniTimeCount = 0f;
        l2Animator.speed = 0.1f;
        
        if (button == 0) // press A
        {
            l2NextButton = 1;
            l2ButtonAImg.sprite = l2OffPressImg;
            l2ButtonARt.anchoredPosition = new Vector2(0, 0);

            l2ButtonDImg.sprite = l2OnPressImg;
            l2ButtonDRt.anchoredPosition = new Vector2(0, 5);

        }
        else if (button == 1)
        {
            l2NextButton = 0;
            l2ButtonAImg.sprite = l2OnPressImg;
            l2ButtonARt.anchoredPosition = new Vector2(0, 5);

            l2ButtonDImg.sprite = l2OffPressImg;
            l2ButtonDRt.anchoredPosition = new Vector2(0, 0);

        }
        
        l2Count += 1;

        laundryGageImg.fillAmount = (float)l2Count / l2CountNum;

        if (l2Count == l2CountNum)
        {
            L2Clear();
        }
        else
        {
            StartCoroutine(L2PressDelay());
        }
    }

    IEnumerator L2PressDelay()
    {
        yield return new WaitForSeconds(l2DelayTime);
        l2CanPress = true;
        yield return null;
    }

    public void L2Clear()
    {
        l2CanPress = false;
        laundryGauges[1].SetActive(true);
        laundryUIButton.interactable = true;
        
        l2PressA.SetActive(false);
        l2PressD.SetActive(false);
    }

    public void L3Active()
    {
        laundryGuidText.text = LoadGameData.Instance.GetString("laundry_a4");
        l2Panel.SetActive(false);
        laundryUIButton.interactable = false;
        
        l3Panel.SetActive(true);
        l2NextButton = -1; // A
        laundryState = 3;
        laundryGageImg.fillAmount = 0;

        l3TimeCount = 0;
        l3CanPress = true;
        
        l3Button.SetActive(true);
        
        for ( KeyCode i = KeyCode.A; i <= KeyCode.Z; ++i ) 
        {  
             l3KeyCodeList.Add(i);
        } 

        _runningCoroutine = StartCoroutine(L3KeyChange());
    }

    IEnumerator L3KeyChange()
    {
        while (l3CanPress)
        {
            float keyTime = Random.Range(l3MinTime, l3MaxTime);
            
            L3SetKey();

            yield return new WaitForSeconds(keyTime);
        }
        
    }

    public void L3SetKey()
    {
        int idx = Random.Range(0, l3KeyList.Length);
        l3TargetKey = l3KeyList[idx];

        l3TargetKeyCode = l3KeyCodeList[idx];
        
        l3TargetText.text = l3TargetKey;

        l3ButtonImg.sprite = l2OnPressImg;
        l3ButtonTextRT.anchoredPosition = new Vector2(0, 5);
    }

    public void L3Clear()
    {
        StopCoroutine(_runningCoroutine);
        l3CanPress = false;
        l3Button.SetActive(false);
        laundryUIButtonText.text = LoadGameData.Instance.GetString("Fence_b3");
        
        laundryGauges[2].SetActive(true);

        laundryUIButton.interactable = true;
        isLaundryClear = true;
        laundryMgr.GetComponent<LaundryMgr>().NonWash();
        
        l3Panel.SetActive(false);
        laundryClearPanel.SetActive(true);
        
    }

    public void ExitLaundryWindow()
    {
        if (isLaundryClear)
        {
            //clear
            DeactiveLaundry();
        }
        else
        {
            //reset
            Reset();
        }
    
        laundryState = 0;
        laundryUIPanel.SetActive(false);
        InGameMgr.Instance.state = State.Camp;
    }

    public void Reset()
    {
        laundryGaugeUI.SetActive(false);
        laundryNeedItemUI.SetActive(true);
        laundryGageImg.fillAmount = 0;

        foreach (var obj in laundryGauges)
        {
            obj.SetActive(false);
        }

        StopCoroutine(_runningCoroutine);
        
        //l1
        l1Panel.SetActive(false);
        l1ClearImage.SetActive(false);
        foreach (GameObject obj in l1Grimes)
        {
            Destroy(obj);
        }
        l1Grimes.Clear();

        //l2
        l2Panel.SetActive(false);
        
        //l3
        l3Panel.SetActive(false);
    }

    public override void ClickButton()
    {
        laundryUIPanel.SetActive(true);
        InGameMgr.Instance.state = State.Laundry;
        //set first scene

        laundryUIButtonText.text = LoadGameData.Instance.GetString("laundry_t1");
        laundryGuidText.text = "";
        laundryGuid.SetActive(true);
        
        //check need item value
        if (ItemManager.Instance.GetItemInfo("Item_Water").num >= needWaterAmount)
        {
            laundryUIButton.interactable = true;
        }
    }

    public override void NextDayAction()
    {
        //fail
        if (!isLaundryClear)
        {
            //increase stress
            HeroStateManager.Instance.heroStates[0].AddStat(hero1Stress,0,0,0);
            HeroStateManager.Instance.heroStates[1].AddStat(hero2Stress,0,0,0);
            HeroStateManager.Instance.heroStates[2].AddStat(hero3Stress,0,0,0);
            HeroStateManager.Instance.heroStates[3].AddStat(hero4Stress,0,0,0);
        }
    }
}
