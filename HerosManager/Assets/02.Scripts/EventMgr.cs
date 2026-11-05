using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class EventMgr : Singleton<EventMgr>
{
    public GameObject EventButton;
    public Image EventNumberImg;
    public Text EventNumberText;
    public GameObject OptionButtonPrefab;
    public GameObject EventPanel;
    public Text mainText;
    public Sprite[] itemImgs;
    [HideInInspector] public List<string> EventList = new List<string>();
    [HideInInspector] public bool isFirstEvent;
    [HideInInspector] public bool isClicked;
    public GameObject[] EventPanels;
    [HideInInspector] public string nextEventID1, nextEventID2;
    public GameObject nextButton, prevButton;
    public int setedEventNum;

    private EventPanel[] panels;
    private Button eventButton;
    private int selectedCount;
    private bool available;

    private void Start()
    {
        eventButton = EventButton.GetComponent<Button>();
        panels = new EventPanel[EventPanels.Length];
        for (int i = 0; i < panels.Length; i++) panels[i] = EventPanels[i].GetComponent<EventPanel>();
        RestorePendingEvents(SaveDataManager.Instance.saveGameData);
        isFirstEvent = true;
        EventButtonDeActive();
        ShowPage(0);
    }

    public void RestorePendingEvents(SaveGameData saved)
    {
        EventList.Clear();
        var definitions = LoadGameData.Instance.eventDatas;
        if (saved != null && saved.hasEventSnapshot)
        {
            // Empty means exhausted; never reseed a completed queue when continuing.
            foreach (string id in saved.pendingEventIds ?? System.Array.Empty<string>())
            {
                if (!string.IsNullOrEmpty(id) && definitions.ContainsKey(id))
                {
                    if (!EventList.Contains(id)) EventList.Add(id);
                }
                else Debug.LogWarning($"Saved event '{id}' no longer exists in EventTable.", this);
            }
            return;
        }
        foreach (var data in definitions.Values)
            if (data.opentype == 1) EventList.Add(data.EventID);
        EventList.Sort(System.StringComparer.Ordinal);
    }

    /// <summary>Capture the pending queue at the daily checkpoint, before the next day's draw.</summary>
    public void CapturePendingEvents(SaveGameData snapshot)
    {
        if (snapshot == null) throw new System.ArgumentNullException(nameof(snapshot));
        snapshot.hasEventSnapshot = true;
        snapshot.pendingEventIds = EventList.ToArray();
    }

    public void NextButtonAction() => ShowPage(1);
    public void PrevButtonAction() => ShowPage(0);

    private void ShowPage(int index)
    {
        for (int i = 0; i < EventPanels.Length; i++)
            EventPanels[i].SetActive(i == index && i < selectedCount);
        prevButton.SetActive(index > 0 && selectedCount > 1);
        nextButton.SetActive(index == 0 && selectedCount > 1);
    }

    public void SetEventNumberText()
    {
        EventNumberText.text = setedEventNum + "/" + EventPanels.Length;
        if (eventButton != null) eventButton.interactable = available && selectedCount > 0;
    }

    public void EventButtonActive() => SetAvailable(true);
    public void EventButtonDeActive() => SetAvailable(false);

    private void SetAvailable(bool value)
    {
        available = value;
        var imageColor = EventNumberImg.color;
        var textColor = EventNumberText.color;
        imageColor.a = textColor.a = value ? 1f : 0.5f;
        EventNumberImg.color = imageColor;
        EventNumberText.color = textColor;
        SetEventNumberText();
    }

    public void ClosePanel()
    {
        EventPanel.SetActive(false);
        if (InGameMgr.Instance.state == State.Event) InGameMgr.Instance.state = State.Camp;
    }

    public void ChoseEvent()
    {
        isClicked = false;
        selectedCount = Mathf.Min(panels.Length, EventList.Count);
        for (int i = 0; i < panels.Length; i++)
        {
            string id = null;
            if (i < selectedCount)
            {
                int index = Random.Range(0, EventList.Count);
                id = EventList[index];
                EventList.RemoveAt(index);
            }
            panels[i].nextEventID = id;
        }
        nextEventID1 = panels.Length > 0 ? panels[0].nextEventID : null;
        nextEventID2 = panels.Length > 1 ? panels[1].nextEventID : null;
        setedEventNum = selectedCount;
        ShowPage(0);
        SetEventNumberText();
    }

    public void QueueEvent(string id)
    {
        if (LoadGameData.Instance.eventDatas.ContainsKey(id) && !EventList.Contains(id))
            EventList.Add(id);
    }

    public void CompleteEvent()
    {
        setedEventNum = Mathf.Max(0, setedEventNum - 1);
        SetEventNumberText();
    }

    public void EventButtonAction()
    {
        if (!available || selectedCount == 0) return;
        EventPanel.SetActive(true);
        InGameMgr.Instance.state = State.Event;
        if (isClicked)
        {
            for (int i = 0; i < selectedCount; i++) panels[i].RefreshChoiceAvailability();
            return;
        }
        for (int i = 0; i < selectedCount; i++) panels[i].SetEventPanel();
        ShowPage(0);
        isClicked = true;
        if (!isFirstEvent) return;
        isFirstEvent = false;
        TutorialMgr.Instance.OpenTutorial("Open_Tutorial_Event_1");
    }
}
