using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockMove : MiniGameObjectMgr
{
    private CaveManager Mgr;
    private RectTransform rt;

    // Start is called before the first frame update
    void Start()
    {
        Mgr = manager.GetComponent<CaveManager>();
        rt = gameObject.GetComponent<RectTransform>();
        gameObject.transform.SetSiblingIndex(2);
        Destroy(gameObject, 5f);

        objectType = 1;
        //objectType = LoadGameData.Instance.miniGameDatas["game2Lay"].value1;
    }

    public override void MoveAction()
    {
        if(Mgr.isCheck)
        {
            rt.anchoredPosition = new Vector2(rt.anchoredPosition.x - (Mgr.realSpeed * Time.deltaTime) , rt.anchoredPosition.y);
        }
    }
}
