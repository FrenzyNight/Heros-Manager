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
    [HideInInspector] public float l2CountNum;
    [HideInInspector] public float l2DelayTime;
    [HideInInspector] public float l3MinTime;
    [HideInInspector] public float l3MaxTime;
    [HideInInspector] public float l3TotalTime;
    [HideInInspector] public int hero1Stress;
    [HideInInspector] public int hero2Stress;
    [HideInInspector] public int hero3Stress;
    [HideInInspector] public int hero4Stress;

    [Header("LaundryUI")] 
    public Button laundryInterButton;
    public GameObject laundryUIPanel;
    public Button laundryUIButton;
    public Text laundryUIButtonText;
    public GameObject laundryNeedItemUI;
    public Text laundryNeedItemText;
    public GameObject laundryGaugeUI;
    public GameObject laundryGauge;
    public GameObject[] laundryGauges;
    [HideInInspector] public int laundryState; 

    [Header("L1 Objects")]
    public GameObject l1Panel;
    public GameObject[] l1GrimePrefab;
    public GameObject l1GrimeArea;
    public GameObject l1ClearImage;
    public int l1Count;
    [HideInInspector] public List<GameObject> l1Grimes = new List<GameObject>();
    [HideInInspector] public BoxCollider2D areaCol;
    
    [Header("L2 Objects")] 
    public GameObject l2Panel;
    public GameObject l2PressA;
    public GameObject l2PressD;

    [Header("L3 Objects")]
    public GameObject l3Panel;

    [HideInInspector] public Image laundryGageImg;
    
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
    }

    public void ActiveLaundry()
    {
        laundryInterButton.interactable = true;
    }

    public void DeactiveLaundry()
    {
        laundryInterButton.interactable = false;
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
                break;
            default:
                break;
        }
    }
    

    public override void Setup()
    {
        laundryGageImg = laundryGauge.GetComponent<Image>();
        l1Count = 0;
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
        
        areaCol = l1GrimeArea.GetComponent<BoxCollider2D>();

        foreach (GameObject obj in laundryGauges)
        {
            obj.SetActive(false);
        }
        
    }

    public void L1Active()
    {
        l1Panel.SetActive(true);
        laundryState = 1;
        laundryUIButton.interactable = false;
        laundryNeedItemUI.SetActive(false);
        laundryGaugeUI.SetActive(true);
        StartCoroutine(GrimeSpawn());
    }

    IEnumerator GrimeSpawn()
    {
        while (true)
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

        obj.GetComponent<LaundryGrimeScript>().InterMgr = gameObject;
        obj.GetComponent<LaundryGrimeScript>().GrimeSet(l1ExitTime);
        
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
        Debug.Log("Grime Click");

        laundryGageImg.fillAmount = l1Count / l1CountNum;

        if (l1Count == l1CountNum)
        {
            L1Clear();
        }
    }

    public void L1Clear()
    {
        l1ClearImage.SetActive(true);
        StopCoroutine(GrimeSpawn());
        
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
        l1ClearImage.SetActive(false);
        l1Panel.SetActive(false);
        laundryUIButton.interactable = false;
        
        l2Panel.SetActive(true);
    }

    public void L2Clear()
    {
        
    }

    public void L3Active()
    {
        
    }

    public void L3Clear()
    {
        
    }

    public override void ClickButton()
    {
        laundryUIPanel.SetActive(true);
        //InGameMgr.Instance.state = State.MiniGame;
        //set first scene
    }

    public override void NextDayAction()
    {
        //clear
        
        //fail
        //increase stress
    }
}
