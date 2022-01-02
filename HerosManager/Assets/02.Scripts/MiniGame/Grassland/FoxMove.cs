﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoxMove : MonoBehaviour
{
    private HuntManager HM;
    private Rigidbody2D rb;
    private int direction;
    private float x, y;
    private bool isIdle;
    private float moveTime;
    private float moveCheck;
    private float foxHP;

    // Start is called before the first frame update
    void Start()
    {
        HM = GameObject.Find("HuntManager").GetComponent<HuntManager>();
        rb = GetComponent<Rigidbody2D>();
        isIdle = false;
        foxHP = HM.huntFoxHP;
        SetMoveTime();
        SetDirection();
        
    }

    // Update is called once per frame
    void Update()
    {
        if(!isIdle)
        {
            //rb.MovePosition(new Vector3(x,y,0) * Time.deltaTime);
            transform.Translate(x * Time.deltaTime,y * Time.deltaTime,0);
            moveCheck += Time.deltaTime;
        }

        if(!isIdle && moveCheck >= moveTime)
        {
            StartCoroutine(SetIdle());
        }
    }

    IEnumerator SetIdle()
    {
        isIdle = true;
        yield return new WaitForSeconds(HM.huntMonsterIdleTime);
        SetDirection();
        SetMoveTime();
        isIdle = false;

    }
    
    void SetMoveTime()
    {
        moveCheck = 0;
        moveTime = Random.Range(HM.huntMonsterMinMoveTime,HM.huntMonsterMaxMoveTime);
    }

    void SetDirection()
    {
        direction = Random.Range(0,360);
        
        x = HM.standardHuntSpeed * HM.huntFoxSpeed * Mathf.Cos(direction * Mathf.Deg2Rad);
        y = HM.standardHuntSpeed * HM.huntFoxSpeed * Mathf.Sin(direction * Mathf.Deg2Rad);


        if(x >= 0)
        {
            transform.localScale = new Vector3(1,1,1);
        }
        else
        {
            transform.localScale = new Vector3(-1,1,1);
        }

    }

    void Death()
    {
        // 여우 사망
        //HM.huntFoxRes 만큼 고기 획득
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
            //Debug.Log("Fox Arrow");
        
            foxHP -= 1;

            if(foxHP <= 0)
            {
                Death();
            }
        }
    }

}
