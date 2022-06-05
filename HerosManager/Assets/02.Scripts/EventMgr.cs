using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class EventMgr : Singleton<EventMgr>
{
    public GameObject EventButton;
    public GameObject OptionButtonPrefab;
    public GameObject EventPanel;
    public Text mainText;
    public GameObject Fullchar;
    public GameObject fullImgObj;
    public GameObject HalfChar;
    public GameObject topImgObj, botImgObj;
    public GameObject Option;
    public GameObject[] OptionButtons;
    public GameObject OptionResultText;

    public Sprite[] itemImgs;

    [HideInInspector]
    public List<string> EventList = new List<string>();
    
    
    [HideInInspector]
    public string nextEventID;
    [HideInInspector]
    public Image topCharImg, botCharImg;
    [HideInInspector]
    public Image fullCharImg;
    [HideInInspector]
    public bool isClicked;


    void Start()
    {
        SetUp();
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

        isClicked = false;
        fullCharImg = fullImgObj.GetComponent<Image>();
        topCharImg = topImgObj.GetComponent<Image>();
        botCharImg = botImgObj.GetComponent<Image>();
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
        nextEventID = EventList[idx];
        EventList.RemoveAt(idx);
    }

    public void EventButtonAction()
    {
        EventPanel.SetActive(true);

        //
        InGameMgr.Instance.state = State.Event;

        if(isClicked)
            return;
        
        Option.SetActive(true);
        OptionResultText.SetActive(false);
        foreach(var btn in OptionButtons)
        {
            btn.SetActive(false);
        }

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
            SetChoiceButton(OptionButtons[0],LoadGameData.Instance.eventDatas[nextEventID].Choice1ID);
        }

        if(LoadGameData.Instance.eventDatas[nextEventID].Choice2ID != "-1")
        {
            SetChoiceButton(OptionButtons[1],LoadGameData.Instance.eventDatas[nextEventID].Choice2ID);
        }

        if(LoadGameData.Instance.eventDatas[nextEventID].Choice3ID != "-1")
        {
            SetChoiceButton(OptionButtons[2], LoadGameData.Instance.eventDatas[nextEventID].Choice3ID);
        }
        
        isClicked = true;
    }

    public void SetChoiceButton(GameObject choiceBtn, string choiceID)
    {
        choiceBtn.SetActive(true);

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



            if(LoadGameData.Instance.choiceDatas[choiceID].NeedAmount > ItemManager.Instance.GetItemInfo(LoadGameData.Instance.choiceDatas[choiceID].NeedItemID).num)
            {
                choiceBtn.GetComponent<Button>().interactable = false;
            }
        }
 
        
        choiceBtn.GetComponent<Button>().onClick.RemoveAllListeners();
        choiceBtn.GetComponent<Button>().onClick.AddListener(() => ChoiceButtonAction(choiceID));
    }

    public void ChoiceButtonAction(string choiceID)
    {
        if(LoadGameData.Instance.choiceDatas[choiceID].NeedItemID != "-1")
        {
            ItemManager.Instance.AddItem(LoadGameData.Instance.choiceDatas[choiceID].NeedItemID, -LoadGameData.Instance.choiceDatas[choiceID].NeedAmount);
        }

        int rnd = Random.Range(1,101);
        StringBuilder optionRT = new StringBuilder();
        string statID = "";
        string stringID = "";

        if(rnd <= LoadGameData.Instance.choiceDatas[choiceID].RewardProb * 100) //성공
        {
            mainText.text = LoadGameData.Instance.GetString(LoadGameData.Instance.choiceDatas[choiceID].SuccessRewardStringID);

            if(LoadGameData.Instance.choiceDatas[choiceID].RewardType != 0) // Hero1
            {
                //Reward1
                if(LoadGameData.Instance.choiceDatas[choiceID].H1Reward1Type != "-1")
                {
                    HeroStateManager.Instance.GetHeroStatInfo(LoadGameData.Instance.choiceDatas[choiceID].Hero1ID).AddStat(LoadGameData.Instance.choiceDatas[choiceID].H1Reward1Type, LoadGameData.Instance.choiceDatas[choiceID].H1Reward1Amount);
                    
                    if(LoadGameData.Instance.choiceDatas[choiceID].H1Reward1Type == "stress")
                    {
                        statID = "HeroState_w1";
                        stringID = "Event_Result_a1";

                        optionRT.Append(string.Format(LoadGameData.Instance.GetString(stringID), LoadGameData.Instance.GetString(LoadGameData.Instance.heroDatas[LoadGameData.Instance.choiceDatas[choiceID].Hero1ID].HeroStringID)));
                        
                        if(LoadGameData.Instance.choiceDatas[choiceID].H1Reward1Amount < 0) //스트레스 음수
                        {
                            //stringID = "Event_Result_a1";
                            optionRT.Append("<color=green>");
                            optionRT.Append(LoadGameData.Instance.GetString("Event_Result_a1_increase"));
                            optionRT.Append("</color>");

                        }
                        else //스트레스 양수
                        {
                            //stringID = "Event_Result_a2";
                            optionRT.Append("<color=red>");
                            optionRT.Append(LoadGameData.Instance.GetString("Event_Result_a2_increase"));
                            optionRT.Append("</color>");
                        }

                    
                    }
                    else
                    {
                        if(LoadGameData.Instance.choiceDatas[choiceID].H1Reward1Type == "power")
                        {
                            statID = "HeroState_w2";
                        }
                        else if(LoadGameData.Instance.choiceDatas[choiceID].H1Reward1Type == "hp")
                        {
                            statID = "HeroState_w3";
                        }
                        else if(LoadGameData.Instance.choiceDatas[choiceID].H1Reward1Type == "exp")
                        {
                            statID = "HeroState_w4";
                        }

                        stringID = "Event_Result_a3";
                        optionRT.Append(string.Format(LoadGameData.Instance.GetString(stringID), LoadGameData.Instance.GetString(LoadGameData.Instance.heroDatas[LoadGameData.Instance.choiceDatas[choiceID].Hero1ID].HeroStringID), LoadGameData.Instance.GetString(statID)));


                        if(LoadGameData.Instance.choiceDatas[choiceID].H1Reward1Amount > 0) //비 스트레스 양수
                        {
                            //stringID = "Event_Result_a3";
                            optionRT.Append("<color=green>");
                            optionRT.Append(LoadGameData.Instance.GetString("Event_Result_a3_increase"));
                            optionRT.Append(LoadGameData.Instance.choiceDatas[choiceID].H1Reward1Amount.ToString());
                            optionRT.Append("</color>");
                        }
                        else //비 스트레스 음수
                        {
                            //stringID = "Event_Result_a4";
                            optionRT.Append("<color=red>");
                            optionRT.Append(LoadGameData.Instance.choiceDatas[choiceID].H1Reward1Amount.ToString());
                            optionRT.Append("</color>");
                        }

                        
                    }
                    // {0}의 {1} /color +{2} /color
                    
                }
                //Reward2
                if(LoadGameData.Instance.choiceDatas[choiceID].H1Reward2Type != "-1")
                {
                    HeroStateManager.Instance.GetHeroStatInfo(LoadGameData.Instance.choiceDatas[choiceID].Hero1ID).AddStat(LoadGameData.Instance.choiceDatas[choiceID].H1Reward2Type, LoadGameData.Instance.choiceDatas[choiceID].H1Reward2Amount);
                    optionRT.Append("\n");
                    if(LoadGameData.Instance.choiceDatas[choiceID].H1Reward2Type == "stress")
                    {
                        stringID = "Event_Result_a1";
                        optionRT.Append(string.Format(LoadGameData.Instance.GetString(stringID), LoadGameData.Instance.GetString(LoadGameData.Instance.heroDatas[LoadGameData.Instance.choiceDatas[choiceID].Hero1ID].HeroStringID)));


                        if(LoadGameData.Instance.choiceDatas[choiceID].H1Reward2Amount < 0) //스트레스 음수
                        {
                            //stringID = "Event_Result_a1";
                            optionRT.Append("<color=green>");
                            optionRT.Append(LoadGameData.Instance.GetString("Event_Result_a1_increase"));
                            optionRT.Append("</color>");
                        }
                        else //스트레스 양수
                        {
                            //stringID = "Event_Result_a2";
                            optionRT.Append("<color=red>");
                            optionRT.Append(LoadGameData.Instance.GetString("Event_Result_a2_increase"));
                            optionRT.Append("</color>");
                        }

                        
                    }
                    else
                    {
                        if(LoadGameData.Instance.choiceDatas[choiceID].H1Reward2Type == "power")
                        {
                            statID = "HeroState_w2";
                        }
                        else if(LoadGameData.Instance.choiceDatas[choiceID].H1Reward2Type == "hp")
                        {
                            statID = "HeroState_w3";
                        }
                        else if(LoadGameData.Instance.choiceDatas[choiceID].H1Reward2Type == "exp")
                        {
                            statID = "HeroState_w4";
                        }

                        stringID = "Event_Result_a3";
                        optionRT.Append(string.Format(LoadGameData.Instance.GetString(stringID), LoadGameData.Instance.GetString(LoadGameData.Instance.heroDatas[LoadGameData.Instance.choiceDatas[choiceID].Hero1ID].HeroStringID), LoadGameData.Instance.GetString(statID) ));


                        if(LoadGameData.Instance.choiceDatas[choiceID].H1Reward2Amount > 0) //비 스트레스 양수
                        {
                            //stringID = "Event_Result_a3";
                            optionRT.Append("<color=green>");
                            optionRT.Append(LoadGameData.Instance.GetString("Event_Result_a3_increase"));
                            optionRT.Append(LoadGameData.Instance.choiceDatas[choiceID].H1Reward2Amount.ToString());
                            optionRT.Append("</color>");
                        }
                        else //비 스트레스 음수
                        {
                            //stringID = "Event_Result_a4";
                            optionRT.Append("<color=red>");
                            optionRT.Append(LoadGameData.Instance.choiceDatas[choiceID].H1Reward2Amount.ToString());
                            optionRT.Append("</color>");
                        }
                    }
                }
            }

            if(LoadGameData.Instance.choiceDatas[choiceID].RewardType == 2) // Hero2
            {
                //Reward1
                if(LoadGameData.Instance.choiceDatas[choiceID].H2Reward1Type != "-1")
                {
                    optionRT.Append("\n");
                    optionRT.Append("\n");
                    HeroStateManager.Instance.GetHeroStatInfo(LoadGameData.Instance.choiceDatas[choiceID].Hero2ID).AddStat(LoadGameData.Instance.choiceDatas[choiceID].H2Reward1Type, LoadGameData.Instance.choiceDatas[choiceID].H2Reward1Amount);
                    
                    if(LoadGameData.Instance.choiceDatas[choiceID].H2Reward1Type == "stress")
                    {
                        stringID = "Event_Result_a1";
                        optionRT.Append(string.Format(LoadGameData.Instance.GetString(stringID), LoadGameData.Instance.GetString(LoadGameData.Instance.heroDatas[LoadGameData.Instance.choiceDatas[choiceID].Hero2ID].HeroStringID)));

                        //statID = "HeroStat_w1"
                        if(LoadGameData.Instance.choiceDatas[choiceID].H2Reward1Amount < 0) //스트레스 음수
                        {
                            //stringID = "Event_Result_a1";
                            optionRT.Append("<color=green>");
                            optionRT.Append(LoadGameData.Instance.GetString("Event_Result_a1_increase"));
                            optionRT.Append("</color>");
                        }
                        else //스트레스 양수
                        {
                            //stringID = "Event_Result_a2";
                            optionRT.Append("<color=red>");
                            optionRT.Append(LoadGameData.Instance.GetString("Event_Result_a2_increase"));
                            optionRT.Append("</color>");
                        }

                        
                    }
                    else
                    {
                        if(LoadGameData.Instance.choiceDatas[choiceID].H2Reward1Type == "power")
                        {
                            statID = "HeroState_w2";
                        }
                        else if(LoadGameData.Instance.choiceDatas[choiceID].H2Reward1Type == "hp")
                        {
                            statID = "HeroState_w3";
                        }
                        else if(LoadGameData.Instance.choiceDatas[choiceID].H2Reward1Type == "exp")
                        {
                            statID = "HeroState_w4";
                        }

                        stringID = "Event_Result_a3";
                        optionRT.Append(string.Format(LoadGameData.Instance.GetString(stringID), LoadGameData.Instance.GetString(LoadGameData.Instance.heroDatas[LoadGameData.Instance.choiceDatas[choiceID].Hero2ID].HeroStringID), LoadGameData.Instance.GetString(statID)));


                        if(LoadGameData.Instance.choiceDatas[choiceID].H2Reward1Amount > 0) //비 스트레스 양수
                        {
                            //stringID = "Event_Result_a3";
                            optionRT.Append("<color=green>");
                            optionRT.Append(LoadGameData.Instance.GetString("Event_Result_a3_increase"));
                            optionRT.Append(LoadGameData.Instance.choiceDatas[choiceID].H2Reward1Amount.ToString());
                            optionRT.Append("</color>");
                        }
                        else //비 스트레스 음수
                        {
                            //stringID = "Event_Result_a4";
                            optionRT.Append("<color=red>");
                            optionRT.Append(LoadGameData.Instance.choiceDatas[choiceID].H2Reward1Amount.ToString());
                            optionRT.Append("</color>");
                        }

                        
                    }
                    
                }
                //Reward2
                if(LoadGameData.Instance.choiceDatas[choiceID].H2Reward2Type != "-1")
                {
                    HeroStateManager.Instance.GetHeroStatInfo(LoadGameData.Instance.choiceDatas[choiceID].Hero2ID).AddStat(LoadGameData.Instance.choiceDatas[choiceID].H2Reward2Type, LoadGameData.Instance.choiceDatas[choiceID].H2Reward2Amount);
                    optionRT.Append("\n");
                    if(LoadGameData.Instance.choiceDatas[choiceID].H2Reward2Type == "stress")
                    {
                        stringID = "Event_Result_a1";
                        optionRT.Append(string.Format(LoadGameData.Instance.GetString(stringID), LoadGameData.Instance.GetString(LoadGameData.Instance.heroDatas[LoadGameData.Instance.choiceDatas[choiceID].Hero2ID].HeroStringID)));
                        //statID = "HeroStat_w1"
                        if(LoadGameData.Instance.choiceDatas[choiceID].H2Reward2Amount < 0) //스트레스 음수
                        {
                            //stringID = "Event_Result_a1";
                            optionRT.Append("<color=green>");
                            optionRT.Append(LoadGameData.Instance.GetString("Event_Result_a1_increase"));
                            optionRT.Append("</color>");
                        }
                        else //스트레스 양수
                        {
                            //stringID = "Event_Result_a2";
                            optionRT.Append("<color=red>");
                            optionRT.Append(LoadGameData.Instance.GetString("Event_Result_a2_increase"));
                            optionRT.Append("</color>");
                        }

                    }
                    else
                    {
                        if(LoadGameData.Instance.choiceDatas[choiceID].H2Reward2Type == "power")
                        {
                            statID = "HeroState_w2";
                        }
                        else if(LoadGameData.Instance.choiceDatas[choiceID].H2Reward2Type == "hp")
                        {
                            statID = "HeroState_w3";
                        }
                        else if(LoadGameData.Instance.choiceDatas[choiceID].H2Reward2Type == "exp")
                        {
                            statID = "HeroState_w4";
                        }

                        stringID = "Event_Result_a3";
                        optionRT.Append(string.Format(LoadGameData.Instance.GetString(stringID), LoadGameData.Instance.GetString(LoadGameData.Instance.heroDatas[LoadGameData.Instance.choiceDatas[choiceID].Hero2ID].HeroStringID), LoadGameData.Instance.GetString(statID)));

                        if(LoadGameData.Instance.choiceDatas[choiceID].H2Reward2Amount > 0) //비 스트레스 양수
                        {
                            //stringID = "Event_Result_a3";
                            optionRT.Append("<color=green>");
                            optionRT.Append(LoadGameData.Instance.GetString("Event_Result_a3_increase"));
                            optionRT.Append(LoadGameData.Instance.choiceDatas[choiceID].H2Reward2Amount.ToString());
                            optionRT.Append("</color>");
                        }
                        else //비 스트레스 음수
                        {
                            //stringID = "Event_Result_a4";
                            optionRT.Append("<color=red>");
                            optionRT.Append(LoadGameData.Instance.choiceDatas[choiceID].H2Reward2Amount.ToString());
                            optionRT.Append("</color>");
                        }

                    }
                }
            }

            if(LoadGameData.Instance.choiceDatas[choiceID].AddEventID != "-1")
                EventList.Add(LoadGameData.Instance.choiceDatas[choiceID].AddEventID);
        }
        else //실패
        {
            mainText.text = LoadGameData.Instance.GetString(LoadGameData.Instance.choiceDatas[choiceID].FailRewardStringID);
        
            if(LoadGameData.Instance.choiceDatas[choiceID].RewardType != 0) // Hero1
            {
                //Reward1
                if(LoadGameData.Instance.choiceDatas[choiceID].FailH1Reward1Type != "-1")
                {
                    HeroStateManager.Instance.GetHeroStatInfo(LoadGameData.Instance.choiceDatas[choiceID].FailHero1ID).AddStat(LoadGameData.Instance.choiceDatas[choiceID].FailH1Reward1Type, LoadGameData.Instance.choiceDatas[choiceID].FailH1Reward1Amount);
                    
                    if(LoadGameData.Instance.choiceDatas[choiceID].FailH1Reward1Type == "stress")
                    {
                        stringID = "Event_Result_a1";
                        optionRT.Append(string.Format(LoadGameData.Instance.GetString(stringID), LoadGameData.Instance.GetString(LoadGameData.Instance.heroDatas[LoadGameData.Instance.choiceDatas[choiceID].FailHero1ID].HeroStringID)));
                        //statID = "HeroState_w1";
                        if(LoadGameData.Instance.choiceDatas[choiceID].FailH1Reward1Amount < 0) //스트레스 음수
                        {
                            //stringID = "Event_Result_a1";
                            optionRT.Append("<color=green>");
                            optionRT.Append(LoadGameData.Instance.GetString("Event_Result_a1_increase"));
                            optionRT.Append("</color>");
                        }
                        else //스트레스 양수
                        {
                            //stringID = "Event_Result_a2";
                            optionRT.Append("<color=red>");
                            optionRT.Append(LoadGameData.Instance.GetString("Event_Result_a2_increase"));
                            optionRT.Append("</color>");
                        }

                    }
                    else
                    {
                        if(LoadGameData.Instance.choiceDatas[choiceID].FailH1Reward1Type == "power")
                        {
                            statID = "HeroState_w2";
                        }
                        else if(LoadGameData.Instance.choiceDatas[choiceID].FailH1Reward1Type == "hp")
                        {
                            statID = "HeroState_w3";
                        }
                        else if(LoadGameData.Instance.choiceDatas[choiceID].FailH1Reward1Type == "exp")
                        {
                            statID = "HeroState_w4";
                        }

                        stringID = "Event_Result_a3";
                        optionRT.Append(string.Format(LoadGameData.Instance.GetString(stringID), LoadGameData.Instance.GetString(LoadGameData.Instance.heroDatas[LoadGameData.Instance.choiceDatas[choiceID].FailHero1ID].HeroStringID), LoadGameData.Instance.GetString(statID) ));


                        if(LoadGameData.Instance.choiceDatas[choiceID].FailH1Reward1Amount > 0) //비 스트레스 양수
                        {
                            //stringID = "Event_Result_a3";
                            optionRT.Append("<color=green>");
                            optionRT.Append(LoadGameData.Instance.GetString("Event_Result_a3_increase"));
                            optionRT.Append(LoadGameData.Instance.choiceDatas[choiceID].FailH1Reward1Amount.ToString());
                            optionRT.Append("</color>");
                        }
                        else //비 스트레스 음수
                        {
                            //stringID = "Event_Result_a4";
                            optionRT.Append("<color=red>");
                            optionRT.Append(LoadGameData.Instance.choiceDatas[choiceID].FailH1Reward1Amount.ToString());
                            optionRT.Append("</color>");
                        }

                        
                    }
                    // {0}의 {1} /color +{2} /color
                    
                }
                //Reward2
                if(LoadGameData.Instance.choiceDatas[choiceID].FailH1Reward2Type != "-1")
                {
                    HeroStateManager.Instance.GetHeroStatInfo(LoadGameData.Instance.choiceDatas[choiceID].FailHero1ID).AddStat(LoadGameData.Instance.choiceDatas[choiceID].FailH1Reward2Type, LoadGameData.Instance.choiceDatas[choiceID].FailH1Reward2Amount);
                    optionRT.Append("\n");
                    if(LoadGameData.Instance.choiceDatas[choiceID].FailH1Reward2Type == "stress")
                    {
                        stringID = "Event_Result_a1";
                        optionRT.Append(string.Format(LoadGameData.Instance.GetString(stringID), LoadGameData.Instance.GetString(LoadGameData.Instance.heroDatas[LoadGameData.Instance.choiceDatas[choiceID].FailHero1ID].HeroStringID)));
                        //statID = "HeroStat_w1"
                        if(LoadGameData.Instance.choiceDatas[choiceID].FailH1Reward2Amount < 0) //스트레스 음수
                        {
                            //stringID = "Event_Result_a1";
                            optionRT.Append("<color=green>");
                            optionRT.Append(LoadGameData.Instance.GetString("Event_Result_a1_increase"));
                            optionRT.Append("</color>");
                        }
                        else //스트레스 양수
                        {
                            //stringID = "Event_Result_a2";
                            optionRT.Append("<color=red>");
                            optionRT.Append(LoadGameData.Instance.GetString("Event_Result_a2_increase"));
                            optionRT.Append("</color>");
                        }
                        
                    }
                    else
                    {
                        if(LoadGameData.Instance.choiceDatas[choiceID].FailH1Reward2Type == "power")
                        {
                            statID = "HeroState_w2";
                        }
                        else if(LoadGameData.Instance.choiceDatas[choiceID].FailH1Reward2Type == "hp")
                        {
                            statID = "HeroState_w3";
                        }
                        else if(LoadGameData.Instance.choiceDatas[choiceID].FailH1Reward2Type == "exp")
                        {
                            statID = "HeroState_w4";
                        }

                        stringID = "Event_Result_a3";
                        optionRT.Append(string.Format(LoadGameData.Instance.GetString(stringID), LoadGameData.Instance.GetString(LoadGameData.Instance.heroDatas[LoadGameData.Instance.choiceDatas[choiceID].FailHero1ID].HeroStringID), LoadGameData.Instance.GetString(statID) ));


                        if(LoadGameData.Instance.choiceDatas[choiceID].FailH1Reward2Amount > 0) //비 스트레스 양수
                        {
                            //stringID = "Event_Result_a3";
                            optionRT.Append("<color=green>");
                            optionRT.Append(LoadGameData.Instance.GetString("Event_Result_a3_increase"));
                            optionRT.Append(LoadGameData.Instance.choiceDatas[choiceID].FailH1Reward2Amount.ToString());
                            optionRT.Append("</color>");
                        }
                        else //비 스트레스 음수
                        {
                            //stringID = "Event_Result_a4";
                            optionRT.Append("<color=red>");
                            optionRT.Append(LoadGameData.Instance.choiceDatas[choiceID].FailH1Reward2Amount.ToString());
                            optionRT.Append("</color>");
                        }

                    }
                }
            }

            if(LoadGameData.Instance.choiceDatas[choiceID].RewardType == 2) // Hero2
            {
                //Reward1
                if(LoadGameData.Instance.choiceDatas[choiceID].FailH2Reward1Type != "-1")
                {
                    optionRT.Append("\n");
                    optionRT.Append("\n");
                    HeroStateManager.Instance.GetHeroStatInfo(LoadGameData.Instance.choiceDatas[choiceID].FailHero2ID).AddStat(LoadGameData.Instance.choiceDatas[choiceID].FailH2Reward1Type, LoadGameData.Instance.choiceDatas[choiceID].FailH2Reward1Amount);
                    
                    if(LoadGameData.Instance.choiceDatas[choiceID].FailH2Reward1Type == "stress")
                    {
                        stringID = "Event_Result_a1";
                        optionRT.Append(string.Format(LoadGameData.Instance.GetString(stringID), LoadGameData.Instance.GetString(LoadGameData.Instance.heroDatas[LoadGameData.Instance.choiceDatas[choiceID].FailHero2ID].HeroStringID)));
                        //statID = "HeroStat_w1"
                        if(LoadGameData.Instance.choiceDatas[choiceID].FailH2Reward1Amount < 0) //스트레스 음수
                        {
                            //stringID = "Event_Result_a1";
                            optionRT.Append("<color=green>");
                            optionRT.Append(LoadGameData.Instance.GetString("Event_Result_a1_increase"));
                            optionRT.Append("</color>");
                        }
                        else //스트레스 양수
                        {
                            //stringID = "Event_Result_a2";
                            optionRT.Append("<color=red>");
                            optionRT.Append(LoadGameData.Instance.GetString("Event_Result_a2_increase"));
                            optionRT.Append("</color>");
                        }

                        
                    }
                    else
                    {
                        if(LoadGameData.Instance.choiceDatas[choiceID].FailH2Reward1Type == "power")
                        {
                            statID = "HeroState_w2";
                        }
                        else if(LoadGameData.Instance.choiceDatas[choiceID].FailH2Reward1Type == "hp")
                        {
                            statID = "HeroState_w3";
                        }
                        else if(LoadGameData.Instance.choiceDatas[choiceID].FailH2Reward1Type == "exp")
                        {
                            statID = "HeroState_w4";
                        }

                        stringID = "Event_Result_a3";
                        optionRT.Append(string.Format(LoadGameData.Instance.GetString(stringID), LoadGameData.Instance.GetString(LoadGameData.Instance.heroDatas[LoadGameData.Instance.choiceDatas[choiceID].FailHero2ID].HeroStringID), LoadGameData.Instance.GetString(statID)));


                        if(LoadGameData.Instance.choiceDatas[choiceID].FailH2Reward1Amount > 0) //비 스트레스 양수
                        {
                            //stringID = "Event_Result_a3";
                            optionRT.Append("<color=green>");
                            optionRT.Append(LoadGameData.Instance.GetString("Event_Result_a3_increase"));
                            optionRT.Append(LoadGameData.Instance.choiceDatas[choiceID].FailH2Reward1Amount.ToString());
                            optionRT.Append("</color>");
                        }
                        else //비 스트레스 음수
                        {
                            //stringID = "Event_Result_a4";
                            optionRT.Append("<color=red>");
                            optionRT.Append(LoadGameData.Instance.choiceDatas[choiceID].FailH2Reward1Amount.ToString());
                            optionRT.Append("</color>");
                        }
                    }
                    
                }
                //Reward2
                if(LoadGameData.Instance.choiceDatas[choiceID].FailH2Reward2Type != "-1")
                {
                    HeroStateManager.Instance.GetHeroStatInfo(LoadGameData.Instance.choiceDatas[choiceID].FailHero2ID).AddStat(LoadGameData.Instance.choiceDatas[choiceID].FailH2Reward2Type, LoadGameData.Instance.choiceDatas[choiceID].FailH2Reward2Amount);
                    optionRT.Append("\n");
                    if(LoadGameData.Instance.choiceDatas[choiceID].FailH2Reward2Type == "stress")
                    {
                        stringID = "Event_Result_a1";
                        optionRT.Append(string.Format(LoadGameData.Instance.GetString(stringID), LoadGameData.Instance.GetString(LoadGameData.Instance.heroDatas[LoadGameData.Instance.choiceDatas[choiceID].FailHero2ID].HeroStringID)));
                        //statID = "HeroStat_w1"
                        if(LoadGameData.Instance.choiceDatas[choiceID].FailH2Reward2Amount < 0) //스트레스 음수
                        {
                            //stringID = "Event_Result_a1";
                            optionRT.Append("<color=green>");
                            optionRT.Append(LoadGameData.Instance.GetString("Event_Result_a1_increase"));
                            optionRT.Append("</color>");
                        }
                        else //스트레스 양수
                        {
                            //stringID = "Event_Result_a2";
                            optionRT.Append("<color=red>");
                            optionRT.Append(LoadGameData.Instance.GetString("Event_Result_a2_increase"));
                            optionRT.Append("</color>");
                        }

                        
                    }
                    else
                    {
                        if(LoadGameData.Instance.choiceDatas[choiceID].FailH2Reward2Type == "power")
                        {
                            statID = "HeroState_w2";
                        }
                        else if(LoadGameData.Instance.choiceDatas[choiceID].FailH2Reward2Type == "hp")
                        {
                            statID = "HeroState_w3";
                        }
                        else if(LoadGameData.Instance.choiceDatas[choiceID].FailH2Reward2Type == "exp")
                        {
                            statID = "HeroState_w4";
                        }
                        
                        stringID = "Event_Result_a3";
                        optionRT.Append(string.Format(LoadGameData.Instance.GetString(stringID), LoadGameData.Instance.GetString(LoadGameData.Instance.heroDatas[LoadGameData.Instance.choiceDatas[choiceID].FailHero2ID].HeroStringID), LoadGameData.Instance.GetString(statID)));

                        if(LoadGameData.Instance.choiceDatas[choiceID].FailH2Reward2Amount > 0) //비 스트레스 양수
                        {
                            //stringID = "Event_Result_a3";
                            optionRT.Append("<color=green>");
                            optionRT.Append(LoadGameData.Instance.GetString("Event_Result_a3_increase"));
                            optionRT.Append(LoadGameData.Instance.choiceDatas[choiceID].FailH2Reward2Amount.ToString());
                            optionRT.Append("</color>");
                        }
                        else //비 스트레스 음수
                        {
                            //stringID = "Event_Result_a4";
                            optionRT.Append("<color=red>");
                            optionRT.Append(LoadGameData.Instance.choiceDatas[choiceID].FailH2Reward2Amount.ToString());
                            optionRT.Append("</color>");
                        }

                    }
                }
            }
        }

        

        OptionResultText.SetActive(true);
        OptionResultText.GetComponent<Text>().text = optionRT.ToString();
        Option.SetActive(false);
    }
}
