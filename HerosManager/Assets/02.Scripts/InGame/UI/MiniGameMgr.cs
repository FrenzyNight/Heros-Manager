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

    // The original DefineTable has no portal interval. Tune this in the Inspector.
    [SerializeField, Min(1f)] float randomGameInterval = 15f;
    readonly int[] rewards = new int[5];
    static readonly string[] ItemCodes =
        { "Item_Wood", "Item_Water", "Item_Meat", "Item_Hub", "Item_Food" };
    Coroutine cycleRandomGameCo;
    bool sessionOpen;

    void Start()
    {
        ExitBtn.onClick.AddListener(CloseMiniGame);
    }

    public void Setup(string _code)
    {
        if (!LoadGameData.Instance.collectSpaceDatas.TryGetValue(_code, out var data))
        {
            Debug.LogError($"Unknown collecting area: {_code}");
            return;
        }
        if (sessionOpen)
            return;

        StopRandomCycle();
        Array.Clear(rewards, 0, rewards.Length);
        HideGames();
        sessionOpen = true;
        InGameMgr.Instance.state = State.MiniGame;
        MiniGamePanelBack.SetActive(true);
        MiniGamePanel.SetActive(true);

        if (data.FunctionID == "MG_S3_Rand_Portal")
            cycleRandomGameCo = StartCoroutine(CycleRandomGameCo());
        else
        {
            ShowGame(data.MiniGameID1, data.MiniGameHelpStringID1, 0);
            ShowGame(data.MiniGameID2, data.MiniGameHelpStringID2, 1);
        }
        SetItemText();
    }

    MiniGameInfo FindGame(string code)
    {
        if (string.IsNullOrEmpty(code) || code == "-1" || miniGames == null)
            return null;
        return Array.Find(miniGames, game => game != null && game.code == code && game.obj != null);
    }

    void ShowGame(string code, string helpCode, int slot)
    {
        if (string.IsNullOrEmpty(code) || code == "-1")
            return;
        var game = FindGame(code);
        if (game == null || minigamePlaceTrans == null || slot >= minigamePlaceTrans.Length ||
            minigamePlaceTrans[slot] == null)
        {
            Debug.LogWarning($"Minigame {code} is not configured for slot {slot}.");
            return;
        }
        var controller = game.obj.GetComponentInChildren<MiniGameSetMgr>(true);
        if (controller == null)
        {
            Debug.LogError($"Minigame {code} has no MiniGameSetMgr.");
            return;
        }
        game.obj.transform.position = minigamePlaceTrans[slot].position;
        game.obj.SetActive(true);
        controller.SetUp();
        if (GuideTexts != null && slot < GuideTexts.Length && GuideTexts[slot] != null)
        {
            GuideTexts[slot].text = LoadGameData.Instance.GetString(helpCode);
            GuideTexts[slot].gameObject.SetActive(true);
        }
    }

    void HideGames()
    {
        if (miniGames != null)
            foreach (var game in miniGames)
                if (game != null && game.obj != null)
                    game.obj.SetActive(false);
        if (GuideTexts != null)
            foreach (var guide in GuideTexts)
                if (guide != null)
                    guide.gameObject.SetActive(false);
    }

    void SetItemText()
    {
        if (woodText != null) woodText.text = rewards[0].ToString();
        if (waterText != null) waterText.text = rewards[1].ToString();
        if (meatText != null) meatText.text = rewards[2].ToString();
        if (hubText != null) hubText.text = rewards[3].ToString();
    }

    IEnumerator CycleRandomGameCo()
    {
        var candidates = new List<CollectSpaceData>();
        foreach (var area in LoadGameData.Instance.collectSpaceDatas.Values)
        {
            if (area.FunctionID != "MG_S3_Rand_Portal" &&
                area.MiniGameID1 != area.MiniGameID2 &&
                FindGame(area.MiniGameID1) != null && FindGame(area.MiniGameID2) != null)
                candidates.Add(area);
        }
        if (candidates.Count == 0)
        {
            Debug.LogWarning("No configured minigame pairs are available for the portal.");
            yield break;
        }
        while (sessionOpen)
        {
            HideGames();
            var area = candidates[UnityEngine.Random.Range(0, candidates.Count)];
            ShowGame(area.MiniGameID1, area.MiniGameHelpStringID1, 0);
            ShowGame(area.MiniGameID2, area.MiniGameHelpStringID2, 1);
            yield return new WaitForSeconds(Mathf.Max(1f, randomGameInterval));
        }
    }

    public void AddTempItem(string _item1Code, int _amount1)
    {
        if (!sessionOpen)
            return;
        int index = Array.IndexOf(ItemCodes, _item1Code);
        if (index < 0)
            return;
        rewards[index] = (int)Math.Max(0L, Math.Min(int.MaxValue, (long)rewards[index] + _amount1));
        SetItemText();
    }

    public void CloseMiniGame()
    {
        if (!sessionOpen)
            return;
        // Consume the session before callbacks or deactivation can close it again.
        sessionOpen = false;
        StopRandomCycle();
        ItemManager.Instance.AddItem(rewards[0], rewards[1], rewards[2], rewards[3], rewards[4]);
        Array.Clear(rewards, 0, rewards.Length);
        HideGames();
        MiniGamePanelBack.SetActive(false);
        MiniGamePanel.SetActive(false);
        if (InGameMgr.Instance.state == State.MiniGame)
            InGameMgr.Instance.state = State.Camp;

        if (GatheringManager.Instance.isFirst)
        {
            GatheringManager.Instance.isFirst = false;
            TutorialMgr.Instance.OpenTutorial("Open_Tutorial_Camp_1");
        }
    }

    void StopRandomCycle()
    {
        if (cycleRandomGameCo == null)
            return;
        StopCoroutine(cycleRandomGameCo);
        cycleRandomGameCo = null;
    }

    void OnDisable()
    {
        StopRandomCycle();
    }
}

[Serializable]
public class MiniGameInfo
{
    public string code;
    public GameObject obj;
}
