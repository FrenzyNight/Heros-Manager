using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicSpringManager : MonoBehaviour
{
    public MiniGameMgr MGM;

    public GameObject GuidText, GuidPanel;
    public GameObject Command;
    public GameObject[] Altars;
    private Vector2 TextTarget;
    
    private float widthScale;
    private float heightScale;
    private bool isFirst = true;

    //road

    public float msSpringRes;
    public float msSpringWaterTime;
    public float msSpringGraceTime;
    public float msSpringJewelTime;

    public float msAlterMinTime;
    public float msAlterMaxTime;

    public float msKeyPadNum;
    public float msKeyPadStun;

    //check
    public bool[] isAltar;
    public bool isSpring;

    // Start is called before the first frame update
    void Start()
    {
        MGM = GameObject.Find("MiniGameMgr").GetComponent<MiniGameMgr>();
        InGameMgr.Instance.EnterMiniGame("Stage_3_Magicspring");

        SetUp();
        StartCoroutine(FirstStart());
    }

    void SetUp()
    {
        widthScale = Screen.width / 1920f;
        heightScale = Screen.height / 1080f;

        msSpringRes = InGameMgr.Instance.miniGameData["game3MagicWell"].water;
        msSpringWaterTime = InGameMgr.Instance.miniGameData["game3MagicWell"].value1;
        msSpringGraceTime = InGameMgr.Instance.miniGameData["game3MagicWell"].value3;
        msSpringJewelTime = InGameMgr.Instance.miniGameData["game3MagicWell"].value2;

        msAlterMinTime = InGameMgr.Instance.miniGameData["game3Altar1"].value2;
        msAlterMaxTime = InGameMgr.Instance.miniGameData["game3Altar1"].value3;

        msKeyPadNum = InGameMgr.Instance.miniGameData["game3KeyPad"].value3;
        msKeyPadStun = InGameMgr.Instance.miniGameData["game3KeyPad"].stun;

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

    public void GetWater()
    {
        MGM.water += (int)msSpringRes;
    }

    public void CommandActive(int n)
    {
        Command.SetActive(true);
        Command.GetComponent<MagicSpringCommand>().SetUp(n);
    }

    public void AltarActive(int n)
    {
        isAltar[n] = true;
        Altars[n].GetComponent<MagicSpringAltar>().Active();
    }
}
