using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogMove : MiniGameObjectMgr
{
    private FishManager FM;
    private RectTransform rt;
    public float direction;

    //public Vector2 limitUp, limitDown;
    // Start is called before the first frame update
    void Start()
    {
        FM = manager.GetComponent<FishManager>();
        rt = gameObject.GetComponent<RectTransform>();
        direction = 1;

        objectType = 3;
    }

    public override void MoveAction()
    {
        rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, rt.anchoredPosition.y + (FM.realLogSpeed * direction * Time.deltaTime));
    }

    public override void UpdateAction()
    {
        if(rt.anchoredPosition.y >= FM.topP)
        {
            direction = -1;
        }
        else if(rt.anchoredPosition.y <= FM.botP)
        {
            direction = 1;
        }
    }
}
