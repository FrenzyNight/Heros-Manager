using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialMgr : Singleton<TutorialMgr>
{
    [Header("Tutorial UI")] 
    public GameObject tutorialPanelBack;
    public GameObject tutorialDialogPanel;
    public GameObject tutorialWindowPanel;
    [HideInInspector] public List<TutorialData> tutoDatas;

    public void OpenTutorial(string openID)
    {
        tutorialPanelBack.SetActive(true);

        tutoDatas.Clear();
        
        foreach (var obj in LoadGameData.Instance.tutorialDatas)
        {
            if (openID == obj.Key)
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
