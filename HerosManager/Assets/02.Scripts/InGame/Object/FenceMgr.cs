using UnityEngine;

public class FenceMgr : MonoBehaviour
{
    public GameObject fenceLv1;
    public GameObject fenceLv2;
    public GameObject fenceLv3;
    public GameObject fenceLv4;
    int fenceLevel = 0;

    FenceData fenceData;

    void Start()
    {
        InGameMgr.Instance.NextStageAct += LoadStageFence;
        Clock.Instance.NextDayAct += Invade;
    }

    void LoadStageFence()
    {
        fenceLevel = 0;
        foreach (var data in LoadGameData.Instance.fenceDatas)
        {
            if (data.Value.StageID == InGameMgr.Instance.stageData.StageID)
            {
                if (data.Value.FenceLevel == fenceLevel)
                {
                    fenceData = data.Value;
                    break;
                }
            }
        }


    }

    void CheckFenceLevelUp()
    {
        if (ItemManager.Instance.GetItemInfo(fenceData.NeedItemID).num < fenceData.Amount)
            return;

        if (fenceLevel >= 4)
            return;
    }

    void FenceLevelUp()
    {
        ItemManager.Instance.AddItem(fenceData.NeedItemID, -fenceData.Amount);

        fenceLevel++;

        foreach (var data in LoadGameData.Instance.fenceDatas)
        {
            if (data.Value.StageID == InGameMgr.Instance.stageData.StageID)
            {
                if (data.Value.FenceLevel == fenceLevel)
                {
                    fenceData = data.Value;
                    break;
                }
            }
        }

        switch (fenceLevel)
        {
            case 1:
                break;
            case 2:
                break;
            case 3:
                break;
            case 4:
                break;
        }
    }

    public bool CheckInvade(InvadeData _invadeData)
    {

        return true;
    }

    [ContextMenu("FenceLv1")]
    void FenceLv1()
    {
        fenceLv1.SetActive(true);
        fenceLv2.SetActive(false);
        fenceLv3.SetActive(false);
        fenceLv4.SetActive(false);
    }

    [ContextMenu("FenceLv2")]
    void FenceLv2()
    {
        fenceLv1.SetActive(false);
        fenceLv2.SetActive(true);
        fenceLv3.SetActive(false);
        fenceLv4.SetActive(false);
    }

    [ContextMenu("FenceLv3")]
    void FenceLv3()
    {
        fenceLv1.SetActive(false);
        fenceLv2.SetActive(false);
        fenceLv3.SetActive(true);
        fenceLv4.SetActive(false);
    }

    [ContextMenu("FenceLv4")]
    void FenceLv4()
    {
        fenceLv1.SetActive(false);
        fenceLv2.SetActive(false);
        fenceLv3.SetActive(false);
        fenceLv4.SetActive(true);
    }
}
