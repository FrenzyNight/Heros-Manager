using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OasisManager : MiniGameSetMgr
{
    public GameObject MeatPrefab, TreePrefab, WaterPrefab;

    private float standardOasisSpeed = 72f;
    //road
    public float oasisCharSpeed;

    public float oasisMeatRes;
    public float oasisWaterRes;
    public float oasisTreeRes;
    public float oasisNetSpeed;

    public float oasisMeatMaxNum;
    public float oasisWaterMaxNum;
    public float oasisTreeMaxNum;

    //real
    public float realOasisNetSpeed;
    public float realOasisCharSpeed;

    private float meatNum, waterNum, treeNum;

    // Start is called before the first frame update
    void Start()
    {
        if (!IsPrepared)
            SetUp();
    }

    public override void SetUp()
    {
        base.SetUp();

        meatNum = 0;
        waterNum = 0;
        treeNum = 0;

        oasisCharSpeed = GameData("game2lay").Velocity;

        oasisMeatRes = GameData("game2meat").GetAmount1;
        oasisMeatMaxNum = GameData("game2meat").value1;

        oasisWaterRes = GameData("game2water").GetAmount1;
        oasisWaterMaxNum = GameData("game2water").value1;

        oasisTreeRes = GameData("game2tree").GetAmount1;
        oasisTreeMaxNum = GameData("game2tree").value1;

        oasisNetSpeed = GameData("game2net").Velocity;

        realOasisNetSpeed = standardOasisSpeed * oasisNetSpeed;
        realOasisCharSpeed = standardOasisSpeed * oasisCharSpeed;
        StartGame();
    }

    public override void StartGame()
    {
        base.StartGame();
        meatNum = waterNum = treeNum = 0;
        StartCoroutine(OasisRoutine());
    }

    void SpawnObject()
    {
        GameObject obj;
        int rnd;
        int meatToSpawn = MiniGameRules.RequiredSpawnCount(meatNum, oasisMeatMaxNum);
        int waterToSpawn = MiniGameRules.RequiredSpawnCount(waterNum, oasisWaterMaxNum);
        int treesToSpawn = MiniGameRules.RequiredSpawnCount(treeNum, oasisTreeMaxNum);
        for (int i = 0; i < meatToSpawn; i++)
        {
            rnd = Random.Range(0,360);
            obj = Spawn(MeatPrefab, transform.position, Quaternion.identity, SpawnContainer("Oasis"));
            obj.GetComponent<RectTransform>().Rotate(new Vector3(0,0,rnd));
            meatNum++;
        }

        for (int i = 0; i < waterToSpawn; i++)
        {
            rnd = Random.Range(0,360);
            obj = Spawn(WaterPrefab, transform.position, Quaternion.identity, SpawnContainer("Oasis"));
            obj.GetComponent<RectTransform>().Rotate(new Vector3(0,0,rnd));
            waterNum++;
        }

        for (int i = 0; i < treesToSpawn; i++)
        {
            rnd = Random.Range(0,360);
            obj = Spawn(TreePrefab, transform.position, Quaternion.identity, SpawnContainer("Oasis"));
            obj.GetComponent<RectTransform>().Rotate(new Vector3(0,0,rnd));
            treeNum++;
        }
    }

    public void GetWater()
    {
        AddItem(GameData("game2water").GetItemID1, (int)oasisWaterRes);
        waterNum = Mathf.Max(0f, waterNum - 1f);
    }

    public void GetMeat()
    {
        AddItem(GameData("game2meat").GetItemID1, (int)oasisMeatRes);
        meatNum = Mathf.Max(0f, meatNum - 1f);
    }

    public void GetTree()
    {
        AddItem(GameData("game2tree").GetItemID1, (int)oasisTreeRes);
        treeNum = Mathf.Max(0f, treeNum - 1f);
    }
    
    IEnumerator OasisRoutine()
    {
        while (true)
        {
            SpawnObject();
            yield return new WaitForSeconds(MiniGameRules.PositiveDelay(oasisCharSpeed * 5f));
        }
    }

    
}
