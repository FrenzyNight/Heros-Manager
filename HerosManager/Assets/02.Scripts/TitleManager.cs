using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class TitleManager : MonoBehaviour
{
    public Transform Shield;
    public Transform Knife1;
    public Transform Knife2;

    public Transform MenuTrans;
    public Transform CheckKnife;

    void Start()
    {
        LoadGameData.Instance.LoadCSVDatas();
        StartCoroutine(TitleEffCo());
        //TitleEff();
        //SceneManager.LoadScene("InGame");
    }

    void TitleEff()
    {
        Shield.DOScale(3, 0.5f).From().SetEase(Ease.OutBack).OnStart(() =>
        {
            Shield.gameObject.SetActive(true);
        });
        Vector3 startVec1 = Knife1.localPosition + new Vector3(-300, 300);
        Knife1.DOLocalMove(startVec1, 0.5f).SetDelay(0.2f).From().SetEase(Ease.OutBack).OnStart(() =>
        {
            Knife1.gameObject.SetActive(true);
        });
        Vector3 startVec2 = Knife2.localPosition + new Vector3(300, 300);
        Knife2.DOLocalMove(startVec2, 0.5f).SetDelay(0.2f).From().SetEase(Ease.OutBack).OnStart(() =>
        {
            Knife2.gameObject.SetActive(true);
        });

        for (int i = 0; i < MenuTrans.childCount; i++)
        {
            int idx = i;
            float delay = 0.7f + (0.2f * i);
            MenuTrans.GetChild(idx).DOLocalMoveY(500, 1.5f).SetDelay(delay).From().SetEase(Ease.OutBack).OnStart(() =>
            {
                MenuTrans.GetChild(idx).gameObject.SetActive(true);
            })
            .OnComplete(() =>
            {
                EventTrigger eventTrigger = MenuTrans.GetChild(idx).gameObject.AddComponent<EventTrigger>();

                EventTrigger.Entry entry_PointerEnter = new EventTrigger.Entry();
                entry_PointerEnter.eventID = EventTriggerType.PointerEnter;
                entry_PointerEnter.callback.AddListener((data) =>
                {
                    OnPointerEnterEvent((PointerEventData)data, idx);
                });
                eventTrigger.triggers.Add(entry_PointerEnter);

                EventTrigger.Entry entry_PointerExit = new EventTrigger.Entry();
                entry_PointerExit.eventID = EventTriggerType.PointerExit;
                entry_PointerExit.callback.AddListener((data) =>
                {
                    OnPointerExitEvent((PointerEventData)data, idx);
                });
                eventTrigger.triggers.Add(entry_PointerExit);
            });
        }
    }

    IEnumerator TitleEffCo()
    {
        Shield.gameObject.SetActive(false);
        Knife1.gameObject.SetActive(false);
        Knife2.gameObject.SetActive(false);
        CheckKnife.gameObject.SetActive(false);
        for (int i = 0; i < MenuTrans.childCount; i++)
        {
            MenuTrans.GetChild(i).gameObject.SetActive(false);
        }

        yield return new WaitForSeconds(2f);

        TitleEff();
    }

    void OnPointerEnterEvent(PointerEventData data, int _idx)
    {
        CheckKnife.gameObject.SetActive(true);
        CheckKnife.localPosition = MenuTrans.GetChild(_idx).localPosition + new Vector3(900, 23);
        CheckKnife.DOKill();
        CheckKnife.DOLocalMoveX(MenuTrans.GetChild(_idx).localPosition.x + 40, 0.5f).SetEase(Ease.OutBack);
    }

    void OnPointerExitEvent(PointerEventData data, int _idx)
    {
        CheckKnife.gameObject.SetActive(false);
    }

}
