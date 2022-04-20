using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FenceMgr : Singleton<FenceMgr>
{
    FenceData fenceData;
    int fenceLevel = 1;
    bool isLeveling;
    float timer;

    public Button FenceBtn;
    public GameObject FencePanel;
    public Image TitleImg;
    public Button ExitBtn;
    public GameObject FenceInfo;
    public GameObject FenceLevelup;
    public Button StartBtn;
    [Header("Level")]
    public Text LevelTxt;
    public Text PrevLevel;
    public Text NextLevel;
    [Header("Invade")]
    public Text InvadeTxt;
    public Text PrevRate;
    public Text NextRate;
    [Header("Wood")]
    public Text WoodTxt;
    public Text PrevNum;
    public Text NextNum;
    [Header("Time")]
    public Text TimeTxt;
    public Text PrevTime;
    public Text NextTime;
    [Header("Levelup")]
    public Image GuageFillImg;
    public Text LevelingTxt;
    public Button CancelBtn;
    public Button CompleteBtn;

    [Header("Object")]
    public GameObject fenceLv1;
    public GameObject fenceLv2;
    public GameObject fenceLv3;
    public GameObject fenceLv4;

    void Start()
    {
        InGameMgr.Instance.NextStageAct += LoadStageFence;

        FenceBtn.onClick.AddListener(Setup);
        ExitBtn.onClick.AddListener(ClosePanel);
        StartBtn.onClick.AddListener(FenceLevelUp);
        CancelBtn.onClick.AddListener(ClosePanel);
        CompleteBtn.onClick.AddListener(Setup);

        LevelTxt.text = LoadGameData.Instance.GetString("Fence_t1");
        InvadeTxt.text = LoadGameData.Instance.GetString("Fence_t2");
        WoodTxt.text = LoadGameData.Instance.GetString("Fence_t3");
        TimeTxt.text = LoadGameData.Instance.GetString("Fence_t4");
        StartBtn.GetComponentInChildren<Text>().text = LoadGameData.Instance.GetString("Fence_b1");
        CancelBtn.GetComponentInChildren<Text>().text = LoadGameData.Instance.GetString("Fence_b2");
        CompleteBtn.GetComponentInChildren<Text>().text = LoadGameData.Instance.GetString("Fence_b3");

        LoadStageFence();
    }

    void LoadStageFence()
    {
        fenceLevel = 1;
        foreach (var data in LoadGameData.Instance.fenceDatas)
        {
            if (data.Value.FenceGroupID == InGameMgr.Instance.stageData.FenceGroupID &&
                data.Value.FenceLevel == fenceLevel)
            {
                fenceData = data.Value;
                break;
            }
        }

        FenceObject();
    }

    void Setup()
    {
        InGameMgr.Instance.state = State.Fence;
        FencePanel.SetActive(true);
        FenceInfo.SetActive(true);
        FenceLevelup.SetActive(false);

        isLeveling = false;

        PrevLevel.text = fenceData.FenceLevel.ToString();
        PrevRate.text = fenceData.InvadeProb.ToString("P0");
        PrevNum.text = fenceData.Amount.ToString();
        PrevTime.text = fenceData.NeedTime.ToString();

        FenceData NextFenceData = null;
        int tempLevel = fenceLevel + 1;
        foreach (var data in LoadGameData.Instance.fenceDatas)
        {
            if (data.Value.FenceGroupID == InGameMgr.Instance.stageData.FenceGroupID &&
                data.Value.FenceLevel == tempLevel)
            {
                NextFenceData = data.Value;
                break;
            }
        }
        if (NextFenceData == null)
        {
            PrevNum.text = "-";
            PrevTime.text = "-";

            NextLevel.text = "-";
            NextRate.text = "-";
            NextNum.text = "-";
            NextTime.text = "-";

            StartBtn.interactable = false;
        }
        else
        {
            NextLevel.text = NextFenceData.FenceLevel.ToString();
            NextRate.text = NextFenceData.InvadeProb.ToString("P0");
            NextNum.text = NextFenceData.Amount.ToString();
            NextTime.text = NextFenceData.NeedTime.ToString();

            StartBtn.interactable = true;
        }
    }

    public void ClosePanel()
    {
        InGameMgr.Instance.state = State.Camp;
        FencePanel.SetActive(false);

        if (isLeveling)
        {
            ItemManager.Instance.AddItem(fenceData.NeedItemID, fenceData.Amount);
            StopCoroutine("LevelupCo");
        }
    }

    void FenceLevelUp()
    {
        if (ItemManager.Instance.GetItemInfo(fenceData.NeedItemID).num < fenceData.Amount)
            return;

        isLeveling = true;

        FenceInfo.SetActive(false);
        FenceLevelup.SetActive(true);
        CancelBtn.gameObject.SetActive(true);
        CompleteBtn.gameObject.SetActive(false);

        ItemManager.Instance.AddItem(fenceData.NeedItemID, -fenceData.Amount);

        timer = 0f;
        LevelingTxt.text = LoadGameData.Instance.GetString("Fence_a2");
        GuageFillImg.fillAmount = 0f;

        StartCoroutine("LevelupCo");
    }

    IEnumerator LevelupCo()
    {
        while (true)
        {
            timer += Time.deltaTime;
            GuageFillImg.fillAmount = timer / fenceData.NeedTime;
            if (timer >= fenceData.NeedTime)
            {
                GuageFillImg.fillAmount = 1;
                break;
            }

            yield return null;
        }

        CompleteLevelup();
    }

    void CompleteLevelup()
    {
        isLeveling = false;
        CancelBtn.gameObject.SetActive(false);
        CompleteBtn.gameObject.SetActive(true);
        LevelingTxt.text = LoadGameData.Instance.GetString("Fence_a3");

        fenceLevel++;
        foreach (var data in LoadGameData.Instance.fenceDatas)
        {
            if (data.Value.FenceGroupID == InGameMgr.Instance.stageData.FenceGroupID &&
                data.Value.FenceLevel == fenceLevel)
            {
                fenceData = data.Value;
                break;
            }
        }

        FenceObject();
    }

    public void Invade()
    {
        InvadeData invadeData = LoadGameData.Instance.invadeDatas[Clock.Instance.stageDayData.InvadeID];

        string msg = "";
        if (CheckInvade())
        {
            msg = LoadGameData.Instance.GetString(invadeData.InvadeStringID);

            List<int> temp = new List<int>() { 1, 2, 3, 4 };
            for (int i = 0; i < invadeData.StealObject; i++)
            {
                int rand = Random.Range(0, temp.Count);

                string itemCode = "";
                int itemCnt = 0;
                switch (temp[rand])
                {
                    case 1:
                        itemCode = invadeData.ItemID1;
                        itemCnt = Random.Range(invadeData.RandMin1, invadeData.RandMax1 + 1);
                        break;
                    case 2:
                        itemCode = invadeData.ItemID2;
                        itemCnt = Random.Range(invadeData.RandMin2, invadeData.RandMax2 + 1);
                        break;
                    case 3:
                        itemCode = invadeData.ItemID3;
                        itemCnt = Random.Range(invadeData.RandMin3, invadeData.RandMax3 + 1);
                        break;
                    case 4:
                        itemCode = invadeData.ItemID4;
                        itemCnt = Random.Range(invadeData.RandMin4, invadeData.RandMax4 + 1);
                        break;
                }
                ItemManager.Instance.AddItem(itemCode, -itemCnt);

                temp.RemoveAt(rand);
            }

            //연출 추가

        }
        else
        {
            msg = LoadGameData.Instance.GetString("Invade_a3");

            //연출 추가

        }

        Notice.Instance.InstNoticeText(msg);
    }

    bool CheckInvade()
    {
        float rand = Random.Range(0f, 0.99f);
        if (rand < fenceData.InvadeProb)
            return true;
        else
            return false;
    }

    void FenceObject()
    {
        switch (fenceLevel)
        {
            case 1:
                FenceLv1();
                break;
            case 2:
                FenceLv2();
                break;
            case 3:
                FenceLv3();
                break;
            case 4:
                FenceLv4();
                break;
            case 5:
                FenceLv5();
                break;
        }
    }

    void FenceLv1()
    {
        fenceLv1.SetActive(true);
        fenceLv2.SetActive(false);
        fenceLv3.SetActive(false);
        fenceLv4.SetActive(false);
    }

    void FenceLv2()
    {
        fenceLv1.SetActive(false);
        fenceLv2.SetActive(true);
        fenceLv3.SetActive(false);
        fenceLv4.SetActive(false);
    }

    void FenceLv3()
    {
        fenceLv1.SetActive(false);
        fenceLv2.SetActive(false);
        fenceLv3.SetActive(true);
        fenceLv4.SetActive(false);
    }

    void FenceLv4()
    {
        fenceLv1.SetActive(false);
        fenceLv2.SetActive(false);
        fenceLv3.SetActive(false);
        fenceLv4.SetActive(true);
    }

    void FenceLv5()
    {
        fenceLv1.SetActive(false);
        fenceLv2.SetActive(false);
        fenceLv3.SetActive(false);
        fenceLv4.SetActive(false);
    }
}
