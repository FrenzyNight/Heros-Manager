using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HubManager : MiniGameSetMgr
{

    public GameObject Hub,GoldHub;
    public Vector2 StartP, EndP;
    private Vector2 StartPoint, EndPoint;
    
    private float x, y;

    // load
    private float standardHubSpeed = 500f;
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
        //SetUp();
    }

    public override void SetUp()
    {
        base.SetUp();
        
        Hub.GetComponent<MiniGameObjectMgr>().manager = gameObject;
        GoldHub.GetComponent<MiniGameObjectMgr>().manager = gameObject;
        
        hubCharSpeed = LoadGameData.Instance.miniGameDatas["game1hub_normal"].value1;
        hubCoolTime = LoadGameData.Instance.miniGameDatas["game1hub_normal"].CoolTime;
        hubGetTime = LoadGameData.Instance.miniGameDatas["game1hub_normal"].value3;
        
        hubGoldPer = LoadGameData.Instance.miniGameDatas["game1hub_gold"].Probability * 100;
        hubGoldRes = (int)LoadGameData.Instance.miniGameDatas["game1hub_gold"].GetAmount1;
        
        hubNormalRes = (int)LoadGameData.Instance.miniGameDatas["game1hub_normal"].GetAmount1;
        hubSpanTime = LoadGameData.Instance.miniGameDatas["game1hub_normal"].value2;

        item1ID = LoadGameData.Instance.miniGameDatas["game1hub_normal"].GetItemID1;
        
        realHubCharSpeed = standardHubSpeed * hubCharSpeed;

        StartPoint = new Vector2(StartP.x * widthScale + transform.position.x , StartP.y * heightScale + transform.position.y);
        EndPoint = new Vector2(EndP.x * widthScale + transform.position.x , EndP.y * heightScale + transform.position.y);

        StartGame();
    }

    public override void StartGame()
    {
        base.StartGame();

        foreach(GameObject obj in spawnObjects)
        {
            Destroy(obj);
        }

        spawnObjects.Clear();

        StartCoroutine(SpawnHub());
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
                spawnObjects.Add(Instantiate(GoldHub, new Vector2(x,y), Quaternion.identity,mother.transform));
            }
            else
            {
                spawnObjects.Add(Instantiate(Hub, new Vector2(x,y), Quaternion.identity,mother.transform));
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
