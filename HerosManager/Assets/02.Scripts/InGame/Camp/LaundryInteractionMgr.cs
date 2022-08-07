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
    [HideInInspector] public int l1CountValue;
    [HideInInspector] public float l1ExitTime;
    [HideInInspector] public float l1MinTime;
    [HideInInspector] public float l1MaxTime;
    [HideInInspector] public float l2CountValue;
    [HideInInspector] public float l3MinTime;
    [HideInInspector] public float l3MaxTime;
    [HideInInspector] public float l3TotalTime;
    [HideInInspector] public int hero1Stress;
    [HideInInspector] public int hero2Stress;
    [HideInInspector] public int hero3Stress;
    [HideInInspector] public int hero4Stress;
    
    [Header("LaundryUI")]
    public GameObject laundryUIPanel;
    public Button laundryUIButton;
    public Text laundryUIButtonText;
    public Text laundryNeedItemText;
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
        
    }

    public void LaundryUIButtonClick()
    {
        switch (laundryState)
        {
            case 0:
                L1Active();
                break;
            case 1:
                break;
            case 2:
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
        laundryState = 0;
        
        switch (InGameMgr.Instance.stage)
        {
            case 1:
                //
                break;
            case 2:
                //
                break;
            case 3:
                //
                break;
            default:
                break;

        }
    }

    public void L1Active()
    {
        l1Panel.SetActive(true);
        laundryState = 1;
        laundryUIButton.interactable = false;
    }

    public void L1SpawnGrime()
    {
        int rnd = Random.Range(0, 2);
        GameObject grime = l1GrimePrefab[rnd];
        l1Grimes.Add(Instantiate(grime, l1GrimeArea.transform));
        //random transform;
    }

    public void L1CleanGrime()
    {
        l1Count += 1;
        
        

        laundryGageImg.fillAmount = l1Count / l1CountValue;

        if (l1Count == l1CountValue)
        {
            L1Clear();
        }
    }

    public void L1Clear()
    {
        l1ClearImage.SetActive(true);
        foreach (GameObject obj in l1Grimes)
        {
            Destroy(obj);
        }

        l1Grimes.Clear();
        laundryUIButton.interactable = true;
    }

    public override void ClickButton()
    {
        laundryUIPanel.SetActive(true);
        //InGameMgr.Instance.state = State.MiniGame;
    }

    public override void NextDayAction()
    {
        
    }
}
