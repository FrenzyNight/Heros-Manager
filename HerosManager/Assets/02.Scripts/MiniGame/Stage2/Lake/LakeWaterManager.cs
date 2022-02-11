using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LakeWaterManager : MonoBehaviour
{
    public MiniGameMgr MGM;
    public GameObject GuidText, GuidPanel;
    private Vector2 TextTarget;
    public float widthScale;
    public float heightScale;
    private bool isFirst = true;

    //road
    public float standardLakeWaterSpeed = 100f;
    
    public float waterBucketSpeed;
    public float waterRock1Speed;
    public float waterRock2Speed;
    public float waterLevel1Res;
    public float waterLevel2Res;

    //real
    public float realBucketSpeed;
    public float realRock1Speed;
    public float realRock2Speed;

    //point
    public float rockMovePoint;

    // Start is called before the first frame update
    void Start()
    {
        MGM = GameObject.Find("MiniGameMgr").GetComponent<MiniGameMgr>();
        InGameMgr.Instance.EnterMiniGame("Stage_2_desertLake");

        SetUp();
        StartCoroutine(FirstStart());
    }

     void SetUp()
    {
        widthScale = Screen.width / 1920f;
        heightScale = Screen.height / 1080f;

        waterBucketSpeed = InGameMgr.Instance.miniGameData["game2bucket"].speed;

        waterRock1Speed = InGameMgr.Instance.miniGameData["game2rock1"].speed;
        waterRock2Speed = InGameMgr.Instance.miniGameData["game2rock2"].speed;

        waterLevel1Res = InGameMgr.Instance.miniGameData["game2level1"].water;
        waterLevel2Res = InGameMgr.Instance.miniGameData["game2level2"].water;

        realBucketSpeed = standardLakeWaterSpeed * waterBucketSpeed;
        realRock1Speed = standardLakeWaterSpeed * waterRock1Speed;
        realRock2Speed = standardLakeWaterSpeed * waterRock2Speed;

        TextTarget = new Vector2(GuidText.GetComponent<RectTransform>().anchoredPosition.x, GuidText.GetComponent<RectTransform>().anchoredPosition.y + MGM.textPosition);
    }

    public void GetWater(int lv)
    {
        if(lv == 1)
            MGM.water += (int)waterLevel1Res;
        else if(lv == 2)
            MGM.water += (int)waterLevel2Res;
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

        //StartCoroutine(SpawnFish());
    }
}
