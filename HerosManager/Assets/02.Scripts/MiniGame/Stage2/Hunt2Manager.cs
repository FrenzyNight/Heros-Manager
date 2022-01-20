using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class Hunt2Manager : MonoBehaviour
{
    public MiniGameMgr MGM;
    public GameObject GuidText, GuidPanel;
    private Vector2 TextTarget;
    private bool isFisrt = true;

    public GameObject FoxPrefab, GoldFoxPrefab, WeaselPrefab;
    public Vector2 StartP, EndP;
    public Vector2 StartP2, EndP2;
    private Vector2 StartPoint, EndPoint;
    private Vector2 StartPoint2, EndPoint2;
    private float x, y;
    public int foxNum;
    public int weaselNum;
    private int rnd;

    //read
    public float standardHuntSpeed = 1000f;
    public float huntArrowSpeed;
    public float huntArrowCoolTime;

    public float huntMonsterCoolTime;
    public float huntMonsterMaxNum;
    public float huntMonsterMinMoveTime;
    public float huntMonsterMaxMoveTime;
    public float huntMonsterIdleTime;

    public float huntFoxSpeed;
    public float huntFoxRes;
    public float huntFoxPer;
    public float huntFoxHP;

    public float huntWeaselSpeed;
    public float huntWeaselRes;
    public float huntWeaselPer;
    public float huntWeaselHP;
    public float huntWeaselAngrySpeed;

    //real
    public float realArrowSpeed;
    public float realFoxSpeed;
    public float realWeaselSpeed;

    public bool isShoot;

    public float resolutionScale;
    public float heightScale;

    // Start is called before the first frame update
    void Start()
    {
        MGM = GameObject.Find("MiniGameMgr").GetComponent<MiniGameMgr>();
        InGameMgr.Instance.EnterMiniGame("Stage_1_Grassland");
        SetUp();
        StartCoroutine(FirstStart());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SetUp()
    {
        foxNum = 0;
        weaselNum = 0;
        isShoot = false;
        huntArrowSpeed = InGameMgr.Instance.miniGameData["game1arrow"].speed;
        huntArrowCoolTime = InGameMgr.Instance.miniGameData["game1arrow"].cooltime;

        huntMonsterCoolTime = InGameMgr.Instance.miniGameData["game1monster_fox"].cooltime;
        huntMonsterMaxNum = InGameMgr.Instance.miniGameData["game1monster_fox"].value6;
        huntMonsterMinMoveTime = InGameMgr.Instance.miniGameData["game1monster_fox"].value4;
        huntMonsterMaxMoveTime = InGameMgr.Instance.miniGameData["game1monster_fox"].value5;
        huntMonsterIdleTime= InGameMgr.Instance.miniGameData["game1monster_fox"].value3;

        huntFoxSpeed = InGameMgr.Instance.miniGameData["game1monster_fox"].speed;
        huntFoxRes = InGameMgr.Instance.miniGameData["game1monster_fox"].meat;
        huntFoxPer = InGameMgr.Instance.miniGameData["game1monster_fox"].probability * 100;
        huntFoxHP = InGameMgr.Instance.miniGameData["game1monster_fox"].value1;

        huntWeaselSpeed = InGameMgr.Instance.miniGameData["game1monster_bear"].speed;
        huntWeaselRes = InGameMgr.Instance.miniGameData["game1monster_bear"].meat;
        huntWeaselPer = InGameMgr.Instance.miniGameData["game1monster_bear"].probability * 100;
        huntWeaselHP = InGameMgr.Instance.miniGameData["game1monster_bear"].value1;
        huntWeaselAngrySpeed = InGameMgr.Instance.miniGameData["game1monster_bear"].value2;

        realArrowSpeed = huntArrowSpeed * standardHuntSpeed;
        realFoxSpeed = huntFoxSpeed * standardHuntSpeed;
        realWeaselSpeed = huntWeaselSpeed * standardHuntSpeed;

        resolutionScale = Screen.width / 1920f;
        heightScale = Screen.height / 1080f;
        StartPoint = new Vector2(StartP.x * resolutionScale + transform.position.x , StartP.y * heightScale + transform.position.y);
        EndPoint = new Vector2(EndP.x * resolutionScale + transform.position.x , EndP.y * heightScale + transform.position.y);
        StartPoint2 = new Vector2(StartP2.x * resolutionScale + transform.position.x , StartP2.y * heightScale + transform.position.y);
        EndPoint2 = new Vector2(EndP2.x * resolutionScale + transform.position.x , EndP2.y * heightScale + transform.position.y);

        TextTarget = new Vector2(GuidText.GetComponent<RectTransform>().anchoredPosition.x, GuidText.GetComponent<RectTransform>().anchoredPosition.y + MGM.textPosition);
    }

    
    IEnumerator SpawnMonster()
    {
        while(true)
        {
            rnd = Random.Range(1,3);
            if(rnd == 1)
            {
                x = Random.Range(StartPoint.x, EndPoint.x);
                y = Random.Range(StartPoint.y, EndPoint.y);
            }
            else
            {
                x = Random.Range(StartPoint2.x, EndPoint2.x);
                y = Random.Range(StartPoint2.y, EndPoint2.y);
            }

            rnd = Random.Range(1,101);

            if(foxNum < huntMonsterMaxNum && weaselNum < huntMonsterMaxNum)
            {
                if(rnd <= huntFoxPer)
                {
                    //fox Spawn
                    Instantiate(FoxPrefab, new Vector2(x,y), Quaternion.identity, GameObject.Find("Hunt").transform);
                    foxNum++;
                }
                else
                {
                    //weaselSpawn
                    Instantiate(WeaselPrefab, new Vector2(x,y), Quaternion.identity, GameObject.Find("Hunt").transform);
                    weaselNum++;
                }
            }
            else if(foxNum < huntMonsterMaxNum && weaselNum == huntMonsterMaxNum)
            {
                //fox
                Instantiate(FoxPrefab, new Vector2(x,y), Quaternion.identity, GameObject.Find("Hunt").transform);
                foxNum++;
            }

            else if(foxNum == huntMonsterMaxNum && weaselNum < huntMonsterMaxNum)
            {
                //bear
                Instantiate(WeaselPrefab, new Vector2(x,y), Quaternion.identity, GameObject.Find("Hunt").transform);
                weaselNum++;
            }

            yield return new WaitForSeconds(huntMonsterCoolTime);
        }
    }

    public void StartGame()
    {
        if(!isFisrt)
            StartCoroutine(ReStart());
    }

    IEnumerator ReStart()
    {
        GuidPanel.SetActive(true);
        isShoot = false;
        GuidText.GetComponent<RectTransform>().anchoredPosition = new Vector2(0,0);
        yield return new WaitForSeconds(MGM.firstTime);
        GuidPanel.SetActive(false);

        while (Vector2.Distance(GuidText.GetComponent<RectTransform>().anchoredPosition, TextTarget) >= 0.1f)
        {
            GuidText.GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(GuidText.GetComponent<RectTransform>().anchoredPosition, TextTarget, Time.deltaTime * MGM.textSpeed);

            yield return null;
        }

        StartCoroutine(SpawnMonster());
    }
    IEnumerator FirstStart()
    {
        yield return new WaitForSeconds(MGM.firstTime);
        GuidPanel.SetActive(false);
        isFisrt = false;

        while (Vector2.Distance(GuidText.GetComponent<RectTransform>().anchoredPosition, TextTarget) >= 0.1f)
        {
            GuidText.GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(GuidText.GetComponent<RectTransform>().anchoredPosition, TextTarget, Time.deltaTime * MGM.textSpeed);

            yield return null;
        }


        

        StartCoroutine(SpawnMonster());
    }

    public void KillFox()
    {
        MGM.meat += (int)huntFoxRes;
    }

    public void KillWeasel()
    {
        MGM.meat += (int)huntWeaselRes;
    }
    

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(StartPoint, 4f);
        Gizmos.DrawSphere(EndPoint, 4f);
        Gizmos.DrawSphere(StartPoint2, 4f);
        Gizmos.DrawSphere(EndPoint2, 4f);
    }

}
