using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LakeWaterManager : MiniGameSetMgr
{
    //road
    public float standardLakeWaterSpeed = 100f;
    
    public float waterBucketSpeed;
    public float waterRock1Speed;
    public float waterRock2Speed;
    public float waterLevel1Res;
    public float waterLevel2Res;

    //real
    public float realBucketSpeed;
    public float realRock1Speed;
    public float realRock2Speed;

    //point
    public float rockMovePoint;

    // Start is called before the first frame update
    void Start()
    {
        if (!IsPrepared)
            SetUp();
    }

    public override void SetUp()
    {
        base.SetUp();

        waterBucketSpeed = GameData("game2bucket").Velocity;

        waterRock1Speed = GameData("game2rock1").Velocity;
        waterRock2Speed = GameData("game2rock2").Velocity;

        waterLevel1Res = GameData("game2level1").GetAmount1;
        waterLevel2Res = GameData("game2level2").GetAmount1;

        realBucketSpeed = standardLakeWaterSpeed * waterBucketSpeed;
        realRock1Speed = standardLakeWaterSpeed * waterRock1Speed;
        realRock2Speed = standardLakeWaterSpeed * waterRock2Speed;

    }

    public void GetWater(int lv)
    {
        /*
        if(lv == 1)
            MGM.water += (int)waterLevel1Res;
        else if(lv == 2)
            MGM.water += (int)waterLevel2Res;
            */
    }

    public override void StartGame()
    {
    }

}
