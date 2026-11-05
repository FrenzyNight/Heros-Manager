using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class TitleManager : MonoBehaviour
{
    public Image LogoImg;
    public Image EffImg;
    //public Text TouchTxt;
    public Image ClickAny;
    public Transform MenuTrans;
    public Transform SelImg;
    public AudioSource audioSource;
    public AudioSource bgmAudioSource;

    public AudioClip touchSound;
    public AudioClip mouseSound;

    bool isStart = false;
    bool isEff = false;
    bool hasSave;
    bool isLoadingScene;
    public bool MenuReady { get; private set; }
    public GameObject cam;
    public GameObject panel;

    public OptionManager optionMgr;
    public WarningManager warningMgr;

    void Start()
    {
        Time.timeScale = 1f;
        LoadGameData.Instance.LoadTitleDatas();
        hasSave = SaveDataManager.Instance.LoadGameDatas();
        SaveDataManager.Instance.LoadOptionDatas();

#if !UNITY_EDITOR
        StartCoroutine(SetResolution());
#endif

        audioSource = gameObject.GetComponent<AudioSource>();
        audioSource.volume = SaveDataManager.Instance.saveOptionData.EffVolume;
        bgmAudioSource.volume = SaveDataManager.Instance.saveOptionData.BgmVolume;

        //TouchTxt.gameObject.SetActive(false);
        ClickAny.gameObject.SetActive(false);
        ClickAny.DOFade(1, 1f).SetId(this).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.Linear).SetDelay(2.5f).OnStart(() =>
        {
            isStart = true;
            ClickAny.gameObject.SetActive(true);
        });
        
        //시연용 bgm 컨트롤
        /*
        isEff = true;
        Time.timeScale = 0;
        */
    }

    private void Update()
    {
        if (GamepadInput.Pressed(GamepadButton.Back))
        {
            if (optionMgr != null && optionMgr.BackPanel.activeInHierarchy)
                optionMgr.CloseBtn.onClick.Invoke();
            else if (warningMgr != null && warningMgr.BackPanel.activeInHierarchy)
                warningMgr.CloseBtn1.onClick.Invoke();
        }
        //시연
        /*
        if(Input.GetKeyDown(KeyCode.Space))
        {
            cam.GetComponent<AudioSource>().Play();
            Time.timeScale = 1;
            isEff = false;
        }
        */

        if (GamepadInput.KeyboardOrMousePressed || GamepadInput.Pressed(GamepadButton.Confirm))
        {
            if (!isEff && isStart)
            {
                TitleEff();
                isEff = true;
            }
        }
    }

    void TitleEff()
    {
        audioSource.clip = touchSound;
        audioSource.Play();

        ClickAny.DOKill();
        ClickAny.DOFade(0, 0.5f).SetId(this).SetEase(Ease.Linear).OnComplete(() =>
        {
            ClickAny.gameObject.SetActive(false);
        });


        LogoImg.DOFade(0, 1.5f).SetId(this).SetEase(Ease.Linear).OnComplete(() =>
        {
            LogoImg.transform.localPosition = new Vector2(650, 200);
            EffImg.DOFade(1, 0.5f).SetId(this).SetEase(Ease.Linear);
            LogoImg.DOFade(1, 1f).SetId(this).SetEase(Ease.Linear);
            LogoImg.transform.DOLocalMoveX(750, 1f).SetId(this).From().SetEase(Ease.OutQuad).OnComplete(() =>
            {
                Setup();
            });
        });
    }

    void Setup()
    {
        for (int i = 0; i < MenuTrans.childCount; i++)
        {
            int idx = i;
            float delay = 0.25f * (i + 1);
            MenuTrans.GetChild(idx).GetComponent<Text>().DOFade(0, 1f).SetId(this).SetDelay(delay).From();
            MenuTrans.GetChild(idx).DOLocalMoveX(70, 1f).SetId(this).From().SetEase(Ease.OutQuad).SetDelay(delay).OnStart(() =>
            {
                 MenuTrans.GetChild(idx).gameObject.SetActive(true);
            })
            .OnComplete(() =>
            {
                EventTrigger eventTrigger = MenuTrans.GetChild(idx).gameObject.AddComponent<EventTrigger>();

                EventTrigger.Entry entry_PointerEnter = new EventTrigger.Entry();
                entry_PointerEnter.eventID = EventTriggerType.PointerEnter;
                entry_PointerEnter.callback.AddListener((data) =>
                {
                   OnPointerEnterEvent((PointerEventData)data, idx);
                });
                eventTrigger.triggers.Add(entry_PointerEnter);

                EventTrigger.Entry entry_PointerExit = new EventTrigger.Entry();
                entry_PointerExit.eventID = EventTriggerType.PointerExit;
                entry_PointerExit.callback.AddListener((data) =>
                {
                    OnPointerExitEvent((PointerEventData)data, idx);
                });
                eventTrigger.triggers.Add(entry_PointerExit);

                Button menuBtn = MenuTrans.GetChild(idx).GetComponent<Button>();
                switch (idx)
                {
                    case 0: //여정시작
                        menuBtn.onClick.AddListener(() =>
                        {
                            if (hasSave && warningMgr != null)
                            {
                                warningMgr.OpenPanel();
                                return;
                            }
                            var saves = SaveDataManager.Instance;
                            if (saves.TrySaveGameSnapshot(saves.CreateNewGameData()))
                                Production("Intro");
                        });
                        break;

                    case 1: //이어하기
                        menuBtn.onClick.AddListener(() =>
                        {
                            if (SaveDataManager.Instance.LoadGameDatas())
                                Production("InGame");
                        });
                        break;

                    case 2: //도감
                        break;

                    case 3: //설정
                        menuBtn.onClick.AddListener(() =>
                        {
                            optionMgr.OpenPanel();
                        });
                        break;

                    case 4: //게임종료
                        menuBtn.onClick.AddListener(() =>
                        {
                            Application.Quit();
                        });
                        break;
                }
                if (idx == MenuTrans.childCount - 1) MenuReady = true;
            });
        }

        MenuTrans.GetChild(1).GetComponent<Button>().interactable = hasSave;
    }

    void OnPointerEnterEvent(PointerEventData data, int _idx)
    {
        audioSource.clip = mouseSound;
        audioSource.Play();
        SelImg.gameObject.SetActive(true);
        SelImg.transform.position = new Vector2(MenuTrans.GetChild(_idx).position.x + 20, MenuTrans.GetChild(_idx).position.y);
        MenuTrans.GetChild(_idx).GetComponent<Text>().color = Color.black;
    }

    void OnPointerExitEvent(PointerEventData data, int _idx)
    {
        SelImg.gameObject.SetActive(false);
        MenuTrans.GetChild(_idx).GetComponent<Text>().color = new Color(249, 248, 220, 255) / 255;
    }

    public void Production(string _scName)
    {
        if (isLoadingScene)
            return;
        isLoadingScene = true;
        Time.timeScale = 1f;
        panel.SetActive(true);
        panel.GetComponent<Image>().DOFade(1,1.5f).SetId(this).OnComplete(() =>
        {
            GameStartBtnEvent(_scName);
        });
    }

    void GameStartBtnEvent(string _scName)
    {
        SceneManager.LoadScene(_scName);
    }

    void OnDestroy()
    {
        DOTween.Kill(this);
        if (ClickAny != null) ClickAny.DOKill();
        if (LogoImg != null)
        {
            LogoImg.DOKill();
            LogoImg.transform.DOKill();
        }
        if (EffImg != null) EffImg.DOKill();
        if (MenuTrans != null)
            foreach (Transform child in MenuTrans)
            {
                child.DOKill();
                var label = child.GetComponent<Text>();
                if (label != null) label.DOKill();
            }
        if (panel != null)
        {
            var fade = panel.GetComponent<Image>();
            if (fade != null) fade.DOKill();
        }
    }

    IEnumerator SetResolution()
    {
        yield return null;

        if (!SaveDataManager.Instance.saveOptionData.isWindow)
        {
            Screen.fullScreen = true;
        }
        else
        {
            Screen.SetResolution(1600, 900, FullScreenMode.Windowed);
        }
    }
}
