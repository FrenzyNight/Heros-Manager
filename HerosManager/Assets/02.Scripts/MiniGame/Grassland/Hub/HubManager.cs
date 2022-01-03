using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HubManager : MonoBehaviour
{
    public GameObject Hub,GoldHub;
    public Vector2 StartP, EndP;
    private Vector2 StartPoint, EndPoint;
    
    private float x, y;
    private int rnd;

    // load
    private float standardHubSpeed = 500f;
    public float hubCoolTime;
    public float hubSpanTime;
    public float hubGetTime;
    

    public float hubGoldPer;
    public float hubGoldRes;
    public float hubNormalRes;

    public float hubCharSpeed;

    //real
    public float realHubCharSpeed;

    public float resolutionScale;

    void Start()
    {
        InGameMgr.Instance.EnterMiniGame("Stage_1_Grassland");
        SetUp();
        StartCoroutine(SpawnHub());
    }

    void SetUp()
    {
        hubCharSpeed = InGameMgr.Instance.miniGameData["game1hub_normal"].value1;
        hubCoolTime = InGameMgr.Instance.miniGameData["game1hub_normal"].cooltime;
        hubGetTime = InGameMgr.Instance.miniGameData["game1hub_normal"].value3;
        
        hubGoldPer = InGameMgr.Instance.miniGameData["game1hub_gold"].probability * 100;
        hubGoldRes = InGameMgr.Instance.miniGameData["game1hub_gold"].hub;
        
        hubNormalRes = InGameMgr.Instance.miniGameData["game1hub_normal"].hub;
        hubSpanTime = InGameMgr.Instance.miniGameData["game1hub_normal"].value2;

        realHubCharSpeed = standardHubSpeed * hubCharSpeed;

        resolutionScale = Screen.width / 1920f;
        StartPoint = new Vector2(StartP.x * resolutionScale + transform.position.x , StartP.y * resolutionScale + transform.position.y);
        EndPoint = new Vector2(EndP.x * resolutionScale + transform.position.x , EndP.y * resolutionScale + transform.position.y);

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
                Instantiate(GoldHub, new Vector2(x,y), Quaternion.identity,GameObject.Find("Hub").transform);
            }
            else
            {
                Instantiate(Hub, new Vector2(x,y), Quaternion.identity,GameObject.Find("Hub").transform);
            }
            
            yield return new WaitForSeconds(hubCoolTime);
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(StartPoint, 4f);
        Gizmos.DrawSphere(EndPoint, 4f);
    }
}
