using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HeroState : MonoBehaviour
{
    public TextMeshProUGUI StateText;
    public Image BraveryFill;
    public Button button;

    RectTransform rectTrans;
    bool isSlide;
    Vector2 closeSlideVec;
    Vector2 openSlideVec;
    Vector2 slideVec;

    void Start()
    {
        Setup();
    }

    void Setup()
    {
        rectTrans = this.GetComponent<RectTransform>();
        isSlide = false;
        closeSlideVec = rectTrans.anchoredPosition;
        openSlideVec = new Vector2(-rectTrans.rect.width / 2, rectTrans.anchoredPosition.y);

        button.onClick.AddListener(() => { Slide(); });
    }

    public void SetState(HeroInfo _heroInfo)
    {
        BraveryFill.fillAmount = _heroInfo.bravery / 100f;

        string str = "스트레스:";
        if (_heroInfo.stress < 30f)
            str += "낮음";
        else if (_heroInfo.stress < 70f)
            str += "보통";
        else
            str += "분노";
        str += "\n전투력:" + _heroInfo.power + "\n체력:" + _heroInfo.hp + "/100";

        StateText.text = str;
    }

    void Slide()
    {
        if (isSlide)
            slideVec = closeSlideVec;
        else
            slideVec = openSlideVec;

        StopCoroutine("SlideCo");
        StartCoroutine("SlideCo");

        isSlide = !isSlide;
    }

    IEnumerator SlideCo()
    {
        while (Vector2.Distance(rectTrans.anchoredPosition, slideVec) >= 0.1f)
        {
            rectTrans.anchoredPosition = Vector2.Lerp(rectTrans.anchoredPosition, slideVec, Time.deltaTime * 3f);

            yield return null;
        }
    }
}