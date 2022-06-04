using UnityEngine;
using UnityEngine.UI;

public class OptionManager : MonoBehaviour
{
    public GameObject BackPanel;
    public Text TitleText;
    public Button OkBtn;
    public Button CloseBtn;
    [Header("Window")]
    public Text WindowText;
    public Button FullBtn;
    public Button WinBtn;
    public Transform SelTrans;

    [Header("Sound")]
    public Text SoundText;
    public Text BgmText;
    public Text SfxText;
    public Slider BgmSlider;
    public Slider SfxSlider;

    void Start()
    {
        FullBtn.onClick.AddListener(FullBtnEvent);
        WinBtn.onClick.AddListener(WinBtnEvent);
        OkBtn.onClick.AddListener(OkBtnEvent);
        CloseBtn.onClick.AddListener(CloseBtnEvent);

        TitleText.text = LoadGameData.Instance.GetString("Title_t4");
        OkBtn.GetComponentInChildren<Text>().text = LoadGameData.Instance.GetString("Title_t16");
        WindowText.text = LoadGameData.Instance.GetString("Title_t10");
        FullBtn.GetComponentInChildren<Text>().text = LoadGameData.Instance.GetString("Title_t11");
        WinBtn.GetComponentInChildren<Text>().text = LoadGameData.Instance.GetString("Title_t12");
        SoundText.text = LoadGameData.Instance.GetString("Title_t13");
        BgmText.text = LoadGameData.Instance.GetString("Title_t14");
        SfxText.text = LoadGameData.Instance.GetString("Title_t15");
    }

    public void OpenPanel()
    {
        BackPanel.SetActive(true);
    }

    void FullBtnEvent()
    {
        SelTrans.position = FullBtn.transform.position;
    }

    void WinBtnEvent()
    {
        SelTrans.position = WinBtn.transform.position;
    }

    void OkBtnEvent()
    {
        BackPanel.SetActive(false);
    }

    void CloseBtnEvent()
    {
        BackPanel.SetActive(false);
    }
}
