﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CampInteractionMgr : MonoBehaviour
{
    public bool isActive;
    public bool isGrace;
    public float activeTime, graceTime;
    public GameObject CampObjMgr;
    public GameObject ActiveButton;
    public Image ActiveGage, GraceGage;
    public Text StringText;

    public Sprite Green, Red, Blue;
    // Start is called before the first frame update
   

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
