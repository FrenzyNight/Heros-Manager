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
       
        if (!IsPrepared)
            SetUp();
    }

    public override void SetUp()
    {
        base.SetUp();
        
        fwTreasure1ResMeat = GameData("game3treasure1").GetAmount1;
        fwTreasure1ResHub = GameData("game3treasure1").GetAmount2;
        fwTreasure1CoolTime = GameData("game3treasure1").CoolTime;
        fwTreasure1GetTime = GameData("game3treasure1").value1;

        fwTreasure2ResMeat= GameData("game3treasure2").GetAmount1;
        fwTreasure2ResHub= GameData("game3treasure2").GetAmount2;
        fwTreasure2CoolTime= GameData("game3treasure2").CoolTime;
        fwTreasure2GetTime = GameData("game3treasure2").value1;

        fwTreasure3ResMeat= GameData("game3treasure3").GetAmount1;
        fwTreasure3ResHub= GameData("game3treasure3").GetAmount2;
        fwTreasure3CoolTime= GameData("game3treasure3").CoolTime;
        fwTreasure3GetTime = GameData("game3treasure3").value1;

        fwLaySpeed= GameData("game3Lay").Velocity;

        fwFairy1Speed= GameData("game3fairy1").Velocity;

        fwFairy2Speed= GameData("game3fairy2").Velocity;

        fwFairy3Speed= GameData("game3fairy3").Velocity;

        fwFairyLeftTime= GameData("game3fairy1").value1;
        fwFairyRightTime= GameData("game3fairy1").value2;
        fwFairyFrontTime= GameData("game3fairy1").value3;

        //real

        fwLayRealSpeed = fwStandardSpeed * fwLaySpeed;
        fwFairy1RealSpeed = fwStandardSpeed * fwFairy1Speed;
        fwFairy2RealSpeed = fwStandardSpeed * fwFairy2Speed;;
        fwFairy3RealSpeed = fwStandardSpeed * fwFairy3Speed;

        isTreasure1 = true;
        isTreasure2 = true;
        isTreasure3 = true;

        Spawn(Treasure1Prefab, ScreenPoint(tp1), Quaternion.identity, SpawnContainer("FairyWarehouse"));
        Spawn(Treasure2Prefab, ScreenPoint(tp2), Quaternion.identity, SpawnContainer("FairyWarehouse"));
        Spawn(Treasure3Prefab, ScreenPoint(tp3), Quaternion.identity, SpawnContainer("FairyWarehouse"));
            
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
            yield return new WaitForSeconds(MiniGameRules.PositiveDelay(fwTreasure1CoolTime));
            Spawn(Treasure1Prefab, ScreenPoint(tp1), Quaternion.identity, SpawnContainer("FairyWarehouse"));
        }
        else if(boxNum == 2)
        {
            isTreasure2 = true;
            yield return new WaitForSeconds(MiniGameRules.PositiveDelay(fwTreasure2CoolTime));
            Spawn(Treasure2Prefab, ScreenPoint(tp2), Quaternion.identity, SpawnContainer("FairyWarehouse"));
        }
        else if(boxNum == 3)
        {
            isTreasure3 = true;
            yield return new WaitForSeconds(MiniGameRules.PositiveDelay(fwTreasure3CoolTime));
            Spawn(Treasure3Prefab, ScreenPoint(tp3), Quaternion.identity, SpawnContainer("FairyWarehouse"));
        }
    }

    public override void StartGame()
    {
    }

}
