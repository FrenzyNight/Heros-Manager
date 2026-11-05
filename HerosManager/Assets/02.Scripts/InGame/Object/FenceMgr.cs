using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FenceMgr : Singleton<FenceMgr>
{
    AudioSource audioSource;
    FenceData fenceData;
    public int fenceLevel = 1;
    bool isLeveling;
    InGameMgr stageOwner;
    ItemCost paidCost;
    float timer;
    public float charWaitTime;
    [HideInInspector]
    float charTimer;

    [Header("UI")]
    public Button FenceBtn;
    public GameObject FencePanel;
    public Image TitleCharImg;
    public Image TitleFenceImg;
    public Button ExitBtn;
    public GameObject FenceInfo;
    public GameObject FenceLevelup;
    public Button StartBtn;
    [Header("UI Img")]
    public Sprite[] TitleFenceImgs;
    public Sprite[] TitleCharImgs;
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
    //public Text NextNum;
    [Header("Time")]
    public Text TimeTxt;
    public Text PrevTime;
    //public Text NextTime;
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
    public GameObject fenceLv5;

    [HideInInspector]
    public bool isInvade;

    [HideInInspector] public bool isFirstFence;

    void Start()
    {
        stageOwner = InGameMgr.Instance;
        stageOwner.NextStageAct += LoadStageFence;
        fenceLevel = SaveDataManager.Instance.saveGameData.fenceLevel;
        audioSource = GetComponent<AudioSource>();
        isFirstFence = true;

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
        CancelUpgrade();
        LoadGameData.Instance.Catalog.TryGetFence(stageOwner.stageData.FenceGroupID, fenceLevel, out fenceData);

        if (fenceData == null)
        {
            Debug.LogError($"No fence data for stage {stageOwner.stage}, level {fenceLevel}.", this);
            return;
        }
        FenceObject();
    }

    void Setup()
    {
        if (fenceData == null || isLeveling)
            return;
        InGameMgr.Instance.state = State.Fence;
        FencePanel.SetActive(true);
        FenceInfo.SetActive(true);
        FenceLevelup.SetActive(false);

        isLeveling = false;

        PrevLevel.text = fenceData.FenceLevel.ToString();
        PrevRate.text = fenceData.InvadeProb.ToString("P0");
        PrevNum.text = fenceData.Amount.ToString();
        PrevTime.text = fenceData.NeedTime.ToString();

        TitleFenceImg.sprite = TitleFenceImgs[fenceLevel-1];
        TitleCharImg.sprite = TitleCharImgs[0];

        LoadGameData.Instance.Catalog.TryGetFence(stageOwner.stageData.FenceGroupID, fenceLevel + 1, out var NextFenceData);
        if (NextFenceData == null)
        {
            PrevNum.text = "-";
            PrevTime.text = "-";

            NextLevel.text = "-";
            NextRate.text = "-";
            //NextNum.text = "-";
            //NextTime.text = "-";

            StartBtn.interactable = false;
        }
        else
        {
            NextLevel.text = NextFenceData.FenceLevel.ToString();
            NextRate.text = NextFenceData.InvadeProb.ToString("P0");
            //NextNum.text = NextFenceData.Amount.ToString();
            //NextTime.text = NextFenceData.NeedTime.ToString();

            StartBtn.interactable = true;
        }

        if (isFirstFence)
        {
            isFirstFence = false;
            TutorialMgr.Instance.OpenTutorial("Open_Tutorial_Fence_1");
        }
    }

    public void ClosePanel()
    {
        if (InGameMgr.Instance.state == State.Fence)
            InGameMgr.Instance.state = State.Camp;
        FencePanel.SetActive(false);
        CancelUpgrade();
    }

    void CancelUpgrade()
    {
        if (!isLeveling)
            return;
        isLeveling = false;
        StopCoroutine("LevelupCo");
        audioSource.Stop();
        ItemManager.Instance.AddItem(paidCost.ItemId, paidCost.Amount);
        paidCost = default;
    }

    void FenceLevelUp()
    {
        if (isLeveling || fenceData == null || !HasNextLevel() ||
            !ItemManager.Instance.TrySpend(fenceData.NeedItemID, fenceData.Amount))
            return;

        isLeveling = true;
        paidCost = new ItemCost(fenceData.NeedItemID, fenceData.Amount);
        

        FenceInfo.SetActive(false);
        FenceLevelup.SetActive(true);
        CancelBtn.gameObject.SetActive(true);
        CompleteBtn.gameObject.SetActive(false);

        timer = 0f;
        charTimer = 0f;
        LevelingTxt.text = LoadGameData.Instance.GetString("Fence_a2");
        GuageFillImg.fillAmount = 0f;

        audioSource.Play();
        StartCoroutine("LevelupCo");
    }

    IEnumerator LevelupCo()
    {
        int idx = 0;
        int rnd = NewRandomNum(idx);
        TitleCharImg.sprite = TitleCharImgs[rnd];
        idx = rnd;
        while (true)
        {
            timer += Time.deltaTime;
            charTimer += Time.deltaTime;
            GuageFillImg.fillAmount = timer / fenceData.NeedTime;
            if (timer >= fenceData.NeedTime)
            {
                GuageFillImg.fillAmount = 1;
                break;
            }

            if(charTimer >= charWaitTime)
            {
                charTimer = 0;

                rnd = NewRandomNum(idx);
                TitleCharImg.sprite = TitleCharImgs[rnd];
                idx = rnd;
            }

            yield return null;
        }

        TitleCharImg.sprite = TitleCharImgs[0];

        CompleteLevelup();
    }

    int NewRandomNum(int prev)
    {
        int count = TitleCharImgs.Length - 1;
        if (count <= 0) return 0;
        if (count == 1) return 1;
        int index = Random.Range(1, TitleCharImgs.Length);
        if (index == prev)
            index = index % count + 1;
        return index;
    }

    bool HasNextLevel()
    {
        return LoadGameData.Instance.Catalog.TryGetFence(stageOwner.stageData.FenceGroupID, fenceLevel + 1, out _);
    }

    void CompleteLevelup()
    {
        if (!isLeveling)
            return;
        audioSource.Stop();
        isLeveling = false;
        paidCost = default;
        CancelBtn.gameObject.SetActive(false);
        CompleteBtn.gameObject.SetActive(true);
        LevelingTxt.text = LoadGameData.Instance.GetString("Fence_a3");

        fenceLevel++;
        TitleFenceImg.sprite = TitleFenceImgs[fenceLevel-1];
        LoadGameData.Instance.Catalog.TryGetFence(stageOwner.stageData.FenceGroupID, fenceLevel, out fenceData);

        FenceObject();
    }

    public void Invade()
    {
        InvadeData invadeData = LoadGameData.Instance.invadeDatas[Clock.Instance.stageDayData.InvadeID];

        string msg = "";
        isInvade = CheckInvade();
        if (isInvade)
        {
            msg = LoadGameData.Instance.GetString("Invade_a1");

            List<int> temp = new List<int>() { 1, 2, 3, 4 };
            for (int i = 0; i < invadeData.StealObject && temp.Count > 0; i++)
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
        return fenceData != null && Random.value < Mathf.Clamp01(fenceData.InvadeProb);
    }

    protected override void OnDestroy()
    {
        if (stageOwner != null)
            stageOwner.NextStageAct -= LoadStageFence;
        if (FenceBtn != null) FenceBtn.onClick.RemoveListener(Setup);
        if (ExitBtn != null) ExitBtn.onClick.RemoveListener(ClosePanel);
        if (StartBtn != null) StartBtn.onClick.RemoveListener(FenceLevelUp);
        if (CancelBtn != null) CancelBtn.onClick.RemoveListener(ClosePanel);
        if (CompleteBtn != null) CompleteBtn.onClick.RemoveListener(Setup);
        base.OnDestroy();
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
        fenceLv5.SetActive(false);
    }

    void FenceLv2()
    {
        fenceLv1.SetActive(false);
        fenceLv2.SetActive(true);
        fenceLv3.SetActive(false);
        fenceLv4.SetActive(false);
        fenceLv5.SetActive(false);
    }

    void FenceLv3()
    {
        fenceLv1.SetActive(false);
        fenceLv2.SetActive(false);
        fenceLv3.SetActive(true);
        fenceLv4.SetActive(false);
        fenceLv5.SetActive(false);
    }

    void FenceLv4()
    {
        fenceLv1.SetActive(false);
        fenceLv2.SetActive(false);
        fenceLv3.SetActive(false);
        fenceLv4.SetActive(true);
        fenceLv5.SetActive(false);
    }

    void FenceLv5()
    {
        fenceLv1.SetActive(false);
        fenceLv2.SetActive(false);
        fenceLv3.SetActive(false);
        fenceLv4.SetActive(false);
        fenceLv5.SetActive(true);
    }
}
