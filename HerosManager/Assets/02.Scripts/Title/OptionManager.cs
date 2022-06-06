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

        Color color = new Color(255, 255, 255, 127) / 255;
        FullBtn.image.color = color;
        WinBtn.image.color = color;
        if (!SaveDataManager.Instance.saveOptionData.isWindow)
        {
            FullBtn.image.color = Color.white;
            SelTrans.position = FullBtn.transform.position;
        }
        else
        {
            WinBtn.image.color = Color.white;
            SelTrans.position = WinBtn.transform.position;
        }

        BgmSlider.value = SaveDataManager.Instance.saveOptionData.BgmVolume;
        SfxSlider.value = SaveDataManager.Instance.saveOptionData.EffVolume;
    }

    void FullBtnEvent()
    {
        SelTrans.position = FullBtn.transform.position;

        FullBtn.image.color = Color.white;
        WinBtn.image.color = new Color(255, 255, 255, 127) / 255;

        SaveDataManager.Instance.saveOptionData.isWindow = false;
        Screen.fullScreen = true;
    }

    void WinBtnEvent()
    {
        SelTrans.position = WinBtn.transform.position;

        FullBtn.image.color = new Color(255, 255, 255, 127) / 255;
        WinBtn.image.color = Color.white;

        SaveDataManager.Instance.saveOptionData.isWindow = true;
        Screen.SetResolution(1600, 900, FullScreenMode.Windowed);
    }

    void OkBtnEvent()
    {
        BackPanel.SetActive(false);

        SaveDataManager.Instance.saveOptionData.BgmVolume = BgmSlider.value;
        SaveDataManager.Instance.saveOptionData.EffVolume = SfxSlider.value;
        SaveDataManager.Instance.SaveOptionDatas();
    }

    void CloseBtnEvent()
    {
        BackPanel.SetActive(false);

        SaveDataManager.Instance.saveOptionData.BgmVolume = BgmSlider.value;
        SaveDataManager.Instance.saveOptionData.EffVolume = SfxSlider.value;
        SaveDataManager.Instance.SaveOptionDatas();
    }
}
