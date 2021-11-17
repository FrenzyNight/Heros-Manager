using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmuletPannel : MonoBehaviour
{
    public GameObject AmuletNotice;
    Vector3 NoticePos;

    //temp
    public GameObject tempObj;

    void Start()
    {
        float width = AmuletNotice.GetComponent<RectTransform>().rect.width * Screen.width / 1920f / 2f;
        float height = AmuletNotice.GetComponent<RectTransform>().rect.height * Screen.height / 1080f / 2f;
        NoticePos = new Vector3(width, -height);
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
        AmuletNotice.transform.position = Input.mousePosition + NoticePos;
    }
}
