using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class EventPanel : MonoBehaviour
{
    public Text mainText;
    public GameObject Fullchar, fullImgObj, HalfChar, topImgObj, botImgObj, Option;
    public GameObject[] OptionButtons;
    public GameObject OptionResultText;
    [HideInInspector] public Image topCharImg, botCharImg, fullCharImg;
    [HideInInspector] public bool isClicked;
    [HideInInspector] public string nextEventID;

    private readonly Dictionary<Button, UnityEngine.Events.UnityAction> listeners = new Dictionary<Button, UnityEngine.Events.UnityAction>();
    private readonly Dictionary<Button, string> choiceIds = new Dictionary<Button, string>();
    private readonly HashSet<string> offeredChoices = new HashSet<string>();

    public void SetEventPanel()
    {
        ClearListeners();
        offeredChoices.Clear();
        isClicked = false;
        Option.SetActive(false);
        OptionResultText.SetActive(false);
        foreach (var button in OptionButtons) button.SetActive(false);
        if (string.IsNullOrEmpty(nextEventID) || !LoadGameData.Instance.eventDatas.TryGetValue(nextEventID, out var data))
        {
            gameObject.SetActive(false);
            return;
        }
        fullCharImg = fullImgObj.GetComponent<Image>();
        topCharImg = topImgObj.GetComponent<Image>();
        botCharImg = botImgObj.GetComponent<Image>();
        bool confrontation = data.EventType == 1;
        Fullchar.SetActive(!confrontation);
        HalfChar.SetActive(confrontation);
        if (confrontation)
        {
            topCharImg.sprite = Resources.Load<Sprite>("Chars/" + data.H1ImageName);
            botCharImg.sprite = Resources.Load<Sprite>("Chars/" + data.H2ImageName);
        }
        else fullCharImg.sprite = Resources.Load<Sprite>("Chars/" + data.H1ImageName);
        mainText.text = LoadGameData.Instance.GetString(data.EventStringID);
        string[] choices = { data.Choice1ID, data.Choice2ID, data.Choice3ID };
        for (int i = 0; i < choices.Length && i < OptionButtons.Length; i++)
            if (!string.IsNullOrEmpty(choices[i]) && choices[i] != "-1")
                SetChoiceButton(OptionButtons[i], choices[i]);
        Option.SetActive(offeredChoices.Count > 0);
    }

    public void SetChoiceButton(GameObject choiceBtn, string choiceID)
    {
        if (!LoadGameData.Instance.choiceDatas.TryGetValue(choiceID, out var choice)) return;
        offeredChoices.Add(choiceID);
        choiceBtn.SetActive(true);
        choiceBtn.transform.GetChild(0).GetComponent<Text>().text = LoadGameData.Instance.GetString(choice.ChoiceStringID);
        var cost = choiceBtn.transform.GetChild(1);
        cost.gameObject.SetActive(choice.NeedAmount > 0 && choice.NeeditemID != "-1");
        if (cost.gameObject.activeSelf)
        {
            cost.GetChild(0).GetComponent<Text>().text = "-" + choice.NeedAmount;
            int index = InventoryRules.IndexOf(choice.NeeditemID);
            var sprites = EventMgr.Instance.itemImgs;
            cost.GetChild(1).GetComponent<Image>().sprite = index >= 0 && index < sprites.Length ? sprites[index] : null;
        }
        var button = choiceBtn.GetComponent<Button>();
        choiceIds[button] = choiceID;
        button.interactable = ItemManager.Instance.CanAfford(EventChoiceRules.GetCost(choice));
        if (listeners.TryGetValue(button, out var previous)) button.onClick.RemoveListener(previous);
        UnityEngine.Events.UnityAction listener = () => ChoiceButtonAction(choiceID);
        listeners[button] = listener;
        button.onClick.AddListener(listener);
    }

    public void RefreshChoiceAvailability()
    {
        if (isClicked) return;
        foreach (var entry in choiceIds)
            if (entry.Key != null)
                entry.Key.interactable = LoadGameData.Instance.choiceDatas.TryGetValue(entry.Value, out var choice) &&
                    ItemManager.Instance.CanAfford(EventChoiceRules.GetCost(choice));
    }

    public void ChoiceButtonAction(string choiceID)
    {
        if (isClicked || !offeredChoices.Contains(choiceID) ||
            !LoadGameData.Instance.choiceDatas.TryGetValue(choiceID, out var choice)) return;
        var outcome = EventChoiceRules.Resolve(choice, Random.Range(1, 101));
        foreach (var change in outcome.Changes)
            if (HeroStateManager.Instance.GetHeroStatInfo(change.HeroId) == null ||
                !LoadGameData.Instance.heroDatas.ContainsKey(change.HeroId))
            {
                Debug.LogError($"Choice {choiceID} references unavailable hero {change.HeroId}.", this);
                return;
            }
        if (!ItemManager.Instance.TrySpend(EventChoiceRules.GetCost(choice))) return;
        isClicked = true;
        var result = new StringBuilder();
        foreach (var change in outcome.Changes)
        {
            HeroStateManager.Instance.GetHeroStatInfo(change.HeroId).AddStat(change.Stat, change.Amount);
            AppendChange(result, change);
        }
        if (outcome.Success && !string.IsNullOrEmpty(choice.AddEventID) && choice.AddEventID != "-1")
            EventMgr.Instance.QueueEvent(choice.AddEventID);
        mainText.text = LoadGameData.Instance.GetString(outcome.StringId);
        EventMgr.Instance.CompleteEvent();
        OptionResultText.GetComponent<Text>().text = result.ToString();
        OptionResultText.SetActive(true);
        Option.SetActive(false);
    }

    private static void AppendChange(StringBuilder result, HeroStatChange change)
    {
        var tables = LoadGameData.Instance;
        if (result.Length > 0) result.AppendLine();
        string heroName = tables.GetString(tables.heroDatas[change.HeroId].HeroStringID);
        if (change.Stat == "stress")
        {
            bool reduced = change.Amount < 0;
            result.AppendFormat(tables.GetString("Event_Result_a1"), heroName);
            result.Append(reduced ? "<color=green>" : "<color=red>");
            result.Append(tables.GetString(reduced ? "Event_Result_a1_increase" : "Event_Result_a2_increase"));
        }
        else
        {
            string statName = change.Stat == "power" ? "HeroState_w2" : change.Stat == "hp" ? "HeroState_w3" : "HeroState_w4";
            result.AppendFormat(tables.GetString("Event_Result_a3"), heroName, tables.GetString(statName));
            result.Append(change.Amount > 0 ? "<color=green>" : "<color=red>");
            if (change.Amount > 0) result.Append(tables.GetString("Event_Result_a3_increase"));
            result.Append(change.Amount);
        }
        result.Append("</color>");
    }

    private void ClearListeners()
    {
        foreach (var entry in listeners)
            if (entry.Key != null) entry.Key.onClick.RemoveListener(entry.Value);
        listeners.Clear();
        choiceIds.Clear();
    }

    private void OnDestroy() => ClearListeners();
}
