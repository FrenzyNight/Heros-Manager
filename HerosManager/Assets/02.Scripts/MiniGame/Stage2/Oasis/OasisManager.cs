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
        SetUp();
    }

    void SetUp()
    {

        meatNum = 0;
        waterNum = 0;
        treeNum = 0;

        oasisCharSpeed = LoadGameData.Instance.miniGameDatas["game2lay"].Velocity;

        oasisMeatRes = LoadGameData.Instance.miniGameDatas["game2meat"].GetAmount1;
        oasisMeatMaxNum = LoadGameData.Instance.miniGameDatas["game2meat"].value1;

        oasisWaterRes = LoadGameData.Instance.miniGameDatas["game2water"].GetAmount1;
        oasisWaterMaxNum = LoadGameData.Instance.miniGameDatas["game2water"].value1;

        oasisTreeRes = LoadGameData.Instance.miniGameDatas["game2tree"].GetAmount1;
        oasisTreeMaxNum = LoadGameData.Instance.miniGameDatas["game2tree"].value1;

        oasisNetSpeed = LoadGameData.Instance.miniGameDatas["game2net"].Velocity;

        realOasisNetSpeed = standardOasisSpeed * oasisNetSpeed;
        realOasisCharSpeed = standardOasisSpeed * oasisCharSpeed;

       
    }

    public void StartGame()
    {
    }

    void SpawnObject()
    {
        GameObject obj;
        int rnd;
        while(meatNum < oasisMeatMaxNum)
        {
            rnd = Random.Range(0,360);
            obj = Instantiate(MeatPrefab, transform.position, Quaternion.identity, GameObject.Find("Oasis").transform);
            obj.GetComponent<RectTransform>().Rotate(new Vector3(0,0,rnd));
            meatNum++;
        }

        while(waterNum < oasisWaterMaxNum)
        {
            rnd = Random.Range(0,360);
            obj = Instantiate(WaterPrefab, transform.position, Quaternion.identity, GameObject.Find("Oasis").transform);
            obj.GetComponent<RectTransform>().Rotate(new Vector3(0,0,rnd));
            waterNum++;
        }

        while(treeNum < oasisTreeMaxNum)
        {
            rnd = Random.Range(0,360);
            obj = Instantiate(TreePrefab, transform.position, Quaternion.identity, GameObject.Find("Oasis").transform);
            obj.GetComponent<RectTransform>().Rotate(new Vector3(0,0,rnd));
            treeNum++;
        }
    }

    public void GetWater()
    {
        //MGM.water += (int)oasisWaterRes;
        waterNum--;
    }

    public void GetMeat()
    {
        //MGM.meat += (int)oasisMeatRes;
        meatNum--;
    }

    public void GetTree()
    {
        //MGM.wood += (int)oasisTreeRes;
        treeNum--;
    }
    
    IEnumerator OasisRoutine()
    {
        SpawnObject();
        yield return new WaitForSeconds(oasisCharSpeed * 5f);
        SpawnObject();
        yield return new WaitForSeconds(oasisCharSpeed * 5f);
        SpawnObject();
        yield return new WaitForSeconds(oasisCharSpeed * 5f);

        //MGM.CloseCurrentGame();
        yield return null;
    }

    
}
