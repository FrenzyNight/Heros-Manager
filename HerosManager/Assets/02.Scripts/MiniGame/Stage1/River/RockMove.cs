using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockMove : MiniGameObjectMgr
{
    private WaterManager WM;
    private RectTransform rt;
    void Start()
    {
        WM = manager.GetComponent<WaterManager>();
        rt = gameObject.GetComponent<RectTransform>();
        Destroy(gameObject, 3f);

        objectType = 1;
    }

    public override void MoveAction()
    {
        rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, rt.anchoredPosition.y - (WM.realRockSpeed * Time.deltaTime));
    }
}
