using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EventMgr : Singleton<EventMgr>
{
    public GameObject OptionButtonPrefab;
    public GameObject EventPanel;
    public Text mainText;
    public GameObject Fullchar;
    public GameObject fullImgObj;
    public GameObject HalfChar;
    public GameObject topImgObj, botImgObj;
    public GameObject Option;

    public Sprite[] itemImgs;

    [HideInInspector]
    public List<string> EventList = new List<string>();
    [HideInInspector]
    public List<GameObject> optionButtons = new List<GameObject>();
    [HideInInspector]
    public string nextEventID;
    [HideInInspector]
    public Image topCharImg, botCharImg;
    [HideInInspector]
    public Image fullCharImg;


    void Start()
    {
        SetUp();
    }

    void Update()
    {
        
    }

    void SetUp()
    {
        foreach(var evt in LoadGameData.Instance.eventDatas)
        {
            if(evt.Value.OpenType == 1)
            {
                EventList.Add(evt.Value.EventID);
            }
        }

        fullCharImg = fullImgObj.GetComponent<Image>();
        topCharImg = topImgObj.GetComponent<Image>();
        botCharImg = botImgObj.GetComponent<Image>();
    }

    public void ChoseEvent()
    {
        int idx = Random.Range(0, EventList.Count);

        nextEventID = EventList[idx];
        EventList.RemoveAt(idx);
    }

    public void EventButtonAction()
    {
        EventPanel.SetActive(true);

        //Load Char Img
        if(LoadGameData.Instance.eventDatas[nextEventID].EventType == 1) // 대립
        {
            Fullchar.SetActive(false);
            HalfChar.SetActive(true);

            topCharImg.sprite = Resources.Load<Sprite>("Chars/" + LoadGameData.Instance.eventDatas[nextEventID].H1ImgName);
            botCharImg.sprite = Resources.Load<Sprite>("Chars/" + LoadGameData.Instance.eventDatas[nextEventID].H2ImgName);
        }
        else //일반(0), 연계(2)
        {
            HalfChar.SetActive(false);
            Fullchar.SetActive(true);

            fullCharImg.sprite = Resources.Load<Sprite>("Chars/" + LoadGameData.Instance.eventDatas[nextEventID].H1ImgName);
        }
        //Load Event Text;
        mainText.text = LoadGameData.Instance.GetString(LoadGameData.Instance.eventDatas[nextEventID].EventStringID);

        //Load Choice Info
        if(LoadGameData.Instance.eventDatas[nextEventID].Choice1ID != "-1")
        {
            SetChoiceButton(LoadGameData.Instance.eventDatas[nextEventID].Choice1ID);
        }

        if(LoadGameData.Instance.eventDatas[nextEventID].Choice2ID != "-1")
        {
            SetChoiceButton(LoadGameData.Instance.eventDatas[nextEventID].Choice2ID);
        }

        if(LoadGameData.Instance.eventDatas[nextEventID].Choice3ID != "-1")
        {
            SetChoiceButton(LoadGameData.Instance.eventDatas[nextEventID].Choice3ID);
        }
        
    }

    public void SetChoiceButton(string choiceID)
    {
        GameObject choiceBtn = Instantiate(OptionButtonPrefab, new Vector3(0,0,0), Quaternion.identity, Option.transform);

        choiceBtn.transform.GetChild(0).GetComponent<Text>().text = LoadGameData.Instance.GetString(LoadGameData.Instance.choiceDatas[choiceID].ChoiceStringID);
         

        if(LoadGameData.Instance.choiceDatas[choiceID].NeedItemID != "-1")
        {
            choiceBtn.transform.GetChild(1).gameObject.SetActive(true);

            choiceBtn.transform.GetChild(1).transform.GetChild(0).GetComponent<Text>().text = "-" + LoadGameData.Instance.choiceDatas[choiceID].NeedAmount.ToString();

            if(LoadGameData.Instance.choiceDatas[choiceID].NeedItemID == "Item_Wood")
            {
                choiceBtn.transform.GetChild(1).transform.GetChild(1).GetComponent<Image>().sprite = itemImgs[0];
            }
            else if(LoadGameData.Instance.choiceDatas[choiceID].NeedItemID == "Item_Water")
            {
                choiceBtn.transform.GetChild(1).transform.GetChild(1).GetComponent<Image>().sprite = itemImgs[1];
            }
            else if(LoadGameData.Instance.choiceDatas[choiceID].NeedItemID == "Item_Meat")
            {
                choiceBtn.transform.GetChild(1).transform.GetChild(1).GetComponent<Image>().sprite = itemImgs[2];
            }
            else if(LoadGameData.Instance.choiceDatas[choiceID].NeedItemID == "Item_Hub")
            {
                choiceBtn.transform.GetChild(1).transform.GetChild(1).GetComponent<Image>().sprite = itemImgs[3];
            }
            else if(LoadGameData.Instance.choiceDatas[choiceID].NeedItemID == "Item_Food")
            {
                choiceBtn.transform.GetChild(1).transform.GetChild(1).GetComponent<Image>().sprite = itemImgs[4];
            }
        }

        choiceBtn.GetComponent<Button>().onClick.AddListener(() => ChoiceButtonAction(choiceID));
        optionButtons.Add(choiceBtn);
    }

    public void ChoiceButtonAction(string choiceID)
    {
        Debug.Log("Button Click");
    }
}
