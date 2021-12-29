using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class CaveManager : MonoBehaviour
{
    public GameObject FCSVR;
    private ForestCSVReader ForestCSV;
    public GameObject JemPrefab, DownStonePrefab, UpStonePrefab;
    public Vector2 BlockSpawnPoint;

    public bool isRun;

    private float standardSpeed = 5f;
    private float standardJump = 10f;

    private int rnd;

    public float realSpeed;
    public float realStunTime;
    public float realCharJump;

    // Start is called before the first frame update
    void Start()
    {
        ForestCSV = FCSVR.GetComponent<ForestCSVReader>();
    
        isRun = true;
        Debug.Log("jemper : " + ForestCSV.jemPer.ToString());
        realSpeed = standardSpeed * ForestCSV.varSpeed;
        realStunTime = ForestCSV.upStoneStunTime;
        realCharJump =standardJump * ForestCSV.charJump;
        StartCoroutine("SpawnObject");
    }
    
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

                if(rnd <= ForestCSV.jemPer)
                {
                    //JemSpawn
                    Instantiate(JemPrefab, BlockSpawnPoint, Quaternion.identity);
                }
                else if(ForestCSV.jemPer < rnd && rnd <= ForestCSV.jemPer + ForestCSV.downStonePer)
                {
                    // DownStoneSpawn
                    Instantiate(DownStonePrefab, BlockSpawnPoint, Quaternion.identity);
                }
                else if(ForestCSV.jemPer + ForestCSV.downStonePer < rnd && rnd <= ForestCSV.jemPer + ForestCSV.downStonePer + ForestCSV.upStonePer)
                {
                    //UpStoneSpawn;
                    Instantiate(UpStonePrefab, new Vector2(BlockSpawnPoint.x, BlockSpawnPoint.y + 2f), Quaternion.identity);
                }
                else
                {
                    //Debug.Log("rnd : " + rnd.ToString());
                }
            }

            yield return new WaitForSeconds(ForestCSV.coolTime);
        }
    }


    
    
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(BlockSpawnPoint ,0.1f);
    }
}
