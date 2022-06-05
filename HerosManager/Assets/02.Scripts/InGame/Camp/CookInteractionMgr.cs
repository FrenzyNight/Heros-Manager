using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CookInteractionMgr : CampInteractionMgr
{
    public GameObject TrashGage;
    public GameObject FireInterMgr;
    public Sprite CookButton, FoodButton, TrashButton;
    public GameObject NeedUI;
    

    public string cookID;
    public string needItem1ID;
    public string needItem2ID;
    public string needItem3ID;
    public int needItem1Amount;
    public int needItem2Amount;
    public int needItem3Amount;

    public string getItemID;
    public int getItemAmount;
    public float cookTime;
    public float getTime;

    public int buttonState; // 0=cook, 1=food, 2=trash


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
        if(isActive && !isGrace && FireInterMgr.GetComponent<FireInteractionMgr>().isActive)
        {
            activeTime -= Time.deltaTime;
            ActiveGage.fillAmount = activeTime / cookTime;
        }
        else if(isGrace && FireInterMgr.GetComponent<FireInteractionMgr>().isActive)
        {
            GraceGage.sprite = Blue;
            graceTime -= Time.deltaTime;
            GraceGage.fillAmount = graceTime / getTime;
        }

        if(isActive && !isGrace && activeTime <= 0)
        {
            AvailableButton();
            isGrace = true;
        }
        else if(isGrace && graceTime <= 0)
        {
            isGrace = false;
            isActive = false;

            SetTrash();
        }

        if(!isActive && buttonState == 0 && FireInterMgr.GetComponent<FireInteractionMgr>().isActive)
        {
            AvailableButton();
        }

        if(buttonState == 0 &&!FireInterMgr.GetComponent<FireInteractionMgr>().isActive)
        {
            UnavailableButton();
        }

    }

    public override void Setup()
    {
        isActive = false;
        ButtonImg = ActiveButton.GetComponent<Image>();
        ButtonImg.sprite = CookButton;
        buttonState = 0;
        UnavailableButton();

        switch(InGameMgr.Instance.stage)
        {
            case 1:
                cookID = "Cook_S1";
                break;
            case 2:
                cookID = "Cook_S2";
                break;
            case 3:
                cookID = "Cook_S3";
                break;
        }

        needItem1ID = LoadGameData.Instance.cookDatas[cookID].NeedItem1ID;
        needItem2ID = LoadGameData.Instance.cookDatas[cookID].NeedItem2ID;
        needItem3ID = LoadGameData.Instance.cookDatas[cookID].NeedItem3ID;

        needItem1Amount = LoadGameData.Instance.cookDatas[cookID].NeedItem1Amount;
        needItem2Amount = LoadGameData.Instance.cookDatas[cookID].NeedItem2Amount;
        needItem3Amount = LoadGameData.Instance.cookDatas[cookID].NeedItem3Amount;

        StringText.text = needItem1Amount.ToString() + "   " + needItem2Amount.ToString() + "   " + needItem3Amount.ToString(); 

        getItemID = LoadGameData.Instance.cookDatas[cookID].GetItemID;
        getItemAmount = LoadGameData.Instance.cookDatas[cookID].GetItemAmount;

        cookTime = LoadGameData.Instance.cookDatas[cookID].CookTime;
        getTime = LoadGameData.Instance.cookDatas[cookID].GetTime;
        
    }

    public override void NextDayAction()
    {
        SetTrash();
    }

    void StartCook()
    {
        if(ItemManager.Instance.GetItemInfo(needItem1ID).num < needItem1Amount || ItemManager.Instance.GetItemInfo(needItem2ID).num < needItem2Amount ||ItemManager.Instance.GetItemInfo(needItem3ID).num < needItem3Amount)
            return;

        ItemManager.Instance.AddItem(needItem1ID, -needItem1Amount);
        ItemManager.Instance.AddItem(needItem2ID, -needItem2Amount);
        ItemManager.Instance.AddItem(needItem3ID, -needItem3Amount);

        GraceGage.sprite = Green;

        ButtonImg.sprite = FoodButton;

        isActive = true;
        isGrace = false;
        activeTime = cookTime;
        graceTime = getTime;

        buttonState = 1;

        ActiveGage.fillAmount = activeTime / cookTime;
        GraceGage.fillAmount = graceTime / getTime;

        CampObjMgr.GetComponent<FireMgr>().OnCook();
        UnavailableButton();
        NeedUI.SetActive(false);
    }

    void GetFood()
    {
        UnavailableButton();
        ItemManager.Instance.AddItem(getItemID, getItemAmount);
        buttonState = 0;
        ButtonImg.sprite = CookButton;
        isActive = false;
        isGrace = false;

        activeTime = 0;
        graceTime = 0;

        ActiveGage.fillAmount = activeTime / cookTime;
        GraceGage.fillAmount = graceTime / getTime;

        CampObjMgr.GetComponent<FireMgr>().NonCook();
        NeedUI.SetActive(true);
    }

    void SetTrash()
    {
        Notice.Instance.InstNoticeText(LoadGameData.Instance.GetString("Cook_a1"));
        buttonState = 2;
        ButtonImg.sprite = TrashButton;
        //TrashGage.SetActive(true);
        isActive = false;
        isGrace = false;

        ActiveGage.fillAmount = 1;
        GraceGage.fillAmount = 1;

        ActiveGage.sprite = Red;
        GraceGage.sprite = Red;

        AvailableButton();
    }

    void ClearTrash()
    {
        buttonState = 0;
        ButtonImg.sprite = CookButton;
        TrashGage.SetActive(false);

        ActiveGage.fillAmount = 0;
        GraceGage.fillAmount = 0;

        ActiveGage.sprite = Green;
        GraceGage.sprite = Green;


        TrashGage.SetActive(false);
        UnavailableButton();
        CampObjMgr.GetComponent<FireMgr>().NonCook();
        NeedUI.SetActive(true);
    }

    public override void ClickButton()
    {
        switch(buttonState)
        {
            case 0:
                StartCook();
                break;
            case 1:
                GetFood();
                break;
            case 2:
                ClearTrash();
                break;
        }

    }
}
