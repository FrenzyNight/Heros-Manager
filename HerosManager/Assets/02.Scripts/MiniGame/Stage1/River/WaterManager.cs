using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterManager : MiniGameSetMgr
{
    public Vector2 SpawnPoint1, SpawnPoint2, SpawnPoint3;
    public Vector2 spawnP1, spawnP2, spawnP3;
    public GameObject Boots, Deadfish, Apple;
    private GameObject SelectedTrash;

    private int max;

    //read

    public float standardWaterSpeed = 300f;

    public int waterRes;
    public float waterGetTime;

    public float waterRockSpeed;
    public float waterRockCoolTime;
    public float waterRockMaxNum;

    public float waterRockInvTime;

    //real
    public float realRockSpeed;

    void Start()
    {
        SetUp();
    }

    public override void SetUp()
    {
        base.SetUp();

        Boots.GetComponent<MiniGameObjectMgr>().manager = gameObject;
        Deadfish.GetComponent<MiniGameObjectMgr>().manager = gameObject;
        Apple.GetComponent<MiniGameObjectMgr>().manager = gameObject;

        waterGetTime = LoadGameData.Instance.miniGameDatas["game1water"].value1;
        waterRes = LoadGameData.Instance.miniGameDatas["game1water"].GetAmount1;
        
        waterRockCoolTime = LoadGameData.Instance.miniGameDatas["game1rock"].CoolTime;
        waterRockInvTime = LoadGameData.Instance.miniGameDatas["game1rock"].Invincibility;
        waterRockMaxNum = LoadGameData.Instance.miniGameDatas["game1rock"].value2;
        waterRockSpeed = LoadGameData.Instance.miniGameDatas["game1rock"].Velocity;

        realRockSpeed = standardWaterSpeed * waterRockSpeed;

        item1ID = LoadGameData.Instance.miniGameDatas["game1water"].GetItemID1;

        SpawnPoint1 = new Vector2(spawnP1.x * widthScale + transform.position.x, spawnP1.y * heightScale + transform.position.y);
        SpawnPoint2 = new Vector2(spawnP2.x * widthScale + transform.position.x, spawnP2.y * heightScale + transform.position.y);
        SpawnPoint3 = new Vector2(spawnP3.x * widthScale + transform.position.x, spawnP3.y * heightScale + transform.position.y);
   
        StartGame();
   }

    public override void StartGame()
    {
        base.StartGame();

        foreach(GameObject obj in spawnObjects)
        {
            Destroy(obj);
        }

        spawnObjects.Clear();

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
                    spawnObjects.Add(Instantiate(SelectedTrash, SpawnPoint1, Quaternion.identity, mother.transform));
                }
                else if(rnd == 2)
                {
                    RandomObject();
                    spawnObjects.Add(Instantiate(SelectedTrash, SpawnPoint2, Quaternion.identity, mother.transform));
                }
                else if(rnd == 3)
                {
                    RandomObject();
                    spawnObjects.Add(Instantiate(SelectedTrash, SpawnPoint3, Quaternion.identity, mother.transform));
                }
            }
            else // 장애물 2개
            {
                rnd = Random.Range(1,4);
                if(rnd == 1) //1번 포인트 제외 소환
                {
                    RandomObject();
                    spawnObjects.Add(Instantiate(SelectedTrash, SpawnPoint2, Quaternion.identity, mother.transform));
                    RandomObject();
                    spawnObjects.Add(Instantiate(SelectedTrash, SpawnPoint3, Quaternion.identity, mother.transform));
                }
                else if(rnd == 2)
                {
                    RandomObject();
                    spawnObjects.Add(Instantiate(SelectedTrash, SpawnPoint1, Quaternion.identity, mother.transform));
                    RandomObject();
                    spawnObjects.Add(Instantiate(SelectedTrash, SpawnPoint3, Quaternion.identity, mother.transform));
                }
                else if(rnd == 3)
                {
                    RandomObject();
                    spawnObjects.Add(Instantiate(SelectedTrash, SpawnPoint1, Quaternion.identity, mother.transform));
                    RandomObject();
                    spawnObjects.Add(Instantiate(SelectedTrash, SpawnPoint2, Quaternion.identity, mother.transform));
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
        base.AddItem(item1ID, waterRes);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(SpawnPoint1, 3f);
        Gizmos.DrawSphere(SpawnPoint2, 3f);
        Gizmos.DrawSphere(SpawnPoint3, 3f);
    }
}
