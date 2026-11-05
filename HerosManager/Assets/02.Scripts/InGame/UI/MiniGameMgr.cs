using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MiniGameMgr : Singleton<MiniGameMgr>
{
    public GameObject MiniGamePanelBack;
    public GameObject MiniGamePanel;
    public Button ExitBtn;
    public MiniGameInfo[] miniGames;
    public Transform[] minigamePlaceTrans;
    public Text[] GuideTexts;
    public Text woodText, waterText, meatText, hubText;

    [SerializeField, Min(1f)] float randomGameInterval = 15f;
    private const string PortalFunction = "MG_S3_Rand_Portal";
    private readonly MiniGameRewardLedger ledger = new MiniGameRewardLedger();
    private readonly Dictionary<string, MiniGameInfo> catalog = new Dictionary<string, MiniGameInfo>(StringComparer.Ordinal);
    private readonly Dictionary<GameObject, MiniGameSetMgr> controllers = new Dictionary<GameObject, MiniGameSetMgr>();
    private readonly List<GameObject> activatedAncestors = new List<GameObject>();
    private Coroutine cycleRandomGameCo;
    private bool closing;

    public int PendingGems => ledger.Gems;
    public int PendingWood => ledger[0];
    public string CurrentArea { get; private set; }

    void Start()
    {
        if (ExitBtn != null)
            ExitBtn.onClick.AddListener(CloseMiniGame);
    }

    public void Setup(string _code)
    {
        if (closing || ledger.IsOpen)
            return;
        if (string.IsNullOrEmpty(_code) || !LoadGameData.Instance.collectSpaceDatas.TryGetValue(_code, out var data))
        {
            Debug.LogError($"Unknown collecting area: {_code}", this);
            return;
        }
        if (MiniGamePanelBack == null || MiniGamePanel == null || !BuildCatalog())
        {
            Debug.LogError("Minigame panel or game configuration is incomplete.", this);
            return;
        }
        List<CollectSpaceData> candidates = null;
        if (data.FunctionID == PortalFunction)
        {
            candidates = PortalCandidates();
            if (candidates.Count == 0)
            {
                Debug.LogWarning("No configured minigame pairs are available for the portal.", this);
                return;
            }
        }
        else if (!CanShowPair(data))
        {
            Debug.LogWarning($"Collecting area {_code} has an unavailable minigame or slot.", this);
            return;
        }

        StopRandomCycle();
        HideGames();
        ledger.Begin();
        CurrentArea = _code;
        InGameMgr.Instance.state = State.MiniGame;
        MiniGamePanelBack.SetActive(true);
        MiniGamePanel.SetActive(true);
        if (candidates != null)
            cycleRandomGameCo = StartCoroutine(CycleRandomGameCo(candidates));
        else if (!ShowPair(data))
            CancelSession();
        SetItemText();
    }

    private bool BuildCatalog()
    {
        catalog.Clear();
        controllers.Clear();
        if (miniGames == null)
            return false;
        foreach (MiniGameInfo game in miniGames)
        {
            if (game == null || IsEmptyCode(game.code) || game.obj == null)
                continue;
            if (!catalog.TryAdd(game.code, game))
            {
                Debug.LogError($"Duplicate minigame code: {game.code}", this);
                return false;
            }
            var found = game.obj.GetComponentsInChildren<MiniGameSetMgr>(true);
            if (found.Length != 1)
            {
                Debug.LogWarning($"Minigame {game.code} requires exactly one controller.", game.obj);
                continue;
            }
            controllers[game.obj] = found[0];
        }
        return catalog.Count > 0;
    }

    private static bool IsEmptyCode(string code) => string.IsNullOrWhiteSpace(code) || code == "-1";

    private bool CanShow(string code, int slot)
    {
        return IsEmptyCode(code) || (catalog.TryGetValue(code, out var game) && controllers.ContainsKey(game.obj) &&
            minigamePlaceTrans != null && slot < minigamePlaceTrans.Length && minigamePlaceTrans[slot] != null);
    }

    private bool CanShowPair(CollectSpaceData data)
    {
        if (!CanShow(data.MiniGameID1, 0) || !CanShow(data.MiniGameID2, 1))
            return false;
        if (IsEmptyCode(data.MiniGameID1))
            return !IsEmptyCode(data.MiniGameID2);
        if (IsEmptyCode(data.MiniGameID2))
            return true;
        return catalog[data.MiniGameID1].obj != catalog[data.MiniGameID2].obj;
    }

    private bool ShowPair(CollectSpaceData data)
    {
        try
        {
            ShowGame(data.MiniGameID1, data.MiniGameHelpStringID1, 0);
            ShowGame(data.MiniGameID2, data.MiniGameHelpStringID2, 1);
            return true;
        }
        catch (Exception exception)
        {
            Debug.LogException(exception, this);
            return false;
        }
    }

    private void ShowGame(string code, string helpCode, int slot)
    {
        if (IsEmptyCode(code))
            return;
        MiniGameInfo game = catalog[code];
        game.obj.transform.position = minigamePlaceTrans[slot].position;
        MiniGameReferences.ActivateWithin(game.obj, MiniGamePanel.transform, activatedAncestors);
        if (!controllers[game.obj].isActiveAndEnabled)
            throw new InvalidOperationException($"Minigame {code} has an inactive controller.");
        controllers[game.obj].SetUp();
        if (code.StartsWith("MG_S2_", StringComparison.Ordinal))
        {
            // Desert scene children contain old keyboard-only instructions over the playfield.
            // The shared, bounded guide above each game now owns those instructions.
            foreach (Text legacyGuide in game.obj.GetComponentsInChildren<Text>(true))
                legacyGuide.gameObject.SetActive(false);
        }
        if (GuideTexts != null && slot < GuideTexts.Length && GuideTexts[slot] != null)
        {
            var guide = GuideTexts[slot];
            guide.text = LoadGameData.Instance.GetString(helpCode);
            guide.rectTransform.sizeDelta = new Vector2(550, 96);
            guide.rectTransform.anchoredPosition = new Vector2(slot == 0 ? -300 : 350, 410);
            GameUiStyle.TextStyle(guide, 23, 20, GameUiStyle.Paper);
            guide.alignment = TextAnchor.MiddleCenter;
            guide.gameObject.SetActive(true);
        }
    }

    private void HideGames()
    {
        if (miniGames != null)
            foreach (MiniGameInfo game in miniGames)
                if (game != null && game.obj != null)
                    game.obj.SetActive(false);
        for (int index = activatedAncestors.Count - 1; index >= 0; index--)
            if (activatedAncestors[index] != null)
                activatedAncestors[index].SetActive(false);
        activatedAncestors.Clear();
        if (GuideTexts != null)
            foreach (Text guide in GuideTexts)
                if (guide != null)
                    guide.gameObject.SetActive(false);
    }

    private void SetItemText()
    {
        if (woodText != null) woodText.text = ledger[0].ToString();
        if (waterText != null) waterText.text = ledger[1].ToString();
        if (meatText != null) meatText.text = ledger[2].ToString();
        if (hubText != null) hubText.text = ledger[3].ToString();
    }

    private List<CollectSpaceData> PortalCandidates()
    {
        var candidates = new List<CollectSpaceData>();
        foreach (CollectSpaceData area in LoadGameData.Instance.collectSpaceDatas.Values)
            if (area.FunctionID != PortalFunction && !IsEmptyCode(area.MiniGameID1) &&
                !IsEmptyCode(area.MiniGameID2) && CanShowPair(area))
                candidates.Add(area);
        return candidates;
    }

    private IEnumerator CycleRandomGameCo(List<CollectSpaceData> candidates)
    {
        while (ledger.IsOpen)
        {
            HideGames();
            var area = candidates[UnityEngine.Random.Range(0, candidates.Count)];
            if (!ShowPair(area))
            {
                CancelSession();
                yield break;
            }
            yield return new WaitForSeconds(MiniGameRules.PositiveDelay(randomGameInterval, 1f));
        }
    }

    public void AddTempItem(string _item1Code, int _amount1)
    {
        if (ledger.Add(_item1Code, _amount1))
            SetItemText();
    }

    public bool AddTempGems(int amount) => ledger.AddGems(amount);

    public void CloseMiniGame()
    {
        if (closing || !ledger.TryComplete(out int[] rewards, out int gems))
            return;
        closing = true;
        try
        {
            // End UI/producers first. Callbacks cannot pay or reopen this session twice.
            StopRandomCycle();
            HideGames();
            if (MiniGamePanelBack != null) MiniGamePanelBack.SetActive(false);
            if (MiniGamePanel != null) MiniGamePanel.SetActive(false);
            RestoreCampState();
            SetItemText();
            ItemManager.Instance.AddItem(rewards[0], rewards[1], rewards[2], rewards[3], rewards[4]);
            CampGemWallet.Instance.Add(gems);
            if (GatheringManager.IsCreateInstance() && GatheringManager.Instance.isFirst)
            {
                GatheringManager.Instance.isFirst = false;
                TutorialMgr.Instance.OpenTutorial("Open_Tutorial_Camp_1");
            }
        }
        finally { closing = false; }
    }

    private void CancelSession()
    {
        ledger.Cancel();
        StopRandomCycle();
        HideGames();
        if (MiniGamePanelBack != null) MiniGamePanelBack.SetActive(false);
        if (MiniGamePanel != null) MiniGamePanel.SetActive(false);
        RestoreCampState();
        SetItemText();
    }

    private static void RestoreCampState()
    {
        if (!InGameMgr.IsCreateInstance())
            return;
        var inGame = InGameMgr.Instance;
        if (inGame != null && inGame.state == State.MiniGame)
            inGame.state = State.Camp;
    }

    private void StopRandomCycle()
    {
        if (cycleRandomGameCo == null)
            return;
        StopCoroutine(cycleRandomGameCo);
        cycleRandomGameCo = null;
    }

    void OnDisable()
    {
        // Scene teardown discards only uncommitted session rewards.
        if (!closing)
            CancelSession();
    }

    protected override void OnDestroy()
    {
        if (ExitBtn != null)
            ExitBtn.onClick.RemoveListener(CloseMiniGame);
        base.OnDestroy();
    }
}

[Serializable]
public class MiniGameInfo
{
    public string code;
    public GameObject obj;
}
