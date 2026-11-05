using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CampInteractionMgr : MonoBehaviour
{
    [Header("InterObject")]
    public bool isActive;
    public bool isGrace;
    public float activeTime, graceTime;
    public GameObject CampObjMgr;
    public GameObject ActiveButton;
    public Image ActiveGage, GraceGage;
    public Text StringText;
    public Sprite Green, Red, Blue;
    InGameMgr stageOwner;

    protected virtual void OnEnable()
    {
        stageOwner = InGameMgr.Instance;
        if (stageOwner != null)
            stageOwner.NextStageAct += RefreshStage;
    }

    protected virtual void OnDisable()
    {
        if (stageOwner != null)
            stageOwner.NextStageAct -= RefreshStage;
        stageOwner = null;
    }

    void RefreshStage()
    {
        StopAllCoroutines();
        isActive = false;
        isGrace = false;
        activeTime = 0f;
        graceTime = 0f;
        Setup();
    }
    
    public void AvailableButton()
    {
        ActiveButton.GetComponent<Button>().interactable = true;
    }

    public void UnavailableButton()
    {
        ActiveButton.GetComponent<Button>().interactable = false;
    }

    public virtual void ClickButton() {}
    public virtual void Setup() {}
    public virtual void NextDayAction() {}
}
