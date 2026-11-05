using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HubCharMove : MiniGameCharMgr
{
    private HubManager Mgr;
    private RectTransform rt;
    private Animator ani;
    private float x, y;
    private int direction;

    // Start is called before the first frame update
    void Start()
    {
        Mgr = manager.GetComponent<HubManager>();
        rt = gameObject.GetComponent<RectTransform>();
        ani = gameObject.GetComponent<Animator>();

        objectType = 5;
        moveType = 2;
        //moveType = LoadGameData.Instance.miniGameDatas["game2Lay"].value1;
        
        isCheck = false;
        isW = false;
        isA = false;
        isS = false;
        isD = false;
    }

    public override void MoveAction()
    {
        if(isCheck)
        {
            rt.anchoredPosition = new Vector2(rt.anchoredPosition.x + (x * Time.deltaTime) , rt.anchoredPosition.y + (y * Time.deltaTime));
            ani.speed = 1.0f;
        }
        else
        {
            ani.speed = 0.0f;
        }
    }


    public override void Action1()
    {
        Vector2 velocity = MiniGameMovement.Direction(isW, isA, isS, isD) * Mgr.realHubCharSpeed;
        x = velocity.x;
        y = velocity.y;
        MiniGameMovement.FaceHorizontal(transform, x);
    }

    void OnCollisionEnter2D(Collision2D c)
    {
        if(c.gameObject.CompareTag("MiniGameObj1")) //block
        {
            isCheck = false;
        }
    }
}
