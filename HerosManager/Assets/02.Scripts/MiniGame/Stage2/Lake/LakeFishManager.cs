using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LakeFishManager : MiniGameSetMgr
{
    public GameObject FishPrefab, GoldFishPrefab;

    private float standardLakeFishSpeed = 50f;

    //road
    public float fishStunTime;
    public float fishCoolTime;

    public float fishNormalRes;
    public float fishNormalSpeed;

    public float fishGoldRes;
    public float fishGoldPer;
    public float fishGoldSpeed;

    public float fishLineSpeed;
    public float fishLineCatchSpeedVar;

    //fish
    public Vector2 StartP, EndP;
    private Vector2 StartPoint, EndPoint;
    private Vector2 spawnPoint;

    //real
    public float realFishSpeed;
    public float realGoldFishSpeed;
    public float realFishLineSpeed;

    // Start is called before the first frame update
    void Start()
    {
        if (!IsPrepared)
            SetUp();
    }



    public override void SetUp()
    {
        base.SetUp();


        fishStunTime = GameData("game2fish_norm").Stun;
        fishCoolTime = GameData("game2fish_norm").CoolTime;

        fishNormalRes = GameData("game2fish_norm").GetAmount1;
        fishNormalSpeed = GameData("game2fish_norm").Velocity;

        fishGoldRes = GameData("game2fish_gold").GetAmount1;
        fishGoldPer = GameData("game2fish_gold").Probability * 100;
        fishGoldSpeed = GameData("game2fish_gold").Velocity;

        fishLineSpeed = GameData("game2fishingline").Velocity;
        fishLineCatchSpeedVar = GameData("game2fishingline").value1;

        realFishSpeed = standardLakeFishSpeed * fishNormalSpeed;
        realGoldFishSpeed = standardLakeFishSpeed * fishGoldSpeed;
        realFishLineSpeed = standardLakeFishSpeed * fishLineSpeed;

        StartPoint = ScreenPoint(StartP);
        EndPoint = ScreenPoint(EndP);
        StartGame();
    }

    public void GetFish()
    {
        AddItem(GameData("game2fish_norm").GetItemID1, (int)fishNormalRes);
    }

    public void GetGoldFish()
    {
        AddItem(GameData("game2fish_gold").GetItemID1, (int)fishGoldRes);
    }

    IEnumerator SpawnFish()
    {
        GameObject Fish, obj;
        while(true)
        {
            rnd = Random.Range(1,101);
            if(MiniGameRules.PercentHit(rnd, fishGoldPer))
            {
                Fish = GoldFishPrefab;
            }
            else
            {
                Fish = FishPrefab;
            }

            rnd = Random.Range(0,2);

            if(rnd == 0) // 왼쪽
            {
                spawnPoint = StartPoint;
            }
            else if(rnd == 1)
            {
                spawnPoint = EndPoint;
            }

            obj = Spawn(Fish, new Vector2(spawnPoint.x, spawnPoint.y), Quaternion.identity,SpawnContainer("LakeFish"));

            if(rnd == 0)
                obj.GetComponent<LakeFishMove>().SetDir(1);
                
            else if(rnd == 1)
                obj.GetComponent<LakeFishMove>().SetDir(-1);

            yield return new WaitForSeconds(MiniGameRules.PositiveDelay(fishCoolTime));
        }

    }

    public override void StartGame()
    {
        base.StartGame();
        StartCoroutine(SpawnFish());
    }

    

    public override void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(StartPoint, 4f);
        Gizmos.DrawSphere(EndPoint, 4f);
    }
}
