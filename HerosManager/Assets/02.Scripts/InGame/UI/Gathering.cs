using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Gathering : MonoBehaviour
{
    public Image MiniGameImg;
    public Text MiniGameText;

    public Transform ItemImgTrans;

    public void Setup()
    {
        //MiniGameImg.sprite = ;
        MiniGameText.text = "";



        this.GetComponent<Button>().onClick.AddListener(StartMiniGame);
    }

    public void StartMiniGame()
    {
        GatheringManager.Instance.CloseGatheringPannel();

        MiniGameMgr.Instance.Setup();
    }
}
