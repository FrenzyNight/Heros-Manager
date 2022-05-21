using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Hub : MiniGameObjectMgr
{
    AudioSource audioSource;
    public bool isGold;
    private HubManager Mgr;
    public GameObject GageImg;
    private GameObject Gage;

    private Transform tr;

    private float HubTime = 0;
    private float hubSpan = 0;

    private bool isTouch = false;
    
    void Start()
    {
        Mgr = manager.GetComponent<HubManager>();
        tr = GetComponent<Transform>();
        Gage = Instantiate(GageImg, tr.position, Quaternion.identity, Mgr.mother.transform);
        audioSource = gameObject.GetComponent<AudioSource>();
        Mgr.spawnObjects.Add(Gage);
        Gage.GetComponent<Image>().fillAmount = 0;
        Gage.transform.SetSiblingIndex(1);
        gameObject.transform.SetSiblingIndex(1);
        objectType = 2;
    }

    public override void UpdateAction()
    {
        Gage.GetComponent<Image>().fillAmount = HubTime / Mgr.hubGetTime;
        if(isTouch)
        {
            HubTime += Time.deltaTime;
        }
        else if(!isTouch)
        {
            hubSpan += Time.deltaTime;
        }

        if(HubTime >= Mgr.hubGetTime)
        {

            Debug.Log("Get Hub");
            //허브 획득
            if(isGold)
                Mgr.GetGoldHub();
            else
                Mgr.GetHub();
            Destroy(Gage);
            Destroy(gameObject);
            
        }

        if(hubSpan >= Mgr.hubSpanTime) //허브사라짐
        {
            Destroy(Gage);
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            audioSource.Play();
        }
    }


    void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            isTouch = true;
            hubSpan = 0;
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            audioSource.Stop();
            isTouch = false;
        }
    }
}
