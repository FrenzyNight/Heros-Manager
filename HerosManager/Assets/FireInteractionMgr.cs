using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        if(isActive)
        {
            CampObjMgr.GetComponent<FireMgr>().OnFire();
        }
        else if(!isActive)
        {
            CampObjMgr.GetComponent<FireMgr>().NonFire();
        }

        if((isActive && isGrace) || !isActive)
        {
            AvailableButton();
        }
        else if(!isGrace && isActive)
        {
            UnavailableButton();
        }
    }

    void Setup()
    {
        isActive = false;
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

        time1 = LoadGameData.Instance.bonfireDatas[BonfireID].Time1;
        time2 = LoadGameData.Instance.bonfireDatas[BonfireID].Time2;
        time3 = LoadGameData.Instance.bonfireDatas[BonfireID].Time3;

        hero1Stress = LoadGameData.Instance.bonfireDatas[BonfireID].Hero1_Stress;
        hero2Stress = LoadGameData.Instance.bonfireDatas[BonfireID].Hero2_Stress;
        hero3Stress = LoadGameData.Instance.bonfireDatas[BonfireID].Hero3_Stress;
        hero4Stress = LoadGameData.Instance.bonfireDatas[BonfireID].Hero4_Stress;
    }

    public override void ClickButton()
    {
        isActive = true;
        activeTime = time1;
        graceTime = time2;
    }
}
