using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowMove : MiniGameObjectMgr
{
    private HuntManager Mgr;
    private RectTransform rt;

    // Start is called before the first frame update
    void Start()
    {
        Mgr = manager.GetComponent<HuntManager>();
        rt = gameObject.GetComponent<RectTransform>();
        Destroy(gameObject.transform.parent.gameObject, 1f);

        objectType = 1;
        //objectType = LoadGameData.Instance.miniGameDatas["game2Lay"].value1;
    }

    public override void MoveAction()
    {
        rt.anchoredPosition = new Vector2(rt.anchoredPosition.x + (Mgr.realArrowSpeed * Time.deltaTime) , rt.anchoredPosition.y);
    }
}
