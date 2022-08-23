using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaveCharMove : MiniGameCharMgr
{
    [Header("For Cave")]
    AudioSource audioSource;
    public AudioClip slidingSound, jumpSound, blockSound, jemSound;
    private CaveManager Mgr;
    public GameObject RunStat;
    public GameObject SlideStat;
    public GameObject StunStat;

    private Rigidbody2D rigid;

    // Start is called before the first frame update
    void Start()
    {
        Mgr = manager.GetComponent<CaveManager>();
        rigid = gameObject.GetComponent<Rigidbody2D>();
        audioSource = gameObject.GetComponent<AudioSource>();
        rigid.gravityScale = rigid.gravityScale * Mgr.heightScale;
        isCheck = false;

        objectType = 5;
        moveType = 1;
        //moveType = LoadGameData.Instance.miniGameDatas["game1Lay"].value1;
        //
    }

    public override void Action1() //Jump
    {
        //Debug.Log("Jump");
        if(!isCheck)
        {
            Action3();
            //isCheck = true;
            audioSource.clip = jumpSound;
            audioSource.Play();
            rigid.velocity = new Vector3(0, Mgr.realCharJump * Mgr.heightScale, 0);
        }
    }

    public override void Action2() //Slide
    {
        if(!isCheck)
        {
            audioSource.clip = slidingSound;
            audioSource.Play();
            RunStat.SetActive(false);
            SlideStat.SetActive(true);
        }
    }

    public override void Action3() //EndSlide
    {
        SlideStat.SetActive(false);
        RunStat.SetActive(true);
    }

    public void Reset()
    {
        RunStat.SetActive(true);
        SlideStat.SetActive(false);
        StunStat.SetActive(false);
    }

    public override IEnumerator Stun()
    {
        RunStat.SetActive(false);
        SlideStat.SetActive(false);
        
        StunStat.SetActive(true);
        isStun = true;
        isCheck = false;
        Mgr.isCheck = false;
        yield return new WaitForSeconds(Mgr.stunTime);

        StunStat.SetActive(false);
        RunStat.SetActive(true);
        isCheck = true;
        isStun = false;
        Mgr.isCheck = true;
        yield return null;
    }

    void Jem()
    {
        //보석획득
        //Mgr.AddJem();
        Debug.Log("Get Jem");
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("MiniGameObj1") && isCheck) //land
        {
            isCheck = false;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("MiniGameObj1") && !isCheck) //land
        {
            isCheck = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D coll)
    {
        if(coll.CompareTag("MiniGameObj2")) //block
        {
            audioSource.clip = blockSound;
            audioSource.Play();
            StartCoroutine("Stun");
            Destroy(coll.gameObject);
        }

        if(coll.CompareTag("MiniGameObj3")) //jem
        {
            audioSource.clip = jemSound;
            audioSource.Play();
            Jem();
            Destroy(coll.gameObject);
        }
    }
}
