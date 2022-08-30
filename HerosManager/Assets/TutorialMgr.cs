using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class TutorialMgr : Singleton<TutorialMgr>
{
    [Header("Tutorial UI")] 
    public GameObject tutorialPanelBack;
    public GameObject tutorialDialogPanel;
    public GameObject tutorialWindowPanel;
    public AudioSource audioSource;
    public AudioSource typingAs;
    public AudioClip typingSound;
    public bool isTutorial;
    [HideInInspector] public int idx;
    [HideInInspector] public List<TutorialData> tutoDatas;
    private IEnumerator _typingCo;
    private IEnumerator _soundCo;
    [HideInInspector] public float vol;

    [Header("Dialog")] 
    public Text dialogText;
    public GameObject waitingArrow;
    [HideInInspector] public bool isDialog;
    //[HideInInspector] public bool isDialogEnd;

    [Header("Window")] 
    public Text windowText;
    public Image windowImg;
    public Button prevBtn;
    public Button nextBtn;
    public Button exitBtn;
    [HideInInspector] public bool isWindowEnd;
    [HideInInspector] public bool isCollect;
    private void Update()
    {
        if (isDialog && Input.anyKeyDown)
        {
            bool isDialogEnd = NextDialog();
            if (isDialogEnd)
            {
                RunTutorial();
            }
        }

        if (!isTutorial && tutorialPanelBack.activeSelf)
        {
            tutorialPanelBack.SetActive(false);
        }
    }


    public void OpenTutorial(string openID)
    {
        Clock.Instance.isStop = true;
        tutorialPanelBack.SetActive(true);
        idx = 0;
        isTutorial = true;
        //isDialogEnd = false;
        isWindowEnd = false;
        isDialog = false;
        
        tutoDatas.Clear();

        typingAs.clip = typingSound;
        vol = SaveDataManager.Instance.saveOptionData.EffVolume;
        typingAs.volume = SaveDataManager.Instance.saveOptionData.EffVolume;
        
        foreach (var obj in LoadGameData.Instance.tutorialDatas)
        {
            if (openID == obj.Value.OpenID)
            {
                tutoDatas.Add(obj.Value);
            }
        }
        
        RunTutorial();
    }

    public void RunTutorial()
    {
        if (tutoDatas == null)
            return;

        
        if (idx + 1 == tutoDatas.Count)
        {
            CloseTutorial();
            return;
        }
        

        if (tutoDatas[idx].TutorialType == 1)
        {
            OpenDialog();
            NextDialog();
        }
        else if (tutoDatas[idx].TutorialType == 2)
        {
            OpenWindow();
        }
        
        
    }

    public void OpenDialog()
    {
        tutorialDialogPanel.SetActive(true);
        isDialog = true;
    }

    public bool NextDialog()
    {
        if (_typingCo == null && (tutoDatas[idx].TutorialType != 1||idx+1 == tutoDatas.Count))
            return true;
        
        waitingArrow.SetActive(false);
        StopCoroutine("WatingEffCo");

        if (_typingCo != null)
        {
            StopCoroutine(_typingCo);
            _typingCo = null;
            dialogText.text = LoadGameData.Instance.GetString(tutoDatas[idx].StringID);
            idx++;

            StartCoroutine("WatingEffCo");

            return false;
        }

        _typingCo = TypingEff1Co(tutoDatas[idx].StringID);
        StartCoroutine(_typingCo);

        return false;
    }

    public void OpenWindow()
    {
        tutorialDialogPanel.SetActive(false);
        tutorialWindowPanel.SetActive(true);
        isDialog = false;
        prevBtn.gameObject.SetActive(false);
        nextBtn.gameObject.SetActive(false);
        exitBtn.gameObject.SetActive(false);
        SetWindow(idx);
    }

    public void SetWindow(int _idx)
    {
        windowImg.sprite = Resources.Load<Sprite>("TutorialImage/" + tutoDatas[_idx].ImageID);
        windowText.text = LoadGameData.Instance.GetString(tutoDatas[_idx].StringID);
        
        //set prevBtn
        if (_idx != 0 && tutoDatas[_idx - 1].TutorialType == 2)
        {
            prevBtn.gameObject.SetActive(true);
            prevBtn.onClick.RemoveAllListeners();
            prevBtn.onClick.AddListener(()=>SetWindow(_idx-1));
        }
        else
        {
            prevBtn.gameObject.SetActive(false);
        }
        
        //set nextBtn
        if (_idx+1 != tutoDatas.Count) 
        {
            if (tutoDatas[_idx + 1].TutorialType == 2)
            {
                nextBtn.gameObject.SetActive(true);
                nextBtn.onClick.RemoveAllListeners();
                nextBtn.onClick.AddListener(() => SetWindow(_idx + 1));
            }
        }
        else
        {
            nextBtn.gameObject.SetActive(false);
            isWindowEnd = true;
            exitBtn.gameObject.SetActive(true);
        }
    }

    public void CloseTutorial()
    {
        tutorialDialogPanel.SetActive(false);
        tutorialWindowPanel.SetActive(false);
        tutorialPanelBack.SetActive(false);
        isTutorial = false;
        isDialog = false;
        isWindowEnd = false;
        Clock.Instance.isStop = false;
        if (isCollect)
        {
            isCollect = false;
            GatheringManager.Instance.Setup();
        }
    }
    
    IEnumerator TypingEff1Co(string _strCode)
    {
        string str = LoadGameData.Instance.GetString(_strCode);

        //yield return new WaitForSecondsRealtime(_delay);

        for (int i = 0; i < str.Length; i++)
        {
            dialogText.text = str.Substring(0, i + 1);
            typingAs.Play();
            yield return new WaitForSecondsRealtime(0.1f);
        }

        idx++;
        _typingCo = null;

        StartCoroutine("WatingEffCo");
    }
    
    
    IEnumerator WatingEffCo()
    {
        while (true)
        {
            waitingArrow.SetActive(true);

            yield return new WaitForSecondsRealtime(0.5f);

            waitingArrow.SetActive(false);

            yield return new WaitForSecondsRealtime(0.5f);
        }
    }
}
