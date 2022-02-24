using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OasisManager : MonoBehaviour
{
    public MiniGameMgr MGM;

    public GameObject GuidText, GuidPanel;
    public GameObject MeatPrefab, TreePrefab, WaterPrefab;
    private Vector2 TextTarget;

    public float widthScale;
    public float heightScale;
    private bool isFirst = true;

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
        MGM = GameObject.Find("MiniGameMgr").GetComponent<MiniGameMgr>();
        InGameMgr.Instance.EnterMiniGame("Stage_2_oasis");

        SetUp();
        StartCoroutine(FirstStart());
    }

    void SetUp()
    {
        widthScale = Screen.width / 1920f;
        heightScale = Screen.height / 1080f;

        meatNum = 0;
        waterNum = 0;
        treeNum = 0;

        oasisCharSpeed = InGameMgr.Instance.miniGameData["game2lay"].speed;

        oasisMeatRes = InGameMgr.Instance.miniGameData["game2meat"].meat;
        oasisMeatMaxNum = InGameMgr.Instance.miniGameData["game2meat"].value1;

        oasisWaterRes = InGameMgr.Instance.miniGameData["game2water"].water;
        oasisWaterMaxNum = InGameMgr.Instance.miniGameData["game2water"].value1;

        oasisTreeRes = InGameMgr.Instance.miniGameData["game2tree"].wood;
        oasisTreeMaxNum = InGameMgr.Instance.miniGameData["game2tree"].value1;

        oasisNetSpeed = InGameMgr.Instance.miniGameData["game2net"].speed;

        realOasisNetSpeed = standardOasisSpeed * oasisNetSpeed;
        realOasisCharSpeed = standardOasisSpeed * oasisCharSpeed;

        TextTarget = new Vector2(GuidText.GetComponent<RectTransform>().anchoredPosition.x, GuidText.GetComponent<RectTransform>().anchoredPosition.y + MGM.textPosition);
    }

    public void StartGame()
    {
        if(!isFirst)
            StartCoroutine(ReStart());
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
        MGM.water += (int)oasisWaterRes;
        waterNum--;
    }

    public void GetMeat()
    {
        MGM.meat += (int)oasisMeatRes;
        meatNum--;
    }

    public void GetTree()
    {
        MGM.wood += (int)oasisTreeRes;
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

        MGM.CloseCurrentGame();
        yield return null;
    }

    IEnumerator ReStart()
    {
        GuidPanel.SetActive(true);
        GuidText.GetComponent<RectTransform>().anchoredPosition = new Vector2(0,0);
        yield return new WaitForSeconds(MGM.firstTime);
        GuidPanel.SetActive(false);

        while (Vector2.Distance(GuidText.GetComponent<RectTransform>().anchoredPosition, TextTarget) >= 0.1f)
        {
            GuidText.GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(GuidText.GetComponent<RectTransform>().anchoredPosition, TextTarget, Time.deltaTime * MGM.textSpeed);

            yield return null;
        }

        StartCoroutine(OasisRoutine());
    }
    IEnumerator FirstStart()
    {
        
        GuidPanel.SetActive(true);
        GuidText.GetComponent<RectTransform>().anchoredPosition = new Vector2(0,0);
        
        yield return new WaitForSeconds(MGM.firstTime);
        GuidPanel.SetActive(false);
        isFirst = false;

        while (Vector2.Distance(GuidText.GetComponent<RectTransform>().anchoredPosition, TextTarget) >= 0.1f)
        {
            GuidText.GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(GuidText.GetComponent<RectTransform>().anchoredPosition, TextTarget, Time.deltaTime * MGM.textSpeed);

            yield return null;
        }

        StartCoroutine(OasisRoutine());
    }
}
