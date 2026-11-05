using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class HuntManager : MiniGameSetMgr
{

    public GameObject FoxPrefab, BearPrefab;
    public GameObject ArrowPrefab;
    public GameObject Bow;
    public Vector2 StartP, EndP;
    public Vector2 StartP2, EndP2;
    private Vector2 StartPoint, EndPoint;
    private Vector2 StartPoint2, EndPoint2;
    private float x, y;
    public int foxNum;
    public int bearNum;

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
    public int huntFoxRes;
    public float huntFoxPer;
    public float huntFoxHP;

    public float huntBearSpeed;
    public int huntBearRes;
    public float huntBearPer;
    public float huntBearHP;
    public float huntBearAngrySpeed;

    //real
    public float realArrowSpeed;
    public float realFoxSpeed;
    public float realBearSpeed;


    // Start is called before the first frame update
    void Start()
    {
        //SetUp();
    }


    public override void SetUp()
    {
        base.SetUp();


        foxNum = 0;
        bearNum = 0;
        huntArrowSpeed = GameData("game1arrow").Velocity;
        huntArrowCoolTime = GameData("game1arrow").CoolTime;

        huntMonsterCoolTime = GameData("game1monster_fox").CoolTime;
        huntMonsterMaxNum = GameData("game1monster_fox").value6;
        huntMonsterMinMoveTime = GameData("game1monster_fox").value4;
        huntMonsterMaxMoveTime = GameData("game1monster_fox").value5;
        huntMonsterIdleTime= GameData("game1monster_fox").value3;

        huntFoxSpeed = GameData("game1monster_fox").Velocity;
        huntFoxRes = (int)GameData("game1monster_fox").GetAmount1;
        huntFoxPer = GameData("game1monster_fox").Probability * 100;
        huntFoxHP = GameData("game1monster_fox").value1;

        huntBearSpeed = GameData("game1monster_bear").Velocity;
        huntBearRes = (int)GameData("game1monster_bear").GetAmount1;
        huntBearPer = GameData("game1monster_bear").Probability * 100;
        huntBearHP = GameData("game1monster_bear").value1;
        huntBearAngrySpeed = GameData("game1monster_bear").value2;

        item1ID = GameData("game1monster_fox").GetItemID1;

        realArrowSpeed = huntArrowSpeed * standardHuntSpeed;
        realFoxSpeed = huntFoxSpeed * standardHuntSpeed;
        realBearSpeed = huntBearSpeed * standardHuntSpeed;

        StartPoint = ScreenPoint(StartP);
        EndPoint = ScreenPoint(EndP);
        StartPoint2 = ScreenPoint(StartP2);
        EndPoint2 = ScreenPoint(EndP2);
    
        StartGame();
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

            if(foxNum < huntMonsterMaxNum && bearNum < huntMonsterMaxNum)
            {
                if(MiniGameRules.PercentHit(rnd, huntFoxPer))
                {
                    //fox Spawn
                    Spawn(FoxPrefab, new Vector2(x,y), Quaternion.identity);
                    foxNum++;
                }
                else
                {
                    //bearSpawn
                    Spawn(BearPrefab, new Vector2(x,y), Quaternion.identity);
                    bearNum++;
                }
            }
            else if(foxNum < huntMonsterMaxNum && bearNum == huntMonsterMaxNum)
            {
                //fox
                Spawn(FoxPrefab, new Vector2(x,y), Quaternion.identity);
                foxNum++;
            }

            else if(foxNum == huntMonsterMaxNum && bearNum < huntMonsterMaxNum)
            {
                //bear
                Spawn(BearPrefab, new Vector2(x,y), Quaternion.identity);
                bearNum++;
            }

            yield return new WaitForSeconds(MiniGameRules.PositiveDelay(huntMonsterCoolTime));
        }
    }

    public override void StartGame()
    {
        base.StartGame();


        Bow.GetComponent<BowAngle>().isCheck = false;

        StartCoroutine(SpawnMonster());
    }


    public void KillFox()
    {
        base.AddItem(item1ID, huntFoxRes);
    }

    public void KillBear()
    {
        base.AddItem(item1ID, huntBearRes);
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
