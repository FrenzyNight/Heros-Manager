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


    public override void Action1() //Set Direction
    {
        if(isW && !isA && !isS && !isD) // only w
        {
            direction = 90;
        }
        else if(!isW && isA && !isS && !isD) // only a
        {
            direction = 180;
        }
        else if(!isW && !isA && isS && !isD) // only a
        {
            direction = 270;
        }
        else if(!isW && !isA && !isS && isD) // only d
        {
            direction = 0;
        }

        else if(isW && !isA && !isS && isD) // w & d
        {
            direction = 45;
        }
        else if(isW && isA && !isS && !isD) // w & a
        {
            direction = 135;
        }
        else if(!isW && isA && isS && !isD) // a & s
        {
            direction = 225;
        }
        else if(!isW && !isA && isS && isD) // d & s
        {
            direction = 315;
        }

        x = Mgr.realHubCharSpeed * Mathf.Cos(direction * Mathf.Deg2Rad);
        y = Mgr.realHubCharSpeed * Mathf.Sin(direction * Mathf.Deg2Rad);

        if(x >= 0)
        {
            transform.localScale = new Vector3(1,1,1);
        }
        else
        {
            transform.localScale = new Vector3(-1,1,1);
        }

    }

    void OnCollisionEnter2D(Collision2D c)
    {
        if(c.gameObject.CompareTag("MiniGameObj1")) //block
        {
            isCheck = false;
        }
    }
}
