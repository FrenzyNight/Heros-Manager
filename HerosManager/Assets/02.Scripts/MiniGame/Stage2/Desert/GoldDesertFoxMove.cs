using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldDesertFoxMove : MonoBehaviour
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
    private float goldFoxHP;

    private float realGoldFoxSpeed;
    // Start is called before the first frame update
    void Start()
    {
        HM = GameObject.Find("Hunt2Manager").GetComponent<Hunt2Manager>();
        rt = gameObject.GetComponent<RectTransform>();
        ani = GetComponent<Animator>();
        isIdle = false;
        goldFoxHP = HM.huntGoldFoxHP;
        realGoldFoxSpeed = HM.realGoldFoxSpeed;
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
        
        x = realGoldFoxSpeed * Mathf.Cos(direction * Mathf.Deg2Rad);
        y = realGoldFoxSpeed * Mathf.Sin(direction * Mathf.Deg2Rad);


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
        realGoldFoxSpeed = realGoldFoxSpeed * HM.huntFoxAngrySpeed;
        SetDirection();
    }

    void Death()
    {
        // 황금사막여우
        HM.KillGoldFox();
        //HM. 만큼 고기 획득
        Debug.Log("Get Meat " + HM.huntGoldFoxRes.ToString());
        HM.goldFoxNum -= 1;
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
            
        
            goldFoxHP -= 1;

            if(goldFoxHP <= 0)
            {
                Death();
            }
            else if(goldFoxHP <= 1)
            {
                Angry();
            }
        }
    }
}
