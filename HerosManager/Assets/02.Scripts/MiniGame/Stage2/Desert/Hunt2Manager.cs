using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
 
public class Hunt2Manager : MiniGameSetMgr
{
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

    // Start is called before the first frame update
    void Start()
    {
        SetUp();
        SpawnCactus();
    }

    void SetUp()
    {
        foxNum = 0;
        weaselNum = 0;
        goldFoxNum = 0;
        isShoot = false;
        
        huntArrowSpeed = LoadGameData.Instance.miniGameDatas["game2arrow"].Velocity;
        huntArrowCoolTime = LoadGameData.Instance.miniGameDatas["game2arrow"].CoolTime;

        huntCactusNum = LoadGameData.Instance.miniGameDatas["game2cactus"].value1;

        huntMonsterCoolTime = LoadGameData.Instance.miniGameDatas["game2monster_weasel"].CoolTime;
        huntMonsterMinMoveTime = LoadGameData.Instance.miniGameDatas["game2monster_weasel"].value4;
        huntMonsterMaxMoveTime = LoadGameData.Instance.miniGameDatas["game2monster_weasel"].value5;
        huntMonsterIdleTime= LoadGameData.Instance.miniGameDatas["game2monster_weasel"].value3;

        huntWeaselSpeed = LoadGameData.Instance.miniGameDatas["game2monster_weasel"].Velocity;
        huntWeaselRes = LoadGameData.Instance.miniGameDatas["game2monster_weasel"].GetAmount1;
        huntWeaselPer = LoadGameData.Instance.miniGameDatas["game2monster_weasel"].Probability * 100;
        huntWeaselHP = LoadGameData.Instance.miniGameDatas["game2monster_weasel"].value1;
        huntWeaselMaxNum = LoadGameData.Instance.miniGameDatas["game2monster_weasel"].value6;

        huntFoxSpeed = LoadGameData.Instance.miniGameDatas["game2monster_fennec"].Velocity;
        huntFoxRes = LoadGameData.Instance.miniGameDatas["game2monster_fennec"].GetAmount1;
        huntFoxPer = LoadGameData.Instance.miniGameDatas["game2monster_fennec"].Probability * 100;
        huntFoxHP = LoadGameData.Instance.miniGameDatas["game2monster_fennec"].value1;
        huntFoxAngrySpeed = LoadGameData.Instance.miniGameDatas["game2monster_fennec"].value2;
        huntFoxMaxNum = LoadGameData.Instance.miniGameDatas["game2monster_fennec"].value6;

        huntGoldFoxSpeed = LoadGameData.Instance.miniGameDatas["game2monser_gfennec"].Velocity;
        huntGoldFoxRes = LoadGameData.Instance.miniGameDatas["game2monser_gfennec"].GetAmount1;
        huntGoldFoxPer = LoadGameData.Instance.miniGameDatas["game2monser_gfennec"].Probability * 100;
        huntGoldFoxHP = LoadGameData.Instance.miniGameDatas["game2monser_gfennec"].value1;
        huntGoldFoxAngrySpeed = LoadGameData.Instance.miniGameDatas["game2monser_gfennec"].value2;
        huntGoldFoxMaxNum = LoadGameData.Instance.miniGameDatas["game2monser_gfennec"].value6;

        realArrowSpeed = huntArrowSpeed * standardHuntSpeed;

        realFoxSpeed = huntFoxSpeed * standardHuntSpeed;
        realGoldFoxSpeed = huntGoldFoxSpeed * standardHuntSpeed;
        realWeaselSpeed = huntWeaselSpeed * standardHuntSpeed;


        StartPoint = new Vector2(StartP.x * widthScale + transform.position.x , StartP.y * heightScale + transform.position.y);
        EndPoint = new Vector2(EndP.x * widthScale + transform.position.x , EndP.y * heightScale + transform.position.y);
        StartPoint2 = new Vector2(StartP2.x * widthScale + transform.position.x , StartP2.y * heightScale + transform.position.y);
        EndPoint2 = new Vector2(EndP2.x * widthScale + transform.position.x , EndP2.y * heightScale + transform.position.y);
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
        
    }

    

    public void KillFox()
    {
        //MGM.meat += (int)huntFoxRes;
    }

    public void KillGoldFox()
    {
        //MGM.meat += (int)huntGoldFoxRes;
    }

    public void KillWeasel()
    {
        //MGM.meat += (int)huntWeaselRes;
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
