using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class HuntManager : MiniGameSetMgr
{

    public GameObject FoxPrefab, BearPrefab;
    public GameObject ArrowPrefab;
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
        SetUp();
    }


    public override void SetUp()
    {
        base.SetUp();

        BearPrefab.GetComponent<MiniGameObjectMgr>().manager = gameObject;
        FoxPrefab.GetComponent<MiniGameObjectMgr>().manager = gameObject;
        ArrowPrefab.transform.GetChild(0).GetComponent<MiniGameObjectMgr>().manager = gameObject;

        foxNum = 0;
        bearNum = 0;
        huntArrowSpeed = LoadGameData.Instance.miniGameDatas["game1arrow"].Velocity;
        huntArrowCoolTime = LoadGameData.Instance.miniGameDatas["game1arrow"].CoolTime;

        huntMonsterCoolTime = LoadGameData.Instance.miniGameDatas["game1monster_fox"].CoolTime;
        huntMonsterMaxNum = LoadGameData.Instance.miniGameDatas["game1monster_fox"].value6;
        huntMonsterMinMoveTime = LoadGameData.Instance.miniGameDatas["game1monster_fox"].value4;
        huntMonsterMaxMoveTime = LoadGameData.Instance.miniGameDatas["game1monster_fox"].value5;
        huntMonsterIdleTime= LoadGameData.Instance.miniGameDatas["game1monster_fox"].value3;

        huntFoxSpeed = LoadGameData.Instance.miniGameDatas["game1monster_fox"].Velocity;
        huntFoxRes = (int)LoadGameData.Instance.miniGameDatas["game1monster_fox"].GetAmount1;
        huntFoxPer = LoadGameData.Instance.miniGameDatas["game1monster_fox"].Probability * 100;
        huntFoxHP = LoadGameData.Instance.miniGameDatas["game1monster_fox"].value1;

        huntBearSpeed = LoadGameData.Instance.miniGameDatas["game1monster_bear"].Velocity;
        huntBearRes = (int)LoadGameData.Instance.miniGameDatas["game1monster_bear"].GetAmount1;
        huntBearPer = LoadGameData.Instance.miniGameDatas["game1monster_bear"].Probability * 100;
        huntBearHP = LoadGameData.Instance.miniGameDatas["game1monster_bear"].value1;
        huntBearAngrySpeed = LoadGameData.Instance.miniGameDatas["game1monster_bear"].value2;

        item1ID = LoadGameData.Instance.miniGameDatas["game1monster_fox"].GetItemID1;

        realArrowSpeed = huntArrowSpeed * standardHuntSpeed;
        realFoxSpeed = huntFoxSpeed * standardHuntSpeed;
        realBearSpeed = huntBearSpeed * standardHuntSpeed;

        StartPoint = new Vector2(StartP.x * widthScale + transform.position.x , StartP.y * heightScale + transform.position.y);
        EndPoint = new Vector2(EndP.x * widthScale + transform.position.x , EndP.y * heightScale + transform.position.y);
        StartPoint2 = new Vector2(StartP2.x * widthScale + transform.position.x , StartP2.y * heightScale + transform.position.y);
        EndPoint2 = new Vector2(EndP2.x * widthScale + transform.position.x , EndP2.y * heightScale + transform.position.y);
    
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
                if(rnd <= huntFoxPer)
                {
                    //fox Spawn
                    Instantiate(FoxPrefab, new Vector2(x,y), Quaternion.identity, mother.transform);
                    foxNum++;
                }
                else
                {
                    //bearSpawn
                    Instantiate(BearPrefab, new Vector2(x,y), Quaternion.identity, mother.transform);
                    bearNum++;
                }
            }
            else if(foxNum < huntMonsterMaxNum && bearNum == huntMonsterMaxNum)
            {
                //fox
                Instantiate(FoxPrefab, new Vector2(x,y), Quaternion.identity, mother.transform);
                foxNum++;
            }

            else if(foxNum == huntMonsterMaxNum && bearNum < huntMonsterMaxNum)
            {
                //bear
                Instantiate(BearPrefab, new Vector2(x,y), Quaternion.identity, mother.transform);
                bearNum++;
            }

            yield return new WaitForSeconds(huntMonsterCoolTime);
        }
    }

    public void StartGame()
    {
        base.StartGame();
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
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(StartPoint, 4f);
        Gizmos.DrawSphere(EndPoint, 4f);
        Gizmos.DrawSphere(StartPoint2, 4f);
        Gizmos.DrawSphere(EndPoint2, 4f);
    }

}
