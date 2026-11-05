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
    private Vector3 initialLocalPosition;
    private float initialGravity;
    private bool initialized;

    // Start is called before the first frame update
    void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        if (initialized) return;
        Mgr = MiniGameReferences.Manager<CaveManager>(this, manager);
        manager = Mgr.gameObject;
        rigid = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        initialLocalPosition = transform.localPosition;
        initialGravity = rigid.gravityScale;
        objectType = 5;
        moveType = 1;
        initialized = true;
    }

    public override void Action1() //Jump
    {
        //Debug.Log("Jump");
        if(!isCheck && !isStun)
        {
            Action3();
            isCheck = true;
            audioSource.clip = jumpSound;
            audioSource.Play();
            rigid.linearVelocity = new Vector3(0, Mgr.realCharJump * Mgr.heightScale, 0);
        }
    }

    public override void Action2() //Slide
    {
        if(!isCheck && !isStun)
        {
            audioSource.clip = slidingSound;
            audioSource.Play();
            RunStat.SetActive(false);
            SlideStat.SetActive(true);
        }
    }

    public override void Action3() //EndSlide
    {
        if (isStun) return;
        SlideStat.SetActive(false);
        RunStat.SetActive(true);
    }

    public void Reset()
    {
        Initialize();
        StopAllCoroutines();
        CancelInvoke();
        audioSource.Stop();
        rigid.linearVelocity = Vector2.zero;
        rigid.angularVelocity = 0;
        transform.localPosition = initialLocalPosition;
        rigid.position = transform.position;
        rigid.gravityScale = initialGravity * Mgr.heightScale;
        isCheck = isStun = false;
        isW = isA = isS = isD = false;
        RunStat.SetActive(true);
        SlideStat.SetActive(false);
        StunStat.SetActive(false);
    }

    public override IEnumerator Stun()
    {
        if (isStun) yield break;
        RunStat.SetActive(false);
        SlideStat.SetActive(false);
        
        StunStat.SetActive(true);
        isStun = true;
        Mgr.isCheck = false;
        yield return new WaitForSeconds(MiniGameRules.PositiveDelay(Mgr.stunTime));

        StunStat.SetActive(false);
        RunStat.SetActive(true);
        isStun = false;
        Mgr.isCheck = true;
        yield return null;
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
        if (!coll.gameObject.activeSelf || !isActiveAndEnabled) return;
        if(coll.CompareTag("MiniGameObj2")) //block
        {
            coll.gameObject.SetActive(false);
            audioSource.clip = blockSound;
            audioSource.Play();
            StartCoroutine("Stun");
            Destroy(coll.gameObject);
        }

        if(coll.CompareTag("MiniGameObj3")) //jem
        {
            if (Mgr.CollectJem(coll.gameObject))
            {
                audioSource.clip = jemSound;
                audioSource.Play();
            }
        }
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        if (!initialized) return;
        audioSource.Stop();
        rigid.linearVelocity = Vector2.zero;
        rigid.angularVelocity = 0;
    }
}
