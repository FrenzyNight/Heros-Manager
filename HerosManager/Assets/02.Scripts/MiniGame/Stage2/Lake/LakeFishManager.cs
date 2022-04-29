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
        SetUp();
    }



    public override void SetUp()
    {
        base.SetUp();

        FishPrefab.GetComponent<MiniGameObjectMgr>().manager = gameObject;
        GoldFishPrefab.GetComponent<MiniGameObjectMgr>().manager = gameObject;

        fishStunTime = LoadGameData.Instance.miniGameDatas["game2fish_norm"].Stun;
        fishCoolTime = LoadGameData.Instance.miniGameDatas["game2fish_norm"].CoolTime;

        fishNormalRes = LoadGameData.Instance.miniGameDatas["game2fish_norm"].GetAmount1;
        fishNormalSpeed = LoadGameData.Instance.miniGameDatas["game2fish_norm"].Velocity;

        fishGoldRes = LoadGameData.Instance.miniGameDatas["game2fish_gold"].GetAmount1;
        fishGoldPer = LoadGameData.Instance.miniGameDatas["game2fish_gold"].Probability * 100;
        fishGoldSpeed = LoadGameData.Instance.miniGameDatas["game2fish_gold"].Velocity;

        fishLineSpeed = LoadGameData.Instance.miniGameDatas["game2fishingline"].Velocity;
        fishLineCatchSpeedVar = LoadGameData.Instance.miniGameDatas["game2fishingline"].value1;

        realFishSpeed = standardLakeFishSpeed * fishNormalSpeed;
        realGoldFishSpeed = standardLakeFishSpeed * fishGoldSpeed;
        realFishLineSpeed = standardLakeFishSpeed * fishLineSpeed;

        StartPoint = new Vector2(StartP.x * widthScale + transform.position.x , StartP.y * heightScale + transform.position.y);
        EndPoint = new Vector2(EndP.x * widthScale + transform.position.x , EndP.y * heightScale + transform.position.y);
    }

    public void GetFish()
    {
       // MGM.meat += (int)fishNormalRes;
    }

    public void GetGoldFish()
    {
       // MGM.meat += (int)fishGoldRes;
    }

    IEnumerator SpawnFish()
    {
        GameObject Fish, obj;
        while(true)
        {
            rnd = Random.Range(1,101);
            if(rnd <= fishGoldPer)
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

            obj = Instantiate(Fish, new Vector2(spawnPoint.x, spawnPoint.y), Quaternion.identity,GameObject.Find("LakeFish").transform);

            if(rnd == 0)
                obj.GetComponent<LakeFishMove>().SetDir(1);
                
            else if(rnd == 1)
                obj.GetComponent<LakeFishMove>().SetDir(-1);

            yield return new WaitForSeconds(fishCoolTime);
        }

    }

    public void StartGame()
    {
    }

    

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(StartPoint, 4f);
        Gizmos.DrawSphere(EndPoint, 4f);
    }
}
