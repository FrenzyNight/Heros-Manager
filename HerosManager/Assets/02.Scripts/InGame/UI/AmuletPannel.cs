using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmuletPannel : MonoBehaviour
{
    public GameObject AmuletNotice;
    RectTransform AmuletNoticeRect;

    //temp
    public GameObject tempObj;

    void Start()
    {
        AmuletNoticeRect = AmuletNotice.GetComponent<RectTransform>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            Instantiate(tempObj, this.transform);
        }
    }

    public void SetAmuletNotice()
    {
        //AmuletNotice.transform.GetChild(0).GetComponent<Text>().text = "";
        AmuletNotice.SetActive(true);
    }

    public void AmuletNoticePosition()
    {
        float width = AmuletNoticeRect.rect.width * Screen.width / 1920f / 2f;
        float height = AmuletNoticeRect.rect.height * Screen.height / 1080f / 2f;
        Vector3 vec = new Vector3(width, -height);

        AmuletNotice.transform.position = Input.mousePosition + vec;
    }
}
