using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
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
    InGameMgr stageOwner;
    readonly List<KeyValuePair<Button, UnityAction>> areaListeners = new List<KeyValuePair<Button, UnityAction>>();
    [HideInInspector] public bool isFirst;
    [HideInInspector] public bool isMini;

    void Start()
    {
        stageOwner = InGameMgr.Instance;
        stageOwner.NextStageAct += LoadStageGathering;

        isFirst = true;
        isMini = false;
        
        GatheringBtn.onClick.AddListener(Setup);
        ExitBtn.onClick.AddListener(CloseGatheringPannel);
        for (int i = 0; i < gatheringObjs.Length; i++)
        {
            int idx = i;
            Button button = gatheringObjs[i].obj.GetComponent<Button>();
            UnityAction listener = () =>
            {
                StartMiniGame(gatheringObjs[idx].code);
            };
            button.onClick.AddListener(listener);
            areaListeners.Add(new KeyValuePair<Button, UnityAction>(button, listener));
        }

        NoticeText.text = LoadGameData.Instance.GetString("MG_a1");

        LoadStageGathering();
    }

    void LoadStageGathering()
    {
        if (gathering4Co != null)
        {
            StopCoroutine(gathering4Co);
            gathering4Co = null;
        }
        CollectData collectData;
        collectData = LoadGameData.Instance.collectDatas[InGameMgr.Instance.stageData.CollectID];

        for (int i = 0; i < gatheringObjs.Length; i++)
        {
            gatheringObjs[i].obj.SetActive(false);
        }

        ShowArea(collectData.CollectSpaceID1, collectData.StringID1, 0);
        ShowArea(collectData.CollectSpaceID2, collectData.StringID2, 1);
        ShowArea(collectData.CollectSpaceID3, collectData.StringID3, 2);

        if (collectData.RanPos == 0)
        {
            ShowArea(collectData.CollectSpaceID4, collectData.StringID4, 3);
        }
        else
        {
            gathering4Co = Gathering4Co(collectData);
            StartCoroutine(gathering4Co);
        }
    }

    IEnumerator Gathering4Co(CollectData _collectData)
    {
        float rand = UnityEngine.Random.Range(_collectData.StartTime, _collectData.EndTime);

        yield return new WaitForSeconds(rand);

        var gatherObj = ShowArea(_collectData.CollectSpaceID4, _collectData.StringID4, 3);
        if (gatherObj == null)
            yield break;
        Notice.Instance.InstNoticeText(LoadGameData.Instance.GetString(_collectData.StringID4));

        yield return new WaitForSeconds(_collectData.ContinueTime);

        gatherObj.obj.SetActive(false);
        gathering4Co = null;
    }

    GatheringObject ShowArea(string code, string label, int slot)
    {
        if (string.IsNullOrEmpty(code) || code == "-1")
            return null;
        var area = Array.Find(gatheringObjs, entry => entry != null && entry.code == code);
        if (area == null || area.obj == null || slot >= gatheringPlaceTrans.Length || gatheringPlaceTrans[slot] == null)
        {
            Debug.LogWarning($"Collecting area {code} has no configured object or slot {slot}.", this);
            return null;
        }
        area.obj.transform.position = gatheringPlaceTrans[slot].position;
        area.obj.SetActive(true);
        area.obj.GetComponentInChildren<Text>().text = LoadGameData.Instance.GetString(label);
        return area;
    }

    public void Setup()
    {
        isMini = false;
        InGameMgr.Instance.state = State.Gathering;
        GatheringPanel.SetActive(true);
    }

    void StartMiniGame(string _code)
    {
        if (!GatheringPanel.activeSelf)
            return;
        isMini = true;
        CloseGatheringPannel();

        MiniGameMgr.Instance.Setup(_code);
    }

    public void CloseGatheringPannel()
    {
        if (!GatheringPanel.activeSelf)
            return;
        if (InGameMgr.Instance.state == State.Gathering)
            InGameMgr.Instance.state = State.Camp;
        GatheringPanel.SetActive(false);

        if (isFirst && !isMini)
        {
            isFirst = false;
            TutorialMgr.Instance.OpenTutorial("Open_Tutorial_Camp_1");
        }
    }

    protected override void OnDestroy()
    {
        if (stageOwner != null) stageOwner.NextStageAct -= LoadStageGathering;
        if (GatheringBtn != null) GatheringBtn.onClick.RemoveListener(Setup);
        if (ExitBtn != null) ExitBtn.onClick.RemoveListener(CloseGatheringPannel);
        foreach (var listener in areaListeners)
            if (listener.Key != null) listener.Key.onClick.RemoveListener(listener.Value);
        base.OnDestroy();
    }
}

[System.Serializable]
public class GatheringObject
{
    public string code;
    public GameObject obj;
}
