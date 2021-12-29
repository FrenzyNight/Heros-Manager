using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Hub : MonoBehaviour
{
    private GrasslandCSVReader GrassCSV;
    public GameObject GageImg;
    private GameObject Gage;
    private Camera hubcamera;

    private Transform tr;

    private float HubTime = 0;
    private float hubSpan = 0;

    private bool isTouch = false;
    
    void Start()
    {
        GrassCSV = GameObject.Find("GrasslandCSVReader").GetComponent<GrasslandCSVReader>();
        hubcamera = GameObject.Find("HubCamera").GetComponent<Camera>();
        tr = GetComponent<Transform>();
        Gage = Instantiate(GageImg, tr.position, Quaternion.identity, GameObject.Find("HubPanel").transform);
        Gage.transform.position = hubcamera.WorldToScreenPoint(tr.position);
        Gage.GetComponent<Image>().fillAmount = 0;
        
    }

    void Update()
    {
        Gage.GetComponent<Image>().fillAmount = HubTime / 2f;
        if(isTouch)
        {
            HubTime += Time.deltaTime;
        }
        else if(!isTouch)
        {
            hubSpan += Time.deltaTime;
        }

        if(HubTime >= GrassCSV.hubGetTime)
        {

            Debug.Log("Get Hub");
            //허브 획득
            Destroy(Gage);
            Destroy(gameObject);
            
        }

        if(hubSpan >= GrassCSV.hubSpanTime) //허브사라짐
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
            //Debug.Log("OnTrigger Test");
            //
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            isTouch = false;
            //Debug.Log("OnTrigger Test");
        }
    }
}
