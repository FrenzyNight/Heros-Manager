using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterManager : MonoBehaviour
{

    public Vector2 SpawnPoint1, SpawnPoint2, SpawnPoint3;
    public Vector2 spawnP1, spawnP2, spawnP3;
    public GameObject Boots, Deadfish, Apple;
    private GameObject SelectedTrash;
    private bool isFirst, isSecond, isThird;

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
        InGameMgr.Instance.EnterMiniGame("Stage_1_river");

        SetUp();
        StartCoroutine(SpawnRock());
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
    

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(SpawnPoint1, 3f);
        Gizmos.DrawSphere(SpawnPoint2, 3f);
        Gizmos.DrawSphere(SpawnPoint3, 3f);
    }
}
