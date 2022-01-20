using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hub2Manager : MonoBehaviour
{
    public MiniGameMgr MGM;
    public GameObject GuidText, GuidPanel;
    private Vector2 TextTarget;

    private bool isFirst = true;

    public GameObject Hub,GoldHub;
    public GameObject Cactus;
    public Vector2 StartP, EndP;
    private Vector2 StartPoint, EndPoint;

    public float quater, plusmin;
    
    private float x, y;
    private int rnd;

    // load
    private float standardHubSpeed = 500f;

    public float hubCactusStun;
    public float hubCactusInv;
    public float hubCactusNum;
    public float hubCoolTime;
    public float hubSpanTime;
    public float hubGetTime;
    

    public float hubGoldPer;
    public float hubGoldRes;
    public float hubNormalRes;

    public float hubCharSpeed;

    //real
    public float realHubCharSpeed;

    
    public float widthScale, heightScale;

    void Start()
    {
        MGM = GameObject.Find("MiniGameMgr").GetComponent<MiniGameMgr>();
        InGameMgr.Instance.EnterMiniGame("Stage_2_desertField");
        SetUp();
        SpawnCactus();
        StartCoroutine(FirstStart());
    }

    void SetUp()
    {
        widthScale = Screen.width / 1920f;
        heightScale = Screen.height / 1080f;

        hubCharSpeed = InGameMgr.Instance.miniGameData["game2hub_normal"].speed;
        hubCoolTime = InGameMgr.Instance.miniGameData["game2hub_normal"].cooltime;
        hubGetTime = InGameMgr.Instance.miniGameData["game2hub_normal"].value2;
        
        hubGoldPer = InGameMgr.Instance.miniGameData["game2hub_gold"].probability * 100;
        hubGoldRes = InGameMgr.Instance.miniGameData["game2hub_gold"].hub;
        
        hubNormalRes = InGameMgr.Instance.miniGameData["game2hub_normal"].hub;
        hubSpanTime = InGameMgr.Instance.miniGameData["game2hub_normal"].value1;

        hubCactusInv = InGameMgr.Instance.miniGameData["game2hubcactus"].invincibility;
        hubCactusNum = InGameMgr.Instance.miniGameData["game2hubcactus"].value1;
        hubCactusStun = InGameMgr.Instance.miniGameData["game2hubcactus"].stun;
        
        realHubCharSpeed = standardHubSpeed * hubCharSpeed;

        
        StartPoint = new Vector2(StartP.x * widthScale + transform.position.x , StartP.y * heightScale + transform.position.y);
        EndPoint = new Vector2(EndP.x * widthScale + transform.position.x , EndP.y * heightScale + transform.position.y);

        TextTarget = new Vector2(GuidText.GetComponent<RectTransform>().anchoredPosition.x, GuidText.GetComponent<RectTransform>().anchoredPosition.y + MGM.textPosition);
    }

    private void SpawnCactus()
    {
        
        for(int j=0;j<(int)hubCactusNum/4;j++) // ++
        {
            x = Random.Range(transform.position.x + quater-plusmin, transform.position.x + quater+plusmin);
            y = Random.Range(transform.position.y + quater-plusmin, transform.position.y + quater+plusmin);

            Instantiate(Cactus, new Vector2(x,y), Quaternion.identity,GameObject.Find("Hub2").transform);
        }
        for(int j=0;j<(int)hubCactusNum/4;j++) // +-
        {
            x = Random.Range(transform.position.x + quater-plusmin, transform.position.x + quater+plusmin);
            y = Random.Range(transform.position.y -quater-plusmin, transform.position.y-quater+plusmin);

            Instantiate(Cactus, new Vector2(x,y), Quaternion.identity,GameObject.Find("Hub2").transform);
        }
        for(int j=0;j<(int)hubCactusNum/4;j++) // --
        {
            x = Random.Range(transform.position.x-quater-plusmin, transform.position.x-quater+plusmin);
            y = Random.Range(transform.position.y-quater-plusmin, transform.position.y-quater+plusmin);

            Instantiate(Cactus, new Vector2(x,y), Quaternion.identity,GameObject.Find("Hub2").transform);
        }
        for(int j=0;j<(int)hubCactusNum/4;j++) // -+
        {
            x = Random.Range(transform.position.x-quater-plusmin, transform.position.x-quater+plusmin);
            y = Random.Range(transform.position.y+ quater-plusmin, transform.position.y+ quater+plusmin);

            Instantiate(Cactus, new Vector2(x,y), Quaternion.identity,GameObject.Find("Hub2").transform);
        }
        
    }

    public void StartGame()
    {
        if(!isFirst)
            StartCoroutine(ReStart());
    }

    IEnumerator ReStart()
    {
        GuidPanel.SetActive(true);
        GuidText.GetComponent<RectTransform>().anchoredPosition = new Vector2(0,0);
        yield return new WaitForSeconds(MGM.firstTime);
        GuidPanel.SetActive(false);

        while (Vector2.Distance(GuidText.GetComponent<RectTransform>().anchoredPosition, TextTarget) >= 0.1f)
        {
            GuidText.GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(GuidText.GetComponent<RectTransform>().anchoredPosition, TextTarget, Time.deltaTime * MGM.textSpeed);

            yield return null;
        }

        StartCoroutine(SpawnHub());
    }

    IEnumerator FirstStart()
    {
        yield return new WaitForSeconds(MGM.firstTime);
        GuidPanel.SetActive(false);
        isFirst = false;

        while (Vector2.Distance(GuidText.GetComponent<RectTransform>().anchoredPosition, TextTarget) >= 0.1f)
        {
            GuidText.GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(GuidText.GetComponent<RectTransform>().anchoredPosition, TextTarget, Time.deltaTime * MGM.textSpeed);

            yield return null;
        }

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
        MGM.hub += (int)hubNormalRes;
    }

    public void GetGoldHub()
    {
        MGM.hub += (int)hubGoldRes;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(StartPoint, 4f);
        Gizmos.DrawSphere(EndPoint, 4f);
    }
}
