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
        
        
        hubCharSpeed = GameData("game1hub_normal").value1;
        hubCoolTime = GameData("game1hub_normal").CoolTime;
        hubGetTime = GameData("game1hub_normal").value3;
        
        hubGoldPer = GameData("game1hub_gold").Probability * 100;
        hubGoldRes = (int)GameData("game1hub_gold").GetAmount1;
        
        hubNormalRes = (int)GameData("game1hub_normal").GetAmount1;
        hubSpanTime = GameData("game1hub_normal").value2;

        item1ID = GameData("game1hub_normal").GetItemID1;
        
        realHubCharSpeed = standardHubSpeed * hubCharSpeed;

        StartPoint = ScreenPoint(StartP);
        EndPoint = ScreenPoint(EndP);

        StartGame();
    }

    public override void StartGame()
    {
        base.StartGame();


        StartCoroutine(SpawnHub());
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
                Spawn(GoldHub, new Vector2(x,y), Quaternion.identity);
            }
            else
            {
                Spawn(Hub, new Vector2(x,y), Quaternion.identity);
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
