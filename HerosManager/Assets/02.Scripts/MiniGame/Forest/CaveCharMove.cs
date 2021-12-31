using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaveCharMove : MonoBehaviour
{
    private CaveManager CM;
    public GameObject RunStat;
    public GameObject SlideStat;
    public GameObject StunStat;
    //public float jump;
    //public float jump2;

    private bool isJump;

    private Rigidbody2D rigid;

    // Start is called before the first frame update
    void Start()
    {
        CM = GameObject.Find("CaveManager").GetComponent<CaveManager>();
        rigid = gameObject.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if(CM.isRun)
        {
            if(Input.GetKeyDown(KeyCode.UpArrow))
            {
                
                Jump();
            }

            if(Input.GetKeyDown(KeyCode.DownArrow))
            {
                Slide();
            }

            if(Input.GetKeyUp(KeyCode.DownArrow))
            {
                EndSlide();
            }
        }
    }

    private void Jump()
    {
        if(!isJump) // 바닥에서 1단점프
        {
            
            rigid.velocity = new Vector3(0, CM.realCharJump, 0);
            isJump = true;
            EndSlide();
        }

        /*  2단점프 삭제
        else if(jumpCount == 1) // 공중에서 2단점프
        {
            rigid.velocity = new Vector3(0,jump2,0);
            jumpCount += 1;
        }
        */
    }

    private void Slide()
    {
        if(!isJump)
        {
            RunStat.SetActive(false);
            SlideStat.SetActive(true);
        }
    }

    private void EndSlide()
    {
        SlideStat.SetActive(false);
        RunStat.SetActive(true);
    }

    IEnumerator Stun()
    {
        RunStat.SetActive(false);
        SlideStat.SetActive(false);
        StunStat.SetActive(true);
        CM.isRun = false;
        yield return new WaitForSeconds(CM.stunTime);

        StunStat.SetActive(false);
        RunStat.SetActive(true);
        CM.isRun = true;
        yield return null;
    }

    void Jem()
    {
        //보석획득
        Debug.Log("Get Jem");
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Land"))
        {
            isJump = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D coll)
    {
        if(coll.gameObject.tag == "Block")
        {
            StartCoroutine("Stun");
            Destroy(coll.gameObject);
        }

        if(coll.gameObject.tag == "Jem")
        {
            Jem();
            Destroy(coll.gameObject);
        }
    }
}
