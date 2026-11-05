using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class WaterManager : MiniGameSetMgr
{
    [Header("Water Mgr")]
    AudioSource audioSource;
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

    [HideInInspector] public Coroutine coroutine;
    
    public override void SetUp()
    {
        base.SetUp();
        audioSource = GetComponent<AudioSource>();


        waterGetTime = GameData("game1water").value1;
        waterRes = GameData("game1water").GetAmount1;
        
        waterRockCoolTime = GameData("game1rock").CoolTime;
        waterRockInvTime = GameData("game1rock").Invincibility;
        waterRockMaxNum = GameData("game1rock").value2;
        waterRockSpeed = GameData("game1rock").Velocity;

        realRockSpeed = standardWaterSpeed * waterRockSpeed;

        item1ID = GameData("game1water").GetItemID1;

        SpawnPoint1 = ScreenPoint(spawnP1);
        SpawnPoint2 = ScreenPoint(spawnP2);
        SpawnPoint3 = ScreenPoint(spawnP3);

     
        StartGame();
   }

    public override void StartGame()
    {
        base.StartGame();


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
                    Spawn(SelectedTrash, SpawnPoint1, Quaternion.identity);
                }
                else if(rnd == 2)
                {
                    RandomObject();
                    Spawn(SelectedTrash, SpawnPoint2, Quaternion.identity);
                }
                else if(rnd == 3)
                {
                    RandomObject();
                    Spawn(SelectedTrash, SpawnPoint3, Quaternion.identity);
                }
            }
            else // 장애물 2개
            {
                rnd = Random.Range(1,4);
                if(rnd == 1) //1번 포인트 제외 소환
                {
                    RandomObject();
                    Spawn(SelectedTrash, SpawnPoint2, Quaternion.identity);
                    RandomObject();
                    Spawn(SelectedTrash, SpawnPoint3, Quaternion.identity);
                }
                else if(rnd == 2)
                {
                    RandomObject();
                    Spawn(SelectedTrash, SpawnPoint1, Quaternion.identity);
                    RandomObject();
                    Spawn(SelectedTrash, SpawnPoint3, Quaternion.identity);
                }
                else if(rnd == 3)
                {
                    RandomObject();
                    Spawn(SelectedTrash, SpawnPoint1, Quaternion.identity);
                    RandomObject();
                    Spawn(SelectedTrash, SpawnPoint2, Quaternion.identity);
                }
            }

            yield return new WaitForSeconds(MiniGameRules.PositiveDelay(waterRockCoolTime));
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
        audioSource.Play();
        base.AddItem(item1ID, waterRes);
    }

    /*
    private void OnEnable()
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
            StartGame();
        }
    }
    */

    public override void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(SpawnPoint1, 3f);
        Gizmos.DrawSphere(SpawnPoint2, 3f);
        Gizmos.DrawSphere(SpawnPoint3, 3f);
    }
}
