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

    private float standardSpeed = 150f;
    private float standardJump = 450f;

    private int rnd;

    
    public float realSpeed;
    public float realCharJump;

    //road data
    public float varSpeed;
    public float coolTime;

    public float stunTime;
    public float downStonePer;
    public float upStonePer;
    
    public float jemResource;
    public float jemPer;

    public float charJump;
    

    // Start is called before the first frame update
    void Start()
    {
        isRun = true;
        //LoadGameData.Instance.LoadCSVDatas();
        InGameMgr.Instance.EnterMiniGame("Stage_1_Forest");
        Debug.Log("Test : " + InGameMgr.Instance.miniGameData["game1cage_normal"].speed);
        
        SetUp();

        StartCoroutine("SpawnObject");
    }
    
    void SetUp()
    {
        varSpeed = InGameMgr.Instance.miniGameData["game1cage_normal"].speed;
        coolTime = InGameMgr.Instance.miniGameData["game1cage_normal"].cooltime;
        stunTime = InGameMgr.Instance.miniGameData["game1cage_stone"].stun;
        downStonePer = InGameMgr.Instance.miniGameData["game1cage_stone"].probability * 100;
        upStonePer = InGameMgr.Instance.miniGameData["game1cage_upstone"].probability * 100;
        jemPer = InGameMgr.Instance.miniGameData["game1cage_jem"].probability * 100;
        jemResource = InGameMgr.Instance.miniGameData["game1cage_jem"].jem;
        charJump = InGameMgr.Instance.miniGameData["game1cage_normal"].value1;

        realCharJump = charJump * standardJump;
        realSpeed = varSpeed * standardSpeed;
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

                if(rnd <= jemPer)
                {
                    //JemSpawn
                    Instantiate(JemPrefab, BlockSpawnPoint, Quaternion.identity,GameObject.Find("Cave").transform);
                }
                else if(jemPer < rnd && rnd <= jemPer + downStonePer)
                {
                    // DownStoneSpawn
                    Instantiate(DownStonePrefab, BlockSpawnPoint, Quaternion.identity, GameObject.Find("Cave").transform);
                }
                else if(jemPer + downStonePer < rnd && rnd <= jemPer + downStonePer + upStonePer)
                {
                    //UpStoneSpawn;
                    Instantiate(UpStonePrefab, new Vector2(BlockSpawnPoint.x, BlockSpawnPoint.y + 55f), Quaternion.identity, GameObject.Find("Cave").transform);
                }
                else
                {
                    //Debug.Log("rnd : " + rnd.ToString());
                }
            }

            yield return new WaitForSeconds(coolTime);
        }
    }

    
    
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(BlockSpawnPoint ,5f);
    }
}
