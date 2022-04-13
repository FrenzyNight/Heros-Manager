using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Amulet : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    bool isMouseOn;

    void Start()
    {
        isMouseOn = false;
    }

    public void Setup()
    {

    }

    void Update()
    {
        if (isMouseOn)
            AmuletManager.Instance.AmuletNotice.transform.position = Input.mousePosition;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isMouseOn = true;

        AmuletManager.Instance.NoticeText.text = "";
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isMouseOn = false;
        AmuletManager.Instance.AmuletNotice.SetActive(isMouseOn);
    }
}
