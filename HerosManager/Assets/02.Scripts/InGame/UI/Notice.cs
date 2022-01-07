using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Notice : MonoBehaviour
{
    public TextMeshProUGUI NoticeText;
    public Transform Content;

    public Button NoticeBtn;

    RectTransform rectTrans;
    bool isSlide;
    Vector2 openSz;
    Vector2 closeSz;
    Vector2 szVec;

    //temp
    public string tempStr = "";

    void Awake()
    {
        NoticeBtn.onClick.AddListener(() => Slide());
    }

    void Start()
    {
        Setup();
    }

    void Setup()
    {
        rectTrans = this.GetComponent<RectTransform>();
        isSlide = false;
        openSz = new Vector2(582f, 264f);
        closeSz = new Vector2(582f, 88f);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            InstNoticeText(tempStr);
        }
    }

    void InstNoticeText(string _msg)
    {
        TextMeshProUGUI text = Instantiate(NoticeText, Content);
        text.text = _msg;
    }

    void Slide()
    {
        if (isSlide)
            szVec = closeSz;
        else
            szVec = openSz;

        StopCoroutine("SlideCo");
        StartCoroutine("SlideCo");

        isSlide = !isSlide;
    }

    IEnumerator SlideCo()
    {
        while (Vector2.Distance(rectTrans.sizeDelta, szVec) >= 0.1f)
        {
            rectTrans.sizeDelta = Vector2.Lerp(rectTrans.sizeDelta, szVec, Time.deltaTime * 3f);
            rectTrans.anchoredPosition = new Vector2(rectTrans.anchoredPosition.x, rectTrans.sizeDelta.y / 2);

            yield return null;
        }
    }
}
