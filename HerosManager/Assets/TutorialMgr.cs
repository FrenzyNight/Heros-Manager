using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialMgr : Singleton<TutorialMgr>
{
    [Header("Tutorial UI")] 
    public GameObject tutorialPanelBack;
    public GameObject tutorialDialogPanel;
    public GameObject tutorialWindowPanel;

    public void OpenTutorial(string openID)
    {
        tutorialPanelBack.SetActive(true);
        
        // 
    }

    public void OpenDialog()
    {
        tutorialDialogPanel.SetActive(true);
    }

    public void OpenWindow()
    {
        tutorialWindowPanel.SetActive(true);
    }
}
