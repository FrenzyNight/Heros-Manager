using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hub2CharMove : MonoBehaviour
{
    private Hub2Manager HM;
    
    private RectTransform rt;
    private float x, y;
    private int direction;
    private bool isMove;
    private bool isW, isA, isS, isD;
    private bool isInv, isStun;
    private Vector2 initialPosition;

    void Awake()
    {
        rt = GetComponent<RectTransform>();
        initialPosition = rt.anchoredPosition;
    }

    void OnEnable()
    {
        rt.anchoredPosition = initialPosition;
        isMove = isInv = isStun = false;
        x = y = 0f;
    }

    // Start is called before the first frame update
    void Start()
    {
        HM = MiniGameReferences.Manager<Hub2Manager>(this);
        rt = gameObject.GetComponent<RectTransform>();
        
        isMove = false;
        isW = false;
        isA = false;
        isS = false;
        isD = false;

        isInv = false;
        isStun = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!GamepadUiController.MiniGameInputAllowed)
        {
            isMove = false;
            return;
        }
        isW = Input.GetKey(KeyCode.W) || GamepadInput.Held(GamepadButton.Up);
        isA = Input.GetKey(KeyCode.A) || GamepadInput.Held(GamepadButton.Left);
        isS = Input.GetKey(KeyCode.S) || GamepadInput.Held(GamepadButton.Down);
        isD = Input.GetKey(KeyCode.D) || GamepadInput.Held(GamepadButton.Right);
        isMove = MiniGameMovement.Direction(isW, isA, isS, isD) != Vector2.zero;
        SetDirection();
    }

    void FixedUpdate()
    {
        if(isMove && !isStun)
        {
            rt.anchoredPosition = new Vector2(rt.anchoredPosition.x + (x * Time.deltaTime) , rt.anchoredPosition.y + (y * Time.deltaTime));
            gameObject.GetComponent<Animator>().speed = 1.0f;
        }
        else
        {
            gameObject.GetComponent<Animator>().speed = 0.0f;
        }
    }


    void SetDirection()
    {
        Vector2 velocity = MiniGameMovement.Direction(isW, isA, isS, isD) * HM.realHubCharSpeed;
        x = velocity.x;
        y = velocity.y;
        MiniGameMovement.FaceHorizontal(transform, x);
    }

    public void StartStun()
    {
        if (isInv || isStun)
            return;
        StartCoroutine(Stun());
    }

    void OnDisable()
    {
        StopAllCoroutines();
        isMove = false;
        isInv = false;
        isStun = false;
        isW = isA = isS = isD = false;
    }

    IEnumerator Stun()
    {
        if(!isInv)
        {
            //스턴 
            isStun = true;
            GamepadInput.Pulse(0.22f, 0.08f, 0.14f);
            isW = false;
            isA = false;
            isS = false; 
            isD = false;
            isMove = false;
            yield return new WaitForSeconds(HM.hubCactusStun);
            isStun = false;
            //isMove = true;

            //무적
            isInv = true;
            yield return new WaitForSeconds(HM.hubCactusInv);
            isInv = false;
        }
    }

    void OnCollisionEnter(Collision c)
    {
        if(c.gameObject.CompareTag("Block"))
        {
            isMove = false;
        }
    }
}
