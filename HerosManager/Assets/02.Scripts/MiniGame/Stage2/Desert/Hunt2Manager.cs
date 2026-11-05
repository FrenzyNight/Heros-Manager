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
        if (!IsPrepared)
            SetUp();
    }

    public override void SetUp()
    {
        base.SetUp();
        CacList.Clear();
        foxNum = 0;
        weaselNum = 0;
        goldFoxNum = 0;
        isShoot = false;
        
        huntArrowSpeed = GameData("game2arrow").Velocity;
        huntArrowCoolTime = GameData("game2arrow").CoolTime;

        huntCactusNum = GameData("game2cactus").value1;

        huntMonsterCoolTime = GameData("game2monster_weasel").CoolTime;
        huntMonsterMinMoveTime = GameData("game2monster_weasel").value4;
        huntMonsterMaxMoveTime = GameData("game2monster_weasel").value5;
        huntMonsterIdleTime= GameData("game2monster_weasel").value3;

        huntWeaselSpeed = GameData("game2monster_weasel").Velocity;
        huntWeaselRes = GameData("game2monster_weasel").GetAmount1;
        huntWeaselPer = GameData("game2monster_weasel").Probability * 100;
        huntWeaselHP = GameData("game2monster_weasel").value1;
        huntWeaselMaxNum = GameData("game2monster_weasel").value6;

        huntFoxSpeed = GameData("game2monster_fennec").Velocity;
        huntFoxRes = GameData("game2monster_fennec").GetAmount1;
        huntFoxPer = GameData("game2monster_fennec").Probability * 100;
        huntFoxHP = GameData("game2monster_fennec").value1;
        huntFoxAngrySpeed = GameData("game2monster_fennec").value2;
        huntFoxMaxNum = GameData("game2monster_fennec").value6;

        huntGoldFoxSpeed = GameData("game2monser_gfennec").Velocity;
        huntGoldFoxRes = GameData("game2monser_gfennec").GetAmount1;
        huntGoldFoxPer = GameData("game2monser_gfennec").Probability * 100;
        huntGoldFoxHP = GameData("game2monser_gfennec").value1;
        huntGoldFoxAngrySpeed = GameData("game2monser_gfennec").value2;
        huntGoldFoxMaxNum = GameData("game2monser_gfennec").value6;

        realArrowSpeed = huntArrowSpeed * standardHuntSpeed;

        realFoxSpeed = huntFoxSpeed * standardHuntSpeed;
        realGoldFoxSpeed = huntGoldFoxSpeed * standardHuntSpeed;
        realWeaselSpeed = huntWeaselSpeed * standardHuntSpeed;


        StartPoint = ScreenPoint(StartP);
        EndPoint = ScreenPoint(EndP);
        StartPoint2 = ScreenPoint(StartP2);
        EndPoint2 = ScreenPoint(EndP2);
        SpawnCactus();
 }

    private void SpawnCactus()
    {
        
        for(int j=0;j<(int)huntCactusNum/4;j++) // ++
        {
            x = Random.Range(transform.position.x + quater-plusmin, transform.position.x + quater+plusmin);
            y = Random.Range(transform.position.y + quater-plusmin, transform.position.y + quater+plusmin);

           CacList.Add(Spawn(Cactus, new Vector2(x,y), Quaternion.identity,SpawnContainer("Hunt2")));
        }
        for(int j=0;j<(int)huntCactusNum/4;j++) // +-
        {
            x = Random.Range(transform.position.x + quater-plusmin, transform.position.x + quater+plusmin);
            y = Random.Range(transform.position.y -quater-plusmin, transform.position.y-quater+plusmin);

            CacList.Add(Spawn(Cactus, new Vector2(x,y), Quaternion.identity,SpawnContainer("Hunt2")));
        }
        for(int j=0;j<(int)huntCactusNum/4;j++) // --
        {
            x = Random.Range(transform.position.x-quater-plusmin, transform.position.x-quater+plusmin);
            y = Random.Range(transform.position.y-quater-plusmin, transform.position.y-quater+plusmin);

            CacList.Add(Spawn(Cactus, new Vector2(x,y), Quaternion.identity,SpawnContainer("Hunt2")));
        }
        for(int j=0;j<(int)huntCactusNum/4;j++) // -+
        {
            x = Random.Range(transform.position.x-quater-plusmin, transform.position.x-quater+plusmin);
            y = Random.Range(transform.position.y+ quater-plusmin, transform.position.y+ quater+plusmin);

            CacList.Add(Spawn(Cactus, new Vector2(x,y), Quaternion.identity,SpawnContainer("Hunt2")));
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
                if(MiniGameRules.PercentHit(rnd, huntGoldFoxPer))
                {
                    //Gold fox Spawn
                    Spawn(GoldFoxPrefab, new Vector2(x,y), Quaternion.identity, SpawnContainer("Hunt2"));
                    goldFoxNum++;
                }
                else if(MiniGameRules.PercentHit(rnd, huntFoxPer+huntGoldFoxPer))
                {
                    //Fox Spawn
                    Spawn(FoxPrefab, new Vector2(x,y), Quaternion.identity, SpawnContainer("Hunt2"));
                    foxNum++;
                }
                else
                {
                    //Weasel Spawn
                    Spawn(WeaselPrefab, new Vector2(x,y), Quaternion.identity, SpawnContainer("Hunt2"));
                    weaselNum++;
                }
            }
            else if(foxNum < huntFoxMaxNum && weaselNum == huntWeaselMaxNum &&  goldFoxNum == huntGoldFoxMaxNum)
            {
                //fox
                Spawn(FoxPrefab, new Vector2(x,y), Quaternion.identity, SpawnContainer("Hunt2"));
                foxNum++;
            }

            else if(foxNum == huntFoxMaxNum && weaselNum < huntWeaselMaxNum &&  goldFoxNum == huntGoldFoxMaxNum)
            {
                //weasel
                Spawn(WeaselPrefab, new Vector2(x,y), Quaternion.identity, SpawnContainer("Hunt2"));
                weaselNum++;
            }
            else if(foxNum == huntFoxMaxNum && weaselNum == huntWeaselMaxNum && goldFoxNum < huntGoldFoxMaxNum)
            {
                //Gold fox Spawn
                Spawn(GoldFoxPrefab, new Vector2(x,y), Quaternion.identity, SpawnContainer("Hunt2"));
                goldFoxNum++;
            }
            else if(foxNum < huntFoxMaxNum && weaselNum == huntWeaselMaxNum &&  goldFoxNum < huntGoldFoxMaxNum)
            {
                if(MiniGameRules.PercentHit(rnd, huntGoldFoxPer))
                {
                    //Gold fox Spawn
                    Spawn(GoldFoxPrefab, new Vector2(x,y), Quaternion.identity, SpawnContainer("Hunt2"));
                    goldFoxNum++;
                }
                else
                {
                    //Fox Spawn
                    Spawn(FoxPrefab, new Vector2(x,y), Quaternion.identity, SpawnContainer("Hunt2"));
                    foxNum++;
                }
            }
            else if(foxNum == huntFoxMaxNum && weaselNum < huntWeaselMaxNum &&  goldFoxNum < huntGoldFoxMaxNum)
            {
                if(MiniGameRules.PercentHit(rnd, huntGoldFoxPer))
                {
                    //Gold fox Spawn
                    Spawn(GoldFoxPrefab, new Vector2(x,y), Quaternion.identity, SpawnContainer("Hunt2"));
                    goldFoxNum++;
                }
                else
                {
                    //weasel
                    Spawn(WeaselPrefab, new Vector2(x,y), Quaternion.identity, SpawnContainer("Hunt2"));
                    weaselNum++;
                }
            }
            else if(foxNum < huntFoxMaxNum && weaselNum < huntWeaselMaxNum &&  goldFoxNum == huntGoldFoxMaxNum)
            {
                if(MiniGameRules.PercentHit(rnd, huntFoxPer))
                {
                    //fox Spawn
                    Spawn(FoxPrefab, new Vector2(x,y), Quaternion.identity, SpawnContainer("Hunt2"));
                    foxNum++;
                }
                else
                {
                    //weasel
                    Spawn(WeaselPrefab, new Vector2(x,y), Quaternion.identity, SpawnContainer("Hunt2"));
                    weaselNum++;
                }
            }

            yield return new WaitForSeconds(MiniGameRules.PositiveDelay(huntMonsterCoolTime));
        }
    }

    public override void StartGame()
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
    

    public override void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(StartPoint, 4f);
        Gizmos.DrawSphere(EndPoint, 4f);
        Gizmos.DrawSphere(StartPoint2, 4f);
        Gizmos.DrawSphere(EndPoint2, 4f);
    }

}
