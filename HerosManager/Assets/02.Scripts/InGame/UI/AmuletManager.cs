using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class AmuletManager : Singleton<AmuletManager>
{
    public GameObject AmuletNotice;
    public Text NoticeText;

    public GameObject AmuletObj;

    public Button JewelButton;
    public GameObject JewelList;
    public bool isSlide;

    public List<Amulet> JewelInven = new List<Amulet>();    

    void Start()
    {
        Setup();
    }

    void Setup()
    {
        isSlide = true;
        JewelListSlide();
    }

    void SpawnAmulet()
    {
        GameObject obj = Instantiate(AmuletObj, this.transform);
        obj.GetComponent<Amulet>().Setup();
    }

    public void JewelListSlide()
    {
        if(isSlide)
        {
            JewelList.GetComponent<RectTransform>().DOAnchorPosX(-485, 1f).SetUpdate(true).SetEase(Ease.OutQuad);
        }
        else
        {
            JewelList.GetComponent<RectTransform>().DOAnchorPosX(485, 1f).SetUpdate(true).SetEase(Ease.OutQuad);
        }
        isSlide = !isSlide;
    }

    public void CloseList()
    {
        isSlide = true;
        JewelListSlide();
    }
}
