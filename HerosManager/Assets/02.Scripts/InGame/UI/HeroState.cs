using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HeroState : MonoBehaviour
{
    public Image ProfileImg;
    public Image BraveryFill;

    public Text NameText;
    public TextMeshProUGUI StateText;
    public Button button;

    RectTransform rectTrans;
    bool isSlide;
    float slideLength;
    Vector2 slideVec;

    void Start()
    {
        Setup();
    }

    void Setup()
    {
        rectTrans = this.GetComponent<RectTransform>();
        isSlide = false;
        slideLength = 200f * Screen.width / 1920f;
        slideVec = rectTrans.position;

        button.onClick.AddListener(() => { Slide(); });
    }

    public void SetState(HeroInfo _heroInfo)
    {
        //BraveryFill.fillAmount = _heroInfo.bravery / 100f;

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
            slideVec = new Vector2(slideVec.x + slideLength, slideVec.y);
        else
            slideVec = new Vector2(slideVec.x - slideLength, slideVec.y);

        StopCoroutine("SlideCo");
        StartCoroutine("SlideCo");

        isSlide = !isSlide;
    }

    IEnumerator SlideCo()
    {
        while (Vector2.Distance(rectTrans.position, slideVec) >= 0.1f)
        {
            rectTrans.position = Vector2.Lerp(rectTrans.position, slideVec, Time.deltaTime * 3f);

            yield return null;
        }
    }
}

public class HeroInfo
{
    public float hp;
    public float power;
    public float stress;
    public float bravery;

    public HeroInfo(float _hp, float _power, float _stress, float _bravery)
    {
        this.hp = _hp;
        this.power = _power;
        this.stress = _stress;
        this.bravery = _bravery;
    }
}