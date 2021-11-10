using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Notice : MonoBehaviour
{
    public Button NoticeBtn;
    public GameObject NoticePannel;

    bool isSlide;
    float slideLength;
    Vector2 slideVec;

    [Header("Scroll View")]
    public Text NoticeText;
    public Transform Content;

    //temp
    int idx = 0;

    private void Awake()
    {
        NoticeBtn.onClick.AddListener(() =>
        {
            Slide();
        });
    }

    void Start()
    {
        Setup();
    }

    void Setup()
    {
        isSlide = false;
        slideLength = NoticePannel.GetComponent<RectTransform>().rect.height * Screen.height / 1080f;
        slideVec = this.transform.position;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            InstantiateNoticeText();
        }
    }

    void Slide()
    {
        if (isSlide)
            slideVec = new Vector2(slideVec.x, slideVec.y - slideLength);
        else
            slideVec = new Vector2(slideVec.x, slideVec.y + slideLength);

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

    void InstantiateNoticeText()
    {
        Text text = Instantiate(NoticeText, Content);
        text.text = idx.ToString();
        idx++;
        //text.transform.SetSiblingIndex(0);
    }
}
