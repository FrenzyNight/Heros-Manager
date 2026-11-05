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
    private Button vibrationButton;

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
        vibrationButton = Instantiate(OkBtn, OkBtn.transform.parent);
        vibrationButton.name = "Gamepad vibration";
        vibrationButton.onClick = new Button.ButtonClickedEvent();
        vibrationButton.onClick.AddListener(ToggleVibration);
        var vibrationRect = vibrationButton.GetComponent<RectTransform>();
        var confirmRect = OkBtn.GetComponent<RectTransform>();
        vibrationRect.anchoredPosition = confirmRect.anchoredPosition + new Vector2(-130, 0);
        confirmRect.anchoredPosition += new Vector2(130, 0);
        RefreshVibration();
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
        RefreshVibration();
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
        SaveAndClose();
    }

    void CloseBtnEvent()
    {
        SaveAndClose();
    }

    void SaveAndClose()
    {
        var saves = SaveDataManager.Instance;
        saves.saveOptionData.BgmVolume = Mathf.Clamp01(BgmSlider.value);
        saves.saveOptionData.EffVolume = Mathf.Clamp01(SfxSlider.value);
        if (saves.TrySaveOptionDatas())
            BackPanel.SetActive(false);
    }

    void ToggleVibration()
    {
        var options = SaveDataManager.Instance.saveOptionData;
        options.disableVibration = !options.disableVibration;
        GamepadInput.RumbleEnabled = !options.disableVibration;
        RefreshVibration();
    }

    void RefreshVibration()
    {
        if (vibrationButton != null)
            vibrationButton.GetComponentInChildren<Text>().text = SaveDataManager.Instance.saveOptionData.disableVibration
                ? "패드 진동: 끔" : "패드 진동: 켬";
    }

    void OnDestroy()
    {
        if (vibrationButton != null) vibrationButton.onClick.RemoveListener(ToggleVibration);
        if (FullBtn != null) FullBtn.onClick.RemoveListener(FullBtnEvent);
        if (WinBtn != null) WinBtn.onClick.RemoveListener(WinBtnEvent);
        if (OkBtn != null) OkBtn.onClick.RemoveListener(OkBtnEvent);
        if (CloseBtn != null) CloseBtn.onClick.RemoveListener(CloseBtnEvent);
    }
}
