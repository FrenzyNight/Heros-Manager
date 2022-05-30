using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class FarmInteractionMgr : CampInteractionMgr
{

    public int needAmount;
    public float time1, time2;
    public int repeat;
    public string FieldID;
    public string NeedItemID;
    public string GetItemID;
    public int getItemAmount;

    public GameObject NeedUI;
    public Sprite HubButton, WaterButton, GetHubButton, TrashButton;
    public GameObject[] RepeatGages;
    public int buttonState; // 0 = hub, 1 = water, 2 = getgub, 3 = trash
    public int farmState; //0단계, 1단계, 2단계, 3단계
    
    [HideInInspector]
    public Image ButtonImg;
    

    // Start is called before the first frame update
    void Start()
    {
        Setup();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Plant()
    {
        NeedUI.SetActive(true);
        ButtonImg.sprite = WaterButton;
        UnavailableButton();

        isActive = true;
        isGrace = false;
        activeTime = time1;
        graceTime = time2;

        buttonState = 1;

        ActiveGage.fillAmount = activeTime / time1;
        GraceGage.fillAmount = graceTime / time2;

        CampObjMgr.GetComponent<FarmMgr>().OnFarm(farmState);
    }

    void Water()
    {
        UnavailableButton();

        isActive = true;
        isGrace = false;
        activeTime = time1;
        graceTime = time2;

        RepeatGages[farmState].SetActive(true);

        farmState += 1;

        ActiveGage.fillAmount = activeTime / time1;
        GraceGage.fillAmount = graceTime / time2;
        

        CampObjMgr.GetComponent<FarmMgr>().OnFarm(farmState);

        if(farmState == 3)
        {
            buttonState = 2;
            ButtonImg.sprite = GetHubButton;
            NeedUI.SetActive(false);
        }
    }

    void GetHub()
    {
        ItemManager.Instance.AddItem(GetItemID, getItemAmount);
        buttonState = 0;
        ButtonImg.sprite = HubButton;
        isActive = false;
        isGrace = false;

        activeTime = 0;
        graceTime = 0;

        ActiveGage.fillAmount = activeTime / time1;
        GraceGage.fillAmount = graceTime / time2;

        foreach(GameObject repeat in RepeatGages)
        {
            repeat.SetActive(false);
        }
    }

    void SetTrash()
    {
        isActive = false;
        isGrace = false;
        
    }

    void ClearTrash()
    {

    }

    public override void Setup()
    {
        buttonState = 0;
        farmState = 0;
        isActive = false;
        isGrace = false;
        ButtonImg = ActiveButton.GetComponent<Image>();
        ButtonImg.sprite = HubButton;
        NeedUI.SetActive(false);
        AvailableButton();

        switch(InGameMgr.Instance.stage)
        {
            case 1:
                FieldID = "Field_S1";
                break;
            case 2:
                FieldID = "Field_S2";
                break;
            case 3:
                FieldID = "Field_S3";
                break;
        }
        
        NeedItemID = LoadGameData.Instance.fieldDatas[FieldID].NeedItemID;
        needAmount = LoadGameData.Instance.fieldDatas[FieldID].NeedAmount;

        time1 = LoadGameData.Instance.fieldDatas[FieldID].Time1;
        time2 = LoadGameData.Instance.fieldDatas[FieldID].Time2;

        repeat = LoadGameData.Instance.fieldDatas[FieldID].Repeat;

        GetItemID = LoadGameData.Instance.fieldDatas[FieldID].GetItemID;
        getItemAmount = LoadGameData.Instance.fieldDatas[FieldID].GetAmount;

        StringText.text = needAmount.ToString();
    }

    public override void ClickButton()
    {
        switch(buttonState)
        {
            case 0:
                Plant();
                break;
            case 1:
                Water();
                break;
            case 2:
                GetHub();
                break;
            case 3:
                ClearTrash();
                break;
        }
    }
}
