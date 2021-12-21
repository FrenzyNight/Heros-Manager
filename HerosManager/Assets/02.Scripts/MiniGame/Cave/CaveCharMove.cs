using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaveCharMove : MonoBehaviour
{
    private CaveManager CM;
    public GameObject RunIdle;
    public GameObject SlideIdle;
    public GameObject StunIdle;
    public float jump;
    public float jump2;

    private int jumpCount = 0;

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
        if(jumpCount == 0) // 바닥에서 1단점프
        {
            EndSlide();
            rigid.velocity = new Vector3(0, jump, 0);
            jumpCount += 1;
        }

        else if(jumpCount == 1) // 공중에서 2단점프
        {
            rigid.velocity = new Vector3(0,jump2,0);
            jumpCount += 1;
        }
    }

    private void Slide()
    {
        if(jumpCount == 0)
        {
            RunIdle.SetActive(false);
            SlideIdle.SetActive(true);
        }
    }

    private void EndSlide()
    {
        SlideIdle.SetActive(false);
        RunIdle.SetActive(true);
    }

    IEnumerator Stun()
    {
        RunIdle.SetActive(false);
        SlideIdle.SetActive(false);
        StunIdle.SetActive(true);
        CM.isRun = false;
        yield return new WaitForSeconds(CM.realStunTime);

        StunIdle.SetActive(false);
        RunIdle.SetActive(true);
        CM.isRun = true;
        yield return null;
    }

    void Jem()
    {

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Land"))
        {
            jumpCount = 0;
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
