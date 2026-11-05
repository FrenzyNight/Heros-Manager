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
    private float previousAimX;
    private Vector3 lastMousePosition;
    private bool stickAiming;
    private bool gamepadSliding;

    void Start()
    {
        objectType = 5;
    }

    public override void UpdateAction()
    {
        if (gamepadSliding && (!GamepadInput.Held(GamepadButton.X) || !GamepadUiController.GameplayInputAllowed))
        {
            Action3();
            gamepadSliding = false;
        }
        if (isStun || Time.timeScale <= 0f || !GamepadUiController.GameplayInputAllowed)
        {
            if (moveType == 2)
            {
                isCheck = false;
                isW = isA = isS = isD = false;
            }
            return;
        }
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
            if(Input.GetKeyDown(KeyCode.UpArrow) || GamepadInput.Pressed(GamepadButton.Confirm))
            {
                Action1();
            }

            if(Input.GetKeyDown(KeyCode.DownArrow) || GamepadInput.Pressed(GamepadButton.X))
            {
                gamepadSliding = GamepadInput.Held(GamepadButton.X);
                Action2();
            }

            if(Input.GetKeyUp(KeyCode.DownArrow) || GamepadInput.Released(GamepadButton.X))
            {
                Action3();
            }
        }
    }

    public void MoveInput2()
    {
        bool up = Input.GetKey(KeyCode.W) || GamepadInput.Move.y > 0.3f;
        bool left = Input.GetKey(KeyCode.A) || GamepadInput.Move.x < -0.3f;
        bool down = Input.GetKey(KeyCode.S) || GamepadInput.Move.y < -0.3f;
        bool right = Input.GetKey(KeyCode.D) || GamepadInput.Move.x > 0.3f;
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
        target = transform.position;
        mouse = Input.mousePosition;
        if (GamepadInput.Aim.sqrMagnitude > 0.05f)
        {
            stickAiming = true;
            angle = Mathf.Atan2(GamepadInput.Aim.y, GamepadInput.Aim.x) * Mathf.Rad2Deg;
        }
        else if ((Input.mousePosition - lastMousePosition).sqrMagnitude > 1f)
            stickAiming = false;
        if (!stickAiming)
            angle = Mathf.Atan2(mouse.y - target.y, mouse.x - target.x) * Mathf.Rad2Deg;
        lastMousePosition = Input.mousePosition;
        
        this.transform.rotation = Quaternion.AngleAxis(angle + 90, Vector3.forward);
        if((Input.GetMouseButtonDown(0) || GamepadInput.Pressed(GamepadButton.RightTrigger)) && !isCheck)
        {
            Action1();
            //StartCoroutine("ShootArrow");
        }
    }

    public void MoveInput4()
    {
        if(Input.GetKeyDown(KeyCode.LeftArrow) || GamepadInput.Pressed(GamepadButton.Left) || (GamepadInput.Aim.x < -0.5f && previousAimX >= -0.5f))
        {
            Action1(); //left
        }

        if(Input.GetKeyDown(KeyCode.RightArrow) || GamepadInput.Pressed(GamepadButton.Right) || (GamepadInput.Aim.x > 0.5f && previousAimX <= 0.5f))
        {
            Action2(); //right
        }
        previousAimX = GamepadInput.Aim.x;
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
        previousAimX = 0f;
        stickAiming = false;
        gamepadSliding = false;
    }
}
