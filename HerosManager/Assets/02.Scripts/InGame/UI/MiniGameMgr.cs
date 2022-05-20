using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class MiniGameMgr : Singleton<MiniGameMgr>
{
    public GameObject MiniGamePanelBack;
    public GameObject MiniGamePanel;
    public Button ExitBtn;

    public MiniGameInfo[] miniGames;
    public Transform[] minigamePlaceTrans;
    public Text[] GuideTexts;

    public Text woodText, waterText, meatText, hubText;

    int wood;
    int water;
    int meat;
    int hub;
    int food;

    IEnumerator cycleRandomGameCo;

    void Start()
    {
        ExitBtn.onClick.AddListener(CloseMiniGame);

    }

    public void Setup(string _code)
    {
        InGameMgr.Instance.state = State.MiniGame;
        MiniGamePanelBack.SetActive(true);
        MiniGamePanel.SetActive(true);
        
        wood = 0;
        water = 0;
        meat = 0;
        hub = 0;
        food = 0;

        CollectSpaceData collectSpaceData;
        collectSpaceData = LoadGameData.Instance.collectSpaceDatas[_code];

        for (int i = 0; i < miniGames.Length; i++)
        {
            miniGames[i].obj.SetActive(false);
        }
        for (int i = 0; i < GuideTexts.Length; i++)
        {
            GuideTexts[i].gameObject.SetActive(false);
        }

        MiniGameInfo miniGameInfo;
        if (collectSpaceData.MiniGameLeftID1 != "-1")
        {
            miniGameInfo = Array.Find(miniGames, x => x.code == collectSpaceData.MiniGameLeftID1);
            miniGameInfo.obj.SetActive(true);
            miniGameInfo.obj.transform.position = minigamePlaceTrans[0].position;

            miniGameInfo.obj.transform.GetChild(0).gameObject.GetComponent<MiniGameSetMgr>().SetUp();

            GuideTexts[0].gameObject.SetActive(true);
            GuideTexts[0].text = LoadGameData.Instance.GetString(collectSpaceData.MiniGameHelpStringID1);
            GuideTexts[0].transform.DOLocalMoveY(0, 1).From().SetEase(Ease.OutBack);
        }
        if (collectSpaceData.MiniGameRightID2 != "-1")
        {
            miniGameInfo = Array.Find(miniGames, x => x.code == collectSpaceData.MiniGameRightID2);
            miniGameInfo.obj.SetActive(true);
            miniGameInfo.obj.transform.position = minigamePlaceTrans[1].position;

            miniGameInfo.obj.transform.GetChild(0).gameObject.GetComponent<MiniGameSetMgr>().SetUp();

            GuideTexts[1].gameObject.SetActive(true);
            GuideTexts[1].text = LoadGameData.Instance.GetString(collectSpaceData.MiniGameHelpStringID2);
            GuideTexts[1].transform.DOLocalMoveY(0, 1).From().SetEase(Ease.OutBack);
        }
        /*
        if (collectSpaceData.MiniGameCenterID != "-1")
        {
            miniGameInfo = Array.Find(miniGames, x => x.code == collectSpaceData.MiniGameCenterID);
            miniGameInfo.obj.SetActive(true);
            miniGameInfo.obj.transform.position = minigamePlaceTrans[2].position;
            GuideTexts[2].gameObject.SetActive(true);
            GuideTexts[2].text = LoadGameData.Instance.GetString(collectSpaceData.MiniGameHelpStringID3);
            GuideTexts[2].transform.DOLocalMoveY(0, 1).From().SetEase(Ease.OutBack);
        }
        */

        if (collectSpaceData.FunctionID == "MG_S3_Rand_Portal")
        {
            if (cycleRandomGameCo != null)
            {
                StopCoroutine(cycleRandomGameCo);
            }
            cycleRandomGameCo = CycleRandomGameCo();
            StartCoroutine(cycleRandomGameCo);
        }

        SetItemText();
    }

    void SetItemText()
    {
        woodText.text = wood.ToString();
        waterText.text =  water.ToString();
        meatText.text = meat.ToString();
        hubText.text =  hub.ToString();
    }

    IEnumerator CycleRandomGameCo()
    {
        yield return null;

        List<CollectSpaceData> randomGameList = new List<CollectSpaceData>();
        foreach (var data in LoadGameData.Instance.collectSpaceDatas)
        {
            if (data.Value.MiniGameLeftID1 != "-1" && data.Value.MiniGameRightID2 != "-1")
            {
                randomGameList.Add(data.Value);
            }
        }

        ChangeRandomGame(randomGameList);
        float changeTime = LoadGameData.Instance.defineDatas[""].value;
        float timer = 0f;

        while (true)
        {
            timer += Time.deltaTime;
            if (timer >= changeTime)
            {
                ChangeRandomGame(randomGameList);
                timer = 0f;
            }

            yield return null;
        }
    }

    void ChangeRandomGame(List<CollectSpaceData> _randomGameList)
    {
        for (int i = 0; i < miniGames.Length; i++)
        {
            miniGames[i].obj.SetActive(false);
        }
        for (int i = 0; i < GuideTexts.Length; i++)
        {
            GuideTexts[i].gameObject.SetActive(false);
        }

        MiniGameInfo miniGameInfo;

        int rand = UnityEngine.Random.Range(0, _randomGameList.Count);
        miniGameInfo = Array.Find(miniGames, x => x.code == _randomGameList[rand].MiniGameLeftID1);
        miniGameInfo.obj.SetActive(true);
        miniGameInfo.obj.transform.position = minigamePlaceTrans[0].position;
        GuideTexts[0].gameObject.SetActive(true);
        GuideTexts[0].text = LoadGameData.Instance.GetString(_randomGameList[rand].MiniGameHelpStringID1);
        GuideTexts[0].transform.DOLocalMoveY(0, 1).From().SetEase(Ease.OutBack);

        rand = UnityEngine.Random.Range(0, _randomGameList.Count);
        miniGameInfo = Array.Find(miniGames, x => x.code == _randomGameList[rand].MiniGameRightID2);
        miniGameInfo.obj.SetActive(true);
        miniGameInfo.obj.transform.position = minigamePlaceTrans[1].position;
        GuideTexts[1].gameObject.SetActive(true);
        GuideTexts[1].text = LoadGameData.Instance.GetString(_randomGameList[rand].MiniGameHelpStringID2);
        GuideTexts[1].transform.DOLocalMoveY(0, 1).From().SetEase(Ease.OutBack);
    }

    public void AddTempItem(string _item1Code, int _amount1)
    {
        switch (_item1Code)
        {
            case "Item_Wood":
                wood += _amount1;
                break;
            case "Item_Water":
                water += _amount1;
                break;
            case "Item_Meat":
                meat += _amount1;
                break;
            case "Item_Hub":
                hub += _amount1;
                break;
            case "Item_Food":
                food += _amount1;
                break;
        }

        SetItemText();

/*
        switch (_item2Code)
        {
            case "Item_Wood":
                wood += _amount2;
                break;
            case "Item_Water":
                water += _amount2;
                break;
            case "Item_Meat":
                meat += _amount2;
                break;
            case "Item_Hub":
                hub += _amount2;
                break;
            case "Item_Food":
                food += _amount2;
                break;
        }
        */
    }

    public void CloseMiniGame()
    {
        InGameMgr.Instance.state = State.Camp;
        MiniGamePanelBack.SetActive(false);
        MiniGamePanel.SetActive(false);

        ItemManager.Instance.AddItem(wood, water, meat, hub, food);

        if (cycleRandomGameCo != null)
        {
            StopCoroutine(cycleRandomGameCo);
        }
    }
}

[System.Serializable]
public class MiniGameInfo
{
    public string code;
    public GameObject obj;
}