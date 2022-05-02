using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class CaveManager : MiniGameSetMgr
{
    //public GameObject GuidText, GuidPanel;
    //private Vector2 TextTarget;
    
    public GameObject JemPrefab, DownStonePrefab, UpStonePrefab;
    public GameObject Char;
    private float standardSpeed = 500f;
    private float standardJump = 1700f;

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
        SetUp();
    }

    public override void SetUp()
    {
        base.SetUp();

        
        isCheck = true;

        DownStonePrefab.GetComponent<MiniGameObjectMgr>().manager = gameObject;
        UpStonePrefab.GetComponent<MiniGameObjectMgr>().manager = gameObject;
        JemPrefab.GetComponent<MiniGameObjectMgr>().manager = gameObject;

        varSpeed = LoadGameData.Instance.miniGameDatas["game1cage_normal"].Velocity;
        coolTime = LoadGameData.Instance.miniGameDatas["game1cage_normal"].CoolTime;
        stunTime = LoadGameData.Instance.miniGameDatas["game1cage_stone"].Stun;
        downStonePer = LoadGameData.Instance.miniGameDatas["game1cage_stone"].Probability * 100;
        upStonePer = LoadGameData.Instance.miniGameDatas["game1cage_upstone"].Probability * 100;
        jemPer = LoadGameData.Instance.miniGameDatas["game1cage_jem"].Probability * 100;
        jemResource = LoadGameData.Instance.miniGameDatas["game1cage_jem"].GetJemAmount;
        charJump = LoadGameData.Instance.miniGameDatas["game1cage_normal"].value1;

        realCharJump = charJump * standardJump;
        realSpeed = varSpeed * standardSpeed;

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
        Char.GetComponent<CaveCharMove>().Reset();

        StartCoroutine(SpawnObject());
    }
    

    IEnumerator SpawnObject()
    {
        while(true)
        {
            if(isCheck)
            {
                rnd = Random.Range(1,101);

                if(rnd <= jemPer)
                {
                    //JemSpawn
                    spawnObjects.Add(Instantiate(JemPrefab, RealSpawnPoint, Quaternion.identity,mother.transform));

                }
                else if(jemPer < rnd && rnd <= jemPer + downStonePer)
                {
                    // DownStoneSpawn
                    spawnObjects.Add(Instantiate(DownStonePrefab, RealSpawnPoint, Quaternion.identity, mother.transform));
                }
                else if(jemPer + downStonePer < rnd && rnd <= jemPer + downStonePer + upStonePer)
                {
                    //UpStoneSpawn;
                    spawnObjects.Add(Instantiate(UpStonePrefab, RealSpawnPoint, Quaternion.identity, mother.transform));
                }
                else
                {
                    //Debug.Log("rnd : " + rnd.ToString());
                }
            }

            yield return new WaitForSeconds(coolTime);
        }
    }
}
