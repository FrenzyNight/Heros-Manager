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
    public Text TouchTxt;
    public Transform MenuTrans;
    public Transform SelImg;
    public AudioSource audioSource;

    public AudioClip touchSound;
    public AudioClip mouseSound;

    bool isStart = false;
    bool isEff = false;
    public GameObject cam;
    public GameObject panel;

    public OptionManager optionMgr;
    public WarningManager warningMgr;

    void Start()
    {
        LoadGameData.Instance.LoadCSVDatas();
        SaveDataManager.Instance.LoadDatas();

        audioSource = gameObject.GetComponent<AudioSource>();

        TouchTxt.gameObject.SetActive(false);
        TouchTxt.DOFade(1, 1f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.Linear).SetDelay(2.5f).OnStart(() =>
        {
            isStart = true;
            TouchTxt.gameObject.SetActive(true);
        });
        
        //시연용 bgm 컨트롤
        /*
        isEff = true;
        Time.timeScale = 0;
        */
    }

    private void Update()
    {
        //시연
        /*
        if(Input.GetKeyDown(KeyCode.Space))
        {
            cam.GetComponent<AudioSource>().Play();
            Time.timeScale = 1;
            isEff = false;
        }
        */

        if (Input.anyKeyDown)
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

        TouchTxt.DOKill();
        TouchTxt.DOFade(0, 0.5f).SetEase(Ease.Linear).OnComplete(() =>
        {
            TouchTxt.gameObject.SetActive(false);
        });
        LogoImg.DOFade(0, 1.5f).SetEase(Ease.Linear).OnComplete(() =>
        {
            LogoImg.transform.localPosition = new Vector2(650, 200);
            EffImg.DOFade(1, 0.5f).SetEase(Ease.Linear);
            LogoImg.DOFade(1, 1f).SetEase(Ease.Linear);
            LogoImg.transform.DOLocalMoveX(750, 1f).From().SetEase(Ease.OutQuad).OnComplete(() =>
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
            MenuTrans.GetChild(idx).GetComponent<Text>().DOFade(0, 1f).SetDelay(delay).From();
            MenuTrans.GetChild(idx).DOLocalMoveX(70, 1f).From().SetEase(Ease.OutQuad).SetDelay(delay).OnStart(() =>
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
                            SaveDataManager.Instance.isContinue = false;
                            Production("Intro");
                        });
                        break;

                    case 1: //이어하기
                        menuBtn.onClick.AddListener(() =>
                        {
                            SaveDataManager.Instance.isContinue = true;
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
            });
        }

        if (!SaveDataManager.Instance.isSaveStage)
            MenuTrans.GetChild(1).GetComponent<Button>().interactable = false;
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

    void Production(string _scName)
    {
        panel.SetActive(true);
        panel.GetComponent<Image>().DOFade(1,1.5f).OnComplete(() =>
        {
            GameStartBtnEvent(_scName);
        });
    }

    void GameStartBtnEvent(string _scName)
    {
        SceneManager.LoadScene(_scName);
    }
}
