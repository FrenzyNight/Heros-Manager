﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FairyCharMove : MonoBehaviour
{
    private FairyWarehouseManager FM;
    // Start is called before the first frame update
    private RectTransform rt;
    private float x, y;
    private int direction;
    private bool isMove;
    private bool isW, isA, isS, isD;

    // Start is called before the first frame update
    void Start()
    {
        FM = GameObject.Find("FairyWarehouseManager").GetComponent<FairyWarehouseManager>();
        rt = gameObject.GetComponent<RectTransform>();
        
        isMove = false;
        isW = false;
        isA = false;
        isS = false;
        isD = false;
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
        if(isMove)
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


        x = FM.fwLayRealSpeed * Mathf.Cos(direction * Mathf.Deg2Rad);
        y = FM.fwLayRealSpeed * Mathf.Sin(direction * Mathf.Deg2Rad);


        if(x >= 0)
        {
            transform.localScale = new Vector3(1,1,1);
        }
        else
        {
            transform.localScale = new Vector3(-1,1,1);
        }

    }
}
