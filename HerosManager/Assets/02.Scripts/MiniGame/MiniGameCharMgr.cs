using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class MiniGameCharMgr : MiniGameObjectMgr
{
    [Header("MiniGameCharMgr")]
    public int moveType;
    public bool isStun;
    public bool isCheck;
    public bool isW, isA, isS, isD;
    public Vector2 target, mouse;
    public float angle;

    void Start()
    {
        objectType = 5;
    }

    public override void UpdateAction()
    {
        if (isStun || Time.timeScale <= 0f)
            return;
        switch(moveType){
            case 1:
                MoveInput1();
                break;
            case 2:
                MoveInput2();
                break;
            case 3: 
                MoveInput3();
                break;
            case 4:
                MoveInput4();
                break;
        }
    }

    public void MoveInput1()
    {
        if(!isStun)
        {
            if(Input.GetKeyDown(KeyCode.UpArrow))
            {
                Action1();
            }

            if(Input.GetKeyDown(KeyCode.DownArrow))
            {
                Action2();
            }

            if(Input.GetKeyUp(KeyCode.DownArrow))
            {
                Action3();
            }
        }
    }

    public void MoveInput2()
    {
        bool up = Input.GetKey(KeyCode.W);
        bool left = Input.GetKey(KeyCode.A);
        bool down = Input.GetKey(KeyCode.S);
        bool right = Input.GetKey(KeyCode.D);
        bool changed = up != isW || left != isA || down != isS || right != isD;
        isW = up;
        isA = left;
        isS = down;
        isD = right;
        isCheck = MiniGameMovement.Direction(up, left, down, right) != Vector2.zero;
        if (changed)
            Action1();
    }

    public void MoveInput3()
    {
        mouse = Input.mousePosition;
        angle = Mathf.Atan2(mouse.y - target.y, mouse.x - target.x) * Mathf.Rad2Deg;
        
        this.transform.rotation = Quaternion.AngleAxis(angle + 90, Vector3.forward);
        if(Input.GetMouseButtonDown(0) && !isCheck)
        {
            Action1();
            //StartCoroutine("ShootArrow");
        }
    }

    public void MoveInput4()
    {
        if(Input.GetKeyDown(KeyCode.LeftArrow))
        {
            Action1(); //left
        }

        if(Input.GetKeyDown(KeyCode.RightArrow))
        {
            Action2(); //right
        }
    }

    public virtual void Action1() {}

    public virtual void Action2() {}

    public virtual void Action3() {}

    public virtual IEnumerator Stun()
    {
        yield return null;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        isStun = false;
        isCheck = false;
        isW = isA = isS = isD = false;
    }
}
