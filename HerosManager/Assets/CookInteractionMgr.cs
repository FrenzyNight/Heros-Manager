using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CookInteractionMgr : CampInteractionMgr
{
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
    // Start is called before the first frame update
    void Start()
    {
        Setup();   
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void Setup()
    {
        isActive = false;
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

        getItemID = LoadGameData.Instance.cookDatas[cookID].GetItemID;
        getItemAmount = LoadGameData.Instance.cookDatas[cookID].GetItemAmount;

        cookTime = LoadGameData.Instance.cookDatas[cookID].CookTime;
        getTime = LoadGameData.Instance.cookDatas[cookID].GetTime;
        
        
    }
}
