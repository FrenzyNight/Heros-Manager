using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Hub2 : MonoBehaviour
{
    private Hub2Manager HM;
    public GameObject GageImg;
    private GameObject Gage;

    private Transform tr;

    private float HubTime = 0;
    private float hubSpan = 0;

    private bool isTouch = false;
    
    void Start()
    {
        HM = GameObject.Find("Hub2Manager").GetComponent<Hub2Manager>();
        tr = GetComponent<Transform>();
        Gage = Instantiate(GageImg, tr.position, Quaternion.identity, GameObject.Find("Hub2").transform);
        Gage.GetComponent<Image>().fillAmount = 0;
        
    }

    void Update()
    {
        Gage.GetComponent<Image>().fillAmount = HubTime / HM.hubGetTime;
        if(isTouch)
        {
            HubTime += Time.deltaTime;
        }
        else if(!isTouch)
        {
            hubSpan += Time.deltaTime;
        }

        if(HubTime >= HM.hubGetTime)
        {

            Debug.Log("Get Hub");
            //허브 획득
            HM.GetHub();
            //HM.hubNormalRes
            Destroy(Gage);
            Destroy(gameObject);
            
        }

        if(hubSpan >= HM.hubSpanTime) //허브사라짐
        {
            Destroy(Gage);
            Destroy(gameObject);
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
            isTouch = false;
        }
    }
}
