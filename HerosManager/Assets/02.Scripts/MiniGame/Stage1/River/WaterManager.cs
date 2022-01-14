using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterManager : MonoBehaviour
{
    public MiniGameMgr MGM;
    public GameObject GuidText, GuidPanel;
    private Vector2 TextTarget;

    private bool isFirst = true;

    public Vector2 SpawnPoint1, SpawnPoint2, SpawnPoint3;
    public Vector2 spawnP1, spawnP2, spawnP3;
    public GameObject Boots, Deadfish, Apple;
    private GameObject SelectedTrash;

    private int max;
    private int rnd;

    //read

    public float standardWaterSpeed = 300f;

    public float waterRes;
    public float waterGetTime;

    public float waterRockSpeed;
    public float waterRockCoolTime;
    public float waterRockMaxNum;

    public float waterRockInvTime;

    //real
    public float realRockSpeed;
    
    //resolution

    public float widthScale, heightScale;

    void Start()
    {
        MGM = GameObject.Find("MiniGameMgr").GetComponent<MiniGameMgr>();
        InGameMgr.Instance.EnterMiniGame("Stage_1_river");

        SetUp();
        StartCoroutine(FirstStart());
    }

    void SetUp()
    {
        widthScale = Screen.width / 1920f;
        heightScale = Screen.height / 1080f;

        waterGetTime = InGameMgr.Instance.miniGameData["game1water"].value1;
        waterRes = InGameMgr.Instance.miniGameData["game1water"].water;
        
        waterRockCoolTime = InGameMgr.Instance.miniGameData["game1rock"].cooltime;
        waterRockInvTime = InGameMgr.Instance.miniGameData["game1rock"].invincibility;
        waterRockMaxNum = InGameMgr.Instance.miniGameData["game1rock"].value2;
        waterRockSpeed = InGameMgr.Instance.miniGameData["game1rock"].speed;

        realRockSpeed = standardWaterSpeed * waterRockSpeed;

        SpawnPoint1 = new Vector2(spawnP1.x * widthScale + transform.position.x, spawnP1.y * heightScale + transform.position.y);
        SpawnPoint2 = new Vector2(spawnP2.x * widthScale + transform.position.x, spawnP2.y * heightScale + transform.position.y);
        SpawnPoint3 = new Vector2(spawnP3.x * widthScale + transform.position.x, spawnP3.y * heightScale + transform.position.y);
   
        TextTarget = new Vector2(GuidText.GetComponent<RectTransform>().anchoredPosition.x, GuidText.GetComponent<RectTransform>().anchoredPosition.y + MGM.textPosition);
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

        
        StartCoroutine(SpawnRock());
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

        StartCoroutine(SpawnRock());
    }
    IEnumerator SpawnRock()
    {
       while(true)
        {
            rnd = Random.Range(0, (int)waterRockMaxNum);
            if(rnd == 0) // 장애물 1개
            {
                rnd = Random.Range(1,4);
                if(rnd == 1) //1번 포인트 소환
                {
                    RandomObject();
                    Instantiate(SelectedTrash, SpawnPoint1, Quaternion.identity, GameObject.Find("Water").transform);
                }
                else if(rnd == 2)
                {
                    RandomObject();
                    Instantiate(SelectedTrash, SpawnPoint2, Quaternion.identity, GameObject.Find("Water").transform);
                }
                else if(rnd == 3)
                {
                    RandomObject();
                    Instantiate(SelectedTrash, SpawnPoint3, Quaternion.identity, GameObject.Find("Water").transform);
                }
            }
            else // 장애물 2개
            {
                rnd = Random.Range(1,4);
                if(rnd == 1) //1번 포인트 제외 소환
                {
                    RandomObject();
                    Instantiate(SelectedTrash, SpawnPoint2, Quaternion.identity, GameObject.Find("Water").transform);
                    RandomObject();
                    Instantiate(SelectedTrash, SpawnPoint3, Quaternion.identity, GameObject.Find("Water").transform);
                }
                else if(rnd == 2)
                {
                    RandomObject();
                    Instantiate(SelectedTrash, SpawnPoint1, Quaternion.identity, GameObject.Find("Water").transform);
                    RandomObject();
                    Instantiate(SelectedTrash, SpawnPoint3, Quaternion.identity, GameObject.Find("Water").transform);
                }
                else if(rnd == 3)
                {
                    RandomObject();
                    Instantiate(SelectedTrash, SpawnPoint1, Quaternion.identity, GameObject.Find("Water").transform);
                    RandomObject();
                    Instantiate(SelectedTrash, SpawnPoint2, Quaternion.identity, GameObject.Find("Water").transform);
                }
            }

            yield return new WaitForSeconds(waterRockCoolTime);
        }
    }

    void RandomObject()
    {
        int rand;
        rand = Random.Range(1,4);
        if(rand == 1)
        {
            SelectedTrash = Boots;
        }
        else if(rand == 2)
        {
            SelectedTrash = Deadfish;
        }
        else if(rand == 3)
        {
            SelectedTrash = Apple;
        }
    }
    
    public void GetWater()
    {
        MGM.water += (int)waterRes;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(SpawnPoint1, 3f);
        Gizmos.DrawSphere(SpawnPoint2, 3f);
        Gizmos.DrawSphere(SpawnPoint3, 3f);
    }
}
