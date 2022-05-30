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
    public int buttonState; // 0 = hub, 1 = water, 2 = getgub, 3 = trash
    
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

    }

    void Water()
    {

    }

    void SetTrash()
    {

    }

    void ClearTrash()
    {

    }

    public override void Setup()
    {
        isActive = false;

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
    }
}
