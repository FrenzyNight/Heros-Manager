using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LakeFishManager : MonoBehaviour
{
    public MiniGameMgr MGM;

    public GameObject FishPrefab, GoldFishPrefab;
    public GameObject GuidText, GuidPanel;
    private Vector2 TextTarget;

    public float widthScale;
    public float heightScale;
    private bool isFirst = true;

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

    int rnd;

    // Start is called before the first frame update
    void Start()
    {
        MGM = GameObject.Find("MiniGameMgr").GetComponent<MiniGameMgr>();
        InGameMgr.Instance.EnterMiniGame("Stage_2_desertLake");

        SetUp();
        StartCoroutine(FirstStart());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SetUp()
    {
        widthScale = Screen.width / 1920f;
        heightScale = Screen.height / 1080f;

        fishStunTime = InGameMgr.Instance.miniGameData["game2fish_norm"].stun;
        fishCoolTime = InGameMgr.Instance.miniGameData["game2fish_norm"].cooltime;

        fishNormalRes = InGameMgr.Instance.miniGameData["game2fish_norm"].meat;
        fishNormalSpeed = InGameMgr.Instance.miniGameData["game2fish_norm"].speed;

        fishGoldRes = InGameMgr.Instance.miniGameData["game2fish_gold"].meat;
        fishGoldPer = InGameMgr.Instance.miniGameData["game2fish_gold"].probability * 100;
        fishGoldSpeed = InGameMgr.Instance.miniGameData["game2fish_gold"].speed;

        fishLineSpeed = InGameMgr.Instance.miniGameData["game2fishingline"].speed;
        fishLineCatchSpeedVar = InGameMgr.Instance.miniGameData["game2fishingline"].value1;


        StartPoint = new Vector2(StartP.x * widthScale + transform.position.x , StartP.y * heightScale + transform.position.y);
        EndPoint = new Vector2(EndP.x * widthScale + transform.position.x , EndP.y * heightScale + transform.position.y);

        TextTarget = new Vector2(GuidText.GetComponent<RectTransform>().anchoredPosition.x, GuidText.GetComponent<RectTransform>().anchoredPosition.y + MGM.textPosition);
    }

    IEnumerator SpawnFish()
    {
        GameObject Fish;
        while(true)
        {
            rnd = Random.Range(0,2);

            if(rnd == 0) // 왼쪽
            {
                spawnPoint = StartPoint;
            }
            else if(rnd == 1)
            {
                spawnPoint = EndPoint;
            }

            rnd = Random.Range(1,101);
            if(rnd <= fishGoldPer)
            {
                Fish = GoldFishPrefab;
            }
            else
            {
                Fish = FishPrefab;
            }

            Instantiate(Fish, new Vector2(spawnPoint.x, spawnPoint.y), Quaternion.identity,GameObject.Find("LakeFish").transform);

            yield return new WaitForSeconds(fishCoolTime);
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

    }
    IEnumerator FirstStart()
    {
        GuidPanel.SetActive(true);
        GuidText.GetComponent<RectTransform>().anchoredPosition = new Vector2(0,0);
        
        yield return new WaitForSeconds(MGM.firstTime);
        GuidPanel.SetActive(false);
        isFirst = false;

        while (Vector2.Distance(GuidText.GetComponent<RectTransform>().anchoredPosition, TextTarget) >= 0.1f)
        {
            GuidText.GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(GuidText.GetComponent<RectTransform>().anchoredPosition, TextTarget, Time.deltaTime * MGM.textSpeed);

            yield return null;
        }

        StartCoroutine(SpawnFish());
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(StartPoint, 4f);
        Gizmos.DrawSphere(EndPoint, 4f);
    }
}
