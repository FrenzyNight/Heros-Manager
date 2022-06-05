using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ProductMgr : Singleton<ProductMgr>
{
    public float timeInterval;
    public float moveTime;
    public float fadeoutTime;

    public GameObject ClockUI;
    public GameObject JewelUI;
    public GameObject[] HeroUIs;
    public GameObject NoticeUI;
    public GameObject FenceButtonUI;
    public GameObject GatheringButtonUI;
    public GameObject EventButtonUI;
    public GameObject ItemUI;
    public GameObject BlackPanel;

    public GameObject MainCam;


    public void UIOutProduct()
    {
        StartCoroutine(OutCoroutine());
    }

    public void UIInProduct()
    {
        StartCoroutine(InCouroutine());
    }
    

    IEnumerator OutCoroutine()
    {
        //step1
        HeroUIs[0].transform.DOLocalMoveX(220,moveTime).SetUpdate(true).SetEase(Ease.OutQuad);
        FenceButtonUI.GetComponent<RectTransform>().DOAnchorPosY(-97.5f,moveTime).SetUpdate(true).SetEase(Ease.OutQuad);
        EventButtonUI.GetComponent<RectTransform>().DOAnchorPosY(-97.5f,moveTime).SetUpdate(true).SetEase(Ease.OutQuad);

        yield return new WaitForSecondsRealtime(timeInterval);

        //step2
        HeroUIs[1].transform.DOLocalMoveX(220,moveTime).SetUpdate(true).SetEase(Ease.OutQuad);
        //Debug.Log(ClockUI.GetComponent<RectTransform>().anchoredPosition.y);

        ClockUI.GetComponent<RectTransform>().DOAnchorPosY(188.5f, moveTime).SetUpdate(true).SetEase(Ease.OutQuad);
        
        NoticeUI.GetComponent<RectTransform>().DOAnchorPosY(-88,moveTime).SetUpdate(true).SetEase(Ease.OutQuad);
        ItemUI.GetComponent<RectTransform>().DOAnchorPosY(-66,moveTime).SetUpdate(true).SetEase(Ease.OutQuad);


        yield return new WaitForSecondsRealtime(timeInterval);

        //step3
        HeroUIs[2].transform.DOLocalMoveX(220,moveTime).SetUpdate(true).SetEase(Ease.OutQuad);
        JewelUI.GetComponent<RectTransform>().DOAnchorPosY(50,moveTime).SetUpdate(true).SetEase(Ease.OutQuad);
        GatheringButtonUI.GetComponent<RectTransform>().DOAnchorPosY(-97.5f,moveTime).SetUpdate(true).SetEase(Ease.OutQuad);

        yield return new WaitForSecondsRealtime(timeInterval);
        //setp4

        HeroUIs[3].transform.DOLocalMoveX(220,moveTime).SetUpdate(true).SetEase(Ease.OutQuad).OnComplete(() =>
        {
            BlackPanel.SetActive(true);
            MainCam.GetComponent<AudioSource>().DOFade(0, fadeoutTime).SetUpdate(true);
            BlackPanel.GetComponent<Image>().DOFade(1, fadeoutTime).SetUpdate(true).OnComplete(() =>
            {
                Clock.Instance.NextDay();
            });
        });

        //blackOut

        
        yield return null;
    }

    IEnumerator InCouroutine()
    {
        
        //step1
        HeroUIs[0].transform.DOLocalMoveX(30,moveTime).SetUpdate(true).SetEase(Ease.OutQuad);
        FenceButtonUI.GetComponent<RectTransform>().DOAnchorPosY(97.5f,moveTime).SetUpdate(true).SetEase(Ease.OutQuad);
        EventButtonUI.GetComponent<RectTransform>().DOAnchorPosY(97.5f,moveTime).SetUpdate(true).SetEase(Ease.OutQuad);

        yield return new WaitForSecondsRealtime(timeInterval);
        //step2
        HeroUIs[1].transform.DOLocalMoveX(30,moveTime).SetUpdate(true).SetEase(Ease.OutQuad);
        ClockUI.GetComponent<RectTransform>().DOAnchorPosY(-188.5f,moveTime).SetUpdate(true).SetEase(Ease.OutQuad);
        NoticeUI.GetComponent<RectTransform>().DOAnchorPosY(44,moveTime).SetUpdate(true).SetEase(Ease.OutQuad);
        ItemUI.GetComponent<RectTransform>().DOAnchorPosY(66,moveTime).SetUpdate(true).SetEase(Ease.OutQuad);

        yield return new WaitForSecondsRealtime(timeInterval);
        //step3
        HeroUIs[2].transform.DOLocalMoveX(30,moveTime).SetUpdate(true).SetEase(Ease.OutQuad);
        JewelUI.GetComponent<RectTransform>().DOAnchorPosY(-50,moveTime).SetUpdate(true).SetEase(Ease.OutQuad);
        GatheringButtonUI.GetComponent<RectTransform>().DOAnchorPosY(97.5f,moveTime).SetUpdate(true).SetEase(Ease.OutQuad);

        yield return new WaitForSecondsRealtime(timeInterval);
        //setp4
        HeroUIs[3].transform.DOLocalMoveX(30,moveTime).SetUpdate(true).SetEase(Ease.OutQuad);
        
        yield return null;
    }
}