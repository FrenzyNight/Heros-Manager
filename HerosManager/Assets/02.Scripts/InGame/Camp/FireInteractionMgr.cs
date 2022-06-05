using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FireInteractionMgr : CampInteractionMgr
{
    AudioSource audioSource;
    public int needAmount;
    public float time1, time2, time3;
    public float stressCnt;
    
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
            GraceGage.sprite = Red;
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

            audioSource.Stop();
            CampObjMgr.GetComponent<FireMgr>().NonFire();
        }

        if(!isActive)
        {
            AvailableButton();
        }

        if((Clock.Instance.nowTime >= Clock.Instance.dayTime*60/2) && !isActive )
        {
            stressCnt += Time.deltaTime;
        }

        if(stressCnt >= time3)
        {
            GetStress();
        }


    }

    public override void NextDayAction()
    {
        isGrace = false;
        isActive = false;
        activeTime = 0;
        graceTime = 0;
        CampObjMgr.GetComponent<FireMgr>().NonFire();
        AvailableButton();
        ActiveGage.fillAmount = 0;
        GraceGage.fillAmount = 0;
    }

    public override void Setup()
    {
        audioSource = GetComponent<AudioSource>();
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
        stressCnt = 0;

        hero1Stress = LoadGameData.Instance.bonfireDatas[BonfireID].Hero1_Stress;
        hero2Stress = LoadGameData.Instance.bonfireDatas[BonfireID].Hero2_Stress;
        hero3Stress = LoadGameData.Instance.bonfireDatas[BonfireID].Hero3_Stress;
        hero4Stress = LoadGameData.Instance.bonfireDatas[BonfireID].Hero4_Stress;
    }


    void Grace()
    {
        AvailableButton();
    }

    void GetStress()
    {
        Notice.Instance.InstNoticeText(LoadGameData.Instance.GetString("BonFire_a1"));
        HeroStateManager.Instance.heroStates[0].AddStat(hero1Stress,0,0,0);
        HeroStateManager.Instance.heroStates[1].AddStat(hero2Stress,0,0,0);
        HeroStateManager.Instance.heroStates[2].AddStat(hero3Stress,0,0,0);
        HeroStateManager.Instance.heroStates[3].AddStat(hero4Stress,0,0,0);
        stressCnt = 0;
    }

    public override void ClickButton()
    {
        if(ItemManager.Instance.GetItemInfo(NeedItemID).num < needAmount)
            return;

        ItemManager.Instance.AddItem(NeedItemID, -needAmount);
        stressCnt = 0;
        
        GraceGage.sprite = Green;
        isActive = true;
        isGrace = false;
        activeTime = time1;
        graceTime = time2;

        ActiveGage.fillAmount = activeTime / time1;
        GraceGage.fillAmount = graceTime / time2;

        audioSource.Play();
        CampObjMgr.GetComponent<FireMgr>().OnFire();
        UnavailableButton();
    }
}
