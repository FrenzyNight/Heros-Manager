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
        HM = GameObject.Find("Hub2Manager").GetComponent<Hub2Manager>();
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
        
        if(Input.GetKey(KeyCode.W))
        {
            isW = true;
            isMove = true;
        }
        if(Input.GetKey(KeyCode.A))
        {
            isA = true;
            isMove = true;
        }
        if(Input.GetKey(KeyCode.S))
        {
            isS = true;
            isMove = true;
        }
        if(Input.GetKey(KeyCode.D))
        {
            isD = true;
            isMove = true;
        }

        if(Input.GetKeyDown(KeyCode.W))
        {
        SetDirection();
        }
        if(Input.GetKeyDown(KeyCode.A))
        {
            SetDirection();
        }
        if(Input.GetKeyDown(KeyCode.S))
        {
            SetDirection();
        }
        if(Input.GetKeyDown(KeyCode.D))
        {
            SetDirection();
        }

        if(Input.GetKeyUp(KeyCode.W))
        {
            isW =false;
            isMove = false;
            SetDirection();
        }
        if(Input.GetKeyUp(KeyCode.A))
        {
            isA = false;
            isMove = false;
            SetDirection();
        }
        if(Input.GetKeyUp(KeyCode.S))
        {
            isS = false;
            isMove = false;
            SetDirection();
        }
        if(Input.GetKeyUp(KeyCode.D))
        {
            isD = false;
            isMove = false;
            SetDirection();
        }
        
    }

    void FixedUpdate()
    {
        if(isMove && !isStun)
        {
            rt.anchoredPosition = new Vector2(rt.anchoredPosition.x + (x * Time.deltaTime) , rt.anchoredPosition.y + (y * Time.deltaTime));
        }
    }


    void SetDirection()
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


        x = HM.realHubCharSpeed * Mathf.Cos(direction * Mathf.Deg2Rad);
        y = HM.realHubCharSpeed * Mathf.Sin(direction * Mathf.Deg2Rad);


        if(x >= 0)
        {
            transform.localScale = new Vector3(1,1,1);
        }
        else
        {
            transform.localScale = new Vector3(-1,1,1);
        }
    }

    public void StartStun()
    {
        StartCoroutine(Stun());
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
