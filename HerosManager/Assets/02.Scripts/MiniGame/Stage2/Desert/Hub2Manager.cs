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
        SetUp();
    }

    public override void SetUp()
    {
        base.SetUp();

        hubCharSpeed = LoadGameData.Instance.miniGameDatas["game2hub_normal"].Velocity;
        hubCoolTime = LoadGameData.Instance.miniGameDatas["game2hub_normal"].CoolTime;
        hubGetTime = LoadGameData.Instance.miniGameDatas["game2hub_normal"].value2;
        
        hubGoldPer = LoadGameData.Instance.miniGameDatas["game2hub_gold"].Probability * 100;
        hubGoldRes = (int)LoadGameData.Instance.miniGameDatas["game2hub_gold"].GetAmount1;
        
        hubNormalRes = (int)LoadGameData.Instance.miniGameDatas["game2hub_normal"].GetAmount1;
        hubSpanTime = LoadGameData.Instance.miniGameDatas["game2hub_normal"].value1;

        hubCactusInv = LoadGameData.Instance.miniGameDatas["game2hubcactus"].Invincibility;
        hubCactusNum = LoadGameData.Instance.miniGameDatas["game2hubcactus"].value1;
        hubCactusStun = LoadGameData.Instance.miniGameDatas["game2hubcactus"].Stun;
        
        realHubCharSpeed = standardHubSpeed * hubCharSpeed;

        item1ID = LoadGameData.Instance.miniGameDatas["game2hub_normal"].GetItemID1;
        
        StartPoint = new Vector2(StartP.x * widthScale + transform.position.x , StartP.y * heightScale + transform.position.y);
        EndPoint = new Vector2(EndP.x * widthScale + transform.position.x , EndP.y * heightScale + transform.position.y);
    
        StartGame();
    }

    private void SpawnCactus()
    {
        
        for(int j=0;j<(int)hubCactusNum/4;j++) // ++
        {
            x = Random.Range(transform.position.x + quater-plusmin, transform.position.x + quater+plusmin);
            y = Random.Range(transform.position.y + quater-plusmin, transform.position.y + quater+plusmin);

            CacList.Add(Instantiate(Cactus, new Vector2(x,y), Quaternion.identity,mother.transform));
        }
        for(int j=0;j<(int)hubCactusNum/4;j++) // +-
        {
            x = Random.Range(transform.position.x + quater-plusmin, transform.position.x + quater+plusmin);
            y = Random.Range(transform.position.y -quater-plusmin, transform.position.y-quater+plusmin);

            CacList.Add(Instantiate(Cactus, new Vector2(x,y), Quaternion.identity,mother.transform));
        }
        for(int j=0;j<(int)hubCactusNum/4;j++) // --
        {
            x = Random.Range(transform.position.x-quater-plusmin, transform.position.x-quater+plusmin);
            y = Random.Range(transform.position.y-quater-plusmin, transform.position.y-quater+plusmin);

            CacList.Add(Instantiate(Cactus, new Vector2(x,y), Quaternion.identity,mother.transform));
        }
        for(int j=0;j<(int)hubCactusNum/4;j++) // -+
        {
            x = Random.Range(transform.position.x-quater-plusmin, transform.position.x-quater+plusmin);
            y = Random.Range(transform.position.y+ quater-plusmin, transform.position.y+ quater+plusmin);

            CacList.Add(Instantiate(Cactus, new Vector2(x,y), Quaternion.identity,mother.transform));
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
            if(rnd <= hubGoldPer)
            {
                Instantiate(GoldHub, new Vector2(x,y), Quaternion.identity,GameObject.Find("Hub2").transform);
            }
            else
            {
                Instantiate(Hub, new Vector2(x,y), Quaternion.identity,GameObject.Find("Hub2").transform);
            }
            
            yield return new WaitForSeconds(hubCoolTime);
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

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(StartPoint, 4f);
        Gizmos.DrawSphere(EndPoint, 4f);
    }
}
