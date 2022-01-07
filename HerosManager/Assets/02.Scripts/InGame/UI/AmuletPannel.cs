using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AmuletPannel : MonoBehaviour
{
    public GameObject AmuletNotice;
    public Text NoticeText;

    //temp
    public GameObject tempObj;

    void Start()
    {

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
        //NoticeText.text = "";
        AmuletNotice.SetActive(true);
    }

    public void AmuletNoticePosition()
    {
        AmuletNotice.transform.position = Input.mousePosition;
    }
}
