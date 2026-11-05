using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class WaterCharMove : MiniGameCharMgr
{
    [Header("Water Char")]
    AudioSource audioSource;
    private WaterManager WM;
    private RectTransform rt;
    private Bucket BK;

    private bool isCrash;
    private Image CharImg;

    public Sprite img1, img2;

    public float moveVar;

    [HideInInspector] public Coroutine coroutine;
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        WM = manager.GetComponent<WaterManager>();
        rt = gameObject.GetComponent<RectTransform>();
        BK = MiniGameReferences.Manager<Bucket>(this);
        CharImg = gameObject.GetComponent<Image>();
        isCrash = false;

        moveType = 4;
        objectType = 5;
    }


    public override void Action1()
    {
        if(rt.anchoredPosition.x != -moveVar)
        {
            rt.anchoredPosition = new Vector2(rt.anchoredPosition.x - (moveVar), rt.anchoredPosition.y);
        }
    }
    public override void Action2()
    {
        if(rt.anchoredPosition.x != moveVar)
        {
            rt.anchoredPosition = new Vector2(rt.anchoredPosition.x + (moveVar), rt.anchoredPosition.y);
        }
    }

    IEnumerator Crash()
    {
        isCrash = true;
        //CharImg.sprite = img2;
        BK.waterTime = 0;

        audioSource.Play();
        
        //gameObject.GetComponent<SpriteRenderer>().DOFade(0,1f);

        yield return new WaitForSeconds(WM.waterRockInvTime);

        isCrash = false;
        CharImg.sprite = img1;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        if (BK != null)
            BK.waterTime = 0;
        coroutine = null;
        isCrash = false;
        if (CharImg != null)
            CharImg.sprite = img1;
    }

    void OnTriggerEnter2D(Collider2D coll)
    {
        if(coll.gameObject.CompareTag("MiniGameObj1") && !isCrash)
        {
            Destroy(coll.gameObject);
            coroutine = StartCoroutine(Crash());
        }
    }
}
