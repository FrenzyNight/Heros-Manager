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
    public GameObject Cactus;
    private List<GameObject> CacList = new List<GameObject>();
    public Vector2 StartP, EndP;
    public Vector2 StartP2, EndP2;
    private Vector2 StartPoint, EndPoint;
    private Vector2 StartPoint2, EndPoint2;
    private float x, y;
    public int foxNum;
    public int weaselNum;
    public int goldFoxNum;
    private int rnd;

    //cactus
    public float quater, plusmin;

    //read
    public float standardHuntSpeed = 200f;
    public float huntArrowSpeed;
    public float huntArrowCoolTime;

    public float huntCactusNum;

    public float huntMonsterCoolTime;
    public float huntMonsterMinMoveTime;
    public float huntMonsterMaxMoveTime;
    public float huntMonsterIdleTime;

    public float huntFoxSpeed;
    public float huntFoxRes;
    public float huntFoxPer;
    public float huntFoxHP;
    public float huntFoxAngrySpeed;
    public float huntFoxMaxNum;

    public float huntGoldFoxSpeed;
    public float huntGoldFoxRes;
    public float huntGoldFoxPer;
    public float huntGoldFoxHP;
    public float huntGoldFoxAngrySpeed;
    public float huntGoldFoxMaxNum;

    public float huntWeaselSpeed;
    public float huntWeaselRes;
    public float huntWeaselPer;
    public float huntWeaselHP;
    public float huntWeasel;
    public float huntWeaselMaxNum;

    //real
    public float realArrowSpeed;
    public float realFoxSpeed;
    public float realGoldFoxSpeed;
    public float realWeaselSpeed;

    public bool isShoot;

    public float resolutionScale;
    public float heightScale;

    // Start is called before the first frame update
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
        foxNum = 0;
        weaselNum = 0;
        goldFoxNum = 0;
        isShoot = false;
        
        huntArrowSpeed = InGameMgr.Instance.miniGameData["game2arrow"].speed;
        huntArrowCoolTime = InGameMgr.Instance.miniGameData["game2arrow"].cooltime;

        huntCactusNum = InGameMgr.Instance.miniGameData["game2cactus"].value1;

        huntMonsterCoolTime = InGameMgr.Instance.miniGameData["game2monster_weasel"].cooltime;
        huntMonsterMinMoveTime = InGameMgr.Instance.miniGameData["game2monster_weasel"].value4;
        huntMonsterMaxMoveTime = InGameMgr.Instance.miniGameData["game2monster_weasel"].value5;
        huntMonsterIdleTime= InGameMgr.Instance.miniGameData["game2monster_weasel"].value3;

        huntWeaselSpeed = InGameMgr.Instance.miniGameData["game2monster_weasel"].speed;
        huntWeaselRes = InGameMgr.Instance.miniGameData["game2monster_weasel"].meat;
        huntWeaselPer = InGameMgr.Instance.miniGameData["game2monster_weasel"].probability * 100;
        huntWeaselHP = InGameMgr.Instance.miniGameData["game2monster_weasel"].value1;
        huntWeaselMaxNum = InGameMgr.Instance.miniGameData["game2monster_weasel"].value6;

        huntFoxSpeed = InGameMgr.Instance.miniGameData["game2monster_fennec"].speed;
        huntFoxRes = InGameMgr.Instance.miniGameData["game2monster_fennec"].meat;
        huntFoxPer = InGameMgr.Instance.miniGameData["game2monster_fennec"].probability * 100;
        huntFoxHP = InGameMgr.Instance.miniGameData["game2monster_fennec"].value1;
        huntFoxAngrySpeed = InGameMgr.Instance.miniGameData["game2monster_fennec"].value2;
        huntFoxMaxNum = InGameMgr.Instance.miniGameData["game2monster_fennec"].value6;

        huntGoldFoxSpeed = InGameMgr.Instance.miniGameData["game2monser_gfennec"].speed;
        huntGoldFoxRes = InGameMgr.Instance.miniGameData["game2monser_gfennec"].meat;
        huntGoldFoxPer = InGameMgr.Instance.miniGameData["game2monser_gfennec"].probability * 100;
        huntGoldFoxHP = InGameMgr.Instance.miniGameData["game2monser_gfennec"].value1;
        huntGoldFoxAngrySpeed = InGameMgr.Instance.miniGameData["game2monser_gfennec"].value2;
        huntGoldFoxMaxNum = InGameMgr.Instance.miniGameData["game2monser_gfennec"].value6;

        realArrowSpeed = huntArrowSpeed * standardHuntSpeed;

        realFoxSpeed = huntFoxSpeed * standardHuntSpeed;
        realGoldFoxSpeed = huntGoldFoxSpeed * standardHuntSpeed;
        realWeaselSpeed = huntWeaselSpeed * standardHuntSpeed;

        resolutionScale = Screen.width / 1920f;
        heightScale = Screen.height / 1080f;

        StartPoint = new Vector2(StartP.x * resolutionScale + transform.position.x , StartP.y * heightScale + transform.position.y);
        EndPoint = new Vector2(EndP.x * resolutionScale + transform.position.x , EndP.y * heightScale + transform.position.y);
        StartPoint2 = new Vector2(StartP2.x * resolutionScale + transform.position.x , StartP2.y * heightScale + transform.position.y);
        EndPoint2 = new Vector2(EndP2.x * resolutionScale + transform.position.x , EndP2.y * heightScale + transform.position.y);

        TextTarget = new Vector2(GuidText.GetComponent<RectTransform>().anchoredPosition.x, GuidText.GetComponent<RectTransform>().anchoredPosition.y + MGM.textPosition);
    }

    private void SpawnCactus()
    {
        
        for(int j=0;j<(int)huntCactusNum/4;j++) // ++
        {
            x = Random.Range(transform.position.x + quater-plusmin, transform.position.x + quater+plusmin);
            y = Random.Range(transform.position.y + quater-plusmin, transform.position.y + quater+plusmin);

           CacList.Add(Instantiate(Cactus, new Vector2(x,y), Quaternion.identity,GameObject.Find("Hunt2").transform));
        }
        for(int j=0;j<(int)huntCactusNum/4;j++) // +-
        {
            x = Random.Range(transform.position.x + quater-plusmin, transform.position.x + quater+plusmin);
            y = Random.Range(transform.position.y -quater-plusmin, transform.position.y-quater+plusmin);

            CacList.Add(Instantiate(Cactus, new Vector2(x,y), Quaternion.identity,GameObject.Find("Hunt2").transform));
        }
        for(int j=0;j<(int)huntCactusNum/4;j++) // --
        {
            x = Random.Range(transform.position.x-quater-plusmin, transform.position.x-quater+plusmin);
            y = Random.Range(transform.position.y-quater-plusmin, transform.position.y-quater+plusmin);

            CacList.Add(Instantiate(Cactus, new Vector2(x,y), Quaternion.identity,GameObject.Find("Hunt2").transform));
        }
        for(int j=0;j<(int)huntCactusNum/4;j++) // -+
        {
            x = Random.Range(transform.position.x-quater-plusmin, transform.position.x-quater+plusmin);
            y = Random.Range(transform.position.y+ quater-plusmin, transform.position.y+ quater+plusmin);

            CacList.Add(Instantiate(Cactus, new Vector2(x,y), Quaternion.identity,GameObject.Find("Hunt2").transform));
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

            if(foxNum < huntFoxMaxNum && weaselNum < huntWeaselMaxNum && goldFoxNum < huntGoldFoxMaxNum)
            {
                if(rnd <= huntGoldFoxPer)
                {
                    //Gold fox Spawn
                    Instantiate(GoldFoxPrefab, new Vector2(x,y), Quaternion.identity, GameObject.Find("Hunt2").transform);
                    goldFoxNum++;
                }
                else if(rnd <= huntFoxPer+huntGoldFoxPer)
                {
                    //Fox Spawn
                    Instantiate(FoxPrefab, new Vector2(x,y), Quaternion.identity, GameObject.Find("Hunt2").transform);
                    foxNum++;
                }
                else
                {
                    //Weasel Spawn
                    Instantiate(WeaselPrefab, new Vector2(x,y), Quaternion.identity, GameObject.Find("Hunt2").transform);
                    weaselNum++;
                }
            }
            else if(foxNum < huntFoxMaxNum && weaselNum == huntWeaselMaxNum &&  goldFoxNum == huntGoldFoxMaxNum)
            {
                //fox
                Instantiate(FoxPrefab, new Vector2(x,y), Quaternion.identity, GameObject.Find("Hunt2").transform);
                foxNum++;
            }

            else if(foxNum == huntFoxMaxNum && weaselNum < huntWeaselMaxNum &&  goldFoxNum == huntGoldFoxMaxNum)
            {
                //weasel
                Instantiate(WeaselPrefab, new Vector2(x,y), Quaternion.identity, GameObject.Find("Hunt2").transform);
                weaselNum++;
            }
            else if(foxNum == huntFoxMaxNum && weaselNum == huntWeaselMaxNum && goldFoxNum < huntGoldFoxMaxNum)
            {
                //Gold fox Spawn
                Instantiate(GoldFoxPrefab, new Vector2(x,y), Quaternion.identity, GameObject.Find("Hunt2").transform);
                goldFoxNum++;
            }
            else if(foxNum < huntFoxMaxNum && weaselNum == huntWeaselMaxNum &&  goldFoxNum < huntGoldFoxMaxNum)
            {
                if(rnd <= huntGoldFoxPer)
                {
                    //Gold fox Spawn
                    Instantiate(GoldFoxPrefab, new Vector2(x,y), Quaternion.identity, GameObject.Find("Hunt2").transform);
                    goldFoxNum++;
                }
                else
                {
                    //Fox Spawn
                    Instantiate(FoxPrefab, new Vector2(x,y), Quaternion.identity, GameObject.Find("Hunt2").transform);
                    foxNum++;
                }
            }
            else if(foxNum == huntFoxMaxNum && weaselNum < huntWeaselMaxNum &&  goldFoxNum < huntGoldFoxMaxNum)
            {
                if(rnd <= huntGoldFoxPer)
                {
                    //Gold fox Spawn
                    Instantiate(GoldFoxPrefab, new Vector2(x,y), Quaternion.identity, GameObject.Find("Hunt2").transform);
                    goldFoxNum++;
                }
                else
                {
                    //weasel
                    Instantiate(WeaselPrefab, new Vector2(x,y), Quaternion.identity, GameObject.Find("Hunt2").transform);
                    weaselNum++;
                }
            }
            else if(foxNum < huntFoxMaxNum && weaselNum < huntWeaselMaxNum &&  goldFoxNum == huntGoldFoxMaxNum)
            {
                if(rnd <= huntFoxPer)
                {
                    //fox Spawn
                    Instantiate(FoxPrefab, new Vector2(x,y), Quaternion.identity, GameObject.Find("Hunt2").transform);
                    foxNum++;
                }
                else
                {
                    //weasel
                    Instantiate(WeaselPrefab, new Vector2(x,y), Quaternion.identity, GameObject.Find("Hunt2").transform);
                    weaselNum++;
                }
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

        RemoveCactus();
        SpawnCactus();
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

    public void KillGoldFox()
    {
        MGM.meat += (int)huntGoldFoxRes;
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
