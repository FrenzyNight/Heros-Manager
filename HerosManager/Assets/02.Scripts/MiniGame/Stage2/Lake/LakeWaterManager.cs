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
        if (lv != 1 && lv != 2) return;
        var reward = GameData(lv == 1 ? "game2level1" : "game2level2");
        AddItem(reward.GetItemID1, (int)reward.GetAmount1);
        GamepadInput.Pulse(0.12f, 0.2f, 0.08f);
    }

    public override void StartGame()
    {
    }

}
