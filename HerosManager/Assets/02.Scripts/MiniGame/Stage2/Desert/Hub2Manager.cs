using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hub2Manager : MiniGameSetMgr
{
    public GameObject Hub,GoldHub;
    public GameObject Cactus;
    private List<GameObject> CacList = new List<GameObject>();
    public Vector2 StartP, EndP;
    private Vector2 StartPoint, EndPoint;

    public float quater, plusmin;
    private float x, y;

    // load
    private float standardHubSpeed = 500f;

    public float hubCactusStun;
    public float hubCactusInv;
    public float hubCactusNum;

    public float hubCoolTime;
    public float hubSpanTime;
    public float hubGetTime;
    

    public float hubGoldPer;
    public int hubGoldRes;
    public int hubNormalRes;

    public float hubCharSpeed;

    //real
    public float realHubCharSpeed;

    void Start()
    {
        if (!IsPrepared)
            SetUp();
    }

    public override void SetUp()
    {
        base.SetUp();
        CacList.Clear();

        hubCharSpeed = GameData("game2hub_normal").Velocity;
        hubCoolTime = GameData("game2hub_normal").CoolTime;
        hubGetTime = GameData("game2hub_normal").value2;
        
        hubGoldPer = GameData("game2hub_gold").Probability * 100;
        hubGoldRes = (int)GameData("game2hub_gold").GetAmount1;
        
        hubNormalRes = (int)GameData("game2hub_normal").GetAmount1;
        hubSpanTime = GameData("game2hub_normal").value1;

        hubCactusInv = GameData("game2hubcactus").Invincibility;
        hubCactusNum = GameData("game2hubcactus").value1;
        hubCactusStun = GameData("game2hubcactus").Stun;
        
        realHubCharSpeed = standardHubSpeed * hubCharSpeed;

        item1ID = GameData("game2hub_normal").GetItemID1;
        
        StartPoint = ScreenPoint(StartP);
        EndPoint = ScreenPoint(EndP);
    
        StartGame();
    }

    private void SpawnCactus()
    {
        
        for(int j=0;j<(int)hubCactusNum/4;j++) // ++
        {
            x = Random.Range(transform.position.x + quater-plusmin, transform.position.x + quater+plusmin);
            y = Random.Range(transform.position.y + quater-plusmin, transform.position.y + quater+plusmin);

            CacList.Add(Spawn(Cactus, new Vector2(x,y), Quaternion.identity));
        }
        for(int j=0;j<(int)hubCactusNum/4;j++) // +-
        {
            x = Random.Range(transform.position.x + quater-plusmin, transform.position.x + quater+plusmin);
            y = Random.Range(transform.position.y -quater-plusmin, transform.position.y-quater+plusmin);

            CacList.Add(Spawn(Cactus, new Vector2(x,y), Quaternion.identity));
        }
        for(int j=0;j<(int)hubCactusNum/4;j++) // --
        {
            x = Random.Range(transform.position.x-quater-plusmin, transform.position.x-quater+plusmin);
            y = Random.Range(transform.position.y-quater-plusmin, transform.position.y-quater+plusmin);

            CacList.Add(Spawn(Cactus, new Vector2(x,y), Quaternion.identity));
        }
        for(int j=0;j<(int)hubCactusNum/4;j++) // -+
        {
            x = Random.Range(transform.position.x-quater-plusmin, transform.position.x-quater+plusmin);
            y = Random.Range(transform.position.y+ quater-plusmin, transform.position.y+ quater+plusmin);

            CacList.Add(Spawn(Cactus, new Vector2(x,y), Quaternion.identity));
        }
        
    }

    void RemoveCactus()
    {
        foreach(GameObject cac in CacList)
        {
            Destroy(cac);
        }
        CacList.Clear();
    }

    public override void StartGame()
    {
        base.StartGame();
        StartCoroutine(SpawnHub());
        SpawnCactus();
    }


    IEnumerator SpawnHub()
    {
        while(true)
        {
            x = Random.Range(StartPoint.x, EndPoint.x);
            y = Random.Range(StartPoint.y, EndPoint.y);

            rnd = Random.Range(1, 101);
            if(MiniGameRules.PercentHit(rnd, hubGoldPer))
            {
                Spawn(GoldHub, new Vector2(x,y), Quaternion.identity,SpawnContainer("Hub2"));
            }
            else
            {
                Spawn(Hub, new Vector2(x,y), Quaternion.identity,SpawnContainer("Hub2"));
            }
            
            yield return new WaitForSeconds(MiniGameRules.PositiveDelay(hubCoolTime));
        }
    }

    public void GetHub()
    {
        base.AddItem(item1ID, hubNormalRes);
    }

    public void GetGoldHub()
    {
        base.AddItem(item1ID, hubGoldRes);
    }

    public override void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(StartPoint, 4f);
        Gizmos.DrawSphere(EndPoint, 4f);
    }
}
