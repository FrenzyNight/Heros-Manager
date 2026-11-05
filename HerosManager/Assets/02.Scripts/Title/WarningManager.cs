using UnityEngine;
using UnityEngine.UI;

public class WarningManager : MonoBehaviour
{
    public GameObject BackPanel;
    public Text TitleText;
    public Text NoticeText;
    public Button OkBtn;
    public Button CloseBtn1;
    public Button CloseBtn2;

    public TitleManager titleMgr;

    void Start()
    {
        OkBtn.onClick.AddListener(OkBtnEvent);
        CloseBtn1.onClick.AddListener(CloseBtnEvent);
        CloseBtn2.onClick.AddListener(CloseBtnEvent);

        TitleText.text = LoadGameData.Instance.GetString("Title_t6");
        NoticeText.text = LoadGameData.Instance.GetString("Title_a2");
        OkBtn.GetComponentInChildren<Text>().text = LoadGameData.Instance.GetString("Title_t7");
        CloseBtn1.GetComponentInChildren<Text>().text = LoadGameData.Instance.GetString("Title_t8");
    }

    public void OpenPanel()
    {
        BackPanel.SetActive(true);
    }

    void OkBtnEvent()
    {
        var saves = SaveDataManager.Instance;
        if (saves.TrySaveGameSnapshot(saves.CreateNewGameData()))
            titleMgr.Production("Intro");
    }

    void CloseBtnEvent()
    {
        BackPanel.SetActive(false);
    }

    private void OnDestroy()
    {
        if (OkBtn != null) OkBtn.onClick.RemoveListener(OkBtnEvent);
        if (CloseBtn1 != null) CloseBtn1.onClick.RemoveListener(CloseBtnEvent);
        if (CloseBtn2 != null) CloseBtn2.onClick.RemoveListener(CloseBtnEvent);
    }

}
