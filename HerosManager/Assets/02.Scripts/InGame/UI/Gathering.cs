using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Gathering : Singleton<Gathering>
{
    public GameObject GatheringPannel;

    public Button[] StageBtns;

    private void Awake()
    {
        this.GetComponent<Button>().onClick.AddListener(() => OpenGatheringPannel());

        StageBtns[0].onClick.AddListener(() =>
        {
            InGameMgr.Instance.EnterMiniGame("Stage_1_Forest");
        });
        StageBtns[1].onClick.AddListener(() =>
        {
            InGameMgr.Instance.EnterMiniGame("Stage_1_Grassland");
        });
        StageBtns[2].onClick.AddListener(() =>
        {
            InGameMgr.Instance.EnterMiniGame("Stage_1_river");
        });
    }

    public void OpenGatheringPannel()
    {
        GatheringPannel.SetActive(true);
        InGameMgr.Instance.state = State.Pannel;
        Notice.Instance.isLock = true;
    }

    public void CloseGatheringPannel()
    {
        GatheringPannel.SetActive(false);
        InGameMgr.Instance.state = State.Camp;
        Notice.Instance.isLock = false;
    }
}
