using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FairyWarehouseManager : MonoBehaviour
{
    //기본
    public MiniGameMgr MGM;
    public GameObject Treasure1Prefab, Treasure2Prefab, Treasure3Prefab;
    public Vector2 tp1, tp2, tp3;

    public GameObject GuidText, GuidPanel;
    private Vector2 TextTarget;
    
    private float widthScale;
    private float heightScale;
    private bool isFirst = true;

    //standard
    public float fwStandardSpeed;

    //road
    public float fwTreasure1ResMeat;
    public float fwTreasure1ResHub;
    public float fwTreasure1CoolTime;
    public float fwTreasure1GetTime;

    public float fwTreasure2ResMeat;
    public float fwTreasure2ResHub;
    public float fwTreasure2CoolTime;
    public float fwTreasure2GetTime;

    public float fwTreasure3ResMeat;
    public float fwTreasure3ResHub;
    public float fwTreasure3CoolTime;
    public float fwTreasure3GetTime;

    public float fwLaySpeed;

    public float fwFairy1Speed;

    public float fwFairy2Speed;

    public float fwFairy3Speed;

    public float fwFairyLeftTime;
    public float fwFairyRightTime;
    public float fwFairyFrontTime;

    //real

    public float fwLayRealSpeed;
    public float fwFairy1RealSpeed;
    public float fwFairy2RealSpeed;
    public float fwFairy3RealSpeed;

    //treasure check
    private bool isTreasure1, isTreasure2, isTreasure3;
    void Start()
    {
        MGM = GameObject.Find("MiniGameMgr").GetComponent<MiniGameMgr>();
        InGameMgr.Instance.EnterMiniGame("Stage_3_fairyWarehouse");

        SetUp();
        StartCoroutine(FirstStart());
    }

    void SetUp()
    {
        widthScale = Screen.width / 1920f;
        heightScale = Screen.height / 1080f;
        
        fwTreasure1ResMeat = InGameMgr.Instance.miniGameData["game3treasure1"].meat;
        fwTreasure1ResHub = InGameMgr.Instance.miniGameData["game3treasure1"].hub;
        fwTreasure1CoolTime = InGameMgr.Instance.miniGameData["game3treasure1"].cooltime;
        fwTreasure1GetTime = InGameMgr.Instance.miniGameData["game3treasure1"].value1;

        fwTreasure2ResMeat= InGameMgr.Instance.miniGameData["game3treasure2"].meat;
        fwTreasure2ResHub= InGameMgr.Instance.miniGameData["game3treasure2"].hub;
        fwTreasure2CoolTime= InGameMgr.Instance.miniGameData["game3treasure2"].cooltime;
        fwTreasure2GetTime = InGameMgr.Instance.miniGameData["game3treasure2"].value1;

        fwTreasure3ResMeat= InGameMgr.Instance.miniGameData["game3treasure3"].meat;
        fwTreasure3ResHub= InGameMgr.Instance.miniGameData["game3treasure3"].hub;
        fwTreasure3CoolTime= InGameMgr.Instance.miniGameData["game3treasure3"].cooltime;
        fwTreasure3GetTime = InGameMgr.Instance.miniGameData["game3treasure3"].value1;

        fwLaySpeed= InGameMgr.Instance.miniGameData["game3Lay"].speed;

        fwFairy1Speed= InGameMgr.Instance.miniGameData["game3fairy1"].speed;

        fwFairy2Speed= InGameMgr.Instance.miniGameData["game3fairy2"].speed;

        fwFairy3Speed= InGameMgr.Instance.miniGameData["game3fairy3"].speed;

        fwFairyLeftTime= InGameMgr.Instance.miniGameData["game3fairy1"].value1;
        fwFairyRightTime= InGameMgr.Instance.miniGameData["game3fairy1"].value2;
        fwFairyFrontTime= InGameMgr.Instance.miniGameData["game3fairy1"].value3;

        //real

        fwLayRealSpeed = fwStandardSpeed * fwLaySpeed;
        fwFairy1RealSpeed = fwStandardSpeed * fwFairy1Speed;
        fwFairy2RealSpeed = fwStandardSpeed * fwFairy2Speed;;
        fwFairy3RealSpeed = fwStandardSpeed * fwFairy3Speed;

        isTreasure1 = true;
        isTreasure2 = true;
        isTreasure3 = true;

        Instantiate(Treasure1Prefab, new Vector2(tp1.x * widthScale, tp1.y * heightScale) , Quaternion.identity,GameObject.Find("FairyWarehouse").transform);
        Instantiate(Treasure2Prefab, new Vector2(tp2.x * widthScale, tp2.y * heightScale) , Quaternion.identity,GameObject.Find("FairyWarehouse").transform);
        Instantiate(Treasure3Prefab, new Vector2(tp3.x * widthScale, tp3.y * heightScale) , Quaternion.identity,GameObject.Find("FairyWarehouse").transform);
            
        TextTarget = new Vector2(GuidText.GetComponent<RectTransform>().anchoredPosition.x, GuidText.GetComponent<RectTransform>().anchoredPosition.y + MGM.textPosition);
    }

    void Update()
    {
        if(!isTreasure1)
        {
            StartCoroutine(SpawnTreasure(1));
        }
        else if(!isTreasure2)
        {
            StartCoroutine(SpawnTreasure(2));
        }
        else if(!isTreasure3)
        {
            StartCoroutine(SpawnTreasure(3));
        }
    }

    public void GetTreasure(int boxNum)
    {
        if(boxNum == 1)
        {
            MGM.meat += (int)fwTreasure1ResMeat;
            MGM.hub += (int)fwTreasure1ResHub;
            isTreasure1 = false;
        }
        else if(boxNum == 2)
        {
            MGM.meat += (int)fwTreasure2ResMeat;
            MGM.hub += (int)fwTreasure2ResHub;
            isTreasure2 = false;
        }
        else if(boxNum == 3)
        {
            MGM.meat += (int)fwTreasure3ResMeat;
            MGM.hub += (int)fwTreasure3ResHub;
            isTreasure3 = false;
        }
    }

    IEnumerator SpawnTreasure(int boxNum)
    {
        if(boxNum == 1)
        {
            isTreasure1 = true;
            yield return new WaitForSeconds(fwTreasure1CoolTime);
            Instantiate(Treasure1Prefab, new Vector2(tp1.x * widthScale, tp1.y * heightScale) , Quaternion.identity,GameObject.Find("FairyWarehouse").transform);
        }
        else if(boxNum == 2)
        {
            isTreasure2 = true;
            yield return new WaitForSeconds(fwTreasure2CoolTime);
            Instantiate(Treasure2Prefab, new Vector2(tp2.x * widthScale, tp2.y * heightScale) , Quaternion.identity,GameObject.Find("FairyWarehouse").transform);
        }
        else if(boxNum == 3)
        {
            isTreasure3 = true;
            yield return new WaitForSeconds(fwTreasure3CoolTime);
            Instantiate(Treasure3Prefab, new Vector2(tp3.x * widthScale, tp3.y * heightScale) , Quaternion.identity,GameObject.Find("FairyWarehouse").transform);
        }
    }

    public void StartGame()
    {
        if(!isFirst)
            StartCoroutine(ReStart());
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
    }
}
