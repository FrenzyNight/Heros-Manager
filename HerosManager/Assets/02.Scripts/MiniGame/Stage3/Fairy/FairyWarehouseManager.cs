using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FairyWarehouseManager : MiniGameSetMgr
{
    public GameObject Treasure1Prefab, Treasure2Prefab, Treasure3Prefab;
    public Vector2 tp1, tp2, tp3;

    //standard
    public float fwStandardSpeed;

    //road
    public float fwTreasure1ResMeat;
    public float fwTreasure1ResHub;
    public float fwTreasure1CoolTime;
    public float fwTreasure1GetTime;

    public float fwTreasure2ResMeat;
    public float fwTreasure2ResHub;
    public float fwTreasure2CoolTime;
    public float fwTreasure2GetTime;

    public float fwTreasure3ResMeat;
    public float fwTreasure3ResHub;
    public float fwTreasure3CoolTime;
    public float fwTreasure3GetTime;

    public float fwLaySpeed;

    public float fwFairy1Speed;

    public float fwFairy2Speed;

    public float fwFairy3Speed;

    public float fwFairyLeftTime;
    public float fwFairyRightTime;
    public float fwFairyFrontTime;

    //real

    public float fwLayRealSpeed;
    public float fwFairy1RealSpeed;
    public float fwFairy2RealSpeed;
    public float fwFairy3RealSpeed;

    //treasure check
    private bool isTreasure1, isTreasure2, isTreasure3;
    void Start()
    {
       
        SetUp();
    }

    void SetUp()
    {
        
        fwTreasure1ResMeat = LoadGameData.Instance.miniGameDatas["game3treasure1"].GetAmount1;
        fwTreasure1ResHub = LoadGameData.Instance.miniGameDatas["game3treasure1"].GetAmount2;
        fwTreasure1CoolTime = LoadGameData.Instance.miniGameDatas["game3treasure1"].CoolTime;
        fwTreasure1GetTime = LoadGameData.Instance.miniGameDatas["game3treasure1"].value1;

        fwTreasure2ResMeat= LoadGameData.Instance.miniGameDatas["game3treasure2"].GetAmount1;
        fwTreasure2ResHub= LoadGameData.Instance.miniGameDatas["game3treasure2"].GetAmount2;
        fwTreasure2CoolTime= LoadGameData.Instance.miniGameDatas["game3treasure2"].CoolTime;
        fwTreasure2GetTime = LoadGameData.Instance.miniGameDatas["game3treasure2"].value1;

        fwTreasure3ResMeat= LoadGameData.Instance.miniGameDatas["game3treasure3"].GetAmount1;
        fwTreasure3ResHub= LoadGameData.Instance.miniGameDatas["game3treasure3"].GetAmount2;
        fwTreasure3CoolTime= LoadGameData.Instance.miniGameDatas["game3treasure3"].CoolTime;
        fwTreasure3GetTime = LoadGameData.Instance.miniGameDatas["game3treasure3"].value1;

        fwLaySpeed= LoadGameData.Instance.miniGameDatas["game3Lay"].Velocity;

        fwFairy1Speed= LoadGameData.Instance.miniGameDatas["game3fairy1"].Velocity;

        fwFairy2Speed= LoadGameData.Instance.miniGameDatas["game3fairy2"].Velocity;

        fwFairy3Speed= LoadGameData.Instance.miniGameDatas["game3fairy3"].Velocity;

        fwFairyLeftTime= LoadGameData.Instance.miniGameDatas["game3fairy1"].value1;
        fwFairyRightTime= LoadGameData.Instance.miniGameDatas["game3fairy1"].value2;
        fwFairyFrontTime= LoadGameData.Instance.miniGameDatas["game3fairy1"].value3;

        //real

        fwLayRealSpeed = fwStandardSpeed * fwLaySpeed;
        fwFairy1RealSpeed = fwStandardSpeed * fwFairy1Speed;
        fwFairy2RealSpeed = fwStandardSpeed * fwFairy2Speed;;
        fwFairy3RealSpeed = fwStandardSpeed * fwFairy3Speed;

        isTreasure1 = true;
        isTreasure2 = true;
        isTreasure3 = true;

        Instantiate(Treasure1Prefab, new Vector2(tp1.x * widthScale, tp1.y * heightScale) , Quaternion.identity,GameObject.Find("FairyWarehouse").transform);
        Instantiate(Treasure2Prefab, new Vector2(tp2.x * widthScale, tp2.y * heightScale) , Quaternion.identity,GameObject.Find("FairyWarehouse").transform);
        Instantiate(Treasure3Prefab, new Vector2(tp3.x * widthScale, tp3.y * heightScale) , Quaternion.identity,GameObject.Find("FairyWarehouse").transform);
            
        }

    void Update()
    {
        if(!isTreasure1)
        {
            StartCoroutine(SpawnTreasure(1));
        }
        else if(!isTreasure2)
        {
            StartCoroutine(SpawnTreasure(2));
        }
        else if(!isTreasure3)
        {
            StartCoroutine(SpawnTreasure(3));
        }
    }

    public void GetTreasure(int boxNum)
    {
        /*
        if(boxNum == 1)
        {
            MGM.meat += (int)fwTreasure1ResMeat;
            MGM.hub += (int)fwTreasure1ResHub;
            isTreasure1 = false;
        }
        else if(boxNum == 2)
        {
            MGM.meat += (int)fwTreasure2ResMeat;
            MGM.hub += (int)fwTreasure2ResHub;
            isTreasure2 = false;
        }
        else if(boxNum == 3)
        {
            MGM.meat += (int)fwTreasure3ResMeat;
            MGM.hub += (int)fwTreasure3ResHub;
            isTreasure3 = false;
        }
        */
    }

    IEnumerator SpawnTreasure(int boxNum)
    {
        if(boxNum == 1)
        {
            isTreasure1 = true;
            yield return new WaitForSeconds(fwTreasure1CoolTime);
            Instantiate(Treasure1Prefab, new Vector2(tp1.x * widthScale, tp1.y * heightScale) , Quaternion.identity,GameObject.Find("FairyWarehouse").transform);
        }
        else if(boxNum == 2)
        {
            isTreasure2 = true;
            yield return new WaitForSeconds(fwTreasure2CoolTime);
            Instantiate(Treasure2Prefab, new Vector2(tp2.x * widthScale, tp2.y * heightScale) , Quaternion.identity,GameObject.Find("FairyWarehouse").transform);
        }
        else if(boxNum == 3)
        {
            isTreasure3 = true;
            yield return new WaitForSeconds(fwTreasure3CoolTime);
            Instantiate(Treasure3Prefab, new Vector2(tp3.x * widthScale, tp3.y * heightScale) , Quaternion.identity,GameObject.Find("FairyWarehouse").transform);
        }
    }

    public void StartGame()
    {
    }

}
