using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FairyWarehouseManager : MonoBehaviour
{
    //기본
    public MiniGameMgr MGM;

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

    public float fwTreasure2ResMeat;
    public float fwTreasure2ResHub;
    public float fwTreasure2CoolTime;

    public float fwTreasure3ResMeat;
    public float fwTreasure3ResHub;
    public float fwTreasure3CoolTime;

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

    // Start is called before the first frame update
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

        fwTreasure2ResMeat= InGameMgr.Instance.miniGameData["game3treasure2"].meat;
        fwTreasure2ResHub= InGameMgr.Instance.miniGameData["game3treasure2"].hub;
        fwTreasure2CoolTime= InGameMgr.Instance.miniGameData["game3treasure2"].cooltime;

        fwTreasure3ResMeat= InGameMgr.Instance.miniGameData["game3treasure3"].meat;
        fwTreasure3ResHub= InGameMgr.Instance.miniGameData["game3treasure3"].hub;
        fwTreasure3CoolTime= InGameMgr.Instance.miniGameData["game3treasure3"].cooltime;

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
            
        TextTarget = new Vector2(GuidText.GetComponent<RectTransform>().anchoredPosition.x, GuidText.GetComponent<RectTransform>().anchoredPosition.y + MGM.textPosition);
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
