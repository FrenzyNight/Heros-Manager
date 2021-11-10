using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Amulet : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    AmuletPannel amuletPannel;

    bool isMouseOn;

    void Start()
    {
        amuletPannel = this.GetComponentInParent<AmuletPannel>();

        isMouseOn = false;
    }

    void Update()
    {
        if (isMouseOn)
            amuletPannel.AmuletNoticePosition();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isMouseOn = true;
        amuletPannel.SetAmuletNotice();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isMouseOn = false;
        amuletPannel.AmuletNotice.SetActive(isMouseOn);
    }
}
