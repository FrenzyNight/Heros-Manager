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
    [HideInInspector] public List<TutorialData> tutoDatas = new List<TutorialData>();
    private IEnumerator _typingCo;
    private IEnumerator _soundCo;
    private bool previousClockStopped;
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
        StopAllCoroutines();
        _typingCo = null;
        if (!isTutorial)
            previousClockStopped = Clock.Instance.isStop;
        Clock.Instance.isStop = true;
        tutorialPanelBack.SetActive(true);
        idx = 0;
        isTutorial = true;
        //isDialogEnd = false;
        isWindowEnd = false;
        isDialog = false;
        
        if (tutoDatas == null)
            tutoDatas = new List<TutorialData>();
        tutoDatas.Clear();

        typingAs.clip = typingSound;
        vol = SaveDataManager.Instance.saveOptionData.EffVolume;
        typingAs.volume = SaveDataManager.Instance.saveOptionData.EffVolume;
        
        tutoDatas.AddRange(LoadGameData.Instance.Catalog.GetTutorials(openID));
        RunTutorial();
    }

    public void RunTutorial()
    {
        if (tutoDatas == null || idx < 0 || idx >= tutoDatas.Count)
        {
            CloseTutorial();
            return;
        }
        

        if (tutoDatas[idx].TutorialType == 1)
        {
            OpenDialog();
        }
        else if (tutoDatas[idx].TutorialType == 2)
        {
            OpenWindow();
        }
        else
        {
            Debug.LogWarning($"Unsupported tutorial type: {tutoDatas[idx].TutorialType}");
            CloseTutorial();
        }
        
        
    }

    public void OpenDialog()
    {
        tutorialWindowPanel.SetActive(false);
        tutorialDialogPanel.SetActive(true);
        isDialog = true;
        waitingArrow.SetActive(false);
        StopCoroutine("WatingEffCo");
        dialogText.text = string.Empty;
        _typingCo = TypingEff1Co(tutoDatas[idx].StringID);
        StartCoroutine(_typingCo);
    }

    public bool NextDialog()
    {
        if (tutoDatas == null || idx < 0 || idx >= tutoDatas.Count)
            return true;
        
        waitingArrow.SetActive(false);
        StopCoroutine("WatingEffCo");

        if (_typingCo != null)
        {
            StopCoroutine(_typingCo);
            _typingCo = null;
            dialogText.text = LoadGameData.Instance.GetString(tutoDatas[idx].StringID);
            StartCoroutine("WatingEffCo");

            return false;
        }

        idx++;
        return true;
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
        if (tutoDatas == null || _idx < 0 || _idx >= tutoDatas.Count)
        {
            CloseTutorial();
            return;
        }
        idx = _idx;
        prevBtn.onClick.RemoveAllListeners();
        nextBtn.onClick.RemoveAllListeners();
        nextBtn.gameObject.SetActive(false);
        exitBtn.gameObject.SetActive(false);
        isWindowEnd = false;
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
        if (_idx + 1 < tutoDatas.Count)
        {
            nextBtn.gameObject.SetActive(true);
            nextBtn.onClick.AddListener(() =>
            {
                idx = _idx + 1;
                RunTutorial();
            });
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
        if (!isTutorial)
            return;
        StopAllCoroutines();
        _typingCo = null;
        waitingArrow.SetActive(false);
        typingAs.Stop();
        tutorialDialogPanel.SetActive(false);
        tutorialWindowPanel.SetActive(false);
        tutorialPanelBack.SetActive(false);
        isTutorial = false;
        isDialog = false;
        isWindowEnd = false;
        Clock.Instance.isStop = previousClockStopped || InGameMgr.Instance.IsComplete;
        if (isCollect)
        {
            isCollect = false;
            GatheringManager.Instance.Setup();
        }
    }
    
    IEnumerator TypingEff1Co(string _strCode)
    {
        string str = LoadGameData.Instance.GetString(_strCode);
        // Ensure the coroutine handle is assigned even for an empty localized string.
        yield return null;

        for (int i = 0; i < str.Length; i++)
        {
            dialogText.text = str.Substring(0, i + 1);
            typingAs.Play();
            yield return new WaitForSecondsRealtime(0.1f);
        }

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
