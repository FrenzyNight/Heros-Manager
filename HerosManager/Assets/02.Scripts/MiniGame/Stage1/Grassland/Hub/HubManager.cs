using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HubManager : MonoBehaviour
{
    public MiniGameMgr MGM;
    public GameObject GuidText, GuidPanel;
    private Vector2 TextTarget;

    private bool isFirst = true;

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
        MGM = GameObject.Find("MiniGameMgr").GetComponent<MiniGameMgr>();
        InGameMgr.Instance.EnterMiniGame("Stage_1_Grassland");
        SetUp();
        StartCoroutine(FirstStart());
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

        TextTarget = new Vector2(GuidText.GetComponent<RectTransform>().anchoredPosition.x, GuidText.GetComponent<RectTransform>().anchoredPosition.y + MGM.textPosition);
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
                Instantiate(GoldHub, new Vector2(x,y), Quaternion.identity,GameObject.Find("Hub").transform);
            }
            else
            {
                Instantiate(Hub, new Vector2(x,y), Quaternion.identity,GameObject.Find("Hub").transform);
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
