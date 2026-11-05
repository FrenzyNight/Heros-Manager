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
    Clock clockOwner;
    readonly List<KeyValuePair<Button, UnityAction>> areaListeners = new List<KeyValuePair<Button, UnityAction>>();
    [HideInInspector] public bool isFirst;
    [HideInInspector] public bool isMini;

    void Start()
    {
        stageOwner = InGameMgr.Instance;
        stageOwner.NextStageAct += LoadStageGathering;
        clockOwner = Clock.Instance;
        clockOwner.NextDayAct += LoadStageGathering;

        isFirst = stageOwner.stage == 1;
        isMini = false;
        
        GatheringBtn.onClick.AddListener(Setup);
        ExitBtn.onClick.AddListener(CloseGatheringPannel);
        EnsureDesertAreas();
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

    void EnsureDesertAreas()
    {
        // The authored scene only contains meadow cards. Reuse their layout before
        // registering callbacks, so generated cards never inherit another area's action.
        if (!LoadGameData.Instance.Catalog.TryGetStage(CampaignProgressionRules.DesertStage, out var desert)) return;
        var collect = LoadGameData.Instance.collectDatas[desert.CollectID];
        string[] codes = { collect.CollectSpaceID1, collect.CollectSpaceID2, collect.CollectSpaceID3, collect.CollectSpaceID4 };
        string[] descriptions = { "벌목 · 미로\n나무 / 약초 / 원석", "약초 채집 · 사냥\n약초 / 고기", "낚시 · 물긷기\n고기 / 물", "잠깐 열리는 보급처\n물 / 고기 / 나무" };
        var entries = new List<GatheringObject>(gatheringObjs);
        var template = Array.Find(gatheringObjs, entry => entry?.obj != null && entry.obj.GetComponent<Button>() != null);
        if (template == null) throw new InvalidOperationException("Desert gathering requires an authored area card template.");
        for (int slot = 0; slot < codes.Length; slot++)
        {
            if (entries.Exists(entry => entry.code == codes[slot])) continue;
            var card = Instantiate(template.obj, template.obj.transform.parent, false);
            card.name = codes[slot] + "_Button";
            card.GetComponent<Button>().onClick = new Button.ButtonClickedEvent();
            var background = card.GetComponent<Image>();
            background.sprite = null;
            background.color = slot == 3 ? new Color(0.64f, 0.83f, 0.76f) : new Color(0.91f, 0.80f, 0.60f);
            GameUiStyle.Shadow(background);
            foreach (var graphic in card.GetComponentsInChildren<Image>(true))
                if (graphic.gameObject != card) graphic.gameObject.SetActive(false);
            var title = card.GetComponentInChildren<Text>(true);
            title.rectTransform.anchorMin = title.rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            title.rectTransform.pivot = new Vector2(0.5f, 0.5f);
            title.rectTransform.anchoredPosition = new Vector2(0, 55);
            title.rectTransform.sizeDelta = new Vector2(520, 70);
            title.alignment = TextAnchor.MiddleCenter;
            GameUiStyle.TextStyle(title, 34, 30, GameUiStyle.Ink);
            title.fontStyle = FontStyle.Bold;
            var detail = Instantiate(title, title.transform.parent, false);
            detail.name = "Resources";
            detail.rectTransform.anchoredPosition = new Vector2(0, -40);
            detail.rectTransform.sizeDelta = new Vector2(520, 100);
            GameUiStyle.TextStyle(detail, 26, 22, GameUiStyle.Ink);
            detail.alignment = TextAnchor.MiddleCenter;
            detail.text = descriptions[slot];
            detail.raycastTarget = false;
            card.SetActive(false);
            entries.Add(new GatheringObject { code = codes[slot], obj = card });
        }
        gatheringObjs = entries.ToArray();
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

        // Use gameplay time: briefings, pause screens and preparation do not consume the visit.
        while (clockOwner != null && clockOwner.nowTime < rand)
            yield return null;
        if (clockOwner == null) yield break;

        var gatherObj = ShowArea(_collectData.CollectSpaceID4, _collectData.StringID4, 3);
        if (gatherObj == null)
            yield break;
        Notice.Instance.InstNoticeText(LoadGameData.Instance.GetString(_collectData.StringID4));

        float closesAt = clockOwner.nowTime + _collectData.ContinueTime;
        while (clockOwner != null && clockOwner.nowTime < closesAt)
            yield return null;

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
        if (clockOwner != null) clockOwner.NextDayAct -= LoadStageGathering;
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
