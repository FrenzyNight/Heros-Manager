using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeCharMove : MonoBehaviour
{
    private MazeManager MM;
    private RectTransform rt;

    private bool isMove;
    private float x, y;
    private int direction;
    private bool isW, isA, isS, isD;
    public Vector2 firstPoint;

    // Start is called before the first frame update
    void Start()
    {
        MM = GameObject.Find("MazeManager").GetComponent<MazeManager>();

        rt = gameObject.GetComponent<RectTransform>();
        
        firstPoint = rt.anchoredPosition;
        isMove = false;
        isW = false;
        isA = false;
        isS = false;
        isD = false;

    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.UpArrow))
        {
            isW = true;
            isMove = true;
        }
        if(Input.GetKey(KeyCode.LeftArrow))
        {
            isA = true;
            isMove = true;
        }
        if(Input.GetKey(KeyCode.DownArrow))
        {
            isS = true;
            isMove = true;
        }
        if(Input.GetKey(KeyCode.RightArrow))
        {
            isD = true;
            isMove = true;
        }

        if(Input.GetKeyDown(KeyCode.UpArrow))
        {
            SetDirection();
        }
        if(Input.GetKeyDown(KeyCode.LeftArrow))
        {
            SetDirection();
        }
        if(Input.GetKeyDown(KeyCode.DownArrow))
        {
            SetDirection();
        }
        if(Input.GetKeyDown(KeyCode.RightArrow))
        {
            SetDirection();
        }

        if(Input.GetKeyUp(KeyCode.UpArrow))
        {
            isW =false;
            isMove = false;
            SetDirection();
        }
        if(Input.GetKeyUp(KeyCode.LeftArrow))
        {
            isA = false;
            isMove = false;
            SetDirection();
        }
        if(Input.GetKeyUp(KeyCode.DownArrow))
        {
            isS = false;
            isMove = false;
            SetDirection();
        }
        if(Input.GetKeyUp(KeyCode.RightArrow))
        {
            isD = false;
            isMove = false;
            SetDirection();
        }
    }

    void FixedUpdate()
    {
        if(isMove)
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


        x = MM.realMazeCharSpeed * Mathf.Cos(direction * Mathf.Deg2Rad);
        y = MM.realMazeCharSpeed * Mathf.Sin(direction * Mathf.Deg2Rad);


        if(x >= 0)
        {
            transform.localScale = new Vector3(1,1,1);
        }
        else
        {
            transform.localScale = new Vector3(-1,1,1);
        }
    }

    void OnTriggerEnter2D(Collider2D c)
    {
        if(c.CompareTag("Treasure"))
        {
            Debug.Log("Get Treasure");
            GetTreasure();
        }
    }

    void GetTreasure()
    {
        rt.anchoredPosition = firstPoint;
        MM.ResetMaze();

    }
}
