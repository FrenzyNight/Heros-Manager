using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GatheringManager : Singleton<GatheringManager>
{
    public Button GatheringBtn;
    public Button ExitBtn;

    public GameObject GatheringPanel;
    public Text NoticeText;

    public GatheringObject[] gatheringObjs;
    public Transform[] gatheringPlaceTrans;

    IEnumerator gathering4Co;

    void Start()
    {
        InGameMgr.Instance.NextStageAct += LoadStageGathering;

        GatheringBtn.onClick.AddListener(Setup);
        ExitBtn.onClick.AddListener(CloseGatheringPannel);
        for (int i = 0; i < gatheringObjs.Length; i++)
        {
            gatheringObjs[i].obj.GetComponent<Button>().onClick.AddListener(() =>
            {
                StartMiniGame(gatheringObjs[i].code);
            });
        }

        NoticeText.text = LoadGameData.Instance.GetString("");

        LoadStageGathering();
    }

    void LoadStageGathering()
    {
        CollectData collectData;
        collectData = LoadGameData.Instance.collectDatas[InGameMgr.Instance.stageData.CollectID];

        for (int i = 0; i < gatheringObjs.Length; i++)
        {
            gatheringObjs[i].obj.SetActive(false);
        }

        GatheringObject gatherObj;
        if (collectData.CollectSpaceID1 != "-1")
        {
            gatherObj = Array.Find(gatheringObjs, x => x.code == collectData.CollectSpaceID1);
            gatherObj.obj.SetActive(true);
            gatherObj.obj.transform.position = gatheringPlaceTrans[0].position;
            gatherObj.obj.GetComponentInChildren<Text>().text = LoadGameData.Instance.GetString(collectData.StringID1);
        }
        if (collectData.CollectSpaceID2 != "-1")
        {
            gatherObj = Array.Find(gatheringObjs, x => x.code == collectData.CollectSpaceID2);
            gatherObj.obj.SetActive(true);
            gatherObj.obj.transform.position = gatheringPlaceTrans[1].position;
            gatherObj.obj.GetComponentInChildren<Text>().text = LoadGameData.Instance.GetString(collectData.StringID2);
        }
        if (collectData.CollectSpaceID3 != "-1")
        {
            gatherObj = Array.Find(gatheringObjs, x => x.code == collectData.CollectSpaceID3);
            gatherObj.obj.SetActive(true);
            gatherObj.obj.transform.position = gatheringPlaceTrans[2].position;
            gatherObj.obj.GetComponentInChildren<Text>().text = LoadGameData.Instance.GetString(collectData.StringID3);
        }

        if (collectData.RanPos == 0)
        {
            if (collectData.CollectSpaceID4 != "-1")
            {
                gatherObj = Array.Find(gatheringObjs, x => x.code == collectData.CollectSpaceID4);
                gatherObj.obj.SetActive(true);
                gatherObj.obj.transform.position = gatheringPlaceTrans[3].position;
                gatherObj.obj.GetComponentInChildren<Text>().text = LoadGameData.Instance.GetString(collectData.StringID4);
            }
        }
        else
        {
            if (gathering4Co != null)
            {
                StopCoroutine(gathering4Co);
            }
            gathering4Co = Gathering4Co(collectData);
            StartCoroutine(gathering4Co);
        }
    }

    IEnumerator Gathering4Co(CollectData _collectData)
    {
        float rand = UnityEngine.Random.Range(_collectData.StartTime, _collectData.EndTime);

        yield return new WaitForSeconds(rand);

        GatheringObject gatherObj;
        gatherObj = Array.Find(gatheringObjs, x => x.code == _collectData.CollectSpaceID4);
        gatherObj.obj.SetActive(true);
        gatherObj.obj.transform.position = gatheringPlaceTrans[3].position;
        gatherObj.obj.GetComponentInChildren<Text>().text = LoadGameData.Instance.GetString(_collectData.StringID4);
        Notice.Instance.InstNoticeText("--");

        yield return new WaitForSeconds(_collectData.ContinueTime);

        gatherObj.obj.SetActive(false);
    }

    void Setup()
    {
        InGameMgr.Instance.state = State.Gathering;
        GatheringPanel.SetActive(true);
    }

    void StartMiniGame(string _code)
    {
        CloseGatheringPannel();

        MiniGameMgr.Instance.Setup(_code);
    }

    public void CloseGatheringPannel()
    {
        InGameMgr.Instance.state = State.Camp;
        GatheringPanel.SetActive(false);
    }
}

[System.Serializable]
public class GatheringObject
{
    public string code;
    public GameObject obj;
}