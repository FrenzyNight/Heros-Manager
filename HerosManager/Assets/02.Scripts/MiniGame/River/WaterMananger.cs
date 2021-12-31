using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterMananger : MonoBehaviour
{
    private RiverCSVReader RiverCSV;

    public Vector2 SpawnPoint1, SpawnPoint2, SpawnPoint3;
    public GameObject RockPrefab;
    private bool isFirst, isSecond, isThird;

    private int max;
    private int rnd;




    // Start is called before the first frame update
    void Start()
    {
        RiverCSV = GameObject.Find("RiverCSVReader").GetComponent<RiverCSVReader>();
        isFirst = false;
        isSecond = false;
        isThird = false;
        max = (int)RiverCSV.waterRockMaxNum;
        //StartCoroutine(SpawnRock());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
/*
    IEnumerator SpawnRock()
    {
       while(true)
        {
            rnd = Random.Range(0,max);
            if(rnd == 0) // 장애물 1개
            {
                rnd = Random.Range(1,4);
                if(rnd == 1) //1번 포인트 소환
                {
                    Instantiate(RockPrefab, SpawnPoint1, Quaternion.identity);
                }
                else if(rnd == 2)
                {
                    Instantiate(RockPrefab, SpawnPoint2, Quaternion.identity);
                }
                else if(rnd == 3)
                {
                    Instantiate(RockPrefab, SpawnPoint3, Quaternion.identity);
                }
            }
            else // 장애물 2개
            {
                rnd = Random.Range(1,4);
                if(rnd == 1) //1번 포인트 제외 소환
                {
                    Instantiate(RockPrefab, SpawnPoint2, Quaternion.identity);
                    Instantiate(RockPrefab, SpawnPoint3, Quaternion.identity);
                }
                else if(rnd == 2)
                {
                    Instantiate(RockPrefab, SpawnPoint1, Quaternion.identity);
                    Instantiate(RockPrefab, SpawnPoint3, Quaternion.identity);
                }
                else if(rnd == 3)
                {
                    Instantiate(RockPrefab, SpawnPoint1, Quaternion.identity);
                    Instantiate(RockPrefab, SpawnPoint2, Quaternion.identity);
                }
            }

            yield return new WaitForSeconds(RiverCSV.waterRockCoolTime);
        }
    }

    */

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(SpawnPoint1, 0.1f);
        Gizmos.DrawSphere(SpawnPoint2, 0.1f);
        Gizmos.DrawSphere(SpawnPoint3, 0.1f);
    }
}
