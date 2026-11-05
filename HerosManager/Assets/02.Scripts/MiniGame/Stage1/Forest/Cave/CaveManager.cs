using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class CaveManager : MiniGameSetMgr
{
    //public GameObject GuidText, GuidPanel;
    //private Vector2 TextTarget;
    [Header("Cave Mgr")]
    public GameObject JemPrefab, DownStonePrefab, UpStonePrefab;
    public GameObject Char;
    private float standardSpeed = 500f;
    private float standardJump = 1700f;

    [HideInInspector]public float realSpeed;
    [HideInInspector]public float realCharJump;

    //road data
    [HideInInspector]public float varSpeed;
    [HideInInspector]public float coolTime;
    [HideInInspector]public float stunTime;
    [HideInInspector]public float downStonePer;
    [HideInInspector]public float upStonePer;
    
    [HideInInspector]public float jemResource;
    [HideInInspector]public float jemPer;

    [HideInInspector]public float charJump;
    
    // Start is called before the first frame update
    void Start()
    {
        //SetUp();
    }

    public override void SetUp()
    {
        base.SetUp();

        
        isCheck = true;


        varSpeed = GameData("game1cage_normal").Velocity;
        coolTime = GameData("game1cage_normal").CoolTime;
        stunTime = GameData("game1cage_stone").Stun;
        downStonePer = GameData("game1cage_stone").Probability * 100;
        upStonePer = GameData("game1cage_upstone").Probability * 100;
        jemPer = GameData("game1cage_jem").Probability * 100;
        jemResource = GameData("game1cage_jem").GetJemAmount;
        charJump = GameData("game1cage_normal").value1;

        realCharJump = charJump * standardJump;
        realSpeed = varSpeed * standardSpeed;

        StartGame();
    }

    public override void StartGame()
    {
        base.StartGame();

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

                if(MiniGameRules.PercentHit(rnd, jemPer))
                {
                    //JemSpawn
                    Spawn(JemPrefab, RealSpawnPoint, Quaternion.identity);

                }
                else if(jemPer < rnd && MiniGameRules.PercentHit(rnd, jemPer + downStonePer))
                {
                    // DownStoneSpawn
                    Spawn(DownStonePrefab, RealSpawnPoint, Quaternion.identity);
                }
                else if(jemPer + downStonePer < rnd && MiniGameRules.PercentHit(rnd, jemPer + downStonePer + upStonePer))
                {
                    //UpStoneSpawn;
                    Spawn(UpStonePrefab, RealSpawnPoint, Quaternion.identity);
                }
                else
                {
                    //Debug.Log("rnd : " + rnd.ToString());
                }
            }

            yield return new WaitForSeconds(MiniGameRules.PositiveDelay(coolTime));
        }
    }
}
