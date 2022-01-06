using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BearMove : MonoBehaviour
{
    private HuntManager HM;
    private RectTransform rt;
    private Animator ani;
    public GameObject AngryEffect;
    private int direction;
    private float x, y;
    private bool isIdle;
    private float moveTime;
    private float moveCheck;
    private float bearHP;

    private float realBearSpeed;
    // Start is called before the first frame update
    void Start()
    {
        HM = GameObject.Find("HuntManager").GetComponent<HuntManager>();
        rt = gameObject.GetComponent<RectTransform>();
        ani = GetComponent<Animator>();
        isIdle = false;
        bearHP = HM.huntBearHP;
        realBearSpeed = HM.standardHuntSpeed * HM.huntBearSpeed;
        SetMoveTime();
        SetDirection();
    }

    // Update is called once per frame
    void Update()
    {
        if(!isIdle)
        {
            //rb.MovePosition(new Vector3(x,y,0) * Time.deltaTime);
            rt.anchoredPosition = new Vector2(rt.anchoredPosition.x + (x * Time.deltaTime) , rt.anchoredPosition.y + (y * Time.deltaTime));
            //transform.Translate(x * Time.deltaTime,y * Time.deltaTime,0);
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
        
        x = realBearSpeed * Mathf.Cos(direction * Mathf.Deg2Rad);
        y = realBearSpeed * Mathf.Sin(direction * Mathf.Deg2Rad);


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
        realBearSpeed = realBearSpeed * HM.huntBearAngrySpeed;
        SetDirection();
    }

    void Death()
    {
        // 곰 사망
        //HM.huntBearRes 만큼 고기 획득
        Debug.Log("Get Meat " + HM.huntBearRes.ToString());
        HM.bearNum -= 1;
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
            
        
            bearHP -= 1;

            if(bearHP <= 0)
            {
                Death();
            }
            else if(bearHP <= 1)
            {
                Angry();
            }
        }
    }
}
