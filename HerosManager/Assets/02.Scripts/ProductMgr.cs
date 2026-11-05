using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ProductMgr : Singleton<ProductMgr>
{
    public float timeInterval, moveTime, fadeoutTime;
    public GameObject ClockUI, JewelUI;
    public GameObject[] HeroUIs;
    public GameObject NoticeUI, FenceButtonUI, GatheringButtonUI, EventButtonUI, ItemUI, BlackPanel, MainCam;

    private Sequence transition;
    private bool leaving;

    public void UIOutProduct()
    {
        if (leaving) return;
        leaving = true;
        BeginTransition(false);
        transition.AppendCallback(() => BlackPanel.SetActive(true));
        transition.Append(MainCam.GetComponent<AudioSource>().DOFade(0, Mathf.Max(0, fadeoutTime)));
        transition.Join(BlackPanel.GetComponent<Image>().DOFade(1, Mathf.Max(0, fadeoutTime)));
        transition.OnComplete(() => Clock.Instance.NextDay());
    }

    public void UIInProduct()
    {
        leaving = false;
        BeginTransition(true);
    }

    private void BeginTransition(bool incoming)
    {
        transition?.Kill();
        transition = DOTween.Sequence().SetUpdate(true);
        float duration = Mathf.Max(0, moveTime);
        float interval = Mathf.Max(0, timeInterval);
        for (int i = 0; i < HeroUIs.Length; i++)
            transition.Insert(interval * i, HeroUIs[i].transform.DOLocalMoveX(incoming ? 30 : 220, duration).SetEase(Ease.OutQuad));
        InsertY(FenceButtonUI, incoming ? 97.5f : -97.5f, 0, duration);
        InsertY(EventButtonUI, incoming ? 97.5f : -97.5f, 0, duration);
        InsertY(ClockUI, incoming ? -188.5f : 188.5f, interval, duration);
        InsertY(NoticeUI, incoming ? 44 : -88, interval, duration);
        InsertY(ItemUI, incoming ? 66 : -66, interval, duration);
        InsertY(JewelUI, incoming ? -75 : 75, interval * 2, duration);
        InsertY(GatheringButtonUI, incoming ? 97.5f : -97.5f, interval * 2, duration);
    }

    private void InsertY(GameObject target, float y, float delay, float duration)
    {
        transition.Insert(delay, target.GetComponent<RectTransform>().DOAnchorPosY(y, duration).SetEase(Ease.OutQuad));
    }

    private void OnDisable()
    {
        transition?.Kill();
        transition = null;
        leaving = false;
    }
}
