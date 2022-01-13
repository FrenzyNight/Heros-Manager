using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GoldHub : MonoBehaviour
{
    private HubManager HM;
    public GameObject GageImg;
    private GameObject Gage;

    private Transform tr;

    private float HubTime = 0;
    private float hubSpan = 0;

    private bool isTouch = false;
    
    void Start()
    {
        HM = GameObject.Find("HubManager").GetComponent<HubManager>();
        tr = GetComponent<Transform>();
        Gage = Instantiate(GageImg, tr.position, Quaternion.identity, GameObject.Find("Hub").transform);
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

            Debug.Log("Get GoldHub");
            //골드허브 획득
            HM.GetGoldHub();
            //HM.hubGoldRes;
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
            //Debug.Log("접촉");
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
