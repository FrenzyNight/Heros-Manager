using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FireInteractionMgr : CampInteractionMgr
{
    public int needAmount;
    public float time1, time2, time3;
    public float activeTime, graceTime;
    public float hero1Stress, hero2Stress, hero3Stress, hero4Stress;
    public string BonfireID;
    public string NeedItemID;


    
    // Start is called before the first frame update
    void Start()
    {
        Setup();
    }

    // Update is called once per frame
    void Update()
    {
        if(isActive && !isGrace)
        {
            activeTime -= Time.deltaTime;
            ActiveGage.fillAmount = activeTime / time1;
        }
        else if(isGrace)
        {
            graceTime -= Time.deltaTime;
            GraceGage.fillAmount = graceTime / time2;
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

            CampObjMgr.GetComponent<FireMgr>().NonFire();
        }

        if(!isActive)
        {
            AvailableButton();
        }
    }

    public override void Setup()
    {
        isActive = false;
        switch(InGameMgr.Instance.stage)
        {
            case 1:
                BonfireID = "Bonfire_S1";
                break;
            case 2:
                BonfireID = "Bonfire_S2";
                break;
            case 3:
                BonfireID = "Bonfire_S3";
                break;
        }

        NeedItemID = LoadGameData.Instance.bonfireDatas[BonfireID].NeedItemID;
        needAmount = LoadGameData.Instance.bonfireDatas[BonfireID].NeedAmount;

        StringText.text = needAmount.ToString();

        time1 = LoadGameData.Instance.bonfireDatas[BonfireID].Time1;
        time2 = LoadGameData.Instance.bonfireDatas[BonfireID].Time2;
        time3 = LoadGameData.Instance.bonfireDatas[BonfireID].Time3;

        hero1Stress = LoadGameData.Instance.bonfireDatas[BonfireID].Hero1_Stress;
        hero2Stress = LoadGameData.Instance.bonfireDatas[BonfireID].Hero2_Stress;
        hero3Stress = LoadGameData.Instance.bonfireDatas[BonfireID].Hero3_Stress;
        hero4Stress = LoadGameData.Instance.bonfireDatas[BonfireID].Hero4_Stress;
    }

    void Grace()
    {
        AvailableButton();
    }

    IEnumerator GetStress()
    {
        yield return null;
    }

    public override void ClickButton()
    {
        Debug.Log("Button Click");
        if(ItemManager.Instance.GetItemInfo(NeedItemID).num < needAmount)
            return;

        ItemManager.Instance.AddItem(NeedItemID, -needAmount);
        
        isActive = true;
        isGrace = false;
        activeTime = time1;
        graceTime = time2;

        ActiveGage.fillAmount = activeTime / time1;
        GraceGage.fillAmount = graceTime / time2;

        CampObjMgr.GetComponent<FireMgr>().OnFire();
        UnavailableButton();
    }
}
