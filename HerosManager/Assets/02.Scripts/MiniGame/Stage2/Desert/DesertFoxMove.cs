﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DesertFoxMove : MonoBehaviour
{
    private Hunt2Manager HM;
    private RectTransform rt;
    private Animator ani;
    public GameObject AngryEffect;
    private int direction;
    private float x, y;
    private bool isIdle;
    private float moveTime;
    private float moveCheck;
    private float foxHP;

    private float realFoxSpeed;
    // Start is called before the first frame update
    void Start()
    {
        HM = GameObject.Find("Hunt2Manager").GetComponent<Hunt2Manager>();
        rt = gameObject.GetComponent<RectTransform>();
        ani = GetComponent<Animator>();
        isIdle = false;
        foxHP = HM.huntFoxHP;
        realFoxSpeed = HM.realFoxSpeed;
        SetMoveTime();
        SetDirection();
    }

    // Update is called once per frame
    void Update()
    {

        if(!isIdle && moveCheck >= moveTime)
        {
            StartCoroutine(SetIdle());
        }
    }

    void FixedUpdate()
    {
        if(!isIdle)
        {
            rt.anchoredPosition = new Vector2(rt.anchoredPosition.x + (x * Time.deltaTime) , rt.anchoredPosition.y + (y * Time.deltaTime));
            moveCheck += Time.deltaTime;
        }
    }

    IEnumerator SetIdle()
    {
        isIdle = true;
        ani.SetBool("isIdle", true);
        yield return new WaitForSeconds(HM.huntMonsterIdleTime);
        SetDirection();
        SetMoveTime();
        isIdle = false;
        ani.SetBool("isIdle", false);

    }

    void SetMoveTime()
    {
        moveCheck = 0;
        moveTime = Random.Range(HM.huntMonsterMinMoveTime,HM.huntMonsterMaxMoveTime);
    }

    void SetDirection()
    {
        direction = Random.Range(0,360);
        
        x = realFoxSpeed * Mathf.Cos(direction * Mathf.Deg2Rad);
        y = realFoxSpeed * Mathf.Sin(direction * Mathf.Deg2Rad);


        if(x >= 0)
        {
            transform.localScale = new Vector3(1,1,1);
        }
        else
        {
            transform.localScale = new Vector3(-1,1,1);
        }
        
    }

    void Angry()
    {
        AngryEffect.SetActive(true);
        realFoxSpeed = realFoxSpeed * HM.huntFoxAngrySpeed;
        SetDirection();
    }

    void Death()
    {
        // 사막여우
        HM.KillFox();
        //HM. 만큼 고기 획득
        Debug.Log("Get Meat " + HM.huntFoxRes.ToString());
        HM.foxNum -= 1;
        Destroy(gameObject);
    }  

    void OnCollisionEnter2D(Collision2D coll)
    {
        if(coll.gameObject.CompareTag("Block") || coll.gameObject.CompareTag("Player") || coll.gameObject.CompareTag("Monster"))
        {
            SetDirection();
        }
    }

    void OnTriggerEnter2D(Collider2D coll)
    {
        if(coll.gameObject.tag == "Arrow")
        {
            Destroy(coll.gameObject);
            
        
            foxHP -= 1;

            if(foxHP <= 0)
            {
                Death();
            }
            else if(foxHP <= 1)
            {
                Angry();
            }
        }
    }
}
