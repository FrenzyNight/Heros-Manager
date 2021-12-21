using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class CaveManager : MonoBehaviour
{
    public GameObject JemPrefab, DownStonePrefab, UpStonePrefab;
    public Vector2 BlockSpawnPoint;

    public bool isRun;

    private float standardSpeed = 5f;

    private float varSpeed;

    public float realSpeed;
    public float realStunTime;

    private float coolTime;

    private float downStoneStunTime;
    private float upStoneStunTime;
    private float downStonePer;
    private float upStonePer;
    
    private float jemResource;
    private float jemPer;

    private int rnd;

    void Start()
    {
        CSVTest();
        isRun = true;
        realSpeed = standardSpeed * varSpeed;
        realStunTime = upStoneStunTime;
        StartCoroutine("SpawnObject");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator SpawnObject()
    {
        while(true)
        {
            if(isRun)
            {
                rnd = Random.Range(1,101);

                if(rnd <= jemPer)
                {
                    //JemSpawn
                    Instantiate(JemPrefab, BlockSpawnPoint, Quaternion.identity);
                }
                else if(jemPer < rnd && rnd <= jemPer + downStonePer)
                {
                    // DownStoneSpawn
                    Instantiate(DownStonePrefab, BlockSpawnPoint, Quaternion.identity);
                }
                else if(jemPer + downStonePer < rnd && rnd <= jemPer + downStonePer + upStonePer)
                {
                    //UpStoneSpawn;
                    Instantiate(UpStonePrefab, new Vector2(BlockSpawnPoint.x, BlockSpawnPoint.y + 2f), Quaternion.identity);
                }
                else
                {
                    //Debug.Log("rnd : " + rnd.ToString());
                }
            }

            yield return new WaitForSeconds(coolTime);
        }
    }


    void CSVTest()
    {
        StreamReader sr = new StreamReader(Application.dataPath + "/" + "Resources/CSVData/MiniGameData.csv");

        int row = 0;
        bool endOfFile = false;
        while(!endOfFile)
        {
            row ++;
            string data_String = sr.ReadLine();
            if(data_String == null)
            {
                endOfFile = true;
                break;
            }

            var data_values = data_String.Split(',');

            if(row == 2) // 속도와 쿨타임
            {
                varSpeed = float.Parse(data_values[11]);
                coolTime = float.Parse(data_values[12]);
            }
            else if(row == 3) // 하단부 바위
            {
                downStonePer = float.Parse(data_values[8]) * 100;
                downStoneStunTime = float.Parse(data_values[9]);
            }
            else if(row == 4) // 상단부 바위
            {
                upStonePer = float.Parse(data_values[8]) * 100;
                upStoneStunTime = float.Parse(data_values[9]);
            }
            else if(row == 5) // 보석
            {
                jemResource = float.Parse(data_values[7]);
                jemPer = float.Parse(data_values[8]) * 100;
            }
            
        }
    }
    // Start is called before the first frame update
    
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(BlockSpawnPoint ,0.1f);
    }
}
