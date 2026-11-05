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
        if (Time.timeScale <= 0f)
            return;
        isW = Input.GetKey(KeyCode.W);
        isA = Input.GetKey(KeyCode.A);
        isS = Input.GetKey(KeyCode.S);
        isD = Input.GetKey(KeyCode.D);
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
            Debug.Log("Stun");
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
