using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeroState : MonoBehaviour
{
    Button button;
    GameObject statePannel;

    bool isSlide;
    float slideLength;
    Vector2 slideVec;

    void Start()
    {
        Setup();
    }

    void Setup()
    {
        button = this.transform.GetChild(0).GetComponent<Button>();
        statePannel = this.transform.GetChild(1).gameObject;

        isSlide = false;
        slideLength = statePannel.GetComponent<RectTransform>().rect.width * Screen.width / 1920f;
        slideVec = this.transform.position;

        button.onClick.AddListener(() => { Slide(); });
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
        while (Vector2.Distance(this.transform.position, slideVec) >= 0.1f)
        {
            this.transform.position = Vector2.Lerp(this.transform.position, slideVec, Time.deltaTime * 3f);

            yield return null;
        }
    }
}
