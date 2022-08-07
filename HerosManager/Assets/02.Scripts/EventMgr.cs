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
    //public GameObject Fullchar;
    //public GameObject fullImgObj;
    //public GameObject HalfChar;
    //public GameObject topImgObj, botImgObj;
    //public GameObject Option;
    //public GameObject[] OptionButtons;
    //public GameObject OptionResultText;

    public Sprite[] itemImgs;

    [HideInInspector]
    public List<string> EventList = new List<string>();
    
    
    /*
    [HideInInspector]
    public string nextEventID;
    [HideInInspector]
    public Image topCharImg, botCharImg;
    [HideInInspector]
    public Image fullCharImg;
    */
    [HideInInspector]
    public bool isClicked;

    //DoubleEvent

    public GameObject[] EventPanels;
    [HideInInspector]
    public string nextEventID1, nextEventID2;

    public GameObject nextButton;
    public GameObject prevButton;

    public int setedEventNum;

    void Start()
    {
        SetUp();
    }

    void SetUp()
    {
        foreach(var evt in LoadGameData.Instance.eventDatas)
        {
            if(evt.Value.opentype == 1)
            {
                EventList.Add(evt.Value.EventID);
            }
        }

        isClicked = false;
        
        SetEventNumberText();
        PrevButtonAction();
    }

    public void NextButtonAction()
    {
        nextButton.SetActive(false);
        EventPanels[0].SetActive(false);
        
        prevButton.SetActive(true);
        EventPanels[1].SetActive(true);
    }

    public void PrevButtonAction()
    {
        prevButton.SetActive(false);
        EventPanels[1].SetActive(false);
        
        nextButton.SetActive(true);
        EventPanels[0].SetActive(true);
    }

    public void SetEventNumberText()
    {
        EventNumberText.text = setedEventNum.ToString() + "/2";
    }

    public void EventButtonActive()
    {
        EventButton.GetComponent<Button>().interactable = true;
        Color imgColor = EventNumberImg.color;
        imgColor.a = 1f;

        Color textColor = EventNumberText.color;
        textColor.a = 1f;

        EventNumberImg.color = imgColor;
        EventNumberText.color = textColor;

        //EventNumberText.text = "2/2";
        EventNumberText.text = setedEventNum.ToString() + "/2";
    }

    public void EventButtonDeActive()
    {
        EventButton.GetComponent<Button>().interactable = false;
        Color imgColor = EventNumberImg.color;
        imgColor.a = 0.5f;

        Color textColor = EventNumberText.color;
        textColor.a = 0.5f;

        EventNumberImg.color = imgColor;
        EventNumberText.color = textColor;

        EventNumberText.text = setedEventNum.ToString() + "/2";
        //EventNumberText.text = "0/2";
    }

    public void ClosePanel()
    {
        EventPanel.SetActive(false);
        InGameMgr.Instance.state = State.Camp;
    }

    public void ChoseEvent()
    {
        int idx = Random.Range(0, EventList.Count);

        isClicked = false;
        
        //nextEventID = EventList[idx];
        //EventList.RemoveAt(idx);

        nextEventID1 = EventList[idx];
        EventList.RemoveAt(idx);
        
        idx = Random.Range(0, EventList.Count);
        nextEventID2 = EventList[idx];
        EventList.RemoveAt(idx);

        setedEventNum = 2;
        
        EventPanels[0].GetComponent<EventPanel>().nextEventID = nextEventID1;
        EventPanels[1].GetComponent<EventPanel>().nextEventID = nextEventID2;
    }

    public void EventButtonAction()
    {
        EventPanel.SetActive(true);

        //
        InGameMgr.Instance.state = State.Event;

        if(isClicked)
            return;
        
        //Panel
        
        EventPanels[0].GetComponent<EventPanel>().SetEventPanel();
        EventPanels[1].GetComponent<EventPanel>().SetEventPanel();
        
        
        
        isClicked = true;
    }
}
