using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GatheringManager : Singleton<GatheringManager>
{
    public Button GatheringBtn;
    public Button ExitBtn;

    public GameObject GatheringPanel;
    public Text NoticeText;
    public Transform MiniGameTrans;
    public GameObject MiniGameBtn;

    void Start()
    {
        GatheringBtn.onClick.AddListener(Setup);
        ExitBtn.onClick.AddListener(CloseGatheringPannel);

        NoticeText.text = LoadGameData.Instance.GetString("");
    }

    void Setup()
    {
        InGameMgr.Instance.state = State.Gathering;

        GatheringPanel.SetActive(true);

        //채집 테이블 불러와서 -> 미니게임
        for (int i = 0; i < MiniGameTrans.childCount; i++)
        {
            Destroy(MiniGameTrans.GetChild(i).gameObject);
        }

        for (int i = 0; i < 3; i++)
        {
            GameObject obj = Instantiate(MiniGameBtn, MiniGameTrans);
            obj.GetComponent<Gathering>().Setup();
        }
    }

    public void CloseGatheringPannel()
    {
        InGameMgr.Instance.state = State.Camp;

        GatheringPanel.SetActive(false);
    }
}
